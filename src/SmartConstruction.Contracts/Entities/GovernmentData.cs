using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 政府监管数据实体
    /// </summary>
    public class GovernmentData : BaseEntity
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 数据类型 (ATTENDANCE,WORKER,SAFETY,PROJECT)
        /// </summary>
        public string DataType { get; set; } = string.Empty;

        /// <summary>
        /// 数据内容（JSON格式）
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 上报时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 状态 (PENDING,SUCCESS,FAILED,RETRYING)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 失败原因
        /// </summary>
        public string? FailReason { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// 最后重试时间
        /// </summary>
        public DateTime? LastRetryTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public virtual Project Project { get; set; } = null!;
    }
}
