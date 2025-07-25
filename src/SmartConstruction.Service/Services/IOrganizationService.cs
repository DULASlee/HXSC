using SmartConstruction.Contracts.Dtos.Organization;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IOrganizationService : IBaseService<Organization, OrganizationDto, CreateOrganizationRequest, UpdateOrganizationRequest>
{
} 