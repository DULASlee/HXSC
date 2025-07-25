import { http } from '@/utils/http'
import type { User, PagedResult, Role } from '@/types/global'

/**
 * 获取用户列表
 */
export function getUsers(params?: any) {
  return http.get<PagedResult<User>>('/api/users', { params })
}

/**
 * 获取用户详情
 */
export function getUser(id: string) {
  return http.get<User>(`/api/users/${id}`)
}

/**
 * 创建用户
 */
export function createUser(data: Partial<User>) {
  return http.post<User>('/api/users', data)
}

/**
 * 更新用户
 */
export function updateUser(id: string, data: Partial<User>) {
  return http.put<User>(`/api/users/${id}`, data)
}

/**
 * 删除用户
 */
export function deleteUser(id: string) {
  return http.delete<void>(`/api/users/${id}`)
}

/**
 * 重置密码
 */
export function resetPassword(id: string, data: { password: string }) {
  return http.post<void>(`/api/users/${id}/reset-password`, data)
}

/**
 * 分配角色
 */
export function assignRoles(id: string, roleIds: string[]) {
  return http.put<void>(`/api/users/${id}/roles`, { roleIds })
}
