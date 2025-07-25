using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Controllers.Base
{
    /// <summary>
    /// 基础API控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns>成功结果</returns>
        protected IActionResult Success(object? data = null, string message = "操作成功")
        {
            return Ok(ApiResponse.CreateSuccess(data, message));
        }

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="statusCode">HTTP状态码</param>
        /// <returns>失败结果</returns>
        protected IActionResult Error(string message, int statusCode = 400)
        {
            return StatusCode(statusCode, ApiResponse.CreateError(message));
        }
    }
} 