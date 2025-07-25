using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 低代码表单实体
    /// </summary>
    public class LowCodeForm : BaseEntity
    {
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; } = null!;

        /// <summary>
        /// 表单编码
        /// </summary>
        public string FormCode { get; set; } = null!;

        /// <summary>
        /// 表单描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 表单配置(JSON格式)
        /// </summary>
        public string FormConfig { get; set; } = null!;

        /// <summary>
        /// 表单类型
        /// </summary>
        public string FormType { get; set; } = null!;

        /// <summary>
        /// 状态 (DRAFT,PUBLISHED,DISABLED)
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; } = 1;

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreatedBy { get; set; } = null!;

        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 是否系统表单
        /// </summary>
        public bool IsSystemForm { get; set; } = false;

        /// <summary>
        /// 排序号
        /// </summary>
        public int SortOrder { get; set; } = 0;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}