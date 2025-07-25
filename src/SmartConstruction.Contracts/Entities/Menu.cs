namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 菜单实体
    /// </summary>
    public class Menu : BaseEntity
    {
        /// <summary>
        /// 租户ID
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// 父级菜单ID
        /// </summary>
        public Guid? ParentId { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜单标题（多语言）
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// 菜单路径
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        public string? Component { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string? Icon { get; set; }

        /// <summary>
        /// 菜单类型（Directory:目录 Menu:菜单 Button:按钮）
        /// </summary>
        public string Type { get; set; } = "Menu";

        /// <summary>
        /// 排序号
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜单层级
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 层级路径
        /// </summary>
        public string? TreePath { get; set; }

        /// <summary>
        /// 状态（1:启用 0:禁用）
        /// </summary>
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 是否显示（1:显示 0:隐藏）
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool IsCache { get; set; } = true;

        /// <summary>
        /// 是否固定标签页
        /// </summary>
        public bool IsAffix { get; set; }

        /// <summary>
        /// 外部链接
        /// </summary>
        public string? ExternalLink { get; set; }

        /// <summary>
        /// 打开方式（_self:当前窗口 _blank:新窗口）
        /// </summary>
        public string? Target { get; set; } = "_self";

        /// <summary>
        /// 权限标识
        /// </summary>
        public string? Permission { get; set; }

        /// <summary>
        /// 菜单元数据（JSON格式）
        /// </summary>
        public string? Meta { get; set; }

        /// <summary>
        /// 重定向路径
        /// </summary>
        public string? Redirect { get; set; }

        /// <summary>
        /// 面包屑导航
        /// </summary>
        public bool ShowBreadcrumb { get; set; } = true;

        /// <summary>
        /// 左侧菜单
        /// </summary>
        public bool ShowInMenu { get; set; } = true;

        /// <summary>
        /// 标签页
        /// </summary>
        public bool ShowInTabs { get; set; } = true;

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 父级菜单
        /// </summary>
        public virtual Menu? Parent { get; set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public virtual ICollection<Menu> Children { get; set; } = new List<Menu>();

        /// <summary>
        /// 角色菜单关系
        /// </summary>
        public virtual ICollection<RoleMenu> RoleMenus { get; set; } = new List<RoleMenu>();

        /// <summary>
        /// 用户菜单关系
        /// </summary>
        public virtual ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();
    }
}