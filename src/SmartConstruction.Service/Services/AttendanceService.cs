using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Attendance;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 考勤服务实现
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AttendanceService> _logger;

        public AttendanceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<AttendanceService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 获取考勤记录列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        public async Task<PagedResult<AttendanceDto>> GetAttendanceRecordsAsync(AttendanceQueryParams queryParams)
        {
            try
            {
                var repository = _unitOfWork.AttendanceRecordRepository;
                var query = repository.GetAll();

                // 应用过滤条件
                if (queryParams.WorkerId != null && queryParams.WorkerId != Guid.Empty)
                {
                    query = query.Where(a => a.WorkerId == queryParams.WorkerId);
                }

                if (!string.IsNullOrEmpty(queryParams.WorkerName))
                {
                    query = query.Where(a => a.Worker.DisplayName.Contains(queryParams.WorkerName));
                }

                if (queryParams.ProjectId != null && queryParams.ProjectId != Guid.Empty)
                {
                    query = query.Where(a => a.ProjectId == queryParams.ProjectId);
                }

                if (queryParams.TeamId != null && queryParams.TeamId != Guid.Empty)
                {
                    query = query.Where(a => a.TeamId == queryParams.TeamId);
                }

                if (queryParams.StartDate.HasValue)
                {
                    query = query.Where(a => a.AttendanceDate >= queryParams.StartDate.Value);
                }

                if (queryParams.EndDate.HasValue)
                {
                    query = query.Where(a => a.AttendanceDate <= queryParams.EndDate.Value);
                }

                if (!string.IsNullOrEmpty(queryParams.AttendanceType))
                {
                    query = query.Where(a => a.AttendanceType == queryParams.AttendanceType);
                }

                if (queryParams.IsSynced.HasValue)
                {
                    query = query.Where(a => a.IsSynced == queryParams.IsSynced.Value);
                }

                // 计算总记录数
                var totalCount = await repository.CountAsync(query);

                // 应用排序和分页
                var items = query
                    .OrderByDescending(a => a.AttendanceDate)
                    .ThenByDescending(a => a.ClockInTime)
                    .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToList();

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<AttendanceDto>>(items);
                return new PagedResult<AttendanceDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = queryParams.PageIndex,
                    PageSize = queryParams.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取考勤记录列表时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <returns>考勤记录DTO</returns>
        public async Task<AttendanceDto> GetAttendanceRecordByIdAsync(Guid id)
        {
            try
            {
                var attendanceRecord = await _unitOfWork.AttendanceRecordRepository.GetByIdAsync(id);
                if (attendanceRecord == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的考勤记录");
                }

                return _mapper.Map<AttendanceDto>(attendanceRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取考勤记录(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 创建考勤记录
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的考勤记录DTO</returns>
        public async Task<AttendanceDto> CreateAttendanceRecordAsync(CreateAttendanceRequest request)
        {
            try
            {
                // 检查工人是否存在
                var worker = await _unitOfWork.WorkerRepository.GetByIdAsync(request.WorkerId);
                if (worker == null)
                {
                    throw new InvalidOperationException($"未找到ID为{request.WorkerId}的工人");
                }

                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
                if (project == null)
                {
                    throw new InvalidOperationException($"未找到ID为{request.ProjectId}的项目");
                }

                // 检查班组是否存在
                if (request.TeamId != null && request.TeamId != Guid.Empty)
                {
                    var team = await _unitOfWork.TeamRepository.GetByIdAsync(request.TeamId);
                    if (team == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.TeamId}的班组");
                    }
                }

                var attendanceRecord = _mapper.Map<AttendanceRecord>(request);

                _unitOfWork.AttendanceRecordRepository.Create(attendanceRecord);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<AttendanceDto>(attendanceRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建考勤记录时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的考勤记录DTO</returns>
        public async Task<AttendanceDto> UpdateAttendanceRecordAsync(Guid id, UpdateAttendanceRequest request)
        {
            try
            {
                var attendanceRecord = await _unitOfWork.AttendanceRecordRepository.GetByIdAsync(id);
                if (attendanceRecord == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的考勤记录");
                }

                // 检查工人是否存在
                if (request.WorkerId != attendanceRecord.WorkerId)
                {
                    var worker = await _unitOfWork.WorkerRepository.GetByIdAsync(request.WorkerId);
                    if (worker == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.WorkerId}的工人");
                    }
                }

                // 检查项目是否存在
                if (request.ProjectId != attendanceRecord.ProjectId)
                {
                    var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
                    if (project == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.ProjectId}的项目");
                    }
                }

                // 检查班组是否存在
                if (request.TeamId != null && request.TeamId != Guid.Empty && request.TeamId != attendanceRecord.TeamId)
                {
                    var team = await _unitOfWork.TeamRepository.GetByIdAsync(request.TeamId);
                    if (team == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.TeamId}的班组");
                    }
                }

                _mapper.Map(request, attendanceRecord);

                _unitOfWork.AttendanceRecordRepository.Update(attendanceRecord);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<AttendanceDto>(attendanceRecord);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新考勤记录(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 删除考勤记录
        /// </summary>
        /// <param name="id">考勤记录ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteAttendanceRecordAsync(Guid id)
        {
            try
            {
                var attendanceRecord = await _unitOfWork.AttendanceRecordRepository.GetByIdAsync(id);
                if (attendanceRecord == null)
                {
                    return false;
                }

                _unitOfWork.AttendanceRecordRepository.Delete(attendanceRecord);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除考勤记录(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取工人的考勤记录
        /// </summary>
        /// <param name="workerId">工人ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        public async Task<PagedResult<AttendanceDto>> GetWorkerAttendanceAsync(Guid workerId, DateTime? startDate, DateTime? endDate, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // 检查工人是否存在
                var worker = await _unitOfWork.WorkerRepository.GetByIdAsync(workerId);
                if (worker == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{workerId}的工人");
                }

                var repository = _unitOfWork.AttendanceRecordRepository;
                var query = repository.GetByCondition(a => a.WorkerId == workerId);

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.AttendanceDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.AttendanceDate <= endDate.Value);
                }

                // 计算总记录数
                var totalCount = await repository.CountAsync(query);

                // 应用排序和分页
                var items = query
                    .OrderByDescending(a => a.AttendanceDate)
                    .ThenByDescending(a => a.ClockInTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<AttendanceDto>>(items);
                return new PagedResult<AttendanceDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = pageIndex,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取工人(ID:{workerId})的考勤记录时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取项目的考勤记录
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="date">日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>考勤记录列表</returns>
        public async Task<PagedResult<AttendanceDto>> GetProjectAttendanceAsync(Guid projectId, DateTime? date, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{projectId}的项目");
                }

                var repository = _unitOfWork.AttendanceRecordRepository;
                var query = repository.GetByCondition(a => a.ProjectId == projectId);

                if (date.HasValue)
                {
                    query = query.Where(a => a.AttendanceDate == date.Value.Date);
                }

                // 计算总记录数
                var totalCount = await repository.CountAsync(query);

                // 应用排序和分页
                var items = query
                    .OrderByDescending(a => a.AttendanceDate)
                    .ThenByDescending(a => a.ClockInTime)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<AttendanceDto>>(items);
                return new PagedResult<AttendanceDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = pageIndex,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})的考勤记录时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 获取项目的考勤统计
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="date">日期</param>
        /// <returns>考勤统计</returns>
        public async Task<AttendanceStatisticsDto> GetProjectAttendanceStatisticsAsync(Guid projectId, DateTime? date)
        {
            try
            {
                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{projectId}的项目");
                }

                // 如果未指定日期，则使用当前日期
                var targetDate = date?.Date ?? DateTime.Today;

                // 获取项目下的工人总数
                var totalWorkers = await _unitOfWork.WorkerRepository.CountAsync(w => w.ProjectId == projectId);

                // 获取当天签到的工人数
                var checkedInToday = await _unitOfWork.AttendanceRecordRepository.CountAsync(
                    a => a.ProjectId == projectId && a.AttendanceDate == targetDate && a.ClockInTime != null);

                // 获取当天签退的工人数
                var checkedOutToday = await _unitOfWork.AttendanceRecordRepository.CountAsync(
                    a => a.ProjectId == projectId && a.AttendanceDate == targetDate && a.ClockOutTime != null);

                // 获取当前在场的工人数
                var presentWorkers = await _unitOfWork.AttendanceRecordRepository.CountAsync(
                    a => a.ProjectId == projectId && a.AttendanceDate == targetDate && a.ClockInTime != null && a.ClockOutTime == null);

                // 创建并返回统计结果
                return new AttendanceStatisticsDto
                {
                    ProjectId = projectId,
                    ProjectName = project.ProjectName,
                    TotalWorkers = totalWorkers,
                    PresentWorkers = presentWorkers,
                    CheckedInToday = checkedInToday,
                    CheckedOutToday = checkedOutToday,
                    StatisticsDate = targetDate,
                    LastUpdate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})的考勤统计时发生错误");
                throw;
            }
        }
    }
} 