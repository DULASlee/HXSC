using SmartConstruction.Contracts.Dtos.Team;
using SmartConstruction.Contracts.Dtos.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 班组服务接口
    /// </summary>
    public interface ITeamService
    {
        /// <summary>
        /// 获取班组分页列表
        /// </summary>
        Task<PagedResult<TeamDto>> GetPagedListAsync(Guid? projectId = null, string? searchKeyword = null, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 根据ID获取班组详情
        /// </summary>
        Task<TeamDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// 创建班组
        /// </summary>
        Task<TeamDto> CreateAsync(CreateTeamRequest createTeamDto);

        /// <summary>
        /// 更新班组
        /// </summary>
        Task<TeamDto> UpdateAsync(Guid id, UpdateTeamRequest updateTeamDto);

        /// <summary>
        /// 删除班组
        /// </summary>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 获取项目下的所有班组
        /// </summary>
        Task<List<TeamDto>> GetByProjectIdAsync(Guid projectId);
    }
}
