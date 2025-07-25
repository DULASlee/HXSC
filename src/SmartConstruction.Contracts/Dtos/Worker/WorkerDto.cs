using System;

namespace SmartConstruction.Contracts.Dtos.Worker
{
    public class WorkerDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public Guid? OrganizationId { get; set; }
        public Guid? TeamId { get; set; }
        public Guid? ProjectId { get; set; }
        public byte Status { get; set; }
        
        // 导航属性
        public string? TeamName { get; set; }
        public string? ProjectName { get; set; }
        public bool? IsVerified { get; set; }
        public string? FaceImage { get; set; }
        public int? Age { get; set; }
    }

    public class WorkerQueryDto
    {
        public Guid? TenantId { get; set; }
        public Guid? TeamId { get; set; }
        public Guid? OrganizationId { get; set; }
        public byte? Status { get; set; }
        public string? Keyword { get; set; }
    }
}
