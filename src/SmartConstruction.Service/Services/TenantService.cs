using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Tenant;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;

namespace SmartConstruction.Service.Services;

public interface ITenantService
{
    Task<TenantDto?> GetByIdAsync(Guid id);
    Task<PagedResult<TenantDto>> GetPagedListAsync(TenantListRequest request);
    Task<TenantDto> CreateAsync(CreateTenantDto createDto);
    Task<TenantDto> UpdateAsync(Guid id, UpdateTenantDto updateDto);
    Task DeleteAsync(Guid id);
    Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null);
}

public class TenantService : ITenantService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<TenantService> _logger;

    public TenantService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TenantService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TenantDto?> GetByIdAsync(Guid id)
    {
        var entity = await _unitOfWork.GetRepository<Tenant>().GetByIdAsync(id);
        return _mapper.Map<TenantDto>(entity);
    }

    public async Task<PagedResult<TenantDto>> GetPagedListAsync(TenantListRequest request)
    {
        var query = _unitOfWork.GetRepository<Tenant>().GetAll();

        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            query = query.Where(x => x.Name.Contains(request.Keyword) || x.Code.Contains(request.Keyword));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        var result = _mapper.Map<List<TenantDto>>(items);

        return new PagedResult<TenantDto>
        {
            Items = result,
            Total = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    public async Task<TenantDto> CreateAsync(CreateTenantDto createDto)
    {
        if (await IsCodeExistsAsync(createDto.Code))
        {
            throw new Exception($"租户编码 {createDto.Code} 已存在");
        }

        var entity = _mapper.Map<Tenant>(createDto);
        _unitOfWork.GetRepository<Tenant>().Create(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TenantDto>(entity);
    }

    public async Task<TenantDto> UpdateAsync(Guid id, UpdateTenantDto updateDto)
    {
        var repository = _unitOfWork.GetRepository<Tenant>();
        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception($"租户 {id} 不存在");
        }

        // TODO: 如需校验租户编码唯一性，请在此处实现
        _mapper.Map(updateDto, entity);
        repository.Update(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TenantDto>(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var repository = _unitOfWork.GetRepository<Tenant>();
        var entity = await repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new Exception($"租户 {id} 不存在");
        }

        repository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> IsCodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _unitOfWork.GetRepository<Tenant>().GetAll().Where(x => x.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}
