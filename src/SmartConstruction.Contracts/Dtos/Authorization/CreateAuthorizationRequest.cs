namespace SmartConstruction.Contracts.Dtos.Authorization;

public class CreateAuthorizationRequest
{
    public Guid TenantId { get; set; }
    public string PrincipalType { get; set; }
    public Guid PrincipalId { get; set; }
    public Guid ResourceId { get; set; }
    public string Permission { get; set; }
    public string? Conditions { get; set; }
    public DateTime? EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public Guid? CreatedBy { get; set; }
} 