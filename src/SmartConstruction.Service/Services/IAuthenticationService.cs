using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 认证服务接口
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>登录响应</returns>
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="request">刷新令牌请求</param>
        /// <returns>新的令牌信息</returns>
        Task<ApiResponse<TokenInfo>> RefreshTokenAsync(RefreshTokenRequest request);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="deviceId">设备ID</param>
        /// <returns>操作结果</returns>
        Task<ApiResponse<object>> LogoutAsync(string userId, string? deviceId = null);

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户信息</returns>
        Task<ApiResponse<CurrentUserResponse>> GetCurrentUserAsync(string userId);

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户菜单</returns>
        Task<ApiResponse<UserMenusResponse>> GetUserMenusAsync(string userId);

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns>验证结果</returns>
        Task<bool> ValidateTokenAsync(string token);
    }
}