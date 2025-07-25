using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;

namespace SmartConstruction.Service.Data
{
    /// <summary>
    /// 应用程序数据库上下文
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 权限管理实体集合
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Authorization> Authorizations { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<TenantIsolation> TenantIsolations { get; set; }
        
        // 关联关系实体集合
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserMenu> UserMenus { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        // 业务实体集合
        public DbSet<Company> Companies { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<SafetyIncident> SafetyIncidents { get; set; }
        public DbSet<TeamProject> TeamProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置实体关系
            ConfigureUserRelations(modelBuilder);
            ConfigureRoleRelations(modelBuilder);
            ConfigureBusinessRelations(modelBuilder);

            // 初始化种子数据
            // SeedData(modelBuilder); // 种子数据逻辑已迁移到 DataSeeder.cs
        }

        /// <summary>
        /// 配置用户相关实体关系
        /// </summary>
        private static void ConfigureUserRelations(ModelBuilder modelBuilder)
        {
            // 用户-租户关系
            modelBuilder.Entity<User>()
                .HasOne(u => u.Tenant)
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 用户-组织关系
            modelBuilder.Entity<User>()
                .HasOne(u => u.Organization)
                .WithMany()
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);

            // 用户-角色多对多关系
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 用户-权限关系
            modelBuilder.Entity<UserPermission>()
                .HasKey(up => new { up.UserId, up.PermissionId });

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.GrantedByUser)
                .WithMany()
                .HasForeignKey(up => up.GrantedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // 用户-菜单关系
            modelBuilder.Entity<UserMenu>()
                .HasKey(um => new { um.UserId, um.MenuId });

            modelBuilder.Entity<UserMenu>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMenus)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserMenu>()
                .HasOne(um => um.GrantedByUser)
                .WithMany()
                .HasForeignKey(um => um.GrantedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserPermission>()
                .HasOne(up => up.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(up => up.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 用户-菜单关系
            modelBuilder.Entity<UserMenu>()
                .HasKey(um => new { um.UserId, um.MenuId });

            modelBuilder.Entity<UserMenu>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMenus)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserMenu>()
                .HasOne(um => um.Menu)
                .WithMany(m => m.UserMenus)
                .HasForeignKey(um => um.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        /// <summary>
        /// 配置角色相关实体关系
        /// </summary>
        private static void ConfigureRoleRelations(ModelBuilder modelBuilder)
        {
            // 角色-租户关系
            modelBuilder.Entity<Role>()
                .HasOne(r => r.Tenant)
                .WithMany()
                .HasForeignKey(r => r.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // 角色-权限多对多关系
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

            // 角色-菜单多对多关系
            modelBuilder.Entity<RoleMenu>()
                .HasKey(rm => new { rm.RoleId, rm.MenuId });

            modelBuilder.Entity<RoleMenu>()
                .HasOne(rm => rm.Role)
                .WithMany(r => r.RoleMenus)
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RoleMenu>()
                .HasOne(rm => rm.Menu)
                .WithMany(m => m.RoleMenus)
                .HasForeignKey(rm => rm.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // 权限层级关系
            modelBuilder.Entity<Permission>()
                .HasOne(p => p.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(p => p.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 菜单层级关系
            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置索引
            modelBuilder.Entity<User>()
                .HasIndex(u => new { u.TenantId, u.Username })
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => new { r.TenantId, r.Code })
                .IsUnique();

            modelBuilder.Entity<Permission>()
                .HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique();

            modelBuilder.Entity<Menu>()
                .HasIndex(m => new { m.TenantId, m.Path })
                .IsUnique();

            // 刷新令牌表配置
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.ReplacedByToken)
                .IsRequired(false); // 设置为可空

            // 刷新令牌表索引
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => new { rt.UserId, rt.DeviceId });
        }

        /// <summary>
        /// 配置业务相关实体关系
        /// </summary>
        private static void ConfigureBusinessRelations(ModelBuilder modelBuilder)
        {
            // Team 和 Worker 的关系
            modelBuilder.Entity<Team>()
                .HasOne(t => t.TeamLeader)
                .WithMany(w => w.LeadingTeams)
                .HasForeignKey(t => t.TeamLeaderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Worker>()
                .HasOne(w => w.Team)
                .WithMany(t => t.Workers)
                .HasForeignKey(w => w.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            // TeamProject 关系配置
            modelBuilder.Entity<TeamProject>()
                .HasOne(tp => tp.Team)
                .WithMany(t => t.TeamProjects)
                .HasForeignKey(tp => tp.TeamId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TeamProject>()
                .HasOne(tp => tp.Project)
                .WithMany(p => p.TeamProjects)
                .HasForeignKey(tp => tp.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            // 配置租户隔离
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;
                if (typeof(BaseEntity).IsAssignableFrom(clrType))
                {
                    // 为所有继承自BaseEntity的实体添加TenantId查询过滤器
                    // 这里暂时注释，生产环境可以启用
                    // modelBuilder.Entity(clrType).HasQueryFilter(/* 添加租户过滤器 */);
                }
            }
        }

        // 种子数据逻辑已迁移到 DataSeeder.cs
    }
}