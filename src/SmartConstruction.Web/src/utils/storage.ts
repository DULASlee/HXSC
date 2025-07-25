// =============================================
// 存储工具函数
// =============================================

/**
 * localStorage 封装
 */
export const localStorage = {
  /**
   * 设置数据
   */
  set(key: string, value: any): void {
    try {
      const serializedValue = JSON.stringify(value)
      window.localStorage.setItem(key, serializedValue)
    } catch (error) {
      console.error('localStorage set error:', error)
    }
  },

  /**
   * 获取数据
   */
  get<T = any>(key: string, defaultValue?: T): T | null {
    try {
      const item = window.localStorage.getItem(key)
      if (item === null) {
        return defaultValue !== undefined ? defaultValue : null
      }
      return JSON.parse(item)
    } catch (error) {
      console.error('localStorage get error:', error)
      return defaultValue !== undefined ? defaultValue : null
    }
  },

  /**
   * 移除数据
   */
  remove(key: string): void {
    try {
      window.localStorage.removeItem(key)
    } catch (error) {
      console.error('localStorage remove error:', error)
    }
  },

  /**
   * 清空所有数据
   */
  clear(): void {
    try {
      window.localStorage.clear()
    } catch (error) {
      console.error('localStorage clear error:', error)
    }
  },

  /**
   * 获取所有键
   */
  keys(): string[] {
    try {
      return Object.keys(window.localStorage)
    } catch (error) {
      console.error('localStorage keys error:', error)
      return []
    }
  },

  /**
   * 检查键是否存在
   */
  has(key: string): boolean {
    return window.localStorage.getItem(key) !== null
  }
}

/**
 * sessionStorage 封装
 */
export const sessionStorage = {
  /**
   * 设置数据
   */
  set(key: string, value: any): void {
    try {
      const serializedValue = JSON.stringify(value)
      window.sessionStorage.setItem(key, serializedValue)
    } catch (error) {
      console.error('sessionStorage set error:', error)
    }
  },

  /**
   * 获取数据
   */
  get<T = any>(key: string, defaultValue?: T): T | null {
    try {
      const item = window.sessionStorage.getItem(key)
      if (item === null) {
        return defaultValue !== undefined ? defaultValue : null
      }
      return JSON.parse(item)
    } catch (error) {
      console.error('sessionStorage get error:', error)
      return defaultValue !== undefined ? defaultValue : null
    }
  },

  /**
   * 移除数据
   */
  remove(key: string): void {
    try {
      window.sessionStorage.removeItem(key)
    } catch (error) {
      console.error('sessionStorage remove error:', error)
    }
  },

  /**
   * 清空所有数据
   */
  clear(): void {
    try {
      window.sessionStorage.clear()
    } catch (error) {
      console.error('sessionStorage clear error:', error)
    }
  },

  /**
   * 获取所有键
   */
  keys(): string[] {
    try {
      return Object.keys(window.sessionStorage)
    } catch (error) {
      console.error('sessionStorage keys error:', error)
      return []
    }
  },

  /**
   * 检查键是否存在
   */
  has(key: string): boolean {
    return window.sessionStorage.getItem(key) !== null
  }
}

/**
 * 带过期时间的存储
 */
export class ExpiredStorage {
  private storage: Storage

  constructor(storage: Storage = window.localStorage) {
    this.storage = storage
  }

  /**
   * 设置数据（带过期时间）
   */
  set(key: string, value: any, expireTime?: number): void {
    try {
      const data = {
        value,
        expireTime: expireTime ? Date.now() + expireTime : null
      }
      this.storage.setItem(key, JSON.stringify(data))
    } catch (error) {
      console.error('ExpiredStorage set error:', error)
    }
  }

  /**
   * 获取数据
   */
  get<T = any>(key: string, defaultValue?: T): T | null {
    try {
      const item = this.storage.getItem(key)
      if (!item) {
        return defaultValue !== undefined ? defaultValue : null
      }

      const data = JSON.parse(item)
      
      // 检查是否过期
      if (data.expireTime && Date.now() > data.expireTime) {
        this.remove(key)
        return defaultValue !== undefined ? defaultValue : null
      }

      return data.value
    } catch (error) {
      console.error('ExpiredStorage get error:', error)
      return defaultValue !== undefined ? defaultValue : null
    }
  }

  /**
   * 移除数据
   */
  remove(key: string): void {
    try {
      this.storage.removeItem(key)
    } catch (error) {
      console.error('ExpiredStorage remove error:', error)
    }
  }

  /**
   * 清理过期数据
   */
  clearExpired(): void {
    try {
      const keys = Object.keys(this.storage)
      keys.forEach(key => {
        const item = this.storage.getItem(key)
        if (item) {
          try {
            const data = JSON.parse(item)
            if (data.expireTime && Date.now() > data.expireTime) {
              this.storage.removeItem(key)
            }
          } catch {
            // 忽略解析错误的项
          }
        }
      })
    } catch (error) {
      console.error('ExpiredStorage clearExpired error:', error)
    }
  }

  /**
   * 检查键是否存在且未过期
   */
  has(key: string): boolean {
    return this.get(key) !== null
  }
}

/**
 * 创建带过期时间的 localStorage 实例
 */
export const expiredLocalStorage = new ExpiredStorage(window.localStorage)

/**
 * 创建带过期时间的 sessionStorage 实例
 */
export const expiredSessionStorage = new ExpiredStorage(window.sessionStorage)

/**
 * Cookie 操作工具
 */
export const cookie = {
  /**
   * 设置 Cookie
   */
  set(
    name: string,
    value: string,
    options: {
      expires?: number | Date
      path?: string
      domain?: string
      secure?: boolean
      sameSite?: 'Strict' | 'Lax' | 'None'
    } = {}
  ): void {
    let cookieString = `${encodeURIComponent(name)}=${encodeURIComponent(value)}`

    if (options.expires) {
      if (typeof options.expires === 'number') {
        const date = new Date()
        date.setTime(date.getTime() + options.expires * 24 * 60 * 60 * 1000)
        cookieString += `; expires=${date.toUTCString()}`
      } else {
        cookieString += `; expires=${options.expires.toUTCString()}`
      }
    }

    if (options.path) {
      cookieString += `; path=${options.path}`
    }

    if (options.domain) {
      cookieString += `; domain=${options.domain}`
    }

    if (options.secure) {
      cookieString += '; secure'
    }

    if (options.sameSite) {
      cookieString += `; samesite=${options.sameSite}`
    }

    document.cookie = cookieString
  },

  /**
   * 获取 Cookie
   */
  get(name: string): string | null {
    const nameEQ = encodeURIComponent(name) + '='
    const cookies = document.cookie.split(';')

    for (let cookie of cookies) {
      cookie = cookie.trim()
      if (cookie.indexOf(nameEQ) === 0) {
        return decodeURIComponent(cookie.substring(nameEQ.length))
      }
    }

    return null
  },

  /**
   * 移除 Cookie
   */
  remove(name: string, path?: string, domain?: string): void {
    this.set(name, '', {
      expires: new Date(0),
      path,
      domain
    })
  },

  /**
   * 检查 Cookie 是否存在
   */
  has(name: string): boolean {
    return this.get(name) !== null
  },

  /**
   * 获取所有 Cookie
   */
  getAll(): Record<string, string> {
    const cookies: Record<string, string> = {}
    const cookieArray = document.cookie.split(';')

    cookieArray.forEach(cookie => {
      const [name, value] = cookie.trim().split('=')
      if (name && value) {
        cookies[decodeURIComponent(name)] = decodeURIComponent(value)
      }
    })

    return cookies
  }
}

/**
 * 存储配额检查
 */
export function getStorageQuota(): Promise<{ quota: number; usage: number }> {
  if ('storage' in navigator && 'estimate' in navigator.storage) {
    return navigator.storage.estimate().then(estimate => ({
      quota: estimate.quota || 0,
      usage: estimate.usage || 0
    }))
  }
  
  return Promise.resolve({ quota: 0, usage: 0 })
}

/**
 * 检查存储是否可用
 */
export function isStorageAvailable(type: 'localStorage' | 'sessionStorage'): boolean {
  try {
    const storage = window[type]
    const testKey = '__storage_test__'
    storage.setItem(testKey, 'test')
    storage.removeItem(testKey)
    return true
  } catch {
    return false
  }
}