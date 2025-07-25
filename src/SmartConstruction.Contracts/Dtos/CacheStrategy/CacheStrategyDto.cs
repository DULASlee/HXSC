using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.CacheStrategy;

public class CacheStrategyDto : BaseDto
{
    public string CacheKey { get; set; }
    public string Provider { get; set; }
    public int TTL { get; set; }
    public byte Priority { get; set; }
    public byte RedisDb { get; set; }
    public string? RedisKeyPrefix { get; set; }
    public string? UpdateStrategy { get; set; }
    public int? UpdateInterval { get; set; }
    public long HitCount { get; set; }
    public long MissCount { get; set; }
    public DateTime? LastAccessAt { get; set; }
    public bool IsEnabled { get; set; }
} 