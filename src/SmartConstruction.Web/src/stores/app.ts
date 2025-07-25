// =============================================
// 应用状态管理
// =============================================
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

// 标签页接口
export interface TabItem {
  name: string
  path: string
  title: string
  icon?: string
  closable: boolean
  affix?: boolean
  query?: Record<string, any>
}

// 访问过的视图接口
export interface VisitedView {
  path: string
  title: string
  name?: string
  icon?: string
  affix?: boolean
  query?: Record<string, any>
  params?: Record<string, any>
}

export const useAppStore = defineStore('app', () => {
  // 状态
  const language = ref('zh-CN')
  const theme = ref<'light' | 'dark' | 'blue'>('light')
  const sidebarCollapsed = ref(false)
  const tabs = ref<TabItem[]>([])
  const activeTab = ref('')
  
  // 访问过的视图管理
  const visitedViews = ref<VisitedView[]>([])
  const cachedViews = ref<string[]>([])

  // 计算属性
  const isDark = computed(() => theme.value === 'dark')
  
  // 为了兼容现有代码，创建sidebar对象结构
  const sidebar = computed(() => ({
    opened: !sidebarCollapsed.value,
    collapsed: sidebarCollapsed.value
  }))

  // 设置语言
  function setLanguage(lang: 'zh-CN' | 'en' | 'ja') {
    language.value = lang
    // 这里可以添加切换语言的逻辑
    document.documentElement.setAttribute('lang', lang)
  }

  // 设置主题
  function setTheme(newTheme: 'light' | 'dark' | 'blue') {
    theme.value = newTheme
    // 这里可以添加切换主题的逻辑
    document.documentElement.setAttribute('data-theme', newTheme)
    
    if (newTheme === 'dark') {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
  }

  // 切换侧边栏
  function toggleSidebar() {
    sidebarCollapsed.value = !sidebarCollapsed.value
  }

  // 设置侧边栏状态
  function setSidebarCollapsed(collapsed: boolean) {
    sidebarCollapsed.value = collapsed
  }

  // 添加标签页
  function addTab(tab: TabItem) {
    const existingTab = tabs.value.find(t => t.path === tab.path)
    if (!existingTab) {
      tabs.value.push(tab)
    }
    activeTab.value = tab.path
  }

  // 移除标签页
  function removeTab(path: string) {
    const index = tabs.value.findIndex(t => t.path === path)
    if (index > -1) {
      tabs.value.splice(index, 1)
      
      // 如果移除的是当前激活的标签页，需要激活其他标签页
      if (activeTab.value === path && tabs.value.length > 0) {
        const newIndex = Math.min(index, tabs.value.length - 1)
        activeTab.value = tabs.value[newIndex].path
      }
    }
  }

  // 设置激活的标签页
  function setActiveTab(path: string) {
    activeTab.value = path
  }

  // 关闭其他标签页
  function closeOtherTabs(currentPath: string) {
    tabs.value = tabs.value.filter(tab => !tab.closable || tab.path === currentPath)
    activeTab.value = currentPath
  }

  // 关闭所有标签页
  function closeAllTabs() {
    tabs.value = tabs.value.filter(tab => !tab.closable)
    if (tabs.value.length > 0) {
      activeTab.value = tabs.value[0].path
    } else {
      activeTab.value = ''
    }
  }

  // 添加访问过的视图
  function addVisitedView(view: VisitedView) {
    if (visitedViews.value.some(v => v.path === view.path)) return
    
    visitedViews.value.push({
      ...view,
      title: view.title || 'No title'
    })
  }

  // 添加缓存视图
  function addCachedView(view: VisitedView) {
    if (view.name && !cachedViews.value.includes(view.name)) {
      cachedViews.value.push(view.name)
    }
  }

  // 删除访问过的视图
  function delVisitedView(view: VisitedView) {
    const index = visitedViews.value.findIndex(v => v.path === view.path)
    if (index > -1) {
      visitedViews.value.splice(index, 1)
    }
  }

  // 删除缓存视图
  function delCachedView(view: VisitedView) {
    if (view.name) {
      const index = cachedViews.value.indexOf(view.name)
      if (index > -1) {
        cachedViews.value.splice(index, 1)
      }
    }
  }

  // 删除其他访问过的视图
  function delOthersVisitedViews(view: VisitedView) {
    visitedViews.value = visitedViews.value.filter(v => v.affix || v.path === view.path)
  }

  // 删除其他缓存视图
  function delOthersCachedViews(view: VisitedView) {
    if (view.name) {
      cachedViews.value = cachedViews.value.filter(name => name === view.name)
    } else {
      cachedViews.value = []
    }
  }

  // 删除所有访问过的视图
  function delAllVisitedViews() {
    visitedViews.value = visitedViews.value.filter(v => v.affix)
  }

  // 删除所有缓存视图
  function delAllCachedViews() {
    cachedViews.value = []
  }

  // 更新访问过的视图
  function updateVisitedView(view: VisitedView) {
    const index = visitedViews.value.findIndex(v => v.path === view.path)
    if (index > -1) {
      visitedViews.value[index] = { ...visitedViews.value[index], ...view }
    }
  }

  // 清空所有状态
  function clearAll() {
    tabs.value = []
    activeTab.value = ''
    visitedViews.value = []
    cachedViews.value = []
  }

  // 初始化应用设置
  function initAppSettings() {
    // 设置主题
    if (theme.value === 'dark') {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
    
    // 设置语言
    document.documentElement.setAttribute('lang', language.value)
    
    // 添加固定标签页
    addVisitedView({
      path: '/dashboard',
      title: '工作台',
      name: 'Dashboard',
      icon: 'House',
      affix: true
    })
  }

  return {
    // 状态
    language,
    theme,
    sidebarCollapsed,
    tabs,
    activeTab,
    visitedViews,
    cachedViews,
    
    // 计算属性
    isDark,
    sidebar,
    
    // 操作
    setLanguage,
    setTheme,
    toggleSidebar,
    setSidebarCollapsed,
    addTab,
    removeTab,
    setActiveTab,
    closeOtherTabs,
    closeAllTabs,
    addVisitedView,
    addCachedView,
    delVisitedView,
    delCachedView,
    delOthersVisitedViews,
    delOthersCachedViews,
    delAllVisitedViews,
    delAllCachedViews,
    updateVisitedView,
    clearAll,
    initAppSettings
  }
}, {
  persist: {
    paths: ['language', 'theme', 'sidebarCollapsed']
  }
})