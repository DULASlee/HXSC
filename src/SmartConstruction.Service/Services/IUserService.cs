using SmartConstruction.Contracts.Dtos.User;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Services.Base;

namespace SmartConstruction.Service.Services;

public interface IUserService : IBaseService<User, UserDto, CreateUserRequest, UpdateUserRequest>
{
    Task<bool> ResetPasswordAsync(Guid id, string password);
    Task<bool> AssignRolesAsync(Guid id, List<Guid> roleIds);
} 