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
  const userStore = useUserStore()
  const menuStore = useMenuStore()

  // 1. 白名单路由直接放行
  if (['/login', '/404', '/403'].includes(to.path)) {
    return next()
  }

  // 2. 检查 token 的强化逻辑
  const token = userStore.token || localStorage.getItem('token')
  if (!token) {
    return next(`/login?redirect=${encodeURIComponent(to.fullPath)}`)
  }

  // 3. 如果已登录但访问登录页，重定向到首页
  if (to.path === '/login') {
    return next('/')
  }

  // 4. 如果路由已生成，直接放行
  if (menuStore.isRoutesGenerated) {
    // 额外检查目标路由是否存在
    if (to.matched.length === 0) {
      return next('/404')
    }
    return next()
  }

  // 5. 初始化菜单和路由
  try {
    // 先确保用户信息已加载
    if (!userStore.userInfo) {
      await userStore.getUserInfo()
    }

    // 获取菜单路由
    const { routes } = await menuStore.fetchUserMenus()

    // 添加动态路由
    if (routes?.length > 0) {
      addRoutes(routes)
    } else {
      console.warn('用户无任何菜单权限')
    }

    // 标记路由已生成
    menuStore.setRoutesGenerated(true)

    // 重定向到原始请求
    return next({ ...to, replace: true })
  } catch (error) {
    console.error('路由初始化失败:', error)
    // refreshToken 失败时，userStore 内部已经处理了状态重置和页面跳转
    // 这里不再需要调用 userStore.logout()，只需要确保跳转到登录页
    const userStore = useUserStore()
    if (userStore.token) {
      // 如果还有token，说明可能不是认证问题，尝试登出一下
      await userStore.resetUser()
    }
    return next(`/login?redirect=${encodeURIComponent(to.fullPath)}`)
  }
})

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