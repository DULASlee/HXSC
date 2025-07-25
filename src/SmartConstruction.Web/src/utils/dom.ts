// =============================================
// DOM操作工具函数
// =============================================

/**
 * 获取元素
 */
export function $(selector: string): HTMLElement | null {
  return document.querySelector(selector)
}

/**
 * 获取多个元素
 */
export function $$(selector: string): NodeListOf<HTMLElement> {
  return document.querySelectorAll(selector)
}

/**
 * 添加类名
 */
export function addClass(element: HTMLElement, className: string): void {
  element.classList.add(className)
}

/**
 * 移除类名
 */
export function removeClass(element: HTMLElement, className: string): void {
  element.classList.remove(className)
}

/**
 * 切换类名
 */
export function toggleClass(element: HTMLElement, className: string): void {
  element.classList.toggle(className)
}

/**
 * 检查是否包含类名
 */
export function hasClass(element: HTMLElement, className: string): boolean {
  return element.classList.contains(className)
}

/**
 * 设置样式
 */
export function setStyle(element: HTMLElement, styles: Partial<CSSStyleDeclaration>): void {
  Object.assign(element.style, styles)
}

/**
 * 获取样式
 */
export function getStyle(element: HTMLElement, property: string): string {
  return window.getComputedStyle(element).getPropertyValue(property)
}

/**
 * 设置属性
 */
export function setAttribute(element: HTMLElement, name: string, value: string): void {
  element.setAttribute(name, value)
}

/**
 * 获取属性
 */
export function getAttribute(element: HTMLElement, name: string): string | null {
  return element.getAttribute(name)
}

/**
 * 移除属性
 */
export function removeAttribute(element: HTMLElement, name: string): void {
  element.removeAttribute(name)
}

/**
 * 创建元素
 */
export function createElement<K extends keyof HTMLElementTagNameMap>(
  tagName: K,
  options?: {
    className?: string
    id?: string
    innerHTML?: string
    textContent?: string
    attributes?: Record<string, string>
    styles?: Partial<CSSStyleDeclaration>
  }
): HTMLElementTagNameMap[K] {
  const element = document.createElement(tagName)

  if (options) {
    if (options.className) element.className = options.className
    if (options.id) element.id = options.id
    if (options.innerHTML) element.innerHTML = options.innerHTML
    if (options.textContent) element.textContent = options.textContent
    
    if (options.attributes) {
      Object.entries(options.attributes).forEach(([name, value]) => {
        element.setAttribute(name, value)
      })
    }
    
    if (options.styles) {
      Object.assign(element.style, options.styles)
    }
  }

  return element
}

/**
 * 插入元素
 */
export function insertElement(
  parent: HTMLElement,
  child: HTMLElement,
  position: 'beforebegin' | 'afterbegin' | 'beforeend' | 'afterend' = 'beforeend'
): void {
  parent.insertAdjacentElement(position, child)
}

/**
 * 移除元素
 */
export function removeElement(element: HTMLElement): void {
  element.parentNode?.removeChild(element)
}

/**
 * 获取元素位置
 */
export function getElementPosition(element: HTMLElement): { top: number; left: number; width: number; height: number } {
  const rect = element.getBoundingClientRect()
  return {
    top: rect.top + window.scrollY,
    left: rect.left + window.scrollX,
    width: rect.width,
    height: rect.height
  }
}

/**
 * 获取元素相对于视口的位置
 */
export function getElementViewportPosition(element: HTMLElement): DOMRect {
  return element.getBoundingClientRect()
}

/**
 * 检查元素是否在视口中
 */
export function isElementInViewport(element: HTMLElement): boolean {
  const rect = element.getBoundingClientRect()
  return (
    rect.top >= 0 &&
    rect.left >= 0 &&
    rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
    rect.right <= (window.innerWidth || document.documentElement.clientWidth)
  )
}

/**
 * 滚动到元素
 */
export function scrollToElement(
  element: HTMLElement,
  options: ScrollIntoViewOptions = { behavior: 'smooth', block: 'center' }
): void {
  element.scrollIntoView(options)
}

/**
 * 滚动到顶部
 */
export function scrollToTop(behavior: ScrollBehavior = 'smooth'): void {
  window.scrollTo({ top: 0, behavior })
}

/**
 * 滚动到底部
 */
export function scrollToBottom(behavior: ScrollBehavior = 'smooth'): void {
  window.scrollTo({ top: document.body.scrollHeight, behavior })
}

/**
 * 获取滚动位置
 */
export function getScrollPosition(): { x: number; y: number } {
  return {
    x: window.pageXOffset || document.documentElement.scrollLeft,
    y: window.pageYOffset || document.documentElement.scrollTop
  }
}

/**
 * 设置滚动位置
 */
export function setScrollPosition(x: number, y: number, behavior: ScrollBehavior = 'auto'): void {
  window.scrollTo({ left: x, top: y, behavior })
}

/**
 * 添加事件监听器
 */
export function addEventListener<K extends keyof HTMLElementEventMap>(
  element: HTMLElement,
  type: K,
  listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any,
  options?: boolean | AddEventListenerOptions
): void {
  element.addEventListener(type, listener, options)
}

/**
 * 移除事件监听器
 */
export function removeEventListener<K extends keyof HTMLElementEventMap>(
  element: HTMLElement,
  type: K,
  listener: (this: HTMLElement, ev: HTMLElementEventMap[K]) => any,
  options?: boolean | EventListenerOptions
): void {
  element.removeEventListener(type, listener, options)
}

/**
 * 触发事件
 */
export function dispatchEvent(element: HTMLElement, eventType: string, detail?: any): void {
  const event = new CustomEvent(eventType, { detail })
  element.dispatchEvent(event)
}

/**
 * 获取表单数据
 */
export function getFormData(form: HTMLFormElement): Record<string, any> {
  const formData = new FormData(form)
  const data: Record<string, any> = {}
  
  formData.forEach((value, key) => {
    if (data[key]) {
      if (Array.isArray(data[key])) {
        data[key].push(value)
      } else {
        data[key] = [data[key], value]
      }
    } else {
      data[key] = value
    }
  })
  
  return data
}

/**
 * 设置表单数据
 */
export function setFormData(form: HTMLFormElement, data: Record<string, any>): void {
  Object.entries(data).forEach(([key, value]) => {
    const element = form.elements.namedItem(key) as HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement
    if (element) {
      if (element.type === 'checkbox' || element.type === 'radio') {
        (element as HTMLInputElement).checked = Boolean(value)
      } else {
        element.value = String(value)
      }
    }
  })
}

/**
 * 复制文本到剪贴板
 */
export async function copyToClipboard(text: string): Promise<boolean> {
  try {
    if (navigator.clipboard) {
      await navigator.clipboard.writeText(text)
      return true
    } else {
      // 降级方案
      const textArea = createElement('textarea', {
        textContent: text,
        styles: {
          position: 'fixed',
          left: '-999999px',
          top: '-999999px'
        }
      })
      
      document.body.appendChild(textArea)
      textArea.focus()
      textArea.select()
      
      const successful = document.execCommand('copy')
      document.body.removeChild(textArea)
      
      return successful
    }
  } catch (error) {
    console.error('复制到剪贴板失败:', error)
    return false
  }
}

/**
 * 下载文件
 */
export function downloadFile(url: string, filename?: string): void {
  const link = createElement('a', {
    attributes: {
      href: url,
      download: filename || ''
    },
    styles: {
      display: 'none'
    }
  })
  
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

/**
 * 全屏操作
 */
export const fullscreen = {
  /**
   * 进入全屏
   */
  enter(element: HTMLElement = document.documentElement): Promise<void> {
    if (element.requestFullscreen) {
      return element.requestFullscreen()
    } else if ((element as any).webkitRequestFullscreen) {
      return (element as any).webkitRequestFullscreen()
    } else if ((element as any).msRequestFullscreen) {
      return (element as any).msRequestFullscreen()
    }
    return Promise.reject(new Error('Fullscreen not supported'))
  },

  /**
   * 退出全屏
   */
  exit(): Promise<void> {
    if (document.exitFullscreen) {
      return document.exitFullscreen()
    } else if ((document as any).webkitExitFullscreen) {
      return (document as any).webkitExitFullscreen()
    } else if ((document as any).msExitFullscreen) {
      return (document as any).msExitFullscreen()
    }
    return Promise.reject(new Error('Exit fullscreen not supported'))
  },

  /**
   * 切换全屏
   */
  toggle(element: HTMLElement = document.documentElement): Promise<void> {
    return this.isActive() ? this.exit() : this.enter(element)
  },

  /**
   * 检查是否处于全屏状态
   */
  isActive(): boolean {
    return !!(
      document.fullscreenElement ||
      (document as any).webkitFullscreenElement ||
      (document as any).msFullscreenElement
    )
  }
}

/**
 * 获取设备信息
 */
export function getDeviceInfo(): {
  isMobile: boolean
  isTablet: boolean
  isDesktop: boolean
  userAgent: string
  platform: string
} {
  const userAgent = navigator.userAgent
  const isMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(userAgent)
  const isTablet = /iPad|Android(?!.*Mobile)/i.test(userAgent)
  const isDesktop = !isMobile && !isTablet

  return {
    isMobile,
    isTablet,
    isDesktop,
    userAgent,
    platform: navigator.platform
  }
}