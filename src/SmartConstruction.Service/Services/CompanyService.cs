using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartConstruction.Contracts.Dtos.Base;
using SmartConstruction.Contracts.Dtos.Company;
using SmartConstruction.Contracts.Entities;
using SmartConstruction.Service.Infrastructure.UnitOfWork;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CompanyService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<CompanyDto>> GetCompaniesAsync(CompanyQueryParams queryParams)
        {
            try
            {
                var repository = _unitOfWork.CompanyRepository;
                var query = repository.GetAll();

                if (!string.IsNullOrEmpty(queryParams.CompanyName))
                {
                    query = query.Where(c => c.CompanyName.Contains(queryParams.CompanyName));
                }

                if (!string.IsNullOrEmpty(queryParams.UnifiedSocialCreditCode))
                {
                    query = query.Where(c => c.UnifiedSocialCreditCode.Contains(queryParams.UnifiedSocialCreditCode));
                }

                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    query = query.Where(c => c.Status.ToString() == queryParams.Status);
                }

                var totalCount = await repository.CountAsync(query);
                var items = query
                    .OrderByDescending(c => c.CreatedAt)
                    .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                    .Take(queryParams.PageSize)
                    .ToList();

                var dtoItems = _mapper.Map<List<CompanyDto>>(items);
                return new PagedResult<CompanyDto>
                {
                    Items = dtoItems,
                    Total = totalCount,
                    Page = queryParams.PageIndex,
                    PageSize = queryParams.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取公司列表时发生错误");
                throw;
            }
        }

        public async Task<CompanyDto> GetCompanyByIdAsync(Guid id)
        {
            try
            {
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
                if (company == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的公司");
                }

                return _mapper.Map<CompanyDto>(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取公司(ID:{id})时发生错误");
                throw;
            }
        }

        public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyRequest request)
        {
            try
            {
                if (await IsCompanyNameExistsAsync(request.CompanyName))
                {
                    throw new InvalidOperationException($"公司名称'{request.CompanyName}'已存在");
                }

                if (await IsUnifiedSocialCreditCodeExistsAsync(request.UnifiedSocialCreditCode))
                {
                    throw new InvalidOperationException($"统一社会信用代码'{request.UnifiedSocialCreditCode}'已存在");
                }

                var company = _mapper.Map<Company>(request);
                company.Status = 1;

                _unitOfWork.CompanyRepository.Create(company);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<CompanyDto>(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建公司时发生错误");
                throw;
            }
        }

        public async Task<CompanyDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequest request)
        {
            try
            {
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
                if (company == null)
                {
                    throw new KeyNotFoundException($"未找到ID为{id}的公司");
                }

                if (request.CompanyName != company.CompanyName &&
                    await IsCompanyNameExistsAsync(request.CompanyName, id))
                {
                    throw new InvalidOperationException($"公司名称'{request.CompanyName}'已存在");
                }

                if (request.UnifiedSocialCreditCode != company.UnifiedSocialCreditCode &&
                    await IsUnifiedSocialCreditCodeExistsAsync(request.UnifiedSocialCreditCode, id))
                {
                    throw new InvalidOperationException($"统一社会信用代码'{request.UnifiedSocialCreditCode}'已存在");
                }

                _mapper.Map(request, company);
                company.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.CompanyRepository.Update(company);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<CompanyDto>(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新公司(ID:{id})时发生错误");
                throw;
            }
        }

        public async Task<bool> DeleteCompanyAsync(Guid id)
        {
            try
            {
                var company = await _unitOfWork.CompanyRepository.GetByIdAsync(id);
                if (company == null)
                {
                    return false;
                }

                var hasProjects = await _unitOfWork.ProjectRepository.ExistsAsync(p => p.CompanyId == id);
                if (hasProjects)
                {
                    throw new InvalidOperationException("该公司下存在项目，无法删除");
                }

                _unitOfWork.CompanyRepository.Delete(company);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除公司(ID:{id})时发生错误");
                throw;
            }
        }

        public async Task<bool> IsCompanyNameExistsAsync(string companyName, Guid? excludeId = null)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                return false;
            }

            return await _unitOfWork.CompanyRepository.ExistsAsync(c => c.CompanyName == companyName && (!excludeId.HasValue || c.Id != excludeId.Value));
        }

        public async Task<bool> IsUnifiedSocialCreditCodeExistsAsync(string code, Guid? excludeId = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                return false;
            }
            return await _unitOfWork.CompanyRepository.ExistsAsync(
                c => c.UnifiedSocialCreditCode == code && (!excludeId.HasValue || c.Id != excludeId.Value));
        }
    }
}
