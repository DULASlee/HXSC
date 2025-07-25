namespace SmartConstruction.Contracts.Dtos.Company;

public class CompanyDto : BaseDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? UnifiedSocialCreditCode { get; set; }
    public string? LegalPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public CompanyType Type { get; set; }
    public string TypeName => Type.ToString();
    public bool IsActive { get; set; }
    public int ProjectCount { get; set; } // 新增：项目数量
}

public class CreateCompanyDto
{
    public Guid TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? UnifiedSocialCreditCode { get; set; }
    public string? LegalPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Address { get; set; }
    public CompanyType Type { get; set; }
}

public class CompanyListRequest : PagedRequestBase
{
    public Guid? TenantId { get; set; }
    public CompanyType? Type { get; set; }
    public bool? IsActive { get; set; }
}