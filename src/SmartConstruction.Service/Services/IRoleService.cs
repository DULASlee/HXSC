using SmartConstruction.Contracts.Dtos.Role;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IRoleService : IBaseService<Role, RoleDto, CreateRoleRequest, UpdateRoleRequest>
{
} 