using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;

namespace SmartConstruction.Service.Services.Base
{
    /// <summary>
    /// 基础服务类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDto">DTO类型</typeparam>
    public abstract class BaseService<TEntity, TDto> : IBaseService<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;

        public BaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>DTO集合</returns>
        public virtual Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = _unitOfWork.GetRepository<TEntity>().GetAll();
            return Task.FromResult(_mapper.Map<IEnumerable<TDto>>(entities));
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>DTO对象</returns>
        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<TEntity>().GetByIdAsync(id);
            return entity == null ? default : _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// 根据条件获取实体
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>DTO集合</returns>
        public virtual Task<IEnumerable<TDto>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = _unitOfWork.GetRepository<TEntity>().GetByCondition(predicate);
            return Task.FromResult(_mapper.Map<IEnumerable<TDto>>(entities));
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="dto">DTO对象</param>
        /// <returns>创建后的DTO对象</returns>
        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _unitOfWork.GetRepository<TEntity>().Create(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="dto">DTO对象</param>
        /// <returns>更新后的DTO对象</returns>
        public virtual async Task<TDto> UpdateAsync(TDto dto)
        {
            var entity = await _unitOfWork.GetRepository<TEntity>().GetByIdAsync(dto.Id);
            if (entity == null)
            {
                throw new Exception($"Entity with id {dto.Id} not found.");
            }

            _mapper.Map(dto, entity);
            _unitOfWork.GetRepository<TEntity>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>是否成功</returns>
        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var success = await _unitOfWork.GetRepository<TEntity>().DeleteByIdAsync(id);
            if (success)
            {
                await _unitOfWork.SaveChangesAsync();
            }
            return success;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="orderBy">排序字段</param>
        /// <returns>分页结果</returns>
        public virtual async Task<PagedResult<TDto>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var repository = _unitOfWork.GetRepository<TEntity>();
            var query = predicate == null ? repository.GetAll() : repository.GetByCondition(predicate);

            // 计算总记录数
            var totalCount = await repository.CountAsync(predicate);

            // 应用排序
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                // 默认按创建时间降序排序
                query = query.OrderByDescending(e => e.CreatedAt);
            }

            // 应用分页
            var items = query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 映射为DTO
            var dtoItems = _mapper.Map<List<TDto>>(items);

            // 构造分页结果
            return new PagedResult<TDto>
            {
                Items = dtoItems,
                Total = totalCount,
                Page = pageIndex,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// 检查是否存在
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>是否存在</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _unitOfWork.GetRepository<TEntity>().ExistsAsync(predicate);
        }

        /// <summary>
        /// 获取记录数
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>记录数</returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            // 修复：调用仓储层的异步CountAsync方法
            return await _unitOfWork.GetRepository<TEntity>().CountAsync(predicate);
        }
    }

    /// <summary>
    /// 支持创建和更新DTO的基础服务类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TDto">DTO类型</typeparam>
    /// <typeparam name="TCreateDto">创建DTO类型</typeparam>
    /// <typeparam name="TUpdateDto">更新DTO类型</typeparam>
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : BaseService<TEntity, TDto>, IBaseService<TEntity, TDto, TCreateDto, TUpdateDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        public BaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
            : base(unitOfWork, mapper, logger)
        {
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="createDto">创建DTO</param>
        /// <returns>创建后的DTO</returns>
        public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
        {
            var entity = _mapper.Map<TEntity>(createDto);
            _unitOfWork.GetRepository<TEntity>().Create(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <param name="updateDto">更新DTO</param>
        /// <returns>更新后的DTO</returns>
        public virtual async Task<TDto> UpdateAsync(Guid id, TUpdateDto updateDto)
        {
            var entity = await _unitOfWork.GetRepository<TEntity>().GetByIdAsync(id);
            if (entity == null)
            {
                throw new Exception($"Entity with id {id} not found.");
            }

            _mapper.Map(updateDto, entity);
            _unitOfWork.GetRepository<TEntity>().Update(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TDto>(entity);
        }
    }
}
