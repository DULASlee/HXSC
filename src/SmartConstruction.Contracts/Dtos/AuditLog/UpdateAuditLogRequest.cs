namespace SmartConstruction.Contracts.Dtos.AuditLog;

public class UpdateAuditLogRequest
{
    public string? EventType { get; set; }
    public DateTime? EventTime { get; set; }
    public string? Username { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? EntityName { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public int? Duration { get; set; }
} 