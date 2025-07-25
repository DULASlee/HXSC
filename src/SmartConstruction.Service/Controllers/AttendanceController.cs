using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Attendance;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 考勤管理控制器
    /// </summary>
    [Route("api/attendances")]
    [ApiController]
    public class AttendanceController : BaseApiController
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ILogger<AttendanceController> _logger;

        public AttendanceController(IAttendanceService attendanceService, ILogger<AttendanceController> logger)
        {
            _attendanceService = attendanceService;
            _logger = logger;
        }

        /// <summary>
        /// 获取考勤记录列表
        /// </summary>
        /// <param name="workerId">工人ID</param>
        /// <param name="workerName">工人姓名</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="teamId">班组ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="attendanceType">考勤类型</param>
        /// <param name="isSynced">是否已同步</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetAttendances(
            [FromQuery] Guid? workerId,
            [FromQuery] string workerName,
            [FromQuery] Guid? projectId,
            [FromQuery] Guid? teamId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string attendanceType,
            [FromQuery] bool? isSynced,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var queryParams = new AttendanceQueryParams
                {
                    WorkerId = workerId,
                    WorkerName = workerName,
                    ProjectId = projectId,
                    TeamId = teamId,
                    StartDate = startDate,
                    EndDate = endDate,
                    AttendanceType = attendanceType,
                    IsSynced = isSynced,
                    PageIndex = pageIndex < 1 ? 1 : pageIndex,
                    PageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize)
                };

                var result = await _attendanceService.GetAttendanceRecordsAsync(queryParams);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取考勤记录列表时发生错误");
                return Error("获取考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <returns>考勤记录信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttendance(Guid id)
        {
            try
            {
                var attendance = await _attendanceService.GetAttendanceRecordByIdAsync(id);
               return Success(attendance);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取考勤记录(ID:{id})时发生错误");
                return Error("获取考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 创建考勤记录
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的考勤记录信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAttendance([FromBody] CreateAttendanceRequest request)
        {
            try
            {
                var attendance = await _attendanceService.CreateAttendanceRecordAsync(request);
               return Success(attendance);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建考勤记录时发生错误");
                return Error("创建考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的考勤记录信息</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAttendance(Guid id, [FromBody] UpdateAttendanceRequest request)
        {
            try
            {
                var attendance = await _attendanceService.UpdateAttendanceRecordAsync(id, request);
               return Success(attendance);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新考勤记录(ID:{id})时发生错误");
                return Error("更新考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAttendance(Guid id)
        {
            try
            {
                var result = await _attendanceService.DeleteAttendanceRecordAsync(id);
                if (result)
                {
                   return Success(null, "删除考勤记录成功");
                }
                return Error("未找到要删除的考勤记录", 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除考勤记录(ID:{id})时发生错误");
                return Error("删除考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取工人的考勤记录
        /// </summary>
        /// <param name="workerId">工人ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        [HttpGet("worker/{workerId}")]
        public async Task<IActionResult> GetWorkerAttendance(
            Guid workerId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _attendanceService.GetWorkerAttendanceAsync(workerId, startDate, endDate, pageIndex, pageSize);
               return Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取工人(ID:{workerId})的考勤记录时发生错误");
                return Error("获取考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取项目的考勤记录
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="date">日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetProjectAttendance(
            Guid projectId,
            [FromQuery] DateTime? date,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _attendanceService.GetProjectAttendanceAsync(projectId, date, pageIndex, pageSize);
               return Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})的考勤记录时发生错误");
                return Error("获取考勤记录失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取项目的考勤统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="date">日期</param>
        /// <returns>考勤统计</returns>
        [HttpGet("project/{projectId}/statistics")]
        public async Task<IActionResult> GetProjectAttendanceStatistics(
            Guid projectId,
            [FromQuery] DateTime? date)
        {
            try
            {
                var result = await _attendanceService.GetProjectAttendanceStatisticsAsync(projectId, date);
               return Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})的考勤统计时发生错误");
                return Error("获取考勤统计失败：" + ex.Message);
            }
        }
    }
} 
