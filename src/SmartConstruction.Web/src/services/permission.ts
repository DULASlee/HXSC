// =============================================
// 权限服务 - 对应后端 IPermissionService
// =============================================
import { http } from '@/utils/http'
import { getUserMenus as apiGetUserMenus } from '@/api/modules/menu'
import type { Menu } from '@/types/global'

export class PermissionService {
  // 检查单个权限
  static async checkPermission(resourceCode: string, context?: Record<string, any>): Promise<boolean> {
    try {
      const data = await http.post<boolean>('/permissions/check', {
        resourceCode,
        context
      })
      return data
    } catch (error) {
      console.error('检查权限失败:', error)
      return false
    }
  }

  // 获取用户所有权限
  static async getUserPermissions(): Promise<string[]> {
    try {
      const data = await http.get<string[]>('/permissions/user')
      return data
    } catch (error) {
      console.error('获取用户权限失败:', error)
      return []
    }
  }

  // 获取用户菜单
  static async getUserMenus(): Promise<Menu[]> {
    try {
      // const { data } = await request.get<Menu[]>('/permissions/menus')
      const response = await apiGetUserMenus()
      return response.menus
    } catch (error) {
      console.error('获取用户菜单失败:', error)
      return []
    }
  }

  // 检查是否拥有任意一个权限
  static async hasAnyPermission(resourceCodes: string[]): Promise<boolean> {
    try {
      const data = await http.post<boolean>('/permissions/check-any', {
        resourceCodes
      })
      return data
    } catch (error) {
      console.error('检查任意权限失败:', error)
      return false
    }
  }

  // 检查是否拥有所有权限
  static async hasAllPermissions(resourceCodes: string[]): Promise<boolean> {
    try {
      const data = await http.post<boolean>('/permissions/check-all', {
        resourceCodes
      })
      return data
    } catch (error) {
      console.error('检查所有权限失败:', error)
      return false
    }
  }

  // 本地权限检查（基于缓存的权限列表）
  static hasPermissionLocal(permission: string, userPermissions: string[]): boolean {
    if (!permission) return true
    return userPermissions.includes(permission)
  }

  // 本地角色检查
  static hasRoleLocal(role: string, userRoles: string[]): boolean {
    if (!role) return true
    return userRoles.includes(role)
  }

  // 检查数据范围权限
  static checkDataScope(dataScope: string, currentUserId: string, targetUserId?: string, targetOrgId?: string): boolean {
    switch (dataScope) {
      case 'All':
        return true
      case 'Organization':
        // 需要检查是否在同一组织
        return true // 简化实现，实际需要组织层级检查
      case 'Department':
        // 需要检查是否在同一部门
        return true // 简化实现，实际需要部门层级检查
      case 'Self':
        return currentUserId === targetUserId
      default:
        return false
    }
  }
}