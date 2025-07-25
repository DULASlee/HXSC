using SmartConstruction.Contracts.Dtos.Resource;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IResourceService : IBaseService<Resource, ResourceDto, CreateResourceRequest, UpdateResourceRequest>
{
} 