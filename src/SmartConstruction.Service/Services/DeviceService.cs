using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Device;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 设备服务实现
    /// </summary>
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<DeviceService> _logger;

        public DeviceService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<DeviceService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        public async Task<PagedResult<DeviceDto>> GetDevicesAsync(DeviceQueryParams queryParams)
        {
            try
            {
                var repository = _unitOfWork.DeviceRepository;
                var query = repository.GetAll();

                // 应用过滤条件
                if (!string.IsNullOrEmpty(queryParams.DeviceCode))
                {
                    query = query.Where(d => d.DeviceCode.Contains(queryParams.DeviceCode));
                }

                if (!string.IsNullOrEmpty(queryParams.DeviceName))
                {
                    query = query.Where(d => d.DeviceName.Contains(queryParams.DeviceName));
                }

                if (queryParams.ProjectId.HasValue)
                {
                    query = query.Where(d => d.ProjectId == queryParams.ProjectId.Value);
                }

                if (!string.IsNullOrEmpty(queryParams.DeviceType))
                {
                    query = query.Where(d => d.DeviceType == queryParams.DeviceType);
                }

                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(d => d.Status == queryParams.Status);
                }

                // 计算总记录数
                var totalCount = await repository.CountAsync(d => query.Any(q => q.Id == d.Id));

                // 应用排序和分页
                var items = query
                    .OrderByDescending(d => d.CreatedAt)
                    .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToList();

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<DeviceDto>>(items);
                return new PagedResult<DeviceDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = queryParams.PageIndex,
                    PageSize = queryParams.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取设备列表时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 根据ID获取设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>设备DTO</returns>
        public async Task<DeviceDto> GetDeviceByIdAsync(Guid id)
        {
            try
            {
                var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
                if (device == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的设备");
                }

                return _mapper.Map<DeviceDto>(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取设备(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 根据设备编号获取设备
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <returns>设备DTO</returns>
        public async Task<DeviceDto> GetDeviceByCodeAsync(string deviceCode)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceCode))
                {
                    throw new ArgumentException("设备编号不能为空", nameof(deviceCode));
                }

                var devices = _unitOfWork.DeviceRepository.GetByCondition(d => d.DeviceCode == deviceCode);
                var device = await Task.FromResult(devices.FirstOrDefault());

                if (device == null)
                {
                    throw new KeyNotFoundException($"未找到编号为{deviceCode}的设备");
                }

                return _mapper.Map<DeviceDto>(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取设备(编号:{deviceCode})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的设备DTO</returns>
        public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request)
        {
            try
            {
                // 检查设备编号是否已存在
                if (await IsDeviceCodeExistsAsync(request.DeviceCode))
                {
                    throw new InvalidOperationException($"设备编号'{request.DeviceCode}'已存在");
                }

                // 检查项目是否存在
                if (request.ProjectId != Guid.Empty)
                {
                    var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
                    if (project == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.ProjectId}的项目");
                    }
                }

                var device = _mapper.Map<Device>(request);
                device.Status = "NORMAL"; // 默认状态为正常

                _unitOfWork.DeviceRepository.Create(device);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<DeviceDto>(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建设备时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的设备DTO</returns>
        public async Task<DeviceDto> UpdateDeviceAsync(Guid id, UpdateDeviceRequest request)
        {
            try
            {
                var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
                if (device == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的设备");
                }

                // 检查设备编号是否已存在
                if (request.DeviceCode != device.DeviceCode &&
                    await IsDeviceCodeExistsAsync(request.DeviceCode, id))
                {
                    throw new InvalidOperationException($"设备编号'{request.DeviceCode}'已存在");
                }

                // 检查项目是否存在
                if (request.ProjectId != Guid.Empty && request.ProjectId != device.ProjectId)
                {
                    var project = await _unitOfWork.ProjectRepository.GetByIdAsync(request.ProjectId);
                    if (project == null)
                    {
                        throw new InvalidOperationException($"未找到ID为{request.ProjectId}的项目");
                    }
                }

                _mapper.Map(request, device);
                device.UpdateTime = DateTime.Now;

                _unitOfWork.DeviceRepository.Update(device);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<DeviceDto>(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新设备(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>是否成功</returns>
        public async Task<bool> DeleteDeviceAsync(Guid id)
        {
            try
            {
                var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
                if (device == null)
                {
                    return false;
                }

                _unitOfWork.DeviceRepository.Delete(device);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除设备(ID:{id})时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 检查设备编号是否已存在
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="excludeId">排除的设备ID</param>
        /// <returns>是否存在</returns>
        public async Task<bool> IsDeviceCodeExistsAsync(string deviceCode, Guid? excludeId = null)
        {
            if (string.IsNullOrEmpty(deviceCode))
            {
                return false;
            }

            var query = _unitOfWork.DeviceRepository.GetByCondition(d => d.DeviceCode == deviceCode);
            if (excludeId.HasValue)
            {
                query = query.Where(d => d.Id != excludeId.Value);
            }

            return await _unitOfWork.DeviceRepository.ExistsAsync(d => d.DeviceCode == deviceCode &&
                (!excludeId.HasValue || d.Id != excludeId.Value));
        }

        /// <summary>
        /// 获取项目下的设备列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>设备列表</returns>
        public async Task<PagedResult<DeviceDto>> GetDevicesByProjectAsync(Guid projectId, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                // 检查项目是否存在
                var project = await _unitOfWork.ProjectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{projectId}的项目");
                }

                var repository = _unitOfWork.DeviceRepository;
                var query = repository.GetByCondition(d => d.ProjectId == projectId);

                // 计算总记录数
                var totalCount = await repository.CountAsync(d => d.ProjectId == projectId);

                // 应用排序和分页
                var devices = await _unitOfWork.DeviceRepository.GetPagedAsync(
                    query.OrderByDescending(d => d.CreatedAt),
                    pageIndex,
                    pageSize);

                // 映射为DTO并返回
                var dtoItems = _mapper.Map<List<DeviceDto>>(devices);
                return new PagedResult<DeviceDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = pageIndex,
                    PageSize = pageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取项目(ID:{projectId})的设备列表时发生错误");
                throw;
            }
        }

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="status">状态</param>
        /// <returns>更新后的设备DTO</returns>
        public async Task<DeviceDto> UpdateDeviceStatusAsync(Guid id, string status)
        {
            try
            {
                var device = await _unitOfWork.DeviceRepository.GetByIdAsync(id);
                if (device == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的设备");
                }

                device.Status = status;
                device.UpdateTime = DateTime.Now;

                _unitOfWork.DeviceRepository.Update(device);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<DeviceDto>(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新设备(ID:{id})状态时发生错误");
                throw;
            }
        }
    }
}
