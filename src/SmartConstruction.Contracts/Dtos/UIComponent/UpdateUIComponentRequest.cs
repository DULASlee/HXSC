namespace SmartConstruction.Contracts.Dtos.UIComponent;

public class UpdateUIComponentRequest
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ComponentPath { get; set; }
    public string? Template { get; set; }
    public string? Schema { get; set; }
    public string? Version { get; set; }
    public bool? IsDefault { get; set; }
    public byte? Status { get; set; }
}