// =============================================
// 权限相关组合式函数
// =============================================
import { computed } from 'vue'
import { useUserStore } from '@/stores/user'
import { PermissionService } from '@/services/permission'

export function usePermission() {
  const userStore = useUserStore()

  // 当前用户信息
  const currentUser = computed(() => userStore.userInfo)
  const currentTenant = computed(() => userStore.currentTenant)
  const userPermissions = computed(() => userStore.permissions)
  const userRoles = computed(() => userStore.roles)
  const dataScope = computed(() => userStore.dataScope)

  // 权限检查（本地缓存）
  const hasPermission = (permission: string): boolean => {
    return PermissionService.hasPermissionLocal(permission, userPermissions.value)
  }

  // 角色检查（本地缓存）
  const hasRole = (role: string): boolean => {
    return PermissionService.hasRoleLocal(role, userRoles.value)
  }

  // 检查是否拥有任意一个权限
  const hasAnyPermission = (permissions: string[]): boolean => {
    return permissions.some(permission => hasPermission(permission))
  }

  // 检查是否拥有所有权限
  const hasAllPermissions = (permissions: string[]): boolean => {
    return permissions.every(permission => hasPermission(permission))
  }

  // 检查是否拥有任意一个角色
  const hasAnyRole = (roles: string[]): boolean => {
    return roles.some(role => hasRole(role))
  }

  // 检查是否拥有所有角色
  const hasAllRoles = (roles: string[]): boolean => {
    return roles.every(role => hasRole(role))
  }

  // 异步权限检查（从服务器验证）
  const checkPermission = async (resourceCode: string, context?: Record<string, any>): Promise<boolean> => {
    return await userStore.checkPermission(resourceCode, context)
  }

  // 检查数据范围权限
  const checkDataScope = (targetUserId?: string, targetOrgId?: string): boolean => {
    return userStore.checkDataScope(targetUserId, targetOrgId)
  }

  // 管理员检查
  const isSystemAdmin = computed(() => userStore.isSystemAdmin())
  const isTenantAdmin = computed(() => userStore.isTenantAdmin())

  // 组织相关
  const organizationPath = computed(() => userStore.getOrganizationPath())
  const isInOrganization = (organizationId: string): boolean => {
    return userStore.isInOrganization(organizationId)
  }

  return {
    // 用户信息
    currentUser,
    currentTenant,
    userPermissions,
    userRoles,
    dataScope,
    
    // 权限检查
    hasPermission,
    hasRole,
    hasAnyPermission,
    hasAllPermissions,
    hasAnyRole,
    hasAllRoles,
    checkPermission,
    checkDataScope,
    
    // 管理员检查
    isSystemAdmin,
    isTenantAdmin,
    
    // 组织相关
    organizationPath,
    isInOrganization
  }
}