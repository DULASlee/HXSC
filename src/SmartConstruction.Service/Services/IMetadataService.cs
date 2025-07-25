using SmartConstruction.Contracts.Dtos.Metadata;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

/// <summary>
/// 元数据服务接口
/// </summary>
public interface IMetadataService : IBaseService<Metadata, MetadataDto, CreateMetadataRequest, UpdateMetadataRequest>
{
    /// <summary>
    /// 根据实体类型获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <returns>元数据集合</returns>
    Task<IEnumerable<MetadataDto>> GetByEntityTypeAsync(string entityType);

    /// <summary>
    /// 根据实体类型和字段名获取元数据
    /// </summary>
    /// <param name="entityType">实体类型</param>
    /// <param name="fieldName">字段名</param>
    /// <returns>元数据</returns>
    Task<MetadataDto?> GetByEntityTypeAndFieldAsync(string entityType, string fieldName);
} 