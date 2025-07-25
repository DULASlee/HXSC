using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartConstruction.Service.src.SmartConstruction.Service.Infrastructure.Migrations.IoTDb
{
    /// <inheritdoc />
    public partial class InitialCreateIoT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    event_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    entity_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    entity_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    old_values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "construction_company",
                columns: table => new
                {
                    company_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    company_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    unified_social_credit_code = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    legal_representative = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    contact_phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    registered_address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    business_license_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_construction_company", x => x.company_id);
                });

            migrationBuilder.CreateTable(
                name: "low_code_form",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    form_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    form_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    form_config = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    form_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    version = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_system_form = table.Column<bool>(type: "bit", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: false),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_low_code_form", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_log",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    level = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    exception = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    request_path = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    request_method = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    user_agent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    execution_time = table.Column<long>(type: "bigint", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "construction_project",
                columns: table => new
                {
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    project_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    project_manager = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    planned_end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    actual_end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    contract_amount = table.Column<decimal>(type: "decimal(15,2)", precision: 15, scale: 2, nullable: true),
                    project_license_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_construction_project", x => x.project_id);
                    table.ForeignKey(
                        name: "FK_construction_project_construction_company_company_id",
                        column: x => x.company_id,
                        principalTable: "construction_company",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "device",
                columns: table => new
                {
                    device_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    device_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    device_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    device_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ip_address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    mac_address = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    last_online_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device", x => x.device_id);
                    table.ForeignKey(
                        name: "FK_device_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "government_data",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    data_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    report_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fail_reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    retry_count = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    last_retry_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    create_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    update_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_government_data", x => x.id);
                    table.ForeignKey(
                        name: "FK_government_data_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "safety_incident",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    detected_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_handled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    handled_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    handled_by = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    handling_result = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_safety_incident", x => x.id);
                    table.ForeignKey(
                        name: "FK_safety_incident_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "safety_training_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    topic = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trainer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    training_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    participant_count = table.Column<int>(type: "int", nullable: false),
                    duration = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    training_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_safety_training_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_safety_training_record_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "device_maintenance_record",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    device_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    maintenance_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    maintenance_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    maintenance_person = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Result = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextMaintenanceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    cost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_device_maintenance_record", x => x.id);
                    table.ForeignKey(
                        name: "FK_device_maintenance_record_device_device_id",
                        column: x => x.device_id,
                        principalTable: "device",
                        principalColumn: "device_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "team_project",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    team_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_project", x => x.id);
                    table.ForeignKey(
                        name: "FK_team_project_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_team",
                columns: table => new
                {
                    team_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    team_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    team_leader_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    specialty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    total_members = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    status = table.Column<byte>(type: "tinyint", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_team", x => x.team_id);
                    table.ForeignKey(
                        name: "FK_work_team_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "worker",
                columns: table => new
                {
                    worker_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    id_card_number = table.Column<string>(type: "nvarchar(18)", maxLength: 18, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    gender = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    birth_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "中国"),
                    ethnicity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    phone_number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hometown_address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    specialty = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    certificate_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    bank_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bank_account = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    emergency_contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    emergency_phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    status = table.Column<byte>(type: "tinyint", maxLength: 20, nullable: false),
                    team_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker", x => x.worker_id);
                    table.ForeignKey(
                        name: "FK_worker_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_worker_work_team_team_id",
                        column: x => x.team_id,
                        principalTable: "work_team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "worker_attendance_profile",
                columns: table => new
                {
                    profile_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    worker_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    face_image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    id_card_front_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    id_card_back_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    contract_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    training_cert_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    health_cert_img = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_verified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    verification_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_attendance_profile", x => x.profile_id);
                    table.ForeignKey(
                        name: "FK_worker_attendance_profile_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_worker_attendance_profile_worker_worker_id",
                        column: x => x.worker_id,
                        principalTable: "worker",
                        principalColumn: "worker_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "worker_attendance_record",
                columns: table => new
                {
                    record_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    worker_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    project_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    team_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    attendance_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    clock_in_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    clock_out_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    attendance_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    device_id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    source = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    is_synced = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsLate = table.Column<bool>(type: "bit", nullable: false),
                    IsEarlyLeave = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_attendance_record", x => x.record_id);
                    table.ForeignKey(
                        name: "FK_worker_attendance_record_construction_project_project_id",
                        column: x => x.project_id,
                        principalTable: "construction_project",
                        principalColumn: "project_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_worker_attendance_record_work_team_team_id",
                        column: x => x.team_id,
                        principalTable: "work_team",
                        principalColumn: "team_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_worker_attendance_record_worker_worker_id",
                        column: x => x.worker_id,
                        principalTable: "worker",
                        principalColumn: "worker_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_created_at",
                table: "audit_log",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_entity_name",
                table: "audit_log",
                column: "entity_name");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_event_type",
                table: "audit_log",
                column: "event_type");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_tenant_id",
                table: "audit_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_log_user_id",
                table: "audit_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_construction_company_company_name",
                table: "construction_company",
                column: "company_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_construction_company_unified_social_credit_code",
                table: "construction_company",
                column: "unified_social_credit_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_construction_project_company_id",
                table: "construction_project",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_construction_project_project_code",
                table: "construction_project",
                column: "project_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_construction_project_status",
                table: "construction_project",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_device_device_code",
                table: "device",
                column: "device_code");

            migrationBuilder.CreateIndex(
                name: "IX_device_device_type",
                table: "device",
                column: "device_type");

            migrationBuilder.CreateIndex(
                name: "IX_device_project_id",
                table: "device",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_status",
                table: "device",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_device_maintenance_record_device_id",
                table: "device_maintenance_record",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_device_maintenance_record_maintenance_date",
                table: "device_maintenance_record",
                column: "maintenance_date");

            migrationBuilder.CreateIndex(
                name: "IX_device_maintenance_record_maintenance_type",
                table: "device_maintenance_record",
                column: "maintenance_type");

            migrationBuilder.CreateIndex(
                name: "IX_government_data_data_type",
                table: "government_data",
                column: "data_type");

            migrationBuilder.CreateIndex(
                name: "IX_government_data_project_id",
                table: "government_data",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_government_data_report_time",
                table: "government_data",
                column: "report_time");

            migrationBuilder.CreateIndex(
                name: "IX_government_data_status",
                table: "government_data",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_low_code_form_created_by",
                table: "low_code_form",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_low_code_form_form_code",
                table: "low_code_form",
                column: "form_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_low_code_form_form_type",
                table: "low_code_form",
                column: "form_type");

            migrationBuilder.CreateIndex(
                name: "IX_low_code_form_status",
                table: "low_code_form",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_low_code_form_tenant_id",
                table: "low_code_form",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_safety_incident_detected_time",
                table: "safety_incident",
                column: "detected_time");

            migrationBuilder.CreateIndex(
                name: "IX_safety_incident_is_handled",
                table: "safety_incident",
                column: "is_handled");

            migrationBuilder.CreateIndex(
                name: "IX_safety_incident_level",
                table: "safety_incident",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_safety_incident_project_id",
                table: "safety_incident",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_safety_incident_type",
                table: "safety_incident",
                column: "type");

            migrationBuilder.CreateIndex(
                name: "IX_safety_training_record_project_id",
                table: "safety_training_record",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_safety_training_record_status",
                table: "safety_training_record",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_safety_training_record_training_time",
                table: "safety_training_record",
                column: "training_time");

            migrationBuilder.CreateIndex(
                name: "IX_safety_training_record_training_type",
                table: "safety_training_record",
                column: "training_type");

            migrationBuilder.CreateIndex(
                name: "IX_system_log_create_time",
                table: "system_log",
                column: "create_time");

            migrationBuilder.CreateIndex(
                name: "IX_system_log_level",
                table: "system_log",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_system_log_tenant_id",
                table: "system_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "IX_system_log_user_id",
                table: "system_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_project_project_id",
                table: "team_project",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_project_status",
                table: "team_project",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_team_project_team_id",
                table: "team_project",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_team_project_team_id_project_id",
                table: "team_project",
                columns: new[] { "team_id", "project_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_team_project_id",
                table: "work_team",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_work_team_team_leader_id",
                table: "work_team",
                column: "team_leader_id");

            migrationBuilder.CreateIndex(
                name: "IX_worker_id_card_number",
                table: "worker",
                column: "id_card_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_worker_phone_number",
                table: "worker",
                column: "phone_number");

            migrationBuilder.CreateIndex(
                name: "IX_worker_project_id",
                table: "worker",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_worker_specialty",
                table: "worker",
                column: "specialty");

            migrationBuilder.CreateIndex(
                name: "IX_worker_team_id",
                table: "worker",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_profile_is_verified",
                table: "worker_attendance_profile",
                column: "is_verified");

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_profile_project_id",
                table: "worker_attendance_profile",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_profile_worker_id",
                table: "worker_attendance_profile",
                column: "worker_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_record_attendance_date_attendance_type",
                table: "worker_attendance_record",
                columns: new[] { "attendance_date", "attendance_type" });

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_record_is_synced",
                table: "worker_attendance_record",
                column: "is_synced");

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_record_project_id_attendance_date",
                table: "worker_attendance_record",
                columns: new[] { "project_id", "attendance_date" });

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_record_team_id",
                table: "worker_attendance_record",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_worker_attendance_record_worker_id_attendance_date",
                table: "worker_attendance_record",
                columns: new[] { "worker_id", "attendance_date" });

            migrationBuilder.AddForeignKey(
                name: "FK_team_project_work_team_team_id",
                table: "team_project",
                column: "team_id",
                principalTable: "work_team",
                principalColumn: "team_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_work_team_worker_team_leader_id",
                table: "work_team",
                column: "team_leader_id",
                principalTable: "worker",
                principalColumn: "worker_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_construction_project_construction_company_company_id",
                table: "construction_project");

            migrationBuilder.DropForeignKey(
                name: "FK_work_team_construction_project_project_id",
                table: "work_team");

            migrationBuilder.DropForeignKey(
                name: "FK_worker_construction_project_project_id",
                table: "worker");

            migrationBuilder.DropForeignKey(
                name: "FK_worker_work_team_team_id",
                table: "worker");

            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "device_maintenance_record");

            migrationBuilder.DropTable(
                name: "government_data");

            migrationBuilder.DropTable(
                name: "low_code_form");

            migrationBuilder.DropTable(
                name: "safety_incident");

            migrationBuilder.DropTable(
                name: "safety_training_record");

            migrationBuilder.DropTable(
                name: "system_log");

            migrationBuilder.DropTable(
                name: "team_project");

            migrationBuilder.DropTable(
                name: "worker_attendance_profile");

            migrationBuilder.DropTable(
                name: "worker_attendance_record");

            migrationBuilder.DropTable(
                name: "device");

            migrationBuilder.DropTable(
                name: "construction_company");

            migrationBuilder.DropTable(
                name: "construction_project");

            migrationBuilder.DropTable(
                name: "work_team");

            migrationBuilder.DropTable(
                name: "worker");
        }
    }
}
