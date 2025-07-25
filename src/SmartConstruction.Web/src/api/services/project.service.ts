// =============================================
// 项目服务
// =============================================
import { BaseApiService } from './base.service'
import { request } from '../request'
import type { ApiResponse } from '@/types/global'
import type { CreateProjectParams, UpdateProjectParams, ProjectListRequest } from '@/types/api'

/**
 * 项目服务类
 */
export class ProjectService extends BaseApiService<any, CreateProjectParams, UpdateProjectParams> {
  constructor() {
    super('/api/projects')
  }

  /**
   * 获取项目列表
   * @param params 查询参数
   * @returns 项目列表
   */
  async getList(params?: ProjectListRequest): Promise<ApiResponse<any>> {
    return request.get(this.baseUrl, params)
  }

  /**
   * 检查项目编号是否存在
   * @param projectCode 项目编号
   * @param excludeId 排除的项目ID
   * @returns 是否存在
   */
  async checkCode(projectCode: string, excludeId?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/check-code`, { projectCode, excludeId })
  }

  /**
   * 获取公司下的项目列表
   * @param companyId 公司ID
   * @param params 查询参数
   * @returns 项目列表
   */
  async getByCompany(companyId: string, params?: ProjectListRequest): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/by-company/${companyId}`, params)
  }
}