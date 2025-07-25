namespace SmartConstruction.Contracts.Dtos.UILayout;

public class UpdateUILayoutRequest
{
    public string? Layout { get; set; }
    public string? ComponentMappings { get; set; }
    public string? Theme { get; set; }
    public string? CustomStyles { get; set; }
    public int? Version { get; set; }
    public bool? IsActive { get; set; }
} 