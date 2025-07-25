namespace SmartConstruction.Contracts.Dtos.TenantIsolation;

public class UpdateTenantIsolationRequest
{
    public string? CurrentLevel { get; set; }
    public string? SchemaName { get; set; }
    public string? MigrationStatus { get; set; }
    public long? RecordCount { get; set; }
    public int? StorageUsageMB { get; set; }
    public int? DailyRequests { get; set; }
    public bool? AutoScaleEnabled { get; set; }
    public int? UpgradeThreshold { get; set; }
    public int? DowngradeThreshold { get; set; }
} 