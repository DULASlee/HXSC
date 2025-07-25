// =============================================
// Pinia状态管理入口
// =============================================
import { createPinia } from 'pinia'

const pinia = createPinia()

// 注意：如果需要持久化功能，请安装 pinia-plugin-persistedstate
// import piniaPluginPersistedstate from 'pinia-plugin-persistedstate'
// pinia.use(piniaPluginPersistedstate)

export default pinia

// Store类型
export type { TabItem } from './app'
export type { SystemSettings } from './settings'
