using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.Metadata;

public class MetadataDto : BaseDto
{
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string MetaKey { get; set; }
    public string MetaValue { get; set; }
    public string ValueType { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string CacheStrategy { get; set; }
    public int CacheTTL { get; set; }
    public string CacheKey { get; set; }
} 