namespace SmartConstruction.Contracts.Dtos.Project;

public class UpdateProjectRequest
{
    public Guid Id { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectType { get; set; } = string.Empty;
    public string ProjectAddress { get; set; } = string.Empty;
    public string ProjectManager { get; set; } = string.Empty;
    public string? ProjectDescription { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CompanyId { get; set; }
    public string? ProjectLicenseImg { get; set; }
}
