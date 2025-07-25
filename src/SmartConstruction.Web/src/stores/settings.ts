// =============================================
// 设置状态管理
// =============================================
import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useSettingsStore = defineStore('settings', () => {
  // 布局设置
  const sidebarCollapsed = ref(false)
  const showTabs = ref(true)
  const showFooter = ref(true)
  const showBreadcrumb = ref(true)
  const fixedHeader = ref(true)
  const tagsView = ref(true)
  
  // 主题设置
  const theme = ref<'light' | 'dark' | 'blue'>('light')
  const primaryColor = ref('#409eff')
  const layout = ref<'vertical' | 'horizontal' | 'mix'>('vertical')
  
  // 动画设置
  const enableTransition = ref(true)
  const transitionName = ref('fade')
  
  // 其他设置
  const language = ref('zh-CN')
  const size = ref<'large' | 'default' | 'small'>('default')
  const showLogo = ref(true)
  const multipleTab = ref(true)
  
  // 操作方法
  const setSidebarCollapsed = (collapsed: boolean) => {
    sidebarCollapsed.value = collapsed
    localStorage.setItem('sidebar-collapsed', String(collapsed))
  }
  
  const setShowTabs = (show: boolean) => {
    showTabs.value = show
    localStorage.setItem('show-tabs', String(show))
  }
  
  const setShowFooter = (show: boolean) => {
    showFooter.value = show
    localStorage.setItem('show-footer', String(show))
  }
  
  const setTheme = (newTheme: 'light' | 'dark' | 'blue') => {
    theme.value = newTheme
    localStorage.setItem('theme', newTheme)
  }
  
  const setPrimaryColor = (color: string) => {
    primaryColor.value = color
    localStorage.setItem('primary-color', color)
  }
  
  const setLanguage = (lang: string) => {
    language.value = lang
    localStorage.setItem('language', lang)
  }
  
  const setSize = (newSize: 'large' | 'default' | 'small') => {
    size.value = newSize
    localStorage.setItem('size', newSize)
  }
  
  // 初始化设置
  const initSettings = () => {
    // 从本地存储恢复设置
    const savedCollapsed = localStorage.getItem('sidebar-collapsed')
    if (savedCollapsed !== null) {
      sidebarCollapsed.value = savedCollapsed === 'true'
    }
    
    const savedShowTabs = localStorage.getItem('show-tabs')
    if (savedShowTabs !== null) {
      showTabs.value = savedShowTabs === 'true'
    }
    
    const savedShowFooter = localStorage.getItem('show-footer')
    if (savedShowFooter !== null) {
      showFooter.value = savedShowFooter === 'true'
    }
    
    const savedTheme = localStorage.getItem('theme')
    if (savedTheme) {
      theme.value = savedTheme as 'light' | 'dark' | 'blue'
    }
    
    const savedPrimaryColor = localStorage.getItem('primary-color')
    if (savedPrimaryColor) {
      primaryColor.value = savedPrimaryColor
    }
    
    const savedLanguage = localStorage.getItem('language')
    if (savedLanguage) {
      language.value = savedLanguage
    }
    
    const savedSize = localStorage.getItem('size')
    if (savedSize) {
      size.value = savedSize as 'large' | 'default' | 'small'
    }
  }
  
  // 重置设置
  const resetSettings = () => {
    sidebarCollapsed.value = false
    showTabs.value = true
    showFooter.value = true
    showBreadcrumb.value = true
    fixedHeader.value = true
    tagsView.value = true
    theme.value = 'light'
    primaryColor.value = '#409eff'
    layout.value = 'vertical'
    enableTransition.value = true
    transitionName.value = 'fade'
    language.value = 'zh-CN'
    size.value = 'default'
    showLogo.value = true
    multipleTab.value = true
    
    // 清除本地存储
    localStorage.removeItem('sidebar-collapsed')
    localStorage.removeItem('show-tabs')
    localStorage.removeItem('show-footer')
    localStorage.removeItem('theme')
    localStorage.removeItem('primary-color')
    localStorage.removeItem('language')
    localStorage.removeItem('size')
  }
  
  return {
    // 状态
    sidebarCollapsed,
    showTabs,
    showFooter,
    showBreadcrumb,
    fixedHeader,
    tagsView,
    theme,
    primaryColor,
    layout,
    enableTransition,
    transitionName,
    language,
    size,
    showLogo,
    multipleTab,
    
    // 操作
    setSidebarCollapsed,
    setShowTabs,
    setShowFooter,
    setTheme,
    setPrimaryColor,
    setLanguage,
    setSize,
    initSettings,
    resetSettings
  }
}, {
  persist: {
    key: 'app-settings',
    storage: localStorage,
    paths: [
      'sidebarCollapsed',
      'showTabs',
      'showFooter',
      'theme',
      'primaryColor',
      'language',
      'size'
    ]
  }
})