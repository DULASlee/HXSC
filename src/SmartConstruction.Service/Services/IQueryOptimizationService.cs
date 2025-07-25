using SmartConstruction.Contracts.Dtos.QueryOptimization;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IQueryOptimizationService : IBaseService<QueryOptimization, QueryOptimizationDto, CreateQueryOptimizationRequest, UpdateQueryOptimizationRequest>
{
} 