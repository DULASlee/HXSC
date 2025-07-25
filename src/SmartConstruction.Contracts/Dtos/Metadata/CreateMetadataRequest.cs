namespace SmartConstruction.Contracts.Dtos.Metadata;

public class CreateMetadataRequest
{
    public string EntityType { get; set; }
    public string EntityId { get; set; }
    public string MetaKey { get; set; }
    public string MetaValue { get; set; }
    public string ValueType { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public DateTime EffectiveFrom { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveTo { get; set; }
    public string CacheStrategy { get; set; } = "Redis";
    public int CacheTTL { get; set; } = 300;
} 