using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.TenantIsolation;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class TenantIsolationService : BaseService<TenantIsolation, TenantIsolationDto, CreateTenantIsolationRequest, UpdateTenantIsolationRequest>, ITenantIsolationService
{
    public TenantIsolationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TenantIsolationService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 