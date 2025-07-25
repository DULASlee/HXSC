using System.ComponentModel;

namespace SmartConstruction.Contracts.Enums
{
    /// <summary>
    /// 考勤类型
    /// </summary>
    public enum AttendanceType
    {
        /// <summary>
        /// 正常出勤
        /// </summary>
        [Description("正常出勤")]
        Normal,

        /// <summary>
        /// 迟到
        /// </summary>
        [Description("迟到")]
        Late,

        /// <summary>
        /// 早退
        /// </summary>
        [Description("早退")]
        EarlyLeave,

        /// <summary>
        /// 缺勤
        /// </summary>
        [Description("缺勤")]
        Absent,

        /// <summary>
        /// 请假
        /// </summary>
        [Description("请假")]
        Leave,

        /// <summary>
        /// 加班
        /// </summary>
        [Description("加班")]
        Overtime
    }
}