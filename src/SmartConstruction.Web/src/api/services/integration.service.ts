// =============================================
// 集成服务
// =============================================
import { BaseApiService } from './base.service'
import { request } from '../request'
import type { ApiResponse } from '@/types/global'
import type { IoTDataParams, IoTAlertParams, DeviceStatusChangeParams } from '@/types/api'

/**
 * 集成服务类
 */
export class IntegrationService extends BaseApiService<any, any, any> {
  constructor() {
    super('/api/integration')
  }

  /**
   * 接收IoT设备数据
   * @param params IoT数据参数
   * @returns 接收结果
   */
  async receiveIoTData(params: IoTDataParams): Promise<ApiResponse<any>> {
    return request.post(`${this.baseUrl}/iot-data`, params)
  }

  /**
   * 接收IoT设备告警
   * @param params IoT告警参数
   * @returns 接收结果
   */
  async receiveIoTAlert(params: IoTAlertParams): Promise<ApiResponse<any>> {
    return request.post(`${this.baseUrl}/iot-alert`, params)
  }

  /**
   * 发送设备状态变更
   * @param params 设备状态变更参数
   * @returns 发送结果
   */
  async sendDeviceStatusChange(params: DeviceStatusChangeParams): Promise<ApiResponse<any>> {
    return request.post(`${this.baseUrl}/device-status`, params)
  }

  /**
   * 获取IoT设备历史数据
   * @param deviceCode 设备编号
   * @param startTime 开始时间
   * @param endTime 结束时间
   * @returns 历史数据
   */
  async getIoTHistory(deviceCode: string, startTime?: string, endTime?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/iot-history`, { deviceCode, startTime, endTime })
  }

  /**
   * 获取IoT设备告警历史
   * @param deviceCode 设备编号
   * @param startTime 开始时间
   * @param endTime 结束时间
   * @returns 告警历史
   */
  async getIoTAlertHistory(deviceCode: string, startTime?: string, endTime?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/iot-alert-history`, { deviceCode, startTime, endTime })
  }

  /**
   * 获取政府推送配置
   * @returns 政府推送配置
   */
  async getGovernmentPushConfig(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/government-config`)
  }

  /**
   * 更新政府推送配置
   * @param config 政府推送配置
   * @returns 更新结果
   */
  async updateGovernmentPushConfig(config: any): Promise<ApiResponse<any>> {
    return request.put(`${this.baseUrl}/government-config`, config)
  }

  /**
   * 测试政府推送连接
   * @returns 测试结果
   */
  async testGovernmentPushConnection(): Promise<ApiResponse<any>> {
    return request.post(`${this.baseUrl}/government-test`)
  }
}