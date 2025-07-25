using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// UI布局配置表
    /// </summary>
    public class UILayout : BaseEntity
    {
        public Guid TenantId { get; set; }
        public string LayoutKey { get; set; }
        public string Layout { get; set; }
        public string ComponentMappings { get; set; }
        public string Theme { get; set; }
        public string CustomStyles { get; set; }
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }
} 