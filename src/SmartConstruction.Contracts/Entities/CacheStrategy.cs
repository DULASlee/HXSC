using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 缓存策略表
    /// </summary>
    public class CacheStrategy : BaseEntity
    {
        public string CacheKey { get; set; }
        public string Provider { get; set; }
        public int TTL { get; set; } = 300;
        public byte Priority { get; set; } = 5;
        public byte RedisDb { get; set; } = 0;
        public string RedisKeyPrefix { get; set; }
        public string UpdateStrategy { get; set; }
        public int? UpdateInterval { get; set; }
        public long HitCount { get; set; } = 0;
        public long MissCount { get; set; } = 0;
        public DateTime? LastAccessAt { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
} 