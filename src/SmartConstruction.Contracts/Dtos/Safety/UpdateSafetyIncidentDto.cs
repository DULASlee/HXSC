using System;
using System.ComponentModel.DataAnnotations;

namespace SmartConstruction.Contracts.Dtos.Safety
{
    /// <summary>
    /// 更新安全事件DTO
    /// </summary>
    public class UpdateSafetyIncidentDto
    {
        /// <summary>
        /// 事故类型
        /// </summary>
        [MaxLength(50)]
        public string? Type { get; set; }

        /// <summary>
        /// 事故级别
        /// </summary>
        [MaxLength(20)]
        public string? Level { get; set; }

        /// <summary>
        /// 事故位置
        /// </summary>
        [MaxLength(100)]
        public string? Location { get; set; }

        /// <summary>
        /// 事故描述
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        [MaxLength(255)]
        public string? ImageUrl { get; set; }
    }

    // 此类已移动到单独的 UpdateSafetyIncidentRequest.cs 文件中
    // public class UpdateSafetyIncidentRequest
    // {
    //     public Guid Id { get; set; }
    //     public Guid ProjectId { get; set; }
    //     public string Type { get; set; } = null!;
    //     public string Level { get; set; } = null!;
    //     public string Location { get; set; } = null!;
    //     public string Description { get; set; } = null!;
    //     public string? ImageUrl { get; set; }
    // }
}
