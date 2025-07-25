using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 班组项目关联实体（多对多关系）
    /// </summary>
    public class TeamProject : BaseEntity
    {
        /// <summary>
        /// 班组ID
        /// </summary>
        public Guid TeamId { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 分配日期
        /// </summary>
        public DateTime AssignedDate { get; set; }

        /// <summary>
        /// 进场日期（别名，保持兼容性）
        /// </summary>
        public DateTime EntryDate 
        { 
            get => AssignedDate; 
            set => AssignedDate = value; 
        }

        /// <summary>
        /// 退场日期
        /// </summary>
        public DateTime? ExitDate { get; set; }

        /// <summary>
        /// 状态 (ACTIVE,COMPLETED,SUSPENDED)
        /// </summary>
        public string Status { get; set; } = "ACTIVE";
        
        /// <summary>
        /// 班组
        /// </summary>
        public virtual Team Team { get; set; } = null!;

        /// <summary>
        /// 项目
        /// </summary>
        public virtual Project Project { get; set; } = null!;
    }
} 