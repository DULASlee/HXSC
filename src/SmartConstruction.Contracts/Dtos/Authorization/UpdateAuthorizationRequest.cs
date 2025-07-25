namespace SmartConstruction.Contracts.Dtos.Authorization;

public class UpdateAuthorizationRequest
{
    public string? Permission { get; set; }
    public string? Conditions { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
} 