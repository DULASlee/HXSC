using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Project;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 项目服务接口
    /// </summary>
    public interface IProjectService
    {
        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        Task<PagedResult<ProjectDto>> GetProjectsAsync(ProjectQueryParams queryParams);

        /// <summary>
        /// 根据ID获取项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>项目DTO</returns>
        Task<ProjectDto> GetProjectByIdAsync(Guid id);

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的项目DTO</returns>
        Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request);

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的项目DTO</returns>
        Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectRequest request);

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteProjectAsync(Guid id);

        /// <summary>
        /// 检查项目编号是否已存在
        /// </summary>
        /// <param name="projectCode">项目编号</param>
        /// <param name="excludeId">排除的项目ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsProjectCodeExistsAsync(string projectCode, Guid? excludeId = null);

        /// <summary>
        /// 获取公司下的项目列表
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <returns>项目列表</returns>
        Task<PagedResult<ProjectDto>> GetProjectsByCompanyAsync(Guid companyId, int pageIndex = 1, int pageSize = 10);
    }
}
