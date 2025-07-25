namespace SmartConstruction.Contracts.Dtos.User;

public class UpdateUserRequest
{
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? DisplayName { get; set; }
    public Guid? OrganizationId { get; set; }
    public byte? Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
} 