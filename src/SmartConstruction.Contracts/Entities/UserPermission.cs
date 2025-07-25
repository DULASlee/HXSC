namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 用户权限关联实体（直接授权给用户的权限）
    /// </summary>
    public class UserPermission : BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 权限ID
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// 权限类型（Allow:允许 Deny:拒绝）
        /// </summary>
        public string PermissionType { get; set; } = "Allow";

        /// <summary>
        /// 权限来源（Direct:直接授权 Role:角色授权 Group:组授权）
        /// </summary>
        public string Source { get; set; } = "Direct";

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
        /// 用户
        /// </summary>
        public virtual User User { get; set; } = null!;

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