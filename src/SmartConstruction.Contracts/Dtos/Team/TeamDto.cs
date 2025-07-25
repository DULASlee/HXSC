using System;
using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.Team
{
    public class TeamDto : BaseDto
    {
        public Guid TenantId { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? TeamLeaderId { get; set; }
        public string Specialty { get; set; } = string.Empty;
        public int TotalMembers { get; set; }
        public byte Status { get; set; }
        
        // 导航属性
        public string? ProjectName { get; set; }
        public string? TeamLeaderName { get; set; }
    }

    public class CreateTeamDto
    {
        public Guid TenantId { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public Guid? TeamLeaderId { get; set; }
        public string Specialty { get; set; }
        public int TotalMembers { get; set; }
        public byte Status { get; set; } = 1;
    }

    public class UpdateTeamDto
    {
        public string Name { get; set; }
        public Guid? TeamLeaderId { get; set; }
        public string Specialty { get; set; }
        public int TotalMembers { get; set; }
        public byte Status { get; set; } = 1;
    }

    public class TeamDetailDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ProjectId { get; set; }
        public string Name { get; set; }
        public Guid? TeamLeaderId { get; set; }
        public string Specialty { get; set; }
        public int TotalMembers { get; set; }
        public byte Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TeamQueryDto
    {
        public Guid? TenantId { get; set; }
        public Guid? ProjectId { get; set; }
        public byte? Status { get; set; }
        public string Keyword { get; set; }
    }
}
