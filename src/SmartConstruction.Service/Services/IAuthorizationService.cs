using SmartConstruction.Contracts.Dtos.Authorization;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IAuthorizationService : IBaseService<Authorization, AuthorizationDto, CreateAuthorizationRequest, UpdateAuthorizationRequest>
{
} 