namespace SmartConstruction.Contracts.Dtos.Safety;

public class UpdateSafetyIncidentRequest
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public DateTime IncidentDate { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? WorkerId { get; set; }
    public bool IsHandled { get; set; }
    public string? HandledBy { get; set; }
    public DateTime? HandledAt { get; set; }
    public string? HandlingResult { get; set; }
}
