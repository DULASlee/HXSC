using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 资源表
    /// </summary>
    public class Resource : BaseEntity
    {
        public Guid? TenantId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Guid? ParentId { get; set; }
        public string TreePath { get; set; } // HIERARCHYID
        public string UIConfig { get; set; }
        public string ApiPath { get; set; }
        public string HttpMethods { get; set; }
        public byte Status { get; set; } = 1;
    }
} 