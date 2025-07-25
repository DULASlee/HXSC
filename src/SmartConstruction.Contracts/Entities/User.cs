using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; } = string.Empty;



        /// <summary>
        /// 密码哈希
        /// </summary>
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// 安全戳
        /// </summary>
        public string SecurityStamp { get; set; } = string.Empty;

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 状态（1:启用 0:禁用）
        /// </summary>
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 用户类型（Normal:普通用户 System:系统用户）
        /// </summary>
        public string UserType { get; set; } = "Normal";

        /// <summary>
        /// 是否锁定
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        public DateTime? LockedAt { get; set; }

        /// <summary>
        /// 密码过期时间
        /// </summary>
        public DateTime? PasswordExpiresAt { get; set; }

        /// <summary>
        /// 登录失败次数
        /// </summary>
        public int LoginFailureCount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 租户
        /// </summary>
        public virtual Tenant Tenant { get; set; } = null!;

        /// <summary>
        /// 组织
        /// </summary>
        public virtual Organization? Organization { get; set; }

        /// <summary>
        /// 用户角色关系
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// 用户权限关系
        /// </summary>
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

        /// <summary>
        /// 用户菜单关系
        /// </summary>
        public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();

        /// <summary>
        /// 刷新令牌
        /// </summary>
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
} 