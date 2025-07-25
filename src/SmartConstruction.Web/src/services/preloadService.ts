// =============================================
// 资源预加载服务
// =============================================

// 预加载类型
type PreloadType = 'script' | 'style' | 'image' | 'font' | 'fetch'

// 预加载选项
interface PreloadOptions {
  priority?: 'high' | 'low' | 'auto'
  crossorigin?: 'anonymous' | 'use-credentials'
  integrity?: string
  as?: string
  type?: string
}

// 预加载项
interface PreloadItem {
  url: string
  type: PreloadType
  options?: PreloadOptions
  loaded: boolean
  loading: boolean
  error?: Error
}

// 预加载服务
class PreloadService {
  private items = new Map<string, PreloadItem>()
  private loadingPromises = new Map<string, Promise<void>>()
  
  // 预加载脚本
  async preloadScript(url: string, options?: PreloadOptions): Promise<void> {
    return this.preload(url, 'script', options)
  }
  
  // 预加载样式
  async preloadStyle(url: string, options?: PreloadOptions): Promise<void> {
    return this.preload(url, 'style', options)
  }
  
  // 预加载图片
  async preloadImage(url: string, options?: PreloadOptions): Promise<void> {
    return this.preload(url, 'image', options)
  }
  
  // 预加载字体
  async preloadFont(url: string, options?: PreloadOptions): Promise<void> {
    return this.preload(url, 'font', {
      ...options,
      as: 'font',
      crossorigin: options?.crossorigin || 'anonymous'
    })
  }
  
  // 预加载数据
  async preloadData(url: string, options?: PreloadOptions): Promise<void> {
    return this.preload(url, 'fetch', options)
  }
  
  // 通用预加载方法
  private async preload(url: string, type: PreloadType, options?: PreloadOptions): Promise<void> {
    // 如果已经加载过，直接返回
    const existingItem = this.items.get(url)
    if (existingItem?.loaded) {
      return Promise.resolve()
    }
    
    // 如果正在加载，返回现有的Promise
    if (existingItem?.loading && this.loadingPromises.has(url)) {
      return this.loadingPromises.get(url)!
    }
    
    // 创建预加载项
    const item: PreloadItem = {
      url,
      type,
      options,
      loaded: false,
      loading: true
    }
    
    this.items.set(url, item)
    
    // 创建加载Promise
    const loadingPromise = this.createLoadingPromise(item)
    this.loadingPromises.set(url, loadingPromise)
    
    try {
      await loadingPromise
      item.loaded = true
      item.loading = false
    } catch (error) {
      item.error = error as Error
      item.loading = false
      throw error
    } finally {
      this.loadingPromises.delete(url)
    }
  }
  
  // 创建加载Promise
  private createLoadingPromise(item: PreloadItem): Promise<void> {
    switch (item.type) {
      case 'script':
        return this.loadScript(item)
      case 'style':
        return this.loadStyle(item)
      case 'image':
        return this.loadImage(item)
      case 'font':
        return this.loadFont(item)
      case 'fetch':
        return this.loadData(item)
      default:
        return Promise.reject(new Error(`Unsupported preload type: ${item.type}`))
    }
  }
  
  // 加载脚本
  private loadScript(item: PreloadItem): Promise<void> {
    return new Promise((resolve, reject) => {
      // 检查是否已经存在
      const existing = document.querySelector(`script[src="${item.url}"]`)
      if (existing) {
        resolve()
        return
      }
      
      const script = document.createElement('script')
      script.src = item.url
      script.async = true
      
      if (item.options?.crossorigin) {
        script.crossOrigin = item.options.crossorigin
      }
      
      if (item.options?.integrity) {
        script.integrity = item.options.integrity
      }
      
      script.onload = () => resolve()
      script.onerror = () => reject(new Error(`Failed to load script: ${item.url}`))
      
      document.head.appendChild(script)
    })
  }
  
  // 加载样式
  private loadStyle(item: PreloadItem): Promise<void> {
    return new Promise((resolve, reject) => {
      // 检查是否已经存在
      const existing = document.querySelector(`link[href="${item.url}"]`)
      if (existing) {
        resolve()
        return
      }
      
      const link = document.createElement('link')
      link.rel = 'stylesheet'
      link.href = item.url
      
      if (item.options?.crossorigin) {
        link.crossOrigin = item.options.crossorigin
      }
      
      if (item.options?.integrity) {
        link.integrity = item.options.integrity
      }
      
      link.onload = () => resolve()
      link.onerror = () => reject(new Error(`Failed to load style: ${item.url}`))
      
      document.head.appendChild(link)
    })
  }
  
  // 加载图片
  private loadImage(item: PreloadItem): Promise<void> {
    return new Promise((resolve, reject) => {
      const img = new Image()
      
      if (item.options?.crossorigin) {
        img.crossOrigin = item.options.crossorigin
      }
      
      img.onload = () => resolve()
      img.onerror = () => reject(new Error(`Failed to load image: ${item.url}`))
      
      img.src = item.url
    })
  }
  
  // 加载字体
  private loadFont(item: PreloadItem): Promise<void> {
    return new Promise((resolve, reject) => {
      const link = document.createElement('link')
      link.rel = 'preload'
      link.as = 'font'
      link.href = item.url
      link.crossOrigin = item.options?.crossorigin || 'anonymous'
      
      if (item.options?.type) {
        link.type = item.options.type
      }
      
      link.onload = () => resolve()
      link.onerror = () => reject(new Error(`Failed to load font: ${item.url}`))
      
      document.head.appendChild(link)
    })
  }
  
  // 加载数据
  private loadData(item: PreloadItem): Promise<void> {
    const fetchOptions: RequestInit = {}
    
    if (item.options?.crossorigin) {
      fetchOptions.mode = item.options.crossorigin === 'anonymous' ? 'cors' : 'same-origin'
    }
    
    if (item.options?.integrity) {
      fetchOptions.integrity = item.options.integrity
    }
    
    return fetch(item.url, fetchOptions)
      .then(response => {
        if (!response.ok) {
          throw new Error(`HTTP ${response.status}: ${response.statusText}`)
        }
        return response.blob() // 预加载但不解析内容
      })
      .then(() => {
        // 数据已预加载到浏览器缓存
      })
  }
  
  // 批量预加载
  async preloadBatch(items: Array<{
    url: string
    type: PreloadType
    options?: PreloadOptions
  }>): Promise<void> {
    const promises = items.map(item => this.preload(item.url, item.type, item.options))
    await Promise.allSettled(promises)
  }
  
  // 预加载路由组件
  async preloadRouteComponents(routes: string[]): Promise<void> {
    const promises = routes.map(route => {
      // 假设路由组件在 /src/views/ 目录下
      const componentPath = `/src/views${route}/index.vue`
      return this.preloadScript(componentPath)
    })
    
    await Promise.allSettled(promises)
  }
  
  // 智能预加载（基于用户行为）
  enableSmartPreload(): void {
    // 预加载鼠标悬停的链接
    document.addEventListener('mouseover', (event) => {
      const target = event.target as HTMLElement
      const link = target.closest('a[href]') as HTMLAnchorElement
      
      if (link && link.href && !link.href.startsWith('javascript:')) {
        // 预加载链接指向的页面资源
        this.preloadData(link.href).catch(() => {
          // 忽略预加载失败
        })
      }
    })
    
    // 预加载即将进入视口的图片
    if ('IntersectionObserver' in window) {
      const imageObserver = new IntersectionObserver((entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            const img = entry.target as HTMLImageElement
            const dataSrc = img.dataset.src
            
            if (dataSrc) {
              this.preloadImage(dataSrc).then(() => {
                img.src = dataSrc
                img.removeAttribute('data-src')
                imageObserver.unobserve(img)
              }).catch(() => {
                // 预加载失败，直接设置src
                img.src = dataSrc
                img.removeAttribute('data-src')
                imageObserver.unobserve(img)
              })
            }
          }
        })
      }, {
        rootMargin: '50px' // 提前50px开始预加载
      })
      
      // 观察所有带有data-src的图片
      document.querySelectorAll('img[data-src]').forEach((img) => {
        imageObserver.observe(img)
      })
    }
  }
  
  // 获取预加载状态
  getStatus(url: string): PreloadItem | undefined {
    return this.items.get(url)
  }
  
  // 获取所有预加载项的状态
  getAllStatus(): Map<string, PreloadItem> {
    return new Map(this.items)
  }
  
  // 清理已加载的项
  cleanup(): void {
    for (const [url, item] of this.items.entries()) {
      if (item.loaded || item.error) {
        this.items.delete(url)
      }
    }
  }
  
  // 清空所有预加载项
  clear(): void {
    this.items.clear()
    this.loadingPromises.clear()
  }
}

// 导出单例实例
export const preloadService = new PreloadService()

// 导出默认实例
export default preloadService

// Vue组合式API
export function usePreload() {
  const preloadScript = (url: string, options?: PreloadOptions) => 
    preloadService.preloadScript(url, options)
  
  const preloadStyle = (url: string, options?: PreloadOptions) => 
    preloadService.preloadStyle(url, options)
  
  const preloadImage = (url: string, options?: PreloadOptions) => 
    preloadService.preloadImage(url, options)
  
  const preloadFont = (url: string, options?: PreloadOptions) => 
    preloadService.preloadFont(url, options)
  
  const preloadData = (url: string, options?: PreloadOptions) => 
    preloadService.preloadData(url, options)
  
  const preloadBatch = (items: Array<{
    url: string
    type: PreloadType
    options?: PreloadOptions
  }>) => preloadService.preloadBatch(items)
  
  const getStatus = (url: string) => preloadService.getStatus(url)
  const getAllStatus = () => preloadService.getAllStatus()
  const cleanup = () => preloadService.cleanup()
  
  return {
    preloadScript,
    preloadStyle,
    preloadImage,
    preloadFont,
    preloadData,
    preloadBatch,
    getStatus,
    getAllStatus,
    cleanup
  }
}