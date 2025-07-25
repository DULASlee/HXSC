using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.Repositories;

namespace SmartConstruction.Service.Infrastructure.UnitOfWork
{
    /// <summary>
    /// 工作单元接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region 事务管理

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns>数据库事务</returns>
        Task<IDbContextTransaction> BeginTransactionAsync();

        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="isolationLevel">事务隔离级别</param>
        /// <returns>数据库事务</returns>
        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel);

        /// <summary>
        /// 提交事务
        /// </summary>
        Task CommitAsync();

        /// <summary>
        /// 回滚事务
        /// </summary>
        Task RollbackAsync();

        /// <summary>
        /// 获取当前事务
        /// </summary>
        IDbContextTransaction? CurrentTransaction { get; }

        #endregion

        #region 仓储获取

        /// <summary>
        /// 获取泛型仓储
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>仓储接口</returns>
        IRepository<T> GetRepository<T>() where T : BaseEntity;

        /// <summary>
        /// 获取自定义仓储
        /// </summary>
        /// <typeparam name="TRepository">仓储接口类型</typeparam>
        /// <returns>自定义仓储接口</returns>
        TRepository GetCustomRepository<TRepository>() where TRepository : class;

        #endregion

        #region 具体仓储属性

        /// <summary>
        /// 租户仓储
        /// </summary>
        IRepository<Tenant> TenantRepository { get; }

        

        /// <summary>
        /// 公司仓储
        /// </summary>
        IRepository<Company> CompanyRepository { get; }

        /// <summary>
        /// 项目仓储
        /// </summary>
        IRepository<Project> ProjectRepository { get; }

        /// <summary>
        /// 班组仓储
        /// </summary>
        IRepository<Team> TeamRepository { get; }

        /// <summary>
        /// 工人仓储
        /// </summary>
        IRepository<Worker> WorkerRepository { get; }

        /// <summary>
        /// 工人实名制考勤资料仓储
        /// </summary>
        IRepository<WorkerAttendanceProfile> WorkerAttendanceProfileRepository { get; }

        /// <summary>
        /// 考勤记录仓储
        /// </summary>
        IRepository<AttendanceRecord> AttendanceRecordRepository { get; }

        /// <summary>
        /// 设备仓储
        /// </summary>
        IRepository<Device> DeviceRepository { get; }

        /// <summary>
        /// 设备维护记录仓储
        /// </summary>
        IRepository<DeviceMaintenanceRecord> DeviceMaintenanceRecordRepository { get; }

        /// <summary>
        /// 安全事故仓储
        /// </summary>
        IRepository<SafetyIncident> SafetyIncidentRepository { get; }

        /// <summary>
        /// 安全培训记录仓储
        /// </summary>
        IRepository<SafetyTrainingRecord> SafetyTrainingRecordRepository { get; }

        /// <summary>
        /// 政府监管数据仓储
        /// </summary>
        IRepository<GovernmentData> GovernmentDataRepository { get; }

        /// <summary>
        /// 低代码表单仓储
        /// </summary>
        IRepository<LowCodeForm> LowCodeFormRepository { get; }

        /// <summary>
        /// 系统日志仓储
        /// </summary>
        IRepository<SystemLog> SystemLogRepository { get; }

        /// <summary>
        /// 审计日志仓储
        /// </summary>
        IRepository<AuditLog> AuditLogRepository { get; }
        
        /// <summary>
        /// 班组项目关联仓储
        /// </summary>
        IRepository<TeamProject> TeamProjectRepository { get; }

        #endregion

        #region 保存更改

        /// <summary>
        /// 保存更改（异步）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>受影响的行数</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 保存更改（同步）
        /// </summary>
        /// <returns>受影响的行数</returns>
        int SaveChanges();

        /// <summary>
        /// 尝试保存更改（异步）
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>保存结果</returns>
        Task<(bool Success, string Error)> TrySaveChangesAsync(CancellationToken cancellationToken = default);

        #endregion

        #region 数据库操作

        /// <summary>
        /// 执行原始SQL命令
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>受影响的行数</returns>
        Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters);

        /// <summary>
        /// 执行原始SQL查询
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">SQL语句</param>
        /// <param name="parameters">参数</param>
        /// <returns>查询结果</returns>
        Task<List<T>> ExecuteSqlQueryAsync<T>(string sql, params object[] parameters) where T : class;

        #endregion

        #region 辅助方法

        /// <summary>
        /// 检查是否有未保存的更改
        /// </summary>
        /// <returns>是否有未保存的更改</returns>
        bool HasChanges();

        /// <summary>
        /// 清除更改跟踪
        /// </summary>
        void ClearChangeTracker();

        /// <summary>
        /// 设置命令超时时间
        /// </summary>
        /// <param name="seconds">超时秒数</param>
        void SetCommandTimeout(int seconds);

        #endregion
    }
}
