using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Organization;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class OrganizationService : BaseService<Organization, OrganizationDto, CreateOrganizationRequest, UpdateOrganizationRequest>, IOrganizationService
{
    public OrganizationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrganizationService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 