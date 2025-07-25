using System.ComponentModel.DataAnnotations;

namespace SmartConstruction.Contracts.Dtos.Team;

public class UpdateTeamRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public string? Leader { get; set; }
    public string? LeaderPhone { get; set; }
    public TeamType Type { get; set; }
    public bool IsActive { get; set; }
}