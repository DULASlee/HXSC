using SmartConstruction.Contracts.Dtos.UserRole;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IUserRoleService : IBaseService<UserRole, UserRoleDto, CreateUserRoleRequest, UpdateUserRoleRequest>
{
} 