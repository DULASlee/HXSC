using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Services.Base;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Infrastructure.UnitOfWork; // 添加 using

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 数字孪生服务实现
    /// </summary>
    public class DigitalTwinService : IDigitalTwinService
    {
        private readonly IDigitalTwinUnitOfWork _unitOfWork;
        private readonly Random _random = new();

        public DigitalTwinService(IDigitalTwinUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region 指挥中心大屏

        public async Task<ApiResponse<object>> GetCommandCenterOverviewAsync(string? projectId = null)
        {
            // 使用真实的仓储来获取数据，而不是模拟数据
            var deviceCount = await _unitOfWork.DeviceRepository.CountAsync();
            var onlineDeviceCount = await _unitOfWork.DeviceRepository.CountAsync(d => d.Status == "ONLINE");

            var data = new
            {
                ProjectSummary = new
                {
                    TotalProjects = 15, // 暂时保留模拟数据
                    ActiveProjects = 12,
                    CompletedProjects = 3,
                    TotalInvestment = 2800000000L
                },
                PersonnelSummary = new
                {
                    TotalPersonnel = 1850, // 暂时保留模拟数据
                    OnSitePersonnel = 1245 + _random.Next(-50, 50),
                    OffSitePersonnel = 605,
                    AttendanceRate = Math.Round(92.5 + _random.NextDouble() * 2 - 1, 1)
                },
                EquipmentSummary = new
                {
                    TotalEquipment = deviceCount, // 使用真实数据
                    OnlineEquipment = onlineDeviceCount, // 使用真实数据
                    OfflineEquipment = deviceCount - onlineDeviceCount, // 计算得出
                    FaultEquipment = await _unitOfWork.DeviceRepository.CountAsync(d => d.Status == "FAULT"), // 使用真实数据
                    OnlineRate = deviceCount > 0 ? Math.Round((double)onlineDeviceCount / deviceCount * 100, 1) : 0
                },
                SafetySummary = new
                {
                    TotalIncidents = await _unitOfWork.SafetyIncidentRepository.CountAsync(), // 使用真实数据
                    PendingIncidents = await _unitOfWork.SafetyIncidentRepository.CountAsync(s => !s.IsHandled), // 使用真实数据
                    ResolvedIncidents = await _unitOfWork.SafetyIncidentRepository.CountAsync(s => s.IsHandled), // 使用真实数据
                    SafetyScore = Math.Round(89.2 + _random.NextDouble() * 6 - 3, 1)
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetProjectListAsync()
        {
            await Task.Delay(80);

            var projects = new[]
            {
                new
                {
                    Id = "proj001",
                    Name = "智慧工地示范项目A区",
                    Status = "Active",
                    Progress = Math.Round(65.8 + _random.NextDouble() * 5 - 2.5, 1),
                    Location = new
                    {
                        Lng = 116.397459,
                        Lat = 39.909042,
                        Address = "北京市朝阳区建国路88号"
                    },
                    Personnel = new
                    {
                        Total = 245,
                        OnSite = 189 + _random.Next(-20, 20)
                    },
                    Equipment = new
                    {
                        Total = 32,
                        Online = 28 + _random.Next(-3, 3)
                    },
                    StartDate = "2024-01-15",
                    EndDate = "2024-12-31",
                    Investment = 150000000L
                },
                new
                {
                    Id = "proj002",
                    Name = "智慧工地示范项目B区",
                    Status = "Active",
                    Progress = Math.Round(78.3 + _random.NextDouble() * 5 - 2.5, 1),
                    Location = new
                    {
                        Lng = 116.407459,
                        Lat = 39.919042,
                        Address = "北京市朝阳区东三环北路38号"
                    },
                    Personnel = new
                    {
                        Total = 298,
                        OnSite = 256 + _random.Next(-30, 30)
                    },
                    Equipment = new
                    {
                        Total = 41,
                        Online = 38 + _random.Next(-4, 4)
                    },
                    StartDate = "2023-11-20",
                    EndDate = "2024-10-31",
                    Investment = 220000000L
                },
                new
                {
                    Id = "proj003",
                    Name = "智慧工地示范项目C区",
                    Status = "Completed",
                    Progress = 100.0,
                    Location = new
                    {
                        Lng = 116.387459,
                        Lat = 39.899042,
                        Address = "北京市朝阳区CBD核心区"
                    },
                    Personnel = new
                    {
                        Total = 0,
                        OnSite = 0
                    },
                    Equipment = new
                    {
                        Total = 0,
                        Online = 0
                    },
                    StartDate = "2023-06-01",
                    EndDate = "2024-05-30",
                    Investment = 180000000L
                }
            };

            return ApiResponseHelpers<object>.Success(projects);
        }

        public async Task<ApiResponse<object>> GetRealtimeStatsAsync()
        {
            await Task.Delay(50);

            var data = new
            {
                AttendanceToday = 1245 + _random.Next(-20, 20),
                EquipmentOnline = 234 + _random.Next(-10, 10),
                SafetyAlerts = _random.Next(0, 6),
                EnvironmentAlerts = _random.Next(0, 4),
                LastUpdateTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetTrendsAsync(string type, string timeRange)
        {
            await Task.Delay(120);

            var dates = GenerateDateRange(timeRange);
            var data = new
            {
                Attendance = new
                {
                    Dates = dates,
                    Values = dates.Select(_ => 1150 + _random.Next(0, 200)).ToArray()
                },
                Equipment = new
                {
                    Dates = dates,
                    Online = dates.Select(_ => 220 + _random.Next(0, 30)).ToArray(),
                    Offline = dates.Select(_ => 20 + _random.Next(0, 20)).ToArray()
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        #endregion

        #region 项目考勤大屏

        public async Task<ApiResponse<object>> GetAttendanceOverviewAsync(string? projectId = null, string? date = null)
        {
            await Task.Delay(100);

            var data = new
            {
                TodayAttendance = new
                {
                    TotalWorkers = 1245,
                    CheckedIn = 1156 + _random.Next(-30, 30),
                    CheckedOut = 856 + _random.Next(-50, 50),
                    Absent = 89 + _random.Next(-10, 10),
                    Late = 23 + _random.Next(-5, 5),
                    EarlyLeave = 12 + _random.Next(-3, 3),
                    AttendanceRate = Math.Round(92.8 + _random.NextDouble() * 4 - 2, 1)
                },
                MonthlyStats = new
                {
                    AverageAttendance = 1189,
                    AverageAttendanceRate = Math.Round(91.2 + _random.NextDouble() * 3 - 1.5, 1),
                    TotalWorkDays = 21,
                    TotalAbsent = 1876
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetAttendanceRealtimeAsync()
        {
            await Task.Delay(50);

            var workers = new[] { "张三", "李四", "王五", "赵六", "孙七", "周八", "吴九", "郑十" };
            var teams = new[] { "钢筋班组A", "混凝土班组B", "钢结构班组C", "装修班组D", "电工班组E" };
            var locations = new[] { "东门", "西门", "南门", "北门" };
            var statuses = new[] { "Normal", "Late", "Early" };

            var recentCheckins = Enumerable.Range(0, 5).Select(i => new
            {
                WorkerId = $"W{(i + 1):D3}",
                WorkerName = workers[_random.Next(workers.Length)],
                TeamName = teams[_random.Next(teams.Length)],
                CheckTime = DateTime.Now.AddMinutes(-i * _random.Next(1, 30)).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Location = locations[_random.Next(locations.Length)],
                Status = statuses[_random.Next(statuses.Length)]
            }).ToArray();

            var data = new
            {
                RecentCheckins = recentCheckins,
                CurrentStats = new
                {
                    CheckedInNow = 1156 + _random.Next(-20, 20),
                    ExpectedTotal = 1245,
                    LateCount = 23 + _random.Next(-5, 5)
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetTeamRankingAsync()
        {
            await Task.Delay(80);

            var teams = new[]
            {
                new
                {
                    TeamId = "T001",
                    TeamName = "钢筋班组A",
                    AttendanceRate = Math.Round(98.5 + _random.NextDouble() * 2 - 1, 1),
                    TotalWorkers = 45,
                    PresentWorkers = 44,
                    Rank = 1
                },
                new
                {
                    TeamId = "T002",
                    TeamName = "混凝土班组B",
                    AttendanceRate = Math.Round(96.8 + _random.NextDouble() * 2 - 1, 1),
                    TotalWorkers = 52,
                    PresentWorkers = 50,
                    Rank = 2
                },
                new
                {
                    TeamId = "T003",
                    TeamName = "钢结构班组C",
                    AttendanceRate = Math.Round(94.2 + _random.NextDouble() * 2 - 1, 1),
                    TotalWorkers = 38,
                    PresentWorkers = 36,
                    Rank = 3
                },
                new
                {
                    TeamId = "T004",
                    TeamName = "装修班组D",
                    AttendanceRate = Math.Round(92.1 + _random.NextDouble() * 2 - 1, 1),
                    TotalWorkers = 29,
                    PresentWorkers = 27,
                    Rank = 4
                },
                new
                {
                    TeamId = "T005",
                    TeamName = "电工班组E",
                    AttendanceRate = Math.Round(89.5 + _random.NextDouble() * 2 - 1, 1),
                    TotalWorkers = 19,
                    PresentWorkers = 17,
                    Rank = 5
                }
            };

            return ApiResponseHelpers<object>.Success(teams);
        }

        public async Task<ApiResponse<object>> GetAttendanceTrendsAsync(string timeRange, string? chartType = null)
        {
            await Task.Delay(120);

            var dates = GenerateDateRange(timeRange);
            var data = new
            {
                DailyTrend = new
                {
                    Dates = dates,
                    Attendance = dates.Select(_ => 1100 + _random.Next(0, 200)).ToArray(),
                    AttendanceRate = dates.Select(_ => Math.Round(88 + _random.NextDouble() * 8, 1)).ToArray()
                },
                HourlyDistribution = new
                {
                    Hours = new[] { "6:00", "7:00", "8:00", "9:00", "10:00", "11:00", "12:00" },
                    Checkins = new[] { 45, 234, 567, 289, 61, 23, 12 }.Select(x => x + _random.Next(-20, 20)).ToArray()
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        #endregion

        #region 视频监控大屏

        public async Task<ApiResponse<object>> GetVideoCamerasAsync(string? projectId = null, string? status = null)
        {
            await Task.Delay(100);

            var cameras = new[]
            {
                new
                {
                    Id = "CAM001",
                    Name = "东门入口监控",
                    Location = new
                    {
                        Area = "入口区域",
                        Position = "东门"
                    },
                    Status = _random.NextDouble() > 0.1 ? "Online" : "Offline",
                    StreamUrl = "rtmp://example.com/live/cam001",
                    Resolution = "1920x1080",
                    Fps = 25,
                    RecordingStatus = "Recording",
                    LastActivity = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                new
                {
                    Id = "CAM002",
                    Name = "西门出口监控",
                    Location = new
                    {
                        Area = "出口区域",
                        Position = "西门"
                    },
                    Status = _random.NextDouble() > 0.1 ? "Online" : "Offline",
                    StreamUrl = "rtmp://example.com/live/cam002",
                    Resolution = "1920x1080",
                    Fps = 25,
                    RecordingStatus = "Recording",
                    LastActivity = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                new
                {
                    Id = "CAM003",
                    Name = "A区施工监控",
                    Location = new
                    {
                        Area = "施工区域",
                        Position = "A区中央"
                    },
                    Status = _random.NextDouble() > 0.1 ? "Online" : "Offline",
                    StreamUrl = "rtmp://example.com/live/cam003",
                    Resolution = "1920x1080",
                    Fps = 30,
                    RecordingStatus = "Recording",
                    LastActivity = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };

            return ApiResponseHelpers<object>.Success(cameras);
        }

        public async Task<ApiResponse<object>> GetVideoStatisticsAsync()
        {
            await Task.Delay(80);

            var data = new
            {
                CameraSummary = new
                {
                    TotalCameras = 45,
                    OnlineCameras = 42 + _random.Next(-2, 2),
                    OfflineCameras = 2,
                    FaultCameras = 1,
                    OnlineRate = Math.Round(93.3 + _random.NextDouble() * 4 - 2, 1)
                },
                RecordingSummary = new
                {
                    TotalStorage = "8.5TB",
                    UsedStorage = "6.2TB",
                    UsageRate = Math.Round(72.9 + _random.NextDouble() * 10 - 5, 1),
                    RetentionDays = 30
                },
                AlertSummary = new
                {
                    TodayAlerts = 12 + _random.Next(-5, 5),
                    UnresolvedAlerts = 3 + _random.Next(-1, 2),
                    AlertTypes = new
                    {
                        Motion = 8 + _random.Next(-3, 3),
                        Intrusion = 2 + _random.Next(-1, 2),
                        Equipment = 2 + _random.Next(-1, 2)
                    }
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetAiAnalysisAsync()
        {
            await Task.Delay(90);

            var data = new
            {
                PersonDetection = new
                {
                    TotalPersons = 234 + _random.Next(-20, 20),
                    HelmetDetection = new
                    {
                        WithHelmets = 221 + _random.Next(-10, 10),
                        WithoutHelmets = 13 + _random.Next(-3, 5),
                        ComplianceRate = Math.Round(94.4 + _random.NextDouble() * 4 - 2, 1)
                    },
                    UnauthorizedAreas = _random.Next(0, 5)
                },
                VehicleDetection = new
                {
                    TotalVehicles = 23 + _random.Next(-5, 5),
                    VehicleTypes = new
                    {
                        Truck = 12 + _random.Next(-3, 3),
                        Car = 8 + _random.Next(-2, 2),
                        Crane = 3 + _random.Next(-1, 1)
                    }
                },
                BehaviorAnalysis = new
                {
                    SmokingDetection = _random.Next(0, 3),
                    FightingDetection = _random.Next(0, 1),
                    CrowdGathering = _random.Next(0, 2)
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        #endregion

        #region 塔吊升降机管理大屏

        //public async Task<ApiResponse<object>> GetCraneElevatorDevicesAsync(string? projectId = null, string? deviceType = null)
        //{
        //    await Task.Delay(120);

        //    var devices = new[]
        //    {
        //        new
        //        {
        //            Id = "TC001",
        //            Name = "1号塔吊",
        //            Type = "TowerCrane",
        //            Model = "QTZ80",
        //            Status = "Running",
        //            Location = new
        //            {
        //                Zone = "A区",
        //                Coordinates = new[] { 116.397459, 39.909042 }
        //            },
        //            RealTimeData = new
        //            {
        //                Load = 2500 + _random.Next(-500, 500),
        //                MaxLoad = 8000,
        //                Height = Math.Round(45.6 + _random.NextDouble() * 10 - 5, 1),
        //                MaxHeight = 60,
        //                WindSpeed = Math.Round(8.2 + _random.NextDouble() * 6 - 3, 1),
        //                MaxWindSpeed = 15,
        //                Rotation = _random.Next(0, 360),
        //                WorkingHours = Math.Round(8.5 + _random.NextDouble() * 2 - 1, 1)
        //            },
        //            SafetyStatus = new
        //            {
        //                Overload = false,
        //                OverHeight = false,
        //                HighWind = false,
        //                Collision = false,
        //                SafetyScore = 95 + _random.Next(-5, 5)
        //            }
        //        },
        //        new
        //        {
        //            Id = "TC002",
        //            Name = "2号塔吊",
        //            Type = "TowerCrane",
        //            Model = "QTZ63",
        //            Status = "Running",
        //            Location = new
        //            {
        //                Zone = "B区",
        //                Coordinates = new[] { 116.407459, 39.919042 }
        //            },
        //            RealTimeData = new
        //            {
        //                Load = 3200 + _random.Next(-500, 500),
        //                MaxLoad = 6300,
        //                Height = Math.Round(38.2 + _random.NextDouble() * 8 - 4, 1),
        //                MaxHeight = 50,
        //                WindSpeed = Math.Round(7.8 + _random.NextDouble() * 5 - 2.5, 1),
        //                MaxWindSpeed = 15,
        //                Rotation = _random.Next(0, 360),
        //                WorkingHours = Math.Round(7.2 + _random.NextDouble() * 2 - 1, 1)
        //            },
        //            SafetyStatus = new
        //            {
        //                Overload = false,
        //                OverHeight = false,
        //                HighWind = false,
        //                Collision = false,
        //                SafetyScore = 92 + _random.Next(-5, 5)
        //            }
        //        },
        //        new
        //        {
        //            Id = "EL001",
        //            Name = "1号升降机",
        //            Type = "Elevator",
        //            Model = "SC200/200",
        //            Status = "Running",
        //            Location = new
        //            {
        //                Zone = "A区",
        //                Coordinates = new[] { 116.397459, 39.909042 }
        //            },
        //            RealTimeData = new
        //            {
        //                Load = 800 + _random.Next(-200, 200),
        //                MaxLoad = 2000,
        //                Height = Math.Round(25.6 + _random.NextDouble() * 20 - 10, 1),
        //                MaxHeight = 100,
        //                WindSpeed = Math.Round(8.2 + _random.NextDouble() * 4 - 2, 1),
        //                MaxWindSpeed = 13,
        //                WorkingHours = Math.Round(9.1 + _random.NextDouble() * 2 - 1, 1)
        //            },
        //            SafetyStatus = new
        //            {
        //                Overload = false,
        //                OverHeight = false,
        //                HighWind = false,
        //                DoorStatus = true,
        //                SafetyScore = 98 + _random.Next(-3, 3)
        //            }
        //        }
        //    };

        //    return ApiResponseHelpers<object>.Success(devices);
        //}

        public async Task<ApiResponse<object>> GetCraneElevatorDevicesAsync(string? projectId = null, string? deviceType = null)
        {
            await Task.Delay(120);

            // 定义统一的结构类型
            var devices = new[]
            {
        new
        {
            Id = "TC001",
            Name = "1号塔吊",
            Type = "TowerCrane",
            Model = "QTZ80",
            Status = "Running",
            Location = new
            {
                Zone = "A区",
                Coordinates = new[] { 116.397459, 39.909042 }
            },
            RealTimeData = new
            {
                Load = 2500 + _random.Next(-500, 500),
                MaxLoad = 8000,
                Height = Math.Round(45.6 + _random.NextDouble() * 10 - 5, 1),
                MaxHeight = 60,
                WindSpeed = Math.Round(8.2 + _random.NextDouble() * 6 - 3, 1),
                MaxWindSpeed = 15,
                Rotation = _random.Next(0, 360),
                WorkingHours = Math.Round(8.5 + _random.NextDouble() * 2 - 1, 1)
            },
            SafetyStatus = new
            {
                Overload = false,
                OverHeight = false,
                HighWind = false,
                Collision = false,
                DoorStatus = false, // 新增统一字段
                SafetyScore = 95 + _random.Next(-5, 5)
            }
        },
        new
        {
            Id = "TC002",
            Name = "2号塔吊",
            Type = "TowerCrane",
            Model = "QTZ63",
            Status = "Running",
            Location = new
            {
                Zone = "B区",
                Coordinates = new[] { 116.407459, 39.919042 }
            },
            RealTimeData = new
            {
                Load = 3200 + _random.Next(-500, 500),
                MaxLoad = 6300,
                Height = Math.Round(38.2 + _random.NextDouble() * 8 - 4, 1),
                MaxHeight = 50,
                WindSpeed = Math.Round(7.8 + _random.NextDouble() * 5 - 2.5, 1),
                MaxWindSpeed = 15,
                Rotation = _random.Next(0, 360),
                WorkingHours = Math.Round(7.2 + _random.NextDouble() * 2 - 1, 1)
            },
            SafetyStatus = new
            {
                Overload = false,
                OverHeight = false,
                HighWind = false,
                Collision = false,
                DoorStatus = false, // 新增统一字段
                SafetyScore = 92 + _random.Next(-5, 5)
            }
        },
        new
        {
            Id = "EL001",
            Name = "1号升降机",
            Type = "Elevator",
            Model = "SC200/200",
            Status = "Running",
            Location = new
            {
                Zone = "A区",
                Coordinates = new[] { 116.397459, 39.909042 }
            },
            RealTimeData = new
            {
                Load = 800 + _random.Next(-200, 200),
                MaxLoad = 2000,
                Height = Math.Round(25.6 + _random.NextDouble() * 20 - 10, 1),
                MaxHeight = 100,
                WindSpeed = Math.Round(8.2 + _random.NextDouble() * 4 - 2, 1),
                MaxWindSpeed = 13,
                Rotation = 0, // 新增统一字段
                WorkingHours = Math.Round(9.1 + _random.NextDouble() * 2 - 1, 1)
            },
            SafetyStatus = new
            {
                Overload = false,
                OverHeight = false,
                HighWind = false,
                Collision = false, // 新增统一字段
                DoorStatus = true,
                SafetyScore = 98 + _random.Next(-3, 3)
            }
        }
    };

            return ApiResponseHelpers<object>.Success(devices);
        }

        public async Task<ApiResponse<object>> GetCraneElevatorStatisticsAsync()
        {
            await Task.Delay(100);

            var data = new
            {
                DeviceSummary = new
                {
                    TotalDevices = 28,
                    RunningDevices = 22 + _random.Next(-2, 2),
                    IdleDevices = 4,
                    MaintenanceDevices = 1,
                    FaultDevices = 1,
                    UtilizationRate = Math.Round(78.6 + _random.NextDouble() * 10 - 5, 1)
                },
                OperationSummary = new
                {
                    TodayOperations = 1456 + _random.Next(-100, 100),
                    TotalWorkingHours = Math.Round(186.5 + _random.NextDouble() * 20 - 10, 1),
                    AverageLoad = 3200 + _random.Next(-300, 300),
                    SafetyIncidents = _random.Next(0, 3)
                },
                MaintenanceSummary = new
                {
                    ScheduledMaintenance = 5,
                    OverdueMaintenance = 1,
                    NextMaintenanceDate = DateTime.Now.AddDays(3).ToString("yyyy-MM-dd")
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetSafetyMonitoringAsync()
        {
            await Task.Delay(80);

            var currentAlerts = new[]
            {
                new
                {
                    DeviceId = "TC001",
                    DeviceName = "1号塔吊",
                    AlertType = "HighWind",
                    AlertLevel = "Warning",
                    Message = "风速接近安全阈值",
                    CurrentValue = Math.Round(13.5 + _random.NextDouble() * 3, 1),
                    Threshold = 15.0,
                    Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };

            var data = new
            {
                CurrentAlerts = currentAlerts,
                SafetyMetrics = new
                {
                    OverallSafetyScore = Math.Round(92.8 + _random.NextDouble() * 4 - 2, 1),
                    RiskLevel = "Low",
                    CriticalAlerts = 0,
                    WarningAlerts = 1 + _random.Next(0, 2),
                    InfoAlerts = 2 + _random.Next(0, 3)
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetEfficiencyAnalysisAsync()
        {
            await Task.Delay(120);

            var dates = GenerateDateRange("week");
            var data = new
            {
                EfficiencyTrend = new
                {
                    Dates = dates,
                    Efficiency = dates.Select(_ => Math.Round(75 + _random.NextDouble() * 15, 1)).ToArray()
                },
                DeviceRanking = new[]
                {
                    new
                    {
                        DeviceId = "TC001",
                        DeviceName = "1号塔吊",
                        Efficiency = Math.Round(89.2 + _random.NextDouble() * 4 - 2, 1),
                        WorkingHours = Math.Round(8.5 + _random.NextDouble() * 2 - 1, 1),
                        OperationCount = 156 + _random.Next(-20, 20),
                        Rank = 1
                    },
                    new
                    {
                        DeviceId = "EL001",
                        DeviceName = "1号升降机",
                        Efficiency = Math.Round(87.8 + _random.NextDouble() * 4 - 2, 1),
                        WorkingHours = Math.Round(9.1 + _random.NextDouble() * 2 - 1, 1),
                        OperationCount = 234 + _random.Next(-30, 30),
                        Rank = 2
                    },
                    new
                    {
                        DeviceId = "TC002",
                        DeviceName = "2号塔吊",
                        Efficiency = Math.Round(82.3 + _random.NextDouble() * 4 - 2, 1),
                        WorkingHours = Math.Round(7.2 + _random.NextDouble() * 2 - 1, 1),
                        OperationCount = 128 + _random.Next(-15, 15),
                        Rank = 3
                    }
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        #endregion

        #region 扬尘噪音监测大屏

        public async Task<ApiResponse<object>> GetEnvironmentMonitoringAsync(string? projectId = null, string? monitorType = null)
        {
            await Task.Delay(100);

            var data = new
            {
                DustMonitoring = new
                {
                    Pm25 = Math.Round(45.6 + _random.NextDouble() * 20 - 10, 1),
                    Pm10 = Math.Round(78.9 + _random.NextDouble() * 30 - 15, 1),
                    Tsp = Math.Round(156.2 + _random.NextDouble() * 50 - 25, 1),
                    Status = "Good",
                    AlertLevel = "Normal",
                    Threshold = new
                    {
                        Pm25 = 75.0,
                        Pm10 = 150.0,
                        Tsp = 300.0
                    }
                },
                NoiseMonitoring = new
                {
                    CurrentLevel = Math.Round(68.5 + _random.NextDouble() * 10 - 5, 1),
                    AverageLevel = Math.Round(65.2 + _random.NextDouble() * 6 - 3, 1),
                    MaxLevel = Math.Round(82.1 + _random.NextDouble() * 8 - 4, 1),
                    Status = "Normal",
                    AlertLevel = "Normal",
                    Threshold = new
                    {
                        Day = 70.0,
                        Night = 55.0
                    }
                },
                WeatherData = new
                {
                    Temperature = Math.Round(28.5 + _random.NextDouble() * 6 - 3, 1),
                    Humidity = Math.Round(65.2 + _random.NextDouble() * 20 - 10, 1),
                    WindSpeed = Math.Round(8.2 + _random.NextDouble() * 4 - 2, 1),
                    WindDirection = "东南风",
                    AirPressure = Math.Round(1013.2 + _random.NextDouble() * 20 - 10, 1),
                    Visibility = Math.Round(15.6 + _random.NextDouble() * 8 - 4, 1)
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetMonitoringPointsAsync()
        {
            await Task.Delay(80);

            var points = new[]
            {
                new
                {
                    Id = "ENV001",
                    Name = "东门环境监测点",
                    Type = "Comprehensive",
                    Location = new
                    {
                        Zone = "东门区域",
                        Coordinates = new[] { 116.397459, 39.909042 }
                    },
                    Status = "Online",
                    Sensors = new
                    {
                        DustSensor = new
                        {
                            Status = "Normal",
                            Pm25 = Math.Round(45.6 + _random.NextDouble() * 10 - 5, 1),
                            Pm10 = Math.Round(78.9 + _random.NextDouble() * 15 - 7.5, 1)
                        },
                        NoiseSensor = new
                        {
                            Status = "Normal",
                            Level = Math.Round(68.5 + _random.NextDouble() * 8 - 4, 1)
                        },
                        WeatherSensor = new
                        {
                            Status = "Normal",
                            Temperature = Math.Round(28.5 + _random.NextDouble() * 4 - 2, 1),
                            Humidity = Math.Round(65.2 + _random.NextDouble() * 10 - 5, 1)
                        }
                    },
                    LastUpdate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                },
                new
                {
                    Id = "ENV002",
                    Name = "西门环境监测点",
                    Type = "Comprehensive",
                    Location = new
                    {
                        Zone = "西门区域",
                        Coordinates = new[] { 116.387459, 39.899042 }
                    },
                    Status = "Online",
                    Sensors = new
                    {
                        DustSensor = new
                        {
                            Status = "Normal",
                            Pm25 = Math.Round(52.3 + _random.NextDouble() * 10 - 5, 1),
                            Pm10 = Math.Round(85.7 + _random.NextDouble() * 15 - 7.5, 1)
                        },
                        NoiseSensor = new
                        {
                            Status = "Normal",
                            Level = Math.Round(72.1 + _random.NextDouble() * 8 - 4, 1)
                        },
                        WeatherSensor = new
                        {
                            Status = "Normal",
                            Temperature = Math.Round(29.1 + _random.NextDouble() * 4 - 2, 1),
                            Humidity = Math.Round(63.8 + _random.NextDouble() * 10 - 5, 1)
                        }
                    },
                    LastUpdate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };

            return ApiResponseHelpers<object>.Success(points);
        }

        public async Task<ApiResponse<object>> GetEnvironmentTrendsAsync(string timeRange, string? dataType = null)
        {
            await Task.Delay(120);

            var times = Enumerable.Range(0, 12).Select(i => $"{i * 2:D2}:00").ToArray();
            var data = new
            {
                DustTrend = new
                {
                    HourlyData = new
                    {
                        Times = times,
                        Pm25 = times.Select(_ => Math.Round(20 + _random.NextDouble() * 40, 1)).ToArray(),
                        Pm10 = times.Select(_ => Math.Round(40 + _random.NextDouble() * 60, 1)).ToArray()
                    }
                },
                NoiseTrend = new
                {
                    HourlyData = new
                    {
                        Times = times,
                        Levels = times.Select(_ => Math.Round(45 + _random.NextDouble() * 30, 1)).ToArray()
                    }
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetEnvironmentAlertsAsync()
        {
            await Task.Delay(80);

            var currentAlerts = new[]
            {
                new
                {
                    Id = "ALT001",
                    Type = "DustExceeded",
                    Level = "Warning",
                    Location = "西门监测点",
                    Message = "PM10浓度偏高",
                    CurrentValue = Math.Round(125.6 + _random.NextDouble() * 20, 1),
                    Threshold = 150.0,
                    Unit = "μg/m³",
                    Timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    Status = "Active"
                }
            };

            var data = new
            {
                CurrentAlerts = currentAlerts,
                AlertStatistics = new
                {
                    TodayAlerts = 8 + _random.Next(-3, 3),
                    ResolvedAlerts = 6 + _random.Next(-2, 2),
                    ActiveAlerts = 2 + _random.Next(-1, 2),
                    AlertTypes = new
                    {
                        Dust = 5 + _random.Next(-2, 2),
                        Noise = 2 + _random.Next(-1, 2),
                        Weather = 1 + _random.Next(0, 2)
                    }
                }
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        #endregion

        #region 通用工具接口

        public async Task<ApiResponse<object>> GetProjectInfoAsync(string projectId)
        {
            await Task.Delay(60);

            var data = new
            {
                Id = projectId,
                Name = "智慧工地示范项目A区",
                Description = "基于IoT、AI、大数据的新一代智慧工地管理系统",
                Status = "Active",
                Manager = "张工程师",
                StartDate = "2024-01-15",
                EndDate = "2024-12-31",
                Budget = 150000000L,
                CurrentProgress = Math.Round(65.8 + _random.NextDouble() * 5 - 2.5, 1)
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetSystemStatusAsync()
        {
            await Task.Delay(40);

            var data = new
            {
                SystemHealth = "Healthy",
                DatabaseStatus = "Connected",
                ApiStatus = "Running",
                IoTConnectionStatus = "Connected",
                LastHealthCheck = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                SystemLoad = Math.Round(65.5 + _random.NextDouble() * 20 - 10, 1),
                MemoryUsage = Math.Round(72.3 + _random.NextDouble() * 15 - 7.5, 1),
                DiskUsage = Math.Round(45.8 + _random.NextDouble() * 10 - 5, 1)
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        public async Task<ApiResponse<object>> GetCurrentTimeAsync()
        {
            await Task.Delay(20);

            var data = new
            {
                CurrentTime = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                TimeZone = "Asia/Shanghai",
                UnixTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds(),
                ServerUptime = TimeSpan.FromHours(72.5 + _random.NextDouble() * 24).ToString(@"dd\.hh\:mm\:ss")
            };

            return ApiResponseHelpers<object>.Success(data);
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 生成日期范围
        /// </summary>
        /// <param name="timeRange">时间范围</param>
        /// <returns>日期数组</returns>
        private string[] GenerateDateRange(string timeRange)
        {
            var now = DateTime.Now;
            var dates = new List<string>();

            int days = timeRange.ToLower() switch
            {
                "week" => 7,
                "month" => 30,
                "year" => 12,
                _ => 7
            };

            for (int i = days - 1; i >= 0; i--)
            {
                var date = now.AddDays(-i);
                if (timeRange.ToLower() == "year")
                {
                    dates.Add(date.ToString("MM"));
                }
                else
                {
                    dates.Add(date.ToString("MM-dd"));
                }
            }

            return dates.ToArray();
        }

        #endregion
    }
}
