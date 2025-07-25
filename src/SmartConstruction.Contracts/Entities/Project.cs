using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 工地项目实体
    /// </summary>
    public class Project : BaseEntity
    {
        /// <summary>
        /// 项目编号 (住建部备案号/内部唯一编码)
        /// </summary>
        public string ProjectCode { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 项目描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 所属公司ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 项目详细地址
        /// </summary>
        public string ProjectAddress { get; set; }

        /// <summary>
        /// 项目经理
        /// </summary>
        public string ProjectManager { get; set; }

        /// <summary>
        /// 开工日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 计划竣工日期
        /// </summary>
        public DateTime? PlannedEndDate { get; set; }

        /// <summary>
        /// 实际竣工日期
        /// </summary>
        public DateTime? ActualEndDate { get; set; }

        /// <summary>
        /// 项目状态 (PLANNING,IN_PROGRESS,SUSPENDED,COMPLETED)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 合同金额(元)
        /// </summary>
        public decimal? ContractAmount { get; set; }

        /// <summary>
        /// 施工许可证扫描件路径
        /// </summary>
        public string ProjectLicenseImg { get; set; }

        // 导航属性
        public virtual Company Company { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
        public virtual ICollection<Worker> Workers { get; set; }
        public virtual ICollection<AttendanceRecord> AttendanceRecords { get; set; }
        public virtual ICollection<Device> Devices { get; set; }
        public virtual ICollection<SafetyIncident> SafetyIncidents { get; set; }
        public virtual ICollection<SafetyTrainingRecord> SafetyTrainingRecords { get; set; }
        public virtual ICollection<TeamProject> TeamProjects { get; set; }
    }
}
