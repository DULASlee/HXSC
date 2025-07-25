using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.ConfigChange;

public class ConfigChangeDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string ConfigType { get; set; }
    public string ConfigKey { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string ChangeType { get; set; }
    public DateTime ChangedAt { get; set; }
    public Guid? ChangedBy { get; set; }
    public bool NotificationSent { get; set; }
    public string? NotificationMethod { get; set; }
} 