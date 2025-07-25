using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Data; // 添加 using

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 数字孪生API控制器 - 专用于数字孪生大屏的核心API
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize]
    public class DigitalTwinApiController : BaseApiController
    {
        private readonly SmartConstructionDbContext _context;
        private readonly Random _random = new();

        public DigitalTwinApiController(SmartConstructionDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取设备实时状态数据
        /// </summary>
        /// <returns>设备实时状态</returns>
        [HttpGet("device/real-time-status")]
        public async Task<IActionResult> GetDeviceRealTimeStatus()
        {
            try
            {
                var devices = await _context.Devices
                    .Include(d => d.Project)
                    .Where(d => d.Status == "Running" || d.Status == "Online")
                    .Select(d => new
                    {
                        DeviceId = d.DeviceCode,
                        DeviceName = d.DeviceName,
                        DeviceType = d.DeviceType,
                        Status = d.Status,
                        Location = d.Location,
                        ProjectName = d.Project.ProjectName,
                        LastOnlineTime = d.LastOnlineTime,
                        // 模拟实时数据
                        RealTimeData = new
                        {
                            Temperature = Math.Round(25.0 + _random.NextDouble() * 10, 1),
                            Humidity = Math.Round(60.0 + _random.NextDouble() * 20, 1),
                            Load = d.DeviceType == "TowerCrane" ? 2000 + _random.Next(0, 5000) :
                                   d.DeviceType == "Elevator" ? 500 + _random.Next(0, 1500) :
                                   _random.Next(0, 100),
                            Power = Math.Round(75.0 + _random.NextDouble() * 25, 1),
                            WorkingHours = Math.Round(_random.NextDouble() * 12, 1)
                        }
                    })
                    .ToListAsync();

                var summary = new
                {
                    TotalDevices = await _context.Devices.CountAsync(),
                    OnlineDevices = devices.Count,
                    OfflineDevices = await _context.Devices.CountAsync(d => d.Status == "Offline"),
                    OnlineRate = devices.Count > 0 ? Math.Round(devices.Count * 100.0 / await _context.Devices.CountAsync(), 1) : 0,
                    LastUpdateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                var result = new
                {
                    Summary = summary,
                    Devices = devices
                };

                return Ok(ApiResponseHelpers<object>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelpers<object>.Error($"获取设备状态失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 获取环境监测数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>环境监测数据</returns>
        [HttpGet("environment/monitoring")]
        public async Task<IActionResult> GetEnvironmentMonitoring([FromQuery] DateTime? startTime = null, [FromQuery] DateTime? endTime = null)
        {
            try
            {
                startTime ??= DateTime.Now.AddHours(-24);
                endTime ??= DateTime.Now;

                // 获取环境监测设备
                var environmentDevices = await _context.Devices
                    .Where(d => d.DeviceType == "EnvironmentSensor" && d.Status == "Online")
                    .Include(d => d.Project)
                    .ToListAsync();

                var monitoringData = environmentDevices.Select(d => new
                {
                    DeviceId = d.DeviceCode,
                    DeviceName = d.DeviceName,
                    Location = d.Location,
                    ProjectName = d.Project.ProjectName,
                    Data = new
                    {
                        PM25 = Math.Round(35.0 + _random.NextDouble() * 40, 1),
                        PM10 = Math.Round(55.0 + _random.NextDouble() * 60, 1),
                        Temperature = Math.Round(20.0 + _random.NextDouble() * 15, 1),
                        Humidity = Math.Round(45.0 + _random.NextDouble() * 30, 1),
                        NoiseLevel = Math.Round(55.0 + _random.NextDouble() * 20, 1),
                        WindSpeed = Math.Round(_random.NextDouble() * 15, 1),
                        AirPressure = Math.Round(1013.0 + _random.NextDouble() * 30 - 15, 1)
                    },
                    Status = "Normal",
                    LastUpdateTime = DateTime.Now.AddMinutes(-_random.Next(0, 30)).ToString("yyyy-MM-ddTHH:mm:ssZ")
                }).ToList();

                var summary = new
                {
                    AverageAQI = Math.Round(85.0 + _random.NextDouble() * 30, 0),
                    AirQuality = "良",
                    TotalMonitoringPoints = environmentDevices.Count,
                    OnlinePoints = environmentDevices.Count(d => d.Status == "Online"),
                    AlertCount = _random.Next(0, 3)
                };

                var result = new
                {
                    Summary = summary,
                    MonitoringData = monitoringData,
                    TimeRange = new { StartTime = startTime, EndTime = endTime }
                };

                return Ok(ApiResponseHelpers<object>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelpers<object>.Error($"获取环境监测数据失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 获取塔吊升降机状态
        /// </summary>
        /// <returns>塔吊升降机状态数据</returns>
        [HttpGet("crane-elevator/status")]
        public async Task<IActionResult> GetCraneElevatorStatus()
        {
            try
            {
                var craneElevators = await _context.Devices
                    .Where(d => (d.DeviceType == "TowerCrane" || d.DeviceType == "Elevator") && 
                               (d.Status == "Running" || d.Status == "Idle"))
                    .Include(d => d.Project)
                    .Select(d => new
                    {
                        DeviceId = d.DeviceCode,
                        DeviceName = d.DeviceName,
                        DeviceType = d.DeviceType,
                        Status = d.Status,
                        Location = d.Location,
                        Model = d.Model ?? "Unknown",
                        ProjectName = d.Project.ProjectName,
                        RealTimeData = new
                        {
                            Load = d.DeviceType == "TowerCrane" ? 2500 + _random.Next(-1000, 3000) : 800 + _random.Next(-400, 1000),
                            MaxLoad = d.DeviceType == "TowerCrane" ? 8000 : 2000,
                            Height = Math.Round(d.DeviceType == "TowerCrane" ? 45.0 + _random.NextDouble() * 20 : 25.0 + _random.NextDouble() * 30, 1),
                            MaxHeight = d.DeviceType == "TowerCrane" ? 60 : 100,
                            WindSpeed = Math.Round(5.0 + _random.NextDouble() * 10, 1),
                            MaxWindSpeed = 15.0,
                            Rotation = d.DeviceType == "TowerCrane" ? _random.Next(0, 360) : 0,
                            WorkingHours = Math.Round(_random.NextDouble() * 12, 1)
                        },
                        SafetyStatus = new
                        {
                            Overload = _random.NextDouble() < 0.05,
                            OverHeight = _random.NextDouble() < 0.02,
                            HighWind = _random.NextDouble() < 0.1,
                            SafetyScore = 85 + _random.Next(0, 15),
                            LastInspection = DateTime.Now.AddDays(-_random.Next(1, 30)).ToString("yyyy-MM-dd")
                        }
                    })
                    .ToListAsync();

                var statistics = new
                {
                    TotalDevices = craneElevators.Count,
                    RunningDevices = craneElevators.Count(d => d.Status == "Running"),
                    IdleDevices = craneElevators.Count(d => d.Status == "Idle"),
                    TowerCranes = craneElevators.Count(d => d.DeviceType == "TowerCrane"),
                    Elevators = craneElevators.Count(d => d.DeviceType == "Elevator"),
                    UtilizationRate = Math.Round(75.0 + _random.NextDouble() * 20, 1),
                    SafetyAlerts = _random.Next(0, 3)
                };

                var result = new
                {
                    Statistics = statistics,
                    Devices = craneElevators
                };

                return Ok(ApiResponseHelpers<object>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelpers<object>.Error($"获取塔吊升降机状态失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 创建安全警报
        /// </summary>
        /// <param name="request">警报请求</param>
        /// <returns>创建结果</returns>
        [HttpPost("alerts")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertRequest request)
        {
            try
            {
                // 验证项目是否存在
                var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id.ToString() == request.ProjectId);
                if (project == null)
                {
                    return BadRequest(ApiResponseHelpers<object>.Error("指定的项目不存在"));
                }

                // 创建安全事故记录
                var safetyIncident = new SafetyIncident
                {
                    ProjectId = project.Id,
                    Type = request.AlertType,
                    Level = request.Level,
                    Location = request.Location ?? "未指定位置",
                    Description = request.Description,
                    DetectedTime = DateTime.Now,
                    IsHandled = false,
                    CreatedAt = DateTime.Now
                };

                _context.SafetyIncidents.Add(safetyIncident);
                await _context.SaveChangesAsync();

                var result = new
                {
                    AlertId = safetyIncident.Id,
                    Message = "安全警报创建成功",
                    CreatedAt = safetyIncident.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")
                };

                return Ok(ApiResponseHelpers<object>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelpers<object>.Error($"创建安全警报失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 获取摄像头监控数据
        /// </summary>
        /// <param name="projectId">项目ID（可选）</param>
        /// <param name="status">状态筛选（可选）</param>
        /// <returns>摄像头监控数据</returns>
        [HttpGet("camera-feeds")]
        public async Task<IActionResult> GetCameraFeeds([FromQuery] string? projectId = null, [FromQuery] string? status = null)
        {
            try
            {
                IQueryable<Device> query = _context.Devices
                    .Where(d => d.DeviceType == "Camera")
                    .Include(d => d.Project);

                if (!string.IsNullOrEmpty(projectId) && Guid.TryParse(projectId, out var projId))
                {
                    query = query.Where(d => d.ProjectId == projId);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(d => d.Status == status);
                }

                var cameras = await query
                    .Select(d => new
                    {
                        CameraId = d.DeviceCode,
                        CameraName = d.DeviceName,
                        Location = d.Location,
                        ProjectName = d.Project.ProjectName,
                        Status = d.Status,
                        StreamUrl = $"rtmp://example.com/live/{d.DeviceCode.ToLower()}",
                        Resolution = "1920x1080",
                        Fps = 25,
                        RecordingStatus = d.Status == "Online" ? "Recording" : "Stopped",
                        LastActivity = d.LastOnlineTime.HasValue ? d.LastOnlineTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ") : null,
                        // AI分析数据
                        AiAnalysis = new
                        {
                            PersonCount = _random.Next(0, 15),
                            VehicleCount = _random.Next(0, 5),
                            HelmetCompliance = Math.Round(85.0 + _random.NextDouble() * 15, 1),
                            SafetyAlerts = _random.Next(0, 3),
                            LastAnalysisTime = DateTime.Now.AddMinutes(-_random.Next(0, 30)).ToString("yyyy-MM-ddTHH:mm:ssZ")
                        }
                    })
                    .ToListAsync();

                var summary = new
                {
                    TotalCameras = await _context.Devices.CountAsync(d => d.DeviceType == "Camera"),
                    OnlineCameras = cameras.Count(c => c.Status == "Online"),
                    OfflineCameras = cameras.Count(c => c.Status == "Offline"),
                    RecordingCameras = cameras.Count(c => c.RecordingStatus == "Recording"),
                    OnlineRate = cameras.Count() > 0 ? Math.Round(cameras.Count(c => c.Status == "Online") * 100.0 / cameras.Count(), 1) : 0
                };

                var result = new
                {
                    Summary = summary,
                    Cameras = cameras
                };

                return Ok(ApiResponseHelpers<object>.Success(result));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponseHelpers<object>.Error($"获取摄像头监控数据失败: {ex.Message}"));
            }
        }
    }

    /// <summary>
    /// 创建警报请求模型
    /// </summary>
    public class CreateAlertRequest
    {
        public string ProjectId { get; set; } = string.Empty;
        public string AlertType { get; set; } = string.Empty;
        public string Level { get; set; } = "Warning";
        public string? Location { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}