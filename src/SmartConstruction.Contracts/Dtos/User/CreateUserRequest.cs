namespace SmartConstruction.Contracts.Dtos.User;

public class CreateUserRequest
{
    public Guid TenantId { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? DisplayName { get; set; }
    public Guid? OrganizationId { get; set; }
    public byte Status { get; set; } = 1;
} 