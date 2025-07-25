namespace SmartConstruction.Contracts.Dtos.Role;

public class UpdateRoleRequest
{
    public string? Name { get; set; }
    public string? DataScope { get; set; }
    public byte? Status { get; set; }
    public bool? IsSystem { get; set; }
} 