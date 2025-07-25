// =============================================
// 公司服务
// =============================================
import { BaseApiService } from './base.service'
import { ApiClient } from '../client'
import type { ApiResponse } from '@/types/global'
import type { CreateCompanyParams, UpdateCompanyParams, CompanyListRequest } from '@/types/api'

/**
 * 公司服务类
 */
export class CompanyService extends BaseApiService<any, CreateCompanyParams, UpdateCompanyParams> {
  private apiClient: ApiClient

  constructor() {
    super('/api/companies')
    this.apiClient = new ApiClient()
  }

  /**
   * 获取公司列表
   * @param params 查询参数
   * @returns 公司列表
   */
  async getList(params?: CompanyListRequest): Promise<ApiResponse<any>> {
    return this.apiClient.get('company', 'getList', params)
  }

  /**
   * 检查公司名称是否存在
   * @param companyName 公司名称
   * @param excludeId 排除的公司ID
   * @returns 是否存在
   */
  async checkName(companyName: string, excludeId?: string): Promise<ApiResponse<any>> {
    return this.apiClient.get('company', 'checkName', { companyName, excludeId })
  }

  /**
   * 检查统一社会信用代码是否存在
   * @param code 统一社会信用代码
   * @param excludeId 排除的公司ID
   * @returns 是否存在
   */
  async checkCode(code: string, excludeId?: string): Promise<ApiResponse<any>> {
    return this.apiClient.get('company', 'checkCode', { code, excludeId })
  }
}