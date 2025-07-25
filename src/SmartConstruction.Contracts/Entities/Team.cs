using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 班组实体
    /// </summary>
    public class Team : BaseEntity
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        public string TeamName { get; set; } = null!;

        /// <summary>
        /// 班组名称（别名）
        /// </summary>
        public string Name => TeamName;

        /// <summary>
        /// 班组长ID
        /// </summary>
        public Guid? TeamLeaderId { get; set; }

        /// <summary>
        /// 专业工种
        /// </summary>
        public string Specialty { get; set; } = null!;

        /// <summary>
        /// 总人数
        /// </summary>
        public int TotalMembers { get; set; } = 0;

        /// <summary>
        /// 状态 (0:禁用, 1:正常)
        /// </summary>
        public byte Status { get; set; } = 1;

        // 导航属性
        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Project Project { get; set; } = null!;

        /// <summary>
        /// 班组长
        /// </summary>
        public virtual Worker? TeamLeader { get; set; }

        /// <summary>
        /// 班组成员
        /// </summary>
        public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();

        /// <summary>
        /// 班组项目关联
        /// </summary>
        public virtual ICollection<TeamProject> TeamProjects { get; set; } = new List<TeamProject>();
    }
}
