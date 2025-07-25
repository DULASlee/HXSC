using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SmartConstruction.Service.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartConstruction.Service.Controllers;

/// <summary>
/// 数字孪生管理控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DigitalTwinController : ControllerBase
{
    private readonly ILogger<DigitalTwinController> _logger;

    public DigitalTwinController(ILogger<DigitalTwinController> logger)
    {
        _logger = logger;
    }

    #region 指挥中心API

    /// <summary>
    /// 获取工地总览数据
    /// </summary>
    [HttpGet("overview")]
    public async Task<ActionResult<ApiResponse<object>>> GetOverviewAsync()
    {
        try
        {
            var overview = new
            {
                SiteInfo = new
                {
                    Name = "智慧工地示范项目",
                    Location = "上海市浦东新区",
                    Area = "50000㎡",
                    Progress = 68.5,
                    StartDate = "2024-01-15",
                    ExpectedCompletion = "2025-12-31"
                },
                Statistics = new
                {
                    OnlineDevices = 48,
                    OnlinePersonnel = 156,
                    ActiveAlerts = 3,
                    TodayAttendance = 92.3,
                    EnvironmentIndex = 87.5
                },
                RecentAlerts = GenerateRecentAlerts(),
                WeatherInfo = new
                {
                    Temperature = 25,
                    Humidity = 65,
                    WindSpeed = 12,
                    WindDirection = "东南风",
                    Weather = "多云"
                }
            };

            return ApiResponseHelpers<object>.Success(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取工地总览数据失败");
            return ApiResponseHelpers<object>.Error("获取数据失败");
        }
    }

    /// <summary>
    /// 获取设备列表
    /// </summary>
    [HttpGet("devices")]
    public async Task<ActionResult<ApiResponse<object>>> GetDevicesAsync()
    {
        try
        {
            var devices = new object[]
            {
                new
                {
                    Id = "crane001",
                    Name = "1号塔吊",
                    Type = "crane",
                    Position = new { X = 30, Y = 0, Z = 30 },
                    Status = "运行中",
                    Metrics = new
                    {
                        Load = 65,
                        Height = 45,
                        Angle = 45,
                        Temperature = 28,
                        Vibration = 0.8
                    },
                    LastUpdate = DateTime.Now.AddMinutes(-2)
                },
                new
                {
                    Id = "crane002",
                    Name = "2号塔吊",
                    Type = "crane",
                    Position = new { X = -30, Y = 0, Z = -30 },
                    Status = "待机",
                    Metrics = new
                    {
                        Load = 0,
                        Height = 20,
                        Angle = 0,
                        Temperature = 25,
                        Vibration = 0.2
                    },
                    LastUpdate = DateTime.Now.AddMinutes(-1)
                },
                new
                {
                    Id = "elevator001",
                    Name = "1号升降机",
                    Type = "elevator",
                    Position = new { X = 50, Y = 0, Z = 0 },
                    Status = "运行中",
                    Metrics = new
                    {
                        Floor = 12,
                        Load = 80,
                        Speed = 2.5,
                        Temperature = 24,
                        PassengerCount = 8
                    },
                    LastUpdate = DateTime.Now.AddSeconds(-30)
                },
                new
                {
                    Id = "monitor001",
                    Name = "环境监测站",
                    Type = "monitor",
                    Position = new { X = 0, Y = 0, Z = 50 },
                    Status = "正常",
                    Metrics = new
                    {
                        Dust = 45,
                        Noise = 68,
                        Temperature = 25,
                        Humidity = 65,
                        AirQuality = 87
                    },
                    LastUpdate = DateTime.Now.AddSeconds(-10)
                }
            };

            return ApiResponseHelpers<object>.Success(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备列表失败");
            return ApiResponseHelpers<object>.Error("获取设备列表失败");
        }
    }

    /// <summary>
    /// 控制设备
    /// </summary>
    [HttpPost("devices/{deviceId}/control")]
    public async Task<ActionResult<ApiResponse<object>>> ControlDeviceAsync(
        string deviceId, 
        [FromBody] DeviceControlRequest request)
    {
        try
        {
            _logger.LogInformation($"设备控制请求: {deviceId}, 操作: {request.Action}");

            // 模拟设备控制逻辑
            await Task.Delay(500); // 模拟控制延迟

            var result = new
            {
                DeviceId = deviceId,
                Action = request.Action,
                Status = "执行成功",
                Timestamp = DateTime.Now,
                Parameters = request.Parameters
            };

            return ApiResponseHelpers<object>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"设备控制失败: {deviceId}");
            return ApiResponseHelpers<object>.Error("设备控制失败");
        }
    }

    #endregion

    #region 实名制考勤API

    /// <summary>
    /// 获取人员位置数据
    /// </summary>
    [HttpGet("attendance/positions")]
    public async Task<ActionResult<ApiResponse<object>>> GetPersonnelPositionsAsync([FromQuery] DateTime? date)
    {
        try
        {
            var targetDate = date ?? DateTime.Today;
            var personnel = GeneratePersonnelData(50);

            var result = new
            {
                Date = targetDate,
                TotalCount = personnel.Length,
                OnSiteCount = personnel.Cast<dynamic>().Count(p => ((dynamic)p).Status == "在场"),
                Personnel = personnel
            };

            return ApiResponseHelpers<object>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取人员位置数据失败");
            return ApiResponseHelpers<object>.Error("获取人员位置数据失败");
        }
    }

    /// <summary>
    /// 获取人员轨迹回放数据
    /// </summary>
    [HttpGet("attendance/trajectory/{userId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetPersonnelTrajectoryAsync(
        string userId, 
        [FromQuery] DateTime date)
    {
        try
        {
            var trajectory = GenerateTrajectoryData(userId, date);

            return ApiResponseHelpers<object>.Success(trajectory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"获取人员轨迹失败: {userId}");
            return ApiResponseHelpers<object>.Error("获取轨迹数据失败");
        }
    }

    #endregion

    #region 塔吊升降机监控API

    /// <summary>
    /// 获取塔吊实时状态
    /// </summary>
    [HttpGet("crane/status")]
    public async Task<ActionResult<ApiResponse<object>>> GetCraneStatusAsync()
    {
        try
        {
            var cranes = new object[]
            {
                new
                {
                    Id = "crane001",
                    Name = "1号塔吊",
                    Position = new { X = 30, Y = 0, Z = 30 },
                    Rotation = 135,
                    JibAngle = 15,
                    HookHeight = 45,
                    Load = 2.5,
                    MaxLoad = 8.0,
                    Status = "运行中",
                    WorkRadius = 50,
                    SafetyZone = new[]
                    {
                        new { X = 30, Z = 30, Radius = 55 }
                    },
                    LastUpdate = DateTime.Now
                },
                new
                {
                    Id = "crane002",
                    Name = "2号塔吊",
                    Position = new { X = -30, Y = 0, Z = -30 },
                    Rotation = 45,
                    JibAngle = 8,
                    HookHeight = 20,
                    Load = 1.2,
                    MaxLoad = 6.0,
                    Status = "待机",
                    WorkRadius = 45,
                    SafetyZone = new[]
                    {
                        new { X = -30, Z = -30, Radius = 50 }
                    },
                    LastUpdate = DateTime.Now
                }
            };

            return ApiResponseHelpers<object>.Success(cranes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取塔吊状态失败");
            return ApiResponseHelpers<object>.Error("获取塔吊状态失败");
        }
    }

    /// <summary>
    /// 控制塔吊动作
    /// </summary>
    [HttpPost("crane/{craneId}/control")]
    public async Task<ActionResult<ApiResponse<object>>> ControlCraneAsync(
        string craneId,
        [FromBody] CraneControlRequest request)
    {
        try
        {
            _logger.LogInformation($"塔吊控制: {craneId}, 动作: {request.Action}");

            // 模拟控制延迟
            await Task.Delay(200);

            var result = new
            {
                CraneId = craneId,
                Action = request.Action,
                Parameters = request.Parameters,
                Status = "执行成功",
                EstimatedDuration = request.Action switch
                {
                    "rotate" => 30,
                    "lift" => 45,
                    "extend" => 25,
                    _ => 10
                },
                Timestamp = DateTime.Now
            };

            return ApiResponseHelpers<object>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"塔吊控制失败: {craneId}");
            return ApiResponseHelpers<object>.Error("塔吊控制失败");
        }
    }

    #endregion

    #region 扬尘噪音管理API

    /// <summary>
    /// 获取环境监测数据
    /// </summary>
    [HttpGet("environment")]
    public async Task<ActionResult<ApiResponse<object>>> GetEnvironmentDataAsync()
    {
        try
        {
            var stations = new[]
            {
                new
                {
                    Id = "env001",
                    Name = "1号监测站",
                    Position = new { X = 0, Y = 0, Z = 50 },
                    Status = "online",
                    Metrics = new
                    {
                        PM25 = new { Value = 45, Unit = "μg/m³", Threshold = 75, Status = "正常" },
                        PM10 = new { Value = 82, Unit = "μg/m³", Threshold = 150, Status = "预警" },
                        Noise = new { Value = 68, Unit = "dB", Threshold = 70, Status = "预警" },
                        Temperature = new { Value = 25, Unit = "°C", Status = "正常" },
                        Humidity = new { Value = 65, Unit = "%", Status = "正常" },
                        WindSpeed = new { Value = 3.2, Unit = "m/s", Direction = "东南风", Status = "正常" },
                        AirQuality = new { Value = 87, Unit = "AQI", Status = "良好" }
                    },
                    IsExceeding = true,
                    ExceedingParameters = new[] { "PM10", "Noise" },
                    LastUpdate = DateTime.Now
                },
                new
                {
                    Id = "env002",
                    Name = "2号监测站",
                    Position = new { X = 80, Y = 0, Z = -20 },
                    Status = "online",
                    Metrics = new
                    {
                        PM25 = new { Value = 35, Unit = "μg/m³", Threshold = 75, Status = "正常" },
                        PM10 = new { Value = 65, Unit = "μg/m³", Threshold = 150, Status = "正常" },
                        Noise = new { Value = 55, Unit = "dB", Threshold = 70, Status = "正常" },
                        Temperature = new { Value = 26, Unit = "°C", Status = "正常" },
                        Humidity = new { Value = 62, Unit = "%", Status = "正常" },
                        WindSpeed = new { Value = 2.8, Unit = "m/s", Direction = "西北风", Status = "正常" },
                        AirQuality = new { Value = 92, Unit = "AQI", Status = "优秀" }
                    },
                    IsExceeding = false,
                    ExceedingParameters = new string[] { },
                    LastUpdate = DateTime.Now
                },
                new
                {
                    Id = "env003",
                    Name = "3号监测站",
                    Position = new { X = -60, Y = 0, Z = 30 },
                    Status = "online",
                    Metrics = new
                    {
                        PM25 = new { Value = 95, Unit = "μg/m³", Threshold = 75, Status = "严重超标" },
                        PM10 = new { Value = 145, Unit = "μg/m³", Threshold = 150, Status = "预警" },
                        Noise = new { Value = 75, Unit = "dB", Threshold = 70, Status = "超标" },
                        Temperature = new { Value = 24, Unit = "°C", Status = "正常" },
                        Humidity = new { Value = 70, Unit = "%", Status = "正常" },
                        WindSpeed = new { Value = 1.5, Unit = "m/s", Direction = "北风", Status = "正常" },
                        AirQuality = new { Value = 156, Unit = "AQI", Status = "中度污染" }
                    },
                    IsExceeding = true,
                    ExceedingParameters = new[] { "PM25", "PM10", "Noise" },
                    LastUpdate = DateTime.Now
                }
            };

            return ApiResponseHelpers<object>.Success(stations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取环境监测数据失败");
            return ApiResponseHelpers<object>.Error("获取环境监测数据失败");
        }
    }

    /// <summary>
    /// 计算污染扩散预测 - 高斯扩散模型
    /// </summary>
    [HttpPost("environment/diffusion/calculate")]
    public async Task<ActionResult<ApiResponse<object>>> CalculatePollutionDiffusionAsync(
        [FromBody] DiffusionCalculationRequest request)
    {
        try
        {
            _logger.LogInformation($"计算污染扩散: 模型={request.Model}, 稳定度={request.AtmosphericStability}");

            var predictions = CalculateGaussianDiffusion(request);

            var result = new
            {
                Model = request.Model,
                CalculationTime = DateTime.Now,
                WindField = new
                {
                    Speed = request.WindSpeed,
                    Direction = request.WindDirection,
                    Influence = request.WindInfluence
                },
                AtmosphericConditions = new
                {
                    Stability = request.AtmosphericStability,
                    Temperature = request.Temperature,
                    Humidity = request.Humidity
                },
                Predictions = predictions,
                Recommendations = GenerateDiffusionRecommendations(predictions)
            };

            return ApiResponseHelpers<object>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "计算污染扩散失败");
            return ApiResponseHelpers<object>.Error("扩散计算失败");
        }
    }

    /// <summary>
    /// 激活智能联动治理
    /// </summary>
    [HttpPost("environment/treatment/activate")]
    public async Task<ActionResult<ApiResponse<object>>> ActivateSmartTreatmentAsync(
        [FromBody] SmartTreatmentRequest request)
    {
        try
        {
            _logger.LogInformation($"激活智能治理: 污染源={request.PollutionSourceId}, 类型={request.PollutionType}");

            // 智能联动规则引擎
            var treatmentPlan = GenerateSmartTreatmentPlan(request);

            // 模拟设备激活
            dynamic dynamicTreatmentPlan = treatmentPlan;
            await SimulateDeviceActivation(dynamicTreatmentPlan.DeviceIds);

            var result = new
            {
                TreatmentId = Guid.NewGuid().ToString(),
                PollutionSource = request.PollutionSourceId,
                TreatmentPlan = treatmentPlan,
                ActivatedDevices = dynamicTreatmentPlan.DeviceIds.Length,
                EstimatedEffectiveness = CalculateTreatmentEffectiveness(request),
                StartTime = DateTime.Now,
                EstimatedDuration = dynamicTreatmentPlan.EstimatedDuration,
                MonitoringFrequency = "30秒/次",
                Status = "执行中"
            };

            return ApiResponseHelpers<object>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "激活智能治理失败");
            return ApiResponseHelpers<object>.Error("智能治理激活失败");
        }
    }

    /// <summary>
    /// 获取治理设备状态
    /// </summary>
    [HttpGet("environment/treatment/devices")]
    public async Task<ActionResult<ApiResponse<object>>> GetTreatmentDevicesAsync()
    {
        try
        {
            var devices = new object[]
            {
                new
                {
                    Id = "fogcannon001",
                    Name = "1号雾炮车",
                    Type = "雾炮车",
                    Position = new { X = 50, Y = 0, Z = 30 },
                    Status = "运行中",
                    Metrics = new
                    {
                        WaterPressure = 8.5, // Bar
                        SprayRadius = 80, // 米
                        WaterConsumption = 120, // L/min
                        PowerConsumption = 25.6, // kW
                        OperatingTime = 45 // 分钟
                    },
                    EffectiveRadius = 100,
                    LastMaintenance = DateTime.Now.AddDays(-5),
                    NextMaintenance = DateTime.Now.AddDays(25)
                },
                new
                {
                    Id = "sprinkler_a",
                    Name = "A区喷淋系统",
                    Type = "固定喷淋",
                    Position = new { X = 0, Y = 0, Z = 0 },
                    Status = "待机",
                    Metrics = new
                    {
                        WaterPressure = 6.0,
                        ActiveNozzles = 24,
                        TotalNozzles = 36,
                        FlowRate = 450, // L/min
                        CoverageArea = 2400 // m²
                    },
                    EffectiveRadius = 50,
                    LastActivation = DateTime.Now.AddHours(-3),
                    AutoActivationRules = new[]
                    {
                        "PM10 > 150μg/m³ 持续10分钟",
                        "风速 < 2m/s 且 PM2.5 > 75μg/m³"
                    }
                },
                new
                {
                    Id = "purifier001",
                    Name = "移动净化车",
                    Type = "移动净化",
                    Position = new { X = -40, Y = 0, Z = -20 },
                    Status = "充电中",
                    Metrics = new
                    {
                        BatteryLevel = 85, // %
                        FilterEfficiency = 99.7, // %
                        AirProcessingRate = 15000, // m³/h
                        MovementSpeed = 0, // km/h
                        ServiceTime = 320 // 小时
                    },
                    EffectiveRadius = 60,
                    ChargingTime = 45, // 分钟
                    FullChargeRange = 8 // 小时
                }
            };

            return ApiResponseHelpers<object>.Success(devices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取治理设备状态失败");
            return ApiResponseHelpers<object>.Error("获取设备状态失败");
        }
    }

    /// <summary>
    /// 开启环境数据实时流 - Server-Sent Events
    /// </summary>
    [HttpGet("environment/stream")]
    public async Task EnvironmentDataStreamAsync()
    {
        Response.Headers.Add("Content-Type", "text/event-stream");
        Response.Headers.Add("Cache-Control", "no-cache");
        Response.Headers.Add("Connection", "keep-alive");

        try
        {
            var cancellationToken = HttpContext.RequestAborted;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var environmentData = new
                {
                    Timestamp = DateTime.Now,
                    Stations = new[]
                    {
                        new
                        {
                            Id = "env001",
                            PM25 = 45 + (Random.Shared.NextDouble() - 0.5) * 10,
                            PM10 = 82 + (Random.Shared.NextDouble() - 0.5) * 15,
                            Noise = 68 + (Random.Shared.NextDouble() - 0.5) * 5,
                            WindSpeed = 3.2 + (Random.Shared.NextDouble() - 0.5) * 1,
                            Temperature = 25 + (Random.Shared.NextDouble() - 0.5) * 2
                        },
                        new
                        {
                            Id = "env002",
                            PM25 = 35 + (Random.Shared.NextDouble() - 0.5) * 8,
                            PM10 = 65 + (Random.Shared.NextDouble() - 0.5) * 12,
                            Noise = 55 + (Random.Shared.NextDouble() - 0.5) * 4,
                            WindSpeed = 2.8 + (Random.Shared.NextDouble() - 0.5) * 0.8,
                            Temperature = 26 + (Random.Shared.NextDouble() - 0.5) * 1.5
                        }
                    },
                    AirQualityIndex = 87 + (Random.Shared.NextDouble() - 0.5) * 10,
                    WeatherConditions = new
                    {
                        WindDirection = Random.Shared.Next(0, 360),
                        Pressure = 1013.25 + (Random.Shared.NextDouble() - 0.5) * 5,
                        Visibility = 10 + (Random.Shared.NextDouble() - 0.5) * 2
                    }
                };

                var json = JsonSerializer.Serialize(environmentData);
                await Response.WriteAsync($"data: {json}\n\n");
                await Response.Body.FlushAsync();

                // 每2秒发送一次数据
                await Task.Delay(2000, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("环境数据流连接已断开");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "环境数据流发送失败");
        }
    }

    #endregion

    #region 私有辅助方法

    /// <summary>
    /// 生成最近警报数据
    /// </summary>
    private object[] GenerateRecentAlerts()
    {
        return new object[]
        {
            new
            {
                Id = Guid.NewGuid().ToString(),
                Type = "环境超标",
                Level = "警告",
                Message = "3号监测站PM2.5浓度超标",
                Location = "施工区域C",
                Timestamp = DateTime.Now.AddMinutes(-5),
                Status = "处理中"
            },
            new
            {
                Id = Guid.NewGuid().ToString(),
                Type = "设备异常",
                Level = "一般",
                Message = "2号塔吊负载接近上限",
                Location = "起重作业区",
                Timestamp = DateTime.Now.AddMinutes(-12),
                Status = "已处理"
            },
            new
            {
                Id = Guid.NewGuid().ToString(),
                Type = "人员安全",
                Level = "紧急",
                Message = "施工人员进入危险区域",
                Location = "塔吊作业半径内",
                Timestamp = DateTime.Now.AddMinutes(-18),
                Status = "已解除"
            }
        };
    }

    /// <summary>
    /// 生成人员数据
    /// </summary>
    private object[] GeneratePersonnelData(int count)
    {
        var random = new Random();
        var personnel = new object[count];

        for (int i = 0; i < count; i++)
        {
            personnel[i] = new
            {
                Id = $"person_{i:D3}",
                Name = $"工人{i + 1:D2}",
                Position = new
                {
                    X = random.Next(-100, 100),
                    Y = 0,
                    Z = random.Next(-100, 100)
                },
                Status = random.NextDouble() > 0.15 ? "在场" : "离场",
                Department = random.NextDouble() > 0.5 ? "建筑施工" : random.NextDouble() > 0.5 ? "设备操作" : "安全管理",
                LastSeen = DateTime.Now.AddMinutes(-random.Next(0, 120)),
                SafetyLevel = random.NextDouble() > 0.1 ? "正常" : "预警"
            };
        }

        return personnel;
    }

    /// <summary>
    /// 生成轨迹数据
    /// </summary>
    private object GenerateTrajectoryData(string userId, DateTime date)
    {
        var random = new Random(userId.GetHashCode());
        var points = new List<object>();
        
        var startX = random.Next(-50, 50);
        var startZ = random.Next(-50, 50);
        var currentX = startX;
        var currentZ = startZ;

        // 生成一天的轨迹点 (每30分钟一个点)
        for (int hour = 8; hour < 18; hour++)
        {
            for (int minute = 0; minute < 60; minute += 30)
            {
                // 随机漫步
                currentX += random.Next(-10, 11);
                currentZ += random.Next(-10, 11);
                
                // 边界约束
                currentX = Math.Max(-100, Math.Min(100, currentX));
                currentZ = Math.Max(-100, Math.Min(100, currentZ));

                points.Add(new
                {
                    Timestamp = date.AddHours(hour).AddMinutes(minute),
                    Position = new { X = currentX, Y = 0, Z = currentZ },
                    Activity = GetRandomActivity(random),
                    HeartRate = random.Next(70, 120),
                    Temperature = 36.0 + random.NextDouble()
                });
            }
        }

        return new
        {
            UserId = userId,
            Date = date,
            TotalPoints = points.Count,
            TotalDistance = CalculateDistance(points),
            WorkingHours = 10,
            TrajectoryPoints = points
        };
    }

    /// <summary>
    /// 获取随机活动
    /// </summary>
    private string GetRandomActivity(Random random)
    {
        var activities = new[] { "行走", "作业", "休息", "检查", "运输", "清理" };
        return activities[random.Next(activities.Length)];
    }

    /// <summary>
    /// 计算距离
    /// </summary>
    private double CalculateDistance(List<object> points)
    {
        // 简化距离计算
        return points.Count * 15.5; // 假设平均每个点间距15.5米
    }

    /// <summary>
    /// 计算高斯扩散
    /// </summary>
    private object[] CalculateGaussianDiffusion(DiffusionCalculationRequest request)
    {
        var predictions = new List<object>();
        var stabilityCoefficients = GetStabilityCoefficients(request.AtmosphericStability);
        
        // 预测未来8小时的扩散情况
        for (int hour = 1; hour <= 8; hour++)
        {
            var time = hour * 3600; // 秒
            var windSpeed = Math.Max(0.1, request.WindSpeed); // 最小风速0.1m/s避免除零
            
            // 高斯扩散模型计算
            var sigmaY = stabilityCoefficients.SigmaY * Math.Pow(time, stabilityCoefficients.PowerY);
            var sigmaZ = stabilityCoefficients.SigmaZ * Math.Pow(time, stabilityCoefficients.PowerZ);
            
            // 扩散距离 (基于风速和时间)
            var diffusionDistance = windSpeed * time;
            
            // 浓度衰减 (简化计算)
            var concentrationRatio = Math.Exp(-diffusionDistance / (sigmaY * sigmaZ * 1000));
            var concentration = request.SourceConcentration * concentrationRatio;
            
            predictions.Add(new
            {
                Time = hour,
                TimeSeconds = time,
                DiffusionDistance = Math.Round(diffusionDistance, 1),
                ConcentrationRatio = Math.Round(concentrationRatio, 3),
                Concentration = Math.Round(concentration, 2),
                SigmaY = Math.Round(sigmaY, 1),
                SigmaZ = Math.Round(sigmaZ, 1),
                WindInfluence = request.WindInfluence,
                AffectedArea = Math.Round(Math.PI * sigmaY * sigmaZ, 0) // m²
            });
        }

        return predictions.ToArray();
    }

    /// <summary>
    /// 获取稳定度系数
    /// </summary>
    private (double SigmaY, double SigmaZ, double PowerY, double PowerZ) GetStabilityCoefficients(string stability)
    {
        return stability switch
        {
            "A" => (0.22, 0.20, 0.89, 0.92), // 极不稳定
            "B" => (0.16, 0.12, 0.86, 0.85), // 不稳定
            "C" => (0.11, 0.08, 0.78, 0.78), // 轻微不稳定
            "D" => (0.08, 0.06, 0.71, 0.71), // 中性
            "E" => (0.06, 0.03, 0.65, 0.61), // 轻微稳定
            "F" => (0.04, 0.016, 0.61, 0.52), // 稳定
            _ => (0.08, 0.06, 0.71, 0.71) // 默认中性
        };
    }

    /// <summary>
    /// 生成扩散建议
    /// </summary>
    private object[] GenerateDiffusionRecommendations(object[] predictions)
    {
        var recommendations = new List<object>();
        
        foreach (dynamic prediction in predictions)
        {
            if (prediction.Concentration > 50) // 浓度阈值
            {
                recommendations.Add(new
                {
                    Time = prediction.Time,
                    Priority = prediction.Concentration > 100 ? "高" : "中",
                    Action = prediction.Concentration > 100 
                        ? "立即启动雾炮车和喷淋系统" 
                        : "准备治理设备，密切监控",
                    EstimatedEffect = "降低污染浓度60-80%",
                    Duration = "30-60分钟"
                });
            }
        }

        if (recommendations.Count == 0)
        {
            recommendations.Add(new
            {
                Time = 0,
                Priority = "低",
                Action = "继续监控，暂无需治理",
                EstimatedEffect = "维持当前环境质量",
                Duration = "持续监控"
            });
        }

        return recommendations.ToArray();
    }

    /// <summary>
    /// 生成智能治理方案
    /// </summary>
    private object GenerateSmartTreatmentPlan(SmartTreatmentRequest request)
    {
        var deviceIds = new List<string>();
        var steps = new List<object>();
        var estimatedDuration = 0;

        // 根据污染类型和严重程度选择设备
        if (request.PollutionType.Contains("PM") || request.PollutionType.Contains("扬尘"))
        {
            if (request.Severity >= 80) // 严重污染
            {
                deviceIds.AddRange(new[] { "fogcannon001", "sprinkler_a", "sprinkler_b", "purifier001" });
                estimatedDuration = 60;
                steps.Add(new
                {
                    Step = 1,
                    Action = "紧急启动所有雾炮车",
                    Duration = 5,
                    Description = "立即部署现场所有雾炮车到污染源周围"
                });
                steps.Add(new
                {
                    Step = 2,
                    Action = "激活全区域喷淋系统",
                    Duration = 10,
                    Description = "开启污染区域及周边的所有喷淋装置"
                });
                steps.Add(new
                {
                    Step = 3,
                    Action = "调度移动净化设备",
                    Duration = 15,
                    Description = "派遣移动净化车到重点污染区域"
                });
            }
            else if (request.Severity >= 50) // 中等污染
            {
                deviceIds.AddRange(new[] { "fogcannon001", "sprinkler_a" });
                estimatedDuration = 30;
                steps.Add(new
                {
                    Step = 1,
                    Action = "启动雾炮车",
                    Duration = 5,
                    Description = "部署最近的雾炮车到污染源"
                });
                steps.Add(new
                {
                    Step = 2,
                    Action = "开启喷淋系统",
                    Duration = 10,
                    Description = "激活污染区域的固定喷淋装置"
                });
            }
            else // 轻微污染
            {
                deviceIds.Add("sprinkler_a");
                estimatedDuration = 15;
                steps.Add(new
                {
                    Step = 1,
                    Action = "局部喷淋降尘",
                    Duration = 15,
                    Description = "开启污染源附近的喷淋系统"
                });
            }
        }

        if (request.PollutionType.Contains("噪音"))
        {
            // 噪音治理主要是设备控制，不需要物理治理设备
            steps.Add(new
            {
                Step = steps.Count + 1,
                Action = "降低施工噪音",
                Duration = 5,
                Description = "通知相关设备降低运行速度或暂停高噪音作业"
            });
        }

        return new
        {
            DeviceIds = deviceIds.ToArray(),
            TotalDevices = deviceIds.Count,
            EstimatedDuration = estimatedDuration,
            Steps = steps.ToArray(),
            TreatmentStrategy = GetTreatmentStrategy(request.Severity),
            MonitoringPoints = GetMonitoringPoints(request.PollutionSourceId),
            SuccessCriteria = new
            {
                PM25Reduction = "降至75μg/m³以下",
                PM10Reduction = "降至150μg/m³以下",
                NoiseReduction = "降至70dB以下",
                VisibilityImprovement = "能见度提升至8km以上"
            }
        };
    }

    /// <summary>
    /// 获取治理策略
    /// </summary>
    private string GetTreatmentStrategy(double severity)
    {
        return severity switch
        {
            >= 80 => "紧急全面治理策略",
            >= 50 => "重点区域治理策略",
            >= 30 => "预防性治理策略",
            _ => "日常监控策略"
        };
    }

    /// <summary>
    /// 获取监控点位
    /// </summary>
    private string[] GetMonitoringPoints(string sourceId)
    {
        return new[]
        {
            $"{sourceId}_上风向_50m",
            $"{sourceId}_下风向_100m",
            $"{sourceId}_下风向_200m",
            $"{sourceId}_侧风向_100m"
        };
    }

    /// <summary>
    /// 模拟设备激活
    /// </summary>
    private async Task SimulateDeviceActivation(string[] deviceIds)
    {
        foreach (var deviceId in deviceIds)
        {
            _logger.LogInformation($"激活治理设备: {deviceId}");
            await Task.Delay(100); // 模拟设备启动延迟
        }
    }

    /// <summary>
    /// 计算治理效果
    /// </summary>
    private object CalculateTreatmentEffectiveness(SmartTreatmentRequest request)
    {
        var baseEffectiveness = request.Severity switch
        {
            >= 80 => 0.75, // 严重污染治理效果75%
            >= 50 => 0.85, // 中等污染治理效果85%
            _ => 0.95 // 轻微污染治理效果95%
        };

        // 考虑风速影响
        var windFactor = Math.Min(1.2, 1.0 + request.WindSpeed / 10.0);
        var finalEffectiveness = Math.Min(0.98, baseEffectiveness * windFactor);

        return new
        {
            BaseEffectiveness = Math.Round(baseEffectiveness * 100, 1),
            WindFactor = Math.Round(windFactor, 2),
            FinalEffectiveness = Math.Round(finalEffectiveness * 100, 1),
            ExpectedReduction = new
            {
                PM25 = Math.Round(request.Severity * finalEffectiveness, 1),
                PM10 = Math.Round(request.Severity * finalEffectiveness * 0.8, 1),
                Noise = request.PollutionType.Contains("噪音") 
                    ? Math.Round(request.Severity * 0.6, 1) 
                    : 0
            },
            ConfidenceLevel = Math.Round(finalEffectiveness * 100, 0)
        };
    }

    #endregion

    #region 请求模型

    /// <summary>
    /// 设备控制请求
    /// </summary>
    public class DeviceControlRequest
    {
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// 塔吊控制请求
    /// </summary>
    public class CraneControlRequest
    {
        public string Action { get; set; } = string.Empty;
        public Dictionary<string, object> Parameters { get; set; } = new();
    }

    /// <summary>
    /// 扩散计算请求
    /// </summary>
    public class DiffusionCalculationRequest
    {
        public string Model { get; set; } = "gaussian";
        public string AtmosphericStability { get; set; } = "D";
        public double WindSpeed { get; set; } = 3.0;
        public double WindDirection { get; set; } = 90.0;
        public double WindInfluence { get; set; } = 0.6;
        public double Temperature { get; set; } = 25.0;
        public double Humidity { get; set; } = 65.0;
        public double SourceConcentration { get; set; } = 100.0;
        public double SourceHeight { get; set; } = 5.0;
    }

    /// <summary>
    /// 智能治理请求
    /// </summary>
    public class SmartTreatmentRequest
    {
        public string PollutionSourceId { get; set; } = string.Empty;
        public string PollutionType { get; set; } = string.Empty;
        public double Severity { get; set; } = 0;
        public double WindSpeed { get; set; } = 3.0;
        public double AffectedRadius { get; set; } = 100.0;
        public string TriggerCondition { get; set; } = string.Empty;
    }

    #endregion
}