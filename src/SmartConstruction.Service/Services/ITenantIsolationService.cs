using SmartConstruction.Contracts.Dtos.TenantIsolation;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface ITenantIsolationService : IBaseService<TenantIsolation, TenantIsolationDto, CreateTenantIsolationRequest, UpdateTenantIsolationRequest>
{
} 