using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 刷新令牌表
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }
        public string DeviceId { get; set; }
        public string DeviceType { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }
        public string? ReplacedByToken { get; set; }
    }
} 