using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.EntityRegistry;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

/// <summary>
/// 实体注册服务实现
/// </summary>
public class EntityRegistryService : BaseService<EntityRegistry, EntityRegistryDto, CreateEntityRegistryRequest, UpdateEntityRegistryRequest>, IEntityRegistryService
{
    public EntityRegistryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<EntityRegistryService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    /// <summary>
    /// 根据实体类型获取注册信息
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>实体注册信息</returns>
    public async Task<EntityRegistryDto?> GetByEntityTypeAsync(string entityType)
    {
        try
        {
            var entities = await GetByConditionAsync(e => e.EntityType == entityType && !e.IsDeleted);
            return entities.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据实体类型获取注册信息失败: EntityType={EntityType}", entityType);
            throw;
        }
    }

    /// <summary>
    /// 获取所有启用的实体注册信息
    /// </summary>
    /// <returns>启用的实体注册信息集合</returns>
    public async Task<IEnumerable<EntityRegistryDto>> GetEnabledAsync()
    {
        try
        {
            var entities = await GetByConditionAsync(e => !e.IsDeleted);
            return entities;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取启用的实体注册信息失败");
            throw;
        }
    }
} 
