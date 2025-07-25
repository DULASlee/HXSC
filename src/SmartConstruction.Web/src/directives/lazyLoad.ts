// =============================================
// 懒加载指令
// =============================================
import type { Directive, DirectiveBinding } from 'vue'

// 懒加载选项
interface LazyLoadOptions {
  loading?: string // 加载中的占位图
  error?: string // 加载失败的占位图
  threshold?: number // 触发加载的阈值
  rootMargin?: string // 根边距
  delay?: number // 延迟加载时间
  attempt?: number // 重试次数
}

// 默认选项
const defaultOptions: LazyLoadOptions = {
  loading: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KICA8cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjVmNWY1Ii8+CiAgPHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIxNCIgZmlsbD0iIzk5OSIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPkxvYWRpbmcuLi48L3RleHQ+Cjwvc3ZnPg==',
  error: 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KICA8cmVjdCB3aWR0aD0iMTAwJSIgaGVpZ2h0PSIxMDAlIiBmaWxsPSIjZjVmNWY1Ii8+CiAgPHRleHQgeD0iNTAlIiB5PSI1MCUiIGZvbnQtZmFtaWx5PSJBcmlhbCwgc2Fucy1zZXJpZiIgZm9udC1zaXplPSIxNCIgZmlsbD0iI2ZmNjY2NiIgdGV4dC1hbmNob3I9Im1pZGRsZSIgZHk9Ii4zZW0iPkVycm9yPC90ZXh0Pgo8L3N2Zz4=',
  threshold: 0.1,
  rootMargin: '50px',
  delay: 0,
  attempt: 3
}

// 懒加载状态
enum LazyLoadState {
  LOADING = 'loading',
  LOADED = 'loaded',
  ERROR = 'error'
}

// 懒加载管理器
class LazyLoadManager {
  private observer: IntersectionObserver | null = null
  private elements = new WeakMap<Element, {
    src: string
    options: LazyLoadOptions
    state: LazyLoadState
    attempts: number
  }>()
  
  constructor() {
    this.createObserver()
  }
  
  // 创建观察器
  private createObserver() {
    if (!('IntersectionObserver' in window)) {
      // 不支持IntersectionObserver，直接加载所有图片
      return
    }
    
    this.observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          this.loadElement(entry.target)
        }
      })
    }, {
      threshold: defaultOptions.threshold,
      rootMargin: defaultOptions.rootMargin
    })
  }
  
  // 观察元素
  observe(element: Element, src: string, options: LazyLoadOptions = {}) {
    const mergedOptions = { ...defaultOptions, ...options }
    
    // 存储元素信息
    this.elements.set(element, {
      src,
      options: mergedOptions,
      state: LazyLoadState.LOADING,
      attempts: 0
    })
    
    // 设置加载中的占位图
    if (element instanceof HTMLImageElement && mergedOptions.loading) {
      element.src = mergedOptions.loading
    }
    
    // 添加CSS类
    element.classList.add('lazy-loading')
    
    if (this.observer) {
      this.observer.observe(element)
    } else {
      // 不支持IntersectionObserver，直接加载
      this.loadElement(element)
    }
  }
  
  // 取消观察元素
  unobserve(element: Element) {
    if (this.observer) {
      this.observer.unobserve(element)
    }
    this.elements.delete(element)
  }
  
  // 加载元素
  private async loadElement(element: Element) {
    const elementData = this.elements.get(element)
    if (!elementData || elementData.state === LazyLoadState.LOADED) {
      return
    }
    
    const { src, options, attempts } = elementData
    
    try {
      // 延迟加载
      if (options.delay && options.delay > 0) {
        await new Promise(resolve => setTimeout(resolve, options.delay))
      }
      
      // 预加载图片
      await this.preloadImage(src)
      
      // 设置实际图片
      if (element instanceof HTMLImageElement) {
        element.src = src
      } else if (element instanceof HTMLElement) {
        element.style.backgroundImage = `url(${src})`
      }
      
      // 更新状态
      elementData.state = LazyLoadState.LOADED
      element.classList.remove('lazy-loading')
      element.classList.add('lazy-loaded')
      
      // 触发加载完成事件
      element.dispatchEvent(new CustomEvent('lazy-loaded', { detail: { src } }))
      
      // 停止观察
      if (this.observer) {
        this.observer.unobserve(element)
      }
      
    } catch (error) {
      console.warn('Lazy load failed:', src, error)
      
      // 增加重试次数
      elementData.attempts++
      
      // 检查是否还能重试
      if (elementData.attempts < (options.attempt || defaultOptions.attempt!)) {
        // 延迟后重试
        setTimeout(() => {
          this.loadElement(element)
        }, 1000 * elementData.attempts)
      } else {
        // 重试次数用完，显示错误图片
        elementData.state = LazyLoadState.ERROR
        
        if (element instanceof HTMLImageElement && options.error) {
          element.src = options.error
        }
        
        element.classList.remove('lazy-loading')
        element.classList.add('lazy-error')
        
        // 触发加载失败事件
        element.dispatchEvent(new CustomEvent('lazy-error', { detail: { src, error } }))
        
        // 停止观察
        if (this.observer) {
          this.observer.unobserve(element)
        }
      }
    }
  }
  
  // 预加载图片
  private preloadImage(src: string): Promise<void> {
    return new Promise((resolve, reject) => {
      const img = new Image()
      img.onload = () => resolve()
      img.onerror = reject
      img.src = src
    })
  }
  
  // 销毁管理器
  destroy() {
    if (this.observer) {
      this.observer.disconnect()
      this.observer = null
    }
    this.elements = new WeakMap()
  }
}

// 创建全局管理器实例
const lazyLoadManager = new LazyLoadManager()

// 懒加载指令
export const vLazyLoad: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value, modifiers } = binding
    
    let src: string
    let options: LazyLoadOptions = {}
    
    // 解析绑定值
    if (typeof value === 'string') {
      src = value
    } else if (typeof value === 'object') {
      src = value.src
      options = { ...value }
      delete options.src
    } else {
      console.warn('v-lazy-load: Invalid binding value')
      return
    }
    
    // 解析修饰符
    if (modifiers.immediate) {
      options.delay = 0
    }
    
    if (modifiers.once) {
      options.attempt = 1
    }
    
    // 开始观察
    lazyLoadManager.observe(el, src, options)
  },
  
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const { value, oldValue } = binding
    
    // 如果src没有变化，不需要重新加载
    if (value === oldValue) {
      return
    }
    
    // 取消旧的观察
    lazyLoadManager.unobserve(el)
    
    // 重新开始观察
    this.mounted!(el, binding, null as any, null as any)
  },
  
  unmounted(el: HTMLElement) {
    lazyLoadManager.unobserve(el)
  }
}

// 懒加载组件指令
export const vLazyComponent: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    
    if (!('IntersectionObserver' in window)) {
      // 不支持IntersectionObserver，直接显示组件
      el.style.display = ''
      return
    }
    
    // 初始隐藏组件
    el.style.display = 'none'
    
    const observer = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting) {
          // 进入视口，显示组件
          el.style.display = ''
          el.classList.add('lazy-component-loaded')
          
          // 触发事件
          if (typeof value === 'function') {
            value()
          }
          
          // 停止观察
          observer.unobserve(el)
        }
      })
    }, {
      threshold: 0.1,
      rootMargin: '50px'
    })
    
    observer.observe(el)
    
    // 保存observer到元素上，用于清理
    ;(el as any)._lazyObserver = observer
  },
  
  unmounted(el: HTMLElement) {
    const observer = (el as any)._lazyObserver
    if (observer) {
      observer.disconnect()
      delete (el as any)._lazyObserver
    }
  }
}

// 导出管理器（用于手动控制）
export { lazyLoadManager }

// 默认导出
export default {
  install(app: any) {
    app.directive('lazy-load', vLazyLoad)
    app.directive('lazy-component', vLazyComponent)
  }
}