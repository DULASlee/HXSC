namespace SmartConstruction.Contracts.Dtos.Attendance;

public class UpdateAttendanceRequest
{
    public Guid Id { get; set; }
    public Guid WorkerId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TeamId { get; set; }
    public string AttendanceType { get; set; } = string.Empty;
    public DateTime CheckTime { get; set; }
    public string DeviceId { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string? Remark { get; set; }
}
