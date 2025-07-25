using System.Collections.Generic;

namespace SmartConstruction.Contracts.Dtos.Dashboard;

/// <summary>
/// 设备状态统计DTO
/// </summary>
public class DeviceStatusStatisticsDto
{
    /// <summary>
    /// 在线设备数
    /// </summary>
    public int OnlineCount { get; set; }

    /// <summary>
    /// 离线设备数
    /// </summary>
    public int OfflineCount { get; set; }

    /// <summary>
    /// 故障设备数
    /// </summary>
    public int FaultCount { get; set; }

    /// <summary>
    /// 维护中设备数
    /// </summary>
    public int MaintenanceCount { get; set; }

    /// <summary>
    /// 设备总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 按类型统计
    /// </summary>
    public Dictionary<string, int> CountByType { get; set; } = new();

    /// <summary>
    /// 按状态统计
    /// </summary>
    public Dictionary<string, int> CountByStatus { get; set; } = new();
}