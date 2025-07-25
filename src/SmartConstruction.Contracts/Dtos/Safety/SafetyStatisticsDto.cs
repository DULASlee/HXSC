using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Dtos.Safety;

/// <summary>
/// 安全统计DTO
/// </summary>
public class SafetyStatisticsDto
{
    /// <summary>
    /// 安全事故总数
    /// </summary>
    public int TotalIncidents { get; set; }

    /// <summary>
    /// 已处理事故数
    /// </summary>
    public int HandledIncidents { get; set; }

    /// <summary>
    /// 未处理事故数
    /// </summary>
    public int UnhandledIncidents { get; set; }

    /// <summary>
    /// 按级别统计
    /// </summary>
    public Dictionary<string, int> IncidentsByLevel { get; set; } = new();

    /// <summary>
    /// 按类型统计
    /// </summary>
    public Dictionary<string, int> IncidentsByType { get; set; } = new();

    /// <summary>
    /// 近30天事故趋势
    /// </summary>
    public List<DailyIncidentCountDto> Last30DaysTrend { get; set; } = new();
}

/// <summary>
/// 每日事故数量DTO
/// </summary>
public class DailyIncidentCountDto
{
    /// <summary>
    /// 日期
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// 事故数量
    /// </summary>
    public int Count { get; set; }
}
