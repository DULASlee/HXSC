// =============================================
// 租户服务
// =============================================
import { BaseApiService } from './base.service'
import { ApiClient } from '../client'
import type { ApiResponse } from '@/types/global'
import type { CreateTenantParams, UpdateTenantParams, TenantStatusParams, TenantListRequest } from '@/types/api'

/**
 * 租户服务类
 */
export class TenantService extends BaseApiService<any, CreateTenantParams, UpdateTenantParams> {
  private apiClient: ApiClient

  constructor() {
    super('/api/tenant')
    this.apiClient = new ApiClient()
  }

  /**
   * 获取租户列表
   * @param params 查询参数
   * @returns 租户列表
   */
  async getList(params?: TenantListRequest): Promise<ApiResponse<any>> {
    return this.apiClient.get('tenant', 'getList', params)
  }

  /**
   * 检查租户编码是否存在
   * @param code 租户编码
   * @param excludeId 排除的租户ID
   * @returns 是否存在
   */
  async checkCode(code: string, excludeId?: string): Promise<ApiResponse<boolean>> {
    return this.apiClient.get('tenant', 'checkCode', { code, excludeId })
  }

  /**
   * 更新租户状态
   * @param id 租户ID
   * @param isActive 是否激活
   * @returns 更新结果
   */
  async updateStatus(id: string, isActive: boolean): Promise<ApiResponse<any>> {
    return this.apiClient.put('tenant', 'updateStatus', { id, isActive })
  }
}