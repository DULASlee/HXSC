// =============================================
// 全局错误处理服务
// =============================================
import { ElMessage, ElMessageBox, ElNotification } from 'element-plus'
import { ErrorHandler, ErrorType, type ErrorInfo } from '@/utils/error'
import { useUserStore } from '@/stores/user'
import router from '@/router'
import { logService, LogLevel } from '@/services/logService'
import { errorI18nService } from '@/services/errorI18nService'

// 错误通知级别
export enum ErrorLevel {
  INFO = 'info',
  WARNING = 'warning',
  ERROR = 'error',
  FATAL = 'error'
}

// 错误通知类型
export enum NotificationType {
  NONE = 'none',
  MESSAGE = 'message',
  NOTIFICATION = 'notification',
  DIALOG = 'dialog'
}

// 错误处理选项
export interface ErrorHandlingOptions {
  level?: ErrorLevel
  notification?: NotificationType
  duration?: number
  showDetails?: boolean
  redirect?: string | boolean
  callback?: (error: ErrorInfo) => void
}

// 默认错误处理选项
const defaultOptions: ErrorHandlingOptions = {
  level: ErrorLevel.ERROR,
  notification: NotificationType.MESSAGE,
  duration: 3000,
  showDetails: import.meta.env.DEV, // 开发环境显示详情
  redirect: false
}

// 错误服务类
class ErrorService {
  // 是否为开发环境
  private isDev = import.meta.env.DEV
  
  // 错误历史
  private errorHistory: ErrorInfo[] = []
  
  // 最大历史记录数
  private maxHistorySize = 50
  
  // 处理错误
  public handleError(error: any, customOptions?: ErrorHandlingOptions): ErrorInfo {
    // 合并选项
    const options = { ...defaultOptions, ...customOptions }
    
    // 解析错误
    const errorInfo = ErrorHandler.parseError(error)
    
    // 使用国际化服务获取错误消息
    if (errorInfo.code) {
      const i18nMessage = errorI18nService.getErrorMessage(errorInfo.code, errorInfo.message)
      if (i18nMessage) {
        errorInfo.message = i18nMessage
      }
    }
    
    // 添加到历史记录
    this.addToHistory(errorInfo)
    
    // 根据错误类型调整处理选项
    this.adjustOptionsByErrorType(errorInfo, options)
    
    // 显示错误通知
    this.showErrorNotification(errorInfo, options)
    
    // 处理重定向
    this.handleRedirect(errorInfo, options)
    
    // 执行回调
    if (options.callback) {
      options.callback(errorInfo)
    }
    
    // 记录到控制台
    this.logError(errorInfo)
    
    return errorInfo
  }
  
  // 根据错误类型调整处理选项
  private adjustOptionsByErrorType(errorInfo: ErrorInfo, options: ErrorHandlingOptions): void {
    switch (errorInfo.type) {
      case ErrorType.AUTH:
        // 认证错误默认重定向到登录页
        if (options.redirect === undefined) {
          options.redirect = '/login'
        }
        // 认证错误使用对话框
        if (options.notification === undefined) {
          options.notification = NotificationType.DIALOG
        }
        break
        
      case ErrorType.PERMISSION:
        // 权限错误默认重定向到403页面
        if (options.redirect === undefined) {
          options.redirect = '/403'
        }
        break
        
      case ErrorType.NETWORK:
        // 网络错误使用通知
        if (options.notification === undefined) {
          options.notification = NotificationType.NOTIFICATION
        }
        break
        
      case ErrorType.SERVER:
        // 服务器错误使用通知
        if (options.notification === undefined) {
          options.notification = NotificationType.NOTIFICATION
        }
        // 严重服务器错误可能重定向到500页面
        if (errorInfo.code === '500' && options.redirect === undefined) {
          options.redirect = '/500'
        }
        break
    }
  }
  
  // 显示错误通知
  private showErrorNotification(errorInfo: ErrorInfo, options: ErrorHandlingOptions): void {
    const title = this.getErrorTitle(errorInfo)
    const message = errorInfo.message
    const details = this.isDev ? this.getErrorDetails(errorInfo) : undefined
    
    switch (options.notification) {
      case NotificationType.MESSAGE:
        ElMessage({
          type: options.level as any,
          message,
          duration: options.duration,
          showClose: true
        })
        break
        
      case NotificationType.NOTIFICATION:
        ElNotification({
          title,
          message,
          type: options.level as any,
          duration: options.duration,
          showClose: true
        })
        break
        
      case NotificationType.DIALOG:
        if (errorInfo.type === ErrorType.AUTH) {
          ElMessageBox.confirm(
            message,
            title,
            {
              confirmButtonText: '重新登录',
              cancelButtonText: '取消',
              type: 'warning'
            }
          ).then(() => {
            const userStore = useUserStore()
            userStore.resetUser()
            router.push(`/login?redirect=${router.currentRoute.value.fullPath}`)
          })
        } else {
          ElMessageBox.alert(
            message,
            title,
            {
              confirmButtonText: '确定',
              type: options.level as any
            }
          )
        }
        break
    }
  }
  
  // 处理重定向
  private handleRedirect(errorInfo: ErrorInfo, options: ErrorHandlingOptions): void {
    if (!options.redirect) return
    
    // 处理认证错误
    if (errorInfo.type === ErrorType.AUTH && options.redirect === '/login') {
      const userStore = useUserStore()
      userStore.resetUser()
      
      // 延迟重定向，让用户看到错误消息
      setTimeout(() => {
        router.push(`/login?redirect=${router.currentRoute.value.fullPath}`)
      }, 1500)
      return
    }
    
    // 其他重定向
    if (typeof options.redirect === 'string') {
      setTimeout(() => {
        router.push(options.redirect as string)
      }, 1500)
    }
  }
  
  // 获取错误标题
  private getErrorTitle(errorInfo: ErrorInfo): string {
    // 使用国际化服务获取错误类型文本
    return errorI18nService.getErrorTypeText(errorInfo.type)
  }
  
  // 获取错误详情
  private getErrorDetails(errorInfo: ErrorInfo): string {
    return `错误类型: ${errorInfo.type}
错误码: ${errorInfo.code}
${errorInfo.details ? `详情: ${errorInfo.details}` : ''}
${errorInfo.url ? `URL: ${errorInfo.url}` : ''}
${errorInfo.stack ? `堆栈: ${errorInfo.stack}` : ''}`
  }
  
  // 记录错误到日志服务
  private logError(errorInfo: ErrorInfo): void {
    // 控制台输出
    console.error(`[${errorInfo.type}] ${errorInfo.code}: ${errorInfo.message}`, errorInfo)
    
    // 记录到日志服务
    logService.logErrorInfo(errorInfo)
  }
  
  // 添加错误到历史记录
  private addToHistory(error: ErrorInfo): void {
    this.errorHistory.unshift(error)
    
    // 限制历史记录大小
    if (this.errorHistory.length > this.maxHistorySize) {
      this.errorHistory.pop()
    }
  }
  
  // 获取错误历史
  public getErrorHistory(): ErrorInfo[] {
    return this.errorHistory
  }
  
  // 清除错误历史
  public clearErrorHistory(): void {
    this.errorHistory = []
  }
  
  // 获取友好的错误消息
  public getFriendlyMessage(error: any): string {
    return ErrorHandler.getFriendlyMessage(error)
  }
  
  // 获取开发者错误详情
  public getDeveloperDetails(error: any): string {
    return ErrorHandler.getDeveloperDetails(error)
  }
}

// 导出单例实例
export const errorService = new ErrorService()

// 导出默认实例
export default errorService