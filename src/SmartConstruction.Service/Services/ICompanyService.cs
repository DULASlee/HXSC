using SmartConstruction.Contracts.Dtos.Company;
using SmartConstruction.Contracts.Dtos.Base;
using System;
using System.Threading.Tasks;

namespace SmartConstruction.Service.Services
{
    /// <summary>
    /// 公司服务接口
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns>分页结果</returns>
        Task<PagedResult<CompanyDto>> GetCompaniesAsync(CompanyQueryParams queryParams);

        /// <summary>
        /// 根据ID获取公司
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns>公司DTO</returns>
        Task<CompanyDto> GetCompanyByIdAsync(Guid id);

        /// <summary>
        /// 创建公司
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的公司DTO</returns>
        Task<CompanyDto> CreateCompanyAsync(CreateCompanyRequest request);

        /// <summary>
        /// 更新公司
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的公司DTO</returns>
        Task<CompanyDto> UpdateCompanyAsync(Guid id, UpdateCompanyRequest request);

        /// <summary>
        /// 删除公司
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteCompanyAsync(Guid id);

        /// <summary>
        /// 检查公司名称是否已存在
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <param name="excludeId">排除的公司ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsCompanyNameExistsAsync(string companyName, Guid? excludeId = null);

        /// <summary>
        /// 检查统一社会信用代码是否已存在
        /// </summary>
        /// <param name="code">统一社会信用代码</param>
        /// <param name="excludeId">排除的公司ID</param>
        /// <returns>是否存在</returns>
        Task<bool> IsUnifiedSocialCreditCodeExistsAsync(string code, Guid? excludeId = null);
    }
}
