namespace SmartConstruction.Contracts.Dtos.Organization;

public class UpdateOrganizationRequest
{
    public Guid? ParentId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public byte? Status { get; set; }
    public int? SortOrder { get; set; }
} 