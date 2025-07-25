using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Exceptions;
using SmartConstruction.Service.Models;
using System.Text.Json;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 菜单管理服务实现
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<MenuService> _logger;

        public MenuService(
            ApplicationDbContext context,
            ILogger<MenuService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        public async Task<ApiResponse<List<MenuTreeDto>>> GetMenuTreeAsync(Guid? tenantId = null)
        {
            try
            {
                var query = _context.Menus.AsQueryable();
                
                if (tenantId.HasValue)
                {
                    query = query.Where(m => m.TenantId == tenantId.Value);
                }
                
                var menus = await query
                    .Where(m => m.Status == 1)
                    .OrderBy(m => m.Sort)
                    .ToListAsync();

                var menuDtos = menus.Select(m => new MenuTreeDto
                {
                    Id = m.Id.ToString(),
                    ParentId = m.ParentId?.ToString(),
                    Name = m.Name,
                    Title = m.Title,
                    Path = m.Path,
                    Component = m.Component,
                    Icon = m.Icon,
                    Type = m.Type,
                    Sort = m.Sort,
                    Level = m.Level,
                    Status = m.Status,
                    IsVisible = m.IsVisible,
                    Permission = m.Permission
                }).ToList();

                var tree = BuildMenuTree(menuDtos, null);
                return ApiResponse<List<MenuTreeDto>>.Success(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取菜单树失败");
                return ApiResponse<List<MenuTreeDto>>.Failure("获取菜单树失败");
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        public async Task<ApiResponse<List<MenuInfo>>> GetUserMenusAsync(Guid userId)
        {
            try
            {
                var menuIds = new List<Guid>();

                // 通过角色获取菜单
                var roleMenuIds = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.Status == 1)
                    .SelectMany(ur => ur.Role.RoleMenus)
                    .Where(rm => rm.Status == 1 && rm.Menu.Status == 1)
                    // .Where(rm => (rm.EffectiveFrom == null || rm.EffectiveFrom <= DateTime.UtcNow) &&
                    //             (rm.EffectiveTo == null || rm.EffectiveTo > DateTime.UtcNow))
                    .Select(rm => rm.MenuId)
                    .ToListAsync();

                menuIds.AddRange(roleMenuIds);

                // 获取直接权限菜单
                var userMenuIds = await _context.UserMenus
                    .Where(um => um.UserId == userId && um.Status == 1 && um.Menu.Status == 1)
                    // .Where(um => (um.EffectiveFrom == null || um.EffectiveFrom <= DateTime.UtcNow) &&
                    //             (um.EffectiveTo == null || um.EffectiveTo > DateTime.UtcNow))
                    .Select(um => um.MenuId)
                    .ToListAsync();

                menuIds.AddRange(userMenuIds);

                // 获取菜单详情
                var distinctMenuIds = menuIds.Distinct().ToList();
                var menus = await _context.Menus
                    .Where(m => distinctMenuIds.Contains(m.Id) && m.Status == 1 && m.IsVisible)
                    .OrderBy(m => m.Sort)
                    .ToListAsync();

                var menuInfos = menus.Select(m => new MenuInfo
                {
                    Id = m.Id.ToString(),
                    ParentId = m.ParentId?.ToString(),
                    Name = m.Name,
                    Title = m.Title,
                    Path = m.Path ?? "",
                    Component = m.Component,
                    Icon = m.Icon,
                    Sort = m.Sort,
                    Type = m.Type ?? "Menu",
                    Status = m.Status,
                    IsVisible = m.IsVisible,
                    IsEnabled = m.Status == 1,
                    // FullPath 将在构建树时计算
                }).ToList();

                var tree = BuildMenuInfoTree(menuInfos, null, "");
                return ApiResponse<List<MenuInfo>>.Success(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户菜单失败");
                return ApiResponse<List<MenuInfo>>.Failure("获取用户菜单失败");
            }
        }

        /// <summary>
        /// 获取角色菜单
        /// </summary>
        public async Task<ApiResponse<List<Guid>>> GetRoleMenusAsync(Guid roleId)
        {
            try
            {
                var menuIds = await _context.RoleMenus
                    .Where(rm => rm.RoleId == roleId && rm.Status == 1)
                    .Select(rm => rm.MenuId)
                    .ToListAsync();

                return ApiResponse<List<Guid>>.Success(menuIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取角色菜单失败");
                return ApiResponse<List<Guid>>.Failure("获取角色菜单失败");
            }
        }

        /// <summary>
        /// 为角色分配菜单
        /// </summary>
        public async Task<ApiResponse<object>> AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds, Guid operatorId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // 删除现有菜单
                var existingMenus = await _context.RoleMenus
                    .Where(rm => rm.RoleId == roleId)
                    .ToListAsync();

                _context.RoleMenus.RemoveRange(existingMenus);

                // 添加新菜单
                var newMenus = menuIds.Select(menuId => new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId,
                    GrantedBy = operatorId,
                    GrantedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString()
                }).ToList();

                await _context.RoleMenus.AddRangeAsync(newMenus);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<object>.Success(null, "菜单分配成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为角色分配菜单失败");
                return ApiResponse<object>.Failure("为角色分配菜单失败");
            }
        }

        /// <summary>
        /// 为用户分配菜单
        /// </summary>
        public async Task<ApiResponse<object>> AssignMenusToUserAsync(Guid userId, List<Guid> menuIds, Guid operatorId)
        {
            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                // 删除现有菜单
                var existingMenus = await _context.UserMenus
                    .Where(um => um.UserId == userId)
                    .ToListAsync();

                _context.UserMenus.RemoveRange(existingMenus);

                // 添加新菜单
                var newMenus = menuIds.Select(menuId => new UserMenu
                {
                    UserId = userId,
                    MenuId = menuId,
                    GrantedBy = operatorId,
                    GrantedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString()
                }).ToList();

                await _context.UserMenus.AddRangeAsync(newMenus);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResponse<object>.Success(null, "菜单分配成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为用户分配菜单失败");
                return ApiResponse<object>.Failure("为用户分配菜单失败");
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        public async Task<ApiResponse<Guid>> CreateMenuAsync(CreateMenuDto dto, Guid operatorId)
        {
            try
            {
                var menu = new Menu
                {
                    Id = Guid.NewGuid(),
                    TenantId = dto.TenantId,
                    ParentId = dto.ParentId,
                    Name = dto.Name,
                    Title = dto.Title,
                    Path = dto.Path,
                    Component = dto.Component,
                    Icon = dto.Icon,
                    Type = dto.Type,
                    Sort = dto.Sort,
                    IsVisible = dto.IsVisible,
                    Permission = dto.Permission,
                    ExternalLink = dto.ExternalLink,
                    Description = dto.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString()
                };

                // 计算层级和路径
                if (dto.ParentId.HasValue)
                {
                    var parent = await _context.Menus.FindAsync(dto.ParentId.Value);
                    if (parent != null)
                    {
                        menu.Level = parent.Level + 1;
                        menu.TreePath = $"{parent.TreePath}/{menu.Id}";
                    }
                }
                else
                {
                    menu.Level = 1;
                    menu.TreePath = menu.Id.ToString();
                }

                await _context.Menus.AddAsync(menu);
                await _context.SaveChangesAsync();

                return ApiResponse<Guid>.Success(menu.Id, "菜单创建成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建菜单失败");
                return ApiResponse<Guid>.Failure("创建菜单失败");
            }
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        public async Task<ApiResponse<object>> UpdateMenuAsync(Guid id, UpdateMenuDto dto, Guid operatorId)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(id);
                if (menu == null)
                {
                    return ApiResponse<object>.Failure("菜单不存在");
                }

                menu.Name = dto.Name;
                menu.Title = dto.Title;
                menu.Path = dto.Path;
                menu.Component = dto.Component;
                menu.Icon = dto.Icon;
                menu.Sort = dto.Sort;
                menu.Status = dto.Status;
                menu.IsVisible = dto.IsVisible;
                menu.Permission = dto.Permission;
                menu.ExternalLink = dto.ExternalLink;
                menu.Description = dto.Description;
                menu.UpdatedAt = DateTime.UtcNow;
                menu.UpdatedBy = operatorId.ToString();

                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "菜单更新成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新菜单失败");
                return ApiResponse<object>.Failure("更新菜单失败");
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        public async Task<ApiResponse<object>> DeleteMenuAsync(Guid id, Guid operatorId)
        {
            try
            {
                var menu = await _context.Menus.FindAsync(id);
                if (menu == null)
                {
                    return ApiResponse<object>.Failure("菜单不存在");
                }

                // 检查是否有子菜单
                var hasChildren = await _context.Menus
                    .AnyAsync(m => m.ParentId == id);

                if (hasChildren)
                {
                    return ApiResponse<object>.Failure("存在子菜单，无法删除");
                }

                menu.IsDeleted = true;
                menu.UpdatedAt = DateTime.UtcNow;
                menu.UpdatedBy = operatorId.ToString();

                await _context.SaveChangesAsync();

                return ApiResponse<object>.Success(null, "菜单删除成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除菜单失败");
                return ApiResponse<object>.Failure("删除菜单失败");
            }
        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        private static List<MenuTreeDto> BuildMenuTree(List<MenuTreeDto> menus, string? parentId)
        {
            return menus
                .Where(m => m.ParentId == parentId)
                .Select(m =>
                {
                    m.Children = BuildMenuTree(menus, m.Id);
                    return m;
                })
                .OrderBy(m => m.Sort)
                .ToList();
        }

        /// <summary>
        /// 构建菜单信息树
        /// </summary>
        private static List<MenuInfo> BuildMenuInfoTree(List<MenuInfo> allMenus, string? parentId, string parentPath)
        {
            var tree = allMenus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Sort)
                .Select(m =>
                {
                    // 计算当前节点的 FullPath
                    m.FullPath = $"{parentPath}/{m.Path}".TrimStart('/');
                    // 递归构建子树
                    m.Children = BuildMenuInfoTree(allMenus, m.Id, m.FullPath);
                    return m;
                })
                .ToList();

            return tree;
        }

        /// <summary>
        /// 【最佳实践】创建一个新的菜单项，并处理所有关联数据（使用事务保证原子性）
        /// </summary>
        /// <param name="dto">创建菜单的DTO</param>
        /// <param name="operatorId">操作员ID</param>
        /// <returns>新创建菜单的ID</returns>
        /// <exception cref="BusinessException">当业务规则校验失败时抛出</exception>
        public async Task<ApiResponse<Guid>> CreateMenuTransactionalAsync(CreateMenuDto dto, Guid operatorId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 业务规则校验：在同一父菜单下，Name 和 Path 必须唯一
                var siblingExists = await _context.Menus
                    .AnyAsync(m => m.ParentId == dto.ParentId && (m.Name == dto.Name || m.Path == dto.Path));
                if (siblingExists)
                {
                    throw new BusinessException($"在同一目录下，菜单名称 '{dto.Name}' 或路径 '{dto.Path}' 已存在。");
                }

                // 业务规则校验：计算层级，并确保父菜单存在
                int level = 1;
                if (dto.ParentId.HasValue)
                {
                    var parentMenu = await _context.Menus.FindAsync(dto.ParentId.Value);
                    if (parentMenu == null)
                    {
                        throw new BusinessException("指定的父菜单不存在。");
                    }
                    level = parentMenu.Level + 1;
                }

                // 创建主实体
                var menu = new Menu
                {
                    Name = dto.Name,
                    Title = dto.Title,
                    Path = dto.Path,
                    Component = dto.Component,
                    ParentId = dto.ParentId,
                    Icon = dto.Icon,
                    Type = dto.Type,
                    Sort = dto.Sort,
                    Level = level, // 使用后端计算的层级
                    Status = 1, // 默认启用
                    IsVisible = dto.IsVisible,
                    Permission = dto.Permission,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = operatorId.ToString(),
                    UpdatedBy = operatorId.ToString(),
                    IsDeleted = false
                };
                _context.Menus.Add(menu);
                await _context.SaveChangesAsync(); // 先保存一次获取 menu.Id

                // 自动为新菜单关联 '系统管理员' 角色，防止菜单丢失
                var adminRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == "Administrator");
                if (adminRole != null)
                {
                    var roleMenu = new RoleMenu
                    {
                        RoleId = adminRole.Id,
                        MenuId = menu.Id,
                        Status = 1,
                        AccessType = "ReadWrite",
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = operatorId.ToString(),
                        IsDeleted = false
                    };
                    _context.RoleMenus.Add(roleMenu);
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return ApiResponse<Guid>.Success(menu.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "创建菜单失败，操作已回滚。DTO: {@dto}", dto);
                
                if (ex is BusinessException) throw;
                throw new BusinessException("创建菜单时发生内部错误。", ex);
            }
        }
    }
}