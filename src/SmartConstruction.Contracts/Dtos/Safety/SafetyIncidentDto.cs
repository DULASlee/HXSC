namespace SmartConstruction.Contracts.Dtos.Safety;

public class SafetyIncidentDto : BaseDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public IncidentLevel Level { get; set; }
    public string LevelName => Level.ToString();
    public DateTime OccurredTime { get; set; }
    public string? Location { get; set; }
    public string? Reporter { get; set; }
    public string? Handler { get; set; }
    public IncidentStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? Result { get; set; }
    public List<string>? Attachments { get; set; }
}

public class CreateSafetyIncidentDto
{
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public IncidentLevel Level { get; set; }
    public DateTime OccurredTime { get; set; }
    public string? Location { get; set; }
    public string? Reporter { get; set; }
    public List<string>? Attachments { get; set; }
}

public class SafetyIncidentListRequest : PagedRequestBase
{
    public Guid? ProjectId { get; set; }
    public IncidentLevel? Level { get; set; }
    public IncidentStatus? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}