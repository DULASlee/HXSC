using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Safety;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 安全事件服务接口
    /// </summary>
    public interface ISafetyIncidentService
    {
        /// <summary>
        /// 获取安全事件分页列表
        /// </summary>
        Task<PagedResult<SafetyIncidentDto>> GetPagedListAsync(Guid? projectId = null, string? type = null, string? level = null, bool? isHandled = null, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 根据ID获取安全事件详情
        /// </summary>
        Task<SafetyIncidentDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// 创建安全事件
        /// </summary>
        Task<SafetyIncidentDto> CreateAsync(CreateSafetyIncidentDto createDto);

        /// <summary>
        /// 更新安全事件
        /// </summary>
        Task<SafetyIncidentDto> UpdateAsync(Guid id, UpdateSafetyIncidentDto updateDto);

        /// <summary>
        /// 删除安全事件
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 处理安全事件
        /// </summary>
        Task<bool> HandleIncidentAsync(Guid id, string handledBy, string handlingResult);

        /// <summary>
        /// 获取安全统计信息
        /// </summary>
        Task<SafetyStatisticsDto> GetStatisticsAsync(Guid? projectId = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// 获取项目安全统计信息
        /// </summary>
        Task<SafetyStatisticsDto> GetProjectSafetyStatisticsAsync(Guid projectId);
    }
}