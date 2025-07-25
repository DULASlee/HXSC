using Microsoft.Extensions.Logging;
using SmartConstruction.Service.Data;
using System.Threading.Tasks;
using System;
using SmartConstruction.Contracts.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Linq;

namespace SmartConstruction.Service.Infrastructure.Database
{
    /// <summary>
    /// 数字孪生数据种子生成器
    /// 生成演示所需的完整数据集：50人员、塔吊、环境监测、菜单权限
    /// </summary>
    public class DigitalTwinDataSeeder
    {
        private readonly SmartConstructionDbContext _context;
        private readonly ILogger<DigitalTwinDataSeeder> _logger;

        public DigitalTwinDataSeeder(
            SmartConstructionDbContext context,
            ILogger<DigitalTwinDataSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 执行数字孪生数据种子生成
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("开始生成数字孪生演示数据...");

                // 检查是否已有数字孪生数据
                if (await _context.Users.AnyAsync(u => u.Username.StartsWith("worker_")))
                {
                    _logger.LogInformation("数字孪生数据已存在，跳过生成");
                    return;
                }

                await SeedDigitalTwinMenusAsync();
                await SeedPersonnelDataAsync();
                await SeedDeviceDataAsync();
                await SeedEnvironmentDataAsync();
                await SeedProjectDataAsync();
                await BindMenuPermissionsAsync();

                await _context.SaveChangesAsync();
                _logger.LogInformation("数字孪生演示数据生成完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成数字孪生数据失败");
                throw;
            }
        }

        /// <summary>
        /// 生成数字孪生菜单
        /// </summary>
        private async Task SeedDigitalTwinMenusAsync()
        {
            var digitalTwinMenus = new[]
            {
                new Menu
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    Name = "数字孪生大屏",
                    Path = "/digital-twin",
                    Component = "views/digital-twin/index",
                    Icon = "View",
                    Sort = 100,
                    IsVisible = true,
                    Status = 1,
                    Type = "Menu",
                    ParentId = null,
                    CreatedAt = DateTime.Now
                },
                new Menu
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000002"),
                    Name = "指挥中心",
                    Path = "/digital-twin/command-center",
                    Component = "views/digital-twin/command-center/index",
                    Icon = "Monitor",
                    Sort = 1,
                    IsVisible = true,
                    Status = 1,
                    Type = "Menu",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.Now
                },
                new Menu
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000003"),
                    Name = "实名制考勤",
                    Path = "/digital-twin/attendance",
                    Component = "views/digital-twin/attendance/index",
                    Icon = "User",
                    Sort = 2,
                    IsVisible = true,
                    Status = 1,
                    Type = "Menu",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.Now
                },
                new Menu
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000004"),
                    Name = "塔吊监控",
                    Path = "/digital-twin/crane-elevator",
                    Component = "views/digital-twin/crane-elevator/index",
                    Icon = "Setting",
                    Sort = 3,
                    IsVisible = true,
                    Status = 1,
                    Type = "Menu",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.Now
                },
                new Menu
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000005"),
                    Name = "扬尘噪音",
                    Path = "/digital-twin/environment",
                    Component = "views/digital-twin/environment/index",
                    Icon = "DataAnalysis",
                    Sort = 4,
                    IsVisible = true,
                    Status = 1,
                    Type = "Menu",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.Now
                },
                new Menu
                {
                    Id = Guid.Parse("10000000-0000-0000-0000-000000000006"),
                    Name = "视频安防",
                    Path = "/digital-twin/video-security",
                    Component = "views/digital-twin/video-security/index",
                    Icon = "VideoCamera",
                    Sort = 5,
                    IsVisible = true,
                    Status = 1,
                    Type = "Menu",
                    ParentId = Guid.Parse("10000000-0000-0000-0000-000000000001"),
                    CreatedAt = DateTime.Now
                }
            };

            foreach (var menu in digitalTwinMenus)
            {
                if (!await _context.Menus.AnyAsync(m => m.Id == menu.Id))
                {
                    _context.Menus.Add(menu);
                }
            }
        }

        /// <summary>
        /// 生成50人员记录
        /// </summary>
        private async Task SeedPersonnelDataAsync()
        {
            var random = new Random();
            var departments = new[] { "建筑施工", "设备操作", "安全管理", "质量检查", "材料运输" };
            var positions = new[] { "工长", "技工", "普工", "安全员", "质检员", "司机", "操作员" };

            for (int i = 1; i <= 50; i++)
            {
                var workerId = $"worker_{i:D3}";
                
                // 检查用户是否已存在
                if (await _context.Users.AnyAsync(u => u.Username == workerId))
                    continue;

                var worker = new User
                {
                    Id = Guid.NewGuid(),
                    Username = workerId,
                    DisplayName = $"工人{i:D2}",
                    Email = $"{workerId}@construction.com",
                    Mobile = $"138{random.Next(10000000, 99999999)}",
                    Status = 1,
                    TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // 默认租户
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(worker);

                // 创建对应的Worker记录
                var workerDetail = new Worker
                {
                    Id = Guid.NewGuid(),
                    IdCardNumber = GenerateIdCardNumber(random),
                    FullName = worker.DisplayName,
                    Gender = random.Next(2) == 0 ? "男" : "女",
                    PhoneNumber = worker.Mobile,
                    Specialty = positions[random.Next(positions.Length)],
                    Status = (byte)(random.NextDouble() > 0.05 ? 1 : 0), // 1: 在职, 0: 离职
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // 默认项目
                    CreatedAt = DateTime.Now
                };

                _context.Workers.Add(workerDetail);

                // 生成考勤记录
                await GenerateAttendanceRecordsAsync(worker.Id, random);
            }
        }

        /// <summary>
        /// 生成设备数据（塔吊、升降机等）
        /// </summary>
        private async Task SeedDeviceDataAsync()
        {
            var devices = new[]
            {
                new Device
                {
                    Id = Guid.NewGuid(),
                    DeviceCode = "TC001-2024",
                    DeviceName = "1号塔吊",
                    DeviceType = "Crane",
                    Model = "QTZ80(5510)",
                    Manufacturer = "中联重科",
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Location = "30,0,30", // X,Y,Z坐标
                    Status = "Online",
                    CreatedAt = DateTime.Now
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    DeviceCode = "TC002-2024",
                    DeviceName = "2号塔吊",
                    DeviceType = "Crane",
                    Model = "QTZ63(5013)",
                    Manufacturer = "徐工集团",
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Location = "-30,0,-30",
                    Status = "Online",
                    CreatedAt = DateTime.Now
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    DeviceCode = "EV001-2024",
                    DeviceName = "1号升降机",
                    DeviceType = "Elevator",
                    Model = "SC200/200G",
                    Manufacturer = "三一重工",
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Location = "50,0,0",
                    Status = "Online",
                    CreatedAt = DateTime.Now
                },
                new Device
                {
                    Id = Guid.NewGuid(),
                    DeviceCode = "FC001-2024",
                    DeviceName = "1号雾炮车",
                    DeviceType = "Other",
                    Model = "WPC-80",
                    Manufacturer = "宇通重工",
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Location = "50,0,30",
                    Status = "Standby",
                    CreatedAt = DateTime.Now
                }
            };

            foreach (var device in devices)
            {
                if (!await _context.Devices.AnyAsync(d => d.DeviceCode == device.DeviceCode))
                {
                    _context.Devices.Add(device);
                }
            }
        }

        /// <summary>
        /// 生成环境监测数据
        /// </summary>
        private async Task SeedEnvironmentDataAsync()
        {
            var random = new Random();
            var monitoringStations = new[]
            {
                new
                {
                    Name = "1号监测站",
                    Location = "0,0,50",
                    PM25 = 45 + random.Next(-10, 15),
                    PM10 = 82 + random.Next(-15, 25),
                    Noise = 68 + random.Next(-5, 10),
                    Temperature = 25 + random.Next(-3, 5),
                    Humidity = 65 + random.Next(-10, 15),
                    WindSpeed = 3.2 + (random.NextDouble() - 0.5) * 2
                },
                new
                {
                    Name = "2号监测站",
                    Location = "80,0,-20",
                    PM25 = 35 + random.Next(-8, 12),
                    PM10 = 65 + random.Next(-12, 20),
                    Noise = 55 + random.Next(-5, 8),
                    Temperature = 26 + random.Next(-2, 4),
                    Humidity = 62 + random.Next(-8, 12),
                    WindSpeed = 2.8 + (random.NextDouble() - 0.5) * 1.5
                },
                new
                {
                    Name = "3号监测站",
                    Location = "-60,0,30",
                    PM25 = 95 + random.Next(-15, 20),
                    PM10 = 145 + random.Next(-20, 30),
                    Noise = 75 + random.Next(-8, 12),
                    Temperature = 24 + random.Next(-3, 5),
                    Humidity = 70 + random.Next(-12, 15),
                    WindSpeed = 1.5 + (random.NextDouble() - 0.5) * 1
                }
            };

            foreach (var station in monitoringStations)
            {
                var device = new Device
                {
                    Id = Guid.NewGuid(),
                    DeviceCode = $"ENV{Array.IndexOf(monitoringStations, station) + 1:D3}-2024",
                    DeviceName = station.Name,
                    DeviceType = "Sensor",
                    Model = "ENV-2024",
                    Manufacturer = "环境监测设备",
                    ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Location = station.Location,
                    Status = "Online",
                    CreatedAt = DateTime.Now
                };

                if (!await _context.Devices.AnyAsync(d => d.DeviceCode == device.DeviceCode))
                {
                    _context.Devices.Add(device);
                }
            }
        }

        /// <summary>
        /// 生成项目数据
        /// </summary>
        private async Task SeedProjectDataAsync()
        {
            if (await _context.Projects.AnyAsync(p => p.ProjectName == "智慧工地示范项目"))
                return;

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectCode = "DT-2024-001",
                ProjectName = "智慧工地示范项目",
                CompanyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                ProjectAddress = "上海市浦东新区",
                ProjectManager = "张经理",
                StartDate = DateTime.Now.AddMonths(-8),
                PlannedEndDate = DateTime.Now.AddMonths(16),
                Status = "InProgress",
                ContractAmount = 50000000, // 5000万
                CreatedAt = DateTime.Now
            };

            _context.Projects.Add(project);
        }

        /// <summary>
        /// 绑定菜单权限到管理员和安全员角色
        /// </summary>
        private async Task BindMenuPermissionsAsync()
        {
            // 获取管理员和安全员角色
            var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "管理员");
            var safetyRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "安全员");

            if (adminRole == null || safetyRole == null)
            {
                _logger.LogWarning("未找到管理员或安全员角色，跳过数字孪生菜单权限绑定");
                return;
            }

            // 获取数字孪生菜单
            var digitalTwinMenus = await _context.Menus
                .Where(m => m.Path.StartsWith("/digital-twin"))
                .ToListAsync();

            foreach (var menu in digitalTwinMenus)
            {
                // 绑定管理员权限
                if (!await _context.RoleMenus.AnyAsync(rm => rm.RoleId == adminRole.Id && rm.MenuId == menu.Id))
                {
                    _context.RoleMenus.Add(new RoleMenu
                    {
                        Id = Guid.NewGuid(),
                        RoleId = adminRole.Id,
                        MenuId = menu.Id,
                        CreatedAt = DateTime.Now
                    });
                }

                // 绑定安全员权限（指挥中心、实名制考勤、扬尘噪音、视频安防）
                if (menu.Path.Contains("command-center") || 
                    menu.Path.Contains("attendance") || 
                    menu.Path.Contains("environment") ||
                    menu.Path.Contains("video-security") ||
                    menu.Path == "/digital-twin")
                {
                    if (!await _context.RoleMenus.AnyAsync(rm => rm.RoleId == safetyRole.Id && rm.MenuId == menu.Id))
                    {
                        _context.RoleMenus.Add(new RoleMenu
                        {
                            Id = Guid.NewGuid(),
                            RoleId = safetyRole.Id,
                            MenuId = menu.Id,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 生成考勤记录
        /// </summary>
        private async Task GenerateAttendanceRecordsAsync(Guid userId, Random random)
        {
            // 生成最近30天的考勤记录
            for (int day = 0; day < 30; day++)
            {
                var date = DateTime.Today.AddDays(-day);
                
                // 跳过周末（模拟休息日）
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                // 90%的出勤率
                if (random.NextDouble() < 0.9)
                {
                    var checkInTime = date.AddHours(7.5 + random.NextDouble() * 1.5); // 7:30-9:00上班
                    var checkOutTime = date.AddHours(17.5 + random.NextDouble() * 1.5); // 17:30-19:00下班

                    var attendance = new AttendanceRecord
                    {
                        Id = Guid.NewGuid(),
                        WorkerId = userId, // 注意：这里应该是WorkerId而不是UserId
                        ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        AttendanceDate = date,
                        ClockInTime = checkInTime,
                        ClockOutTime = checkOutTime,
                        AttendanceType = "Normal",
                        Location = GenerateRandomLocation(random),
                        Source = "FaceRecognition",
                        CreatedAt = checkInTime
                    };

                    _context.AttendanceRecords.Add(attendance);
                }
            }
        }

        /// <summary>
        /// 生成身份证号码
        /// </summary>
        private string GenerateIdCardNumber(Random random)
        {
            var prefix = "31011";
            var year = random.Next(1970, 2000).ToString();
            var month = random.Next(1, 13).ToString("D2");
            var day = random.Next(1, 29).ToString("D2");
            var suffix = random.Next(100, 999).ToString();
            var checkDigit = random.Next(0, 10).ToString();

            return $"{prefix}{year}{month}{day}{suffix}{checkDigit}";
        }

        /// <summary>
        /// 生成技能列表
        /// </summary>
        private string GenerateSkills(Random random)
        {
            var allSkills = new[] 
            { 
                "钢筋工", "混凝土工", "架子工", "焊工", "电工", 
                "塔吊操作", "挖掘机操作", "安全管理", "质量检查", "测量" 
            };
            
            var skillCount = random.Next(1, 4);
            var skills = allSkills.OrderBy(x => random.Next()).Take(skillCount);
            
            return string.Join(",", skills);
        }

        /// <summary>
        /// 生成随机位置
        /// </summary>
        private string GenerateRandomLocation(Random random)
        {
            var x = random.Next(-100, 100);
            var z = random.Next(-100, 100);
            return $"{x},{z}";
        }

        /// <summary>
        /// 获取随机风向
        /// </summary>
        private string GetRandomWindDirection(Random random)
        {
            var directions = new[] { "北风", "东北风", "东风", "东南风", "南风", "西南风", "西风", "西北风" };
            return directions[random.Next(directions.Length)];
        }
    }
} 