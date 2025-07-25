namespace SmartConstruction.Contracts.Dtos.RefreshToken;

public class UpdateRefreshTokenRequest
{
    public string? JwtId { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceType { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByToken { get; set; }
} 