import { http } from '@/utils/http'
import type { ApiResponse, User, PagedResult } from '@/types/global'

// 用户查询请求参数
export interface UserQueryRequest {
  keyword?: string
  organizationId?: string
  roleIds?: string[]
  status?: number
  ids?: string[]
  pageIndex: number
  pageSize: number
}

// 用户搜索请求参数
export interface UserSearchRequest {
  keyword: string
  organizationId?: string
  roleIds?: string[]
  status?: number
  pageIndex: number
  pageSize: number
}

// 创建用户请求参数
export interface CreateUserRequest {
  username: string
  email?: string
  mobile?: string
  displayName: string
  organizationId?: string
  roleIds?: string[]
  password?: string
}

// 更新用户请求参数
export interface UpdateUserRequest {
  email?: string
  mobile?: string
  displayName: string
  organizationId?: string
  roleIds?: string[]
}

// 修改密码请求参数
export interface ChangePasswordRequest {
  oldPassword: string
  newPassword: string
}

// 重置密码请求参数
export interface ResetPasswordRequest {
  userId: string
  newPassword: string
}

/**
 * 获取用户列表
 */
export function getUserList(params: UserQueryRequest) {
  return http.get<ApiResponse<PagedResult<User>>>('/api/users', { params })
}

/**
 * 搜索用户
 */
export function searchUsers(params: UserSearchRequest) {
  return http.get<ApiResponse<PagedResult<User>>>('/api/users/search', { params })
}

/**
 * 获取用户详情
 */
export function getUserDetail(id: string) {
  return http.get<ApiResponse<User>>(`/api/users/${id}`)
}

/**
 * 创建用户
 */
export function createUser(data: CreateUserRequest) {
  return http.post<ApiResponse<User>>('/api/users', data)
}

/**
 * 更新用户
 */
export function updateUser(id: string, data: UpdateUserRequest) {
  return http.put<ApiResponse<User>>(`/api/users/${id}`, data)
}

/**
 * 删除用户
 */
export function deleteUser(id: string) {
  return http.delete<ApiResponse<void>>(`/api/users/${id}`)
}

/**
 * 启用/禁用用户
 */
export function toggleUserStatus(id: string, status: number) {
  return http.put<ApiResponse<void>>(`/api/users/${id}/status`, { status })
}

/**
 * 修改密码
 */
export function changePassword(data: ChangePasswordRequest) {
  return http.put<ApiResponse<void>>('/api/users/change-password', data)
}

/**
 * 重置密码
 */
export function resetPassword(data: ResetPasswordRequest) {
  return http.put<ApiResponse<void>>('/api/users/reset-password', data)
}

/**
 * 获取用户角色
 */
export function getUserRoles(id: string) {
  return http.get<ApiResponse<any[]>>(`/api/users/${id}/roles`)
}

/**
 * 设置用户角色
 */
export function setUserRoles(id: string, roleIds: string[]) {
  return http.put<ApiResponse<void>>(`/api/users/${id}/roles`, { roleIds })
}

/**
 * 获取用户权限
 */
export function getPermissionsByUserId(id: string) {
  return http.get<ApiResponse<string[]>>(`/api/users/${id}/permissions`)
}

/**
 * 上传用户头像
 */
export function uploadUserAvatar(id: string, file: File) {
  const formData = new FormData()
  formData.append('avatar', file)
  
  return http.post<ApiResponse<{ url: string }>>(`/api/users/${id}/avatar`, formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 获取当前用户信息
 */
export function getCurrentUser() {
  return http.get<ApiResponse<User>>('/api/auth/me')
}

/**
 * 更新当前用户信息
 */
export function updateCurrentUser(data: Partial<UpdateUserRequest>) {
  return http.put<ApiResponse<User>>('/api/users/current', data)
}

/**
 * 批量导入用户
 */
export function importUsers(file: File) {
  const formData = new FormData()
  formData.append('file', file)
  
  return http.post<ApiResponse<{
    success: number
    failed: number
    errors: string[]
  }>>('/api/users/import', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  })
}

/**
 * 导出用户
 */
export function exportUsers(params?: UserQueryRequest) {
  return http.get('/api/users/export', {
    params,
    responseType: 'blob'
  })
}