namespace SmartConstruction.Contracts.Dtos.Attendance;

public class AttendanceDto : BaseDto
{
    public Guid WorkerId { get; set; }
    public string WorkerName { get; set; } = null!;
    public string IdCardNumber { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public AttendanceType Type { get; set; }
    public string TypeName => Type.ToString();
    public string? DeviceCode { get; set; }
    public string? FaceImage { get; set; }
    public decimal? Temperature { get; set; }
    public double? WorkHours => CheckOutTime.HasValue ? (CheckOutTime.Value - CheckInTime).TotalHours : null;
    public string TeamName { get; set; } = null!;
}

public class CreateAttendanceDto
{
    public Guid WorkerId { get; set; }
    public Guid ProjectId { get; set; }
    public DateTime CheckInTime { get; set; }
    public AttendanceType Type { get; set; }
    public string? DeviceCode { get; set; }
    public string? FaceImage { get; set; }
    public decimal? Temperature { get; set; }
}

public class AttendanceListRequest : PagedRequestBase
{
    public Guid? WorkerId { get; set; }
    public Guid? ProjectId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public AttendanceType? Type { get; set; }
}

public class AttendanceStatisticsDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public DateTime Date { get; set; }
    public int TotalWorkers { get; set; }
    public int CheckedInWorkers { get; set; }
    public int AbsentWorkers { get; set; }
    public double AttendanceRate => TotalWorkers > 0 ? (double)CheckedInWorkers / TotalWorkers * 100 : 0;
    public int PresentWorkers { get; set; }
    public int CheckedInToday { get; set; }
    public int CheckedOutToday { get; set; }
    public DateTime StatisticsDate { get; set; }
    public DateTime LastUpdate { get; set; }
}