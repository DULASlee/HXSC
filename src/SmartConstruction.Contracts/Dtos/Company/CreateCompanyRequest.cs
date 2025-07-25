namespace SmartConstruction.Contracts.Dtos.Company;

public class CreateCompanyRequest
{
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyType { get; set; } = string.Empty;
    public string UnifiedSocialCreditCode { get; set; } = string.Empty;
    public string LegalRepresentative { get; set; } = string.Empty;
    public string ContactPhone { get; set; } = string.Empty;
    public string RegisteredAddress { get; set; } = string.Empty;
    public string? BusinessScope { get; set; }
    public string? BusinessLicenseImg { get; set; }
    public string Status { get; set; } = string.Empty;
}
