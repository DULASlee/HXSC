namespace SmartConstruction.Contracts.Dtos.EntityRegistry;

public class CreateEntityRegistryRequest
{
    public string EntityType { get; set; }
    public string TableName { get; set; }
    public string SchemaName { get; set; } = "dbo";
    public string PartitionStrategy { get; set; }
    public string PartitionColumn { get; set; }
    public string IsolationStrategy { get; set; }
    public int? IsolationThreshold { get; set; }
    public string UIComponent { get; set; }
    public string UIMetadata { get; set; }
    public bool EnableAudit { get; set; } = true;
    public bool EnableCache { get; set; } = true;
    public bool EnableSoftDelete { get; set; } = true;
    public string Metadata { get; set; }
} 