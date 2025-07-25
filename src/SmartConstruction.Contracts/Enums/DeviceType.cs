using System.ComponentModel;

namespace SmartConstruction.Contracts.Enums
{
    /// <summary>
    /// 设备类型
    /// </summary>
    public enum DeviceType
    {
        /// <summary>
        /// 塔式起重机
        /// </summary>
        [Description("塔式起重机")]
        Crane,

        /// <summary>
        /// 施工升降机
        /// </summary>
        [Description("施工升降机")]
        Elevator,

        /// <summary>
        /// 环境监测传感器
        /// </summary>
        [Description("环境监测传感器")]
        Sensor,

        /// <summary>
        /// 视频监控
        /// </summary>
        [Description("视频监控")]
        Camera,
        
        /// <summary>
        /// 其他设备
        /// </summary>
        [Description("其他设备")]
        Other
    }
}