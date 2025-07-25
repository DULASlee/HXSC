using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 权限管理服务实现
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            ApplicationDbContext context,
            ILogger<PermissionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        public async Task<ApiResponse<List<PermissionTreeDto>>> GetPermissionTreeAsync(Guid? tenantId = null)
        {
            try
            {
                var query = _context.Permissions.AsQueryable();
                
                if (tenantId.HasValue)
                {
                    query = query.Where(p => p.TenantId == tenantId.Value);
                }
                
                var permissions = await query
                    .Where(p => p.Status == 1)
                    .OrderBy(p => p.Sort)
                    .ToListAsync();

                var permissionDtos = permissions.Select(p => new PermissionTreeDto
                {
                    Id = p.Id.ToString(),
                    ParentId = p.ParentId?.ToString(),
                    Code = p.Code,
                    Name = p.Name,
                    Description = p.Description,
                    Type = p.Type,
                    Level = p.Level,
                    Sort = p.Sort,
                    Status = p.Status,
                    IsSystem = p.IsSystem,
                    Icon = p.Icon
                }).ToList();

                var tree = BuildPermissionTree(permissionDtos, null);
                return ApiResponse<List<PermissionTreeDto>>.Success(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取权限树失败");
                return ApiResponse<List<PermissionTreeDto>>.Failure("获取权限树失败");
            }
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        public async Task<ApiResponse<bool>> CheckPermissionAsync(Guid userId, string permissionCode, Dictionary<string, object>? context = null)
        {
            try
            {
                // 检查是否有超级权限
                var hasSuperPermission = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.Status == 1)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => rp.Status == 1 && rp.Permission.Code == "*")
                    .AnyAsync();

                if (hasSuperPermission)
                {
                    return ApiResponse<bool>.Success(true);
                }

                // 检查角色权限
                var hasRolePermission = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.Status == 1)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => rp.Status == 1 && rp.Permission.Status == 1 && rp.Permission.Code == permissionCode)
                    .Where(rp => (rp.EffectiveFrom == null || rp.EffectiveFrom <= DateTime.UtcNow) &&
                                (rp.EffectiveTo == null || rp.EffectiveTo > DateTime.UtcNow))
                    .AnyAsync();

                if (hasRolePermission)
                {
                    return ApiResponse<bool>.Success(true);
                }

                // 检查直接权限
                var hasDirectPermission = await _context.UserPermissions
                    .Where(up => up.UserId == userId && up.Status == 1 && up.Permission.Status == 1 && up.Permission.Code == permissionCode)
                    .Where(up => (up.EffectiveFrom == null || up.EffectiveFrom <= DateTime.UtcNow) &&
                                (up.EffectiveTo == null || up.EffectiveTo > DateTime.UtcNow))
                    .AnyAsync();

                return ApiResponse<bool>.Success(hasDirectPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查权限失败");
                return ApiResponse<bool>.Failure("检查权限失败");
            }
        }

        /// <summary>
        /// 检查任意权限
        /// </summary>
        public async Task<ApiResponse<bool>> CheckAnyPermissionAsync(Guid userId, List<string> permissionCodes)
        {
            try
            {
                foreach (var code in permissionCodes)
                {
                    var result = await CheckPermissionAsync(userId, code);
                    if (result.IsSuccess && result.Data)
                    {
                        return ApiResponse<bool>.Success(true);
                    }
                }

                return ApiResponse<bool>.Success(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查任意权限失败");
                return ApiResponse<bool>.Failure("检查任意权限失败");
            }
        }

        /// <summary>
        /// 检查所有权限
        /// </summary>
        public async Task<ApiResponse<bool>> CheckAllPermissionsAsync(Guid userId, List<string> permissionCodes)
        {
            try
            {
                foreach (var code in permissionCodes)
                {
                    var result = await CheckPermissionAsync(userId, code);
                    if (!result.IsSuccess || !result.Data)
                    {
                        return ApiResponse<bool>.Success(false);
                    }
                }

                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查所有权限失败");
                return ApiResponse<bool>.Failure("检查所有权限失败");
            }
        }

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        public async Task<ApiResponse<List<string>>> GetUserPermissionsAsync(Guid userId)
        {
            try
            {
                var permissions = new List<string>();

                // 通过角色获取权限
                var rolePermissions = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.Status == 1)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => rp.Status == 1 && rp.Permission.Status == 1)
                    .Where(rp => (rp.EffectiveFrom == null || rp.EffectiveFrom <= DateTime.UtcNow) &&
                                (rp.EffectiveTo == null || rp.EffectiveTo > DateTime.UtcNow))
                    .Select(rp => rp.Permission.Code)
                    .ToListAsync();

                permissions.AddRange(rolePermissions);

                // 获取直接权限
                var userPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == userId && up.Status == 1 && up.Permission.Status == 1)
                    .Where(up => (up.EffectiveFrom == null || up.EffectiveFrom <= DateTime.UtcNow) &&
                                (up.EffectiveTo == null || up.EffectiveTo > DateTime.UtcNow))
                    .Select(up => up.Permission.Code)
                    .ToListAsync();

                permissions.AddRange(userPermissions);

                var distinctPermissions = permissions.Distinct().OrderBy(p => p).ToList();
                return ApiResponse<List<string>>.Success(distinctPermissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户权限列表失败");
                return ApiResponse<List<string>>.Failure("获取用户权限列表失败");
            }
        }

        /// <summary>
        /// 检查数据范围权限
        /// </summary>
        public async Task<ApiResponse<bool>> CheckDataScopeAsync(Guid userId, string dataScope, Guid? targetUserId = null, Guid? targetOrgId = null)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return ApiResponse<bool>.Success(false);
                }

                var userDataScope = user.UserRoles.FirstOrDefault()?.Role?.DataScope ?? "Self";

                switch (userDataScope)
                {
                    case "All":
                        return ApiResponse<bool>.Success(true);
                    case "Organization":
                        if (targetUserId.HasValue)
                        {
                            var targetUser = await _context.Users.FindAsync(targetUserId.Value);
                            return ApiResponse<bool>.Success(targetUser?.OrganizationId == user.OrganizationId);
                        }
                        return ApiResponse<bool>.Success(targetOrgId == user.OrganizationId);
                    case "Department":
                        // 这里需要实现部门级权限检查
                        return ApiResponse<bool>.Success(false);
                    case "Self":
                        return ApiResponse<bool>.Success(targetUserId == userId);
                    default:
                        return ApiResponse<bool>.Success(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查数据范围权限失败");
                return ApiResponse<bool>.Failure("检查数据范围权限失败");
            }
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        public async Task<ApiResponse<object>> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, Guid operatorId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // 删除现有权限
                var existingPermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId)
                    .ToListAsync();

                _context.RolePermissions.RemoveRange(existingPermissions);

                // 添加新权限
                var newPermissions = permissionIds.Select(permissionId => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId,
                    GrantedBy = operatorId,
                    GrantedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString()
                }).ToList();

                await _context.RolePermissions.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<object>.Success(null, "权限分配成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为角色分配权限失败");
                return ApiResponse<object>.Failure("为角色分配权限失败");
            }
        }

        /// <summary>
        /// 为用户分配权限
        /// </summary>
        public async Task<ApiResponse<object>> AssignPermissionsToUserAsync(Guid userId, List<Guid> permissionIds, Guid operatorId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // 删除现有权限
                var existingPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == userId)
                    .ToListAsync();

                _context.UserPermissions.RemoveRange(existingPermissions);

                // 添加新权限
                var newPermissions = permissionIds.Select(permissionId => new UserPermission
                {
                    UserId = userId,
                    PermissionId = permissionId,
                    GrantedBy = operatorId,
                    GrantedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString()
                }).ToList();

                await _context.UserPermissions.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<object>.Success(null, "权限分配成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为用户分配权限失败");
                return ApiResponse<object>.Failure("为用户分配权限失败");
            }
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        public async Task<ApiResponse<Guid>> CreatePermissionAsync(CreatePermissionDto dto, Guid operatorId)
        {
            try
            {
                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    TenantId = dto.TenantId,
                    Code = dto.Code,
                    Name = dto.Name,
                    Description = dto.Description,
                    Type = dto.Type,
                    ParentId = dto.ParentId,
                    Sort = dto.Sort,
                    ApiPath = dto.ApiPath,
                    HttpMethods = dto.HttpMethods,
                    Icon = dto.Icon,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString()
                };

                // 计算层级和路径
                if (dto.ParentId.HasValue)
                {
                    var parent = await _context.Permissions.FindAsync(dto.ParentId.Value);
                    if (parent != null)
                    {
                        permission.Level = parent.Level + 1;
                        permission.TreePath = $"{parent.TreePath}/{permission.Id}";
                    }
                }
                else
                {
                    permission.Level = 1;
                    permission.TreePath = permission.Id.ToString();
                }

                await _context.Permissions.AddAsync(permission);
                await _context.SaveChangesAsync();

                return ApiResponse<Guid>.Success(permission.Id, "权限创建成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建权限失败");
                return ApiResponse<Guid>.Failure("创建权限失败");
            }
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        public async Task<ApiResponse<object>> UpdatePermissionAsync(Guid id, UpdatePermissionDto dto, Guid operatorId)
        {
            try
            {
                var permission = await _context.Permissions.FindAsync(id);
                if (permission == null)
                {
                    return ApiResponse<object>.Failure("权限不存在");
                }

                if (permission.IsSystem)
                {
                    return ApiResponse<object>.Failure("系统权限不允许修改");
                }

                permission.Name = dto.Name;
                permission.Description = dto.Description;
                permission.Sort = dto.Sort;
                permission.Status = dto.Status;
                permission.ApiPath = dto.ApiPath;
                permission.HttpMethods = dto.HttpMethods;
                permission.Icon = dto.Icon;
                permission.Remarks = dto.Remarks;
                permission.UpdatedAt = DateTime.UtcNow;
                permission.UpdatedBy = operatorId.ToString();

                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "权限更新成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新权限失败");
                return ApiResponse<object>.Failure("更新权限失败");
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        public async Task<ApiResponse<object>> DeletePermissionAsync(Guid id, Guid operatorId)
        {
            try
            {
                var permission = await _context.Permissions.FindAsync(id);
                if (permission == null)
                {
                    return ApiResponse<object>.Failure("权限不存在");
                }

                if (permission.IsSystem)
                {
                    return ApiResponse<object>.Failure("系统权限不允许删除");
                }

                // 检查是否有子权限
                var hasChildren = await _context.Permissions
                    .AnyAsync(p => p.ParentId == id);

                if (hasChildren)
                {
                    return ApiResponse<object>.Failure("存在子权限，无法删除");
                }

                permission.IsDeleted = true;
                permission.UpdatedAt = DateTime.UtcNow;
                permission.UpdatedBy = operatorId.ToString();

                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "权限删除成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除权限失败");
                return ApiResponse<object>.Failure("删除权限失败");
            }
        }

        /// <summary>
        /// 构建权限树
        /// </summary>
        private static List<PermissionTreeDto> BuildPermissionTree(List<PermissionTreeDto> permissions, string? parentId)
        {
            return permissions
                .Where(p => p.ParentId == parentId)
                .Select(p =>
                {
                    p.Children = BuildPermissionTree(permissions, p.Id);
                    return p;
                })
                .OrderBy(p => p.Sort)
                .ToList();
        }
    }
}