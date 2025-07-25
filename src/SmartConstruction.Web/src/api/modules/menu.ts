// =============================================
// 菜单相关API
// =============================================
import { http } from '@/utils/http'
import type { ApiResponse, Menu } from '@/types/global'

// 用户菜单响应
export interface UserMenusResponse {
  menus: Menu[]
  permissions: string[]
}

/**
 * 获取用户菜单和权限
 */
export function getUserMenus() {
  return http.get<ApiResponse<UserMenusResponse>>('/api/auth/menus')
}

// 获取租户菜单列表
export function getTenantMenus() {
  return http.get<Menu[]>('/api/auth/menus')
}

// 获取菜单列表
export function getMenuList(params?: {
  tenantId?: string
  parentId?: string
  type?: string
  status?: number
}) {
  return http.get<PagedResult<Menu>>('/api/menus/list', { params })
}

// 获取菜单详情
export function getMenuById(id: string) {
  return http.get<Menu>(`/api/menus/${id}`)
}

// 创建菜单
export function createMenu(data: Partial<Menu>) {
  return http.post<Menu>('/api/menus', data)
}

// 更新菜单
export function updateMenu(id: string, data: Partial<Menu>) {
  return http.put<Menu>(`/api/menus/${id}`, data)
}

// 删除菜单
export function deleteMenu(id: string) {
  return http.delete(`/api/menus/${id}`)
}

// 获取菜单树
export function getMenuTree(tenantId?: string) {
  return http.get<Menu[]>('/api/menus/tree', { params: { tenantId } })
}

// 批量更新菜单排序
export function updateMenuSort(data: { id: string; sortOrder: number }[]) {
  return http.post('/api/menus/sort', { items: data })
}