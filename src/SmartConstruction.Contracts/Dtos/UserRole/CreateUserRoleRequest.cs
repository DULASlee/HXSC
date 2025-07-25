namespace SmartConstruction.Contracts.Dtos.UserRole;

public class CreateUserRoleRequest
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? AssignedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
} 