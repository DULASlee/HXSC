namespace SmartConstruction.Contracts.Dtos.UserRole;

public class UpdateUserRoleRequest
{
    public Guid? AssignedBy { get; set; }
    public DateTime? ExpiresAt { get; set; }
} 