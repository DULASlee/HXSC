using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Models;
using System;
using System.Reflection;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 系统信息控制器
    /// </summary>
    [Route("api/system")]
    [ApiController]
    public class SystemController : BaseApiController
    {
        private readonly ILogger<SystemController> _logger;

        public SystemController(ILogger<SystemController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取系统基本信息
        /// </summary>
        /// <returns>系统信息</returns>
        [HttpGet("info")]
        [AllowAnonymous]
        public IActionResult GetSystemInfo()
        {
            try
            {
                var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                var info = new
                {
                    SystemVersion = version,
                    Environment = environment,
                    ServerTime = DateTime.UtcNow,
                    SystemName = "智慧工地管理平台"
                };

                return Success(info);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取系统信息时发生错误");
                return Error("获取系统信息失败");
            }
        }
    }
} 