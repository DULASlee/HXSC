using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.User;

public class UserDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string Username { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public byte[]? PasswordHash { get; set; }
    public string? SecurityStamp { get; set; }
    public string? DisplayName { get; set; }
    public Guid? OrganizationId { get; set; }
    public byte Status { get; set; }
    public DateTime? LastLoginAt { get; set; }
} 