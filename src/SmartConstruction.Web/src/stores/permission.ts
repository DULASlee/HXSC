// =============================================
// 权限状态管理
// =============================================
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { useUserStore } from './user'

export const usePermissionStore = defineStore('permission', () => {
  // 状态
  const permissions = ref<string[]>([])
  const roles = ref<string[]>([])

  // 计算属性
  const hasPermissions = computed(() => permissions.value.length > 0)
  const hasRoles = computed(() => roles.value.length > 0)

  // 从用户store初始化权限数据
  function initFromUserStore() {
    const userStore = useUserStore()
    permissions.value = userStore.permissions
    roles.value = userStore.roles
  }

  // 检查是否有指定权限
  function hasPermission(permission: string): boolean {
    if (!permission) return true
    return permissions.value.includes(permission)
  }

  // 检查是否有任意一个权限
  function hasAnyPermission(permissionList: string[]): boolean {
    return permissionList.some(permission => hasPermission(permission))
  }

  // 检查是否有所有权限
  function hasAllPermissions(permissionList: string[]): boolean {
    return permissionList.every(permission => hasPermission(permission))
  }

  // 检查是否有指定角色
  function hasRole(role: string): boolean {
    if (!role) return true
    return roles.value.includes(role)
  }

  // 检查是否有任意一个角色
  function hasAnyRole(roleList: string[]): boolean {
    return roleList.some(role => hasRole(role))
  }

  // 检查是否有所有角色
  function hasAllRoles(roleList: string[]): boolean {
    return roleList.every(role => hasRole(role))
  }

  // 设置权限
  function setPermissions(newPermissions: string[]) {
    permissions.value = newPermissions
  }

  // 设置角色
  function setRoles(newRoles: string[]) {
    roles.value = newRoles
  }

  // 清空权限数据
  function clearPermissions() {
    permissions.value = []
    roles.value = []
  }

  return {
    // 状态
    permissions,
    roles,
    
    // 计算属性
    hasPermissions,
    hasRoles,
    
    // 操作
    initFromUserStore,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    hasRole,
    hasAnyRole,
    hasAllRoles,
    setPermissions,
    setRoles,
    clearPermissions
  }
})