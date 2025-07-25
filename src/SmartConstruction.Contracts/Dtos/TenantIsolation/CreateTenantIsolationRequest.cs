namespace SmartConstruction.Contracts.Dtos.TenantIsolation;

public class CreateTenantIsolationRequest
{
    public Guid TenantId { get; set; }
    public string CurrentLevel { get; set; } = "Shared";
    public string? SchemaName { get; set; }
    public string? MigrationStatus { get; set; }
    public long RecordCount { get; set; } = 0;
    public int StorageUsageMB { get; set; } = 0;
    public int DailyRequests { get; set; } = 0;
    public bool AutoScaleEnabled { get; set; } = true;
    public int UpgradeThreshold { get; set; } = 10000;
    public int DowngradeThreshold { get; set; } = 1000;
} 