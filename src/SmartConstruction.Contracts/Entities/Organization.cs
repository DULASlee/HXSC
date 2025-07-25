using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 组织表
    /// </summary>
    public class Organization : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TreePath { get; set; } // HIERARCHYID
        public int? Level { get; set; } // 计算列
        public string Type { get; set; } = "Department";
        public byte Status { get; set; } = 1;
        public int SortOrder { get; set; } = 0;
    }
} 