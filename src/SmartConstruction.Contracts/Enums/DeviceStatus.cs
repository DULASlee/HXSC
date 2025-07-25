using System.ComponentModel;

namespace SmartConstruction.Contracts.Enums
{
    /// <summary>
    /// 设备状态
    /// </summary>
    public enum DeviceStatus
    {
        /// <summary>
        /// 离线
        /// </summary>
        [Description("离线")]
        Offline,

        /// <summary>
        /// 在线
        /// </summary>
        [Description("在线")]
        Online,

        /// <summary>
        /// 待机
        /// </summary>
        [Description("待机")]
        Standby,

        /// <summary>
        /// 故障
        /// </summary>
        [Description("故障")]
        Fault,

        /// <summary>
        /// 维修中
        /// </summary>
        [Description("维修中")]
        Maintenance
    }
}