using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 工人实体
    /// </summary>
    public class Worker : BaseEntity
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IdCardNumber { get; set; } = null!;

        /// <summary>
        /// 姓名
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// 姓名（别名）
        /// </summary>
        public string Name => FullName;

        /// <summary>
        /// 显示名称（别名）
        /// </summary>
        public string DisplayName => FullName;

        /// <summary>
        /// 性别 (M:男, F:女)
        /// </summary>
        public string Gender { get; set; } = null!;

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        public string Nationality { get; set; } = "中国";

        /// <summary>
        /// 民族
        /// </summary>
        public string? Ethnicity { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// 手机号（别名）
        /// </summary>
        public string Mobile => PhoneNumber;

        /// <summary>
        /// 邮箱
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// 户籍地址
        /// </summary>
        public string? HometownAddress { get; set; }

        /// <summary>
        /// 专业工种
        /// </summary>
        public string Specialty { get; set; } = null!;

        /// <summary>
        /// 职业资格证书编号
        /// </summary>
        public string? CertificateNo { get; set; }

        /// <summary>
        /// 开户银行
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string? BankAccount { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        public string? EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        public string? EmergencyPhone { get; set; }

        /// <summary>
        /// 状态 (0:禁用, 1:正常)
        /// </summary>
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 班组ID
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid? ProjectId { get; set; }

        /// <summary>
        /// 组织ID
        /// </summary>
        public Guid? OrganizationId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        // 导航属性
        /// <summary>
        /// 所属班组
        /// </summary>
        public virtual Team? Team { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Project? Project { get; set; }

        /// <summary>
        /// 考勤资料
        /// </summary>
        public virtual WorkerAttendanceProfile? AttendanceProfile { get; set; }

        /// <summary>
        /// 考勤记录
        /// </summary>
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();

        /// <summary>
        /// 担任班组长的班组
        /// </summary>
        public virtual ICollection<Team> LeadingTeams { get; set; } = new List<Team>();
    }
}