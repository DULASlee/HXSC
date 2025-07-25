using System;
using System.ComponentModel.DataAnnotations;

namespace SmartConstruction.Contracts.Dtos.Worker
{
    /// <summary>
    /// 更新工人DTO
    /// </summary>
    public class UpdateWorkerDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string? FullName { get; set; }

        /// <summary>
        /// 性别 (M:男, F:女)
        /// </summary>
        [MaxLength(1)]
        public string? Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// 民族
        /// </summary>
        [MaxLength(20)]
        public string? Ethnicity { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// 户籍地址
        /// </summary>
        [MaxLength(200)]
        public string? HometownAddress { get; set; }

        /// <summary>
        /// 工种/技能
        /// </summary>
        [MaxLength(50)]
        public string? Specialty { get; set; }

        /// <summary>
        /// 职业资格证书编号
        /// </summary>
        [MaxLength(50)]
        public string? CertificateNo { get; set; }

        /// <summary>
        /// 工资卡开户行
        /// </summary>
        [MaxLength(100)]
        public string? BankName { get; set; }

        /// <summary>
        /// 工资卡账号
        /// </summary>
        [MaxLength(50)]
        public string? BankAccount { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [MaxLength(50)]
        public string? EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [MaxLength(20)]
        public string? EmergencyPhone { get; set; }

        /// <summary>
        /// 工人状态
        /// </summary>
        [MaxLength(20)]
        public string? Status { get; set; }

        /// <summary>
        /// 所属班组ID
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// 所属项目ID
        /// </summary>
        public Guid? ProjectId { get; set; }
    }

    public class UpdateWorkerRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string IdCardNumber { get; set; } = null!;
        public Guid TeamId { get; set; }
        public Gender Gender { get; set; }
        public string? Phone { get; set; }
        public WorkerType Type { get; set; }
        public bool IsActive { get; set; }
    }
}