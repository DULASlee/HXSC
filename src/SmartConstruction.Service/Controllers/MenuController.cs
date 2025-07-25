using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Services;
using System.Security.Claims;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 菜单管理控制器
    /// </summary>
    [ApiController]
    [Route("api/menu")]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(
            IMenuService menuService,
            ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns>菜单树</returns>
        [HttpGet("tree")]
        public async Task<ActionResult<ApiResponse<List<MenuTreeDto>>>> GetMenuTree([FromQuery] Guid? tenantId = null)
        {
            try
            {
                var result = await _menuService.GetMenuTreeAsync(tenantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取菜单树失败");
                return StatusCode(500, ApiResponse<List<MenuTreeDto>>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <returns>用户菜单</returns>
        [HttpGet("user-menus")]
        public async Task<ActionResult<ApiResponse<List<MenuInfo>>>> GetUserMenus()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<List<MenuInfo>>.Failure("用户信息无效"));
                }

                var result = await _menuService.GetUserMenusAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户菜单失败");
                return StatusCode(500, ApiResponse<List<MenuInfo>>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>角色菜单ID列表</returns>
        [HttpGet("role-menus/{roleId}")]
        public async Task<ActionResult<ApiResponse<List<Guid>>>> GetRoleMenus(Guid roleId)
        {
            try
            {
                var result = await _menuService.GetRoleMenusAsync(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取角色菜单失败");
                return StatusCode(500, ApiResponse<List<Guid>>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 为角色分配菜单
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="request">分配请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("assign-to-role/{roleId}")]
        public async Task<ActionResult<ApiResponse<object>>> AssignMenusToRole(Guid roleId, [FromBody] AssignMenusRequest request)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _menuService.AssignMenusToRoleAsync(roleId, request.MenuIds, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为角色分配菜单失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 为用户分配菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="request">分配请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("assign-to-user/{userId}")]
        public async Task<ActionResult<ApiResponse<object>>> AssignMenusToUser(Guid userId, [FromBody] AssignMenusRequest request)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _menuService.AssignMenusToUserAsync(userId, request.MenuIds, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "为用户分配菜单失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="dto">菜单DTO</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateMenu([FromBody] CreateMenuDto dto)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<Guid>.Failure("用户信息无效"));
                }

                var result = await _menuService.CreateMenuAsync(dto, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建菜单失败");
                return StatusCode(500, ApiResponse<Guid>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="dto">菜单DTO</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateMenu(Guid id, [FromBody] UpdateMenuDto dto)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _menuService.UpdateMenuAsync(id, dto, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新菜单失败");
                return StatusCode(500, ApiResponse<object>.Failure("服务器内部错误"));
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMenu(Guid id)
        {
            try
            {
                var operatorId = GetCurrentUserId();
                if (operatorId == Guid.Empty)
                {
                    return BadRequest(ApiResponse<object>.Failure("用户信息无效"));
                }

                var result = await _menuService.DeleteMenuAsync(id, operatorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除菜单失败");
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
    /// 分配菜单请求
    /// </summary>
    public class AssignMenusRequest
    {
        public List<Guid> MenuIds { get; set; } = new();
    }
}