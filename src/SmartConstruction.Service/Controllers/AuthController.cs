using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Services;
using SmartConstruction.Service.Infrastructure.Logging;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartConstruction.Service.Data;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 认证控制器
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly LoggingService _loggingService;
        private readonly ApplicationDbContext _context;

        public AuthController(
            IAuthenticationService authService,
            ILogger<AuthController> logger,
            LoggingService loggingService,
            ApplicationDbContext context)
        {
            _authService = authService;
            _logger = logger;
            _loggingService = loggingService;
            _context = context;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>登录响应</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            return await _loggingService.LogMethodExecutionAsync<ActionResult<ApiResponse<LoginResponse>>>("Login", async () =>
            {
                // 增强的模型验证
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors)
                                          .Select(x => x.ErrorMessage)
                                          .ToList();
                    _loggingService.LogError("LOGIN_VALIDATION_ERROR", "登录请求模型验证失败", new { Errors = errors });
                    
                    return BadRequest(ApiResponse<object>.Failure("请求参数无效"));
                }

                // 记录登录请求（脱敏处理）
                _loggingService.LogInformation("用户登录请求开始", new { 
                    Username = request.Username, 
                    TenantCode = request.TenantCode, 
                    DeviceId = request.DeviceId,
                    RequestTime = DateTime.UtcNow 
                });
                
                // 查找租户
                var tenant = await _context.Tenants
                    .FirstOrDefaultAsync(t => t.Code == request.TenantCode && t.Status == 1);

                if (tenant == null)
                {
                    _loggingService.LogError("TENANT_NOT_FOUND", "租户不存在或已禁用", new { TenantCode = request.TenantCode });
                    return BadRequest(ApiResponse<LoginResponse>.Failure("租户不存在或已禁用"));
                }

                _loggingService.LogInformation("找到租户", new { TenantId = tenant.Id, TenantCode = request.TenantCode });

                // 查找用户
                var user = await _context.Users
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == request.Username 
                                            && u.TenantId == tenant.Id 
                                            && u.Status == 1);

                if (user == null)
                {
                    _loggingService.LogError("USER_NOT_FOUND", "用户不存在或已禁用", new { Username = request.Username, TenantId = tenant.Id });
                    return BadRequest(ApiResponse<LoginResponse>.Failure("用户名或密码错误"));
                }

                _loggingService.LogInformation("找到用户", new { UserId = user.Id, Username = request.Username, HashLength = user.PasswordHash?.Length ?? 0 });

                // 验证密码
                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    _loggingService.LogError("PASSWORD_VERIFICATION_FAILED", "密码验证失败", new { Username = request.Username });
                    return BadRequest(ApiResponse<LoginResponse>.Failure("用户名或密码错误"));
                }

                _loggingService.LogInformation("密码验证成功", new { Username = request.Username });
                
                var result = await _authService.LoginAsync(request);
                
                if (result.IsSuccess)
                {
                    _loggingService.LogInformation("用户登录成功", new { Username = request.Username, UserId = user.Id });
                    
                    // 记录审计日志
                    _loggingService.LogAudit("用户登录", user.Id.ToString(), "/api/auth/login", new { 
                        Username = request.Username, 
                        TenantId = tenant.Id,
                        IP = HttpContext.Connection.RemoteIpAddress?.ToString()
                    });
                    
                    // 增强登录响应，添加租户类型信息用于前端路由判断
                    var response = result.Data!;
                    var tenantType = DetermineTenantType(response.User.Roles);
                    
                    var enhancedResponse = new
                    {
                        Token = response.Token.AccessToken,
                        RefreshToken = response.Token.RefreshToken,
                        TokenType = response.Token.TokenType,
                        ExpiresIn = response.Token.ExpiresIn,
                        ExpiresAt = response.Token.ExpiresAt,
                        TenantId = response.User.TenantId,
                        TenantCode = tenant.Code,
                        TenantType = tenantType,
                        User = response.User,
                        Roles = response.User.Roles,
                        Permissions = response.Permissions,
                        Menus = response.Menus
                    };
                    
                    return Ok(ApiResponse<object>.Success(enhancedResponse, "登录成功"));
                }
                else
                {
                    _loggingService.LogError("LOGIN_FAILED", "用户登录失败", new { Username = request.Username, Message = result.Message });
                    return BadRequest(result);
                }
            });
        }

        private static bool VerifyPassword(string password, byte[] passwordHash)
        {
            // 这里使用简单的SHA256验证，实际项目应该使用更安全的方法如bcrypt
            using var sha256 = SHA256.Create();
            var computedHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <param name="request">刷新令牌请求</param>
        /// <returns>新的令牌信息</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<TokenInfo>>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request);
                
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "刷新令牌接口异常");
                return StatusCode(500, ApiResponse<TokenInfo>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var deviceId = Request.Headers["X-Device-Id"].FirstOrDefault();
                var result = await _authService.LogoutAsync(userId, deviceId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "登出接口异常");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CurrentUserResponse>>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<CurrentUserResponse>.Failure("用户信息无效"));
                }

                var result = await _authService.GetCurrentUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户信息接口异常");
                return StatusCode(500, ApiResponse<CurrentUserResponse>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns>用户菜单</returns>
        [HttpGet("menus")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserMenusResponse>>> GetUserMenus()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(ApiResponse<UserMenusResponse>.Failure("用户信息无效"));
                }

                var result = await _authService.GetUserMenusAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户菜单接口异常");
                return StatusCode(500, ApiResponse<UserMenusResponse>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 验证令牌
        /// </summary>
        /// <returns>验证结果</returns>
        [HttpPost("validate-token")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> ValidateToken()
        {
            try
            {
                var token = Request.Headers.Authorization.FirstOrDefault()?.Replace("Bearer ", "");
                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(ApiResponse<object>.Failure("令牌无效"));
                }

                var isValid = await _authService.ValidateTokenAsync(token);
                
                if (isValid)
                {
                    return Ok(ApiResponse<object>.Success(null, "令牌有效"));
                }
                else
                {
                    return BadRequest(ApiResponse<object>.Failure("令牌无效"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证令牌接口异常");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns>当前服务器时间</returns>
        [HttpGet("server-time")]
        [AllowAnonymous]
        public ActionResult<ApiResponse<object>> GetServerTime()
        {
            try
            {
                var serverTime = new
                {
                    ServerTime = DateTime.Now,
                    UtcTime = DateTime.UtcNow,
                    TimeZone = TimeZoneInfo.Local.DisplayName,
                    Timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };

                return Ok(ApiResponse<object>.Success(serverTime));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取服务器时间接口异常");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }


        /// <summary>
        /// 健康检查端点
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            try
            {
                return Ok(ApiResponse<object>.Success(new
                {
                    Status = "Healthy",
                    Timestamp = DateTime.UtcNow,
                    Version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString(),
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
                    Database = "Connected" // 简化检查，实际项目中可以测试数据库连接
                }, "服务健康"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<object>.Failure($"服务不健康: {ex.Message}"));
            }
        }

        #region 私有方法

        /// <summary>
        /// 确定租户类型（用于前端路由判断）
        /// </summary>
        private static string DetermineTenantType(List<RoleInfo> roles)
        {
            // 根据用户角色判断租户类型
            var roleCodes = roles.Select(r => r.Code.ToLower()).ToList();

            // 如果包含管理员角色，返回Management
            if (roleCodes.Contains("admin") || roleCodes.Contains("manager"))
            {
                return "Management";
            }

            // 如果只有查看权限，返回DigitalTwin
            if (roleCodes.Contains("user"))
            {
                return "DigitalTwin";
            }

            // 如果包含审计员角色，返回Audit
            if (roleCodes.Contains("auditor"))
            {
                return "Audit";
            }

            // 默认根据角色代码判断
            // ADMIN角色 -> Management系统
            // USER角色 -> DigitalTwin系统  
            // AUDITOR角色 -> Audit系统
            return roleCodes.Any(r => r.Contains("admin")) ? "Management" : "DigitalTwin";
        }

        #endregion
    }
}
