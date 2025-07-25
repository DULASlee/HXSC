using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Device;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 设备服务接口
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        Task<PagedResult<DeviceDto>> GetDevicesAsync(DeviceQueryParams queryParams);

        /// <summary>
        /// 根据ID获取设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>设备DTO</returns>
        Task<DeviceDto> GetDeviceByIdAsync(Guid id);

        /// <summary>
        /// 根据设备编号获取设备
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <returns>设备DTO</returns>
        Task<DeviceDto> GetDeviceByCodeAsync(string deviceCode);

        /// <summary>
        /// 创建设备
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的设备DTO</returns>
        Task<DeviceDto> CreateDeviceAsync(CreateDeviceRequest request);

        /// <summary>
        /// 更新设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的设备DTO</returns>
        Task<DeviceDto> UpdateDeviceAsync(Guid id, UpdateDeviceRequest request);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteDeviceAsync(Guid id);

        /// <summary>
        /// 检查设备编号是否已存在
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="excludeId">排除的设备ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsDeviceCodeExistsAsync(string deviceCode, Guid? excludeId = null);

        /// <summary>
        /// 获取项目下的设备列表
        /// </summary>
        /// <param name="projectId">项目ID</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>设备列表</returns>
        Task<PagedResult<DeviceDto>> GetDevicesByProjectAsync(Guid projectId, int pageIndex = 1, int pageSize = 10);

        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="id">设备ID</param>
        /// <param name="status">状态</param>
        /// <returns>更新后的设备DTO</returns>
        Task<DeviceDto> UpdateDeviceStatusAsync(Guid id, string status);
    }
}
