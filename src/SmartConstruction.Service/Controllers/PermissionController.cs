using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Services;
using System.Security.Claims;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 权限管理控制器
    /// </summary>
    [ApiController]
    [Route("api/permission")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(
            IPermissionService permissionService,
            ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns>权限树</returns>
        [HttpGet("tree")]
        public async Task<ActionResult<ApiResponse<List<PermissionTreeDto>>>> GetPermissionTree([FromQuery] Guid? tenantId = null)
        {
            try
            {
                var result = await _permissionService.GetPermissionTreeAsync(tenantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取权限树失败");
                return StatusCode(500, ApiResponse<List<PermissionTreeDto>>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="request">权限检查请求</param>
        /// <returns>检查结果</returns>
        [HttpPost("check")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckPermission([FromBody] CheckPermissionRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<bool>.Failure("用户信息无效"));
                }

                var result = await _permissionService.CheckPermissionAsync(userId, request.PermissionCode, request.Context);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查权限失败");
                return StatusCode(500, ApiResponse<bool>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 检查任意权限
        /// </summary>
        /// <param name="request">权限检查请求</param>
        /// <returns>检查结果</returns>
        [HttpPost("check-any")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckAnyPermission([FromBody] CheckAnyPermissionRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<bool>.Failure("用户信息无效"));
                }

                var result = await _permissionService.CheckAnyPermissionAsync(userId, request.PermissionCodes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查任意权限失败");
                return StatusCode(500, ApiResponse<bool>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 检查所有权限
        /// </summary>
        /// <param name="request">权限检查请求</param>
        /// <returns>检查结果</returns>
        [HttpPost("check-all")]
        public async Task<ActionResult<ApiResponse<bool>>> CheckAllPermissions([FromBody] CheckAllPermissionsRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<bool>.Failure("用户信息无效"));
                }

                var result = await _permissionService.CheckAllPermissionsAsync(userId, request.PermissionCodes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查所有权限失败");
                return StatusCode(500, ApiResponse<bool>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <returns>用户权限列表</returns>
        [HttpGet("user-permissions")]
        public async Task<ActionResult<ApiResponse<List<string>>>> GetUserPermissions()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<List<string>>.Failure("用户信息无效"));
                }

                var result = await _permissionService.GetUserPermissionsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户权限列表失败");
                return StatusCode(500, ApiResponse<List<string>>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="request">分配请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("assign-to-role/{roleId}")]
        public async Task<ActionResult<ApiResponse<object>>> AssignPermissionsToRole(Guid roleId, [FromBody] AssignPermissionsRequest request)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _permissionService.AssignPermissionsToRoleAsync(roleId, request.PermissionIds, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为角色分配权限失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 为用户分配权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">分配请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("assign-to-user/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> AssignPermissionsToUser(Guid userId, [FromBody] AssignPermissionsRequest request)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _permissionService.AssignPermissionsToUserAsync(userId, request.PermissionIds, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为用户分配权限失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="dto">权限DTO</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreatePermission([FromBody] CreatePermissionDto dto)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<Guid>.Failure("用户信息无效"));
                }

                var result = await _permissionService.CreatePermissionAsync(dto, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建权限失败");
                return StatusCode(500, ApiResponse<Guid>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <param name="dto">权限DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdatePermission(Guid id, [FromBody] UpdatePermissionDto dto)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _permissionService.UpdatePermissionAsync(id, dto, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新权限失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePermission(Guid id)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _permissionService.DeletePermissionAsync(id, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除权限失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        /// <returns>用户ID</returns>
        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

    /// <summary>
    /// 权限检查请求
    /// </summary>
    public class CheckPermissionRequest
    {
        public string PermissionCode { get; set; } = string.Empty;
        public Dictionary<string, object>? Context { get; set; }
    }

    /// <summary>
    /// 任意权限检查请求
    /// </summary>
    public class CheckAnyPermissionRequest
    {
        public List<string> PermissionCodes { get; set; } = new();
    }

    /// <summary>
    /// 所有权限检查请求
    /// </summary>
    public class CheckAllPermissionsRequest
    {
        public List<string> PermissionCodes { get; set; } = new();
    }

    /// <summary>
    /// 分配权限请求
    /// </summary>
    public class AssignPermissionsRequest
    {
        public List<Guid> PermissionIds { get; set; } = new();
    }
}