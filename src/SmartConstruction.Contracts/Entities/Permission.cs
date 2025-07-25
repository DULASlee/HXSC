namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 权限实体
    /// </summary>
    public class Permission : BaseEntity
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 权限名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 权限描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 权限类型（System、Menu、Button、API、Data）
        /// </summary>
        public string Type { get; set; } = "Menu";

        /// <summary>
        /// 父级权限ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 权限层级路径
        /// </summary>
        public string? TreePath { get; set; }

        /// <summary>
        /// 权限层级
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态（1:启用 0:禁用）
        /// </summary>
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 是否系统权限
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// API路径
        /// </summary>
        public string? ApiPath { get; set; }

        /// <summary>
        /// HTTP方法
        /// </summary>
        public string? HttpMethods { get; set; }

        /// <summary>
        /// 资源标识符
        /// </summary>
        public string? ResourceIdentifier { get; set; }

        /// <summary>
        /// 权限条件（JSON格式）
        /// </summary>
        public string? Conditions { get; set; }

        /// <summary>
        /// 权限图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 权限颜色
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 父级权限
        /// </summary>
        public virtual Permission? Parent { get; set; }

        /// <summary>
        /// 子权限列表
        /// </summary>
        public virtual ICollection<Permission> Children { get; set; } = new List<Permission>();

        /// <summary>
        /// 角色权限关系
        /// </summary>
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

        /// <summary>
        /// 用户权限关系
        /// </summary>
        public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
    }
}