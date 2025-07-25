using System;

namespace SmartConstruction.Contracts.Dtos.Dashboard;

/// <summary>
/// 考勤统计DTO
/// </summary>
public class AttendanceStatisticsDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 应到人数
    /// </summary>
    public int ExpectedCount { get; set; }

    /// <summary>
    /// 实到人数
    /// </summary>
    public int ActualCount { get; set; }

    /// <summary>
    /// 出勤率
    /// </summary>
    public double AttendanceRate { get; set; }

    /// <summary>
    /// 迟到人数
    /// </summary>
    public int LateCount { get; set; }

    /// <summary>
    /// 早退人数
    /// </summary>
    public int EarlyLeaveCount { get; set; }

    /// <summary>
    /// 缺勤人数
    /// </summary>
    public int AbsentCount { get; set; }

    /// <summary>
    /// 出勤人数
    /// </summary>
    public int PresentCount { get; set; }

    /// <summary>
    /// 请假人数
    /// </summary>
    public int LeaveCount { get; set; }
    public int PresentWorkers { get; set; }
    public int CheckedInToday { get; set; }
    public int CheckedOutToday { get; set; }
    public DateTime StatisticsDate { get; set; }
    public DateTime LastUpdate { get; set; }
}
