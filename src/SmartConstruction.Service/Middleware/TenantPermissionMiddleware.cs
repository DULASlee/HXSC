using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SmartConstruction.Service.Data;

namespace SmartConstruction.Service.Middleware
{
    /// <summary>
    /// 多租户权限验证中间件
    /// </summary>
    public class TenantPermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantPermissionMiddleware> _logger;

        public TenantPermissionMiddleware(RequestDelegate next, ILogger<TenantPermissionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // 跳过无需验证的路径
            if (ShouldSkipValidation(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // 检查用户是否已认证
            if (context.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    // 从JWT中提取租户信息
                    var tenantId = context.User.FindFirst("tenant_id")?.Value;
                    var tenantCode = context.User.FindFirst("tenant_code")?.Value;
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var permissions = context.User.FindAll("permission").Select(c => c.Value).ToList();

                    if (!string.IsNullOrEmpty(tenantId) && !string.IsNullOrEmpty(userId))
                    {
                        // 验证租户是否仍然有效
                        var tenant = await dbContext.Tenants
                            .FirstOrDefaultAsync(t => t.Id.ToString() == tenantId && t.Status == 1);

                        if (tenant == null)
                        {
                            _logger.LogWarning("租户已禁用或不存在: TenantId={TenantId}", tenantId);
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("租户已禁用或不存在");
                            return;
                        }

                        // 验证用户是否仍然有效
                        var user = await dbContext.Users
                            .FirstOrDefaultAsync(u => u.Id.ToString() == userId && 
                                               u.TenantId.ToString() == tenantId && 
                                               u.Status == 1);

                        if (user == null)
                        {
                            _logger.LogWarning("用户已禁用或不存在: UserId={UserId}, TenantId={TenantId}", userId, tenantId);
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("用户已禁用或不存在");
                            return;
                        }

                        // 设置租户上下文信息
                        var tenantContext = new TenantContext
                        {
                            TenantId = tenantId,
                            TenantCode = tenantCode ?? "",
                            UserId = userId,
                            Permissions = permissions
                        };

                        context.Items["TenantContext"] = tenantContext;

                        // 记录访问日志（可选）
                        _logger.LogDebug("用户 {UserId} 从租户 {TenantCode} 访问 {Path}", 
                            userId, tenantCode, context.Request.Path);

                        // 检查特定API路径的权限
                        if (!HasPermissionForPath(context.Request.Path, context.Request.Method, permissions))
                        {
                            _logger.LogWarning("用户 {UserId} 访问 {Path} 权限不足", userId, context.Request.Path);
                            context.Response.StatusCode = 403;
                            await context.Response.WriteAsync("权限不足");
                            return;
                        }
                    }
                    else
                    {
                        _logger.LogWarning("JWT令牌中缺少必要的租户信息");
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("令牌信息不完整");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "租户权限验证过程中发生异常");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("服务器内部错误");
                    return;
                }
            }

            await _next(context);
        }

        /// <summary>
        /// 判断是否应该跳过权限验证
        /// </summary>
        private static bool ShouldSkipValidation(PathString path)
        {
            var skipPaths = new[]
            {
                "/api/auth/login",
                "/api/auth/refresh-token", 
                "/api/auth/server-time",
                "/swagger",
                "/health",
                "/metrics"
            };

            return skipPaths.Any(skipPath => path.StartsWithSegments(skipPath, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// 检查用户是否有访问特定路径的权限
        /// </summary>
        private bool HasPermissionForPath(PathString path, string method, List<string> userPermissions)
        {
            try
            {
                // 如果用户有超级管理员权限，允许所有操作
                if (userPermissions.Contains("*"))
                {
                    return true;
                }

                // 根据路径和方法检查权限
                var requiredPermission = GetRequiredPermission(path, method);
                
                if (string.IsNullOrEmpty(requiredPermission))
                {
                    // 如果没有特定权限要求，默认允许
                    return true;
                }

                return userPermissions.Contains(requiredPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查路径权限时发生异常: {Path}", path);
                // 发生异常时，默认拒绝访问
                return false;
            }
        }

        /// <summary>
        /// 根据API路径和HTTP方法获取所需权限
        /// </summary>
        private static string? GetRequiredPermission(PathString path, string method)
        {
            var pathLower = path.Value?.ToLower() ?? "";

            // 数字孪生API权限映射
            if (pathLower.StartsWith("/api/digital-twin"))
            {
                return "digital-twin.view";
            }

            // 设备管理API权限映射
            if (pathLower.StartsWith("/api/device"))
            {
                return method.ToUpper() switch
                {
                    "GET" => "device.view",
                    "POST" => "device.create",
                    "PUT" => "device.update",
                    "DELETE" => "device.delete",
                    _ => "device.view"
                };
            }

            // 项目管理API权限映射
            if (pathLower.StartsWith("/api/project"))
            {
                return method.ToUpper() switch
                {
                    "GET" => "project.view",
                    "POST" => "project.create",
                    "PUT" => "project.update",
                    "DELETE" => "project.delete",
                    _ => "project.view"
                };
            }

            // 用户管理API权限映射
            if (pathLower.StartsWith("/api/user"))
            {
                return method.ToUpper() switch
                {
                    "GET" => "user.view",
                    "POST" => "user.create",
                    "PUT" => "user.update",
                    "DELETE" => "user.delete",
                    _ => "user.view"
                };
            }

            // 系统管理API权限映射
            if (pathLower.StartsWith("/api/system"))
            {
                return "system";
            }

            // 开发工具API权限映射（仅超级管理员）
            if (pathLower.StartsWith("/api/dev"))
            {
                return "*";
            }

            // 默认不需要特殊权限
            return null;
        }
    }

    /// <summary>
    /// 租户上下文信息
    /// </summary>
    public class TenantContext
    {
        public string TenantId { get; set; } = string.Empty;
        public string TenantCode { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new();
    }

    /// <summary>
    /// 扩展方法用于注册中间件
    /// </summary>
    public static class TenantPermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantPermission(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantPermissionMiddleware>();
        }
    }
}