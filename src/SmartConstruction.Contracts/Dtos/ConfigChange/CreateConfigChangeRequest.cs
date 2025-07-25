namespace SmartConstruction.Contracts.Dtos.ConfigChange;

public class CreateConfigChangeRequest
{
    public Guid TenantId { get; set; }
    public string ConfigType { get; set; }
    public string ConfigKey { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangeType { get; set; }
    public Guid? ChangedBy { get; set; }
    public bool NotificationSent { get; set; } = false;
    public string? NotificationMethod { get; set; }
} 