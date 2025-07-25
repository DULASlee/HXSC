using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.UIComponent;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;

namespace SmartConstruction.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UIComponentController : BaseApiController
{
    private readonly IUIComponentService _service;
    private readonly ILogger<UIComponentController> _logger;

    public UIComponentController(IUIComponentService service, ILogger<UIComponentController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try { var result = await _service.GetAllAsync(); return Success(result); }
        catch (Exception ex) { _logger.LogError(ex, "获取UI组件失败"); return Error("获取失败"); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try { var result = await _service.GetByIdAsync(id); if (result == null) return Error("不存在", 404); return Success(result); }
        catch (Exception ex) { _logger.LogError(ex, "获取UI组件失败: Id={Id}", id); return Error("获取失败"); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUIComponentRequest request)
    {
        try { var result = await _service.CreateAsync(request); return Success(result, "创建成功"); }
        catch (Exception ex) { _logger.LogError(ex, "创建UI组件失败"); return Error("创建失败"); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUIComponentRequest request)
    {
        try { var result = await _service.UpdateAsync(id, request); return Success(result, "更新成功"); }
        catch (Exception ex) { _logger.LogError(ex, "更新UI组件失败: Id={Id}", id); return Error("更新失败"); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try { var result = await _service.DeleteAsync(id); if (result) return Success(null, "删除成功"); return Error("删除失败"); }
        catch (Exception ex) { _logger.LogError(ex, "删除UI组件失败: Id={Id}", id); return Error("删除失败"); }
    }
} 