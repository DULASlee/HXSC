namespace SmartConstruction.Contracts.Dtos.RefreshToken;

public class CreateRefreshTokenRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string? JwtId { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceType { get; set; }
    public DateTime ExpiresAt { get; set; }
}