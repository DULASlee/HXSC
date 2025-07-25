using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Resource;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class ResourceService : BaseService<Resource, ResourceDto, CreateResourceRequest, UpdateResourceRequest>, IResourceService
{
    public ResourceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ResourceService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 