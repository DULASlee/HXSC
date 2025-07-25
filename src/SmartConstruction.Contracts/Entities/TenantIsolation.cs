using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 租户隔离配置表
    /// </summary>
    public class TenantIsolation : BaseEntity
    {
        public Guid TenantId { get; set; }
        public string CurrentLevel { get; set; } = "Shared";
        public byte[] ConnectionString { get; set; }
        public string SchemaName { get; set; }
        public string MigrationStatus { get; set; }
        public DateTime? LastMigrationAt { get; set; }
        public long RecordCount { get; set; } = 0;
        public int StorageUsageMB { get; set; } = 0;
        public int DailyRequests { get; set; } = 0;
        public bool AutoScaleEnabled { get; set; } = true;
        public int UpgradeThreshold { get; set; } = 10000;
        public int DowngradeThreshold { get; set; } = 1000;
    }
} 