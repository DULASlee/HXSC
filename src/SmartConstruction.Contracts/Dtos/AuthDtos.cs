using System.ComponentModel.DataAnnotations;

namespace SmartConstruction.Contracts.Dtos
{
    /// <summary>
    /// 登录请求DTO
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 租户代码
        /// </summary>
        [Required]
        public string TenantCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 记住我
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string? DeviceId { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string? DeviceType { get; set; }
    }

    /// <summary>
    /// 登录响应DTO
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// 认证令牌信息
        /// </summary>
        public TokenInfo Token { get; set; } = new();

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo User { get; set; } = new();

        /// <summary>
        /// 用户权限列表
        /// </summary>
        public List<string> Permissions { get; set; } = new();

        /// <summary>
        /// 用户菜单列表
        /// </summary>
        public List<MenuInfo> Menus { get; set; } = new();
    }

    /// <summary>
    /// 令牌信息
    /// </summary>
    public class TokenInfo
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// 令牌类型
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// 过期时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 租户ID
        /// </summary>
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string? Mobile { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 组织路径
        /// </summary>
        public string? OrganizationPath { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public List<RoleInfo> Roles { get; set; } = new();

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 角色信息
    /// </summary>
    public class RoleInfo
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 角色代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 数据范围
        /// </summary>
        public string DataScope { get; set; } = "Self";

        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 是否系统角色
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 菜单信息
    /// </summary>
    public class MenuInfo
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 父级菜单ID
        /// </summary>
        public string? ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜单标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 菜单路径
        /// </summary>
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 完整路由路径
        /// </summary>
        public string? FullPath { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        public string? Component { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜单类型
        /// </summary>
        public string Type { get; set; } = "Menu";

        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// 是否可见
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 子菜单
        /// </summary>
        public List<MenuInfo> Children { get; set; } = new();
    }

    /// <summary>
    /// 刷新令牌请求
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// 刷新令牌
        /// </summary>
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string? DeviceId { get; set; }
    }

    /// <summary>
    /// 获取当前用户信息响应
    /// </summary>
    public class CurrentUserResponse
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo User { get; set; } = new();

        /// <summary>
        /// 用户权限列表
        /// </summary>
        public List<string> Permissions { get; set; } = new();
    }

    /// <summary>
    /// 获取用户菜单响应
    /// </summary>
    public class UserMenusResponse
    {
        /// <summary>
        /// 用户菜单列表
        /// </summary>
        public List<MenuInfo> Menus { get; set; } = new();

        /// <summary>
        /// 用户权限列表
        /// </summary>
        public List<string> Permissions { get; set; } = new();
    }
}