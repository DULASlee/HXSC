namespace SmartConstruction.Contracts.Dtos.Tenant;

public class TenantDto : BaseDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpireDate { get; set; }
}

public class CreateTenantDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public DateTime? ExpireDate { get; set; }
}

public class UpdateTenantDto : CreateTenantDto
{
    public Guid Id { get; set; }
}

public class TenantListRequest : PagedRequestBase
{
    public bool? IsActive { get; set; }
}