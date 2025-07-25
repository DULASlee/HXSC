// =============================================
// 权限控制 - 路由守卫
// =============================================
import router from './router'
import type { RouteLocationNormalized } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { useMenuStore } from '@/stores/menu'
import { useAppStore } from '@/stores/app'
import { usePermissionStore } from '@/stores/permission'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import { ElMessage } from 'element-plus'

// 配置进度条
NProgress.configure({ showSpinner: false })

// 白名单路由（不需要登录）
const whiteList = ['/login', '/404', '/403']

// 路由前置守卫
router.beforeEach(async (to: RouteLocationNormalized, from: RouteLocationNormalized, next) => {
  // 开始进度条
  NProgress.start()

  // 设置页面标题
  const title = to.meta.title as string
  document.title = title ? `${title} - ${import.meta.env.VITE_APP_TITLE}` : import.meta.env.VITE_APP_TITLE

  const userStore = useUserStore()
  const appStore = useAppStore()
  const menuStore = useMenuStore()

  if (userStore.isLoggedIn) {
    if (to.path === '/login') {
      next({ path: '/dashboard' })
      NProgress.done()
    } else {
      if (menuStore.isRoutesGenerated) {
        next()
      } else {
        if (menuStore.isInitializing) {
          console.log('路由正在初始化，请稍候...')
          NProgress.done() // 避免进度条一直加载
          return
        }

        try {
          menuStore.isInitializing = true
          console.log('权限守卫：开始初始化路由')

          if (!userStore.userInfo) {
            await userStore.getUserInfo()
          }

          const { menus } = await menuStore.fetchUserMenus()
          const accessRoutes = await menuStore.generateRoutes(menus)

          if (accessRoutes && accessRoutes.length > 0) {
            accessRoutes.forEach(route => router.addRoute(route))
            menuStore.isRoutesGenerated = true
            console.log('✅ 动态路由已添加，重定向到目标页面')
            next({ ...to, replace: true })
          } else {
            console.warn('没有可用的路由数据，将使用默认路由')
            await menuStore.generateDefaultRoutes()
            next({ ...to, replace: true })
          }
        } catch (error) {
          console.error('权限守卫：初始化路由失败:', error)
          ElMessage.error('系统初始化失败，请重新登录')
          await userStore.logout()
          next(`/login?redirect=${to.path}`)
        } finally {
          menuStore.isInitializing = false
          NProgress.done()
        }
      }
    }
  } else {
    // 未登录
    if (whiteList.includes(to.path)) {
      next()
    } else {
      next(`/login?redirect=${to.path}`)
      NProgress.done()
    }
  }
})

// 路由后置守卫
router.afterEach((to: RouteLocationNormalized) => {
  // 结束进度条
  NProgress.done()
  
  // 设置当前激活的标签页
  const appStore = useAppStore()
  if (to.path !== appStore.activeTab) {
    appStore.setActiveTab(to.path)
  }
})

// 路由错误处理
router.onError((error) => {
  console.error('路由错误:', error)
  NProgress.done()
})