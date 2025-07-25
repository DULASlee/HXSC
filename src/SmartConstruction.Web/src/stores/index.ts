// =============================================
// Pinia状态管理入口
// =============================================
import { createPinia } from 'pinia'
import { useUserStore } from './user'
import { useMenuStore } from './menu'

const pinia = createPinia()

// 注意：如果需要持久化功能，请安装 pinia-plugin-persistedstate
// import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
// pinia.use(piniaPluginPersistedstate)

export default pinia

/**
 * @description [顶层协调] 统一重置所有相关的 Store
 * 这是登出或认证失败时的唯一清理入口
 */
export function resetAllStores() {
  const userStore = useUserStore();
  const menuStore = useMenuStore();

  userStore.resetUser();
  menuStore.clearMenus();
  
  // 未来可以扩展，例如：
  // const appStore = useAppStore();
  // appStore.resetState();

  console.log('所有相关的 Store 已被重置');
}

// Store类型
export type { TabItem } from './app'
export type { SystemSettings } from './settings'
