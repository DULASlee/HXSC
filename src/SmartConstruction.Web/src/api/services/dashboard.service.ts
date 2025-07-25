// =============================================
// 仪表盘服务
// =============================================
import { BaseApiService } from './base.service'
import { request } from '../request'
import type { ApiResponse } from '@/types/global'
import type { DashboardParams, ProjectDashboardParams, CompanyDashboardParams } from '@/types/api'

/**
 * 仪表盘服务类
 */
export class DashboardService extends BaseApiService<any, any, any> {
  constructor() {
    super('/api/dashboard')
  }

  /**
   * 获取项目概览
   * @param projectId 项目ID
   * @returns 项目概览
   */
  async getProjectOverview(projectId: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/projects/${projectId}/overview`)
  }

  /**
   * 获取项目考勤统计
   * @param projectId 项目ID
   * @param startDate 开始日期
   * @param endDate 结束日期
   * @returns 考勤统计
   */
  async getProjectAttendance(projectId: string, startDate?: string, endDate?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/projects/${projectId}/attendance`, { startDate, endDate })
  }

  /**
   * 获取项目安全事件统计
   * @param projectId 项目ID
   * @param startDate 开始日期
   * @param endDate 结束日期
   * @returns 安全事件统计
   */
  async getProjectSafety(projectId: string, startDate?: string, endDate?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/projects/${projectId}/safety`, { startDate, endDate })
  }

  /**
   * 获取项目设备状态统计
   * @param projectId 项目ID
   * @returns 设备状态统计
   */
  async getProjectDevices(projectId: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/projects/${projectId}/devices`)
  }

  /**
   * 获取项目完整仪表盘数据
   * @param params 查询参数
   * @returns 仪表盘数据
   */
  async getProjectDashboard(params: ProjectDashboardParams): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/projects/${params.projectId}`, {
      startDate: params.startDate,
      endDate: params.endDate
    })
  }

  /**
   * 获取公司整体仪表盘数据
   * @param params 查询参数
   * @returns 仪表盘数据
   */
  async getCompanyDashboard(params: CompanyDashboardParams): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/companies/${params.companyId}`, {
      startDate: params.startDate,
      endDate: params.endDate
    })
  }
}