// =============================================
// 服务层统一导出
// =============================================

export { PermissionService } from './permission'
export { ResourceService } from './resource'
export { TenantService } from './tenant'
export { MetadataService } from './metadata'

// 服务层工具类
export class ServiceUtils {
  // 处理API错误
  static handleApiError(error: any): string {
    if (error.response) {
      return error.response.data?.message || '请求失败'
    } else if (error.message) {
      return error.message
    } else {
      return '未知错误'
    }
  }

  // 格式化日期
  static formatDate(date: string | Date, format: 'date' | 'datetime' | 'time' = 'datetime'): string {
    const d = typeof date === 'string' ? new Date(date) : date
    
    if (isNaN(d.getTime())) {
      return '-'
    }

    switch (format) {
      case 'date':
        return d.toLocaleDateString('zh-CN')
      case 'time':
        return d.toLocaleTimeString('zh-CN')
      case 'datetime':
      default:
        return d.toLocaleString('zh-CN')
    }
  }

  // 格式化文件大小
  static formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 B'
    
    const k = 1024
    const sizes = ['B', 'KB', 'MB', 'GB', 'TB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  // 防抖函数
  static debounce<T extends (...args: any[]) => any>(
    func: T,
    wait: number
  ): (...args: Parameters<T>) => void {
    let timeout: NodeJS.Timeout
    
    return (...args: Parameters<T>) => {
      clearTimeout(timeout)
      timeout = setTimeout(() => func.apply(this, args), wait)
    }
  }

  // 节流函数
  static throttle<T extends (...args: any[]) => any>(
    func: T,
    limit: number
  ): (...args: Parameters<T>) => void {
    let inThrottle: boolean
    
    return (...args: Parameters<T>) => {
      if (!inThrottle) {
        func.apply(this, args)
        inThrottle = true
        setTimeout(() => inThrottle = false, limit)
      }
    }
  }

  // 深拷贝
  static deepClone<T>(obj: T): T {
    if (obj === null || typeof obj !== 'object') {
      return obj
    }
    
    if (obj instanceof Date) {
      return new Date(obj.getTime()) as unknown as T
    }
    
    if (obj instanceof Array) {
      return obj.map(item => this.deepClone(item)) as unknown as T
    }
    
    if (typeof obj === 'object') {
      const clonedObj = {} as T
      for (const key in obj) {
        if (obj.hasOwnProperty(key)) {
          clonedObj[key] = this.deepClone(obj[key])
        }
      }
      return clonedObj
    }
    
    return obj
  }

  // 生成UUID
  static generateUUID(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0
      const v = c === 'x' ? r : (r & 0x3 | 0x8)
      return v.toString(16)
    })
  }

  // 树形数据扁平化
  static flattenTree<T extends { children?: T[] }>(tree: T[], childrenKey: keyof T = 'children' as keyof T): T[] {
    const result: T[] = []
    
    function traverse(nodes: T[]) {
      nodes.forEach(node => {
        result.push(node)
        if (node[childrenKey] && Array.isArray(node[childrenKey])) {
          traverse(node[childrenKey] as T[])
        }
      })
    }
    
    traverse(tree)
    return result
  }

  // 扁平数据转树形
  static arrayToTree<T extends { id: string; parentId?: string; children?: T[] }>(
    array: T[],
    options: {
      idKey?: keyof T
      parentIdKey?: keyof T
      childrenKey?: keyof T
    } = {}
  ): T[] {
    const { idKey = 'id', parentIdKey = 'parentId', childrenKey = 'children' } = options
    const tree: T[] = []
    const map = new Map<string, T>()

    // 创建映射
    array.forEach(item => {
      map.set(item[idKey] as string, { ...item, [childrenKey]: [] })
    })

    // 构建树
    array.forEach(item => {
      const node = map.get(item[idKey] as string)!
      const parentId = item[parentIdKey] as string
      
      if (parentId && map.has(parentId)) {
        const parent = map.get(parentId)!
        ;(parent[childrenKey] as T[]).push(node)
      } else {
        tree.push(node)
      }
    })

    return tree
  }
}