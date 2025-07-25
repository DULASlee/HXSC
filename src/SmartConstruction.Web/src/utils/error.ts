// =============================================
// 统一错误处理工具
// =============================================
import { ElMessage, ElMessageBox } from 'element-plus'
import router from '@/router'
import { useUserStore } from '@/stores/user'

// 错误类型定义
export enum ErrorType {
  // 网络错误
  NETWORK = 'NETWORK',
  // 认证错误
  AUTH = 'AUTH',
  // 权限错误
  PERMISSION = 'PERMISSION',
  // 业务错误
  BUSINESS = 'BUSINESS',
  // 服务器错误
  SERVER = 'SERVER',
  // 客户端错误
  CLIENT = 'CLIENT',
  // 未知错误
  UNKNOWN = 'UNKNOWN'
}

// 错误信息接口
export interface ErrorInfo {
  type: ErrorType
  code: string | number
  message: string
  details?: string
  stack?: string
  data?: any
  timestamp: number
  url?: string
  handled: boolean
}

// 错误码映射表
const errorCodeMap: Record<string, { type: ErrorType; message: string }> = {
  // 网络错误
  'NETWORK_ERROR': { type: ErrorType.NETWORK, message: '网络连接失败，请检查您的网络连接' },
  'TIMEOUT_ERROR': { type: ErrorType.NETWORK, message: '请求超时，请稍后重试' },
  
  // HTTP状态码
  '400': { type: ErrorType.CLIENT, message: '请求参数错误' },
  '401': { type: ErrorType.AUTH, message: '登录状态已过期，请重新登录' },
  '403': { type: ErrorType.PERMISSION, message: '您没有权限访问该资源' },
  '404': { type: ErrorType.CLIENT, message: '请求的资源不存在' },
  '405': { type: ErrorType.CLIENT, message: '请求方法不允许' },
  '408': { type: ErrorType.NETWORK, message: '请求超时，请稍后重试' },
  '429': { type: ErrorType.CLIENT, message: '请求过于频繁，请稍后再试' },
  '500': { type: ErrorType.SERVER, message: '服务器内部错误' },
  '501': { type: ErrorType.SERVER, message: '服务未实现' },
  '502': { type: ErrorType.SERVER, message: '网关错误' },
  '503': { type: ErrorType.SERVER, message: '服务不可用' },
  '504': { type: ErrorType.SERVER, message: '网关超时' },
  
  // 业务错误码
  'AUTH_INVALID_TOKEN': { type: ErrorType.AUTH, message: '无效的访问令牌' },
  'AUTH_EXPIRED_TOKEN': { type: ErrorType.AUTH, message: '访问令牌已过期' },
  'AUTH_INVALID_CREDENTIALS': { type: ErrorType.AUTH, message: '用户名或密码错误' },
  'AUTH_ACCOUNT_DISABLED': { type: ErrorType.AUTH, message: '账号已被禁用' },
  'AUTH_ACCOUNT_LOCKED': { type: ErrorType.AUTH, message: '账号已被锁定' },
  'AUTH_ACCOUNT_EXPIRED': { type: ErrorType.AUTH, message: '账号已过期' },
  'AUTH_PASSWORD_EXPIRED': { type: ErrorType.AUTH, message: '密码已过期，请修改密码' },
  'AUTH_INVALID_CAPTCHA': { type: ErrorType.AUTH, message: '验证码错误' },
  'AUTH_INVALID_TENANT': { type: ErrorType.AUTH, message: '无效的租户' },
  'AUTH_TENANT_DISABLED': { type: ErrorType.AUTH, message: '租户已被禁用' },
  'AUTH_TENANT_EXPIRED': { type: ErrorType.AUTH, message: '租户已过期' },
  'AUTH_TENANT_QUOTA_EXCEEDED': { type: ErrorType.AUTH, message: '租户配额已用尽' },
  'PERMISSION_DENIED': { type: ErrorType.PERMISSION, message: '权限不足，无法执行此操作' },
  'RESOURCE_NOT_FOUND': { type: ErrorType.BUSINESS, message: '请求的资源不存在' },
  'RESOURCE_ALREADY_EXISTS': { type: ErrorType.BUSINESS, message: '资源已存在' },
  'RESOURCE_CONFLICT': { type: ErrorType.BUSINESS, message: '资源冲突' },
  'VALIDATION_ERROR': { type: ErrorType.BUSINESS, message: '数据验证失败' },
  'BUSINESS_ERROR': { type: ErrorType.BUSINESS, message: '业务处理失败' },
  'SYSTEM_ERROR': { type: ErrorType.SERVER, message: '系统内部错误' },
  'DATABASE_ERROR': { type: ErrorType.SERVER, message: '数据库操作失败' },
  'EXTERNAL_SERVICE_ERROR': { type: ErrorType.SERVER, message: '外部服务调用失败' }
}

// 错误处理类
export class ErrorHandler {
  // 错误历史记录
  private static errorHistory: ErrorInfo[] = []
  
  // 最大历史记录数
  private static maxHistorySize = 50
  
  // 获取错误历史
  static getErrorHistory(): ErrorInfo[] {
    return this.errorHistory
  }
  
  // 清除错误历史
  static clearErrorHistory(): void {
    this.errorHistory = []
  }
  
  // 添加错误到历史记录
  private static addToHistory(error: ErrorInfo): void {
    this.errorHistory.unshift(error)
    
    // 限制历史记录大小
    if (this.errorHistory.length > this.maxHistorySize) {
      this.errorHistory.pop()
    }
  }
  
  // 解析错误
  static parseError(error: any): ErrorInfo {
    const now = Date.now()
    
    // 默认错误信息
    const errorInfo: ErrorInfo = {
      type: ErrorType.UNKNOWN,
      code: 'UNKNOWN_ERROR',
      message: '发生未知错误',
      timestamp: now,
      handled: false
    }
    
    try {
      // 处理Axios错误
      if (error.isAxiosError) {
        // 网络错误
        if (error.message.includes('Network Error')) {
          errorInfo.type = ErrorType.NETWORK
          errorInfo.code = 'NETWORK_ERROR'
          errorInfo.message = '网络连接失败，请检查您的网络连接'
        }
        // 超时错误
        else if (error.message.includes('timeout')) {
          errorInfo.type = ErrorType.NETWORK
          errorInfo.code = 'TIMEOUT_ERROR'
          errorInfo.message = '请求超时，请稍后重试'
        }
        // HTTP错误
        else if (error.response) {
          const { status, data } = error.response
          errorInfo.code = status.toString()
          
          // 使用错误码映射
          const mappedError = errorCodeMap[errorInfo.code]
          if (mappedError) {
            errorInfo.type = mappedError.type
            errorInfo.message = mappedError.message
          }
          
          // 如果响应中包含详细错误信息，优先使用
          if (data) {
            if (data.message) {
              errorInfo.message = data.message
            }
            if (data.code) {
              errorInfo.code = data.code
            }
            if (data.details) {
              errorInfo.details = data.details
            }
            
            errorInfo.data = data
          }
        }
        
        // 保存请求URL
        if (error.config && error.config.url) {
          errorInfo.url = error.config.url
        }
      }
      // 处理业务错误
      else if (error.code) {
        errorInfo.code = error.code
        
        // 使用错误码映射
        const mappedError = errorCodeMap[error.code]
        if (mappedError) {
          errorInfo.type = mappedError.type
          errorInfo.message = mappedError.message
        }
        
        if (error.message) {
          errorInfo.message = error.message
        }
        if (error.details) {
          errorInfo.details = error.details
        }
        if (error.data) {
          errorInfo.data = error.data
        }
      }
      // 处理普通Error对象
      else if (error instanceof Error) {
        errorInfo.message = error.message
        errorInfo.stack = error.stack
      }
      // 处理字符串错误
      else if (typeof error === 'string') {
        errorInfo.message = error
      }
    } catch (e) {
      console.error('Error parsing error:', e)
    }
    
    // 添加到历史记录
    this.addToHistory(errorInfo)
    
    return errorInfo
  }
  
  // 处理错误
  static handleError(error: any, options: {
    showNotification?: boolean
    showDialog?: boolean
    redirectToLogin?: boolean
    redirectTo?: string
    callback?: (errorInfo: ErrorInfo) => void
  } = {}): ErrorInfo {
    const errorInfo = this.parseError(error)
    
    // 标记为已处理
    errorInfo.handled = true
    
    // 根据错误类型处理
    switch (errorInfo.type) {
      case ErrorType.AUTH:
        // 认证错误，重定向到登录页
        if (options.redirectToLogin !== false) {
          const userStore = useUserStore()
          userStore.resetUser()
          
          // 使用消息框提示用户
          if (options.showDialog !== false) {
            ElMessageBox.confirm(
              errorInfo.message || '登录状态已过期，请重新登录',
              '认证失败',
              {
                confirmButtonText: '重新登录',
                cancelButtonText: '取消',
                type: 'warning'
              }
            ).then(() => {
              router.push(`/login?redirect=${router.currentRoute.value.fullPath}`)
            })
          } else if (options.showNotification !== false) {
            ElMessage.error(errorInfo.message)
            setTimeout(() => {
              router.push(`/login?redirect=${router.currentRoute.value.fullPath}`)
            }, 1500)
          } else {
            router.push(`/login?redirect=${router.currentRoute.value.fullPath}`)
          }
        }
        break
        
      case ErrorType.PERMISSION:
        // 权限错误，显示错误消息
        if (options.showNotification !== false) {
          ElMessage.error(errorInfo.message)
        }
        
        // 可选重定向到403页面
        if (options.redirectTo === '403') {
          router.push('/403')
        }
        break
        
      case ErrorType.NETWORK:
      case ErrorType.SERVER:
      case ErrorType.BUSINESS:
      case ErrorType.CLIENT:
      case ErrorType.UNKNOWN:
      default:
        // 显示错误消息
        if (options.showNotification !== false) {
          ElMessage.error(errorInfo.message)
        }
        
        // 可选重定向
        if (options.redirectTo) {
          router.push(options.redirectTo)
        }
        break
    }
    
    // 执行回调
    if (options.callback) {
      options.callback(errorInfo)
    }
    
    // 记录到控制台
    console.error(`[${errorInfo.type}] ${errorInfo.code}: ${errorInfo.message}`, errorInfo)
    
    return errorInfo
  }
  
  // 获取友好的错误消息
  static getFriendlyMessage(error: any): string {
    const errorInfo = this.parseError(error)
    return errorInfo.message
  }
  
  // 获取开发者错误详情
  static getDeveloperDetails(error: any): string {
    const errorInfo = this.parseError(error)
    return `错误类型: ${errorInfo.type}
错误码: ${errorInfo.code}
错误消息: ${errorInfo.message}
${errorInfo.details ? `详情: ${errorInfo.details}` : ''}
${errorInfo.url ? `URL: ${errorInfo.url}` : ''}
${errorInfo.stack ? `堆栈: ${errorInfo.stack}` : ''}`
  }
}

// 导出默认实例
export default ErrorHandler