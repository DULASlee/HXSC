using SmartConstruction.Contracts.Dtos.EntityRegistry;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

/// <summary>
/// 实体注册服务接口
/// </summary>
public interface IEntityRegistryService : IBaseService<EntityRegistry, EntityRegistryDto, CreateEntityRegistryRequest, UpdateEntityRegistryRequest>
{
    /// <summary>
    /// 根据实体类型获取注册信息
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>实体注册信息</returns>
    Task<EntityRegistryDto?> GetByEntityTypeAsync(string entityType);

    /// <summary>
    /// 获取所有启用的实体注册信息
    /// </summary>
    /// <returns>启用的实体注册信息集合</returns>
    Task<IEnumerable<EntityRegistryDto>> GetEnabledAsync();
} 