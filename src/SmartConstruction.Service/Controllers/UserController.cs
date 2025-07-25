using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.User;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;

namespace SmartConstruction.Service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : BaseApiController
{
    private readonly IUserService _service;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService service, ILogger<UserController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try { var result = await _service.GetAllAsync(); return Success(result); }
        catch (Exception ex) { _logger.LogError(ex, "获取用户失败"); return Error("获取失败"); }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        try { var result = await _service.GetByIdAsync(id); if (result == null) return Error("不存在", 404); return Success(result); }
        catch (Exception ex) { _logger.LogError(ex, "获取用户失败: Id={Id}", id); return Error("获取失败"); }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        try { var result = await _service.CreateAsync(request); return Success(result, "创建成功"); }
        catch (Exception ex) { _logger.LogError(ex, "创建用户失败"); return Error("创建失败"); }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        try { var result = await _service.UpdateAsync(id, request); return Success(result, "更新成功"); }
        catch (Exception ex) { _logger.LogError(ex, "更新用户失败: Id={Id}", id); return Error("更新失败"); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try { var result = await _service.DeleteAsync(id); if (result) return Success(null, "删除成功"); return Error("删除失败"); }
        catch (Exception ex) { _logger.LogError(ex, "删除用户失败: Id={Id}", id); return Error("删除失败"); }
    }

    [HttpPost("{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
    {
        try 
        { 
            _logger.LogInformation("重置用户密码: Id={Id}", id);
            var result = await _service.ResetPasswordAsync(id, request.Password); 
            return Success(null, "密码重置成功"); 
        }
        catch (Exception ex) 
        { 
            _logger.LogError(ex, "重置密码失败: Id={Id}", id); 
            return Error("重置失败"); 
        }
    }

    [HttpPut("{id}/roles")]
    public async Task<IActionResult> AssignRoles(Guid id, [FromBody] AssignRolesRequest request)
    {
        try 
        { 
            _logger.LogInformation("分配用户角色: Id={Id}, RoleIds={RoleIds}", id, string.Join(",", request.RoleIds));
            var result = await _service.AssignRolesAsync(id, request.RoleIds); 
            return Success(null, "角色分配成功"); 
        }
        catch (Exception ex) 
        { 
            _logger.LogError(ex, "分配角色失败: Id={Id}", id); 
            return Error("分配失败"); 
        }
    }
}