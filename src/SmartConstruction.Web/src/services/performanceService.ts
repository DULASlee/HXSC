// =============================================
// 性能监控服务
// =============================================

// 性能指标接口
export interface PerformanceMetrics {
  // 页面加载性能
  pageLoadTime: number
  domContentLoadedTime: number
  firstContentfulPaint: number
  largestContentfulPaint: number
  firstInputDelay: number
  cumulativeLayoutShift: number
  
  // 资源加载性能
  resourceLoadTime: Record<string, number>
  
  // 内存使用情况
  memoryUsage?: {
    usedJSHeapSize: number
    totalJSHeapSize: number
    jsHeapSizeLimit: number
  }
  
  // 网络信息
  networkInfo?: {
    effectiveType: string
    downlink: number
    rtt: number
  }
  
  // 自定义性能指标
  customMetrics: Record<string, number>
}

// 性能监控类
class PerformanceService {
  private metrics: PerformanceMetrics
  private observers: Map<string, PerformanceObserver> = new Map()
  private customTimers: Map<string, number> = new Map()
  
  constructor() {
    this.metrics = {
      pageLoadTime: 0,
      domContentLoadedTime: 0,
      firstContentfulPaint: 0,
      largestContentfulPaint: 0,
      firstInputDelay: 0,
      cumulativeLayoutShift: 0,
      resourceLoadTime: {},
      customMetrics: {}
    }
    
    this.init()
  }
  
  // 初始化性能监控
  private init() {
    // 监控页面加载性能
    this.observePageLoad()
    
    // 监控Web Vitals
    this.observeWebVitals()
    
    // 监控资源加载
    this.observeResourceLoad()
    
    // 监控内存使用
    this.observeMemoryUsage()
    
    // 监控网络信息
    this.observeNetworkInfo()
  }
  
  // 监控页面加载性能
  private observePageLoad() {
    if (typeof window !== 'undefined' && 'performance' in window) {
      window.addEventListener('load', () => {
        const navigation = performance.getEntriesByType('navigation')[0] as PerformanceNavigationTiming
        
        if (navigation) {
          this.metrics.pageLoadTime = navigation.loadEventEnd - navigation.fetchStart
          this.metrics.domContentLoadedTime = navigation.domContentLoadedEventEnd - navigation.fetchStart
        }
      })
    }
  }
  
  // 监控Web Vitals
  private observeWebVitals() {
    if (typeof window !== 'undefined' && 'PerformanceObserver' in window) {
      // First Contentful Paint (FCP)
      const fcpObserver = new PerformanceObserver((list) => {
        const entries = list.getEntries()
        const fcpEntry = entries.find(entry => entry.name === 'first-contentful-paint')
        if (fcpEntry) {
          this.metrics.firstContentfulPaint = fcpEntry.startTime
        }
      })
      
      try {
        fcpObserver.observe({ entryTypes: ['paint'] })
        this.observers.set('fcp', fcpObserver)
      } catch (e) {
        console.warn('FCP observer not supported')
      }
      
      // Largest Contentful Paint (LCP)
      const lcpObserver = new PerformanceObserver((list) => {
        const entries = list.getEntries()
        const lastEntry = entries[entries.length - 1]
        if (lastEntry) {
          this.metrics.largestContentfulPaint = lastEntry.startTime
        }
      })
      
      try {
        lcpObserver.observe({ entryTypes: ['largest-contentful-paint'] })
        this.observers.set('lcp', lcpObserver)
      } catch (e) {
        console.warn('LCP observer not supported')
      }
      
      // First Input Delay (FID)
      const fidObserver = new PerformanceObserver((list) => {
        const entries = list.getEntries()
        entries.forEach((entry: any) => {
          if (entry.processingStart && entry.startTime) {
            this.metrics.firstInputDelay = entry.processingStart - entry.startTime
          }
        })
      })
      
      try {
        fidObserver.observe({ entryTypes: ['first-input'] })
        this.observers.set('fid', fidObserver)
      } catch (e) {
        console.warn('FID observer not supported')
      }
      
      // Cumulative Layout Shift (CLS)
      const clsObserver = new PerformanceObserver((list) => {
        let clsValue = 0
        const entries = list.getEntries()
        entries.forEach((entry: any) => {
          if (!entry.hadRecentInput) {
            clsValue += entry.value
          }
        })
        this.metrics.cumulativeLayoutShift = clsValue
      })
      
      try {
        clsObserver.observe({ entryTypes: ['layout-shift'] })
        this.observers.set('cls', clsObserver)
      } catch (e) {
        console.warn('CLS observer not supported')
      }
    }
  }
  
  // 监控资源加载
  private observeResourceLoad() {
    if (typeof window !== 'undefined' && 'PerformanceObserver' in window) {
      const resourceObserver = new PerformanceObserver((list) => {
        const entries = list.getEntries()
        entries.forEach((entry) => {
          if (entry.name && entry.duration) {
            this.metrics.resourceLoadTime[entry.name] = entry.duration
          }
        })
      })
      
      try {
        resourceObserver.observe({ entryTypes: ['resource'] })
        this.observers.set('resource', resourceObserver)
      } catch (e) {
        console.warn('Resource observer not supported')
      }
    }
  }
  
  // 监控内存使用
  private observeMemoryUsage() {
    if (typeof window !== 'undefined' && 'performance' in window && 'memory' in (performance as any)) {
      const updateMemoryUsage = () => {
        const memory = (performance as any).memory
        this.metrics.memoryUsage = {
          usedJSHeapSize: memory.usedJSHeapSize,
          totalJSHeapSize: memory.totalJSHeapSize,
          jsHeapSizeLimit: memory.jsHeapSizeLimit
        }
      }
      
      // 定期更新内存使用情况
      setInterval(updateMemoryUsage, 5000)
      updateMemoryUsage()
    }
  }
  
  // 监控网络信息
  private observeNetworkInfo() {
    if (typeof window !== 'undefined' && 'navigator' in window && 'connection' in navigator) {
      const connection = (navigator as any).connection
      if (connection) {
        this.metrics.networkInfo = {
          effectiveType: connection.effectiveType || 'unknown',
          downlink: connection.downlink || 0,
          rtt: connection.rtt || 0
        }
        
        // 监听网络变化
        connection.addEventListener('change', () => {
          this.metrics.networkInfo = {
            effectiveType: connection.effectiveType || 'unknown',
            downlink: connection.downlink || 0,
            rtt: connection.rtt || 0
          }
        })
      }
    }
  }
  
  // 开始自定义计时
  public startTimer(name: string) {
    this.customTimers.set(name, performance.now())
  }
  
  // 结束自定义计时
  public endTimer(name: string): number {
    const startTime = this.customTimers.get(name)
    if (startTime) {
      const duration = performance.now() - startTime
      this.metrics.customMetrics[name] = duration
      this.customTimers.delete(name)
      return duration
    }
    return 0
  }
  
  // 记录自定义指标
  public recordMetric(name: string, value: number) {
    this.metrics.customMetrics[name] = value
  }
  
  // 获取性能指标
  public getMetrics(): PerformanceMetrics {
    return { ...this.metrics }
  }
  
  // 获取性能评分
  public getPerformanceScore(): {
    score: number
    details: Record<string, { score: number; value: number; threshold: number }>
  } {
    const details = {
      fcp: {
        score: this.scoreMetric(this.metrics.firstContentfulPaint, 1800, 3000),
        value: this.metrics.firstContentfulPaint,
        threshold: 1800
      },
      lcp: {
        score: this.scoreMetric(this.metrics.largestContentfulPaint, 2500, 4000),
        value: this.metrics.largestContentfulPaint,
        threshold: 2500
      },
      fid: {
        score: this.scoreMetric(this.metrics.firstInputDelay, 100, 300),
        value: this.metrics.firstInputDelay,
        threshold: 100
      },
      cls: {
        score: this.scoreMetric(this.metrics.cumulativeLayoutShift, 0.1, 0.25, true),
        value: this.metrics.cumulativeLayoutShift,
        threshold: 0.1
      }
    }
    
    const totalScore = Object.values(details).reduce((sum, item) => sum + item.score, 0) / 4
    
    return {
      score: Math.round(totalScore * 100),
      details
    }
  }
  
  // 计算指标评分
  private scoreMetric(value: number, goodThreshold: number, poorThreshold: number, lowerIsBetter = false): number {
    if (value === 0) return 0
    
    if (lowerIsBetter) {
      if (value <= goodThreshold) return 1
      if (value >= poorThreshold) return 0
      return 1 - (value - goodThreshold) / (poorThreshold - goodThreshold)
    } else {
      if (value <= goodThreshold) return 1
      if (value >= poorThreshold) return 0
      return 1 - (value - goodThreshold) / (poorThreshold - goodThreshold)
    }
  }
  
  // 生成性能报告
  public generateReport(): string {
    const metrics = this.getMetrics()
    const score = this.getPerformanceScore()
    
    return `
性能报告
========
总体评分: ${score.score}/100

核心指标:
- 首次内容绘制 (FCP): ${metrics.firstContentfulPaint.toFixed(2)}ms
- 最大内容绘制 (LCP): ${metrics.largestContentfulPaint.toFixed(2)}ms
- 首次输入延迟 (FID): ${metrics.firstInputDelay.toFixed(2)}ms
- 累积布局偏移 (CLS): ${metrics.cumulativeLayoutShift.toFixed(3)}

页面加载:
- 页面加载时间: ${metrics.pageLoadTime.toFixed(2)}ms
- DOM内容加载时间: ${metrics.domContentLoadedTime.toFixed(2)}ms

内存使用:
${metrics.memoryUsage ? `
- 已使用堆内存: ${(metrics.memoryUsage.usedJSHeapSize / 1024 / 1024).toFixed(2)}MB
- 总堆内存: ${(metrics.memoryUsage.totalJSHeapSize / 1024 / 1024).toFixed(2)}MB
- 堆内存限制: ${(metrics.memoryUsage.jsHeapSizeLimit / 1024 / 1024).toFixed(2)}MB
` : '- 内存信息不可用'}

网络信息:
${metrics.networkInfo ? `
- 网络类型: ${metrics.networkInfo.effectiveType}
- 下行速度: ${metrics.networkInfo.downlink}Mbps
- 往返时间: ${metrics.networkInfo.rtt}ms
` : '- 网络信息不可用'}

自定义指标:
${Object.entries(metrics.customMetrics).map(([name, value]) => `- ${name}: ${value.toFixed(2)}ms`).join('\n')}
    `.trim()
  }
  
  // 清理资源
  public dispose() {
    this.observers.forEach((observer) => {
      observer.disconnect()
    })
    this.observers.clear()
    this.customTimers.clear()
  }
}

// 导出单例实例
export const performanceService = new PerformanceService()

// 导出默认实例
export default performanceService

// Vue组合式API
export function usePerformance() {
  const startTimer = (name: string) => performanceService.startTimer(name)
  const endTimer = (name: string) => performanceService.endTimer(name)
  const recordMetric = (name: string, value: number) => performanceService.recordMetric(name, value)
  const getMetrics = () => performanceService.getMetrics()
  const getPerformanceScore = () => performanceService.getPerformanceScore()
  const generateReport = () => performanceService.generateReport()
  
  return {
    startTimer,
    endTimer,
    recordMetric,
    getMetrics,
    getPerformanceScore,
    generateReport
  }
}