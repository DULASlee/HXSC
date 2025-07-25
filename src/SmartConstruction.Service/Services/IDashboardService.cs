using SmartConstruction.Contracts.Dtos.Dashboard;
using SmartConstruction.Contracts.Dtos.Safety;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 仪表盘服务接口
    /// </summary>
    public interface IDashboardService
    {
        /// <summary>
        /// 获取项目概览
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>项目概览DTO</returns>
        Task<ProjectOverviewDto> GetProjectOverviewAsync(Guid projectId);

        /// <summary>
        /// 获取考勤统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>考勤统计DTO</returns>
        Task<AttendanceStatisticsDto> GetAttendanceStatisticsAsync(Guid projectId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// 获取安全事件统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>安全事件统计DTO</returns>
        Task<SafetyStatisticsDto> GetSafetyStatisticsAsync(Guid projectId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// 获取设备状态统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>设备状态统计DTO</returns>
        Task<DeviceStatusStatisticsDto> GetDeviceStatusStatisticsAsync(Guid projectId);

        /// <summary>
        /// 获取项目完整仪表盘数据
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>项目仪表盘DTO</returns>
        Task<ProjectDashboardDto> GetProjectDashboardAsync(Guid projectId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// 获取公司整体仪表盘数据
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>公司仪表盘DTO</returns>
        Task<CompanyDashboardDto> GetCompanyDashboardAsync(Guid companyId, DateTime? startDate, DateTime? endDate);

        /// <summary>
        /// 获取全局统计信息
        /// </summary>
        /// <returns>全局统计DTO</returns>
        Task<GlobalStatsDto> GetGlobalStatsAsync();
    }
}
