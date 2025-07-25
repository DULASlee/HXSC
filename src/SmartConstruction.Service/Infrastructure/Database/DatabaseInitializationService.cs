using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using System.Security.Cryptography;
using System.Text;

namespace SmartConstruction.Service.Infrastructure.Database
{
    /// <summary>
    /// 数据库初始化服务
    /// </summary>
    public class DatabaseInitializationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseInitializationService> _logger;

        public DatabaseInitializationService(ApplicationDbContext context, ILogger<DatabaseInitializationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("开始初始化数据库...");

                // 确保数据库已创建
                await _context.Database.EnsureCreatedAsync();

                // 检查是否需要初始化种子数据
                if (!await _context.Tenants.AnyAsync())
                {
                    //await SeedDataAsync();
                    _logger.LogInformation("数据库种子数据初始化完成");
                }
                else
                {
                    _logger.LogInformation("数据库已包含数据，跳过种子数据初始化");
                }
                
                _logger.LogInformation("数据库初始化完成");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "数据库初始化失败");
                throw;
            }
        }

        /// <summary>
        /// 初始化种子数据
        /// </summary>
        //private async Task SeedDataAsync()
        //{
        //    // 创建演示租户
        //    var demoTenant = new Tenant
        //    {
        //        Id = Guid.NewGuid(),
        //        Name = "演示租户",
        //        Code = "SYSTEM",
        //        Status = 1,
        //        CreatedAt = DateTime.UtcNow,
        //        IsDeleted = false
        //    };

        //    _context.Tenants.Add(demoTenant);

        //    // 创建演示公司
        //    var company = new Company
        //    {
        //        Id = Guid.NewGuid(),
        //        TenantId = demoTenant.Id,
        //        CompanyName = "智慧建筑科技有限公司",
        //        Code = "COMP001",
        //        Address = "北京市朝阳区智慧大厦",
        //        ContactPerson = "张经理",
        //        ContactPhone = "13800138000",
        //        ContactEmail = "contact@smartconstruction.com",
        //        Status = 1,
        //        CreatedAt = DateTime.UtcNow,
        //        IsDeleted = false
        //    };

        //    _context.Companies.Add(company);

        //    // 创建演示项目
        //    var projects = new List<Project>
        //    {
        //        new Project
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            CompanyId = company.Id,
        //            ProjectName = "智慧办公楼建设项目",
        //            ProjectCode = "PRJ001",
        //            Code = "PRJ001",
        //            Description = "现代化智慧办公楼建设，包含IoT设备集成和数字孪生系统",
        //            StartDate = DateTime.UtcNow.AddMonths(-6),
        //            PlannedEndDate = DateTime.UtcNow.AddMonths(6),
        //            Status = "IN_PROGRESS",
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        },
        //        new Project
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            CompanyId = company.Id,
        //            ProjectName = "智能住宅小区项目",
        //            ProjectCode = "PRJ002",
        //            Code = "PRJ002",
        //            Description = "智能化住宅小区建设，集成安防、环境监测等系统",
        //            StartDate = DateTime.UtcNow.AddMonths(-3),
        //            PlannedEndDate = DateTime.UtcNow.AddMonths(9),
        //            Status = "IN_PROGRESS",
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        }
        //    };

        //    _context.Projects.AddRange(projects);

        //    // 创建演示设备
        //    var devices = new List<Device>
        //    {
        //        new Device
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            ProjectId = projects[0].Id,
        //            DeviceCode = "DEV001",
        //            DeviceName = "温度传感器-01",
        //            DeviceType = "TemperatureSensor",
        //            Location = "1楼大厅",
        //            Status = "Online",
        //            LastUpdateTime = DateTime.UtcNow,
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        },
        //        new Device
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            ProjectId = projects[0].Id,
        //            DeviceCode = "DEV002",
        //            DeviceName = "湿度传感器-01",
        //            DeviceType = "HumiditySensor",
        //            Location = "1楼大厅",
        //            Status = "Online",
        //            LastUpdateTime = DateTime.UtcNow,
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        },
        //        new Device
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            ProjectId = projects[0].Id,
        //            DeviceCode = "DEV003",
        //            DeviceName = "摄像头-01",
        //            DeviceType = "Camera",
        //            Location = "1楼入口",
        //            Status = "Online",
        //            LastUpdateTime = DateTime.UtcNow,
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        }
        //    };

        //    _context.Devices.AddRange(devices);

        //    // 创建演示用户
        //    var users = new List<User>
        //    {
        //        new User
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            Username = "admin",
        //            Email = "admin@smartconstruction.com",
        //            Mobile = "13800138001",
        //            PhoneNumber = "13800138001",
        //            RealName = "系统管理员",
        //            PasswordHash = GeneratePasswordHash("123456"),
        //            SecurityStamp = Guid.NewGuid().ToString(),
        //            DisplayName = "系统管理员",
        //            Status = 1,
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        },
        //        new User
        //        {
        //            Id = Guid.NewGuid(),
        //            TenantId = demoTenant.Id,
        //            Username = "engineer",
        //            Email = "engineer@smartconstruction.com",
        //            Mobile = "13800138002",
        //            PhoneNumber = "13800138002",
        //            RealName = "李工程师",
        //            PasswordHash = GeneratePasswordHash("123456"),
        //            SecurityStamp = Guid.NewGuid().ToString(),
        //            DisplayName = "李工程师",
        //            Status = 1,
        //            CreatedAt = DateTime.UtcNow,
        //            IsDeleted = false
        //        }
        //    };

        //    _context.Users.AddRange(users);

        //    await _context.SaveChangesAsync();
        //}



        /// <summary>
        /// 生成密码哈希
        /// </summary>
        private byte[] GeneratePasswordHash(string password)
        {
            using var sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
} 
