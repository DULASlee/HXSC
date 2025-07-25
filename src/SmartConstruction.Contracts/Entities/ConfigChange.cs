using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 配置变更追踪表
    /// </summary>
    public class ConfigChange : BaseEntity
    {
        public Guid TenantId { get; set; }
        public string ConfigType { get; set; }
        public string ConfigKey { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string ChangeType { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        public Guid? ChangedBy { get; set; }
        public bool NotificationSent { get; set; } = false;
        public string NotificationMethod { get; set; }
    }
} 