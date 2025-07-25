using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Metadata;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

/// <summary>
/// 元数据服务实现
/// </summary>
public class MetadataService : BaseService<Metadata, MetadataDto, CreateMetadataRequest, UpdateMetadataRequest>, IMetadataService
{
    public MetadataService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MetadataService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// 根据实体类型获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>元数据集合</returns>
    public async Task<IEnumerable<MetadataDto>> GetByEntityTypeAsync(string entityType)
    {
        try
        {
            var entities = await GetByConditionAsync(m => m.EntityType == entityType && !m.IsDeleted);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取实体类型元数据失败: EntityType={EntityType}", entityType);
            throw;
        }
    }

    /// <summary>
    /// 根据实体类型和字段名获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="fieldName">字段名</param>
    /// <returns>元数据</returns>
    public async Task<MetadataDto?> GetByEntityTypeAndFieldAsync(string entityType, string fieldName)
    {
        try
        {
            var entities = await GetByConditionAsync(m => m.EntityType == entityType && m.FieldName == fieldName && !m.IsDeleted);
            return entities.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取实体类型和字段名元数据失败: EntityType={EntityType}, FieldName={FieldName}", entityType, fieldName);
            throw;
        }
    }

    /// <summary>
    /// 根据实体类型和ID获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="entityId">实体ID</param>
    /// <returns>元数据集合</returns>
    public async Task<IEnumerable<MetadataDto>> GetByEntityAsync(string entityType, string entityId)
    {
        try
        {
            var entities = await GetByConditionAsync(m => m.EntityType == entityType && m.EntityId == entityId && !m.IsDeleted);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取实体元数据失败: EntityType={EntityType}, EntityId={EntityId}", entityType, entityId);
            throw;
        }
    }

    /// <summary>
    /// 根据元数据键获取元数据
    /// </summary>
    /// <param name="metaKey">元数据键</param>
    /// <returns>元数据集合</returns>
    public async Task<IEnumerable<MetadataDto>> GetByMetaKeyAsync(string metaKey)
    {
        try
        {
            var entities = await GetByConditionAsync(m => m.MetaKey == metaKey && !m.IsDeleted);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据元数据键获取元数据失败: MetaKey={MetaKey}", metaKey);
            throw;
        }
    }

    /// <summary>
    /// 获取活跃的元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="entityId">实体ID</param>
    /// <returns>活跃元数据集合</returns>
    public async Task<IEnumerable<MetadataDto>> GetActiveMetadataAsync(string entityType, string entityId)
    {
        try
        {
            var now = DateTime.UtcNow;
            var entities = await GetByConditionAsync(m => 
                m.EntityType == entityType && 
                m.EntityId == entityId && 
                m.IsActive && 
                !m.IsDeleted &&
                (m.EffectiveFrom <= now) &&
                (m.EffectiveTo == null || m.EffectiveTo >= now));
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取活跃元数据失败: EntityType={EntityType}, EntityId={EntityId}", entityType, entityId);
            throw;
        }
    }
} 
