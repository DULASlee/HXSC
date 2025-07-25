using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Role;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public class RoleService : BaseService<Role, RoleDto, CreateRoleRequest, UpdateRoleRequest>, IRoleService
{
    public RoleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<RoleService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
} 