namespace SmartConstruction.Contracts.Dtos.Role;

public class CreateRoleRequest
{
    public Guid TenantId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string DataScope { get; set; } = "Self";
    public byte Status { get; set; } = 1;
    public bool IsSystem { get; set; } = false;
} 