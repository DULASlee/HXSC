using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Contracts.Dtos.RefreshToken;

public class RefreshTokenDto : BaseDto
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string? JwtId { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceType { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
} 