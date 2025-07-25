using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.QueryOptimization;

public class QueryOptimizationDto : BaseDto
{
    public string QueryPattern { get; set; }
    public string? EntityType { get; set; }
    public string? OptimizationType { get; set; }
    public string? OptimizationHint { get; set; }
    public string? PartitionColumn { get; set; }
    public string? PartitionFunction { get; set; }
    public string? SuggestedIndexes { get; set; }
    public long ExecutionCount { get; set; }
    public int AvgDuration { get; set; }
    public DateTime? LastExecutedAt { get; set; }
    public bool IsEnabled { get; set; }
    public bool AutoApply { get; set; }
} 