using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Integration;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Hubs
{
    /// <summary>
    /// IoT数据Hub，用于实时推送IoT设备数据
    /// </summary>
    [Authorize]
    public class IoTDataHub : Hub
    {
        private readonly ILogger<IoTDataHub> _logger;

        public IoTDataHub(ILogger<IoTDataHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 客户端连接事件
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// 客户端断开连接事件
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}, Exception: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 订阅项目数据
        /// </summary>
        public async Task SubscribeToProject(Guid projectId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"project-{projectId}");
            _logger.LogInformation($"Client {Context.ConnectionId} subscribed to project {projectId}");
        }

        /// <summary>
        /// 取消订阅项目数据
        /// </summary>
        public async Task UnsubscribeFromProject(Guid projectId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project-{projectId}");
            _logger.LogInformation($"Client {Context.ConnectionId} unsubscribed from project {projectId}");
        }

        /// <summary>
        /// 订阅设备数据
        /// </summary>
        public async Task SubscribeToDevice(string deviceId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"device-{deviceId}");
            _logger.LogInformation($"Client {Context.ConnectionId} subscribed to device {deviceId}");
        }

        /// <summary>
        /// 取消订阅设备数据
        /// </summary>
        public async Task UnsubscribeFromDevice(string deviceId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"device-{deviceId}");
            _logger.LogInformation($"Client {Context.ConnectionId} unsubscribed from device {deviceId}");
        }

        /// <summary>
        /// 发送IoT实时数据
        /// </summary>
        public async Task SendIoTData(IoTRealtimeDataViewModel data)
        {
            await Clients.Group($"device-{data.DeviceId}").SendAsync("IoTDataReceived", data);
            await Clients.Group($"project-{data.ProjectId}").SendAsync("IoTDataReceived", data);
            _logger.LogDebug($"IoT data sent for device {data.DeviceId}");
        }

        /// <summary>
        /// 发送IoT告警
        /// </summary>
        public async Task SendIoTAlert(IoTAlertViewModel alert)
        {
            await Clients.Group($"device-{alert.DeviceId}").SendAsync("IoTAlert", alert);
            await Clients.Group($"project-{alert.ProjectId}").SendAsync("IoTAlert", alert);
            _logger.LogWarning($"IoT alert sent for device {alert.DeviceId}: {alert.AlertType} - {alert.Content}");
        }

        /// <summary>
        /// 发送设备状态变更
        /// </summary>
        public async Task SendDeviceStatusChange(string deviceId, string deviceName, string status)
        {
            await Clients.Group($"device-{deviceId}").SendAsync("DeviceStatusChanged", new { DeviceId = deviceId, DeviceName = deviceName, Status = status });
            _logger.LogInformation($"Device status change sent for device {deviceId}: {status}");
        }

        /// <summary>
        /// 发送全局通知
        /// </summary>
        public async Task SendGlobalNotification(string title, string message)
        {
            await Clients.All.SendAsync("Notification", title, message);
            _logger.LogInformation($"Global notification sent: {title}");
        }
    }
} 