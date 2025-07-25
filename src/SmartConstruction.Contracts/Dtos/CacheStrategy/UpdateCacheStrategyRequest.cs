namespace SmartConstruction.Contracts.Dtos.CacheStrategy;

public class UpdateCacheStrategyRequest
{
    public string? Provider { get; set; }
    public int? TTL { get; set; }
    public byte? Priority { get; set; }
    public byte? RedisDb { get; set; }
    public string? RedisKeyPrefix { get; set; }
    public string? UpdateStrategy { get; set; }
    public int? UpdateInterval { get; set; }
    public bool? IsEnabled { get; set; }
} 