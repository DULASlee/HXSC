import { http } from '@/utils/http'
import type { Role, PagedResult } from '@/types/global'

/**
 * 获取角色列表
 */
export function getRoles(params?: any) {
  return http.get<PagedResult<Role>>('/api/roles', { params })
}

/**
 * 获取角色详情
 */
export function getRole(id: string) {
  return http.get<Role>(`/api/roles/${id}`)
}

/**
 * 创建角色
 */
export function createRole(data: Partial<Role>) {
  return http.post<Role>('/api/roles', data)
}

/**
 * 更新角色
 */
export function updateRole(id: string, data: Partial<Role>) {
  return http.put<Role>(`/api/roles/${id}`, data)
}

/**
 * 删除角色
 */
export function deleteRole(id: string) {
  return http.delete<void>(`/api/roles/${id}`)
}

/**
 * 分配权限
 */
export function assignPermissions(id: string, permissions: string[]) {
  return http.put<void>(`/api/roles/${id}/permissions`, { permissions })
}
