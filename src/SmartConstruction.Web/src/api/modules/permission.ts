// =============================================
// 权限相关API
// =============================================
import { http } from '@/utils/http'
import type { ApiResponse } from '@/types/global'

// 权限树节点
export interface PermissionTreeNode {
  id: string
  name: string
  code: string
  type: 'Menu' | 'Button' | 'API'
  children?: PermissionTreeNode[]
}

/**
 * 获取权限树
 */
export function getPermissionTree() {
  return http.get<ApiResponse<PermissionTreeNode[]>>('/api/permissions/tree')
}

/**
 * 获取用户权限
 */
export function getUserPermissions() {
  return http.get<ApiResponse<string[]>>('/api/permissions/user')
}

/**
 * 检查权限
 */
export function checkPermission(permissionCode: string) {
  return http.post<ApiResponse<boolean>>('/api/permissions/check', { permissionCode })
}

/**
 * 检查多个权限（任意一个）
 */
export function checkAnyPermission(permissionCodes: string[]) {
  return http.post<ApiResponse<boolean>>('/api/permissions/check-any', { permissionCodes })
}

/**
 * 检查多个权限（全部）
 */
export function checkAllPermissions(permissionCodes: string[]) {
  return http.post<ApiResponse<boolean>>('/api/permissions/check-all', { permissionCodes })
}