// =============================================
// 设备服务
// =============================================
import { BaseApiService } from './base.service'
import { request } from '../request'
import type { ApiResponse } from '@/types/global'
import type { CreateDeviceParams, UpdateDeviceParams, DeviceListRequest, DeviceStatusParams } from '@/types/api'

/**
 * 设备服务类
 */
export class DeviceService extends BaseApiService<any, CreateDeviceParams, UpdateDeviceParams> {
  constructor() {
    super('/api/devices')
  }

  /**
   * 获取设备列表
   * @param params 查询参数
   * @returns 设备列表
   */
  async getList(params?: DeviceListRequest): Promise<ApiResponse<any>> {
    return request.get(this.baseUrl, params)
  }

  /**
   * 根据设备编号获取设备
   * @param code 设备编号
   * @returns 设备详情
   */
  async getByCode(code: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/code/${code}`)
  }

  /**
   * 检查设备编号是否存在
   * @param deviceCode 设备编号
   * @param excludeId 排除的设备ID
   * @returns 是否存在
   */
  async checkCode(deviceCode: string, excludeId?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/check-code`, { deviceCode, excludeId })
  }

  /**
   * 获取项目下的设备列表
   * @param projectId 项目ID
   * @param params 查询参数
   * @returns 设备列表
   */
  async getByProject(projectId: string, params?: DeviceListRequest): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/project/${projectId}`, params)
  }

  /**
   * 更新设备状态
   * @param id 设备ID
   * @param status 设备状态
   * @returns 更新结果
   */
  async updateStatus(id: string, status: string): Promise<ApiResponse<any>> {
    return request.put(`${this.baseUrl}/${id}/status`, { status })
  }
}