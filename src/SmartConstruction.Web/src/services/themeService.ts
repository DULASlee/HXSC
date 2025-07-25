// =============================================
// 主题服务 - 无闪烁主题切换系统
// =============================================

export type ThemeType = 'light' | 'dark' | 'blue' | 'green' | 'gray'

export interface ThemeConfig {
  name: string
  label: string
  icon: string
  preview: string
  description: string
}

export const THEME_CONFIGS: Record<ThemeType, ThemeConfig> = {
  light: {
    name: 'light',
    label: '浅色主题',
    icon: '☀️',
    preview: 'linear-gradient(135deg, #f0f2f5 0%, #ffffff 100%)',
    description: '清新明亮的浅色主题，适合日间使用'
  },
  dark: {
    name: 'dark',
    label: '深色主题',
    icon: '🌙',
    preview: 'linear-gradient(135deg, #141414 0%, #1f1f1f 100%)',
    description: '护眼的深色主题，适合夜间使用'
  },
  blue: {
    name: 'blue',
    label: '深蓝主题',
    icon: '🌊',
    preview: 'linear-gradient(135deg, #e3f2fd 0%, #1e88e5 100%)',
    description: '专业的蓝色主题，商务风格'
  },
  green: {
    name: 'green',
    label: '深淡绿主题',
    icon: '🌿',
    preview: 'linear-gradient(135deg, #f1f8e9 0%, #2e7d32 100%)',
    description: '自然的绿色主题，清新护眼'
  },
  gray: {
    name: 'gray',
    label: '黑灰主题',
    icon: '⚫',
    preview: 'linear-gradient(135deg, #fafafa 0%, #424242 100%)',
    description: '简约的灰色主题，低调优雅'
  }
}

class ThemeService {
  private currentTheme: ThemeType = 'light'
  private readonly STORAGE_KEY = 'smart-construction-theme'
  private readonly TRANSITION_CLASS = 'theme-transition'
  
  constructor() {
    this.init()
  }

  /**
   * 初始化主题系统
   */
  private init() {
    // 从localStorage恢复主题
    const savedTheme = localStorage.getItem(this.STORAGE_KEY) as ThemeType
    if (savedTheme && THEME_CONFIGS[savedTheme]) {
      this.currentTheme = savedTheme
    }
    
    // 应用主题
    this.applyTheme(this.currentTheme, false)
    
    // 监听系统主题变化
    this.watchSystemTheme()
  }

  /**
   * 获取当前主题
   */
  getCurrentTheme(): ThemeType {
    return this.currentTheme
  }

  /**
   * 获取所有主题配置
   */
  getAllThemes(): ThemeConfig[] {
    return Object.values(THEME_CONFIGS)
  }

  /**
   * 切换主题（无闪烁）
   */
  async setTheme(theme: ThemeType): Promise<void> {
    if (!THEME_CONFIGS[theme] || theme === this.currentTheme) {
      return
    }

    // 添加过渡动画
    this.addTransition()

    try {
      // 应用新主题
      await this.applyTheme(theme, true)
      
      // 更新当前主题
      this.currentTheme = theme
      
      // 保存到localStorage
      localStorage.setItem(this.STORAGE_KEY, theme)
      
      // 触发主题变化事件
      this.dispatchThemeChangeEvent(theme)
      
    } finally {
      // 移除过渡动画
      setTimeout(() => this.removeTransition(), 300)
    }
  }

  /**
   * 应用主题
   */
  private async applyTheme(theme: ThemeType, animated: boolean = false): Promise<void> {
    const html = document.documentElement
    
    // 设置主题属性
    html.setAttribute('data-theme', theme)
    
    // 处理深色主题的特殊类
    if (theme === 'dark') {
      html.classList.add('dark')
    } else {
      html.classList.remove('dark')
    }
    
    // 应用Element Plus主题
    this.applyElementTheme(theme)
    
    // 如果是动画切换，等待CSS变量生效
    if (animated) {
      await new Promise(resolve => setTimeout(resolve, 50))
    }
  }

  /**
   * 应用Element Plus主题
   */
  private applyElementTheme(theme: ThemeType) {
    const html = document.documentElement
    
    // 移除所有Element Plus主题类
    html.classList.remove('el-theme-light', 'el-theme-dark', 'el-theme-blue', 'el-theme-green', 'el-theme-gray')
    
    // 添加对应的主题类
    html.classList.add(`el-theme-${theme}`)
  }

  /**
   * 添加过渡动画
   */
  private addTransition() {
    const style = document.createElement('style')
    style.id = 'theme-transition-style'
    style.textContent = `
      .${this.TRANSITION_CLASS} * {
        transition: background-color 0.3s ease, 
                   border-color 0.3s ease, 
                   color 0.3s ease,
                   box-shadow 0.3s ease !important;
      }
    `
    document.head.appendChild(style)
    document.documentElement.classList.add(this.TRANSITION_CLASS)
  }

  /**
   * 移除过渡动画
   */
  private removeTransition() {
    document.documentElement.classList.remove(this.TRANSITION_CLASS)
    const style = document.getElementById('theme-transition-style')
    if (style) {
      document.head.removeChild(style)
    }
  }

  /**
   * 监听系统主题变化
   */
  private watchSystemTheme() {
    if (window.matchMedia) {
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
      
      const handleChange = (e: MediaQueryListEvent) => {
        // 只有在用户没有手动设置主题时才自动切换
        const savedTheme = localStorage.getItem(this.STORAGE_KEY)
        if (!savedTheme) {
          this.setTheme(e.matches ? 'dark' : 'light')
        }
      }
      
      mediaQuery.addEventListener('change', handleChange)
    }
  }

  /**
   * 触发主题变化事件
   */
  private dispatchThemeChangeEvent(theme: ThemeType) {
    const event = new CustomEvent('theme-change', {
      detail: { theme, config: THEME_CONFIGS[theme] }
    })
    window.dispatchEvent(event)
  }

  /**
   * 获取主题预览色彩
   */
  getThemePreview(theme: ThemeType): string {
    return THEME_CONFIGS[theme]?.preview || ''
  }

  /**
   * 检查是否为深色主题
   */
  isDarkTheme(theme?: ThemeType): boolean {
    const targetTheme = theme || this.currentTheme
    return targetTheme === 'dark' || targetTheme === 'gray'
  }

  /**
   * 获取主题对应的CSS变量值
   */
  getThemeVariable(variable: string, theme?: ThemeType): string {
    if (theme && theme !== this.currentTheme) {
      // 临时切换主题获取变量值
      const tempDiv = document.createElement('div')
      tempDiv.setAttribute('data-theme', theme)
      tempDiv.style.position = 'absolute'
      tempDiv.style.visibility = 'hidden'
      document.body.appendChild(tempDiv)
      
      const value = getComputedStyle(tempDiv).getPropertyValue(variable)
      document.body.removeChild(tempDiv)
      return value.trim()
    }
    
    return getComputedStyle(document.documentElement).getPropertyValue(variable).trim()
  }

  /**
   * 重置主题到默认
   */
  resetTheme(): void {
    localStorage.removeItem(this.STORAGE_KEY)
    this.setTheme('light')
  }
}

// 导出单例
export const themeService = new ThemeService()

// 导出类型和配置
export { ThemeService }