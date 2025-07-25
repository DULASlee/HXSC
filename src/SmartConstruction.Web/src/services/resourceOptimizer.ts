// 资源优化服务
interface OptimizationConfig {
  enableImageOptimization: boolean
  enableCSSOptimization: boolean
  enableJSOptimization: boolean
  enableCaching: boolean
  compressionLevel: number
}

interface ResourceInfo {
  url: string
  type: 'image' | 'css' | 'js' | 'font' | 'other'
  size: number
  optimizedSize?: number
  compressionRatio?: number
  loadTime: number
  cached: boolean
}

class ResourceOptimizerService {
  private config: OptimizationConfig = {
    enableImageOptimization: true,
    enableCSSOptimization: true,
    enableJSOptimization: true,
    enableCaching: true,
    compressionLevel: 6
  }

  private resources: Map<string, ResourceInfo> = new Map()
  private observer: IntersectionObserver | null = null

  constructor() {
    this.initIntersectionObserver()
    this.monitorResourceLoading()
  }

  // 初始化交叉观察器
  private initIntersectionObserver() {
    if ('IntersectionObserver' in window) {
      this.observer = new IntersectionObserver(
        (entries) => {
          entries.forEach(entry => {
            if (entry.isIntersecting) {
              this.optimizeVisibleResource(entry.target as HTMLElement)
            }
          })
        },
        {
          rootMargin: '50px',
          threshold: 0.1
        }
      )
    }
  }

  // 监控资源加载
  private monitorResourceLoading() {
    // 监控图片加载
    document.addEventListener('load', (e) => {
      const target = e.target as HTMLElement
      if (target.tagName === 'IMG') {
        this.recordResourceLoad(target as HTMLImageElement)
      }
    }, true)

    // 监控CSS加载
    document.addEventListener('load', (e) => {
      const target = e.target as HTMLElement
      if (target.tagName === 'LINK' && (target as HTMLLinkElement).rel === 'stylesheet') {
        this.recordResourceLoad(target as HTMLLinkElement)
      }
    }, true)

    // 监控JS加载
    document.addEventListener('load', (e) => {
      const target = e.target as HTMLElement
      if (target.tagName === 'SCRIPT') {
        this.recordResourceLoad(target as HTMLScriptElement)
      }
    }, true)
  }

  // 记录资源加载信息
  private recordResourceLoad(element: HTMLImageElement | HTMLLinkElement | HTMLScriptElement) {
    const url = this.getResourceUrl(element)
    if (!url) return

    const resourceInfo: ResourceInfo = {
      url,
      type: this.getResourceType(element),
      size: 0, // 实际项目中可以通过Performance API获取
      loadTime: performance.now(),
      cached: this.isResourceCached(url)
    }

    this.resources.set(url, resourceInfo)
  }

  // 获取资源URL
  private getResourceUrl(element: HTMLImageElement | HTMLLinkElement | HTMLScriptElement): string {
    if (element.tagName === 'IMG') {
      return (element as HTMLImageElement).src
    } else if (element.tagName === 'LINK') {
      return (element as HTMLLinkElement).href
    } else if (element.tagName === 'SCRIPT') {
      return (element as HTMLScriptElement).src
    }
    return ''
  }

  // 获取资源类型
  private getResourceType(element: HTMLElement): ResourceInfo['type'] {
    if (element.tagName === 'IMG') return 'image'
    if (element.tagName === 'LINK') return 'css'
    if (element.tagName === 'SCRIPT') return 'js'
    return 'other'
  }

  // 检查资源是否已缓存
  private isResourceCached(url: string): boolean {
    // 简单的缓存检测，实际项目中可以更复杂
    return performance.getEntriesByName(url).some(entry => 
      (entry as PerformanceResourceTiming).transferSize === 0
    )
  }

  // 优化可见资源
  private optimizeVisibleResource(element: HTMLElement) {
    if (element.tagName === 'IMG') {
      this.optimizeImage(element as HTMLImageElement)
    }
  }

  // 图片优化
  private optimizeImage(img: HTMLImageElement) {
    if (!this.config.enableImageOptimization) return

    // 懒加载
    if (img.dataset.src && !img.src) {
      img.src = img.dataset.src
      img.removeAttribute('data-src')
    }

    // 响应式图片
    this.applyResponsiveImage(img)

    // WebP支持检测
    if (this.supportsWebP()) {
      this.convertToWebP(img)
    }
  }

  // 应用响应式图片
  private applyResponsiveImage(img: HTMLImageElement) {
    const containerWidth = img.parentElement?.clientWidth || window.innerWidth
    const devicePixelRatio = window.devicePixelRatio || 1
    const targetWidth = containerWidth * devicePixelRatio

    // 如果图片宽度大于容器宽度的2倍，建议使用更小的图片
    if (img.naturalWidth > targetWidth * 2) {
      console.warn(`Image ${img.src} is too large for its container`)
    }
  }

  // 检测WebP支持
  private supportsWebP(): boolean {
    const canvas = document.createElement('canvas')
    canvas.width = 1
    canvas.height = 1
    return canvas.toDataURL('image/webp').indexOf('data:image/webp') === 0
  }

  // 转换为WebP格式
  private convertToWebP(img: HTMLImageElement) {
    const webpUrl = img.src.replace(/\.(jpg|jpeg|png)$/i, '.webp')
    
    // 检查WebP版本是否存在
    const testImg = new Image()
    testImg.onload = () => {
      img.src = webpUrl
    }
    testImg.onerror = () => {
      // WebP版本不存在，保持原格式
    }
    testImg.src = webpUrl
  }

  // CSS优化
  optimizeCSS() {
    if (!this.config.enableCSSOptimization) return

    const stylesheets = document.querySelectorAll('link[rel="stylesheet"]')
    stylesheets.forEach(link => {
      this.optimizeStylesheet(link as HTMLLinkElement)
    })

    // 移除未使用的CSS
    this.removeUnusedCSS()
  }

  // 优化样式表
  private optimizeStylesheet(link: HTMLLinkElement) {
    // 添加预加载提示
    if (!link.hasAttribute('rel')) {
      link.setAttribute('rel', 'preload')
      link.setAttribute('as', 'style')
    }

    // 添加媒体查询优化
    if (!link.media && link.href.includes('mobile')) {
      link.media = '(max-width: 768px)'
    }
  }

  // 移除未使用的CSS
  private removeUnusedCSS() {
    // 这里可以集成PurgeCSS或类似工具
    console.log('Analyzing unused CSS...')
  }

  // JavaScript优化
  optimizeJS() {
    if (!this.config.enableJSOptimization) return

    // 延迟加载非关键JavaScript
    this.deferNonCriticalJS()

    // 预加载关键JavaScript
    this.preloadCriticalJS()
  }

  // 延迟加载非关键JavaScript
  private deferNonCriticalJS() {
    const scripts = document.querySelectorAll('script[src]')
    scripts.forEach(script => {
      const scriptElement = script as HTMLScriptElement
      if (!scriptElement.hasAttribute('defer') && !scriptElement.hasAttribute('async')) {
        // 非关键脚本添加defer属性
        if (!this.isCriticalScript(scriptElement.src)) {
          scriptElement.defer = true
        }
      }
    })
  }

  // 判断是否为关键脚本
  private isCriticalScript(src: string): boolean {
    const criticalScripts = [
      'main.js',
      'app.js',
      'vendor.js',
      'polyfill'
    ]
    return criticalScripts.some(critical => src.includes(critical))
  }

  // 预加载关键JavaScript
  private preloadCriticalJS() {
    const criticalScripts = [
      '/js/vendor.js',
      '/js/app.js'
    ]

    criticalScripts.forEach(src => {
      const link = document.createElement('link')
      link.rel = 'preload'
      link.as = 'script'
      link.href = src
      document.head.appendChild(link)
    })
  }

  // 字体优化
  optimizeFonts() {
    const fontLinks = document.querySelectorAll('link[href*="fonts"]')
    fontLinks.forEach(link => {
      const linkElement = link as HTMLLinkElement
      
      // 添加字体显示策略
      if (!linkElement.hasAttribute('font-display')) {
        linkElement.style.fontDisplay = 'swap'
      }

      // 预连接字体服务
      if (linkElement.href.includes('fonts.googleapis.com')) {
        this.addPreconnect('https://fonts.gstatic.com')
      }
    })
  }

  // 添加预连接
  private addPreconnect(url: string) {
    if (!document.querySelector(`link[rel="preconnect"][href="${url}"]`)) {
      const link = document.createElement('link')
      link.rel = 'preconnect'
      link.href = url
      link.crossOrigin = 'anonymous'
      document.head.appendChild(link)
    }
  }

  // 启用资源压缩
  enableCompression() {
    // 这通常在服务器端配置，这里提供客户端检测
    const supportsGzip = this.checkCompressionSupport('gzip')
    const supportsBrotli = this.checkCompressionSupport('br')

    if (supportsBrotli) {
      console.log('Brotli compression is supported')
    } else if (supportsGzip) {
      console.log('Gzip compression is supported')
    }
  }

  // 检查压缩支持
  private checkCompressionSupport(encoding: string): boolean {
    return navigator.userAgent.toLowerCase().includes(encoding) ||
           document.documentElement.getAttribute('data-compression')?.includes(encoding) || false
  }

  // 观察元素进行优化
  observeElement(element: HTMLElement) {
    if (this.observer) {
      this.observer.observe(element)
    }
  }

  // 停止观察元素
  unobserveElement(element: HTMLElement) {
    if (this.observer) {
      this.observer.unobserve(element)
    }
  }

  // 获取资源统计
  getResourceStats() {
    const stats = {
      totalResources: this.resources.size,
      cachedResources: 0,
      totalSize: 0,
      optimizedSize: 0,
      averageLoadTime: 0,
      resourceTypes: {
        image: 0,
        css: 0,
        js: 0,
        font: 0,
        other: 0
      }
    }

    let totalLoadTime = 0

    this.resources.forEach(resource => {
      if (resource.cached) stats.cachedResources++
      stats.totalSize += resource.size
      if (resource.optimizedSize) {
        stats.optimizedSize += resource.optimizedSize
      }
      totalLoadTime += resource.loadTime
      stats.resourceTypes[resource.type]++
    })

    stats.averageLoadTime = totalLoadTime / this.resources.size || 0

    return stats
  }

  // 生成优化报告
  generateOptimizationReport(): string {
    const stats = this.getResourceStats()
    
    let report = '# Resource Optimization Report\n\n'
    
    report += `## Summary\n`
    report += `- Total Resources: ${stats.totalResources}\n`
    report += `- Cached Resources: ${stats.cachedResources} (${((stats.cachedResources / stats.totalResources) * 100).toFixed(2)}%)\n`
    report += `- Average Load Time: ${stats.averageLoadTime.toFixed(2)}ms\n\n`

    report += `## Resource Types\n`
    Object.entries(stats.resourceTypes).forEach(([type, count]) => {
      report += `- ${type}: ${count}\n`
    })

    report += `\n## Optimization Recommendations\n`
    
    if (stats.resourceTypes.image > 10) {
      report += `- Consider implementing image lazy loading for ${stats.resourceTypes.image} images\n`
    }
    
    if (stats.cachedResources / stats.totalResources < 0.5) {
      report += `- Low cache hit ratio (${((stats.cachedResources / stats.totalResources) * 100).toFixed(2)}%), consider improving caching strategy\n`
    }

    if (stats.averageLoadTime > 1000) {
      report += `- High average load time (${stats.averageLoadTime.toFixed(2)}ms), consider resource optimization\n`
    }

    return report
  }

  // 更新配置
  updateConfig(newConfig: Partial<OptimizationConfig>) {
    this.config = { ...this.config, ...newConfig }
  }

  // 获取配置
  getConfig(): OptimizationConfig {
    return { ...this.config }
  }

  // 销毁服务
  destroy() {
    if (this.observer) {
      this.observer.disconnect()
      this.observer = null
    }
    this.resources.clear()
  }
}

export const resourceOptimizer = new ResourceOptimizerService()
export default resourceOptimizer