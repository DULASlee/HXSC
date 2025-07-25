namespace SmartConstruction.Contracts.Dtos.Company;

public class CompanyQueryParams
{
    public string? CompanyName { get; set; }
    public string? UnifiedSocialCreditCode { get; set; }
    public string? ContactPhone { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
