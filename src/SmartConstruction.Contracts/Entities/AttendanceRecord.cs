using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 工人考勤记录实体
    /// </summary>
    public class AttendanceRecord : BaseEntity
    {
        /// <summary>
        /// 工人ID
        /// </summary>
        public Guid WorkerId { get; set; }

        /// <summary>
        /// 项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// 考勤日期
        /// </summary>
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// 上班打卡时间
        /// </summary>
        public DateTime? ClockInTime { get; set; }

        /// <summary>
        /// 下班打卡时间
        /// </summary>
        public DateTime? ClockOutTime { get; set; }

        /// <summary>
        /// 考勤状态 (NORMAL,LATE,EARLY_LEAVE,ABSENT,LEAVE,OVERTIME,HOLIDAY)
        /// </summary>
        public string AttendanceType { get; set; }

        /// <summary>
        /// 考勤设备ID
        /// </summary>
        public string DeviceId { get; set; }

        /// <summary>
        /// 考勤位置
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 考勤来源 (GATE,FACE_RECOG,MOBILE_APP,MANUAL)
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 是否同步至监管平台 (0:未同步, 1:已同步)
        /// </summary>
        public bool IsSynced { get; set; }

        /// <summary>
        /// 是否迟到
        /// </summary>
        public bool IsLate { get; set; }

        /// <summary>
        /// 是否早退
        /// </summary>
        public bool IsEarlyLeave { get; set; }

        // 导航属性
        public virtual Worker Worker { get; set; }
        public virtual Project Project { get; set; }
        public virtual Team Team { get; set; }
    }
}
