using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 设备维护记录实体
    /// </summary>
    public class DeviceMaintenanceRecord : BaseEntity
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public Guid DeviceId { get; set; }

        /// <summary>
        /// 维护类型 (REGULAR,REPAIR,CALIBRATION,INSPECTION)
        /// </summary>
        public string MaintenanceType { get; set; } = null!;

        /// <summary>
        /// 维护日期
        /// </summary>
        public DateTime MaintenanceDate { get; set; }

        /// <summary>
        /// 维护人员
        /// </summary>
        public string Maintainer { get; set; } = null!;

        /// <summary>
        /// 维护内容
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// 维护结果
        /// </summary>
        public string Result { get; set; } = null!;

        /// <summary>
        /// 下次维护日期
        /// </summary>
        public DateTime? NextMaintenanceDate { get; set; }

        /// <summary>
        /// 维护费用
        /// </summary>
        public decimal? Cost { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remarks { get; set; }

        // 导航属性
        /// <summary>
        /// 设备
        /// </summary>
        public virtual Device Device { get; set; } = null!;
    }
}