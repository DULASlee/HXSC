namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 角色权限关联实体
    /// </summary>
    public class RolePermission : BaseEntity
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public Guid RoleId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 权限类型（Allow:允许 Deny:拒绝）
        /// </summary>
        public string PermissionType { get; set; } = "Allow";

        /// <summary>
        /// 权限条件（JSON格式）
        /// </summary>
        public string? Conditions { get; set; }

        /// <summary>
        /// 数据范围（All:全部 Organization:组织 Department:部门 Self:个人）
        /// </summary>
        public string? DataScope { get; set; }

        /// <summary>
        /// 数据范围条件（JSON格式）
        /// </summary>
        public string? DataScopeConditions { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? EffectiveFrom { get; set; }

        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? EffectiveTo { get; set; }

        /// <summary>
        /// 授权人ID
        /// </summary>
        public Guid? GrantedBy { get; set; }

        /// <summary>
        /// 授权时间
        /// </summary>
        public DateTime? GrantedAt { get; set; }

        /// <summary>
        /// 状态（1:启用 0:禁用）
        /// </summary>
        public byte Status { get; set; } = 1;

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 角色
        /// </summary>
        public virtual Role Role { get; set; } = null!;

        /// <summary>
        /// 权限
        /// </summary>
        public virtual Permission Permission { get; set; } = null!;

        /// <summary>
        /// 授权人
        /// </summary>
        public virtual User? GrantedByUser { get; set; }
    }
}