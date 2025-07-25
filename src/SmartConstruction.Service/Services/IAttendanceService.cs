using SmartConstruction.Contracts.Dtos.Attendance;
using SmartConstruction.Contracts.Dtos.Base;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 考勤服务接口
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// 获取考勤记录列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        Task<PagedResult<AttendanceDto>> GetAttendanceRecordsAsync(AttendanceQueryParams queryParams);

        /// <summary>
        /// 根据ID获取考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <returns>考勤记录DTO</returns>
        Task<AttendanceDto> GetAttendanceRecordByIdAsync(Guid id);

        /// <summary>
        /// 创建考勤记录
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的考勤记录DTO</returns>
        Task<AttendanceDto> CreateAttendanceRecordAsync(CreateAttendanceRequest request);

        /// <summary>
        /// 更新考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的考勤记录DTO</returns>
        Task<AttendanceDto> UpdateAttendanceRecordAsync(Guid id, UpdateAttendanceRequest request);

        /// <summary>
        /// 删除考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAttendanceRecordAsync(Guid id);

        /// <summary>
        /// 获取工人的考勤记录
        /// </summary>
        /// <param name="workerId">工人ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        Task<PagedResult<AttendanceDto>> GetWorkerAttendanceAsync(Guid workerId, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 获取项目的考勤记录
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="date">日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        Task<PagedResult<AttendanceDto>> GetProjectAttendanceAsync(Guid projectId, DateTime? date, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 获取项目的考勤统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="date">日期</param>
        /// <returns>考勤统计</returns>
        Task<AttendanceStatisticsDto> GetProjectAttendanceStatisticsAsync(Guid projectId, DateTime? date);
    }
}
