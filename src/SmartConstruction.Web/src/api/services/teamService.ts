import { http } from '@/utils/http'
import type { PagedRequest, PagedResult } from '@/types/common'

/**
 * 班组服务
 */
export default {
  /**
   * 获取班组列表
   * @param params 查询参数
   * @returns 班组列表
   */
  getList(params: PagedRequest & {
    keyword?: string
    companyId?: string
    type?: string
    isActive?: boolean
  }) {
    return http.get<PagedResult<any>>('/api/teams', { params })
  },

  /**
   * 获取所有班组
   * @returns 所有班组
   */
  getAll() {
    return http.get<any[]>('/api/teams/all')
  },

  /**
   * 根据ID获取班组
   * @param id 班组ID
   * @returns 班组详情
   */
  getById(id: string) {
    return http.get<any>(`/api/teams/${id}`)
  },

  /**
   * 创建班组
   * @param data 班组数据
   * @returns 创建结果
   */
  create(data: any) {
    return http.post<any>('/api/teams', data)
  },

  /**
   * 更新班组
   * @param id 班组ID
   * @param data 班组数据
   * @returns 更新结果
   */
  update(id: string, data: any) {
    return http.put<any>(`/api/teams/${id}`, data)
  },

  /**
   * 更新班组状态
   * @param id 班组ID
   * @param isActive 是否启用
   * @returns 更新结果
   */
  updateStatus(id: string, isActive: boolean) {
    return http.put<any>(`/api/teams/${id}/status`, { isActive })
  },

  /**
   * 删除班组
   * @param id 班组ID
   * @returns 删除结果
   */
  delete(id: string) {
    return http.delete<boolean>(`/api/teams/${id}`)
  },

  /**
   * 获取班组统计信息
   * @param id 班组ID
   * @returns 统计信息
   */
  getStats(id: string) {
    return http.get<any>(`/api/teams/${id}/stats`)
  },

  /**
   * 获取班组项目
   * @param id 班组ID
   * @returns 项目列表
   */
  getProjects(id: string) {
    return http.get<any[]>(`/api/teams/${id}/projects`)
  },

  /**
   * 添加工人到班组
   * @param teamId 班组ID
   * @param workerIds 工人ID列表
   * @returns 添加结果
   */
  addWorkers(teamId: string, workerIds: string[]) {
    return http.post<boolean>(`/api/teams/${teamId}/workers`, { workerIds })
  },

  /**
   * 从班组移除工人
   * @param teamId 班组ID
   * @param workerId 工人ID
   * @returns 移除结果
   */
  removeWorker(teamId: string, workerId: string) {
    return http.delete<boolean>(`/api/teams/${teamId}/workers/${workerId}`)
  },

  /**
   * 根据公司ID获取班组
   * @param companyId 公司ID
   * @returns 班组列表
   */
  getByCompany(companyId: string) {
    return http.get<any[]>(`/api/teams/company/${companyId}`)
  },

  /**
   * 根据项目ID获取班组
   * @param projectId 项目ID
   * @returns 班组列表
   */
  getByProject(projectId: string) {
    return http.get<any[]>(`/api/teams/project/${projectId}`)
  }
}