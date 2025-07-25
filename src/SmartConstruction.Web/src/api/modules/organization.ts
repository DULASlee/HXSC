import { http } from '@/utils/http'
import type { ApiResponse, Organization, PagedResult } from '@/types/global'

// 组织查询请求参数
export interface OrganizationQueryRequest {
  keyword?: string
  type?: string
  parentId?: string
  status?: number
  pageIndex: number
  pageSize: number
}

// 组织树查询请求参数
export interface OrganizationTreeRequest {
  includeUsers?: boolean
  types?: string[]
  status?: number
}

// 创建组织请求参数
export interface CreateOrganizationRequest {
  parentId?: string
  code: string
  name: string
  type: 'Company' | 'Department' | 'Team' | 'Group'
  managerName?: string
  description?: string
  sortOrder?: number
}

// 更新组织请求参数
export interface UpdateOrganizationRequest {
  code: string
  name: string
  managerName?: string
  description?: string
  sortOrder?: number
}

/**
 * 获取组织列表
 */
export function getOrganizationList(params: OrganizationQueryRequest) {
  return http.get<ApiResponse<PagedResult<Organization>>>('/api/organizations', { params })
}

/**
 * 获取组织树
 */
export function getOrganizationTree(params?: OrganizationTreeRequest) {
  return http.get<ApiResponse<Organization[]>>('/api/organizations/tree', { params })
}

/**
 * 获取组织详情
 */
export function getOrganizationDetail(id: string) {
  return http.get<ApiResponse<Organization>>(`/api/organizations/${id}`)
}

/**
 * 创建组织
 */
export function createOrganization(data: CreateOrganizationRequest) {
  return http.post<ApiResponse<Organization>>('/api/organizations', data)
}

/**
 * 更新组织
 */
export function updateOrganization(id: string, data: UpdateOrganizationRequest) {
  return http.put<ApiResponse<Organization>>(`/api/organizations/${id}`, data)
}

/**
 * 删除组织
 */
export function deleteOrganization(id: string) {
  return http.delete<ApiResponse<void>>(`/api/organizations/${id}`)
}

/**
 * 移动组织
 */
export function moveOrganization(id: string, targetParentId?: string) {
  return http.put<ApiResponse<void>>(`/api/organizations/${id}/move`, {
    targetParentId
  })
}

/**
 * 获取组织成员
 */
export function getOrganizationMembers(id: string, params?: {
  keyword?: string
  pageIndex: number
  pageSize: number
}) {
  return http.get<ApiResponse<PagedResult<any>>>(`/api/organizations/${id}/members`, { params })
}

/**
 * 添加组织成员
 */
export function addOrganizationMember(id: string, userIds: string[]) {
  return http.post<ApiResponse<void>>(`/api/organizations/${id}/members`, {
    userIds
  })
}

/**
 * 移除组织成员
 */
export function removeOrganizationMember(id: string, userId: string) {
  return http.delete<ApiResponse<void>>(`/api/organizations/${id}/members/${userId}`)
}

/**
 * 设置组织管理员
 */
export function setOrganizationManager(id: string, userId: string) {
  return http.put<ApiResponse<void>>(`/api/organizations/${id}/manager`, {
    userId
  })
}