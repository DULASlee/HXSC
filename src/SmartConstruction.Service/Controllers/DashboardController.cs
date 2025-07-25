using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Service.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SmartConstruction.Service.Models; // 引入ApiResponse

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 仪表盘控制器
    /// </summary>
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase // 不再继承 BaseApiController
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        /// <summary>
        /// 获取项目概览
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>项目概览</returns>
        [HttpGet("projects/{projectId}/overview")]
        public async Task<IActionResult> GetProjectOverview(Guid projectId)
        {
            try
            {
                var result = await _dashboardService.GetProjectOverviewAsync(projectId);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return StatusCode(404, ApiResponse<object>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})概览时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取项目概览失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 获取考勤统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>考勤统计</returns>
        [HttpGet("projects/{projectId}/attendance")]
        public async Task<IActionResult> GetAttendanceStatistics(
            Guid projectId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _dashboardService.GetAttendanceStatisticsAsync(projectId, startDate, endDate);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return StatusCode(404, ApiResponse<object>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})考勤统计时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取考勤统计失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 获取安全事件统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>安全事件统计</returns>
        [HttpGet("projects/{projectId}/safety")]
        public async Task<IActionResult> GetSafetyStatistics(
            Guid projectId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _dashboardService.GetSafetyStatisticsAsync(projectId, startDate, endDate);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return StatusCode(404, ApiResponse<object>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})安全事件统计时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取安全事件统计失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 获取设备状态统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>设备状态统计</returns>
        [HttpGet("projects/{projectId}/devices")]
        public async Task<IActionResult> GetDeviceStatusStatistics(Guid projectId)
        {
            try
            {
                var result = await _dashboardService.GetDeviceStatusStatisticsAsync(projectId);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return StatusCode(404, ApiResponse<object>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})设备状态统计时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取设备状态统计失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 获取项目完整仪表盘数据
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>项目仪表盘数据</returns>
        [HttpGet("projects/{projectId}")]
        public async Task<IActionResult> GetProjectDashboard(
            Guid projectId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _dashboardService.GetProjectDashboardAsync(projectId, startDate, endDate);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return StatusCode(404, ApiResponse<object>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})仪表盘数据时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取仪表盘数据失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 获取公司整体仪表盘数据
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>公司仪表盘数据</returns>
        [HttpGet("companies/{companyId}")]
        public async Task<IActionResult> GetCompanyDashboard(
            Guid companyId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var result = await _dashboardService.GetCompanyDashboardAsync(companyId, startDate, endDate);
                return Ok(ApiResponse<object>.Success(result));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return StatusCode(404, ApiResponse<object>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取公司(ID:{companyId})仪表盘数据时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取仪表盘数据失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 获取全局统计信息
        /// </summary>
        /// <returns>全局仪表盘统计数据</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var result = await _dashboardService.GetGlobalStatsAsync();
                return Ok(ApiResponse<object>.Success(result)); // 使用 ApiResponse
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取全局统计信息时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取统计信息失败：" + ex.Message)); // 使用 ApiResponse
            }
        }

        /// <summary>
        /// 获取最近活动
        /// </summary>
        /// <param name="pageSize">条目数</param>
        /// <returns>最近活动列表</returns>
        [HttpGet("activities")]
        public async Task<IActionResult> GetRecentActivities([FromQuery] int pageSize = 5)
        {
            try
            {
                // 注意：这里需要一个真正的活动日志服务，暂时返回模拟数据
                var activities = new List<object>
                {
                    new { id = 1, title = "用户 admin 登录成功", time = DateTime.UtcNow.AddMinutes(-5), type = "login" },
                    new { id = 2, title = "项目 '智慧工地A区' 更新了进度", time = DateTime.UtcNow.AddMinutes(-15), type = "project_update" },
                    new { id = 3, title = "设备 'T001' 状态变为离线", time = DateTime.UtcNow.AddHours(-1), type = "device_status" },
                    new { id = 4, title = "安全警报: 塔吊区域有人闯入", time = DateTime.UtcNow.AddHours(-2), type = "security_alert" },
                    new { id = 5, title = "用户 zhangsan 提交了考勤记录", time = DateTime.UtcNow.AddHours(-3), type = "attendance" }
                };
                await Task.Delay(50); // 模拟异步操作
                return Ok(ApiResponse<object>.Success(activities.Take(pageSize))); // 使用 ApiResponse
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取最近活动时发生错误");
                return StatusCode(500, ApiResponse<object>.Failure("获取最近活动失败：" + ex.Message)); // 使用 ApiResponse
            }
        }
    }
} 