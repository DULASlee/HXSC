using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Dtos.Integration
{
    /// <summary>
    /// IoT设备实时数据DTO
    /// </summary>
    public class IoTDataDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 数据时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 设备数据
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 位置信息
        /// </summary>
        public string Location { get; set; }
    }

    /// <summary>
    /// IoT设备告警DTO
    /// </summary>
    public class IoTAlertDto
    {
        /// <summary>
        /// 告警ID
        /// </summary>
        public string AlertId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 告警类型
        /// </summary>
        public string AlertType { get; set; }

        /// <summary>
        /// 告警级别 (INFO,WARNING,ERROR,CRITICAL)
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 告警内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 告警时间
        /// </summary>
        public DateTime AlertTime { get; set; }

        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsHandled { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? HandledTime { get; set; }

        /// <summary>
        /// 处理人
        /// </summary>
        public string HandledBy { get; set; }
    }

    /// <summary>
    /// IoT实时数据视图模型
    /// </summary>
    public class IoTRealtimeDataViewModel
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 数据时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 设备数据
        /// </summary>
        public Dictionary<string, object> Data { get; set; }

        /// <summary>
        /// 设备状态
        /// </summary>
        public string Status { get; set; }
    }

    /// <summary>
    /// IoT告警视图模型
    /// </summary>
    public class IoTAlertViewModel
    {
        /// <summary>
        /// 告警ID
        /// </summary>
        public string AlertId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 告警类型
        /// </summary>
        public string AlertType { get; set; }

        /// <summary>
        /// 告警级别
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 告警内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 告警时间
        /// </summary>
        public DateTime AlertTime { get; set; }
    }
} 