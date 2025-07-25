// =============================================
// 仪表盘相关API
// =============================================
import { http } from '@/utils/http'
import type { ApiResponse } from '@/types/global'

// 仪表盘统计数据接口
export interface DashboardStats {
  userCount: number
  tenantCount: number
  roleCount: number
  menuCount: number
  systemInfo: {
    version: string
    environment: string
    serverTime: string
    uptime: string
    cpuUsage: number
    memoryUsage: number
    diskUsage: number
  }
  recentActivities: Array<{
    id: string
    title: string
    time: string
    type: string
    userId: string
    username: string
    ipAddress: string
  }>
}

/**
 * 获取仪表盘统计数据
 */
export function getDashboardStats() {
  return http.get<ApiResponse<DashboardStats>>('/api/dashboard/stats')
}

/**
 * 获取最近活动
 */
export function getRecentActivities(params?: {
  pageIndex?: number
  pageSize?: number
  startDate?: string
  endDate?: string
}) {
  return http.get<ApiResponse<{
    items: Array<{
      id: string
      title: string
      time: string
      type: string
      userId: string
      username: string
      ipAddress: string
    }>
    total: number
  }>>('/api/dashboard/activities', { params })
}

/**
 * 获取用户统计数据
 */
export function getUserStats() {
  return http.get<ApiResponse<{
    total: number
    active: number
    inactive: number
    newToday: number
    newThisWeek: number
    newThisMonth: number
  }>>('/api/dashboard/user-stats')
}