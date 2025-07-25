namespace SmartConstruction.Contracts.Dtos.ConfigChange;

public class UpdateConfigChangeRequest
{
    public string? NewValue { get; set; }
    public string? ChangeType { get; set; }
    public bool? NotificationSent { get; set; }
    public string? NotificationMethod { get; set; }
} 