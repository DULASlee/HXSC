using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Project;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 项目服务实现
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProjectService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        public async Task<PagedResult<ProjectDto>> GetProjectsAsync(ProjectQueryParams queryParams)
        {
            try
            {
                var repository = _unitOfWork.ProjectRepository;
                var query = repository.GetAll();

                // 应用过滤条件
                if (!string.IsNullOrEmpty(queryParams.ProjectCode))
                {
                    query = query.Where(p => p.ProjectCode.Contains(queryParams.ProjectCode));
                }

                if (!string.IsNullOrEmpty(queryParams.ProjectName))
                {
                    query = query.Where(p => p.ProjectName.Contains(queryParams.ProjectName));
                }

                if (queryParams.CompanyId.HasValue)
                {
                    query = query.Where(p => p.CompanyId == queryParams.CompanyId.Value);
                }

                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(p => p.Status == queryParams.Status);
                }

                // 计算总记录数
                var totalCount = await repository.CountAsync(query);

                // 应用排序和分页
                var items = query
                    .OrderByDescending(p => p.CreatedAt)
                    .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToList();

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<ProjectDto>>(items);
                return new PagedResult<ProjectDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = queryParams.PageIndex,
                    PageSize = queryParams.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取项目列表时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>项目DTO</returns>
        public async Task<ProjectDto> GetProjectByIdAsync(Guid id)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的项目");
                }

                return _mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 创建项目
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的项目DTO</returns>
        public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request)
        {
            try
            {
                // 检查项目编号是否已存在
                if (await IsProjectCodeExistsAsync(request.ProjectCode))
                {
                    throw new InvalidOperationException($"项目编号'{request.ProjectCode}'已存在");
                }

                // 检查公司是否存在
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(request.CompanyId);
                if (company == null)
                {
                    throw new InvalidOperationException($"未找到ID为{request.CompanyId}的公司");
                }

                var project = _mapper.Map<Project>(request);
                project.Status = "IN_PROGRESS"; // 默认状态为进行中

                _unitOfWork.ProjectRepository.Create(project);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建项目时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的项目DTO</returns>
        public async Task<ProjectDto> UpdateProjectAsync(Guid id, UpdateProjectRequest request)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的项目");
                }

                // 检查项目编号是否已存在
                if (request.ProjectCode != project.ProjectCode && 
                    await IsProjectCodeExistsAsync(request.ProjectCode, id))
                {
                    throw new InvalidOperationException($"项目编号'{request.ProjectCode}'已存在");
                }

                // 检查公司是否存在
                if (request.CompanyId != project.CompanyId)
                {
                    var company = await _unitOfWork.CompanyRepository.GetByIdAsync(request.CompanyId);
                    if (company == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.CompanyId}的公司");
                    }
                }

                _mapper.Map(request, project);

                _unitOfWork.ProjectRepository.Update(project);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<ProjectDto>(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新项目(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="id">项目ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteProjectAsync(Guid id)
        {
            try
            {
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return false;
                }

                // 检查是否有关联的班组
                var hasTeams = await _unitOfWork.TeamRepository.ExistsAsync(t => t.ProjectId == id);
                if (hasTeams)
                {
                    throw new InvalidOperationException("该项目下存在班组，无法删除");
                }

                // 检查是否有关联的工人
                var hasWorkers = await _unitOfWork.WorkerRepository.ExistsAsync(w => w.ProjectId == id);
                if (hasWorkers)
                {
                    throw new InvalidOperationException("该项目下存在工人，无法删除");
                }

                // 检查是否有关联的设备
                var hasDevices = await _unitOfWork.DeviceRepository.ExistsAsync(d => d.ProjectId == id);
                if (hasDevices)
                {
                    throw new InvalidOperationException("该项目下存在设备，无法删除");
                }

                _unitOfWork.ProjectRepository.Delete(project);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除项目(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 检查项目编号是否已存在
        /// </summary>
        /// <param name="projectCode">项目编号</param>
        /// <param name="excludeId">排除的项目ID</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsProjectCodeExistsAsync(string projectCode, Guid? excludeId = null)
        {
            if (string.IsNullOrEmpty(projectCode))
            {
                return false;
            }

            var query = _unitOfWork.ProjectRepository.GetByCondition(p => p.ProjectCode == projectCode);
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await _unitOfWork.ProjectRepository.CountAsync(query) > 0;
        }

        /// <summary>
        /// 获取公司下的项目列表
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>项目列表</returns>
        public async Task<PagedResult<ProjectDto>> GetProjectsByCompanyAsync(Guid companyId, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // 检查公司是否存在
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId);
                if (company == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{companyId}的公司");
                }

                var repository = _unitOfWork.ProjectRepository;
                var query = repository.GetByCondition(p => p.CompanyId == companyId);

                // 计算总记录数
                var totalCount = await repository.CountAsync(query);

                // 应用排序和分页
                var projects = await _unitOfWork.ProjectRepository.GetPagedAsync(
                    query.OrderByDescending(p => p.CreatedAt),
                    pageIndex,
                    pageSize);

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<ProjectDto>>(projects);
                return new PagedResult<ProjectDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = pageIndex,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取公司(ID:{companyId})的项目列表时发生错误");
                throw;
            }
        }
    }
} 