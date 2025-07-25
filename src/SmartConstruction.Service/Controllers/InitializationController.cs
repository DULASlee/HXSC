using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Service.Models;
using SmartConstruction.Service.Scripts;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 系统初始化控制器
    /// </summary>
    [ApiController]
    [Route("api/initialization")]
    [Authorize]
    public class InitializationController : ControllerBase
    {
        private readonly InitializeMenus _initializeMenus;
        private readonly ILogger<InitializationController> _logger;

        public InitializationController(
            InitializeMenus initializeMenus,
            ILogger<InitializationController> logger)
        {
            _initializeMenus = initializeMenus;
            _logger = logger;
        }

        /// <summary>
        /// 初始化系统菜单
        /// </summary>
        /// <returns>初始化结果</returns>
        [HttpPost("menus")]
        public async Task<ActionResult<ApiResponse<object>>> InitializeMenus()
        {
            try
            {
                _logger.LogInformation("开始初始化系统菜单...");
                
                await _initializeMenus.InitializeAllMenusAsync();
                
                _logger.LogInformation("系统菜单初始化完成");
                
                return Ok(ApiResponse<object>.Success(null, "系统菜单初始化成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "初始化系统菜单失败");
                return StatusCode(500, ApiResponse<object>.Failure($"初始化失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 重置系统菜单（危险操作）
        /// </summary>
        /// <param name="confirmCode">确认码（必须为"RESET_MENUS"）</param>
        /// <returns>重置结果</returns>
        [HttpPost("reset-menus")]
        public async Task<ActionResult<ApiResponse<object>>> ResetMenus([FromQuery] string confirmCode)
        {
            try
            {
                if (confirmCode != "RESET_MENUS")
                {
                    return BadRequest(ApiResponse<object>.Failure("确认码错误，操作被拒绝"));
                }

                _logger.LogWarning("开始重置系统菜单...");
                
                await _initializeMenus.InitializeAllMenusAsync();
                
                _logger.LogWarning("系统菜单重置完成");
                
                return Ok(ApiResponse<object>.Success(null, "系统菜单重置成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "重置系统菜单失败");
                return StatusCode(500, ApiResponse<object>.Failure($"重置失败: {ex.Message}"));
            }
        }

        /// <summary>
        /// 获取菜单初始化状态
        /// </summary>
        /// <returns>初始化状态</returns>
        [HttpGet("menu-status")]
        public async Task<ActionResult<ApiResponse<object>>> GetMenuStatus()
        {
            try
            {
                // 这里可以检查菜单是否已经初始化
                // 简单实现：检查是否存在菜单
                
                var result = new
                {
                    IsInitialized = true, // 这里应该实际检查
                    MenuCount = 0, // 这里应该实际统计
                    LastInitTime = DateTime.UtcNow // 这里应该从数据库获取
                };
                
                return Ok(ApiResponse<object>.Success(result, "获取菜单状态成功"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取菜单状态失败");
                return StatusCode(500, ApiResponse<object>.Failure($"获取状态失败: {ex.Message}"));
            }
        }
    }
}