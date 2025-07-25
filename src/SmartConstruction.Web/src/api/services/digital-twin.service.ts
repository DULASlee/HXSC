// =============================================
// 数字孪生大屏API服务
// =============================================
import { BaseApiService } from './base.service'
import { http as request } from '@/utils/http'
import type { ApiResponse } from '@/types/api'

/**
 * 数字孪生大屏API服务类
 */
export class DigitalTwinService extends BaseApiService<any, any, any> {
  constructor() {
    super('/api/digital-twin')
  }

  // ==================== 指挥中心大屏 ====================
  
  /**
   * 获取指挥中心总览数据
   */
  async getCommandCenterOverview(projectId?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/command-center/overview`, { projectId })
  }

  /**
   * 获取项目列表及状态
   */
  async getProjectList(params?: any): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/command-center/projects`, params)
  }

  /**
   * 获取实时数据统计
   */
  async getRealtimeStats(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/command-center/realtime-stats`)
  }

  /**
   * 获取趋势图表数据
   */
  async getTrends(type: string, timeRange: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/command-center/trends`, { type, timeRange })
  }

  // ==================== 项目考勤大屏 ====================
  
  /**
   * 获取考勤总览统计
   */
  async getAttendanceOverview(projectId?: string, date?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/attendance/overview`, { projectId, date })
  }

  /**
   * 获取实时考勤动态
   */
  async getAttendanceRealtime(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/attendance/realtime`)
  }

  /**
   * 获取班组考勤排行
   */
  async getTeamRanking(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/attendance/team-ranking`)
  }

  /**
   * 获取考勤趋势图表
   */
  async getAttendanceTrends(timeRange: string, chartType: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/attendance/trends`, { timeRange, chartType })
  }

  // ==================== 视频监控大屏 ====================
  
  /**
   * 获取监控点位信息
   */
  async getVideoCameras(projectId?: string, status?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/video-monitor/cameras`, { projectId, status })
  }

  /**
   * 获取视频监控统计
   */
  async getVideoStatistics(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/video-monitor/statistics`)
  }

  /**
   * 获取智能分析结果
   */
  async getAiAnalysis(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/video-monitor/ai-analysis`)
  }

  // ==================== 塔吊升降机管理大屏 ====================
  
  /**
   * 获取设备列表及状态
   */
  async getCraneElevatorDevices(projectId?: string, deviceType?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/crane-elevator/devices`, { projectId, deviceType })
  }

  /**
   * 获取设备运行统计
   */
  async getCraneElevatorStatistics(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/crane-elevator/statistics`)
  }

  /**
   * 获取安全监控数据
   */
  async getSafetyMonitoring(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/crane-elevator/safety-monitoring`)
  }

  /**
   * 获取工作效率分析
   */
  async getEfficiencyAnalysis(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/crane-elevator/efficiency-analysis`)
  }

  // ==================== 扬尘噪音监测大屏 ====================
  
  /**
   * 获取环境监测数据
   */
  async getEnvironmentMonitoring(projectId?: string, monitorType?: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/environment/monitoring-data`, { projectId, monitorType })
  }

  /**
   * 获取监测点位信息
   */
  async getMonitoringPoints(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/environment/monitoring-points`)
  }

  /**
   * 获取环境趋势分析
   */
  async getEnvironmentTrends(timeRange: string, dataType: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/environment/trend-analysis`, { timeRange, dataType })
  }

  /**
   * 获取环境告警信息
   */
  async getEnvironmentAlerts(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/environment/alerts`)
  }

  // ==================== 通用工具接口 ====================
  
  /**
   * 获取项目基础信息
   */
  async getProjectInfo(projectId: string): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/common/project-info/${projectId}`)
  }

  /**
   * 获取系统状态
   */
  async getSystemStatus(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/common/system-status`)
  }

  /**
   * 获取实时时间
   */
  async getCurrentTime(): Promise<ApiResponse<any>> {
    return request.get(`${this.baseUrl}/common/current-time`)
  }
}

// 导出实例
export const digitalTwinService = new DigitalTwinService()