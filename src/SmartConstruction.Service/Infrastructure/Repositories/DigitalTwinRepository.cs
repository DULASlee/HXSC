using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Data;

namespace SmartConstruction.Service.Infrastructure.Repositories
{
    /// <summary>
    /// 数字孪生数据仓储实现
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public class DigitalTwinRepository<T> : Repository<T>, IDigitalTwinRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbContext">IoT数据库上下文</param>
        public DigitalTwinRepository(SmartConstructionDbContext dbContext) : base(dbContext)
        {
            // 这里的 base(dbContext) 会将 SmartConstructionDbContext 传递给基类 Repository<T>
            // 这意味着我们需要调整基类 Repository<T> 的构造函数来接受通用的 DbContext
        }
    }
} 