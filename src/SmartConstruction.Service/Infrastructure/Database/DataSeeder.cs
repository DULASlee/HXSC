using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using System.Security.Cryptography;
using System.Text;

namespace SmartConstruction.Service.Infrastructure.Database
{
    /// <summary>
    /// 数据种子服务
    /// </summary>
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 执行种子数据填充
        /// </summary>
        public async Task SeedAsync()
        {
            // 如果已经有用户了，就认为种子数据已填充
            if (await _context.Users.AnyAsync())
            {
                return;
            }

            // 使用固定的GUID用于种子数据，确保数据一致性
            var systemTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var adminUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var userUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var auditorUserId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var adminRoleId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var userRoleId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var auditorRoleId = Guid.Parse("77777777-7777-7777-7777-777777777777");

            // 种子租户
            var tenants = new[] {
                new Tenant
                {
                    Id = systemTenantId,
                    Code = "SYSTEM",
                    Name = "系统管理租户",
                    Status = 1,
                    IsolationMode = "Shared",
                    Logo = "/logo.png",
                    Theme = "default",
                    CreatedAt = DateTime.UtcNow
                }
            };
            await _context.Tenants.AddRangeAsync(tenants);

            // 种子角色
            var roles = new[] {
                new Role { Id = adminRoleId, TenantId = systemTenantId, Code = "ADMIN", Name = "管理员", DataScope = "All", Status = 1, IsSystem = true, Sort = 1, CreatedAt = DateTime.UtcNow },
                new Role { Id = userRoleId, TenantId = systemTenantId, Code = "USER", Name = "普通用户", DataScope = "Self", Status = 1, IsSystem = false, Sort = 2, CreatedAt = DateTime.UtcNow },
                new Role { Id = auditorRoleId, TenantId = systemTenantId, Code = "AUDITOR", Name = "审计员", DataScope = "Organization", Status = 1, IsSystem = false, Sort = 3, CreatedAt = DateTime.UtcNow }
            };
            await _context.Roles.AddRangeAsync(roles);
            
            // 种子用户（密码为Admin@123的SHA256哈希）
            var passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes("Admin@123"));
            var users = new[] {
                new User { Id = adminUserId, TenantId = systemTenantId, Username = "admin", Email = "admin@smartconstruction.com", Mobile = "13800138000", PasswordHash = passwordHash, SecurityStamp = Guid.NewGuid().ToString(), DisplayName = "系统管理员", Status = 1, CreatedAt = DateTime.UtcNow },
                new User { Id = userUserId, TenantId = systemTenantId, Username = "zhangsan", Email = "zhangsan@smartconstruction.com", Mobile = "13800138001", PasswordHash = passwordHash, SecurityStamp = Guid.NewGuid().ToString(), DisplayName = "张三", Status = 1, CreatedAt = DateTime.UtcNow },
                new User { Id = auditorUserId, TenantId = systemTenantId, Username = "auditor001", Email = "auditor001@smartconstruction.com", Mobile = "13800138002", PasswordHash = passwordHash, SecurityStamp = Guid.NewGuid().ToString(), DisplayName = "审计员001", Status = 1, CreatedAt = DateTime.UtcNow }
            };
            await _context.Users.AddRangeAsync(users);
            
            // 种子用户角色关系
            var userRoles = new[] {
                new UserRole { UserId = adminUserId, RoleId = adminRoleId, CreatedAt = DateTime.UtcNow },
                new UserRole { UserId = userUserId, RoleId = userRoleId, CreatedAt = DateTime.UtcNow },
                new UserRole { UserId = auditorUserId, RoleId = auditorRoleId, CreatedAt = DateTime.UtcNow }
            };
            await _context.UserRoles.AddRangeAsync(userRoles);

            // 种子权限
            var allPermissionId = Guid.Parse("88888888-8888-8888-8888-888888888888");
            var dashboardPermId = Guid.Parse("89888888-8888-8888-8888-888888888888");
            var digitalTwinPermId = Guid.Parse("8A888888-8888-8888-8888-888888888888");
            var projectPermId = Guid.Parse("8B888888-8888-8888-8888-888888888888");
            var userPermId = Guid.Parse("8C888888-8888-8888-8888-888888888888");
            var devicePermId = Guid.Parse("8D888888-8888-8888-8888-888888888888");
            var systemPermId = Guid.Parse("8E888888-8888-8888-8888-888888888888");

            var permissions = new[] {
                new Permission { Id = allPermissionId, TenantId = systemTenantId, Code = "*", Name = "所有权限", Type = "System", Status = 1, IsSystem = true, Sort = 1, CreatedAt = DateTime.UtcNow },
                new Permission { Id = systemPermId, TenantId = systemTenantId, Code = "system", Name = "系统管理", Type = "System", Status = 1, IsSystem = true, Sort = 2, CreatedAt = DateTime.UtcNow },
                new Permission { Id = dashboardPermId, TenantId = systemTenantId, Code = "dashboard.view", Name = "查看工作台", Type = "Menu", Status = 1, Sort = 10, CreatedAt = DateTime.UtcNow },
                new Permission { Id = digitalTwinPermId, TenantId = systemTenantId, Code = "digital-twin.view", Name = "查看数字孪生", Type = "Menu", Status = 1, Sort = 20, CreatedAt = DateTime.UtcNow },
                new Permission { Id = projectPermId, TenantId = systemTenantId, Code = "project.view", Name = "查看项目", Type = "Menu", Status = 1, Sort = 30, CreatedAt = DateTime.UtcNow },
                new Permission { Id = userPermId, TenantId = systemTenantId, Code = "user.view", Name = "查看用户", Type = "Menu", Status = 1, Sort = 40, CreatedAt = DateTime.UtcNow },
                new Permission { Id = devicePermId, TenantId = systemTenantId, Code = "device.view", Name = "查看设备", Type = "Menu", Status = 1, Sort = 50, CreatedAt = DateTime.UtcNow }
            };
            await _context.Permissions.AddRangeAsync(permissions);

            // 种子菜单
            var dashboardMenuId = Guid.Parse("91111111-1111-1111-1111-111111111111");
            var digitalTwinMenuId = Guid.Parse("92222222-2222-2222-2222-222222222222");
            var projectMenuId = Guid.Parse("93333333-3333-3333-3333-333333333333");
            var workerMenuId = Guid.Parse("94444444-4444-4444-4444-444444444444");
            var deviceMenuId = Guid.Parse("95555555-5555-5555-5555-555555555555");
            var systemMenuId = Guid.Parse("96666666-6666-6666-6666-666666666666");

            var menus = new[] {
                new Menu { Id = dashboardMenuId, TenantId = systemTenantId, Name = "工作台", Path = "/dashboard", Icon = "Dashboard", Sort = 1, Type = "Menu", Status = 1, Permission = "dashboard.view", CreatedAt = DateTime.UtcNow },
                new Menu { Id = digitalTwinMenuId, TenantId = systemTenantId, Name = "数字孪生", Path = "/digital-twin", Icon = "Monitor", Sort = 2, Type = "Menu", Status = 1, Permission = "digital-twin.view", CreatedAt = DateTime.UtcNow },
                new Menu { Id = projectMenuId, TenantId = systemTenantId, Name = "项目管理", Path = "/project", Icon = "OfficeBuilding", Sort = 3, Type = "Menu", Status = 1, Permission = "project.view", CreatedAt = DateTime.UtcNow },
                new Menu { Id = workerMenuId, TenantId = systemTenantId, Name = "人员管理", Path = "/worker", Icon = "User", Sort = 4, Type = "Menu", Status = 1, Permission = "user.view", CreatedAt = DateTime.UtcNow },
                new Menu { Id = deviceMenuId, TenantId = systemTenantId, Name = "设备管理", Path = "/device", Icon = "Setting", Sort = 5, Type = "Menu", Status = 1, Permission = "device.view", CreatedAt = DateTime.UtcNow },
                new Menu { Id = systemMenuId, TenantId = systemTenantId, Name = "系统管理", Path = "/system", Icon = "Tools", Sort = 99, Type = "Directory", Status = 1, Permission = "system", CreatedAt = DateTime.UtcNow }
            };
            await _context.Menus.AddRangeAsync(menus);

            // 种子角色权限关系
            var rolePermissions = new[] {
                new RolePermission { RoleId = adminRoleId, PermissionId = allPermissionId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = userRoleId, PermissionId = dashboardPermId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = userRoleId, PermissionId = digitalTwinPermId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = userRoleId, PermissionId = projectPermId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = auditorRoleId, PermissionId = dashboardPermId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = auditorRoleId, PermissionId = digitalTwinPermId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = auditorRoleId, PermissionId = projectPermId, CreatedAt = DateTime.UtcNow },
                new RolePermission { RoleId = auditorRoleId, PermissionId = userPermId, CreatedAt = DateTime.UtcNow }
            };
            await _context.RolePermissions.AddRangeAsync(rolePermissions);

            // 种子角色菜单关系
            var roleMenus = new[] {
                new RoleMenu { RoleId = adminRoleId, MenuId = dashboardMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = adminRoleId, MenuId = digitalTwinMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = adminRoleId, MenuId = projectMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = adminRoleId, MenuId = workerMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = adminRoleId, MenuId = deviceMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = adminRoleId, MenuId = systemMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = userRoleId, MenuId = dashboardMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = userRoleId, MenuId = digitalTwinMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = userRoleId, MenuId = projectMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = auditorRoleId, MenuId = dashboardMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = auditorRoleId, MenuId = digitalTwinMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = auditorRoleId, MenuId = projectMenuId, CreatedAt = DateTime.UtcNow },
                new RoleMenu { RoleId = auditorRoleId, MenuId = workerMenuId, CreatedAt = DateTime.UtcNow }
            };
            await _context.RoleMenus.AddRangeAsync(roleMenus);

            await _context.SaveChangesAsync();
        }
    }
} 