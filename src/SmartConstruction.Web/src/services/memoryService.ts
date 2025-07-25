// =============================================
// 内存管理服务
// =============================================

// 内存使用信息接口
interface MemoryInfo {
  usedJSHeapSize: number
  totalJSHeapSize: number
  jsHeapSizeLimit: number
  usedPercent: number
}

// 内存监控配置
interface MemoryMonitorConfig {
  warningThreshold: number // 警告阈值（百分比）
  criticalThreshold: number // 危险阈值（百分比）
  checkInterval: number // 检查间隔（毫秒）
  autoCleanup: boolean // 是否自动清理
}

// 内存泄漏检测器
class MemoryLeakDetector {
  private listeners = new Set<EventListener>()
  private timers = new Set<number>()
  private observers = new Set<MutationObserver | IntersectionObserver | ResizeObserver>()
  private components = new WeakSet<any>()
  
  // 注册事件监听器
  addListener(element: EventTarget, event: string, listener: EventListener, options?: AddEventListenerOptions) {
    element.addEventListener(event, listener, options)
    this.listeners.add(listener)
    
    // 返回清理函数
    return () => {
      element.removeEventListener(event, listener, options)
      this.listeners.delete(listener)
    }
  }
  
  // 注册定时器
  addTimer(callback: TimerHandler, delay?: number, ...args: any[]): number {
    const timerId = window.setTimeout(callback, delay, ...args)
    this.timers.add(timerId)
    
    // 返回清理函数
    return timerId
  }
  
  // 注册间隔定时器
  addInterval(callback: TimerHandler, delay?: number, ...args: any[]): number {
    const intervalId = window.setInterval(callback, delay, ...args)
    this.timers.add(intervalId)
    
    // 返回清理函数
    return intervalId
  }
  
  // 注册观察器
  addObserver(observer: MutationObserver | IntersectionObserver | ResizeObserver) {
    this.observers.add(observer)
    
    // 返回清理函数
    return () => {
      observer.disconnect()
      this.observers.delete(observer)
    }
  }
  
  // 注册组件
  addComponent(component: any) {
    this.components.add(component)
  }
  
  // 清理所有资源
  cleanup() {
    // 清理定时器
    this.timers.forEach(timerId => {
      clearTimeout(timerId)
      clearInterval(timerId)
    })
    this.timers.clear()
    
    // 清理观察器
    this.observers.forEach(observer => {
      observer.disconnect()
    })
    this.observers.clear()
    
    // 清理监听器（注意：这里无法自动清理，需要组件自己清理）
    this.listeners.clear()
  }
  
  // 获取资源统计
  getStats() {
    return {
      listeners: this.listeners.size,
      timers: this.timers.size,
      observers: this.observers.size
    }
  }
}

// 内存管理服务
class MemoryService {
  private config: MemoryMonitorConfig
  private monitorInterval: number | null = null
  private memoryHistory: MemoryInfo[] = []
  private maxHistorySize = 100
  private leakDetector = new MemoryLeakDetector()
  private callbacks = {
    warning: new Set<(info: MemoryInfo) => void>(),
    critical: new Set<(info: MemoryInfo) => void>(),
    cleanup: new Set<() => void>()
  }
  
  constructor(config: Partial<MemoryMonitorConfig> = {}) {
    this.config = {
      warningThreshold: 70, // 70%
      criticalThreshold: 85, // 85%
      checkInterval: 10000, // 10秒
      autoCleanup: true,
      ...config
    }
  }
  
  // 开始内存监控
  startMonitoring(): void {
    if (this.monitorInterval) {
      return
    }
    
    this.monitorInterval = window.setInterval(() => {
      this.checkMemoryUsage()
    }, this.config.checkInterval)
    
    // 立即检查一次
    this.checkMemoryUsage()
  }
  
  // 停止内存监控
  stopMonitoring(): void {
    if (this.monitorInterval) {
      clearInterval(this.monitorInterval)
      this.monitorInterval = null
    }
  }
  
  // 检查内存使用情况
  private checkMemoryUsage(): void {
    const memoryInfo = this.getMemoryInfo()
    
    if (!memoryInfo) {
      return
    }
    
    // 添加到历史记录
    this.memoryHistory.push(memoryInfo)
    if (this.memoryHistory.length > this.maxHistorySize) {
      this.memoryHistory.shift()
    }
    
    // 检查阈值
    if (memoryInfo.usedPercent >= this.config.criticalThreshold) {
      this.callbacks.critical.forEach(callback => callback(memoryInfo))
      
      if (this.config.autoCleanup) {
        this.performCleanup()
      }
    } else if (memoryInfo.usedPercent >= this.config.warningThreshold) {
      this.callbacks.warning.forEach(callback => callback(memoryInfo))
    }
  }
  
  // 获取内存信息
  getMemoryInfo(): MemoryInfo | null {
    if (typeof window !== 'undefined' && 'performance' in window && 'memory' in (performance as any)) {
      const memory = (performance as any).memory
      const usedPercent = (memory.usedJSHeapSize / memory.jsHeapSizeLimit) * 100
      
      return {
        usedJSHeapSize: memory.usedJSHeapSize,
        totalJSHeapSize: memory.totalJSHeapSize,
        jsHeapSizeLimit: memory.jsHeapSizeLimit,
        usedPercent
      }
    }
    
    return null
  }
  
  // 获取内存历史记录
  getMemoryHistory(): MemoryInfo[] {
    return [...this.memoryHistory]
  }
  
  // 执行内存清理
  performCleanup(): void {
    console.log('🧹 执行内存清理...')
    
    // 触发垃圾回收（如果可用）
    if ('gc' in window && typeof (window as any).gc === 'function') {
      (window as any).gc()
    }
    
    // 清理缓存
    this.clearCaches()
    
    // 清理DOM
    this.cleanupDOM()
    
    // 清理事件监听器
    this.leakDetector.cleanup()
    
    // 触发自定义清理回调
    this.callbacks.cleanup.forEach(callback => callback())
    
    console.log('✅ 内存清理完成')
  }
  
  // 清理缓存
  private clearCaches(): void {
    // 清理图片缓存
    const images = document.querySelectorAll('img')
    images.forEach(img => {
      if (img.complete && !img.closest('[data-keep-cache]')) {
        // 对于不在视口中的图片，清理其缓存
        const rect = img.getBoundingClientRect()
        if (rect.bottom < 0 || rect.top > window.innerHeight) {
          const src = img.src
          img.src = ''
          img.src = src
        }
      }
    })
    
    // 清理浏览器缓存（如果支持）
    if ('caches' in window) {
      caches.keys().then(names => {
        names.forEach(name => {
          if (name.includes('temp') || name.includes('cache')) {
            caches.delete(name)
          }
        })
      })
    }
  }
  
  // 清理DOM
  private cleanupDOM(): void {
    // 移除不可见的元素
    const hiddenElements = document.querySelectorAll('[style*="display: none"], .hidden')
    hiddenElements.forEach(element => {
      if (!element.closest('[data-keep-hidden]')) {
        element.remove()
      }
    })
    
    // 清理空的文本节点
    const walker = document.createTreeWalker(
      document.body,
      NodeFilter.SHOW_TEXT,
      {
        acceptNode: (node) => {
          return node.textContent?.trim() === '' ? NodeFilter.FILTER_ACCEPT : NodeFilter.FILTER_REJECT
        }
      }
    )
    
    const emptyTextNodes: Node[] = []
    let node
    while (node = walker.nextNode()) {
      emptyTextNodes.push(node)
    }
    
    emptyTextNodes.forEach(node => node.remove())
  }
  
  // 检测内存泄漏
  detectMemoryLeaks(): {
    potentialLeaks: string[]
    recommendations: string[]
  } {
    const potentialLeaks: string[] = []
    const recommendations: string[] = []
    
    // 检查事件监听器数量
    const stats = this.leakDetector.getStats()
    if (stats.listeners > 100) {
      potentialLeaks.push(`过多的事件监听器: ${stats.listeners}`)
      recommendations.push('检查是否有未清理的事件监听器')
    }
    
    if (stats.timers > 50) {
      potentialLeaks.push(`过多的定时器: ${stats.timers}`)
      recommendations.push('检查是否有未清理的定时器')
    }
    
    if (stats.observers > 20) {
      potentialLeaks.push(`过多的观察器: ${stats.observers}`)
      recommendations.push('检查是否有未清理的观察器')
    }
    
    // 检查内存增长趋势
    if (this.memoryHistory.length >= 10) {
      const recent = this.memoryHistory.slice(-10)
      const trend = recent[recent.length - 1].usedJSHeapSize - recent[0].usedJSHeapSize
      
      if (trend > 10 * 1024 * 1024) { // 10MB增长
        potentialLeaks.push(`内存持续增长: ${(trend / 1024 / 1024).toFixed(2)}MB`)
        recommendations.push('检查是否有内存泄漏')
      }
    }
    
    return { potentialLeaks, recommendations }
  }
  
  // 注册回调
  onWarning(callback: (info: MemoryInfo) => void): () => void {
    this.callbacks.warning.add(callback)
    return () => this.callbacks.warning.delete(callback)
  }
  
  onCritical(callback: (info: MemoryInfo) => void): () => void {
    this.callbacks.critical.add(callback)
    return () => this.callbacks.critical.delete(callback)
  }
  
  onCleanup(callback: () => void): () => void {
    this.callbacks.cleanup.add(callback)
    return () => this.callbacks.cleanup.delete(callback)
  }
  
  // 获取内存泄漏检测器
  getLeakDetector(): MemoryLeakDetector {
    return this.leakDetector
  }
  
  // 生成内存报告
  generateReport(): string {
    const memoryInfo = this.getMemoryInfo()
    const leakInfo = this.detectMemoryLeaks()
    const stats = this.leakDetector.getStats()
    
    return `
内存使用报告
============
${memoryInfo ? `
当前内存使用:
- 已使用: ${(memoryInfo.usedJSHeapSize / 1024 / 1024).toFixed(2)}MB
- 总计: ${(memoryInfo.totalJSHeapSize / 1024 / 1024).toFixed(2)}MB
- 限制: ${(memoryInfo.jsHeapSizeLimit / 1024 / 1024).toFixed(2)}MB
- 使用率: ${memoryInfo.usedPercent.toFixed(2)}%
` : '内存信息不可用'}

资源统计:
- 事件监听器: ${stats.listeners}
- 定时器: ${stats.timers}
- 观察器: ${stats.observers}

潜在内存泄漏:
${leakInfo.potentialLeaks.length > 0 ? leakInfo.potentialLeaks.map(leak => `- ${leak}`).join('\n') : '- 未发现明显的内存泄漏'}

建议:
${leakInfo.recommendations.length > 0 ? leakInfo.recommendations.map(rec => `- ${rec}`).join('\n') : '- 内存使用正常'}
    `.trim()
  }
  
  // 销毁服务
  dispose(): void {
    this.stopMonitoring()
    this.leakDetector.cleanup()
    this.callbacks.warning.clear()
    this.callbacks.critical.clear()
    this.callbacks.cleanup.clear()
  }
}

// 导出单例实例
export const memoryService = new MemoryService()

// 导出默认实例
export default memoryService

// Vue组合式API
export function useMemory() {
  const startMonitoring = () => memoryService.startMonitoring()
  const stopMonitoring = () => memoryService.stopMonitoring()
  const getMemoryInfo = () => memoryService.getMemoryInfo()
  const getMemoryHistory = () => memoryService.getMemoryHistory()
  const performCleanup = () => memoryService.performCleanup()
  const detectMemoryLeaks = () => memoryService.detectMemoryLeaks()
  const generateReport = () => memoryService.generateReport()
  const getLeakDetector = () => memoryService.getLeakDetector()
  
  const onWarning = (callback: (info: MemoryInfo) => void) => memoryService.onWarning(callback)
  const onCritical = (callback: (info: MemoryInfo) => void) => memoryService.onCritical(callback)
  const onCleanup = (callback: () => void) => memoryService.onCleanup(callback)
  
  return {
    startMonitoring,
    stopMonitoring,
    getMemoryInfo,
    getMemoryHistory,
    performCleanup,
    detectMemoryLeaks,
    generateReport,
    getLeakDetector,
    onWarning,
    onCritical,
    onCleanup
  }
}