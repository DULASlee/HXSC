using System;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 工人实名制考勤资料实体
    /// </summary>
    public class WorkerAttendanceProfile : BaseEntity
    {
        /// <summary>
        /// 工人ID
        /// </summary>
        public Guid WorkerId { get; set; }

        /// <summary>
        /// 当前项目ID
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// 人脸照片路径
        /// </summary>
        public string FaceImage { get; set; }

        /// <summary>
        /// 身份证人像面照片路径
        /// </summary>
        public string IdCardFrontImg { get; set; }

        /// <summary>
        /// 身份证国徽面照片路径
        /// </summary>
        public string IdCardBackImg { get; set; }

        /// <summary>
        /// 劳动合同扫描件路径
        /// </summary>
        public string ContractImg { get; set; }

        /// <summary>
        /// 安全培训合格证路径
        /// </summary>
        public string TrainingCertImg { get; set; }

        /// <summary>
        /// 健康证明路径
        /// </summary>
        public string HealthCertImg { get; set; }

        /// <summary>
        /// 是否通过实名认证 (0:未认证, 1:已认证)
        /// </summary>
        public bool IsVerified { get; set; }

        /// <summary>
        /// 认证通过时间
        /// </summary>
        public DateTime? VerificationTime { get; set; }

        // 导航属性
        public virtual Worker Worker { get; set; }
        public virtual Project Project { get; set; }
    }
}
