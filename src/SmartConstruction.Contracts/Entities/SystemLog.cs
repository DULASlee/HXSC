using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 系统日志实体
    /// </summary>
    public class SystemLog : BaseEntity
    {
        /// <summary>
        /// 日志级别 (INFO,WARN,ERROR,FATAL)
        /// </summary>
        public string Level { get; set; } = null!;

        /// <summary>
        /// 日志消息
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// 异常信息
        /// </summary>
        public string? Exception { get; set; }

        /// <summary>
        /// 日志来源
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
        public string? RequestPath { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string? RequestMethod { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// 用户代理
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// 执行时间(毫秒)
        /// </summary>
        public long? ExecutionTime { get; set; }

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    }
}