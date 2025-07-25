using AutoMapper;
using SmartConstruction.Contracts.Dtos.Team;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public TeamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResult<TeamDto>> GetPagedListAsync(Guid? projectId = null, string? searchKeyword = null, int pageIndex = 1, int pageSize = 10)
        {
            var q = _unitOfWork.TeamRepository.GetAll();
            if (projectId.HasValue) q = q.Where(x => x.ProjectId == projectId);
            if (!string.IsNullOrWhiteSpace(searchKeyword))
                q = q.Where(x => x.Name.Contains(searchKeyword) || x.Specialty.Contains(searchKeyword));
            var total = await _unitOfWork.TeamRepository.CountAsync(q);
            var items = await _unitOfWork.TeamRepository.GetPagedAsync(q, pageIndex, pageSize);
            return new PagedResult<TeamDto>
            {
                Items = _mapper.Map<List<TeamDto>>(items),
                Total = total,
                Page = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<TeamDto?> GetByIdAsync(Guid id)
        {
            var entity = await _unitOfWork.TeamRepository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TeamDto>(entity);
        }

        public async Task<TeamDto> CreateAsync(CreateTeamRequest createTeamDto)
        {
            var entity = _mapper.Map<Team>(createTeamDto);
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.Now;
            var added = await _unitOfWork.TeamRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TeamDto>(added);
        }

        public async Task<TeamDto> UpdateAsync(Guid id, UpdateTeamRequest updateTeamDto)
        {
            var entity = await _unitOfWork.TeamRepository.GetByIdAsync(id);
            if (entity == null) throw new Exception("班组不存在");
            _mapper.Map(updateTeamDto, entity);
            entity.UpdatedAt = DateTime.Now;
            await _unitOfWork.TeamRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TeamDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _unitOfWork.TeamRepository.GetByIdAsync(id);
            if (entity == null) return false;
            entity.Status = 0; // 软删除
            await _unitOfWork.TeamRepository.UpdateAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<TeamDto>> GetByProjectIdAsync(Guid projectId)
        {
            var q = _unitOfWork.TeamRepository.GetAll().Where(x => x.ProjectId == projectId);
            var items = await _unitOfWork.TeamRepository.GetListAsync(q);
            return _mapper.Map<List<TeamDto>>(items);
        }
    }
}
