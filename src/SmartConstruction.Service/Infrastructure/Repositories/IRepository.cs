using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartConstruction.Contracts.Entities;

namespace SmartConstruction.Service.Infrastructure.Repositories
{
    /// <summary>
    /// 通用仓储接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IRepository<T> where T : BaseEntity
    {
        #region 查询方法

        /// <summary>
        /// 获取查询对象（解决您的问题）
        /// </summary>
        /// <returns>可查询对象</returns>
        IQueryable<T> GetQueryable();

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体集合</returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体集合</returns>
        IQueryable<T> GetByCondition(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取包含导航属性的查询
        /// </summary>
        /// <param name="includeProperties">包含的导航属性</param>
        /// <returns>实体集合</returns>
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体对象</returns>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// 根据ID获取实体（包含导航属性）
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="includeProperties">包含的导航属性</param>
        /// <returns>实体对象</returns>
        Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includeProperties);

        /// <summary>
        /// 获取第一个满足条件的实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体对象</returns>
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取单个满足条件的实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体对象</returns>
        Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region 列表和分页

        /// <summary>
        /// 获取满足条件的实体列表
        /// </summary>
        /// <param name="query">查询</param>
        /// <returns>实体列表</returns>
        Task<List<T>> GetListAsync(IQueryable<T> query);

        /// <summary>
        /// 获取满足条件的实体列表
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体列表</returns>
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 分页获取实体列表
        /// </summary>
        /// <param name="query">查询</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页大小</param>
        /// <returns>实体列表</returns>
        Task<List<T>> GetPagedAsync(IQueryable<T> query, int pageIndex, int pageSize);

        #endregion

        #region 统计方法

        /// <summary>
        /// 检查是否存在满足条件的实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 获取满足条件的实体数量
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体数量</returns>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
        
        /// <summary>
        /// 获取查询结果的实体数量
        /// </summary>
        /// <param name="query">查询</param>
        /// <returns>实体数量</returns>
        Task<int> CountAsync(IQueryable<T> query);

        #endregion

        #region 异步增删改方法（推荐使用）

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>添加的实体</returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>添加的实体集合</returns>
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>更新的实体</returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>更新的实体集合</returns>
        Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="entities">实体集合</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteByIdAsync(Guid id);

        #endregion

        #region 同步方法（如果需要保留）

        /// <summary>
        /// 创建实体（建议使用AddAsync）
        /// </summary>
        /// <param name="entity">实体对象</param>
        void Create(T entity);

        /// <summary>
        /// 批量创建实体（建议使用AddRangeAsync）
        /// </summary>
        /// <param name="entities">实体集合</param>
        void CreateRange(IEnumerable<T> entities);

        /// <summary>
        /// 更新实体（建议使用UpdateAsync）
        /// </summary>
        /// <param name="entity">实体对象</param>
        void Update(T entity);

        /// <summary>
        /// 批量更新实体（建议使用UpdateRangeAsync）
        /// </summary>
        /// <param name="entities">实体集合</param>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// 删除实体（建议使用DeleteAsync）
        /// </summary>
        /// <param name="entity">实体对象</param>
        void Delete(T entity);

        /// <summary>
        /// 批量删除实体（建议使用DeleteRangeAsync）
        /// </summary>
        /// <param name="entities">实体集合</param>
        void DeleteRange(IEnumerable<T> entities);

        #endregion
    }
}
