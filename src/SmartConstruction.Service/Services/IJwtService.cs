using System.Security.Claims;
using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Contracts.Entities;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// JWT服务接口
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// 生成访问令牌
        /// </summary>
        /// <param name="user">用户信息</param>
        /// <param name="tenant">租户信息</param>
        /// <param name="roles">用户角色</param>
        /// <param name="permissions">用户权限</param>
        /// <returns>访问令牌</returns>
        string GenerateAccessToken(User user, Tenant tenant, List<Role> roles, List<string> permissions);

        /// <summary>
        /// 生成刷新令牌
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="tenantId">租户ID</param>
        /// <param name="deviceId">设备ID</param>
        /// <param name="deviceType">设备类型</param>
        /// <returns>刷新令牌</returns>
        Task<string> GenerateRefreshTokenAsync(Guid userId, Guid tenantId, string? deviceId = null, string? deviceType = null);

        /// <summary>
        /// 验证访问令牌
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns>令牌中的声明</returns>
        ClaimsPrincipal? ValidateAccessToken(string token);

        /// <summary>
        /// 验证刷新令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>刷新令牌实体</returns>
        Task<RefreshToken?> ValidateRefreshTokenAsync(string refreshToken);

        /// <summary>
        /// 撤销刷新令牌
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <param name="replacedByToken">替换令牌</param>
        /// <returns>操作结果</returns>
        Task<bool> RevokeRefreshTokenAsync(string refreshToken, string? replacedByToken = null);

        /// <summary>
        /// 从令牌中获取用户ID
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns>用户ID</returns>
        string? GetUserIdFromToken(string token);

        /// <summary>
        /// 从令牌中获取租户ID
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns>租户ID</returns>
        string? GetTenantIdFromToken(string token);
    }
}