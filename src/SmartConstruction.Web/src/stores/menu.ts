import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { RouteRecordRaw } from 'vue-router'
// 使用惰性加载来避免循环依赖
const getDefaultLayout = () => import('@/layouts/DefaultLayout.vue')

// 菜单类型定义
export interface Menu {
  id: string
  code: string
  name: string
  type: string
  parentId?: string
  path?: string
  component?: string
  icon?: string
  permission?: string
  sortOrder: number
  isEnabled: boolean
  isVisible: boolean
  isKeepAlive: boolean
  isExternal: boolean
  createdAt: string
  children?: Menu[]
  fullPath?: string // 新增：用于存储后端计算好的完整路径
}

/// <summary>
/// 菜单状态管理Store
/// </summary>
export const useMenuStore = defineStore('menu', () => {
  // 状态
  const menus = ref<Menu[]>([])
  const routes = ref<RouteRecordRaw[]>([])
  const permissions = ref<string[]>([])
  const loading = ref(false)
  const isRoutesGenerated = ref(false)
  const isInitializing = ref(false)
  const lastUpdateTime = ref<number>(0)

  // 常量定义
  const CACHE_KEY = {
    MENU_DATA: 'smart_construction_menu_data',
    PERMISSIONS: 'smart_construction_permissions',
    ROUTES_GENERATED: 'smart_construction_routes_generated',
    LAST_UPDATE: 'smart_construction_menu_last_update'
  }
  
  const CACHE_EXPIRE_TIME = 30 * 60 * 1000 // 30分钟缓存过期

  // 计算属性
  const menuTree = computed(() => {
    // 直接返回从后端获取的、已经是树形的菜单数据
    return menus.value
  })

  const flatMenus = computed(() => {
    return flattenMenus(menus.value)
  })

  /// <summary>
  /// 扁平化菜单
  /// </summary>
  function flattenMenus(menuList: Menu[]): Menu[] {
    const result: Menu[] = []
    
    function traverse(menus: Menu[]) {
      menus.forEach(menu => {
        result.push(menu)
        if (menu.children && menu.children.length > 0) {
          traverse(menu.children)
        }
      })
    }
    
    traverse(menuList)
    return result
  }

  /**
   * @description [终极加固] 将菜单树递归转换为符合 vue-router 标准的路由树
   * @param {Menu[]} menuList - 菜单项数组
   * @returns {RouteRecordRaw[]} - vue-router 路由记录数组
   */
  function menusToRoutes(menuList: Menu[]): RouteRecordRaw[] {
    const generatedRoutes: RouteRecordRaw[] = [];

    menuList.forEach(menu => {
      // 1. 数据校验：跳过无效菜单项
      if (!menu.path || !menu.name) {
        console.warn('跳过无效菜单项 (缺少 path 或 name):', menu.name);
        return;
      }
      
      const currentRoute: RouteRecordRaw = {
        path: menu.path,
        name: menu.name, // 使用 menu.name 作为唯一标识
        meta: {
          title: menu.name,
          icon: menu.icon,
          isKeepAlive: menu.isKeepAlive,
          permission: menu.permission,
        },
        // 初始化 children 为空数组，符合 RouteRecordRaw 类型
        children: [], 
      };

      // 2. 处理组件
      const isParent = menu.children && menu.children.length > 0;
      const componentPath = menu.component;

      if (componentPath) {
        currentRoute.component = resolveComponent(componentPath);
      } else if (isParent) {
        // If no component field but has children, treat as a layout/container
        currentRoute.component = getDefaultLayout();
      }
      
      // 3. Recursively process child routes
      if (isParent) {
        currentRoute.children = menusToRoutes(menu.children!);
      }

      generatedRoutes.push(currentRoute);
    });

    return generatedRoutes;
  }

  /// <summary>
  /// 解析组件路径 - 增强容错处理
  /// </summary>
  const views = import.meta.glob('@/views/**/*.vue')

  function resolveComponent(componentPath: string) {
    if (!componentPath) {
      console.warn('resolveComponent: componentPath 为空')
      return getDefaultLayout
    }

    // 如果菜单配置的是 "Layout"，直接返回主布局组件
    if (componentPath.toLowerCase() === 'layout') {
      return getDefaultLayout
    }

    // 去掉开头的 @ 或 /src，标准化路径
    let cleanPath = componentPath
      .replace(/^@?\/src\//, '')
      .replace(/^@?\/views\//, '')
      .replace(/^views\//, '')
      .replace(/\.vue$/, '')

    // 尝试多种路径组合
    const possiblePaths = [
      `@/views/${cleanPath}.vue`,
      `@/views/${cleanPath}/index.vue`,
      `/src/views/${cleanPath}.vue`,
      `/src/views/${cleanPath}/index.vue`
    ]

    for (const path of possiblePaths) {
      const component = views[path]
      if (component) {
        console.log(`组件解析成功: ${componentPath} -> ${path}`)
        return component
      }
    }

    console.error(`Component not found for path: ${componentPath}, tried:`, possiblePaths)
    // 返回占位符组件而非404，防止页面完全崩溃
    return () => import('@/components/PlaceholderView.vue').catch(() => 
      ({ template: '<div class="placeholder-view">组件加载失败：{{componentPath}}</div>', data: () => ({ componentPath }) })
    )
  }

  /// <summary>
  /// 保存菜单数据到本地存储
  /// </summary>
  function saveMenuDataToCache(menuData: Menu[], permissionData: string[]) {
    try {
      const cacheData = {
        menus: menuData,
        permissions: permissionData,
        timestamp: Date.now(),
        version: '1.0'
      }
      
      localStorage.setItem(CACHE_KEY.MENU_DATA, JSON.stringify(cacheData))
      localStorage.setItem(CACHE_KEY.ROUTES_GENERATED, JSON.stringify(isRoutesGenerated.value))
      localStorage.setItem(CACHE_KEY.LAST_UPDATE, Date.now().toString())
      
      console.log('菜单数据已缓存到本地存储')
    } catch (error) {
      console.error('保存菜单数据到缓存失败:', error)
    }
  }

  /// <summary>
  /// 从本地存储加载菜单数据
  /// </summary>
  function loadMenuDataFromCache(): { menus: Menu[], permissions: string[] } | null {
    try {
      const cachedData = localStorage.getItem(CACHE_KEY.MENU_DATA)
      const routesGenerated = localStorage.getItem(CACHE_KEY.ROUTES_GENERATED)
      const lastUpdate = localStorage.getItem(CACHE_KEY.LAST_UPDATE)
      
      if (!cachedData) return null
      
      const parsedData = JSON.parse(cachedData)
      const now = Date.now()
      
      // 检查缓存是否过期
      if (parsedData.timestamp && (now - parsedData.timestamp) > CACHE_EXPIRE_TIME) {
        console.log('菜单缓存已过期，将重新获取')
        clearMenuCache()
        return null
      }
      
      // 验证数据完整性
      if (!parsedData.menus || !Array.isArray(parsedData.menus)) {
        console.warn('缓存的菜单数据格式错误')
        clearMenuCache()
        return null
      }
      
      // 恢复路由生成状态
      if (routesGenerated) {
        isRoutesGenerated.value = JSON.parse(routesGenerated)
      }
      
      // 恢复最后更新时间
      if (lastUpdate) {
        lastUpdateTime.value = parseInt(lastUpdate)
      }
      
      console.log('成功从缓存加载菜单数据')
      return {
        menus: parsedData.menus,
        permissions: parsedData.permissions || []
      }
    } catch (error) {
      console.error('从缓存加载菜单数据失败:', error)
      clearMenuCache()
      return null
    }
  }

  /// <summary>
  /// 清除菜单缓存
  /// </summary>
  function clearMenuCache() {
    Object.values(CACHE_KEY).forEach(key => {
      localStorage.removeItem(key)
    })
    console.log('菜单缓存已清除')
  }

  /**
   * @description [重构] 处理菜单数据，生成路由并更新状态
   * @param {Menu[]} menuData - 从API获取的原始菜单数据
   * @param {string[]} permissionData - 从API获取的权限数据
   */
  function processAndSetMenus(menuData: Menu[], permissionData: string[]) {
    if (!Array.isArray(menuData)) {
      console.error('processAndSetMenus 接收到的菜单数据格式错误');
      menus.value = [];
      permissions.value = [];
      routes.value = [];
      isRoutesGenerated.value = false;
      return { menus: [], routes: [], permissions: [] };
    }
    
    menus.value = menuData;
    permissions.value = permissionData || [];
    routes.value = menusToRoutes(menus.value);
    lastUpdateTime.value = Date.now();
    isRoutesGenerated.value = true; // 关键：在这里设置状态

    // 保存到缓存
    saveMenuDataToCache(menuData, permissions.value);

    console.log('✅ [MenuStore] 菜单和路由处理完毕');
    
    return {
      menus: menus.value,
      routes: routes.value,
      permissions: permissions.value
    };
  }

  /// <summary>
  /// 获取用户菜单 - [重构] 简化职责，仅作为获取数据的入口
  /// </summary>
  async function fetchUserMenus() {
    // 该函数现在是获取菜单数据的唯一入口，它总是尝试从API获取最新数据。
    // 缓存和预加载逻辑已移出，由调用方（如路由守卫）按需处理，以简化Store内部状态。
    loading.value = true;
    try {
      return await fetchMenuDataFromApi();
    } catch (error) {
      console.error('获取用户菜单失败:', error);
      // 失败时，清除所有状态，避免脏数据
      clearMenus();
      throw error; // 将错误向上抛出，由调用方处理（如跳转到登录页）
    } finally {
      loading.value = false;
    }
  }

  /// <summary>
  /// 从API获取菜单数据 - [重构] 作为核心的API调用函数
  /// </summary>
  async function fetchMenuDataFromApi() {
    try {
      const { getUserMenus } = await import('@/api/modules/menu');
      const response = await getUserMenus();

      if (response && response.menus) {
        return processAndSetMenus(response.menus, response.permissions || []);
      } else {
        throw new Error('从API获取的菜单数据无效');
      }
    } catch (error) {
      console.error('从API获取菜单数据时出错:', error);
      throw error; // 向上抛出，由 fetchUserMenus 捕获
    }
  }

  /// <summary>
  /// 异步更新菜单数据（不阻塞UI）
  /// </summary>
  async function fetchMenuDataFromApiAsync() {
    try {
      const now = Date.now()
      
      // 如果上次更新时间距今超过5分钟，则更新
      if ((now - lastUpdateTime.value) > 5 * 60 * 1000) {
        console.log('开始后台更新菜单数据')
        await fetchMenuDataFromApi()
        console.log('菜单数据已在后台更新')
      }
    } catch (error) {
      console.warn('后台更新菜单数据失败', error)
    }
  }

  /// <summary>
  /// 检查权限
  /// </summary>
  function hasPermission(permission: string): boolean {
    if (!permission) return true
    return permissions.value.includes(permission)
  }

  /// <summary>
  /// 检查多个权限（或关系）
  /// </summary>
  function hasAnyPermission(permissionList: string[]): boolean {
    return permissionList.some(permission => hasPermission(permission))
  }

  /// <summary>
  /// 检查多个权限（且关系）
  /// </summary>
  function hasAllPermissions(permissionList: string[]): boolean {
    return permissionList.every(permission => hasPermission(permission))
  }

  /// <summary>
  /// 根据路径查找菜单
  /// </summary>
  function findMenuByPath(path: string): Menu | undefined {
    return flatMenus.value.find(menu => menu.path === path)
  }

  /// <summary>
  /// 根据权限过滤菜单
  /// </summary>
  function filterMenusByPermissions(menuList: Menu[]): Menu[] {
    return menuList.filter(menu => {
      // 如果菜单没有权限要求，则显示
      if (!menu.permission) {
        return true
      }
      
      // 检查是否有权限
      if (!hasPermission(menu.permission)) {
        return false
      }
      
      // 递归过滤子菜单
      if (menu.children && menu.children.length > 0) {
        menu.children = filterMenusByPermissions(menu.children)
      }
      
      return true
    })
  }

  /// <summary>
  /// 获取面包屑导航
  /// </summary>
  function getBreadcrumbs(path: string): Menu[] {
    const breadcrumbs: Menu[] = []
    
    function findPath(menuList: Menu[], targetPath: string, parents: Menu[] = []): boolean {
      for (const menu of menuList) {
        const currentPath = [...parents, menu]
        
        if (menu.path === targetPath) {
          breadcrumbs.push(...currentPath)
          return true
        }
        
        if (menu.children && menu.children.length > 0) {
          if (findPath(menu.children, targetPath, currentPath)) {
            return true
          }
        }
      }
      return false
    }
    
    findPath(menuTree.value, path)
    return breadcrumbs
  }

  /// <summary>
  /// 生成路由
  /// </summary>
  async function generateRoutes(menuList: Menu[] = []) {
    console.log('生成路由，菜单数据:', menuList)
    routes.value = menusToRoutes(menuList)
    isRoutesGenerated.value = true
    
    // 保存路由生成状态
    localStorage.setItem(CACHE_KEY.ROUTES_GENERATED, JSON.stringify(true))
    
    console.log('生成的路由:', routes.value)
    return routes.value
  }

  /// <summary>
  /// 生成默认路由（当没有菜单数据时）
  /// </summary>
  async function generateDefaultRoutes() {
    console.log('生成默认路由')
    
    const defaultMenus: Menu[] = [
      {
        id: '1',
        code: 'dashboard',
        name: '工作台',
        type: 'Menu',
        path: '/dashboard',
        component: 'dashboard/index',
        icon: 'HomeFilled',
        permission: '',
        sortOrder: 1,
        isEnabled: true,
        isVisible: true,
        isKeepAlive: false,
        isExternal: false,
        createdAt: new Date().toISOString()
      }
    ]

    return generateRoutes(defaultMenus)
  }

  /// <summary>
  /// 清空菜单和路由
  /// </summary>
  function clearMenus() {
    menus.value = []
    routes.value = []
    permissions.value = []
    isRoutesGenerated.value = false
    lastUpdateTime.value = 0
    
    // 清除缓存
    clearMenuCache()
    
    console.log('菜单数据已清除')
  }

  /// <summary>
  /// 设置路由生成状态
  /// </summary>
  function setRoutesGenerated(value: boolean) {
    isRoutesGenerated.value = value
    // 同步到localStorage
    localStorage.setItem(CACHE_KEY.ROUTES_GENERATED, JSON.stringify(value))
  }

  /// <summary>
  /// 刷新菜单数据
  /// </summary>
  async function refreshMenus() {
    console.log('手动刷新菜单数据')
    clearMenuCache()
    return await fetchMenuDataFromApi()
  }

  return {
    // 状态
    menus,
    routes,
    permissions,
    loading,
    isRoutesGenerated,
    isInitializing,
    lastUpdateTime,
    
    // 计算属性
    menuTree,
    flatMenus,
    
    // 操作
    fetchUserMenus,
    generateRoutes,
    generateDefaultRoutes,
    hasPermission,
    hasAnyPermission,
    hasAllPermissions,
    findMenuByPath,
    filterMenusByPermissions,
    getBreadcrumbs,
    clearMenus,
    refreshMenus,
    processAndSetMenus, // 导出新方法
    setRoutesGenerated, // 添加新方法

    // [移除] 不再需要从外部直接操作缓存
    // saveMenuDataToCache,
    // loadMenuDataFromCache,
    clearMenuCache
  }
})