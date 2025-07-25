import { http } from '../client'
import type { ApiResponse } from '../types'

// =============================================
// 数字孪生API接口
// =============================================

export interface DeviceInfo {
  id: string
  name: string
  type: string
  position: { x: number; y: number; z: number }
  status: string
  metrics: Record<string, any>
  lastUpdate: string
}

export interface PersonnelInfo {
  userId: string
  userName: string
  position: {
    x: number
    y: number
    z: number
    timestamp: string
  }
  status: string
  team: string
  profession: string
  safetyLevel: string
}

export interface CraneInfo {
  id: string
  name: string
  position: { x: number; y: number; z: number }
  rotation: number
  jibAngle: number
  hookHeight: number
  load: number
  maxLoad: number
  status: string
  workRadius: number
  safetyZone: Array<{ x: number; z: number; radius: number }>
  lastUpdate: string
}

export interface EnvironmentStation {
  id: string
  name: string
  position: { x: number; y: number; z: number }
  metrics: {
    dust: { value: number; unit: string; threshold: number; status: string }
    noise: { value: number; unit: string; threshold: number; status: string }
    temperature: { value: number; unit: string; status: string }
    humidity: { value: number; unit: string; status: string }
    windSpeed: { value: number; unit: string; status: string }
    airQuality: { value: number; unit: string; status: string }
  }
  lastUpdate: string
}

export interface DeviceControlRequest {
  action: string
  parameters?: Record<string, any>
}

export interface CraneControlRequest {
  action: string
  parameters?: Record<string, any>
}

export interface EnvironmentTreatmentRequest {
  treatmentType: string
  targetArea?: any
}

/// <summary>
/// 数字孪生API服务
/// </summary>
export const digitalTwinApi = {
  // =============================================
  // 指挥中心API
  // =============================================

  /**
   * 获取工地总览数据
   */
  getOverview: (): Promise<ApiResponse<any>> => {
    return http.get<ApiResponse<any>>('/api/DigitalTwin/overview')
  },

  /**
   * 获取设备列表
   */
  getDevices: (): Promise<ApiResponse<DeviceInfo[]>> => {
    return http.get<ApiResponse<DeviceInfo[]>>('/api/DigitalTwin/devices')
  },

  /**
   * 控制设备
   */
  controlDevice: (deviceId: string, request: DeviceControlRequest): Promise<ApiResponse<any>> => {
    return http.post<ApiResponse<any>>(`/api/DigitalTwin/devices/${deviceId}/control`, request)
  },

  // =============================================
  // 实名制考勤API
  // =============================================

  /**
   * 获取人员位置数据
   */
  getPersonnelPositions: (date?: string): Promise<ApiResponse<{
    date: string
    totalCount: number
    onSiteCount: number
    personnel: PersonnelInfo[]
  }>> => {
    const params = date ? { date } : {}
    return http.get<ApiResponse<any>>('/api/DigitalTwin/attendance/positions', { params })
  },

  /**
   * 获取人员轨迹回放数据
   */
  getPersonnelTrajectory: (userId: string, date: string): Promise<ApiResponse<{
    userId: string
    date: string
    totalDistance: number
    workDuration: number
    trajectoryPoints: Array<{
      timestamp: string
      position: { x: number; y: number; z: number }
      activity: string
    }>
  }>> => {
    return http.get<ApiResponse<any>>(`/api/DigitalTwin/attendance/trajectory/${userId}`, {
      params: { date }
    })
  },

  // =============================================
  // 塔吊升降机监控API
  // =============================================

  /**
   * 获取塔吊实时状态
   */
  getCraneStatus: (): Promise<ApiResponse<CraneInfo[]>> => {
    return http.get<ApiResponse<CraneInfo[]>>('/api/DigitalTwin/crane/status')
  },

  /**
   * 控制塔吊动作
   */
  controlCrane: (craneId: string, request: CraneControlRequest): Promise<ApiResponse<any>> => {
    return http.post<ApiResponse<any>>(`/api/DigitalTwin/crane/${craneId}/control`, request)
  },

  // =============================================
  // 扬尘噪音管理API
  // =============================================

  /**
   * 获取环境监测数据
   */
  getEnvironmentData: (): Promise<ApiResponse<{
    stations: EnvironmentStation[]
    heatmapData: {
      dust: Array<{ x: number; z: number; value: number }>
      noise: Array<{ x: number; z: number; value: number }>
      updateTime: string
    }
    overallStatus: {
      dustLevel: string
      noiseLevel: string
      airQuality: string
      lastUpdate: string
    }
  }>> => {
    return http.get<ApiResponse<any>>('/api/DigitalTwin/environment')
  },

  /**
   * 启动环境治理措施
   */
  startEnvironmentTreatment: (request: EnvironmentTreatmentRequest): Promise<ApiResponse<any>> => {
    return http.post<ApiResponse<any>>('/api/DigitalTwin/environment/treatment', request)
  },

  // =============================================
  // WebSocket/SSE 实时数据接口
  // =============================================

  /**
   * 建立设备状态实时连接
   */
  connectDeviceStream: (callback: (data: any) => void): EventSource => {
    const eventSource = new EventSource('/api/DigitalTwin/devices/stream')
    
    eventSource.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data)
        callback(data)
      } catch (error) {
        console.error('解析设备状态数据失败:', error)
      }
    }
    
    eventSource.onerror = (error) => {
      console.error('设备状态流连接错误:', error)
    }
    
    return eventSource
  },

  /**
   * 建立人员位置实时连接
   */
  connectPersonnelStream: (callback: (data: any) => void): EventSource => {
    const eventSource = new EventSource('/api/DigitalTwin/attendance/stream')
    
    eventSource.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data)
        callback(data)
      } catch (error) {
        console.error('解析人员位置数据失败:', error)
      }
    }
    
    return eventSource
  },

  /**
   * 建立环境数据实时连接
   */
  connectEnvironmentStream: (callback: (data: any) => void): EventSource => {
    const eventSource = new EventSource('/api/DigitalTwin/environment/stream')
    
    eventSource.onmessage = (event) => {
      try {
        const data = JSON.parse(event.data)
        callback(data)
      } catch (error) {
        console.error('解析环境数据失败:', error)
      }
    }
    
    return eventSource
  },

  // =============================================
  // 工具方法
  // =============================================

  /**
   * 模拟实时数据生成器
   */
  generateMockRealtimeData: () => {
    return {
      devices: [
        {
          id: 'crane001',
          metrics: {
            load: Math.round(30 + Math.random() * 40),
            temperature: Math.round(20 + Math.random() * 15),
            vibration: Math.round(Math.random() * 100) / 100
          },
          timestamp: new Date().toISOString()
        }
      ],
      personnel: [
        {
          userId: 'worker001',
          position: {
            x: Math.random() * 200 - 100,
            y: 1,
            z: Math.random() * 200 - 100
          },
          timestamp: new Date().toISOString()
        }
      ],
      environment: [
        {
          stationId: 'env001',
          dust: Math.round(30 + Math.random() * 50),
          noise: Math.round(45 + Math.random() * 30),
          timestamp: new Date().toISOString()
        }
      ]
    }
  },

  /**
   * 关闭所有实时连接
   */
  closeAllStreams: (streams: EventSource[]) => {
    streams.forEach(stream => {
      if (stream.readyState !== EventSource.CLOSED) {
        stream.close()
      }
    })
  }
}

// =============================================
// 响应类型定义
// =============================================

export interface DigitalTwinOverview {
  siteInfo: {
    name: string
    location: string
    area: string
    progress: number
    startDate: string
    expectedCompletion: string
  }
  statistics: {
    onlineDevices: number
    onlinePersonnel: number
    activeAlerts: number
    todayAttendance: number
    environmentIndex: number
  }
  recentAlerts: Array<{
    id: string
    level: string
    title: string
    description: string
    source: string
    timestamp: string
  }>
  weatherInfo: {
    temperature: number
    humidity: number
    windSpeed: number
    windDirection: string
    weather: string
  }
}

export interface TrajectoryPoint {
  timestamp: string
  position: { x: number; y: number; z: number }
  activity: string
}

export interface HeatmapData {
  dust: Array<{ x: number; z: number; value: number }>
  noise: Array<{ x: number; z: number; value: number }>
  updateTime: string
}

// =============================================
// 常量定义
// =============================================

export const DEVICE_TYPES = {
  CRANE: 'crane',
  ELEVATOR: 'elevator',
  MONITOR: 'monitor',
  CAMERA: 'camera'
} as const

export const ALERT_LEVELS = {
  INFO: 'info',
  WARNING: 'warning',
  CRITICAL: 'critical'
} as const

export const CRANE_ACTIONS = {
  ROTATE: 'rotate',
  LIFT: 'lift',
  EXTEND: 'extend',
  STOP: 'stop'
} as const

export const TREATMENT_TYPES = {
  SPRAY: 'spray',
  COVER: 'cover',
  BARRIER: 'barrier',
  VENTILATION: 'ventilation'
} as const

export default digitalTwinApi 