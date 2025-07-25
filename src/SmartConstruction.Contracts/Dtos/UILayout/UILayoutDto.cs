using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.UILayout;

public class UILayoutDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string LayoutKey { get; set; }
    public string Layout { get; set; }
    public string? ComponentMappings { get; set; }
    public string? Theme { get; set; }
    public string? CustomStyles { get; set; }
    public int Version { get; set; }
    public bool IsActive { get; set; }
} 