using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.RefreshToken;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;

namespace SmartConstruction.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RefreshTokenController : BaseApiController
{
    private readonly IRefreshTokenService _service;
    private readonly ILogger<RefreshTokenController> _logger;

    public RefreshTokenController(IRefreshTokenService service, ILogger<RefreshTokenController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try { var result = await _service.GetAllAsync(); return Success(result); }
        catch (Exception ex) { _logger.LogError(ex, "获取刷新令牌失败"); return Error("获取失败"); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try { var result = await _service.GetByIdAsync(id); if (result == null) return Error("不存在", 404); return Success(result); }
        catch (Exception ex) { _logger.LogError(ex, "获取刷新令牌失败: Id={Id}", id); return Error("获取失败"); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRefreshTokenRequest request)
    {
        try { var result = await _service.CreateAsync(request); return Success(result, "创建成功"); }
        catch (Exception ex) { _logger.LogError(ex, "创建刷新令牌失败"); return Error("创建失败"); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRefreshTokenRequest request)
    {
        try { var result = await _service.UpdateAsync(id, request); return Success(result, "更新成功"); }
        catch (Exception ex) { _logger.LogError(ex, "更新刷新令牌失败: Id={Id}", id); return Error("更新失败"); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try { var result = await _service.DeleteAsync(id); if (result) return Success(null, "删除成功"); return Error("删除失败"); }
        catch (Exception ex) { _logger.LogError(ex, "删除刷新令牌失败: Id={Id}", id); return Error("删除失败"); }
    }
} 