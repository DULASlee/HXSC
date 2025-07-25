// =============================================
// 应用入口文件
// =============================================
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import pinia from './stores'
import i18n from './locales'

// Element Plus
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import 'element-plus/theme-chalk/dark/css-vars.css'
import 'element-plus/es/components/sub-menu/style/css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'

// 样式
import './styles/index.scss'
import './styles/layout-fix.scss' // 布局修复样式
import './assets/styles/themes/_variables.scss' // 主题变量
import './assets/styles/responsive.scss' // 响应式样式
import './styles/themes.scss' // 主题样式

// 权限控制
import './permission'

// 指令
import { setupDirectives } from './directives'

// 性能优化服务
import { performanceService } from './services/performanceService'
import { cacheService } from './services/cacheService'
import { preloadService } from './services/preloadService'
import { memoryService } from './services/memoryService'
import { resourceOptimizer } from './services/resourceOptimizer'
import { webWorkerService } from './services/webWorkerService'

// 主题服务
import { themeService } from './services/themeService'

// 创建应用实例
const app = createApp(App)

// 注册 Element Plus 图标
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

// 安装插件
app.use(pinia)
app.use(router)
app.use(i18n)
app.use(ElementPlus)

// 安装指令
setupDirectives(app)

// 注册全局服务
app.config.globalProperties.$performance = performanceService
app.config.globalProperties.$cache = cacheService
app.config.globalProperties.$preload = preloadService
app.config.globalProperties.$memory = memoryService
app.config.globalProperties.$resourceOptimizer = resourceOptimizer
app.config.globalProperties.$webWorker = webWorkerService

// 启动性能监控
performanceService.startTimer('app-init')

// 启动内存监控
memoryService.startMonitoring()

// 启动资源优化
resourceOptimizer.optimizeCSS()
// resourceOptimizer.optimizeJS() // 禁用JS优化，因为它在dev模式下会导致404
resourceOptimizer.optimizeFonts()
resourceOptimizer.enableCompression()

// 预加载关键资源
preloadService.preloadBatch([
  { url: '/api/system/info', type: 'fetch' }
  // { url: '/api/auth/menus', type: 'fetch' } // 移除菜单预加载，因为它需要认证
])

// 启用智能预加载
preloadService.enableSmartPreload()

// 初始化主题服务（themeService会在构造函数中自动初始化）

// 挂载应用
app.mount('#app')

// 记录应用初始化时间
performanceService.endTimer('app-init')

// 开发环境下添加性能监控工具
if (import.meta.env.DEV) {
  // 添加性能监控到window对象
  window.__PERFORMANCE__ = performanceService
  window.__CACHE__ = cacheService
  window.__MEMORY__ = memoryService
  window.__RESOURCE_OPTIMIZER__ = resourceOptimizer
  window.__WEB_WORKER__ = webWorkerService
  
  // 打印性能报告
  console.log('Performance Report:', performanceService.generateReport())
  console.log('Resource Optimization Report:', resourceOptimizer.generateOptimizationReport())
}