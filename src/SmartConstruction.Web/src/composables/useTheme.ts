// =============================================
// 主题组合式函数 - 提供主题相关的响应式状态和方法
// =============================================

import { ref, computed, onMounted, onUnmounted } from 'vue'
import { themeService, type ThemeType, THEME_CONFIGS } from '@/services/themeService'
import { useAppStore } from '@/stores/app'

export function useTheme() {
  const appStore = useAppStore()
  
  // 当前主题
  const currentTheme = ref<ThemeType>(themeService.getCurrentTheme())
  
  // 主题配置
  const themeConfig = computed(() => THEME_CONFIGS[currentTheme.value])
  
  // 是否为深色主题
  const isDark = computed(() => themeService.isDarkTheme(currentTheme.value))
  
  // 所有可用主题
  const availableThemes = computed(() => themeService.getAllThemes())
  
  // 切换主题
  const setTheme = async (theme: ThemeType) => {
    try {
      await themeService.setTheme(theme)
      appStore.setTheme(theme)
      currentTheme.value = theme
    } catch (error) {
      console.error('主题切换失败:', error)
      throw error
    }
  }
  
  // 切换到下一个主题
  const nextTheme = async () => {
    const themes = Object.keys(THEME_CONFIGS) as ThemeType[]
    const currentIndex = themes.indexOf(currentTheme.value)
    const nextIndex = (currentIndex + 1) % themes.length
    await setTheme(themes[nextIndex])
  }
  
  // 重置主题
  const resetTheme = async () => {
    await themeService.resetTheme()
    currentTheme.value = 'light'
    appStore.setTheme('light')
  }
  
  // 获取主题变量值
  const getThemeVariable = (variable: string, theme?: ThemeType) => {
    return themeService.getThemeVariable(variable, theme)
  }
  
  // 获取主题预览
  const getThemePreview = (theme: ThemeType) => {
    return themeService.getThemePreview(theme)
  }
  
  // 监听主题变化事件
  const handleThemeChange = (event: CustomEvent) => {
    currentTheme.value = event.detail.theme
  }
  
  // 生命周期钩子
  onMounted(() => {
    // 监听主题变化
    window.addEventListener('theme-change', handleThemeChange as EventListener)
    
    // 同步当前主题
    currentTheme.value = themeService.getCurrentTheme()
  })
  
  onUnmounted(() => {
    window.removeEventListener('theme-change', handleThemeChange as EventListener)
  })
  
  return {
    // 状态
    currentTheme,
    themeConfig,
    isDark,
    availableThemes,
    
    // 方法
    setTheme,
    nextTheme,
    resetTheme,
    getThemeVariable,
    getThemePreview
  }
}

// 主题相关的工具函数
export const themeUtils = {
  /**
   * 根据主题获取对应的图标
   */
  getThemeIcon(theme: ThemeType): string {
    return THEME_CONFIGS[theme]?.icon || '🎨'
  },
  
  /**
   * 根据主题获取对应的描述
   */
  getThemeDescription(theme: ThemeType): string {
    return THEME_CONFIGS[theme]?.description || ''
  },
  
  /**
   * 检查主题是否为深色系
   */
  isDarkTheme(theme: ThemeType): boolean {
    return theme === 'dark' || theme === 'gray'
  },
  
  /**
   * 获取主题的主色调
   */
  getThemePrimaryColor(theme: ThemeType): string {
    const colorMap: Record<ThemeType, string> = {
      light: '#1890ff',
      dark: '#177ddc',
      blue: '#1e88e5',
      green: '#2e7d32',
      gray: '#424242'
    }
    return colorMap[theme] || '#1890ff'
  },
  
  /**
   * 生成主题CSS类名
   */
  getThemeClassName(theme: ThemeType): string {
    return `theme-${theme}`
  }
}