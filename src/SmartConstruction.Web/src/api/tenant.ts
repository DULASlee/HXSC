import { http } from '@/utils/http'
import type { Tenant, TenantDetail, PagedResult } from '@/types/global'

/**
 * 获取租户列表
 */
export function getTenants(params?: any) {
  return http.get<PagedResult<Tenant>>('/api/tenants', { params })
}

/**
 * 获取租户详情
 */
export function getTenant(id: string) {
  return http.get<TenantDetail>(`/api/tenants/${id}`)
}

/**
 * 创建租户
 */
export function createTenant(data: Partial<Tenant>) {
  return http.post<Tenant>('/api/tenants', data)
}

/**
 * 更新租户
 */
export function updateTenant(id: string, data: Partial<Tenant>) {
  return http.put<Tenant>(`/api/tenants/${id}`, data)
}

/**
 * 删除租户
 */
export function deleteTenant(id: string) {
  return http.delete<void>(`/api/tenants/${id}`)
}

/**

 * 获取租户连接字符串等隔离信息
 * (假设这是一个单独的敏感接口)
 */
export function getTenantIsolation(tenantId: string) {
  return http.get<any>(`/api/tenants/${tenantId}/isolation`)
}

/**
 * 更新租户隔离信息
 */
export function updateTenantIsolation(tenantId: string, data: any) {
  return http.put<any>(`/api/tenants/${tenantId}/isolation`, data)
}
