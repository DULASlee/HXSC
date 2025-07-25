using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartConstruction.Contracts.Dtos.Worker;
using SmartConstruction.Contracts.Dtos.Base;

namespace SmartConstruction.Service.Services
{
    public interface IWorkerService
    {
        Task<WorkerDto> GetByIdAsync(Guid id);
        Task<IEnumerable<WorkerDto>> GetListAsync(WorkerQueryDto query);
        Task<PagedResult<WorkerDto>> GetPagedListAsync(WorkerQueryDto query, int pageIndex, int pageSize);
        Task<WorkerDto> CreateAsync(CreateWorkerRequest dto);
        Task<WorkerDto> UpdateAsync(Guid id, UpdateWorkerRequest dto);
        Task<bool> DeleteAsync(Guid id);
    }
}