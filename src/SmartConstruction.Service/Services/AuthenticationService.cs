using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 认证服务实现
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            ApplicationDbContext context,
            IJwtService jwtService,
            ILogger<AuthenticationService> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            try
            {
                // 查找租户
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Code == request.TenantCode && t.Status == 1);

                if (tenant == null)
                {
                    return ApiResponse<LoginResponse>.Failure("租户不存在或已禁用");
                }

                // 查找用户
                var user = await _context.Users
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == request.Username 
                                            && u.TenantId == tenant.Id 
                                            && u.Status == 1);

                if (user == null)
                {
                    return ApiResponse<LoginResponse>.Failure("用户名或密码错误");
                }

                // 验证密码
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    return ApiResponse<LoginResponse>.Failure("用户名或密码错误");
                }

                // 获取用户角色和权限
                var roles = user.UserRoles.Select(ur => ur.Role).ToList();
                var permissions = await GetUserPermissionsAsync(user.Id);

                // 如果包含系统管理员角色，自动授予万能权限
                if (roles.Any(r => r.Code == "SYSTEM_ADMIN" || r.Code=="SUPER_ADMIN"))
                {
                    if (!permissions.Contains("*"))
                        permissions.Add("*");
                }
                var menus = await GetUserMenusAsync(user.Id);

                // 生成令牌
                var accessToken = _jwtService.GenerateAccessToken(user, tenant, roles, permissions);
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(
                    user.Id, tenant.Id, request.DeviceId, request.DeviceType);

                // 更新最后登录时间
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var response = new LoginResponse
                {
                    Token = new TokenInfo
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        TokenType = "Bearer",
                        ExpiresIn = 3600, // 1小时
                        ExpiresAt = DateTime.UtcNow.AddHours(1)
                    },
                    User = MapToUserInfo(user, roles),
                    Permissions = permissions,
                    Menus = menus
                };

                _logger.LogInformation("用户 {Username} 登录成功", request.Username);
                return ApiResponse<LoginResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登录过程中发生错误");
                return ApiResponse<LoginResponse>.Failure("登录失败，请稍后重试");
            }
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public async Task<ApiResponse<TokenInfo>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var refreshToken = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken);
                if (refreshToken == null)
                {
                    return ApiResponse<TokenInfo>.Failure("刷新令牌无效或已过期");
                }

                // 获取用户和租户信息
                var user = await _context.Users
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id == refreshToken.UserId);

                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Id == refreshToken.TenantId);

                if (user == null || tenant == null)
                {
                    return ApiResponse<TokenInfo>.Failure("用户或租户信息无效");
                }

                // 获取权限
                var roles = user.UserRoles.Select(ur => ur.Role).ToList();
                var permissions = await GetUserPermissionsAsync(user.Id);

                // 生成新令牌
                var newAccessToken = _jwtService.GenerateAccessToken(user, tenant, roles, permissions);
                var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(
                    user.Id, tenant.Id, request.DeviceId, refreshToken.DeviceType);

                // 撤销旧的刷新令牌
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken, newRefreshToken);

                var tokenInfo = new TokenInfo
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    TokenType = "Bearer",
                    ExpiresIn = 3600,
                    ExpiresAt = DateTime.UtcNow.AddHours(1)
                };

                return ApiResponse<TokenInfo>.Success(tokenInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刷新令牌过程中发生错误");
                return ApiResponse<TokenInfo>.Failure("刷新令牌失败");
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        public async Task<ApiResponse<object>> LogoutAsync(string userId, string? deviceId = null)
        {
            try
            {
                // 撤销用户的所有刷新令牌
                var refreshTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId.ToString() == userId && rt.RevokedAt == null)
                    .ToListAsync();

                if (!string.IsNullOrEmpty(deviceId))
                {
                    // 只撤销指定设备的令牌
                    refreshTokens = refreshTokens.Where(rt => rt.DeviceId == deviceId).ToList();
                }

                foreach (var token in refreshTokens)
                {
                    token.RevokedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("用户 {UserId} 登出成功", userId);
                return ApiResponse<object>.Success(null, "登出成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登出过程中发生错误");
                return ApiResponse<object>.Failure("登出失败");
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        public async Task<ApiResponse<CurrentUserResponse>> GetCurrentUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Id.ToString() == userId && u.Status == 1);

                if (user == null)
                {
                    return ApiResponse<CurrentUserResponse>.Failure("用户不存在");
                }

                var roles = user.UserRoles.Select(ur => ur.Role).ToList();
                var permissions = await GetUserPermissionsAsync(user.Id);

                var response = new CurrentUserResponse
                {
                    User = MapToUserInfo(user, roles),
                    Permissions = permissions
                };

                return ApiResponse<CurrentUserResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户信息时发生错误");
                return ApiResponse<CurrentUserResponse>.Failure("获取用户信息失败");
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        public async Task<ApiResponse<UserMenusResponse>> GetUserMenusAsync(string userId)
        {
            try
            {
                var menus = await GetUserMenusAsync(Guid.Parse(userId));
                var permissions = await GetUserPermissionsAsync(Guid.Parse(userId));

                var response = new UserMenusResponse
                {
                    Menus = menus,
                    Permissions = permissions
                };

                return ApiResponse<UserMenusResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户菜单时发生错误");
                return ApiResponse<UserMenusResponse>.Failure("获取用户菜单失败");
            }
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = _jwtService.ValidateAccessToken(token);
                if (principal == null) return false;

                var userId = _jwtService.GetUserIdFromToken(token);
                if (string.IsNullOrEmpty(userId)) return false;

                // 检查用户是否仍然有效
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id.ToString() == userId && u.Status == 1);

                return user != null;
            }
            catch
            {
                return false;
            }
        }

        #region 私有方法

        /// <summary>
        /// 验证密码
        /// </summary>
        private static bool VerifyPassword(string password, byte[] passwordHash)
        {
            // 这里使用简单的SHA256验证，实际项目应该使用更安全的方法如bcrypt
            using var sha256 = SHA256.Create();
            var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// 计算密码哈希
        /// </summary>
        public static byte[] ComputePasswordHash(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// 获取用户权限
        /// </summary>
        private async Task<List<string>> GetUserPermissionsAsync(Guid userId)
        {
            try
            {
                var permissions = new List<string>();

                // 1. 通过角色获取权限
                var rolePermissions = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.Status == 1)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Where(rp => rp.Status == 1 && rp.Permission.Status == 1)
                    .Where(rp => (rp.EffectiveFrom == null || rp.EffectiveFrom <= DateTime.UtcNow) &&
                                (rp.EffectiveTo == null || rp.EffectiveTo > DateTime.UtcNow))
                    .Select(rp => rp.Permission.Code)
                    .ToListAsync();

                permissions.AddRange(rolePermissions);

                // 2. 获取直接授权给用户的权限
                var userPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == userId && up.Status == 1 && up.Permission.Status == 1)
                    .Where(up => (up.EffectiveFrom == null || up.EffectiveFrom <= DateTime.UtcNow) &&
                                (up.EffectiveTo == null || up.EffectiveTo > DateTime.UtcNow))
                    .Select(up => up.Permission.Code)
                    .ToListAsync();

                permissions.AddRange(userPermissions);

                // 3. 去重并排序
                return permissions.Distinct().OrderBy(p => p).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户权限失败，用户ID: {UserId}", userId);
                // 如果查询失败，返回基础权限
                return new List<string> { "user.view" };
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        private async Task<List<MenuInfo>> GetUserMenusAsync(Guid userId)
        {
            try
            {
                var menuIds = new List<Guid>();

                // 1. 通过角色获取菜单
                var roleMenuIds = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.Status == 1)
                    .SelectMany(ur => ur.Role.RoleMenus)
                    .Where(rm => rm.Status == 1 && rm.Menu.Status == 1)
                    .Where(rm => (rm.EffectiveFrom == null || rm.EffectiveFrom <= DateTime.UtcNow) &&
                                (rm.EffectiveTo == null || rm.EffectiveTo > DateTime.UtcNow))
                    .Select(rm => rm.MenuId)
                    .ToListAsync();

                menuIds.AddRange(roleMenuIds);

                // 2. 获取直接授权给用户的菜单
                var userMenuIds = await _context.UserMenus
                    .Where(um => um.UserId == userId && um.Status == 1 && um.Menu.Status == 1)
                    .Where(um => (um.EffectiveFrom == null || um.EffectiveFrom <= DateTime.UtcNow) &&
                                (um.EffectiveTo == null || um.EffectiveTo > DateTime.UtcNow))
                    .Select(um => um.MenuId)
                    .ToListAsync();

                menuIds.AddRange(userMenuIds);

                // 3. 获取菜单详情，并去重
                var distinctMenuIds = menuIds.Distinct().ToList();
                var menus = await _context.Menus
                    .Where(m => distinctMenuIds.Contains(m.Id) && m.Status == 1)
                    .OrderBy(m => m.Sort)
                    .ToListAsync();

                // 4. 构建菜单树
                return BuildMenuTree(menus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户菜单失败，用户ID: {UserId}", userId);
                // 如果查询失败，返回基础菜单
                return GetDefaultMenus();
            }
        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        private static List<MenuInfo> BuildMenuTree(List<Menu> menus)
        {
            var menuInfos = menus.Select(m => new MenuInfo
            {
                Id = m.Id.ToString(),
                ParentId = m.ParentId?.ToString(),
                Name = m.Name,
                Path = m.Path ?? "",
                Component = m.Component,
                Icon = m.Icon,
                Sort = m.Sort,
                Type = m.Type ?? "Menu",
                Status = m.Status
            }).ToList();

            return BuildTree(menuInfos, null);
        }

        /// <summary>
        /// 递归构建树结构
        /// </summary>
        private static List<MenuInfo> BuildTree(List<MenuInfo> allMenus, string? parentId)
        {
            return allMenus
                .Where(m => m.ParentId == parentId)
                .Select(m =>
                {
                    m.Children = BuildTree(allMenus, m.Id);
                    return m;
                })
                .OrderBy(m => m.Sort)
                .ToList();
        }

        /// <summary>
        /// 获取默认菜单
        /// </summary>
        private static List<MenuInfo> GetDefaultMenus()
        {
            return new List<MenuInfo>
            {
                new() { Id = "1", Name = "工作台", Path = "/dashboard", Icon = "Dashboard", Sort = 1 },
                new() { Id = "2", Name = "数字孪生", Path = "/digital-twin", Icon = "Monitor", Sort = 2 },
                new() { Id = "3", Name = "项目管理", Path = "/project", Icon = "OfficeBuilding", Sort = 3 },
                new() { Id = "4", Name = "人员管理", Path = "/worker", Icon = "User", Sort = 4 },
                new() { Id = "5", Name = "设备管理", Path = "/device", Icon = "Setting", Sort = 5 }
            };
        }

        /// <summary>
        /// 映射用户信息
        /// </summary>
        private static UserInfo MapToUserInfo(User user, List<Role> roles)
        {
            return new UserInfo
            {
                Id = user.Id.ToString(),
                TenantId = user.TenantId.ToString(),
                Username = user.Username,
                DisplayName = user.DisplayName ?? user.Username,
                Email = user.Email,
                Mobile = user.Mobile,
                Avatar = "/avatar/default.png",
                Status = user.Status,
                OrganizationPath = user.OrganizationId?.ToString(),
                Roles = roles.Select(r => new RoleInfo
                {
                    Id = r.Id.ToString(),
                    Code = r.Code,
                    Name = r.Name,
                    DataScope = r.DataScope ?? "Self",
                    Status = r.Status,
                    IsSystem = r.IsSystem,
                    CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                }).ToList(),
                CreatedAt = user.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        #endregion
    }
}