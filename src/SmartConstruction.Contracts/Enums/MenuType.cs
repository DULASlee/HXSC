using System.ComponentModel;

namespace SmartConstruction.Contracts.Enums
{
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 目录
        /// </summary>
        [Description("目录")]
        Directory,

        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单")]
        Menu,

        /// <summary>
        /// 按钮
        /// </summary>
        [Description("按钮")]
        Button
    }
} 