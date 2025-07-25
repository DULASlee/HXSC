using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// UI组件注册表
    /// </summary>
    public class UIComponent : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ComponentPath { get; set; }
        public string Template { get; set; }
        public string Schema { get; set; }
        public string Version { get; set; }
        public bool IsDefault { get; set; } = false;
        public byte Status { get; set; } = 1;
    }
} 