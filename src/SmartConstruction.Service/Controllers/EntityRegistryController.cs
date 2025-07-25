using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.EntityRegistry;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;

namespace SmartConstruction.Service.Controllers;

/// <summary>
/// 实体注册控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EntityRegistryController : BaseApiController
{
    private readonly IEntityRegistryService _entityRegistryService;
    private readonly ILogger<EntityRegistryController> _logger;

    public EntityRegistryController(IEntityRegistryService entityRegistryService, ILogger<EntityRegistryController> logger)
    {
        _entityRegistryService = entityRegistryService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有实体注册信息
    /// </summary>
    /// <returns>实体注册信息列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _entityRegistryService.GetAllAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有实体注册信息失败");
            return Error("获取实体注册信息失败");
        }
    }

    /// <summary>
    /// 根据ID获取实体注册信息
    /// </summary>
    /// <param name="id">实体注册信息ID</param>
    /// <returns>实体注册信息详情</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _entityRegistryService.GetByIdAsync(id);
            if (result == null)
            {
                return Error("实体注册信息不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据ID获取实体注册信息失败: Id={Id}", id);
            return Error("获取实体注册信息失败");
        }
    }

    /// <summary>
    /// 根据实体类型获取注册信息
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>实体注册信息</returns>
    [HttpGet("type/{entityType}")]
    public async Task<IActionResult> GetByEntityType(string entityType)
    {
        try
        {
            var result = await _entityRegistryService.GetByEntityTypeAsync(entityType);
            if (result == null)
            {
                return Error("实体注册信息不存在", 404);
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据实体类型获取注册信息失败: EntityType={EntityType}", entityType);
            return Error("获取实体注册信息失败");
        }
    }

    /// <summary>
    /// 获取所有启用的实体注册信息
    /// </summary>
    /// <returns>启用的实体注册信息列表</returns>
    [HttpGet("enabled")]
    public async Task<IActionResult> GetEnabled()
    {
        try
        {
            var result = await _entityRegistryService.GetEnabledAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用的实体注册信息失败");
            return Error("获取启用的实体注册信息失败");
        }
    }

    /// <summary>
    /// 创建实体注册信息
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEntityRegistryRequest request)
    {
        try
        {
            var result = await _entityRegistryService.CreateAsync(request);
            return Success(result, "实体注册信息创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建实体注册信息失败");
            return Error("创建实体注册信息失败");
        }
    }

    /// <summary>
    /// 更新实体注册信息
    /// </summary>
    /// <param name="id">实体注册信息ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEntityRegistryRequest request)
    {
        try
        {
            var result = await _entityRegistryService.UpdateAsync(id, request);
            return Success(result, "实体注册信息更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新实体注册信息失败: Id={Id}", id);
            return Error("更新实体注册信息失败");
        }
    }

    /// <summary>
    /// 删除实体注册信息
    /// </summary>
    /// <param name="id">实体注册信息ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _entityRegistryService.DeleteAsync(id);
            if (result)
            {
                return Success(null, "实体注册信息删除成功");
            }
            return Error("实体注册信息删除失败");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除实体注册信息失败: Id={Id}", id);
            return Error("删除实体注册信息失败");
        }
    }
} 