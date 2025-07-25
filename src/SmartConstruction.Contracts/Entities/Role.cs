using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role : BaseEntity
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid TenantId { get; set; }

        /// <summary>
        /// 角色代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 角色描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 数据范围（All:全部 Organization:组织 Department:部门 Self:个人）
        /// </summary>
        public string DataScope { get; set; } = "Self";

        /// <summary>
        /// 状态（1:启用 0:禁用）
        /// </summary>
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 是否系统角色
        /// </summary>
        public bool IsSystem { get; set; } = false;

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 角色类型（System:系统角色 Business:业务角色 Custom:自定义角色）
        /// </summary>
        public string RoleType { get; set; } = "Business";

        /// <summary>
        /// 角色级别
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 租户
        /// </summary>
        public virtual Tenant Tenant { get; set; } = null!;

        /// <summary>
        /// 用户角色关系
        /// </summary>
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// 角色权限关系
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        /// <summary>
        /// 角色菜单关系
        /// </summary>
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();
    }
} 