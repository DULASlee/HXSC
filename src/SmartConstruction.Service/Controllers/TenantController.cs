using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Models;
using System.Security.Claims;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 租户管理控制器
    /// </summary>
    [ApiController]
    [Route("api/tenant")]
    public class TenantController : BaseApiController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TenantController> _logger;

        public TenantController(
            ApplicationDbContext context,
            ILogger<TenantController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 获取租户列表（公开接口，用于登录页面）
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>租户列表</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTenantList([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var query = _context.Tenants
                    .Where(t => t.Status == 1 && !t.IsDeleted)
                    .OrderBy(t => t.Code);

                var totalCount = await query.CountAsync();
                var items = await query
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .Select(t => new TenantDto
                    {
                        Id = t.Id.ToString(),
                        Code = t.Code,
                        Name = t.Name,
                        Status = t.Status,
                        IsolationMode = t.IsolationMode,
                        Logo = t.Logo,
                        Theme = t.Theme
                    })
                    .ToListAsync();

                var result = new PagedResult<TenantDto>
                {
                    Items = items,
                    TotalCount = totalCount,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };

                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取租户列表失败");
                return Error("服务器内部错误", 500);
            }
        }
    }

    /// <summary>
    /// 租户DTO
    /// </summary>
    public class TenantDto
    {
        public string Id { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public byte Status { get; set; }
        public string? IsolationMode { get; set; }
        public string? Logo { get; set; }
        public string? Theme { get; set; }
        public string? CreatedAt { get; set; }
    }

    /// <summary>
    /// 分页结果
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
