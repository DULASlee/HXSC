using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Metadata;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;

namespace SmartConstruction.Service.Controllers;

/// <summary>
/// 元数据控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MetadataController : BaseApiController
{
    private readonly IMetadataService _metadataService;
    private readonly ILogger<MetadataController> _logger;

    public MetadataController(IMetadataService metadataService, ILogger<MetadataController> logger)
    {
        _metadataService = metadataService;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有元数据
    /// </summary>
    /// <returns>元数据列表</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var result = await _metadataService.GetAllAsync();
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取元数据列表失败");
            return Error("获取元数据失败");
        }
    }

    /// <summary>
    /// 根据ID获取元数据
    /// </summary>
    /// <param name="id">元数据ID</param>
    /// <returns>元数据</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var result = await _metadataService.GetByIdAsync(id);
            if (result == null)
            {
                return Error("元数据不存在");
            }
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取元数据失败: Id={Id}", id);
            return Error("获取元数据失败");
        }
    }

    /// <summary>
    /// 根据实体类型获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>元数据列表</returns>
    [HttpGet("type/{entityType}")]
    public async Task<IActionResult> GetByEntityType(string entityType)
    {
        try
        {
            var result = await _metadataService.GetByEntityTypeAsync(entityType);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据实体类型获取元数据失败: EntityType={EntityType}", entityType);
            return Error("获取元数据失败");
        }
    }

    /// <summary>
    /// 根据实体类型和字段名获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="fieldName">字段名</param>
    /// <returns>元数据</returns>
    [HttpGet("type/{entityType}/field/{fieldName}")]
    public async Task<IActionResult> GetByEntityTypeAndField(string entityType, string fieldName)
    {
        try
        {
            var result = await _metadataService.GetByEntityTypeAndFieldAsync(entityType, fieldName);
            return Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据实体类型和字段名获取元数据失败: EntityType={EntityType}, FieldName={FieldName}", entityType, fieldName);
            return Error("获取元数据失败");
        }
    }

    /// <summary>
    /// 创建元数据
    /// </summary>
    /// <param name="request">创建请求</param>
    /// <returns>创建结果</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMetadataRequest request)
    {
        try
        {
            var result = await _metadataService.CreateAsync(request);
            return Success(result, "元数据创建成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建元数据失败");
            return Error("创建元数据失败");
        }
    }

    /// <summary>
    /// 更新元数据
    /// </summary>
    /// <param name="id">元数据ID</param>
    /// <param name="request">更新请求</param>
    /// <returns>更新结果</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMetadataRequest request)
    {
        try
        {
            var result = await _metadataService.UpdateAsync(id, request);
            return Success(result, "元数据更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新元数据失败: Id={Id}", id);
            return Error("更新元数据失败");
        }
    }

    /// <summary>
    /// 删除元数据
    /// </summary>
    /// <param name="id">元数据ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _metadataService.DeleteAsync(id);
            return Success(result, "元数据删除成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除元数据失败: Id={Id}", id);
            return Error("删除元数据失败");
        }
    }
} 