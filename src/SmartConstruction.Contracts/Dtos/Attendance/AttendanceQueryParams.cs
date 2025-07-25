namespace SmartConstruction.Contracts.Dtos.Attendance;

public class AttendanceQueryParams
{
    public Guid? ProjectId { get; set; }
    public Guid? WorkerId { get; set; }
    public Guid? TeamId { get; set; }
    public string? AttendanceType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? WorkerName { get; set; }
    public bool? IsSynced { get; set; }
}
