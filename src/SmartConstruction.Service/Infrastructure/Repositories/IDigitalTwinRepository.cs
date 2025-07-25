using SmartConstruction.Contracts.Entities;

namespace SmartConstruction.Service.Infrastructure.Repositories
{
    /// <summary>
    /// 数字孪生数据仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IDigitalTwinRepository<T> : IRepository<T> where T : BaseEntity
    {
        // 目前不需要额外的特定方法，但通过创建独立接口
        // 我们可以为数字孪生仓储进行特定的依赖注入注册
    }
} 