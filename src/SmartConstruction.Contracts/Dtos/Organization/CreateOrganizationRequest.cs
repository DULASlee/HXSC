namespace SmartConstruction.Contracts.Dtos.Organization;

public class CreateOrganizationRequest
{
    public Guid TenantId { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; } = "Department";
    public byte Status { get; set; } = 1;
    public int SortOrder { get; set; } = 0;
} 