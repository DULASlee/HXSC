using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 统一元数据表
    /// </summary>
    public class Metadata : BaseEntity
    {
        public string EntityType { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string MetaKey { get; set; } = string.Empty;
        public string MetaValue { get; set; } = string.Empty;
        public string ValueType { get; set; } = "String";
        public string? FieldName { get; set; }
        public int Version { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
        public DateTime? EffectiveTo { get; set; }
        public string CacheStrategy { get; set; } = "Redis";
        public int CacheTTL { get; set; } = 300;
        public string CacheKey => $"{EntityType}:{EntityId}:{MetaKey}";
    }
} 