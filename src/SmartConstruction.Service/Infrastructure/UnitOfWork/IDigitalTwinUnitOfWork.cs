using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.Repositories;
using System.Threading.Tasks;
using System;

namespace SmartConstruction.Service.Infrastructure.UnitOfWork
{
    /// <summary>
    /// 数字孪生工作单元接口
    /// </summary>
    public interface IDigitalTwinUnitOfWork : IDisposable
    {
        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>仓储接口</returns>
        IDigitalTwinRepository<T> GetRepository<T>() where T : BaseEntity;

        /// <summary>
        /// 设备仓储
        /// </summary>
        IDigitalTwinRepository<Device> DeviceRepository { get; }
        
        /// <summary>
        /// 安全事故仓储
        /// </summary>
        IDigitalTwinRepository<SafetyIncident> SafetyIncidentRepository { get; }

        // 根据需要添加其他 IoT 相关的仓储...

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync();
    }
} 