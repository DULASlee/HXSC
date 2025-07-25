/**
 * 布局验证工具
 * 用于验证路由配置和菜单结构
 */
import type { RouteRecordRaw } from 'vue-router'
import type { Menu } from '@/types/global'

/**
 * 验证路由配置
 * @param routes 路由配置
 * @returns 验证结果
 */
export function validateRoutes(routes: RouteRecordRaw[]): { valid: boolean; issues: string[] } {
  const issues: string[] = []
  
  // 检查路由是否为空
  if (!routes || routes.length === 0) {
    issues.push('路由配置为空')
    return { valid: false, issues }
  }
  
  // 递归检查路由配置
  function checkRoute(route: RouteRecordRaw, parentPath = '') {
    // 检查路由路径
    if (!route.path) {
      issues.push(`路由缺少路径: ${JSON.stringify(route)}`)
    }
    
    // 检查路由名称
    if (!route.name && !route.redirect) {
      issues.push(`路由缺少名称: ${route.path}`)
    }
    
    // 检查路由组件
    if (!route.component && !route.redirect && !route.children) {
      issues.push(`路由缺少组件: ${route.path}`)
    }
    
    // 检查路由元数据
    if (!route.meta) {
      issues.push(`路由缺少元数据: ${route.path}`)
    }
    
    // 检查路由路径是否重复
    const fullPath = route.path.startsWith('/')
      ? route.path
      : `${parentPath}/${route.path}`.replace(/\/+/g, '/')
    
    // 递归检查子路由
    if (route.children && route.children.length > 0) {
      route.children.forEach(child => checkRoute(child, fullPath))
    }
  }
  
  // 检查所有路由
  routes.forEach(route => checkRoute(route))
  
  return {
    valid: issues.length === 0,
    issues
  }
}

/**
 * 验证菜单结构
 * @param menus 菜单配置
 * @returns 验证结果
 */
export function validateMenus(menus: Menu[]): { valid: boolean; issues: string[] } {
  const issues: string[] = []
  
  // 检查菜单是否为空
  if (!menus || menus.length === 0) {
    issues.push('菜单配置为空')
    return { valid: false, issues }
  }
  
  // 递归检查菜单配置
  function checkMenu(menu: Menu) {
    // 检查菜单ID
    if (!menu.id) {
      issues.push(`菜单缺少ID: ${JSON.stringify(menu)}`)
    }
    
    // 检查菜单名称
    if (!menu.name) {
      issues.push(`菜单缺少名称: ${menu.id}`)
    }
    
    // 检查菜单路径
    if (menu.type === 'Menu' && !menu.path) {
      issues.push(`菜单类型为"Menu"但缺少路径: ${menu.id}`)
    }
    
    // 检查菜单组件
    if (menu.type === 'Menu' && !menu.component) {
      issues.push(`菜单类型为"Menu"但缺少组件: ${menu.id}`)
    }
    
    // 递归检查子菜单
    if (menu.children && menu.children.length > 0) {
      menu.children.forEach(child => checkMenu(child))
    }
  }
  
  // 检查所有菜单
  menus.forEach(menu => checkMenu(menu))
  
  return {
    valid: issues.length === 0,
    issues
  }
}

/**
 * 验证菜单与路由的一致性
 * @param menus 菜单配置
 * @param routes 路由配置
 * @returns 验证结果
 */
export function validateMenuRouteConsistency(
  menus: Menu[],
  routes: RouteRecordRaw[]
): { valid: boolean; issues: string[] } {
  const issues: string[] = []
  
  // 提取所有路由路径
  const routePaths = new Set<string>()
  
  function extractRoutePaths(route: RouteRecordRaw, parentPath = '') {
    const fullPath = route.path.startsWith('/')
      ? route.path
      : `${parentPath}/${route.path}`.replace(/\/+/g, '/')
    
    routePaths.add(fullPath)
    
    if (route.children && route.children.length > 0) {
      route.children.forEach(child => extractRoutePaths(child, fullPath))
    }
  }
  
  routes.forEach(route => extractRoutePaths(route))
  
  // 检查菜单路径是否存在对应的路由
  function checkMenuPath(menu: Menu) {
    if (menu.type === 'Menu' && menu.path) {
      // 忽略外部链接
      if (!menu.isExternal && !routePaths.has(menu.path)) {
        issues.push(`菜单路径在路由中不存在: ${menu.path}`)
      }
    }
    
    if (menu.children && menu.children.length > 0) {
      menu.children.forEach(child => checkMenuPath(child))
    }
  }
  
  menus.forEach(menu => checkMenuPath(menu))
  
  return {
    valid: issues.length === 0,
    issues
  }
}

// 导出验证工具集
export const layoutValidator = {
  validateRoutes,
  validateMenus,
  validateMenuRouteConsistency
}

export default layoutValidator