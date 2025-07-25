using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.UserRole;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class UserRoleService : BaseService<UserRole, UserRoleDto, CreateUserRoleRequest, UpdateUserRoleRequest>, IUserRoleService
{
    public UserRoleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserRoleService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 