using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 动态查询优化表
    /// </summary>
    public class QueryOptimization : BaseEntity
    {
        public string QueryPattern { get; set; }
        public string EntityType { get; set; }
        public string OptimizationType { get; set; }
        public string OptimizationHint { get; set; }
        public string PartitionColumn { get; set; }
        public string PartitionFunction { get; set; }
        public string SuggestedIndexes { get; set; }
        public long ExecutionCount { get; set; } = 0;
        public int? AvgDuration { get; set; }
        public DateTime? LastExecutedAt { get; set; }
        public bool IsEnabled { get; set; } = true;
        public bool AutoApply { get; set; } = false;
    }
} 