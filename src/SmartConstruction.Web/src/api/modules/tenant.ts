import { http } from '@/utils/http'
import type { ApiResponse, Tenant, PagedResult, TenantQueryRequest, CreateTenantRequest, UpdateTenantRequest, TenantDetailDto } from '@/types/global'

// 用户租户响应
export interface UserTenantsResponse {
  tenants: Tenant[]
  recentTenants: Tenant[]
}

/**
 * 获取租户列表
 */
export function getTenantList(params: TenantQueryRequest) {
  return http.get<ApiResponse<PagedResult<Tenant>>>('/api/tenant', { params })
}

/**
 * 获取租户详情
 */
export function getTenantDetail(id: string) {
  return http.get<ApiResponse<TenantDetailDto>>(`/api/tenant/${id}`)
}

/**
 * 创建租户
 */
export function createTenant(data: CreateTenantRequest) {
  return http.post<ApiResponse<Tenant>>('/api/tenant', data)
}

/**
 * 更新租户
 */
export function updateTenant(id: string, data: UpdateTenantRequest) {
  return http.put<ApiResponse<Tenant>>(`/api/tenant/${id}`, data)
}

/**
 * 删除租户
 */
export function deleteTenant(id: string) {
  return http.delete<ApiResponse<void>>(`/api/tenant/${id}`)
}

/**
 * 启用/禁用租户
 */
export function toggleTenantStatus(id: string, status: number) {
  return http.put<ApiResponse<void>>(`/api/tenant/${id}/status`, { status })
}

/**
 * 获取用户可访问的租户列表
 */
export function getUserTenants() {
  return http.get<ApiResponse<UserTenantsResponse>>('/api/tenant/user-tenants')
}

/**
 * 切换租户
 */
export function switchTenant(tenantId: string) {
  return http.post<ApiResponse<{
    tenant: Tenant
    token: {
      accessToken: string
      refreshToken: string
      expiresIn: number
    }
  }>>('/api/tenant/switch', { tenantId })
}

/**
 * 获取租户统计信息
 */
export function getTenantStats(id: string) {
  return http.get<ApiResponse<{
    userCount: number
    roleCount: number
    organizationCount: number
    menuCount: number
    resourceCount: number
    storageUsage: number
    requestCount: number
    lastActiveTime: string
  }>>(`/api/tenant/${id}/stats`)
}

/**
 * 获取租户配置
 */
export function getTenantConfig(id: string) {
  return http.get<ApiResponse<{
    theme: string
    logo: string
    features: string[]
    settings: Record<string, any>
  }>>(`/api/tenant/${id}/config`)
}

/**
 * 更新租户配置
 */
export function updateTenantConfig(id: string, data: {
  theme?: string
  logo?: string
  features?: string[]
  settings?: Record<string, any>
}) {
  return http.put<ApiResponse<void>>(`/api/tenant/${id}/config`, data)
}

/**
 * 上传租户Logo
 */
export function uploadTenantLogo(id: string, file: File) {
  const formData = new FormData()
  formData.append('logo', file)
  
  return http.post<ApiResponse<{ url: string }>>(`/api/tenant/${id}/logo`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 初始化租户数据
 */
export function initTenantData(id: string, data: {
  includeDemo?: boolean
  adminUser?: {
    username: string
    email: string
    password: string
  }
}) {
  return http.post<ApiResponse<void>>(`/api/tenant/${id}/init`, data)
}

/**
 * 备份租户数据
 */
export function backupTenantData(id: string) {
  return http.post<ApiResponse<{
    backupId: string
    downloadUrl: string
  }>>(`/api/tenant/${id}/backup`)
}

/**
 * 恢复租户数据
 */
export function restoreTenantData(id: string, backupFile: File) {
  const formData = new FormData()
  formData.append('backup', backupFile)
  
  return http.post<ApiResponse<void>>(`/api/tenant/${id}/restore`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 获取租户审计日志
 */
export function getTenantAuditLogs(id: string, params: {
  startDate?: string
  endDate?: string
  action?: string
  userId?: string
  pageIndex: number
  pageSize: number
}) {
  return http.get<ApiResponse<PagedResult<{
    id: string
    action: string
    resource: string
    userId: string
    userName: string
    ipAddress: string
    userAgent: string
    details: string
    createdAt: string
  }>>>(`/api/tenant/${id}/audit-logs`, { params })
}