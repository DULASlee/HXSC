using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 用户角色关系
    /// </summary>
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public Guid? AssignedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public byte Status { get; set; } = 1; // 状态 (0:禁用, 1:正常)

        // 导航属性
        /// <summary>
        /// 用户
        /// </summary>
        public virtual User User { get; set; } = null!;

        /// <summary>
        /// 角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;
    }
} 