using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 安全培训记录实体
    /// </summary>
    public class SafetyTrainingRecord : BaseEntity
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 培训主题
        /// </summary>
        public string Topic { get; set; } = null!;

        /// <summary>
        /// 培训内容
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// 培训讲师
        /// </summary>
        public string Trainer { get; set; } = null!;

        /// <summary>
        /// 培训时间
        /// </summary>
        public DateTime TrainingTime { get; set; }

        /// <summary>
        /// 培训地点
        /// </summary>
        public string Location { get; set; } = null!;

        /// <summary>
        /// 参与人数
        /// </summary>
        public int ParticipantCount { get; set; }

        /// <summary>
        /// 培训时长(小时)
        /// </summary>
        public decimal Duration { get; set; }

        /// <summary>
        /// 培训类型 (REGULAR,SPECIAL,EMERGENCY)
        /// </summary>
        public string TrainingType { get; set; } = null!;

        /// <summary>
        /// 状态 (PLANNED,COMPLETED,CANCELLED)
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Project Project { get; set; } = null!;
    }
}