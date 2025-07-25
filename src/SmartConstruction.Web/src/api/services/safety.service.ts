// =============================================
// 安全事件服务
// =============================================
import { BaseApiService } from './base.service'
import { ApiClient } from '../client'
import type { ApiResponse } from '@/types/global'
import type { CreateSafetyIncidentParams, UpdateSafetyIncidentParams, SafetyIncidentListRequest, HandleSafetyIncidentParams } from '@/types/api'

/**
 * 安全事件服务类
 */
export class SafetyService extends BaseApiService<any, CreateSafetyIncidentParams, UpdateSafetyIncidentParams> {
  private apiClient: ApiClient

  constructor() {
    super('/api/safety-incidents')
    this.apiClient = new ApiClient()
  }

  /**
   * 获取安全事件列表
   * @param params 查询参数
   * @returns 安全事件列表
   */
  async getList(params?: SafetyIncidentListRequest): Promise<ApiResponse<any>> {
    return this.apiClient.get('safety', 'getList', params)
  }

  /**
   * 处理安全事件
   * @param id 安全事件ID
   * @param params 处理参数
   * @returns 处理结果
   */
  async handle(id: string, params: HandleSafetyIncidentParams): Promise<ApiResponse<any>> {
    return this.apiClient.post('safety', 'handle', { id, ...params })
  }

  /**
   * 获取安全统计
   * @param projectId 项目ID
   * @param startDate 开始日期
   * @param endDate 结束日期
   * @returns 安全统计
   */
  async getStatistics(projectId?: string, startDate?: string, endDate?: string): Promise<ApiResponse<any>> {
    return this.apiClient.get('safety', 'getStatistics', { projectId, startDate, endDate })
  }

  /**
   * 上传附件
   * @param id 安全事件ID
   * @param file 附件文件
   * @returns 上传结果
   */
  async uploadAttachment(id: string, file: File): Promise<ApiResponse<any>> {
    return this.apiClient.upload('safety', 'uploadAttachment', file, { id })
  }

  /**
   * 删除附件
   * @param id 安全事件ID
   * @param attachmentId 附件ID
   * @returns 删除结果
   */
  async deleteAttachment(id: string, attachmentId: string): Promise<ApiResponse<any>> {
    return this.apiClient.delete('safety', 'deleteAttachment', { id, attachmentId })
  }
}