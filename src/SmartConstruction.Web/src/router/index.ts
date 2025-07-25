// =============================================
// 路由配置 - 多租户权限管理系统
// =============================================
import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'
import { useMenuStore } from '@/stores/menu'
import { useUserStore } from '@/stores/user'

const Layout = () => import('@/layouts/DefaultLayout.vue')

// 静态路由
export const constantRoutes: RouteRecordRaw[] = [
  {
    path: '/redirect',
    component: Layout,
    meta: { hidden: true },
    children: [
      {
        path: '/redirect/:path(.*)',
        component: () => import('@/views/redirect/index.vue')
      }
    ]
  },
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录', hidden: true }
  },
  {
    path: '/403',
    name: '403',
    component: () => import('@/views/error/403.vue'),
    meta: { title: '无权限', hidden: true }
  },
  {
    path: '/404',
    name: '404',
    component: () => import('@/views/error/404.vue'),
    meta: { title: '页面不存在', hidden: true }
  },
  {
    path: '/',
    name: 'Root',
    component: Layout,
    redirect: '/dashboard',
    children: [
      {
        path: 'dashboard',
        name: 'Dashboard',
        component: () => import('@/views/dashboard/index.vue'),
        meta: { title: '工作台', icon: 'House', affix: true }
      }
    ]
  },
  {
    path: '/profile',
    component: Layout,
    redirect: '/profile/index',
    meta: { hidden: true },
    children: [
      {
        path: 'index',
        name: 'Profile',
        component: () => import('@/views/profile/index.vue'),
        meta: { title: '个人中心', icon: 'User' }
      }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: constantRoutes,
  scrollBehavior: () => ({ top: 0 })
})

router.beforeEach(async (to, from, next) => {
  const userStore = useUserStore();
  const menuStore = useMenuStore();
  const isLoggedIn = userStore.isLoggedIn;

  // 1. 白名单路由直接放行
  if (to.path === '/login' || to.path === '/404' || to.path === '/403') {
    return next();
  }

  // 2. 如果未登录，重定向到登录页
  if (!isLoggedIn) {
    const redirectPath = to.fullPath !== '/' ? `?redirect=${encodeURIComponent(to.fullPath)}` : '';
    return next(`/login${redirectPath}`);
  }

  // 3. 如果已登录，但系统尚未初始化，则执行初始化流程
  if (!menuStore.isRoutesGenerated) {
    try {
      console.log('🚀 [Router Guard] 系统未初始化，开始执行初始化流程...');
      
      // 统一的初始化函数
      await initializeSystem();

      console.log('✅ [Router Guard] 初始化成功，重定向到目标路由...');
      return next({ ...to, replace: true });
    } catch (error) {
      console.error('❌ [Router Guard] 系统初始化失败:', error);
      // 初始化失败时，userStore 内部会处理状态重置
      return next('/login');
    }
  }
  
  // 4. 如果已登录且已初始化，直接放行
  // 额外检查目标路由是否存在
  if (to.matched.length === 0) {
    return next('/404');
  }
  return next();
});

/**
 * @description 统一的系统初始化函数
 */
async function initializeSystem() {
  const userStore = useUserStore();
  const menuStore = useMenuStore();
  
  // 1. 获取用户信息（如果尚未获取）
  if (!userStore.userInfo) {
    await userStore.getUserInfo();
  }
  
  // 2. 获取菜单并生成动态路由
  const { routes } = await menuStore.fetchUserMenus();
  if (routes?.length > 0) {
    addRoutes(routes);
    menuStore.setRoutesGenerated(true);
  } else {
    console.warn('用户无任何菜单权限，仅可访问静态路由');
    // 即使没有菜单，也标记为已生成，避免重复初始化
    menuStore.setRoutesGenerated(true);
  }
  
  // 可以在这里扩展其他初始化逻辑，例如：
  // await userStore.refreshPermissions();
  // await dictionaryStore.fetchAllDictionaries();
}

/**
 * @description 重置路由 - 登出时清理动态路由
 */
export function resetRouter() {
  const newRouter = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: constantRoutes,
    scrollBehavior: () => ({ top: 0 })
  })
  
  ;(router as any).matcher = (newRouter as any).matcher
}

/**
 * @description 添加动态路由的辅助函数
 */
export function addRoutes(routes: readonly RouteRecordRaw[]) {
  if (!routes || routes.length === 0) return

  const parentRouteName = 'Root'
  if (!router.hasRoute(parentRouteName)) {
    console.error(`[动态路由错误] 无法找到名为 "${parentRouteName}" 的父路由！`)
    return
  }

  routes.forEach(route => {
    try {
      if (route.name && router.hasRoute(route.name)) {
        console.warn(`[动态路由] 跳过已存在的路由: ${String(route.name)}`)
        return
      }
      router.addRoute(parentRouteName, route)
    } catch (error) {
      console.error(`[动态路由错误] 添加失败:`, route, error)
    }
  })
}

export default router