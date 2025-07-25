using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Services;

namespace SmartConstruction.Service.Scripts
{
    /// <summary>
    /// 初始化菜单脚本
    /// </summary>
    public class InitializeMenus
    {
        private readonly ApplicationDbContext _context;
        private readonly IMenuService _menuService;
        private readonly ILogger<InitializeMenus> _logger;

        public InitializeMenus(
            ApplicationDbContext context,
            IMenuService menuService,
            ILogger<InitializeMenus> logger)
        {
            _context = context;
            _menuService = menuService;
            _logger = logger;
        }

        /// <summary>
        /// 初始化所有菜单
        /// </summary>
        public async Task InitializeAllMenusAsync()
        {
            try
            {
                _logger.LogInformation("开始初始化菜单...");

                // 获取SYSTEM租户
                var systemTenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Code == "SYSTEM");

                if (systemTenant == null)
                {
                    _logger.LogError("未找到SYSTEM租户");
                    return;
                }

                // 获取admin用户
                var adminUser = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == "admin" && u.TenantId == systemTenant.Id);

                if (adminUser == null)
                {
                    _logger.LogError("未找到admin用户");
                    return;
                }

                // 获取管理员角色
                var adminRole = adminUser.UserRoles.FirstOrDefault()?.Role;
                if (adminRole == null)
                {
                    _logger.LogError("admin用户没有分配角色");
                    return;
                }

                // 清除现有菜单
                await ClearExistingMenusAsync(systemTenant.Id);

                // 创建菜单结构
                var menuIds = await CreateMenuStructureAsync(systemTenant.Id, adminUser.Id);

                // 为管理员角色分配所有菜单
                await AssignMenusToRoleAsync(adminRole.Id, menuIds, adminUser.Id);

                _logger.LogInformation($"菜单初始化完成，共创建 {menuIds.Count} 个菜单");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化菜单失败");
                throw;
            }
        }

        /// <summary>
        /// 清除现有菜单
        /// </summary>
        private async Task ClearExistingMenusAsync(Guid tenantId)
        {
            var existingMenus = await _context.Menus
                .Where(m => m.TenantId == tenantId)
                .ToListAsync();

            if (existingMenus.Any())
            {
                _context.Menus.RemoveRange(existingMenus);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"清除了 {existingMenus.Count} 个现有菜单");
            }
        }

        /// <summary>
        /// 创建菜单结构
        /// </summary>
        private async Task<List<Guid>> CreateMenuStructureAsync(Guid tenantId, Guid operatorId)
        {
            var menuIds = new List<Guid>();
            var now = DateTime.UtcNow;

            // 1. 工作台
            var dashboardMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Dashboard",
                Title = "工作台",
                Path = "/dashboard",
                Component = "Layout",
                Icon = "House",
                Type = "Menu",
                Sort = 1,
                Level = 1,
                TreePath = "1",
                Status = 1,
                IsVisible = true,
                IsCache = true,
                IsAffix = true,
                Permission = "dashboard.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(dashboardMenu);
            menuIds.Add(dashboardMenu.Id);

            // 2. 租户管理
            var tenantManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "TenantManagement",
                Title = "租户管理",
                Path = "/tenant",
                Component = "Layout",
                Icon = "OfficeBuilding",
                Type = "Directory",
                Sort = 2,
                Level = 1,
                TreePath = "2",
                Status = 1,
                IsVisible = true,
                Permission = "tenant.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(tenantManagementMenu);
            menuIds.Add(tenantManagementMenu.Id);

            var tenantListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = tenantManagementMenu.Id,
                Name = "TenantList",
                Title = "租户列表",
                Path = "list",
                Component = "@/views/tenant/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "2.1",
                Status = 1,
                IsVisible = true,
                Permission = "tenant.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(tenantListMenu);
            menuIds.Add(tenantListMenu.Id);

            // 3. 公司管理
            var companyManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "CompanyManagement",
                Title = "公司管理",
                Path = "/company",
                Component = "Layout",
                Icon = "OfficeBuilding",
                Type = "Directory",
                Sort = 3,
                Level = 1,
                TreePath = "3",
                Status = 1,
                IsVisible = true,
                Permission = "company.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(companyManagementMenu);
            menuIds.Add(companyManagementMenu.Id);

            var companyListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = companyManagementMenu.Id,
                Name = "CompanyList",
                Title = "公司列表",
                Path = "list",
                Component = "@/views/company/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "3.1",
                Status = 1,
                IsVisible = true,
                Permission = "company.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(companyListMenu);
            menuIds.Add(companyListMenu.Id);

            // 4. 项目管理
            var projectManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "ProjectManagement",
                Title = "项目管理",
                Path = "/project",
                Component = "Layout",
                Icon = "Document",
                Type = "Directory",
                Sort = 4,
                Level = 1,
                TreePath = "4",
                Status = 1,
                IsVisible = true,
                Permission = "project.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(projectManagementMenu);
            menuIds.Add(projectManagementMenu.Id);

            var projectListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = projectManagementMenu.Id,
                Name = "ProjectList",
                Title = "项目列表",
                Path = "list",
                Component = "@/views/project/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "4.1",
                Status = 1,
                IsVisible = true,
                Permission = "project.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(projectListMenu);
            menuIds.Add(projectListMenu.Id);

            // 5. 班组管理
            var teamManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "TeamManagement",
                Title = "班组管理",
                Path = "/team",
                Component = "Layout",
                Icon = "User",
                Type = "Directory",
                Sort = 5,
                Level = 1,
                TreePath = "5",
                Status = 1,
                IsVisible = true,
                Permission = "team.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(teamManagementMenu);
            menuIds.Add(teamManagementMenu.Id);

            var teamListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = teamManagementMenu.Id,
                Name = "TeamList",
                Title = "班组列表",
                Path = "list",
                Component = "@/views/team/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "5.1",
                Status = 1,
                IsVisible = true,
                Permission = "team.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(teamListMenu);
            menuIds.Add(teamListMenu.Id);

            // 6. 工人管理
            var workerManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "WorkerManagement",
                Title = "工人管理",
                Path = "/worker",
                Component = "Layout",
                Icon = "User",
                Type = "Directory",
                Sort = 6,
                Level = 1,
                TreePath = "6",
                Status = 1,
                IsVisible = true,
                Permission = "worker.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(workerManagementMenu);
            menuIds.Add(workerManagementMenu.Id);

            var workerListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = workerManagementMenu.Id,
                Name = "WorkerList",
                Title = "工人列表",
                Path = "list",
                Component = "@/views/worker/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "6.1",
                Status = 1,
                IsVisible = true,
                Permission = "worker.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(workerListMenu);
            menuIds.Add(workerListMenu.Id);

            // 7. 考勤管理
            var attendanceManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "AttendanceManagement",
                Title = "考勤管理",
                Path = "/attendance",
                Component = "Layout",
                Icon = "Calendar",
                Type = "Directory",
                Sort = 7,
                Level = 1,
                TreePath = "7",
                Status = 1,
                IsVisible = true,
                Permission = "attendance.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(attendanceManagementMenu);
            menuIds.Add(attendanceManagementMenu.Id);

            var attendanceListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = attendanceManagementMenu.Id,
                Name = "AttendanceList",
                Title = "考勤记录",
                Path = "list",
                Component = "@/views/attendance/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "7.1",
                Status = 1,
                IsVisible = true,
                Permission = "attendance.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(attendanceListMenu);
            menuIds.Add(attendanceListMenu.Id);

            var attendanceStatisticsMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = attendanceManagementMenu.Id,
                Name = "AttendanceStatistics",
                Title = "考勤统计",
                Path = "statistics",
                Component = "@/views/attendance/statistics/index.vue",
                Icon = "PieChart",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "7.2",
                Status = 1,
                IsVisible = true,
                Permission = "attendance.statistics",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(attendanceStatisticsMenu);
            menuIds.Add(attendanceStatisticsMenu.Id);

            // 8. 设备管理
            var deviceManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "DeviceManagement",
                Title = "设备管理",
                Path = "/device",
                Component = "Layout",
                Icon = "Monitor",
                Type = "Directory",
                Sort = 8,
                Level = 1,
                TreePath = "8",
                Status = 1,
                IsVisible = true,
                Permission = "device.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(deviceManagementMenu);
            menuIds.Add(deviceManagementMenu.Id);

            var deviceListMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = deviceManagementMenu.Id,
                Name = "DeviceList",
                Title = "设备列表",
                Path = "list",
                Component = "@/views/device/list/index.vue",
                Icon = "List",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "8.1",
                Status = 1,
                IsVisible = true,
                Permission = "device.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(deviceListMenu);
            menuIds.Add(deviceListMenu.Id);

            var deviceMonitorMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = deviceManagementMenu.Id,
                Name = "DeviceMonitor",
                Title = "设备监控",
                Path = "monitor",
                Component = "@/views/device/monitor/index.vue",
                Icon = "Monitor",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "8.2",
                Status = 1,
                IsVisible = true,
                Permission = "device.monitor",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(deviceMonitorMenu);
            menuIds.Add(deviceMonitorMenu.Id);

            // 9. 安全管理
            var safetyManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "SafetyManagement",
                Title = "安全管理",
                Path = "/safety",
                Component = "Layout",
                Icon = "Warning",
                Type = "Directory",
                Sort = 9,
                Level = 1,
                TreePath = "9",
                Status = 1,
                IsVisible = true,
                Permission = "safety.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(safetyManagementMenu);
            menuIds.Add(safetyManagementMenu.Id);

            var safetyIncidentMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = safetyManagementMenu.Id,
                Name = "SafetyIncident",
                Title = "安全事件",
                Path = "incident",
                Component = "@/views/safety/incident/index.vue",
                Icon = "Warning",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "9.1",
                Status = 1,
                IsVisible = true,
                Permission = "safety.incident.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(safetyIncidentMenu);
            menuIds.Add(safetyIncidentMenu.Id);

            var safetyStatisticsMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = safetyManagementMenu.Id,
                Name = "SafetyStatistics",
                Title = "安全统计",
                Path = "statistics",
                Component = "@/views/safety/statistics/index.vue",
                Icon = "PieChart",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "9.2",
                Status = 1,
                IsVisible = true,
                Permission = "safety.statistics",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(safetyStatisticsMenu);
            menuIds.Add(safetyStatisticsMenu.Id);

            // 10. 数字孪生
            var digitalTwinMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "DigitalTwin",
                Title = "数字孪生",
                Path = "/digital-twin",
                Component = "@/views/digital-twin/index.vue",
                Icon = "Monitor",
                Type = "Menu",
                Sort = 10,
                Level = 1,
                TreePath = "10",
                Status = 1,
                IsVisible = true,
                IsCache = false,
                Permission = "digital-twin.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(digitalTwinMenu);
            menuIds.Add(digitalTwinMenu.Id);

            // 11. 集成管理
            var integrationManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "IntegrationManagement",
                Title = "集成管理",
                Path = "/integration",
                Component = "Layout",
                Icon = "Connection",
                Type = "Directory",
                Sort = 11,
                Level = 1,
                TreePath = "11",
                Status = 1,
                IsVisible = true,
                Permission = "integration.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(integrationManagementMenu);
            menuIds.Add(integrationManagementMenu.Id);

            var governmentPushMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = integrationManagementMenu.Id,
                Name = "GovernmentPush",
                Title = "政府推送",
                Path = "government",
                Component = "@/views/integration/government/index.vue",
                Icon = "Upload",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "11.1",
                Status = 1,
                IsVisible = true,
                Permission = "integration.government",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(governmentPushMenu);
            menuIds.Add(governmentPushMenu.Id);

            var iotIntegrationMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = integrationManagementMenu.Id,
                Name = "IoTIntegration",
                Title = "IoT集成",
                Path = "iot",
                Component = "@/views/integration/iot/index.vue",
                Icon = "Connection",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "11.2",
                Status = 1,
                IsVisible = true,
                Permission = "integration.iot",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(iotIntegrationMenu);
            menuIds.Add(iotIntegrationMenu.Id);

            // 12. 系统管理
            var systemManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "System",
                Title = "系统管理",
                Path = "/system",
                Component = "Layout",
                Icon = "Setting",
                Type = "Directory",
                Sort = 12,
                Level = 1,
                TreePath = "12",
                Status = 1,
                IsVisible = true,
                Permission = "system.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(systemManagementMenu);
            menuIds.Add(systemManagementMenu.Id);

            var userManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = systemManagementMenu.Id,
                Name = "User",
                Title = "用户管理",
                Path = "user",
                Component = "@/views/system/user/index.vue",
                Icon = "User",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "12.1",
                Status = 1,
                IsVisible = true,
                Permission = "user.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(userManagementMenu);
            menuIds.Add(userManagementMenu.Id);

            var roleManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = systemManagementMenu.Id,
                Name = "Role",
                Title = "角色管理",
                Path = "role",
                Component = "@/views/system/role/index.vue",
                Icon = "Avatar",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "12.2",
                Status = 1,
                IsVisible = true,
                Permission = "role.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(roleManagementMenu);
            menuIds.Add(roleManagementMenu.Id);

            var menuManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = systemManagementMenu.Id,
                Name = "Menu",
                Title = "菜单管理",
                Path = "menu",
                Component = "@/views/system/menu/index.vue",
                Icon = "Menu",
                Type = "Menu",
                Sort = 3,
                Level = 2,
                TreePath = "12.3",
                Status = 1,
                IsVisible = true,
                Permission = "menu.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(menuManagementMenu);
            menuIds.Add(menuManagementMenu.Id);

            var permissionManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = systemManagementMenu.Id,
                Name = "Permission",
                Title = "权限管理",
                Path = "permission",
                Component = "@/views/system/permission/index.vue",
                Icon = "Lock",
                Type = "Menu",
                Sort = 4,
                Level = 2,
                TreePath = "12.4",
                Status = 1,
                IsVisible = true,
                Permission = "permission.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(permissionManagementMenu);
            menuIds.Add(permissionManagementMenu.Id);

            var resourceManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = systemManagementMenu.Id,
                Name = "Resource",
                Title = "资源管理",
                Path = "resource",
                Component = "@/views/system/resource/index.vue",
                Icon = "Files",
                Type = "Menu",
                Sort = 5,
                Level = 2,
                TreePath = "12.5",
                Status = 1,
                IsVisible = true,
                Permission = "resource.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(resourceManagementMenu);
            menuIds.Add(resourceManagementMenu.Id);

            // 13. 元数据管理
            var metadataManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Metadata",
                Title = "元数据管理",
                Path = "/metadata",
                Component = "Layout",
                Icon = "DataAnalysis",
                Type = "Directory",
                Sort = 13,
                Level = 1,
                TreePath = "13",
                Status = 1,
                IsVisible = true,
                Permission = "metadata.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(metadataManagementMenu);
            menuIds.Add(metadataManagementMenu.Id);

            var metadataDefinitionMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = metadataManagementMenu.Id,
                Name = "MetadataDefinition",
                Title = "元数据定义",
                Path = "definition",
                Component = "@/views/metadata/definition/index.vue",
                Icon = "Document",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "13.1",
                Status = 1,
                IsVisible = true,
                Permission = "metadata.definition.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(metadataDefinitionMenu);
            menuIds.Add(metadataDefinitionMenu.Id);

            var metadataConfigMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = metadataManagementMenu.Id,
                Name = "MetadataConfig",
                Title = "元数据配置",
                Path = "config",
                Component = "@/views/metadata/config/index.vue",
                Icon = "Setting",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "13.2",
                Status = 1,
                IsVisible = true,
                Permission = "metadata.config.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(metadataConfigMenu);
            menuIds.Add(metadataConfigMenu.Id);

            var metadataPreviewMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = metadataManagementMenu.Id,
                Name = "MetadataPreview",
                Title = "动态表单预览",
                Path = "preview",
                Component = "@/views/metadata/preview/index.vue",
                Icon = "View",
                Type = "Menu",
                Sort = 3,
                Level = 2,
                TreePath = "13.3",
                Status = 1,
                IsVisible = true,
                Permission = "metadata.preview",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(metadataPreviewMenu);
            menuIds.Add(metadataPreviewMenu.Id);

            // 14. 系统监控
            var monitorManagementMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = "Monitor",
                Title = "系统监控",
                Path = "/monitor",
                Component = "Layout",
                Icon = "Monitor",
                Type = "Directory",
                Sort = 14,
                Level = 1,
                TreePath = "14",
                Status = 1,
                IsVisible = true,
                Permission = "monitor.view",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(monitorManagementMenu);
            menuIds.Add(monitorManagementMenu.Id);

            var onlineUserMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = monitorManagementMenu.Id,
                Name = "OnlineUser",
                Title = "在线用户",
                Path = "online",
                Component = "@/views/monitor/online/index.vue",
                Icon = "UserFilled",
                Type = "Menu",
                Sort = 1,
                Level = 2,
                TreePath = "14.1",
                Status = 1,
                IsVisible = true,
                Permission = "monitor.online",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(onlineUserMenu);
            menuIds.Add(onlineUserMenu.Id);

            var systemLogsMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = monitorManagementMenu.Id,
                Name = "SystemLogs",
                Title = "系统日志",
                Path = "logs",
                Component = "@/views/monitor/logs/index.vue",
                Icon = "Document",
                Type = "Menu",
                Sort = 2,
                Level = 2,
                TreePath = "14.2",
                Status = 1,
                IsVisible = true,
                Permission = "monitor.logs",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(systemLogsMenu);
            menuIds.Add(systemLogsMenu.Id);

            var errorMonitorMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = monitorManagementMenu.Id,
                Name = "ErrorMonitor",
                Title = "错误监控",
                Path = "errors",
                Component = "@/views/monitor/errors/index.vue",
                Icon = "Warning",
                Type = "Menu",
                Sort = 3,
                Level = 2,
                TreePath = "14.3",
                Status = 1,
                IsVisible = true,
                Permission = "monitor.errors",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(errorMonitorMenu);
            menuIds.Add(errorMonitorMenu.Id);

            var performanceMenu = new Menu
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ParentId = monitorManagementMenu.Id,
                Name = "Performance",
                Title = "性能监控",
                Path = "performance",
                Component = "@/views/monitor/performance/index.vue",
                Icon = "TrendCharts",
                Type = "Menu",
                Sort = 4,
                Level = 2,
                TreePath = "14.4",
                Status = 1,
                IsVisible = true,
                Permission = "monitor.performance",
                ShowBreadcrumb = true,
                ShowInMenu = true,
                ShowInTabs = true,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            };
            _context.Menus.Add(performanceMenu);
            menuIds.Add(performanceMenu.Id);

            // 保存所有菜单
            await _context.SaveChangesAsync();

            return menuIds;
        }

        /// <summary>
        /// 为角色分配菜单
        /// </summary>
        private async Task AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds, Guid operatorId)
        {
            // 清除现有的角色菜单关系
            var existingRoleMenus = await _context.RoleMenus
                .Where(rm => rm.RoleId == roleId)
                .ToListAsync();

            if (existingRoleMenus.Any())
            {
                _context.RoleMenus.RemoveRange(existingRoleMenus);
            }

            // 创建新的角色菜单关系
            var now = DateTime.UtcNow;
            var roleMenus = menuIds.Select(menuId => new RoleMenu
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                MenuId = menuId,
                CreatedAt = now,
                CreatedBy = operatorId.ToString()
            }).ToList();

            _context.RoleMenus.AddRange(roleMenus);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"为角色 {roleId} 分配了 {menuIds.Count} 个菜单");
        }
    }
}