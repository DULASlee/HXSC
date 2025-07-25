using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.Role;

public class RoleDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string DataScope { get; set; }
    public byte Status { get; set; }
    public bool IsSystem { get; set; }
} 