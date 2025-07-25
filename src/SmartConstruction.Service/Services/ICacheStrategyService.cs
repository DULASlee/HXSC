using SmartConstruction.Contracts.Dtos.CacheStrategy;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface ICacheStrategyService : IBaseService<CacheStrategy, CacheStrategyDto, CreateCacheStrategyRequest, UpdateCacheStrategyRequest>
{
} 