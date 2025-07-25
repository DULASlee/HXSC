using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.Resource;

public class ResourceDto : BaseDto
{
    public Guid? TenantId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public Guid? ParentId { get; set; }
    public string? TreePath { get; set; }
    public string? UIConfig { get; set; }
    public string? ApiPath { get; set; }
    public string? HttpMethods { get; set; }
    public byte Status { get; set; }
} 