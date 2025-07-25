namespace SmartConstruction.Contracts.Dtos.Dashboard
{
    /// <summary>
    /// 全局统计数据DTO
    /// </summary>
    public class GlobalStatsDto
    {
        /// <summary>
        /// 用户总数
        /// </summary>
        public int UserCount { get; set; }

        /// <summary>
        /// 租户总数
        /// </summary>
        public int TenantCount { get; set; }

        /// <summary>
        /// 项目总数
        /// </summary>
        public int ProjectCount { get; set; }

        /// <summary>
        /// 设备总数
        /// </summary>
        public int DeviceCount { get; set; }

        /// <summary>
        /// 系统版本
        /// </summary>
        public string SystemVersion { get; set; }

        /// <summary>
        
        /// </summary>
        public string Environment { get; set; }
    }
} 