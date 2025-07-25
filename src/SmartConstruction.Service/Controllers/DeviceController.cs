using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Device;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 设备管理控制器
    /// </summary>
    [Route("api/devices")]
    [ApiController]
    public class DeviceController : BaseApiController
    {
        private readonly IDeviceService _deviceService;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger)
        {
            _deviceService = deviceService;
            _logger = logger;
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="deviceName">设备名称</param>
        /// <param name="projectId">项目ID</param>
        /// <param name="deviceType">设备类型</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>设备列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetDevices(
            [FromQuery] string deviceCode,
            [FromQuery] string deviceName,
            [FromQuery] Guid? projectId,
            [FromQuery] string deviceType,
            [FromQuery] string status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var queryParams = new DeviceQueryParams
                {
                    DeviceCode = deviceCode,
                    DeviceName = deviceName,
                    ProjectId = projectId,
                    DeviceType = deviceType,
                    Status = status,
                    PageIndex = pageIndex < 1 ? 1 : pageIndex,
                    PageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize)
                };

                var result = await _deviceService.GetDevicesAsync(queryParams);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备列表时发生错误");
                return Error("获取设备列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>设备信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDevice(Guid id)
        {
            try
            {
                var device = await _deviceService.GetDeviceByIdAsync(id);
                return Success(device);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取设备(ID:{id})时发生错误");
                return Error("获取设备信息失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据设备编号获取设备
        /// </summary>
        /// <param name="code">设备编号</param>
        /// <returns>设备信息</returns>
        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetDeviceByCode(string code)
        {
            try
            {
                var device = await _deviceService.GetDeviceByCodeAsync(code);
                return Success(device);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取设备(编号:{code})时发生错误");
                return Error("获取设备信息失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的设备信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request)
        {
            try
            {
                var device = await _deviceService.CreateDeviceAsync(request);
                return Success(device);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建设备时发生错误");
                return Error("创建设备失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的设备信息</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice(Guid id, [FromBody] UpdateDeviceRequest request)
        {
            try
            {
                var device = await _deviceService.UpdateDeviceAsync(id, request);
                return Success(device);
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
                _logger.LogError(ex, $"更新设备(ID:{id})时发生错误");
                return Error("更新设备失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice(Guid id)
        {
            try
            {
                var result = await _deviceService.DeleteDeviceAsync(id);
                if (result)
                {
                    return Success(null, "删除设备成功");
                }
                return Error("未找到要删除的设备", 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除设备(ID:{id})时发生错误");
                return Error("删除设备失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 检查设备编号是否已存在
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="excludeId">排除的设备ID</param>
        /// <returns>检查结果</returns>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckDeviceCode([FromQuery] string deviceCode, [FromQuery] Guid? excludeId = null)
        {
            try
            {
                var exists = await _deviceService.IsDeviceCodeExistsAsync(deviceCode, excludeId);
                return Success(new { Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查设备编号时发生错误");
                return Error("检查设备编号失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取项目下的设备列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>设备列表</returns>
        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetDevicesByProject(
            Guid projectId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _deviceService.GetDevicesByProjectAsync(projectId, pageIndex, pageSize);
                return Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})的设备列表时发生错误");
                return Error("获取设备列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="status">状态</param>
        /// <returns>更新后的设备信息</returns>
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateDeviceStatus(Guid id, [FromBody] string status)
        {
            try
            {
                if (string.IsNullOrEmpty(status))
                {
                    return Error("状态不能为空", 400);
                }

                var device = await _deviceService.UpdateDeviceStatusAsync(id, status);
                return Success(device);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新设备(ID:{id})状态时发生错误");
                return Error("更新设备状态失败：" + ex.Message);
            }
        }
    }
} 