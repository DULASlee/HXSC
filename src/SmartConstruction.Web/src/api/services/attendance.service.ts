// =============================================
// 考勤服务
// =============================================
import { BaseApiService } from './base.service'
import { request } from '../request'
import type { ApiResponse } from '@/types/global'
import type { CreateAttendanceParams, UpdateAttendanceParams, AttendanceListRequest } from '@/types/api'

/**
 * 考勤服务类
 */
export class AttendanceService extends BaseApiService<any, CreateAttendanceParams, UpdateAttendanceParams> {
  constructor() {
    super('/api/attendances')
  }

  /**
   * 获取考勤记录列表
   * @param params 查询参数
   * @returns 考勤记录列表
   */
  async getList(params?: AttendanceListRequest): Promise<ApiResponse<any>> {
    return request.get(this.baseUrl, params)
  }

  /**
   * 获取工人的考勤记录
   * @param workerId 工人ID
   * @param params 查询参数
   * @returns 考勤记录列表
   */
  async getByWorker(workerId: string, params?: AttendanceListRequest): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/worker/${workerId}`, params)
  }

  /**
   * 获取项目的考勤记录
   * @param projectId 项目ID
   * @param params 查询参数
   * @returns 考勤记录列表
   */
  async getByProject(projectId: string, params?: AttendanceListRequest): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/project/${projectId}`, params)
  }

  /**
   * 获取项目考勤统计
   * @param projectId 项目ID
   * @param date 日期
   * @returns 考勤统计
   */
  async getProjectStatistics(projectId: string, date?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/project/${projectId}/statistics`, { date })
  }

  /**
   * 签到
   * @param params 签到参数
   * @returns 签到结果
   */
  async checkIn(params: CreateAttendanceParams): Promise<ApiResponse<any>> {
    return request.post(`${this.baseUrl}/check-in`, params)
  }

  /**
   * 签退
   * @param id 考勤记录ID
   * @param checkOutTime 签退时间
   * @returns 签退结果
   */
  async checkOut(id: string, checkOutTime: string): Promise<ApiResponse<any>> {
    return request.put(`${this.baseUrl}/${id}/check-out`, { checkOutTime })
  }
}