using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Models;
using Microsoft.EntityFrameworkCore;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// JWT服务实现
    /// </summary>
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IOptions<JwtSettings> jwtSettings,
            ApplicationDbContext context,
            ILogger<JwtService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 生成访问令牌
        /// </summary>
        public string GenerateAccessToken(User user, Tenant tenant, List<Role> roles, List<string> permissions)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new("tenant_id", tenant.Id.ToString()),
                new("tenant_code", tenant.Code),
                new("display_name", user.DisplayName ?? user.Username),
                new("user_status", user.Status.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // 添加角色信息
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Code));
                claims.Add(new Claim("role_name", role.Name));
                claims.Add(new Claim("data_scope", role.DataScope ?? "Self"));
            }

            // 添加权限信息
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("permission", permission));
            }

            // 添加组织信息
            if (user.OrganizationId.HasValue)
            {
                claims.Add(new Claim("organization_id", user.OrganizationId.Value.ToString()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        public async Task<string> GenerateRefreshTokenAsync(Guid userId, Guid tenantId, string? deviceId = null, string? deviceType = null)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                Token = GenerateSecureRandomToken(),
                JwtId = Guid.NewGuid().ToString(),
                DeviceId = deviceId ?? "unknown",
                DeviceType = deviceType ?? "Web",
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(30), // 30天有效期
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken.Token;
        }

        /// <summary>
        /// 验证访问令牌
        /// </summary>
        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed: {Token}", token);
                return null;
            }
        }

        /// <summary>
        /// 验证刷新令牌
        /// </summary>
        public async Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null);

                if (token == null || token.ExpiresAt < DateTime.UtcNow)
                {
                    return null;
                }

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refresh token validation failed: {RefreshToken}", refreshToken);
                return null;
            }
        }

        /// <summary>
        /// 撤销刷新令牌
        /// </summary>
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? replacedByToken = null)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token == null)
                {
                    return false;
                }

                token.RevokedAt = DateTime.UtcNow;
                token.ReplacedByToken = replacedByToken;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke refresh token: {RefreshToken}", refreshToken);
                return false;
            }
        }

        /// <summary>
        /// 从令牌中获取用户ID
        /// </summary>
        public string? GetUserIdFromToken(string token)
        {
            try
            {
                var principal = ValidateAccessToken(token);
                return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 从令牌中获取租户ID
        /// </summary>
        public string? GetTenantIdFromToken(string token)
        {
            try
            {
                var principal = ValidateAccessToken(token);
                return principal?.FindFirst("tenant_id")?.Value;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 生成安全随机令牌
        /// </summary>
        private static string GenerateSecureRandomToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}