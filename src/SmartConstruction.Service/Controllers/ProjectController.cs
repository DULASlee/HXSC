using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Project;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 项目管理控制器
    /// </summary>
    [Route("api/projects")]
    [ApiController]
    public class ProjectController : BaseApiController
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="projectCode">项目编号</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="companyId">公司ID</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>项目列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetProjects(
            [FromQuery] string projectCode,
            [FromQuery] string projectName,
            [FromQuery] Guid? companyId,
            [FromQuery] string status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var queryParams = new ProjectQueryParams
                {
                    ProjectCode = projectCode,
                    ProjectName = projectName,
                    CompanyId = companyId,
                    Status = status,
                    PageIndex = pageIndex < 1 ? 1 : pageIndex,
                    PageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize)
                };

                var result = await _projectService.GetProjectsAsync(queryParams);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取项目列表时发生错误");
                return Error("获取项目列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>项目信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            try
            {
                var project = await _projectService.GetProjectByIdAsync(id);
                return Success(project);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{id})时发生错误");
                return Error("获取项目信息失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的项目信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            try
            {
                var project = await _projectService.CreateProjectAsync(request);
                return Success(project);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建项目时发生错误");
                return Error("创建项目失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的项目信息</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] UpdateProjectRequest request)
        {
            try
            {
                var project = await _projectService.UpdateProjectAsync(id, request);
                return Success(project);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新项目(ID:{id})时发生错误");
                return Error("更新项目失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                var result = await _projectService.DeleteProjectAsync(id);
                if (result)
                {
                    return Success(null, "删除项目成功");
                }
                return Error("未找到要删除的项目", 404);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除项目(ID:{id})时发生错误");
                return Error("删除项目失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 检查项目编号是否已存在
        /// </summary>
        /// <param name="projectCode">项目编号</param>
        /// <param name="excludeId">排除的项目ID</param>
        /// <returns>检查结果</returns>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckProjectCode([FromQuery] string projectCode, [FromQuery] Guid? excludeId = null)
        {
            try
            {
                var exists = await _projectService.IsProjectCodeExistsAsync(projectCode, excludeId);
                return Success(new { Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查项目编号时发生错误");
                return Error("检查项目编号失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 获取公司下的项目列表
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>项目列表</returns>
        [HttpGet("by-company/{companyId}")]
        public async Task<IActionResult> GetProjectsByCompany(Guid companyId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _projectService.GetProjectsByCompanyAsync(companyId, pageIndex, pageSize);
                return Success(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取公司(ID:{companyId})的项目列表时发生错误");
                return Error("获取项目列表失败：" + ex.Message);
            }
        }
    }
} 