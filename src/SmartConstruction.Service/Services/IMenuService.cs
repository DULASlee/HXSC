using SmartConstruction.Contracts.Dtos;
using SmartConstruction.Service.Models;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 菜单管理服务接口
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="tenantId">租户ID</param>
        /// <returns>菜单树</returns>
        Task<ApiResponse<List<MenuTreeDto>>> GetMenuTreeAsync(Guid? tenantId = null);

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户菜单</returns>
        Task<ApiResponse<List<MenuInfo>>> GetUserMenusAsync(Guid userId);

        /// <summary>
        /// 为角色分配菜单
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <param name="menuIds">菜单ID列表</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>操作结果</returns>
        Task<ApiResponse<object>> AssignMenusToRoleAsync(Guid roleId, List<Guid> menuIds, Guid operatorId);

        /// <summary>
        /// 为用户分配菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="menuIds">菜单ID列表</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>操作结果</returns>
        Task<ApiResponse<object>> AssignMenusToUserAsync(Guid userId, List<Guid> menuIds, Guid operatorId);

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="dto">菜单DTO</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>创建结果</returns>
        Task<ApiResponse<Guid>> CreateMenuAsync(CreateMenuDto dto, Guid operatorId);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="dto">菜单DTO</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>更新结果</returns>
        Task<ApiResponse<object>> UpdateMenuAsync(Guid id, UpdateMenuDto dto, Guid operatorId);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <param name="operatorId">操作人ID</param>
        /// <returns>删除结果</returns>
        Task<ApiResponse<object>> DeleteMenuAsync(Guid id, Guid operatorId);

        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>角色菜单ID列表</returns>
        Task<ApiResponse<List<Guid>>> GetRoleMenusAsync(Guid roleId);
    }

    /// <summary>
    /// 菜单树DTO
    /// </summary>
    public class MenuTreeDto
    {
        public string Id { get; set; } = string.Empty;
        public string? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Component { get; set; }
        public string? Icon { get; set; }
        public string Type { get; set; } = "Menu";
        public int Sort { get; set; }
        public int Level { get; set; }
        public byte Status { get; set; }
        public bool IsVisible { get; set; }
        public string? Permission { get; set; }
        public List<MenuTreeDto> Children { get; set; } = new();
    }

    /// <summary>
    /// 创建菜单DTO
    /// </summary>
    public class CreateMenuDto
    {
        public Guid? TenantId { get; set; }
        public Guid? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Component { get; set; }
        public string? Icon { get; set; }
        public string Type { get; set; } = "Menu";
        public int Sort { get; set; }
        public bool IsVisible { get; set; } = true;
        public string? Permission { get; set; }
        public string? ExternalLink { get; set; }
        public string? Description { get; set; }
    }

    /// <summary>
    /// 更新菜单DTO
    /// </summary>
    public class UpdateMenuDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Path { get; set; }
        public string? Component { get; set; }
        public string? Icon { get; set; }
        public int Sort { get; set; }
        public byte Status { get; set; }
        public bool IsVisible { get; set; }
        public string? Permission { get; set; }
        public string? ExternalLink { get; set; }
        public string? Description { get; set; }
    }
}