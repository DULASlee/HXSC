using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Entities;

namespace SmartConstruction.Service.Services.Base
{
    /// <summary>
    /// 基础服务接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDto">DTO类型</typeparam>
    public interface IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>DTO集合</returns>
        Task<IEnumerable<TDto>> GetAllAsync();

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>DTO对象</returns>
        Task<TDto?> GetByIdAsync(Guid id);

        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>DTO集合</returns>
        Task<IEnumerable<TDto>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="dto">DTO对象</param>
        /// <returns>创建后的DTO对象</returns>
        Task<TDto> CreateAsync(TDto dto);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="dto">DTO对象</param>
        /// <returns>更新后的DTO对象</returns>
        Task<TDto> UpdateAsync(TDto dto);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序</param>
        /// <returns>分页结果</returns>
        Task<PagedResult<TDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        /// <summary>
        /// 检查实体是否存在
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// 获取实体数量
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>实体数量</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
    }

    /// <summary>
    /// 基础服务接口（带创建和更新DTO）
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDto">DTO类型</typeparam>
    /// <typeparam name="TCreateDto">创建DTO类型</typeparam>
    /// <typeparam name="TUpdateDto">更新DTO类型</typeparam>
    public interface IBaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="createDto">创建DTO对象</param>
        /// <returns>创建后的DTO对象</returns>
        Task<TDto> CreateAsync(TCreateDto createDto);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="updateDto">更新DTO对象</param>
        /// <returns>更新后的DTO对象</returns>
        Task<TDto> UpdateAsync(Guid id, TUpdateDto updateDto);
    }
} 