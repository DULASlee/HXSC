using System;
using System.Collections.Generic;
using SmartConstruction.Contracts.Dtos.Safety;

namespace SmartConstruction.Contracts.Dtos.Dashboard;

/// <summary>
/// 公司仪表盘DTO
/// </summary>
public class CompanyDashboardDto
{
    /// <summary>
    /// 公司ID
    /// </summary>
    public Guid CompanyId { get; set; }

    /// <summary>
    /// 公司名称
    /// </summary>
    public string CompanyName { get; set; } = null!;

    /// <summary>
    /// 项目总数
    /// </summary>
    public int TotalProjects { get; set; }

    /// <summary>
    /// 活跃项目数
    /// </summary>
    public int ActiveProjects { get; set; }

    /// <summary>
    /// 已完成项目数
    /// </summary>
    public int CompletedProjects { get; set; }

    /// <summary>
    /// 总工人数
    /// </summary>
    public int TotalWorkers { get; set; }

    /// <summary>
    /// 今日出勤人数
    /// </summary>
    public int TodayAttendance { get; set; }

    /// <summary>
    /// 出勤率
    /// </summary>
    public double AttendanceRate { get; set; }

    /// <summary>
    /// 设备总数
    /// </summary>
    public int TotalDevices { get; set; }

    /// <summary>
    /// 在线设备数
    /// </summary>
    public int OnlineDevices { get; set; }

    /// <summary>
    /// 未处理安全事故数
    /// </summary>
    public int UnresolvedIncidents { get; set; }

    /// <summary>
    /// 本月安全事故数
    /// </summary>
    public int MonthlyIncidents { get; set; }

    /// <summary>
    /// 项目列表
    /// </summary>
    public List<ProjectSummaryDto> Projects { get; set; } = new();

    /// <summary>
    /// 最近考勤统计
    /// </summary>
    public List<AttendanceStatisticsDto> RecentAttendance { get; set; } = new();

    /// <summary>
    /// 设备状态统计
    /// </summary>
    public DeviceStatusStatisticsDto DeviceStatistics { get; set; } = new();

    /// <summary>
    /// 安全统计
    /// </summary>
    public SafetyStatisticsDto SafetyStatistics { get; set; } = new();
}
