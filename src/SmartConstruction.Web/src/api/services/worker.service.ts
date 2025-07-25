import { apiClient } from '../client';
import type { PagedRequest, PagedResult } from '../types/common';
import type { Worker } from '../types/models';

/**
 * 工人服务
 */
export class WorkerService {
  private static readonly BASE_URL = '/api/workers';

  /**
   * 获取工人列表
   * @param params 查询参数
   * @returns 工人列表
   */
  getList(params: PagedRequest & {
    keyword?: string
    companyId?: string
    projectId?: string
    teamId?: string
    type?: string
    isActive?: boolean
  }) {
    return apiClient.get<PagedResult<any>>(WorkerService.BASE_URL, { params })
  }

  /**
   * 根据ID获取工人
   * @param id 工人ID
   * @returns 工人详情
   */
  getById(id: string) {
    return apiClient.get<any>(`${WorkerService.BASE_URL}/${id}`)
  }

  /**
   * 创建工人
   * @param data 工人数据
   * @returns 创建结果
   */
  create(data: any) {
    return apiClient.post<any>(WorkerService.BASE_URL, data)
  }

  /**
   * 更新工人
   * @param id 工人ID
   * @param data 工人数据
   * @returns 更新结果
   */
  update(id: string, data: any) {
    return apiClient.put<any>(`${WorkerService.BASE_URL}/${id}`, data)
  }

  /**
   * 更新工人状态
   * @param id 工人ID
   * @param isActive 是否启用
   * @returns 更新结果
   */
  updateStatus(id: string, isActive: boolean) {
    return apiClient.put<any>(`${WorkerService.BASE_URL}/${id}/status`, { isActive })
  }

  /**
   * 删除工人
   * @param id 工人ID
   * @returns 删除结果
   */
  delete(id: string) {
    return apiClient.delete<boolean>(`${WorkerService.BASE_URL}/${id}`)
  }

  /**
   * 获取工人统计信息
   * @param id 工人ID
   * @returns 统计信息
   */
  getStats(id: string) {
    return apiClient.get<any>(`${WorkerService.BASE_URL}/${id}/stats`)
  }

  /**
   * 根据项目获取工人
   * @param projectId 项目ID
   * @param params 查询参数
   * @returns 工人列表
   */
  getByProject(projectId: string, params: PagedRequest) {
    return apiClient.get<PagedResult<any>>(`${WorkerService.BASE_URL}/project/${projectId}`, { params })
  }

  /**
   * 根据班组获取工人
   * @param teamId 班组ID
   * @param params 查询参数
   * @returns 工人列表
   */
  getByTeam(teamId: string, params: PagedRequest) {
    return apiClient.get<PagedResult<any>>(`${WorkerService.BASE_URL}/team/${teamId}`, { params })
  }

  /**
   * 获取可用工人（未分配班组或可以重新分配的工人）
   * @param params 查询参数
   * @returns 工人列表
   */
  getAvailable(params: PagedRequest & {
    keyword?: string
    idCardNumber?: string
    excludeTeamId?: string
  }) {
    return apiClient.get<PagedResult<any>>(`${WorkerService.BASE_URL}/available`, { params })
  }

  /**
   * 获取工人考勤记录
   * @param workerId 工人ID
   * @param params 查询参数
   * @returns 考勤记录
   */
  getAttendance(workerId: string, params: PagedRequest & {
    startDate?: string
    endDate?: string
  }) {
    return apiClient.get<PagedResult<any>>(`${WorkerService.BASE_URL}/${workerId}/attendance`, { params })
  }

  /**
   * 获取工人考勤统计
   * @param workerId 工人ID
   * @param params 查询参数
   * @returns 考勤统计
   */
  getAttendanceStats(workerId: string, params: {
    startDate?: string
    endDate?: string
  }) {
    return apiClient.get<any>(`${WorkerService.BASE_URL}/${workerId}/attendance/stats`, { params })
  }
}