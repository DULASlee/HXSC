// =============================================
// 角色相关API
// =============================================
import { http } from '@/utils/http'
import type { ApiResponse, PagedResult, Role } from '@/types/global'

// 角色查询请求参数
export interface RoleQueryRequest {
  keyword?: string
  status?: number
  pageIndex: number
  pageSize: number
}

// 创建角色请求参数
export interface CreateRoleRequest {
  name: string
  code: string
  description?: string
  dataScope: string
  status: number
}

// 更新角色请求参数
export interface UpdateRoleRequest {
  name: string
  description?: string
  dataScope: string
  status: number
}

/**
 * 获取角色列表
 */
export function getRoleList(params: RoleQueryRequest) {
  return http.get<ApiResponse<PagedResult<Role>>>('/api/roles', { params })
}

/**
 * 获取角色详情
 */
export function getRoleById(id: string) {
  return http.get<ApiResponse<Role>>(`/api/roles/${id}`)
}

/**
 * 创建角色
 */
export function createRole(data: CreateRoleRequest) {
  return http.post<ApiResponse<Role>>('/api/roles', data)
}

/**
 * 更新角色
 */
export function updateRole(id: string, data: UpdateRoleRequest) {
  return http.put<ApiResponse<Role>>(`/api/roles/${id}`, data)
}

/**
 * 删除角色
 */
export function deleteRole(id: string) {
  return http.delete<ApiResponse<void>>(`/api/roles/${id}`)
}

/**
 * 启用/禁用角色
 */
export function toggleRoleStatus(id: string, status: number) {
  return http.put<ApiResponse<void>>(`/api/roles/${id}/status`, { status })
}

/**
 * 获取角色权限
 */
export function getRolePermissions(id: string) {
  return http.get<ApiResponse<string[]>>(`/api/roles/${id}/permissions`)
}

/**
 * 分配角色权限
 */
export function assignRolePermissions(id: string, permissionIds: string[]) {
  return http.post<ApiResponse<void>>(`/api/roles/${id}/permissions`, { permissionIds })
}

/**
 * 获取所有角色
 */
export function getAllRoles() {
  return http.get<ApiResponse<Role[]>>('/api/roles/all')
}

/**
 * 获取用户的角色
 */
export function getRolesByUserId(userId: string) {
  return http.get<ApiResponse<Role[]>>(`/api/users/${userId}/roles`)
}

/**
 * 为用户分配角色
 */
export function assignRolesToUser(userId: string, roleIds: string[]) {
  return http.post<ApiResponse<void>>(`/api/users/${userId}/roles`, { roleIds })
}