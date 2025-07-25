using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SmartConstruction.Contracts.Dtos.Integration;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Hubs;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 集成控制器，用于处理IoT数据集成
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationController : BaseApiController
    {
        private readonly IHubContext<IoTDataHub> _hubContext;
        private readonly ILogger<IntegrationController> _logger;

        public IntegrationController(
            IHubContext<IoTDataHub> hubContext,
            ILogger<IntegrationController> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        /// <summary>
        /// 接收IoT设备数据
        /// </summary>
        /// <param name="data">IoT数据</param>
        /// <returns>处理结果</returns>
        [HttpPost("iot-data")]
        [AllowAnonymous] // 允许匿名访问，通常IoT设备使用API密钥认证
        public async Task<IActionResult> ReceiveIoTData([FromBody] IoTDataDto data)
        {
            try
            {
                _logger.LogInformation($"Received IoT data from device {data.DeviceId}");

                // 转换为前端视图模型
                var viewModel = new IoTRealtimeDataViewModel
                {
                    DeviceId = data.DeviceId,
                    DeviceName = data.DeviceName,
                    DeviceType = data.DeviceType,
                    Timestamp = data.Timestamp,
                    Data = data.Data,
                    Status = data.Status
                };

                // 通过SignalR推送数据
                await _hubContext.Clients.Group($"device-{data.DeviceId}").SendAsync("IoTDataReceived", viewModel);
                await _hubContext.Clients.Group($"project-{data.ProjectId}").SendAsync("IoTDataReceived", viewModel);

                return Success(new { Received = true, DeviceId = data.DeviceId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing IoT data from device {data.DeviceId}");
                return Error("处理IoT数据时发生错误", 500);
            }
        }

        /// <summary>
        /// 接收IoT设备告警
        /// </summary>
        /// <param name="alert">告警数据</param>
        /// <returns>处理结果</returns>
        [HttpPost("iot-alert")]
        [AllowAnonymous] // 允许匿名访问，通常IoT设备使用API密钥认证
        public async Task<IActionResult> ReceiveIoTAlert([FromBody] IoTAlertDto alert)
        {
            try
            {
                _logger.LogWarning($"Received IoT alert from device {alert.DeviceId}: {alert.AlertType} - {alert.Content}");

                // 转换为前端视图模型
                var viewModel = new IoTAlertViewModel
                {
                    AlertId = alert.AlertId,
                    DeviceId = alert.DeviceId,
                    DeviceName = alert.DeviceName,
                    AlertType = alert.AlertType,
                    Level = alert.Level,
                    Content = alert.Content,
                    AlertTime = alert.AlertTime
                };

                // 通过SignalR推送告警
                await _hubContext.Clients.Group($"device-{alert.DeviceId}").SendAsync("IoTAlert", viewModel);
                await _hubContext.Clients.Group($"project-{alert.DeviceId}").SendAsync("IoTAlert", viewModel);

                return Success(new { Received = true, AlertId = alert.AlertId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing IoT alert from device {alert.DeviceId}");
                return Error("处理IoT告警时发生错误", 500);
            }
        }

        /// <summary>
        /// 发送设备状态变更
        /// </summary>
        /// <param name="deviceId">设备ID</param>
        /// <param name="status">设备状态</param>
        /// <returns>处理结果</returns>
        [HttpPost("device-status")]
        [AllowAnonymous] // 允许匿名访问，通常IoT设备使用API密钥认证
        public async Task<IActionResult> UpdateDeviceStatus([FromQuery] string deviceId, [FromQuery] string deviceName, [FromQuery] string status)
        {
            try
            {
                _logger.LogInformation($"Updating status for device {deviceId} to {status}");

                // 通过SignalR推送状态变更
                await _hubContext.Clients.Group($"device-{deviceId}").SendAsync("DeviceStatusChanged", new { DeviceId = deviceId, DeviceName = deviceName, Status = status });

                return Success(new { Updated = true, DeviceId = deviceId, Status = status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating status for device {deviceId}");
                return Error("更新设备状态时发生错误", 500);
            }
        }
    }
} 