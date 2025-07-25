using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.User;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services;

public class UserService : BaseService<User, UserDto, CreateUserRequest, UpdateUserRequest>, IUserService
{
    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }
    
    public async Task<bool> ResetPasswordAsync(Guid id, string password)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("重置密码失败：用户不存在 Id={Id}", id);
                return false;
            }
            
            // 使用安全的密码哈希算法
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            user.PasswordHash = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            
            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("用户密码重置成功 Id={Id}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重置密码时发生错误 Id={Id}", id);
            throw;
        }
    }
    
    public async Task<bool> AssignRolesAsync(Guid id, List<Guid> roleIds)
    {
        try
        {
            var user = await _unitOfWork.GetRepository<User>()
                .GetByIdAsync(id);
                
            if (user == null)
            {
                _logger.LogWarning("分配角色失败：用户不存在 Id={Id}", id);
                return false;
            }
            
            // 清除现有角色
            user.UserRoles.Clear();
            
            // 添加新角色
            foreach (var roleId in roleIds)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = id,
                    RoleId = roleId,
                    CreatedAt = DateTime.UtcNow
                });
            }
            
            await _unitOfWork.GetRepository<User>().UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            
            _logger.LogInformation("用户角色分配成功 Id={Id}, RoleIds={RoleIds}", id, string.Join(",", roleIds));
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "分配角色时发生错误 Id={Id}", id);
            throw;
        }
    }
} 