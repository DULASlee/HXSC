using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 安全事故实体
    /// </summary>
    public class SafetyIncident : BaseEntity
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 事故类型
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 事故级别
        /// </summary>
        public string Level { get; set; } = string.Empty;

        /// <summary>
        /// 事故位置
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// 事故描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 图片URL
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// 检测时间
        /// </summary>
        public DateTime DetectedTime { get; set; }

        /// <summary>
        /// 事故发生日期
        /// </summary>
        public DateTime IncidentDate { get; set; }

        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? HandledTime { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string? HandledBy { get; set; }

        /// <summary>
        /// 处理结果
        /// </summary>
        public string? HandlingResult { get; set; }

        // 导航属性
        public virtual Project Project { get; set; } = null!;
    }
}
