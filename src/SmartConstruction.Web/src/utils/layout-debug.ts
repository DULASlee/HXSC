// =============================================
// 布局调试工具
// =============================================

interface DebugInfo {
  viewport: { width: number; height: number }
  sidebar: { width: number; visible: boolean }
  route: { path: string; name?: string; matched: number }
  menu: { total: number; visible: number; errors: string[] }
}

export class LayoutDebugger {
  private static instance: LayoutDebugger
  private debugElement?: HTMLElement

  static getInstance(): LayoutDebugger {
    if (!LayoutDebugger.instance) {
      LayoutDebugger.instance = new LayoutDebugger()
    }
    return LayoutDebugger.instance
  }

  // 开启布局调试模式
  enableDebugMode(): void {
    this.addDebugStyles()
    this.createDebugPanel()
    console.log('[布局调试] 调试模式已开启')
  }

  // 关闭调试模式
  disableDebugMode(): void {
    this.removeDebugStyles()
    this.removeDebugPanel()
    console.log('[布局调试] 调试模式已关闭')
  }

  // 添加调试样式
  private addDebugStyles(): void {
    const style = document.createElement('style')
    style.id = 'layout-debug-styles'
    style.textContent = `
      .debug-outline * {
        outline: 1px solid rgba(255, 0, 0, 0.3) !important;
        outline-offset: -1px;
      }
      
      .sidebar-container {
        outline: 2px solid #409eff !important;
      }
      
      .main-container {
        outline: 2px solid #67c23a !important;
      }
      
      .navbar {
        outline: 2px solid #e6a23c !important;
      }
      
      .debug-panel {
        position: fixed;
        top: 10px;
        right: 10px;
        background: rgba(0, 0, 0, 0.8);
        color: white;
        padding: 10px;
        border-radius: 4px;
        font-size: 12px;
        z-index: 9999;
        min-width: 200px;
      }
    `
    document.head.appendChild(style)
    document.body.classList.add('debug-outline')
  }

  // 移除调试样式
  private removeDebugStyles(): void {
    const style = document.getElementById('layout-debug-styles')
    if (style) {
      style.remove()
    }
    document.body.classList.remove('debug-outline')
  }

  // 创建调试面板
  private createDebugPanel(): void {
    this.debugElement = document.createElement('div')
    this.debugElement.className = 'debug-panel'
    document.body.appendChild(this.debugElement)
    
    this.updateDebugInfo()
    
    // 每秒更新调试信息
    setInterval(() => {
      this.updateDebugInfo()
    }, 1000)
  }

  // 移除调试面板
  private removeDebugPanel(): void {
    if (this.debugElement) {
      this.debugElement.remove()
      this.debugElement = undefined
    }
  }

  // 更新调试信息
  private updateDebugInfo(): void {
    if (!this.debugElement) return

    const info = this.collectDebugInfo()
    
    this.debugElement.innerHTML = `
      <div><strong>布局调试信息</strong></div>
      <div>视口: ${info.viewport.width}x${info.viewport.height}</div>
      <div>侧边栏: ${info.sidebar.width}px (${info.sidebar.visible ? '显示' : '隐藏'})</div>
      <div>当前路由: ${info.route.path}</div>
      <div>路由匹配: ${info.route.matched}层</div>
      <div>菜单总数: ${info.menu.total}</div>
      <div>可见菜单: ${info.menu.visible}</div>
      ${info.menu.errors.length > 0 ? `<div style="color: #f56c6c;">错误: ${info.menu.errors.join(', ')}</div>` : ''}
    `
  }

  // 收集调试信息
  private collectDebugInfo(): DebugInfo {
    const sidebar = document.querySelector('.sidebar-container') as HTMLElement
    const route = window.location
    
    return {
      viewport: {
        width: window.innerWidth,
        height: window.innerHeight
      },
      sidebar: {
        width: sidebar ? sidebar.offsetWidth : 0,
        visible: sidebar ? !sidebar.classList.contains('collapsed') : false
      },
      route: {
        path: route.pathname,
        name: route.hash.slice(1) || undefined,
        matched: document.querySelectorAll('[data-v-router-view]').length
      },
      menu: {
        total: document.querySelectorAll('.el-menu-item, .el-sub-menu').length,
        visible: document.querySelectorAll('.el-menu-item:not([style*="display: none"]), .el-sub-menu:not([style*="display: none"])').length,
        errors: this.detectMenuErrors()
      }
    }
  }

  // 检测菜单错误
  private detectMenuErrors(): string[] {
    const errors: string[] = []
    
    // 检查空菜单项
    const emptyMenuItems = document.querySelectorAll('.el-menu-item:empty, .el-sub-menu__title:empty')
    if (emptyMenuItems.length > 0) {
      errors.push(`${emptyMenuItems.length}个空菜单项`)
    }
    
    // 检查重叠元素
    const sidebarRect = document.querySelector('.sidebar-container')?.getBoundingClientRect()
    const mainRect = document.querySelector('.main-container')?.getBoundingClientRect()
    
    if (sidebarRect && mainRect && sidebarRect.right > mainRect.left) {
      errors.push('侧边栏与主内容区重叠')
    }
    
    return errors
  }

  // 强制重绘
  forceRepaint(): void {
    const body = document.body
    body.style.display = 'none'
    // 触发重排
    body.offsetHeight
    body.style.display = ''
    console.log('[布局调试] 强制重绘完成')
  }

  // 检查CSS Grid支持
  checkGridSupport(): boolean {
    const supported = CSS.supports('display', 'grid')
    console.log(`[布局调试] CSS Grid支持: ${supported ? '是' : '否'}`)
    return supported
  }

  // 检查Flexbox支持
  checkFlexSupport(): boolean {
    const supported = CSS.supports('display', 'flex')
    console.log(`[布局调试] Flexbox支持: ${supported ? '是' : '否'}`)
    return supported
  }
}

// 开发环境自动注册全局调试器
if (import.meta.env.DEV) {
  (window as any).layoutDebugger = LayoutDebugger.getInstance()
  console.log('[布局调试] 调试器已注册到 window.layoutDebugger')
}

export default LayoutDebugger