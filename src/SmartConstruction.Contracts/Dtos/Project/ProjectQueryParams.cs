namespace SmartConstruction.Contracts.Dtos.Project;

public class ProjectQueryParams
{
    public string? ProjectCode { get; set; }
    public string? ProjectName { get; set; }
    public string? Status { get; set; }
    public Guid? CompanyId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
