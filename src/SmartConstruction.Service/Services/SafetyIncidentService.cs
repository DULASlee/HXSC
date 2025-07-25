using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Safety;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using SmartConstruction.Service.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 安全事件服务实现
    /// </summary>
    public class SafetyIncidentService : ISafetyIncidentService
    {
        private readonly ILogger<SafetyIncidentService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SafetyIncidentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SafetyIncidentService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 获取安全事件分页列表
        /// </summary>
        public async Task<PagedResult<SafetyIncidentDto>> GetPagedListAsync(Guid? projectId = null, string? type = null, string? level = null, bool? isHandled = null, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
               
                var query = _unitOfWork.SafetyIncidentRepository.GetAll();

                // 应用过滤条件
                if (projectId.HasValue)
                {
                    query = query.Where(s => s.ProjectId == projectId.Value);
                }
                //if (!string.IsNullOrEmpty(type))
                //{
                //    query = query.Where(s => s.Type == type);
                //}
                //if (!string.IsNullOrEmpty(level))
                //{
                //    query = query.Where(s => s.Level == level);
                //}
                //if (isHandled.HasValue)
                //{
                //    query = query.Where(s => s.IsHandled == isHandled.Value);
                //}



                var totalCount = await _unitOfWork.SafetyIncidentRepository.CountAsync(query);
                var incidents = await _unitOfWork.SafetyIncidentRepository.GetPagedAsync(
                    query.OrderByDescending(s => s.CreatedAt),
                    pageIndex,
                    pageSize);

                var incidentDtos = _mapper.Map<List<SafetyIncidentDto>>(incidents);

                return new PagedResult<SafetyIncidentDto>
                {
                    Items = incidentDtos,
                    Total = totalCount,
                    Page = pageIndex,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取安全事件列表时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取安全事件详情
        /// </summary>
        public async Task<SafetyIncidentDto?> GetByIdAsync(Guid id)
        {
            try
            {
                var incident = await _unitOfWork.SafetyIncidentRepository.GetByIdAsync(id, s => s.Project);
                return incident != null ? _mapper.Map<SafetyIncidentDto>(incident) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取安全事件详情时发生错误, ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// 创建安全事件
        /// </summary>
        public async Task<SafetyIncidentDto> CreateAsync(CreateSafetyIncidentDto createDto)
        {
            if (createDto == null)
            {
                throw new ArgumentNullException(nameof(createDto));
            }

            try
            {
                var incident = _mapper.Map<SafetyIncident>(createDto);
                incident.CreatedAt = DateTime.UtcNow;
                incident.IncidentDate = DateTime.Now;
                incident.IsHandled = false;

                await _unitOfWork.SafetyIncidentRepository.AddAsync(incident);
                await _unitOfWork.SaveChangesAsync();

                // 重新加载包含导航属性的实体
                var createdIncident = await _unitOfWork.SafetyIncidentRepository.GetByIdAsync(incident.Id, s => s.Project);
                return _mapper.Map<SafetyIncidentDto>(createdIncident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建安全事件时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新安全事件
        /// </summary>
        public async Task<SafetyIncidentDto> UpdateAsync(Guid id, UpdateSafetyIncidentDto updateDto)
        {
            if (updateDto == null)
            {
                throw new ArgumentNullException(nameof(updateDto));
            }

            try
            {
                var incident = await _unitOfWork.SafetyIncidentRepository.GetByIdAsync(id);
                if (incident == null)
                {
                    throw new InvalidOperationException($"安全事件不存在，ID: {id}");
                }

                _mapper.Map(updateDto, incident);
                incident.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SafetyIncidentRepository.UpdateAsync(incident);
                await _unitOfWork.SaveChangesAsync();

                // 重新加载包含导航属性的实体
                var updatedIncident = await _unitOfWork.SafetyIncidentRepository.GetByIdAsync(id, s => s.Project);
                return _mapper.Map<SafetyIncidentDto>(updatedIncident);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新安全事件时发生错误, ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// 删除安全事件
        /// </summary>
        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var incident = await _unitOfWork.SafetyIncidentRepository.GetByIdAsync(id);
                if (incident == null)
                {
                    return false;
                }

                await _unitOfWork.SafetyIncidentRepository.DeleteAsync(incident);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除安全事件时发生错误, ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// 处理安全事件
        /// </summary>
        public async Task<bool> HandleIncidentAsync(Guid id, string handledBy, string handlingResult)
        {
            if (string.IsNullOrWhiteSpace(handledBy))
            {
                throw new ArgumentException("处理人不能为空", nameof(handledBy));
            }

            if (string.IsNullOrWhiteSpace(handlingResult))
            {
                throw new ArgumentException("处理结果不能为空", nameof(handlingResult));
            }

            try
            {
                var incident = await _unitOfWork.SafetyIncidentRepository.GetByIdAsync(id);
                if (incident == null)
                {
                    return false;
                }

                incident.IsHandled = true;
                incident.HandledTime = DateTime.Now;
                incident.HandledBy = handledBy;
                incident.HandlingResult = handlingResult;
                incident.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SafetyIncidentRepository.UpdateAsync(incident);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理安全事件时发生错误, ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// 获取安全统计信息
        /// </summary>
        public async Task<SafetyStatisticsDto> GetStatisticsAsync(Guid? projectId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var start = startDate ?? DateTime.Now.AddDays(-30);
                var end = endDate ?? DateTime.Now;

                var query = _unitOfWork.SafetyIncidentRepository.GetAll();

                if (projectId.HasValue)
                {
                    query = query.Where(s => s.ProjectId == projectId.Value);
                }

                query = query.Where(s => s.IncidentDate >= start && s.IncidentDate <= end);

                // 使用异步方法获取数据
                var incidents = await query.ToListAsync();

                // 在内存中进行统计计算
                var statistics = new SafetyStatisticsDto
                {
                    TotalIncidents = incidents.Count,
                    HandledIncidents = incidents.Count(i => i.IsHandled),
                    UnhandledIncidents = incidents.Count(i => !i.IsHandled),
                    IncidentsByLevel = incidents
                        .GroupBy(i => i.Level ?? "未分级")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    IncidentsByType = incidents
                        .GroupBy(i => i.Type ?? "未分类")
                        .ToDictionary(g => g.Key, g => g.Count()),
                    Last30DaysTrend = GetLast30DaysTrend(incidents, start, end)
                };

                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取安全统计信息时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取最近30天的趋势数据
        /// </summary>
        private List<DailyIncidentCountDto> GetLast30DaysTrend(List<SafetyIncident> incidents, DateTime start, DateTime end)
        {
            var groupedData = incidents
                .GroupBy(i => i.IncidentDate.Date)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = new List<DailyIncidentCountDto>();
            var currentDate = start.Date;

            while (currentDate <= end.Date)
            {
                result.Add(new DailyIncidentCountDto
                {
                    Date = currentDate,
                    Count = groupedData.ContainsKey(currentDate) ? groupedData[currentDate] : 0
                });
                currentDate = currentDate.AddDays(1);
            }

            return result.OrderBy(d => d.Date).ToList();
        }

        /// <summary>
        /// 获取项目安全统计信息
        /// </summary>
        public async Task<SafetyStatisticsDto> GetProjectSafetyStatisticsAsync(Guid projectId)
        {
            return await GetStatisticsAsync(projectId);
        }
    }
}
