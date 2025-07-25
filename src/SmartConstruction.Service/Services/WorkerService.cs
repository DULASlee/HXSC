using AutoMapper;
using SmartConstruction.Contracts.Dtos.Worker;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    public class WorkerService : IWorkerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public WorkerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<WorkerDto> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.WorkerRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<WorkerDto>(entity);
        }

        public async Task<IEnumerable<WorkerDto>> GetListAsync(WorkerQueryDto query)
        {
            var q = _unitOfWork.WorkerRepository.GetAll();
            if (query.TenantId.HasValue) q = q.Where(x => x.TenantId == query.TenantId);
            if (query.TeamId.HasValue) q = q.Where(x => x.TeamId == query.TeamId);
            if (query.OrganizationId.HasValue) q = q.Where(x => x.OrganizationId == query.OrganizationId);
            if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status);
            if (!string.IsNullOrWhiteSpace(query.Keyword))
                q = q.Where(x => x.FullName.Contains(query.Keyword) || x.DisplayName.Contains(query.Keyword) || x.Mobile.Contains(query.Keyword));
            var list = await _unitOfWork.WorkerRepository.GetListAsync(q);
            return _mapper.Map<List<WorkerDto>>(list);
        }

        public async Task<PagedResult<WorkerDto>> GetPagedListAsync(WorkerQueryDto query, int pageIndex, int pageSize)
        {
            var q = _unitOfWork.WorkerRepository.GetAll();
            if (query.TenantId.HasValue) q = q.Where(x => x.TenantId == query.TenantId);
            if (query.TeamId.HasValue) q = q.Where(x => x.TeamId == query.TeamId);
            if (query.OrganizationId.HasValue) q = q.Where(x => x.OrganizationId == query.OrganizationId);
            if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status);
            if (!string.IsNullOrWhiteSpace(query.Keyword))
                q = q.Where(x => x.FullName.Contains(query.Keyword) || x.DisplayName.Contains(query.Keyword) || x.Mobile.Contains(query.Keyword));
            var total = await _unitOfWork.WorkerRepository.CountAsync(q);
            var items = await _unitOfWork.WorkerRepository.GetPagedAsync(q, pageIndex, pageSize);
            return new PagedResult<WorkerDto>
            {
                Items = _mapper.Map<List<WorkerDto>>(items),
                Total = total,
                Page = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<WorkerDto> CreateAsync(CreateWorkerRequest dto)
        {
            var entity = _mapper.Map<Worker>(dto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
            var added = await _unitOfWork.WorkerRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WorkerDto>(added);
        }

        public async Task<WorkerDto> UpdateAsync(Guid id, UpdateWorkerRequest dto)
        {
            var entity = await _unitOfWork.WorkerRepository.GetByIdAsync(id);
            if (entity == null) throw new Exception("工人不存在");
            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.WorkerRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WorkerDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.WorkerRepository.GetByIdAsync(id);
            if (entity == null) return false;
            entity.Status = 0; // 软删除
            await _unitOfWork.WorkerRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
