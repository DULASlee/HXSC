using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.Organization;

public class OrganizationDto : BaseDto
{
    public Guid TenantId { get; set; }
    public Guid? ParentId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string? TreePath { get; set; }
    public int Level { get; set; }
    public string Type { get; set; }
    public byte Status { get; set; }
    public int SortOrder { get; set; }
} 