using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.QueryOptimization;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class QueryOptimizationService : BaseService<QueryOptimization, QueryOptimizationDto, CreateQueryOptimizationRequest, UpdateQueryOptimizationRequest>, IQueryOptimizationService
{
    public QueryOptimizationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<QueryOptimizationService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 
