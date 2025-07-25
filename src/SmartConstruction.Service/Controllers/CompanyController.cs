using Microsoft.AspNetCore.Mvc;
using SmartConstruction.Contracts.Dtos.Company;
using SmartConstruction.Service.Controllers.Base;
using SmartConstruction.Service.Services;
using System;
using System.Threading.Tasks;
using System.Collections.Generic; // Added for KeyNotFoundException
using Microsoft.Extensions.Logging; // Added for ILogger

namespace SmartConstruction.Service.Controllers
{
    /// <summary>
    /// 公司管理控制器
    /// </summary>
    [Route("api/companies")]
    [ApiController]
    public class CompanyController : BaseApiController
    {
        private readonly ICompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ICompanyService companyService, ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <param name="unifiedSocialCreditCode">统一社会信用代码</param>
        /// <param name="status">状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>公司列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetCompanies(
            [FromQuery] string companyName,
            [FromQuery] string unifiedSocialCreditCode,
            [FromQuery] int? status,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var queryParams = new CompanyQueryParams
                {
                    CompanyName = companyName,
                    UnifiedSocialCreditCode = unifiedSocialCreditCode,
                    Status = status?.ToString(),
                    PageIndex = pageIndex < 1 ? 1 : pageIndex,
                    PageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize)
                };

                var result = await _companyService.GetCompaniesAsync(queryParams);
                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取公司列表时发生错误");
                return Error("获取公司列表失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 根据ID获取公司
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns>公司信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                return Success(company);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"获取公司(ID:{id})时发生错误");
                return Error("获取公司信息失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 创建公司
        /// </summary>
        /// <param name="request">创建请求</param>
        /// <returns>创建后的公司信息</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyRequest request)
        {
            try
            {
                var company = await _companyService.CreateCompanyAsync(request);
                return Success(company);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建公司时发生错误");
                return Error("创建公司失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 更新公司
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新后的公司信息</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] UpdateCompanyRequest request)
        {
            try
            {
                var company = await _companyService.UpdateCompanyAsync(id, request);
                return Success(company);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message, 404);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新公司(ID:{id})时发生错误");
                return Error("更新公司失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 删除公司
        /// </summary>
        /// <param name="id">公司ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            try
            {
                var result = await _companyService.DeleteCompanyAsync(id);
                if (result)
                {
                    return Success(null, "删除公司成功");
                }
                return Error("未找到要删除的公司", 404);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return Error(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"删除公司(ID:{id})时发生错误");
                return Error("删除公司失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 检查公司名称是否已存在
        /// </summary>
        /// <param name="companyName">公司名称</param>
        /// <param name="excludeId">排除的公司ID</param>
        /// <returns>检查结果</returns>
        [HttpGet("check-name")]
        public async Task<IActionResult> CheckCompanyName([FromQuery] string companyName, [FromQuery] Guid? excludeId = null)
        {
            try
            {
                var exists = await _companyService.IsCompanyNameExistsAsync(companyName, excludeId);
                return Success(new { Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查公司名称时发生错误");
                return Error("检查公司名称失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 检查统一社会信用代码是否已存在
        /// </summary>
        /// <param name="code">统一社会信用代码</param>
        /// <param name="excludeId">排除的公司ID</param>
        /// <returns>检查结果</returns>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckUnifiedSocialCreditCode([FromQuery] string code, [FromQuery] Guid? excludeId = null)
        {
            try
            {
                var exists = await _companyService.IsUnifiedSocialCreditCodeExistsAsync(code, excludeId);
                return Success(new { Exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "检查统一社会信用代码时发生错误");
                return Error("检查统一社会信用代码失败：" + ex.Message);
            }
        }
    }
} 