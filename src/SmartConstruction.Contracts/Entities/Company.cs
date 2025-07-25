using System;
using System.Collections.Generic;

namespace SmartConstruction.Contracts.Entities
{
    /// <summary>
    /// 建筑公司实体
    /// </summary>
    public class Company : BaseEntity
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称（别名）
        /// </summary>
        public string Name => CompanyName;

        /// <summary>
        /// 公司地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 联系人
        /// </summary>
        public string ContactPerson { get; set; } = string.Empty;

        /// <summary>
        /// 联系邮箱
        /// </summary>
        public string ContactEmail { get; set; } = string.Empty;

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string UnifiedSocialCreditCode { get; set; }

        /// <summary>
        /// 法定代表人
        /// </summary>
        public string LegalRepresentative { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisteredAddress { get; set; }

        /// <summary>
        /// 营业执照扫描件路径
        /// </summary>
        public string BusinessLicenseImg { get; set; }

        /// <summary>
        /// 状态 (0:禁用, 1:正常)
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        // 导航属性
        public virtual ICollection<Project> Projects { get; set; }
    }
} 