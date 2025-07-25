namespace SmartConstruction.Contracts.Dtos.Project;

public class ProjectDto : BaseDto
{
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = null!;
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ProjectStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public decimal? TotalInvestment { get; set; }
    public string? ProjectManager { get; set; }
    public string? ManagerPhone { get; set; }
    public int DeviceCount { get; set; }
    public int WorkerCount { get; set; }
    public int TeamCount { get; set; } // 新增：班组数量
}

public class CreateProjectDto
{
    public Guid CompanyId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? TotalInvestment { get; set; }
    public string? ProjectManager { get; set; }
    public string? ManagerPhone { get; set; }
}

public class ProjectListRequest : PagedRequestBase
{
    public Guid? CompanyId { get; set; }
    public ProjectStatus? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}