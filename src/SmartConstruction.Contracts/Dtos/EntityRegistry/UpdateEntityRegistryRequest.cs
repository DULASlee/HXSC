namespace SmartConstruction.Contracts.Dtos.EntityRegistry;

public class UpdateEntityRegistryRequest
{
    public string TableName { get; set; }
    public string SchemaName { get; set; }
    public string PartitionStrategy { get; set; }
    public string PartitionColumn { get; set; }
    public string IsolationStrategy { get; set; }
    public int? IsolationThreshold { get; set; }
    public string UIComponent { get; set; }
    public string UIMetadata { get; set; }
    public bool? EnableAudit { get; set; }
    public bool? EnableCache { get; set; }
    public bool? EnableSoftDelete { get; set; }
    public string Metadata { get; set; }
} 