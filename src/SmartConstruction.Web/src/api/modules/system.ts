// =============================================
// 系统信息相关API
// =============================================
import { http } from '@/utils/http'
import type { ApiResponse } from '@/types/global'

// 系统信息接口
export interface SystemInfo {
  version: string
  environment: string
  serverTime: string
  uptime: string
  features: string[]
  settings: Record<string, any>
  stats: {
    cpuUsage: number
    memoryUsage: number
    diskUsage: number
    userCount: number
    tenantCount: number
    activeUsers: number
  }
}

/**
 * 获取系统信息
 */
export function getSystemInfo() {
  return http.get<ApiResponse<SystemInfo>>('/api/system/info')
}

/**
 * 获取系统设置
 */
export function getSystemSettings() {
  return http.get<ApiResponse<Record<string, any>>>('/api/system/settings')
}

/**
 * 更新系统设置
 */
export function updateSystemSettings(data: Record<string, any>) {
  return http.put<ApiResponse<void>>('/api/system/settings', data)
}

/**
 * 获取系统资源使用情况
 */
export function getSystemResources() {
  return http.get<ApiResponse<{
    cpu: {
      usage: number
      cores: number
    }
    memory: {
      total: number
      used: number
      free: number
      usage: number
    }
    disk: {
      total: number
      used: number
      free: number
      usage: number
    }
  }>>('/api/system/resources')
}

/**
 * 清理系统缓存
 */
export function clearSystemCache(type?: string) {
  return http.post<ApiResponse<{
    success: boolean
    clearedItems: number
  }>>('/api/system/clear-cache', { type })
}

/**
 * 系统健康检查
 */
export function checkSystemHealth() {
  return http.get<ApiResponse<{
    status: 'healthy' | 'unhealthy' | 'degraded'
    checks: Array<{
      name: string
      status: 'passed' | 'failed' | 'warning'
      message?: string
    }>
  }>>('/api/system/health')
}

/**
 * 获取系统日志
 */
export function getSystemLogs(params: {
  level?: 'info' | 'warning' | 'error' | 'debug'
  startDate?: string
  endDate?: string
  pageIndex: number
  pageSize: number
}) {
  return http.get<ApiResponse<{
    items: Array<{
      id: string
      timestamp: string
      level: string
      message: string
      source: string
      details?: any
    }>
    total: number
  }>>('/api/system/logs', { params })
}

/**
 * 初始化系统数据
 */
export function initSystemData() {
  return http.post<ApiResponse<{
    success: boolean
    message: string
  }>>('/api/system/init-data')
}