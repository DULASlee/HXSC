using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 权限管理服务接口
    /// </summary>
    public interface IPermissionService
    {
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns>权限树</returns>
        Task<ApiResponse<List<PermissionTreeDto>>> GetPermissionTreeAsync(Guid? tenantId = null);

        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionCode">权限代码</param>
        /// <param name="context">上下文</param>
        /// <returns>是否有权限</returns>
        Task<ApiResponse<bool>> CheckPermissionAsync(Guid userId, string permissionCode, Dictionary<string, object>? context = null);

        /// <summary>
        /// 检查任意权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionCodes">权限代码列表</param>
        /// <returns>是否有任意权限</returns>
        Task<ApiResponse<bool>> CheckAnyPermissionAsync(Guid userId, List<string> permissionCodes);

        /// <summary>
        /// 检查所有权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionCodes">权限代码列表</param>
        /// <returns>是否有所有权限</returns>
        Task<ApiResponse<bool>> CheckAllPermissionsAsync(Guid userId, List<string> permissionCodes);

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>权限列表</returns>
        Task<ApiResponse<List<string>>> GetUserPermissionsAsync(Guid userId);

        /// <summary>
        /// 检查数据范围权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="dataScope">数据范围</param>
        /// <param name="targetUserId">目标用户ID</param>
        /// <param name="targetOrgId">目标组织ID</param>
        /// <returns>是否有权限</returns>
        Task<ApiResponse<bool>> CheckDataScopeAsync(Guid userId, string dataScope, Guid? targetUserId = null, Guid? targetOrgId = null);

        /// <summary>
        /// 为角色分配权限
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="permissionIds">权限ID列表</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>操作结果</returns>
        Task<ApiResponse<object>> AssignPermissionsToRoleAsync(Guid roleId, List<Guid> permissionIds, Guid operatorId);

        /// <summary>
        /// 为用户分配权限
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="permissionIds">权限ID列表</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>操作结果</returns>
        Task<ApiResponse<object>> AssignPermissionsToUserAsync(Guid userId, List<Guid> permissionIds, Guid operatorId);

        /// <summary>
        /// 创建权限
        /// </summary>
        /// <param name="dto">权限DTO</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>创建结果</returns>
        Task<ApiResponse<Guid>> CreatePermissionAsync(CreatePermissionDto dto, Guid operatorId);

        /// <summary>
        /// 更新权限
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <param name="dto">权限DTO</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>更新结果</returns>
        Task<ApiResponse<object>> UpdatePermissionAsync(Guid id, UpdatePermissionDto dto, Guid operatorId);

        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="id">权限ID</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>删除结果</returns>
        Task<ApiResponse<object>> DeletePermissionAsync(Guid id, Guid operatorId);
    }

    /// <summary>
    /// 权限树DTO
    /// </summary>
    public class PermissionTreeDto
    {
        public string Id { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Menu";
        public int Level { get; set; }
        public int Sort { get; set; }
        public byte Status { get; set; }
        public bool IsSystem { get; set; }
        public string? Icon { get; set; }
        public List<PermissionTreeDto> Children { get; set; } = new();
    }

    /// <summary>
    /// 创建权限DTO
    /// </summary>
    public class CreatePermissionDto
    {
        public Guid? TenantId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = "Menu";
        public Guid? ParentId { get; set; }
        public int Sort { get; set; }
        public string? ApiPath { get; set; }
        public string? HttpMethods { get; set; }
        public string? Icon { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// 更新权限DTO
    /// </summary>
    public class UpdatePermissionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Sort { get; set; }
        public byte Status { get; set; }
        public string? ApiPath { get; set; }
        public string? HttpMethods { get; set; }
        public string? Icon { get; set; }
        public string? Remarks { get; set; }
    }
}