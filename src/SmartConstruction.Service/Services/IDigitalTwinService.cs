using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 数字孪生服务接口
    /// </summary>
    public interface IDigitalTwinService
    {
        #region 指挥中心大屏

        /// <summary>
        /// 获取指挥中心总览数据
        /// </summary>
        /// <param name="projectId">项目ID（可选）</param>
        /// <returns>指挥中心总览数据</returns>
        Task<ApiResponse<object>> GetCommandCenterOverviewAsync(string? projectId = null);

        /// <summary>
        /// 获取项目列表及状态
        /// </summary>
        /// <returns>项目列表数据</returns>
        Task<ApiResponse<object>> GetProjectListAsync();

        /// <summary>
        /// 获取实时数据统计
        /// </summary>
        /// <returns>实时统计数据</returns>
        Task<ApiResponse<object>> GetRealtimeStatsAsync();

        /// <summary>
        /// 获取趋势图表数据
        /// </summary>
        /// <param name="type">图表类型</param>
        /// <param name="timeRange">时间范围</param>
        /// <returns>趋势图表数据</returns>
        Task<ApiResponse<object>> GetTrendsAsync(string type, string timeRange);

        #endregion

        #region 项目考勤大屏

        /// <summary>
        /// 获取考勤总览统计
        /// </summary>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="date">日期（可选）</param>
        /// <returns>考勤总览数据</returns>
        Task<ApiResponse<object>> GetAttendanceOverviewAsync(string? projectId = null, string? date = null);

        /// <summary>
        /// 获取实时考勤动态
        /// </summary>
        /// <returns>实时考勤数据</returns>
        Task<ApiResponse<object>> GetAttendanceRealtimeAsync();

        /// <summary>
        /// 获取班组考勤排行
        /// </summary>
        /// <returns>班组考勤排行数据</returns>
        Task<ApiResponse<object>> GetTeamRankingAsync();

        /// <summary>
        /// 获取考勤趋势图表
        /// </summary>
        /// <param name="timeRange">时间范围</param>
        /// <param name="chartType">图表类型</param>
        /// <returns>考勤趋势数据</returns>
        Task<ApiResponse<object>> GetAttendanceTrendsAsync(string timeRange, string? chartType = null);

        #endregion

        #region 视频监控大屏

        /// <summary>
        /// 获取监控点位信息
        /// </summary>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="status">状态筛选（可选）</param>
        /// <returns>监控点位数据</returns>
        Task<ApiResponse<object>> GetVideoCamerasAsync(string? projectId = null, string? status = null);

        /// <summary>
        /// 获取视频监控统计
        /// </summary>
        /// <returns>视频监控统计数据</returns>
        Task<ApiResponse<object>> GetVideoStatisticsAsync();

        /// <summary>
        /// 获取智能分析结果
        /// </summary>
        /// <returns>AI智能分析数据</returns>
        Task<ApiResponse<object>> GetAiAnalysisAsync();

        #endregion

        #region 塔吊升降机管理大屏

        /// <summary>
        /// 获取设备列表及状态
        /// </summary>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="deviceType">设备类型（可选）</param>
        /// <returns>设备列表数据</returns>
        Task<ApiResponse<object>> GetCraneElevatorDevicesAsync(string? projectId = null, string? deviceType = null);

        /// <summary>
        /// 获取设备运行统计
        /// </summary>
        /// <returns>设备运行统计数据</returns>
        Task<ApiResponse<object>> GetCraneElevatorStatisticsAsync();

        /// <summary>
        /// 获取安全监控数据
        /// </summary>
        /// <returns>安全监控数据</returns>
        Task<ApiResponse<object>> GetSafetyMonitoringAsync();

        /// <summary>
        /// 获取工作效率分析
        /// </summary>
        /// <returns>工作效率分析数据</returns>
        Task<ApiResponse<object>> GetEfficiencyAnalysisAsync();

        #endregion

        #region 扬尘噪音监测大屏

        /// <summary>
        /// 获取环境监测数据
        /// </summary>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="monitorType">监测类型（可选）</param>
        /// <returns>环境监测数据</returns>
        Task<ApiResponse<object>> GetEnvironmentMonitoringAsync(string? projectId = null, string? monitorType = null);

        /// <summary>
        /// 获取监测点位信息
        /// </summary>
        /// <returns>监测点位数据</returns>
        Task<ApiResponse<object>> GetMonitoringPointsAsync();

        /// <summary>
        /// 获取环境趋势分析
        /// </summary>
        /// <param name="timeRange">时间范围</param>
        /// <param name="dataType">数据类型</param>
        /// <returns>环境趋势数据</returns>
        Task<ApiResponse<object>> GetEnvironmentTrendsAsync(string timeRange, string? dataType = null);

        /// <summary>
        /// 获取环境告警信息
        /// </summary>
        /// <returns>环境告警数据</returns>
        Task<ApiResponse<object>> GetEnvironmentAlertsAsync();

        #endregion

        #region 通用工具接口

        /// <summary>
        /// 获取项目基础信息
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>项目基础信息</returns>
        Task<ApiResponse<object>> GetProjectInfoAsync(string projectId);

        /// <summary>
        /// 获取系统状态
        /// </summary>
        /// <returns>系统状态信息</returns>
        Task<ApiResponse<object>> GetSystemStatusAsync();

        /// <summary>
        /// 获取实时时间
        /// </summary>
        /// <returns>当前时间</returns>
        Task<ApiResponse<object>> GetCurrentTimeAsync();

        #endregion
    }
}