namespace SmartConstruction.Contracts.Dtos.QueryOptimization;

public class CreateQueryOptimizationRequest
{
    public string QueryPattern { get; set; }
    public string? EntityType { get; set; }
    public string? OptimizationType { get; set; }
    public string? OptimizationHint { get; set; }
    public string? PartitionColumn { get; set; }
    public string? PartitionFunction { get; set; }
    public string? SuggestedIndexes { get; set; }
    public long ExecutionCount { get; set; } = 0;
    public int AvgDuration { get; set; } = 0;
    public bool IsEnabled { get; set; } = true;
    public bool AutoApply { get; set; } = false;
} 