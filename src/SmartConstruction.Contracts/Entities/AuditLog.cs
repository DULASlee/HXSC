using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 审计日志实体
    /// </summary>
    public class AuditLog : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 事件类型 (CREATE,UPDATE,DELETE,READ)
        /// </summary>
        public string EventType { get; set; } = null!;

        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName { get; set; } = null!;

        /// <summary>
        /// 实体ID
        /// </summary>
        public string EntityId { get; set; } = null!;

        /// <summary>
        /// 旧值(JSON格式)
        /// </summary>
        public string? OldValues { get; set; }

        /// <summary>
        /// 新值(JSON格式)
        /// </summary>
        public string? NewValues { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}