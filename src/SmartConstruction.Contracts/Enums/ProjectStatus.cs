using System.ComponentModel;

namespace SmartConstruction.Contracts.Enums
{
    /// <summary>
    /// 项目状态
    /// </summary>
    public enum ProjectStatus
    {
        /// <summary>
        /// 规划中
        /// </summary>
        [Description("规划中")]
        Planning,

        /// <summary>
        /// 进行中
        /// </summary>
        [Description("进行中")]
        InProgress,

        /// <summary>
        /// 已完成
        /// </summary>
        [Description("已完成")]
        Completed,

        /// <summary>
        /// 已暂停
        /// </summary>
        [Description("已暂停")]
        OnHold,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancelled
    }
}