using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;
using SmartConstruction.Service.Infrastructure.Repositories;

namespace SmartConstruction.Service.Infrastructure.UnitOfWork
{
    /// <summary>
    /// 数字孪生工作单元实现
    /// </summary>
    public class DigitalTwinUnitOfWork : IDigitalTwinUnitOfWork
    {
        private readonly SmartConstructionDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;

        public DigitalTwinUnitOfWork(SmartConstructionDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// 获取仓储
        /// </summary>
        public IDigitalTwinRepository<T> GetRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new DigitalTwinRepository<T>(_dbContext);
            }
            return (IDigitalTwinRepository<T>)_repositories[type];
        }

        /// <summary>
        /// 设备仓储
        /// </summary>
        public IDigitalTwinRepository<Device> DeviceRepository => GetRepository<Device>();
        
        /// <summary>
        /// 安全事故仓储
        /// </summary>
        public IDigitalTwinRepository<SafetyIncident> SafetyIncidentRepository => GetRepository<SafetyIncident>();

        /// <summary>
        /// 保存更改
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
                _disposed = true;
            }
        }
    }
} 