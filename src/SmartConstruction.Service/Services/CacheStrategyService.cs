using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.CacheStrategy;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class CacheStrategyService : BaseService<CacheStrategy, CacheStrategyDto, CreateCacheStrategyRequest, UpdateCacheStrategyRequest>, ICacheStrategyService
{
    public CacheStrategyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CacheStrategyService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 
