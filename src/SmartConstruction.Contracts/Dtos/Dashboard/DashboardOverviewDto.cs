using System;
using System.Collections.Generic;
using SmartConstruction.Contracts.Dtos.Safety;

namespace SmartConstruction.Contracts.Dtos.Dashboard;

/// <summary>
/// 仪表盘概览DTO
/// </summary>
public class DashboardOverviewDto
{
    /// <summary>
    /// 项目总数
    /// </summary>
    public int TotalProjects { get; set; }

    /// <summary>
    /// 活跃项目数
    /// </summary>
    public int ActiveProjects { get; set; }

    /// <summary>
    /// 总工人数
    /// </summary>
    public int TotalWorkers { get; set; }

    /// <summary>
    /// 在线设备数
    /// </summary>
    public int OnlineDevices { get; set; }

    /// <summary>
    /// 今日出勤人数
    /// </summary>
    public int TodayAttendance { get; set; }

    /// <summary>
    /// 出勤率
    /// </summary>
    public double AttendanceRate { get; set; }

    /// <summary>
    /// 未处理安全事故数
    /// </summary>
    public int UnresolvedIncidents { get; set; }

    /// <summary>
    /// 最近项目列表
    /// </summary>
    public List<ProjectSummaryDto> RecentProjects { get; set; } = new();
}

/// <summary>
/// 项目摘要DTO
/// </summary>
public class ProjectSummaryDto
{
    /// <summary>
    /// 项目ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// 项目状态
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// 工人数
    /// </summary>
    public int WorkerCount { get; set; }

    /// <summary>
    /// 设备数
    /// </summary>
    public int DeviceCount { get; set; }

    /// <summary>
    /// 进度百分比
    /// </summary>
    public double Progress { get; set; }
}

/// <summary>
/// 项目概览DTO
/// </summary>
public class ProjectOverviewDto
{
    /// <summary>
    /// 项目ID
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; } = null!;

    /// <summary>
    /// 项目状态
    /// </summary>
    public string Status { get; set; } = null!;

    /// <summary>
    /// 开始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 计划结束日期
    /// </summary>
    public DateTime? PlannedEndDate { get; set; }

    /// <summary>
    /// 进行天数
    /// </summary>
    public int DaysInProgress { get; set; }

    /// <summary>
    /// 总工人数
    /// </summary>
    public int TotalWorkers { get; set; }

    /// <summary>
    /// 在场工人数
    /// </summary>
    public int PresentWorkers { get; set; }

    /// <summary>
    /// 班组数量
    /// </summary>
    public int TeamCount { get; set; }

    /// <summary>
    /// 设备数量
    /// </summary>
    public int DeviceCount { get; set; }

    /// <summary>
    /// 在线设备数量
    /// </summary>
    public int OnlineDeviceCount { get; set; }

    /// <summary>
    /// 未处理安全事件数量
    /// </summary>
    public int UnhandledSafetyIncidentCount { get; set; }
}

/// <summary>
/// 项目仪表盘DTO
/// </summary>
public class ProjectDashboardDto
{
    /// <summary>
    /// 项目概览
    /// </summary>
    public ProjectOverviewDto ProjectOverview { get; set; } = new();

    /// <summary>
    /// 考勤统计
    /// </summary>
    public List<AttendanceStatisticsDto> AttendanceStatistics { get; set; } = new();

    /// <summary>
    /// 安全统计
    /// </summary>
    public SafetyStatisticsDto SafetyStatistics { get; set; } = new();

    /// <summary>
    /// 设备统计
    /// </summary>
    public DeviceStatisticsDto DeviceStatistics { get; set; } = new();
}

/// <summary>
/// 设备统计DTO
/// </summary>
public class DeviceStatisticsDto
{
    /// <summary>
    /// 总设备数
    /// </summary>
    public int TotalDevices { get; set; }

    /// <summary>
    /// 在线设备数
    /// </summary>
    public int OnlineDevices { get; set; }

    /// <summary>
    /// 离线设备数
    /// </summary>
    public int OfflineDevices { get; set; }

    /// <summary>
    /// 故障设备数
    /// </summary>
    public int FaultDevices { get; set; }

    /// <summary>
    /// 维护中设备数
    /// </summary>
    public int MaintenanceDevices { get; set; }

    /// <summary>
    /// 按类型统计的设备
    /// </summary>
    public Dictionary<string, int> DevicesByType { get; set; } = new();
}
