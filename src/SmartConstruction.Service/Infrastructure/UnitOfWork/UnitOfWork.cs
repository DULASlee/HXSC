using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.Repositories;
using SmartConstruction.Service.Data; // 添加 using

namespace SmartConstruction.Service.Infrastructure.UnitOfWork
{
    /// <summary>
    /// 工作单元实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposed;

        private IRepository<Company>? _companyRepository;
        private IRepository<Project>? _projectRepository;
        private IRepository<Team>? _teamRepository;
        private IRepository<Worker>? _workerRepository;
        private IRepository<WorkerAttendanceProfile>? _workerAttendanceProfileRepository;
        private IRepository<AttendanceRecord>? _attendanceRecordRepository;
        private IRepository<Device>? _deviceRepository;
        private IRepository<SafetyIncident>? _safetyIncidentRepository;
        private IRepository<GovernmentData>? _governmentDataRepository;
        private IRepository<Tenant>? _tenantRepository;
        private IRepository<AuditLog>? _auditLogRepository;
        private IRepository<SystemLog>? _systemLogRepository;
        private IRepository<DeviceMaintenanceRecord>? _deviceMaintenanceRecordRepository;
        private IRepository<SafetyTrainingRecord>? _safetyTrainingRecordRepository;
        private IRepository<LowCodeForm>? _lowCodeFormRepository;
        private IRepository<TeamProject>? _teamProjectRepository;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Dictionary<Type, object>();
        }

        /// <summary>
        /// 当前事务
        /// </summary>
        public IDbContextTransaction? CurrentTransaction { get; private set; }

        /// <summary>
        /// 获取仓储
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>仓储接口</returns>
        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new Repository<T>(_dbContext);
            }

            return (IRepository<T>)_repositories[type];
        }

        /// <summary>
        /// 获取自定义仓储
        /// </summary>
        /// <typeparam name="TRepository">仓储类型</typeparam>
        /// <returns>自定义仓储实例</returns>
        public TRepository GetCustomRepository<TRepository>() where TRepository : class
        {
            throw new NotImplementedException("自定义仓储获取功能暂未实现");
        }

        /// <summary>
        /// 公司仓储
        /// </summary>
        public IRepository<Company> CompanyRepository => _companyRepository ??= GetRepository<Company>();

        /// <summary>
        /// 项目仓储
        /// </summary>
        public IRepository<Project> ProjectRepository => _projectRepository ??= GetRepository<Project>();

        /// <summary>
        /// 班组仓储
        /// </summary>
        public IRepository<Team> TeamRepository => _teamRepository ??= GetRepository<Team>();

        /// <summary>
        /// 工人仓储
        /// </summary>
        public IRepository<Worker> WorkerRepository => _workerRepository ??= GetRepository<Worker>();

        /// <summary>
        /// 工人实名制考勤资料仓储
        /// </summary>
        public IRepository<WorkerAttendanceProfile> WorkerAttendanceProfileRepository => _workerAttendanceProfileRepository ??= GetRepository<WorkerAttendanceProfile>();

        /// <summary>
        /// 考勤记录仓储
        /// </summary>
        public IRepository<AttendanceRecord> AttendanceRecordRepository => _attendanceRecordRepository ??= GetRepository<AttendanceRecord>();

        /// <summary>
        /// 设备仓储
        /// </summary>
        public IRepository<Device> DeviceRepository => _deviceRepository ??= GetRepository<Device>();

        /// <summary>
        /// 安全事故仓储
        /// </summary>
        public IRepository<SafetyIncident> SafetyIncidentRepository => _safetyIncidentRepository ??= GetRepository<SafetyIncident>();

        /// <summary>
        /// 政府监管数据仓储
        /// </summary>
        public IRepository<GovernmentData> GovernmentDataRepository => _governmentDataRepository ??= GetRepository<GovernmentData>();

        /// <summary>
        /// 租户仓储
        /// </summary>
        public IRepository<Tenant> TenantRepository => _tenantRepository ??= GetRepository<Tenant>();

        /// <summary>
        /// 审计日志仓储
        /// </summary>
        public IRepository<AuditLog> AuditLogRepository => _auditLogRepository ??= GetRepository<AuditLog>();

        /// <summary>
        /// 系统日志仓储
        /// </summary>
        public IRepository<SystemLog> SystemLogRepository => _systemLogRepository ??= GetRepository<SystemLog>();

        /// <summary>
        /// 设备维护记录仓储
        /// </summary>
        public IRepository<DeviceMaintenanceRecord> DeviceMaintenanceRecordRepository => _deviceMaintenanceRecordRepository ??= GetRepository<DeviceMaintenanceRecord>();

        /// <summary>
        /// 安全培训记录仓储
        /// </summary>
        public IRepository<SafetyTrainingRecord> SafetyTrainingRecordRepository => _safetyTrainingRecordRepository ??= GetRepository<SafetyTrainingRecord>();

        /// <summary>
        /// 低代码表单仓储
        /// </summary>
        public IRepository<LowCodeForm> LowCodeFormRepository => _lowCodeFormRepository ??= GetRepository<LowCodeForm>();

        /// <summary>
        /// 班组项目关联仓储
        /// </summary>
        public IRepository<TeamProject> TeamProjectRepository => _teamProjectRepository ??= GetRepository<TeamProject>();

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 同步保存更改
        /// </summary>
        /// <returns>受影响的行数</returns>
        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }

        /// <summary>
        /// 执行原始SQL命令
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>受影响的行数</returns>
        public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
        {
            return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <summary>
        /// 执行原始SQL查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>查询结果</returns>
        public async Task<List<T>> ExecuteSqlQueryAsync<T>(string sql, params object[] parameters) where T : class
        {
            return await _dbContext.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }

        /// <summary>
        /// 检查是否有更改
        /// </summary>
        public bool HasChanges()
        {
            return _dbContext.ChangeTracker.HasChanges();
        }

        /// <summary>
        /// 清空更改跟踪器
        /// </summary>
        public void ClearChangeTracker()
        {
            _dbContext.ChangeTracker.Clear();
        }

        /// <summary>
        /// 设置命令超时时间
        /// </summary>
        /// <param name="seconds">超时秒数</param>
        public void SetCommandTimeout(int seconds)
        {
            _dbContext.Database.SetCommandTimeout(seconds);
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>数据库事务</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            CurrentTransaction = await _dbContext.Database.BeginTransactionAsync();
            return CurrentTransaction;
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>数据库事务</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            CurrentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            return CurrentTransaction;
        }

        /// <summary>
        /// 开始事务（指定隔离级别）
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别</param>
        /// <returns>数据库事务</returns>
        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel)
        {
            CurrentTransaction = await _dbContext.Database.BeginTransactionAsync(isolationLevel);
            return CurrentTransaction;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public async Task CommitAsync()
        {
            if (CurrentTransaction != null)
            {
                await CurrentTransaction.CommitAsync();
                await CurrentTransaction.DisposeAsync();
                CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public async Task RollbackAsync()
        {
            if (CurrentTransaction != null)
            {
                await CurrentTransaction.RollbackAsync();
                await CurrentTransaction.DisposeAsync();
                CurrentTransaction = null;
            }
        }

        /// <summary>
        /// 尝试保存更改
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>保存是否成功和错误信息</returns>
        public async Task<(bool Success, string Error)> TrySaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
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
        /// <param name="disposing">是否释放托管资源</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CurrentTransaction?.Dispose();
                    _dbContext.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
