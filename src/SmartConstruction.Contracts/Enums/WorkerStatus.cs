using System.ComponentModel;

namespace SmartConstruction.Contracts.Enums
{
    /// <summary>
    /// 工人状态
    /// </summary>
    public enum WorkerStatus
    {
        /// <summary>
        /// 在职
        /// </summary>
        [Description("在职")]
        Active,

        /// <summary>
        /// 离职
        /// </summary>
        [Description("离职")]
        Inactive,

        /// <summary>
        /// 休假中
        /// </summary>
        [Description("休假中")]
        OnLeave
    }
} 