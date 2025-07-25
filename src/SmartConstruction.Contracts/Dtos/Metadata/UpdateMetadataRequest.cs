namespace SmartConstruction.Contracts.Dtos.Metadata;

public class UpdateMetadataRequest
{
    public string MetaValue { get; set; }
    public int? Version { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string CacheStrategy { get; set; }
    public int? CacheTTL { get; set; }
} 