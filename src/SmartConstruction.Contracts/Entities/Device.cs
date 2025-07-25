using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 设备实体
    /// </summary>
    public class Device : BaseEntity
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 安装位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// MAC地址
        /// </summary>
        public string MacAddress { get; set; }

        /// <summary>
        /// 设备状态 (ONLINE,OFFLINE,MAINTENANCE,FAULT)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 最后在线时间
        /// </summary>
        public DateTime? LastOnlineTime { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? LastUpdateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        // 导航属性
        public virtual Project Project { get; set; }
        public virtual ICollection<DeviceMaintenanceRecord> MaintenanceRecords { get; set; } = new List<DeviceMaintenanceRecord>();
    }
}
