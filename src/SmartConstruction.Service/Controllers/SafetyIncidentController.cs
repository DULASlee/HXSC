using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Safety;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 安全事件管理控制器
    /// </summary>
    [Route("api/safety-incidents")]
    [ApiController]
    public class SafetyIncidentController : BaseApiController
    {
        private readonly ISafetyIncidentService _safetyIncidentService;
        private readonly ILogger<SafetyIncidentController> _logger;

        public SafetyIncidentController(ISafetyIncidentService safetyIncidentService, ILogger<SafetyIncidentController> logger)
        {
            _safetyIncidentService = safetyIncidentService;
            _logger = logger;
        }

        /// <summary>
        /// 获取安全事件分页列表
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetSafetyIncidents(
            [FromQuery] Guid? projectId = null,
            [FromQuery] string? type = null,
            [FromQuery] string? level = null,
            [FromQuery] bool? isHandled = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _safetyIncidentService.GetPagedListAsync(projectId, type, level, isHandled, pageIndex, pageSize);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取安全事件列表时发生错误");
                return Error("获取安全事件列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取安全事件详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSafetyIncidentById(Guid id)
        {
            try
            {
                var result = await _safetyIncidentService.GetByIdAsync(id);
                if (result == null)
                {
                    return NotFound("未找到指定的安全事件");
                }
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取安全事件详情时发生错误, ID: {Id}", id);
                return Error("获取安全事件详情失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 创建安全事件
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSafetyIncident([FromBody] CreateSafetyIncidentDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _safetyIncidentService.CreateAsync(createDto);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建安全事件时发生错误");
                return Error("创建安全事件失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新安全事件
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSafetyIncident(Guid id, [FromBody] UpdateSafetyIncidentDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _safetyIncidentService.UpdateAsync(id, updateDto);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新安全事件时发生错误, ID: {Id}", id);
                return Error("更新安全事件失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除安全事件
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSafetyIncident(Guid id)
        {
            try
            {
                var result = await _safetyIncidentService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound("未找到指定的安全事件");
                }
                return Success("删除成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除安全事件时发生错误, ID: {Id}", id);
                return Error("删除安全事件失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 处理安全事件
        /// </summary>
        [HttpPost("{id}/handle")]
        public async Task<IActionResult> HandleIncident(Guid id, [FromBody] HandleIncidentRequest request)
        {
            try
            {
                var result = await _safetyIncidentService.HandleIncidentAsync(id, request.HandledBy, request.HandlingResult);
                if (!result)
                {
                    return NotFound("未找到指定的安全事件");
                }
                return Success("处理成功");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理安全事件时发生错误, ID: {Id}", id);
                return Error("处理安全事件失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取安全统计信息
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] Guid? projectId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var result = await _safetyIncidentService.GetStatisticsAsync(projectId, startDate, endDate);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取安全统计信息时发生错误");
                return Error("获取安全统计信息失败：" + ex.Message);
            }
        }
    }

    /// <summary>
    /// 处理事件请求DTO
    /// </summary>
    public class HandleIncidentRequest
    {
        public string HandledBy { get; set; } = string.Empty;
        public string HandlingResult { get; set; } = string.Empty;
    }
}