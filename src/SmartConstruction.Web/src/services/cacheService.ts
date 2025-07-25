// =============================================
// 缓存管理服务
// =============================================

// 缓存项接口
interface CacheItem<T = any> {
  data: T
  timestamp: number
  expiry?: number
  version?: string
}

// 缓存配置接口
interface CacheConfig {
  maxSize?: number
  defaultTTL?: number
  version?: string
  storage?: 'memory' | 'localStorage' | 'sessionStorage'
}

// LRU缓存实现
class LRUCache<T = any> {
  private cache = new Map<string, CacheItem<T>>()
  private maxSize: number
  private defaultTTL: number
  private version: string
  private storage: 'memory' | 'localStorage' | 'sessionStorage'
  
  constructor(config: CacheConfig = {}) {
    this.maxSize = config.maxSize || 100
    this.defaultTTL = config.defaultTTL || 5 * 60 * 1000 // 5分钟
    this.version = config.version || '1.0.0'
    this.storage = config.storage || 'memory'
    
    // 从持久化存储恢复缓存
    if (this.storage !== 'memory') {
      this.loadFromStorage()
    }
  }
  
  // 设置缓存
  set(key: string, data: T, ttl?: number): void {
    const now = Date.now()
    const expiry = ttl ? now + ttl : now + this.defaultTTL
    
    const item: CacheItem<T> = {
      data,
      timestamp: now,
      expiry,
      version: this.version
    }
    
    // 如果缓存已满，删除最旧的项
    if (this.cache.size >= this.maxSize && !this.cache.has(key)) {
      const firstKey = this.cache.keys().next().value
      this.cache.delete(firstKey)
    }
    
    // 如果key已存在，先删除再添加（更新LRU顺序）
    if (this.cache.has(key)) {
      this.cache.delete(key)
    }
    
    this.cache.set(key, item)
    
    // 持久化到存储
    if (this.storage !== 'memory') {
      this.saveToStorage()
    }
  }
  
  // 获取缓存
  get(key: string): T | null {
    const item = this.cache.get(key)
    
    if (!item) {
      return null
    }
    
    // 检查版本
    if (item.version !== this.version) {
      this.cache.delete(key)
      return null
    }
    
    // 检查过期时间
    if (item.expiry && Date.now() > item.expiry) {
      this.cache.delete(key)
      return null
    }
    
    // 更新LRU顺序
    this.cache.delete(key)
    this.cache.set(key, item)
    
    return item.data
  }
  
  // 检查缓存是否存在且有效
  has(key: string): boolean {
    return this.get(key) !== null
  }
  
  // 删除缓存
  delete(key: string): boolean {
    const result = this.cache.delete(key)
    
    if (this.storage !== 'memory') {
      this.saveToStorage()
    }
    
    return result
  }
  
  // 清空缓存
  clear(): void {
    this.cache.clear()
    
    if (this.storage !== 'memory') {
      this.clearStorage()
    }
  }
  
  // 获取缓存大小
  size(): number {
    return this.cache.size
  }
  
  // 获取所有键
  keys(): string[] {
    return Array.from(this.cache.keys())
  }
  
  // 清理过期缓存
  cleanup(): number {
    const now = Date.now()
    let cleanedCount = 0
    
    for (const [key, item] of this.cache.entries()) {
      if (item.expiry && now > item.expiry) {
        this.cache.delete(key)
        cleanedCount++
      }
    }
    
    if (cleanedCount > 0 && this.storage !== 'memory') {
      this.saveToStorage()
    }
    
    return cleanedCount
  }
  
  // 获取缓存统计信息
  getStats() {
    const now = Date.now()
    let expiredCount = 0
    let totalSize = 0
    
    for (const [key, item] of this.cache.entries()) {
      if (item.expiry && now > item.expiry) {
        expiredCount++
      }
      
      // 估算数据大小
      totalSize += JSON.stringify(item).length
    }
    
    return {
      size: this.cache.size,
      maxSize: this.maxSize,
      expiredCount,
      totalSize,
      hitRate: this.getHitRate()
    }
  }
  
  // 计算命中率（简化实现）
  private hitCount = 0
  private missCount = 0
  
  private getHitRate(): number {
    const total = this.hitCount + this.missCount
    return total > 0 ? this.hitCount / total : 0
  }
  
  // 从存储加载缓存
  private loadFromStorage(): void {
    try {
      const storage = this.getStorage()
      const data = storage.getItem(`cache_${this.version}`)
      
      if (data) {
        const parsed = JSON.parse(data)
        this.cache = new Map(parsed)
      }
    } catch (error) {
      console.warn('Failed to load cache from storage:', error)
    }
  }
  
  // 保存缓存到存储
  private saveToStorage(): void {
    try {
      const storage = this.getStorage()
      const data = JSON.stringify(Array.from(this.cache.entries()))
      storage.setItem(`cache_${this.version}`, data)
    } catch (error) {
      console.warn('Failed to save cache to storage:', error)
    }
  }
  
  // 清空存储
  private clearStorage(): void {
    try {
      const storage = this.getStorage()
      storage.removeItem(`cache_${this.version}`)
    } catch (error) {
      console.warn('Failed to clear cache storage:', error)
    }
  }
  
  // 获取存储对象
  private getStorage(): Storage {
    if (this.storage === 'localStorage') {
      return localStorage
    } else if (this.storage === 'sessionStorage') {
      return sessionStorage
    }
    throw new Error('Invalid storage type')
  }
}

// 缓存管理服务
class CacheService {
  private caches = new Map<string, LRUCache>()
  private cleanupInterval: number | null = null
  
  constructor() {
    // 定期清理过期缓存
    this.startCleanup()
  }
  
  // 创建或获取缓存实例
  getCache<T = any>(name: string, config?: CacheConfig): LRUCache<T> {
    if (!this.caches.has(name)) {
      this.caches.set(name, new LRUCache<T>(config))
    }
    return this.caches.get(name)!
  }
  
  // 删除缓存实例
  deleteCache(name: string): boolean {
    const cache = this.caches.get(name)
    if (cache) {
      cache.clear()
      return this.caches.delete(name)
    }
    return false
  }
  
  // 清空所有缓存
  clearAll(): void {
    for (const cache of this.caches.values()) {
      cache.clear()
    }
    this.caches.clear()
  }
  
  // 获取所有缓存统计信息
  getAllStats() {
    const stats: Record<string, any> = {}
    
    for (const [name, cache] of this.caches.entries()) {
      stats[name] = cache.getStats()
    }
    
    return stats
  }
  
  // 开始定期清理
  private startCleanup(): void {
    this.cleanupInterval = window.setInterval(() => {
      for (const cache of this.caches.values()) {
        cache.cleanup()
      }
    }, 60000) // 每分钟清理一次
  }
  
  // 停止定期清理
  stopCleanup(): void {
    if (this.cleanupInterval) {
      clearInterval(this.cleanupInterval)
      this.cleanupInterval = null
    }
  }
  
  // 销毁服务
  dispose(): void {
    this.stopCleanup()
    this.clearAll()
  }
}

// 导出单例实例
export const cacheService = new CacheService()

// 预定义的缓存实例
export const apiCache = cacheService.getCache('api', {
  maxSize: 200,
  defaultTTL: 5 * 60 * 1000, // 5分钟
  storage: 'memory'
})

export const userCache = cacheService.getCache('user', {
  maxSize: 50,
  defaultTTL: 30 * 60 * 1000, // 30分钟
  storage: 'localStorage'
})

export const routeCache = cacheService.getCache('route', {
  maxSize: 100,
  defaultTTL: 60 * 60 * 1000, // 1小时
  storage: 'sessionStorage'
})

// 导出默认实例
export default cacheService

// Vue组合式API
export function useCache<T = any>(name: string, config?: CacheConfig) {
  const cache = cacheService.getCache<T>(name, config)
  
  const set = (key: string, data: T, ttl?: number) => cache.set(key, data, ttl)
  const get = (key: string) => cache.get(key)
  const has = (key: string) => cache.has(key)
  const del = (key: string) => cache.delete(key)
  const clear = () => cache.clear()
  const size = () => cache.size()
  const keys = () => cache.keys()
  const stats = () => cache.getStats()
  
  return {
    set,
    get,
    has,
    delete: del,
    clear,
    size,
    keys,
    stats
  }
}