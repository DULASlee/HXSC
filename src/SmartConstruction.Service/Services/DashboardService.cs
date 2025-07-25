using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Dashboard;
using SmartConstruction.Contracts.Dtos.Safety;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data; // 添加 ApplicationDbContext 的命名空间
using Microsoft.EntityFrameworkCore; // 添加 .CountAsync() 的命名空间

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 仪表盘服务实现
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _applicationDbContext; // 添加 ApplicationDbContext
        private readonly IAttendanceService _attendanceService;
        private readonly ISafetyIncidentService _safetyIncidentService;
        private readonly IMapper _mapper;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IUnitOfWork unitOfWork,
            ApplicationDbContext applicationDbContext, // 注入 ApplicationDbContext
            IAttendanceService attendanceService,
            ISafetyIncidentService safetyIncidentService,
            IMapper mapper,
            ILogger<DashboardService> logger)
        {
            _unitOfWork = unitOfWork;
            _applicationDbContext = applicationDbContext; // 赋值
            _attendanceService = attendanceService;
            _safetyIncidentService = safetyIncidentService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 获取项目概览
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>项目概览DTO</returns>
        public async Task<ProjectOverviewDto> GetProjectOverviewAsync(Guid projectId)
        {
            try
            {
                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{projectId}的项目");
                }

                // 获取项目下的班组数量
                var teamCount = await _unitOfWork.TeamRepository.CountAsync(t => t.ProjectId == projectId);

                // 获取项目下的工人数量
                var totalWorkers = await _unitOfWork.WorkerRepository.CountAsync(w => w.ProjectId.HasValue && w.ProjectId.Value == projectId);

                // 获取项目下的设备数量
                var deviceCount = await _unitOfWork.DeviceRepository.CountAsync(d => d.ProjectId == projectId);
                var onlineDeviceCount = await _unitOfWork.DeviceRepository.CountAsync(d => d.ProjectId == projectId && d.Status == "ONLINE");

                // 获取项目下的安全事件数量
                var unhandledSafetyIncidentCount = await _unitOfWork.SafetyIncidentRepository.CountAsync(s => s.ProjectId == projectId && !s.IsHandled);

                // 创建并返回项目概览
                return new ProjectOverviewDto
                {
                    ProjectId = projectId,
                    ProjectName = project.ProjectName,
                    Status = project.Status,
                    StartDate = project.StartDate,
                    PlannedEndDate = project.PlannedEndDate,
                    DaysInProgress = project.StartDate.HasValue ? (DateTime.Now - project.StartDate.Value).Days : 0,
                    TotalWorkers = totalWorkers,
                    PresentWorkers = 0, // TODO: 需要从考勤记录中获取
                    TeamCount = teamCount,
                    DeviceCount = deviceCount,
                    OnlineDeviceCount = onlineDeviceCount,
                    UnhandledSafetyIncidentCount = unhandledSafetyIncidentCount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})概览时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取考勤统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>考勤统计DTO</returns>
        public async Task<AttendanceStatisticsDto> GetAttendanceStatisticsAsync(Guid projectId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{projectId}的项目");
                }

                // 如果未指定日期范围，则使用当前月份
                var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var end = endDate ?? DateTime.Now;

                // 获取项目下的工人总数
                var totalWorkers = await _unitOfWork.WorkerRepository.CountAsync(w => w.ProjectId.HasValue && w.ProjectId.Value == projectId);

                // 获取日期范围内的考勤记录
                var attendanceRecords = _unitOfWork.AttendanceRecordRepository
                    .GetByCondition(a => a.ProjectId == projectId && a.AttendanceDate >= start && a.AttendanceDate <= end)
                    .ToList();

                // 计算出勤率
                var attendanceDays = attendanceRecords
                    .GroupBy(a => new { a.WorkerId, a.AttendanceDate })
                    .Select(g => g.First())
                    .Count();

                var workDays = (end - start).Days + 1;
                var attendanceRate = totalWorkers > 0 && workDays > 0
                    ? (double)attendanceDays / (totalWorkers * workDays) * 100
                    : 0;

                // 创建并返回考勤统计
                return new AttendanceStatisticsDto
                {
                    Date = DateTime.Now.Date,
                    PresentCount = attendanceRecords.Select(a => a.WorkerId).Distinct().Count(),
                    AbsentCount = Math.Max(0, totalWorkers - attendanceRecords.Select(a => a.WorkerId).Distinct().Count()),
                    LateCount = attendanceRecords.Count(a => a.IsLate),
                    EarlyLeaveCount = attendanceRecords.Count(a => a.IsEarlyLeave),
                    LeaveCount = 0, // TODO: 需要从请假记录中获取
                    AttendanceRate = Math.Round(attendanceRate, 2)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})考勤统计时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取安全事件统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>安全事件统计DTO</returns>
        public async Task<SafetyStatisticsDto> GetSafetyStatisticsAsync(Guid projectId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var safetyStats = await _safetyIncidentService.GetStatisticsAsync(projectId, startDate, endDate);
                return MapSafetyStatistics(safetyStats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})安全事件统计时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取设备状态统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <returns>设备状态统计DTO</returns>
        public async Task<DeviceStatusStatisticsDto> GetDeviceStatusStatisticsAsync(Guid projectId)
        {
            try
            {
                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{projectId}的项目");
                }

                // 获取项目下的设备
                var devices = _unitOfWork.DeviceRepository
                    .GetByCondition(d => d.ProjectId == projectId)
                    .ToList();

                // 按状态统计设备数量
                var onlineCount = devices.Count(d => d.Status == "ONLINE");
                var offlineCount = devices.Count(d => d.Status == "OFFLINE");
                var faultCount = devices.Count(d => d.Status == "FAULT");
                var maintenanceCount = devices.Count(d => d.Status == "MAINTENANCE");

                // 按类型统计设备数量
                var countByType = devices
                    .GroupBy(d => d.DeviceType)
                    .ToDictionary(g => g.Key, g => g.Count());

                // 按状态统计设备数量
                var countByStatus = devices
                    .GroupBy(d => d.Status)
                    .ToDictionary(g => g.Key, g => g.Count());

                // 创建并返回设备状态统计
                return new DeviceStatusStatisticsDto
                {
                    OnlineCount = onlineCount,
                    OfflineCount = offlineCount,
                    FaultCount = faultCount,
                    MaintenanceCount = maintenanceCount,
                    TotalCount = devices.Count,
                    CountByType = countByType,
                    CountByStatus = countByStatus
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})设备状态统计时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取项目完整仪表盘数据
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>项目仪表盘DTO</returns>
        public async Task<ProjectDashboardDto> GetProjectDashboardAsync(Guid projectId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // 并行获取各项统计数据
                var overviewTask = GetProjectOverviewAsync(projectId);
                var attendanceTask = GetAttendanceStatisticsAsync(projectId, startDate, endDate);
                var safetyTask = GetSafetyStatisticsAsync(projectId, startDate, endDate);
                var deviceStatusTask = GetDeviceStatusStatisticsAsync(projectId);

                await Task.WhenAll(overviewTask, attendanceTask, safetyTask, deviceStatusTask);

                // 创建并返回项目仪表盘数据
                return new ProjectDashboardDto
                {
                    ProjectOverview = await overviewTask,
                    AttendanceStatistics = new List<AttendanceStatisticsDto> { await attendanceTask },
                    SafetyStatistics = await safetyTask,
                    DeviceStatistics = ConvertToDeviceStatistics(await deviceStatusTask)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})仪表盘数据时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取公司整体仪表盘数据
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>公司仪表盘DTO</returns>
        public async Task<CompanyDashboardDto> GetCompanyDashboardAsync(Guid companyId, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                // 检查公司是否存在
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(companyId);
                if (company == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{companyId}的公司");
                }

                // 如果未指定日期范围，则使用当前月份
                var start = startDate ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var end = endDate ?? DateTime.Now;

                // 获取公司下的项目列表
                var projects = _unitOfWork.ProjectRepository
                    .GetByCondition(p => p.CompanyId == companyId)
                    .ToList();

                var projectIds = projects.Select(p => p.Id).ToList();

                // 获取公司下的项目数量
                var projectCount = projects.Count;

                // 获取公司下的工人总数
                var workerCount = await _unitOfWork.WorkerRepository.CountAsync(w => w.ProjectId.HasValue && projectIds.Contains(w.ProjectId.Value));

                // 获取公司下的设备总数
                var deviceCount = await _unitOfWork.DeviceRepository.CountAsync(d => projectIds.Contains(d.ProjectId));

                // 获取公司下的安全事件总数
                var safetyIncidentCount = await _unitOfWork.SafetyIncidentRepository.CountAsync(s =>
                    projectIds.Contains(s.ProjectId) &&
                    s.IncidentDate >= start &&
                    s.IncidentDate <= end);

                // 获取公司下的项目概览列表
                var projectOverviews = new List<ProjectOverviewDto>();
                foreach (var project in projects)
                {
                    try
                    {
                        var overview = await GetProjectOverviewAsync(project.Id);
                        projectOverviews.Add(overview);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"获取项目(ID:{project.Id})概览时发生错误，已跳过");
                    }
                }

                // 创建并返回公司仪表盘数据
                return new CompanyDashboardDto
                {
                    CompanyId = companyId,
                    CompanyName = company.Name,
                    TotalProjects = projectCount,
                    ActiveProjects = projects.Count(p => p.Status == "ACTIVE"),
                    CompletedProjects = projects.Count(p => p.Status == "COMPLETED"),
                    TotalWorkers = workerCount,
                    TodayAttendance = 0, // TODO: 需要从今日考勤记录中获取
                    AttendanceRate = 0, // TODO: 需要计算出勤率
                    TotalDevices = deviceCount,
                    OnlineDevices = 0, // TODO: 需要从设备状态中获取
                    UnresolvedIncidents = safetyIncidentCount,
                    MonthlyIncidents = safetyIncidentCount,
                    Projects = projectOverviews.Select(p => new ProjectSummaryDto
                    {
                        Id = p.ProjectId,
                        Name = p.ProjectName,
                        Status = p.Status,
                        WorkerCount = p.TotalWorkers,
                        DeviceCount = p.DeviceCount,
                        Progress = 0 // TODO: 需要计算项目进度
                    }).ToList(),
                    RecentAttendance = new List<AttendanceStatisticsDto>(),
                    DeviceStatistics = new DeviceStatusStatisticsDto(),
                    SafetyStatistics = new SafetyStatisticsDto()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取公司(ID:{companyId})仪表盘数据时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 将DeviceStatusStatisticsDto转换为DeviceStatisticsDto
        /// </summary>
        private DeviceStatisticsDto ConvertToDeviceStatistics(DeviceStatusStatisticsDto deviceStatus)
        {
            return new DeviceStatisticsDto
            {
                TotalDevices = deviceStatus.TotalCount,
                OnlineDevices = deviceStatus.OnlineCount,
                OfflineDevices = deviceStatus.OfflineCount,
                FaultDevices = deviceStatus.FaultCount,
                MaintenanceDevices = deviceStatus.MaintenanceCount,
                DevicesByType = deviceStatus.CountByType
            };
        }

        /// <summary>
        /// 映射安全统计数据
        /// </summary>
        private SafetyStatisticsDto MapSafetyStatistics(SafetyStatisticsDto safetyStats)
        {
            return new SafetyStatisticsDto
            {
                TotalIncidents = safetyStats.TotalIncidents,
                HandledIncidents = safetyStats.HandledIncidents,
                UnhandledIncidents = safetyStats.UnhandledIncidents,
                IncidentsByLevel = safetyStats.IncidentsByLevel,
                IncidentsByType = safetyStats.IncidentsByType,
                Last30DaysTrend = safetyStats.Last30DaysTrend
            };
        }

        /// <summary>
        /// 获取全局统计信息
        /// </summary>
        /// <returns>全局统计DTO</returns>
        public async Task<GlobalStatsDto> GetGlobalStatsAsync()
        {
            try
            {
                // 从正确的DbContext中查询
                var userCount = await _applicationDbContext.Users.CountAsync();
                var tenantCount = await _applicationDbContext.Tenants.CountAsync();
                var projectCount = await _unitOfWork.ProjectRepository.CountAsync();
                var deviceCount = await _unitOfWork.DeviceRepository.CountAsync();
                
                var version = System.Reflection.Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0";
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                return new GlobalStatsDto
                {
                    UserCount = userCount,
                    TenantCount = tenantCount,
                    ProjectCount = projectCount,
                    DeviceCount = deviceCount,
                    SystemVersion = version,
                    Environment = environment
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取全局统计信息时发生错误");
                throw;
            }
        }
    }
}
