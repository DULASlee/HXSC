using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Authorization;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class AuthorizationService : BaseService<Authorization, AuthorizationDto, CreateAuthorizationRequest, UpdateAuthorizationRequest>, IAuthorizationService
{
    public AuthorizationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AuthorizationService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 