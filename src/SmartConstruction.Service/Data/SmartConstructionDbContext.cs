using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Data; // 修正命名空间

/// <summary>
/// 智慧工地物联网(IoT)数据上下文
/// </summary>
public class SmartConstructionDbContext : DbContext
{
    public SmartConstructionDbContext(DbContextOptions<SmartConstructionDbContext> options) : base(options)
    {
    }

    // IoT业务实体集合
    public DbSet<Company> Companies { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<WorkerAttendanceProfile> WorkerAttendanceProfiles { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<SafetyIncident> SafetyIncidents { get; set; }
    public DbSet<GovernmentData> GovernmentData { get; set; }
    public DbSet<SystemLog> SystemLogs { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<DeviceMaintenanceRecord> DeviceMaintenanceRecords { get; set; }
    public DbSet<SafetyTrainingRecord> SafetyTrainingRecords { get; set; }
    public DbSet<LowCodeForm> LowCodeForms { get; set; }
    public DbSet<TeamProject> TeamProjects { get; set; }

    // System & Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // Relational Entities
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserMenu> UserMenus { get; set; }
    public DbSet<UserPermission> UserPermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 配置IoT业务实体关系

        // 公司实体配置
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("construction_company");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("company_id");
            entity.Property(e => e.CompanyName).HasColumnName("company_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.UnifiedSocialCreditCode).HasColumnName("unified_social_credit_code").IsRequired().HasMaxLength(18);
            entity.Property(e => e.LegalRepresentative).HasColumnName("legal_representative").IsRequired().HasMaxLength(50);
            entity.Property(e => e.ContactPhone).HasColumnName("contact_phone").IsRequired().HasMaxLength(20);
            entity.Property(e => e.RegisteredAddress).HasColumnName("registered_address").IsRequired().HasMaxLength(200);
            entity.Property(e => e.BusinessLicenseImg).HasColumnName("business_license_img").HasMaxLength(255);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            // 唯一索引
            entity.HasIndex(e => e.CompanyName).IsUnique();
            entity.HasIndex(e => e.UnifiedSocialCreditCode).IsUnique();
        });

        // 项目实体配置
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("construction_project");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("project_id");
            entity.Property(e => e.ProjectCode).HasColumnName("project_code").IsRequired().HasMaxLength(50);
            entity.Property(e => e.ProjectName).HasColumnName("project_name").IsRequired().HasMaxLength(200);
            entity.Property(e => e.CompanyId).HasColumnName("company_id").IsRequired();
            entity.Property(e => e.ProjectAddress).HasColumnName("project_address").IsRequired().HasMaxLength(200);
            entity.Property(e => e.ProjectManager).HasColumnName("project_manager").IsRequired().HasMaxLength(50);
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.PlannedEndDate).HasColumnName("planned_end_date");
            entity.Property(e => e.ActualEndDate).HasColumnName("actual_end_date");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ContractAmount).HasColumnName("contract_amount").HasPrecision(15, 2);
            entity.Property(e => e.ProjectLicenseImg).HasColumnName("project_license_img").HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => e.ProjectCode).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.CompanyId);
            entity.HasIndex(e => e.Status);

            // 外键关系
            entity.HasOne(e => e.Company)
                .WithMany(c => c.Projects)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 班组实体配置
        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("work_team");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("team_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.TeamName).HasColumnName("team_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.TeamLeaderId).HasColumnName("team_leader_id");
            entity.Property(e => e.Specialty).HasColumnName("specialty").IsRequired().HasMaxLength(50);
            entity.Property(e => e.TotalMembers).HasColumnName("total_members").HasDefaultValue(0);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.TeamLeaderId);

            // 外键关系
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Teams)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TeamLeader)
                .WithMany(w => w.LeadingTeams)
                .HasForeignKey(e => e.TeamLeaderId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 工人实体配置
        modelBuilder.Entity<Worker>(entity =>
        {
            entity.ToTable("worker");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("worker_id");
            entity.Property(e => e.IdCardNumber).HasColumnName("id_card_number").IsRequired().HasMaxLength(18);
            entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Gender).HasColumnName("gender").IsRequired().HasMaxLength(1);
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Nationality).HasColumnName("nationality").HasMaxLength(50).HasDefaultValue("中国");
            entity.Property(e => e.Ethnicity).HasColumnName("ethnicity").HasMaxLength(20);
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number").IsRequired().HasMaxLength(20);
            entity.Property(e => e.HometownAddress).HasColumnName("hometown_address").HasMaxLength(200);
            entity.Property(e => e.Specialty).HasColumnName("specialty").IsRequired().HasMaxLength(50);
            entity.Property(e => e.CertificateNo).HasColumnName("certificate_no").HasMaxLength(50);
            entity.Property(e => e.BankName).HasColumnName("bank_name").HasMaxLength(100);
            entity.Property(e => e.BankAccount).HasColumnName("bank_account").HasMaxLength(50);
            entity.Property(e => e.EmergencyContact).HasColumnName("emergency_contact").HasMaxLength(50);
            entity.Property(e => e.EmergencyPhone).HasColumnName("emergency_phone").HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            // 唯一索引
            entity.HasIndex(e => e.IdCardNumber).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.TeamId);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.Specialty);

            // 外键关系
            entity.HasOne(e => e.Team)
                .WithMany(t => t.Workers)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.Workers)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 工人实名制考勤资料实体配置
        modelBuilder.Entity<WorkerAttendanceProfile>(entity =>
        {
            entity.ToTable("worker_attendance_profile");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("profile_id");
            entity.Property(e => e.WorkerId).HasColumnName("worker_id").IsRequired();
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.FaceImage).HasColumnName("face_image").IsRequired().HasMaxLength(255);
            entity.Property(e => e.IdCardFrontImg).HasColumnName("id_card_front_img").IsRequired().HasMaxLength(255);
            entity.Property(e => e.IdCardBackImg).HasColumnName("id_card_back_img").IsRequired().HasMaxLength(255);
            entity.Property(e => e.ContractImg).HasColumnName("contract_img").HasMaxLength(255);
            entity.Property(e => e.TrainingCertImg).HasColumnName("training_cert_img").HasMaxLength(255);
            entity.Property(e => e.HealthCertImg).HasColumnName("health_cert_img").HasMaxLength(255);
            entity.Property(e => e.IsVerified).HasColumnName("is_verified").IsRequired().HasDefaultValue(false);
            entity.Property(e => e.VerificationTime).HasColumnName("verification_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => e.WorkerId).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.IsVerified);

            // 外键关系
            entity.HasOne(e => e.Worker)
                .WithOne(w => w.AttendanceProfile)
                .HasForeignKey<WorkerAttendanceProfile>(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 工人考勤记录实体配置
        modelBuilder.Entity<AttendanceRecord>(entity =>
        {
            entity.ToTable("worker_attendance_record");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("record_id");
            entity.Property(e => e.WorkerId).HasColumnName("worker_id").IsRequired();
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.AttendanceDate).HasColumnName("attendance_date").IsRequired();
            entity.Property(e => e.ClockInTime).HasColumnName("clock_in_time");
            entity.Property(e => e.ClockOutTime).HasColumnName("clock_out_time");
            entity.Property(e => e.AttendanceType).HasColumnName("attendance_type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.DeviceId).HasColumnName("device_id").HasMaxLength(50);
            entity.Property(e => e.Location).HasColumnName("location").HasMaxLength(100);
            entity.Property(e => e.Source).HasColumnName("source").IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsSynced).HasColumnName("is_synced").IsRequired().HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => new { e.WorkerId, e.AttendanceDate });
            entity.HasIndex(e => new { e.ProjectId, e.AttendanceDate });
            entity.HasIndex(e => new { e.AttendanceDate, e.AttendanceType });
            entity.HasIndex(e => e.IsSynced);

            // 外键关系
            entity.HasOne(e => e.Worker)
                .WithMany(w => w.AttendanceRecords)
                .HasForeignKey(e => e.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.AttendanceRecords)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Team)
                .WithMany()
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 设备实体配置
        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("device");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("device_id");
            entity.Property(e => e.DeviceCode).HasColumnName("device_code").IsRequired().HasMaxLength(50);
            entity.Property(e => e.DeviceName).HasColumnName("device_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.DeviceType).HasColumnName("device_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Model).HasColumnName("model").HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasColumnName("manufacturer").HasMaxLength(100);
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.Location).HasColumnName("location").HasMaxLength(100);
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(50);
            entity.Property(e => e.MacAddress).HasColumnName("mac_address").HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.LastOnlineTime).HasColumnName("last_online_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            // 索引
            entity.HasIndex(e => e.DeviceCode);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.DeviceType);
            entity.HasIndex(e => e.Status);

            // 外键关系
            entity.HasOne(e => e.Project)
                .WithMany(p => p.Devices)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 安全事故实体配置
        modelBuilder.Entity<SafetyIncident>(entity =>
        {
            entity.ToTable("safety_incident");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Level).HasColumnName("level").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Location).HasColumnName("location").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasColumnName("description").IsRequired().HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasColumnName("image_url").HasMaxLength(255);
            entity.Property(e => e.DetectedTime).HasColumnName("detected_time").IsRequired();
            entity.Property(e => e.IsHandled).HasColumnName("is_handled").IsRequired().HasDefaultValue(false);
            entity.Property(e => e.HandledTime).HasColumnName("handled_time");
            entity.Property(e => e.HandledBy).HasColumnName("handled_by").HasMaxLength(50);
            entity.Property(e => e.HandlingResult).HasColumnName("handling_result").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

            // 索引
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.IsHandled);
            entity.HasIndex(e => e.DetectedTime);

            // 外键关系
            entity.HasOne(e => e.Project)
                .WithMany(p => p.SafetyIncidents)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 政府监管数据实体配置
        modelBuilder.Entity<GovernmentData>(entity =>
        {
            entity.ToTable("government_data");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.DataType).HasColumnName("data_type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.ReportTime).HasColumnName("report_time").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.FailReason).HasColumnName("fail_reason").HasMaxLength(500);
            entity.Property(e => e.RetryCount).HasColumnName("retry_count").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.LastRetryTime).HasColumnName("last_retry_time");
            entity.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
            entity.Property(e => e.UpdateTime).HasColumnName("update_time");

            // 索引
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.DataType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.ReportTime);

            // 外键关系
            entity.HasOne(e => e.Project)
                .WithMany()
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 系统日志实体配置
        modelBuilder.Entity<SystemLog>(entity =>
        {
            entity.ToTable("system_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Level).HasColumnName("level").IsRequired().HasMaxLength(10);
            entity.Property(e => e.Message).HasColumnName("message").IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Exception).HasColumnName("exception").HasMaxLength(2000);
            entity.Property(e => e.Source).HasColumnName("source").HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.RequestPath).HasColumnName("request_path").HasMaxLength(200);
            entity.Property(e => e.RequestMethod).HasColumnName("request_method").HasMaxLength(10);
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
            entity.Property(e => e.ExecutionTime).HasColumnName("execution_time");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => e.Level);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CreateTime);
        });

        // 审计日志实体配置
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("audit_log");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.EventType).HasColumnName("event_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.EntityName).HasColumnName("entity_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.EntityId).HasColumnName("entity_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.OldValues).HasColumnName("old_values");
            entity.Property(e => e.NewValues).HasColumnName("new_values");
            entity.Property(e => e.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
            entity.Property(e => e.UserAgent).HasColumnName("user_agent").HasMaxLength(500);
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => e.EntityName);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CreatedAt);
        });

        // 设备维护记录实体配置
        modelBuilder.Entity<DeviceMaintenanceRecord>(entity =>
        {
            entity.ToTable("device_maintenance_record");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DeviceId).HasColumnName("device_id").IsRequired();
            entity.Property(e => e.MaintenanceType).HasColumnName("maintenance_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.MaintenanceDate).HasColumnName("maintenance_date").IsRequired();
            entity.Property(e => e.Maintainer).HasColumnName("maintenance_person").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Cost).HasColumnName("cost").HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => e.DeviceId);
            entity.HasIndex(e => e.MaintenanceType);
            entity.HasIndex(e => e.MaintenanceDate);

            // 外键关系
            entity.HasOne(e => e.Device)
                .WithMany(d => d.MaintenanceRecords)
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 安全培训记录实体配置
        modelBuilder.Entity<SafetyTrainingRecord>(entity =>
        {
            entity.ToTable("safety_training_record");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.Topic).HasColumnName("topic").IsRequired().HasMaxLength(200);
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.Trainer).HasColumnName("trainer").IsRequired().HasMaxLength(50);
            entity.Property(e => e.TrainingTime).HasColumnName("training_time").IsRequired();
            entity.Property(e => e.Location).HasColumnName("location").IsRequired().HasMaxLength(100);
            entity.Property(e => e.ParticipantCount).HasColumnName("participant_count").IsRequired();
            entity.Property(e => e.Duration).HasColumnName("duration").IsRequired().HasPrecision(5, 2);
            entity.Property(e => e.TrainingType).HasColumnName("training_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.TrainingType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.TrainingTime);

            // 外键关系
            entity.HasOne(e => e.Project)
                .WithMany(p => p.SafetyTrainingRecords)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 低代码表单实体配置
        modelBuilder.Entity<LowCodeForm>(entity =>
        {
            entity.ToTable("low_code_form");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FormName).HasColumnName("form_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.FormCode).HasColumnName("form_code").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.FormConfig).HasColumnName("form_config").IsRequired();
            entity.Property(e => e.FormType).HasColumnName("form_type").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Version).HasColumnName("version").IsRequired();
            entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.IsSystemForm).HasColumnName("is_system_form").IsRequired();
            entity.Property(e => e.SortOrder).HasColumnName("sort_order").IsRequired();
            entity.Property(e => e.CreateTime).HasColumnName("create_time").IsRequired();
            entity.Property(e => e.UpdateTime).HasColumnName("update_time");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => e.FormCode).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.FormType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedBy);
            entity.HasIndex(e => e.TenantId);
        });

        // 班组项目关联实体配置
        modelBuilder.Entity<TeamProject>(entity =>
        {
            entity.ToTable("team_project");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TeamId).HasColumnName("team_id").IsRequired();
            entity.Property(e => e.ProjectId).HasColumnName("project_id").IsRequired();
            entity.Property(e => e.AssignedDate).HasColumnName("assigned_date").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.TeamId, e.ProjectId }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.TeamId);
            entity.HasIndex(e => e.ProjectId);
            entity.HasIndex(e => e.Status);

            // 外键关系
            entity.HasOne(e => e.Team)
                .WithMany(t => t.TeamProjects)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Project)
                .WithMany(p => p.TeamProjects)
                .HasForeignKey(e => e.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 用户菜单关联实体配置
        modelBuilder.Entity<UserMenu>(entity =>
        {
            entity.ToTable("user_menu");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.MenuId).HasColumnName("menu_id").IsRequired();
            entity.Property(e => e.AccessType).HasColumnName("access_type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Source).HasColumnName("source").IsRequired().HasMaxLength(20);
            entity.Property(e => e.DataScope).HasColumnName("data_scope").HasMaxLength(20);
            entity.Property(e => e.DataScopeConditions).HasColumnName("data_scope_conditions");
            entity.Property(e => e.EffectiveFrom).HasColumnName("effective_from");
            entity.Property(e => e.EffectiveTo).HasColumnName("effective_to");
            entity.Property(e => e.GrantedBy).HasColumnName("granted_by");
            entity.Property(e => e.GrantedAt).HasColumnName("granted_at");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.UserId, e.MenuId }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.MenuId);
            entity.HasIndex(e => e.Status);

            // 外键关系
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserMenus)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Menu)
                .WithMany()
                .HasForeignKey(e => e.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.GrantedByUser)
                .WithMany()
                .HasForeignKey(e => e.GrantedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 用户角色关联实体配置
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_role");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.UserId, e.RoleId }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RoleId);

            // 外键关系
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 角色菜单关联实体配置
        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.ToTable("role_menu");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
            entity.Property(e => e.MenuId).HasColumnName("menu_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.RoleId, e.MenuId }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.RoleId);
            entity.HasIndex(e => e.MenuId);

            // 外键关系
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RoleMenus)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Menu)
                .WithMany(m => m.RoleMenus)
                .HasForeignKey(e => e.MenuId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 角色权限关联实体配置
        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.ToTable("role_permission");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id").IsRequired();
            entity.Property(e => e.PermissionId).HasColumnName("permission_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.RoleId);
            entity.HasIndex(e => e.PermissionId);

            // 外键关系
            entity.HasOne(e => e.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 用户权限关联实体配置
        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.ToTable("user_permission");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.PermissionId).HasColumnName("permission_id").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.UserId, e.PermissionId }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.PermissionId);

            // 外键关系
            entity.HasOne(e => e.User)
                .WithMany(u => u.UserPermissions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Permission)
                .WithMany(p => p.UserPermissions)
                .HasForeignKey(e => e.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // 用户实体配置
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(e => e.Username).HasColumnName("username").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).HasColumnName("email").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Mobile).HasColumnName("mobile").IsRequired().HasMaxLength(20);
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash").IsRequired();
            entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp").IsRequired().HasMaxLength(100);
            entity.Property(e => e.DisplayName).HasColumnName("display_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.OrganizationId).HasColumnName("organization_id");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.LastLoginAt).HasColumnName("last_login_at");
            entity.Property(e => e.Avatar).HasColumnName("avatar").HasMaxLength(255);
            entity.Property(e => e.UserType).HasColumnName("user_type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.IsLocked).HasColumnName("is_locked").IsRequired();
            entity.Property(e => e.LockedAt).HasColumnName("locked_at");
            entity.Property(e => e.PasswordExpiresAt).HasColumnName("password_expires_at");
            entity.Property(e => e.LoginFailureCount).HasColumnName("login_failure_count").IsRequired();
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.TenantId, e.Username }).IsUnique();
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.UserType);

            // 外键关系
            entity.HasOne(e => e.Tenant)
                .WithMany()
                .HasForeignKey(e => e.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Organization)
                .WithMany()
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 角色实体配置
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("role");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Code).HasColumnName("code").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.Sort).HasColumnName("sort").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Status);
        });

        // 菜单实体配置
        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("menu");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(100);
            entity.Property(e => e.Path).HasColumnName("path").HasMaxLength(200);
            entity.Property(e => e.Component).HasColumnName("component").HasMaxLength(200);
            entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(50);
            entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Sort).HasColumnName("sort").IsRequired();
            entity.Property(e => e.Level).HasColumnName("level").IsRequired();
            entity.Property(e => e.TreePath).HasColumnName("tree_path").HasMaxLength(500);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.IsVisible).HasColumnName("is_visible").IsRequired();
            entity.Property(e => e.IsCache).HasColumnName("is_cache").IsRequired();
            entity.Property(e => e.IsAffix).HasColumnName("is_affix").IsRequired();
            entity.Property(e => e.ExternalLink).HasColumnName("external_link").HasMaxLength(255);
            entity.Property(e => e.Target).HasColumnName("target").HasMaxLength(20);
            entity.Property(e => e.Permission).HasColumnName("permission").HasMaxLength(100);
            entity.Property(e => e.Meta).HasColumnName("meta");
            entity.Property(e => e.Redirect).HasColumnName("redirect").HasMaxLength(200);
            entity.Property(e => e.ShowBreadcrumb).HasColumnName("show_breadcrumb").IsRequired();
            entity.Property(e => e.ShowInMenu).HasColumnName("show_in_menu").IsRequired();
            entity.Property(e => e.ShowInTabs).HasColumnName("show_in_tabs").IsRequired();
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Sort);

            // 外键关系
            entity.HasOne(e => e.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 权限实体配置
        modelBuilder.Entity<Permission>(entity =>
        {
            entity.ToTable("permission");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).HasColumnName("code").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.TreePath).HasColumnName("tree_path").HasMaxLength(500);
            entity.Property(e => e.Level).HasColumnName("level").IsRequired();
            entity.Property(e => e.Sort).HasColumnName("sort").IsRequired();
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.IsSystem).HasColumnName("is_system").IsRequired();
            entity.Property(e => e.ApiPath).HasColumnName("api_path").HasMaxLength(200);
            entity.Property(e => e.HttpMethods).HasColumnName("http_methods").HasMaxLength(100);
            entity.Property(e => e.ResourceIdentifier).HasColumnName("resource_identifier").HasMaxLength(100);
            entity.Property(e => e.Conditions).HasColumnName("conditions");
            entity.Property(e => e.Icon).HasColumnName("icon").HasMaxLength(50);
            entity.Property(e => e.Color).HasColumnName("color").HasMaxLength(20);
            entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);
            entity.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);

            // 外键关系
            entity.HasOne(e => e.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 组织实体配置
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.ToTable("organization");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).HasColumnName("code").IsRequired().HasMaxLength(50);
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.TreePath).HasColumnName("tree_path").HasMaxLength(500);
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Type).HasColumnName("type").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.SortOrder).HasColumnName("sort_order").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.ParentId);
            entity.HasIndex(e => e.Status);
        });

        // 租户实体配置
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.ToTable("tenant");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).HasColumnName("code").IsRequired().HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.IsolationMode).HasColumnName("isolation_mode").IsRequired().HasMaxLength(20);
            entity.Property(e => e.Logo).HasColumnName("logo").HasMaxLength(255);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => e.Code).IsUnique();
            entity.HasIndex(e => e.Status);
        });

        // 刷新令牌实体配置
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_token");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Token).HasColumnName("token").IsRequired().HasMaxLength(500);
            entity.Property(e => e.JwtId).HasColumnName("jwt_id").IsRequired().HasMaxLength(100);
            entity.Property(e => e.DeviceId).HasColumnName("device_id").HasMaxLength(100);
            entity.Property(e => e.DeviceType).HasColumnName("device_type").HasMaxLength(50);
            entity.Property(e => e.IssuedAt).HasColumnName("issued_at").IsRequired();
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at").IsRequired();
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.ReplacedByToken).HasColumnName("replaced_by_token").HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();

            // 唯一索引
            entity.HasIndex(e => e.Token).IsUnique();
            
            // 索引
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpiresAt);
            entity.HasIndex(e => e.JwtId);
        });

        // Apply all configurations from the current assembly that implement IEntityTypeConfiguration
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    // Don't auto-set Id as it's a long type and should be handled by the database
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    // Dynamically check if the entity has an UpdatedAt property
                    var property = entry.Entity.GetType().GetProperty("UpdatedAt");
                    if (property != null && property.PropertyType == typeof(DateTime?))
                    {
                        property.SetValue(entry.Entity, DateTime.UtcNow);
                    }
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }


} 