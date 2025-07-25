// =============================================
// 错误国际化服务
// =============================================
import { useI18n } from 'vue-i18n'
import { ErrorType } from '@/utils/error'
import { useAppStore } from '@/stores/app'

// 错误码映射表
const errorCodeMap: Record<string, { type: string; key: string; params?: Record<string, any> }> = {
  // 网络错误
  'NETWORK_ERROR': { type: 'network', key: 'connectionFailed' },
  'TIMEOUT_ERROR': { type: 'network', key: 'requestTimeout' },
  
  // HTTP状态码
  '400': { type: 'client', key: 'badRequest' },
  '401': { type: 'auth', key: 'sessionExpired' },
  '403': { type: 'permission', key: 'forbidden' },
  '404': { type: 'common', key: 'notFound' },
  '405': { type: 'client', key: 'unsupportedOperation' },
  '408': { type: 'network', key: 'requestTimeout' },
  '429': { type: 'client', key: 'tooManyRequests' },
  '500': { type: 'server', key: 'internalError' },
  '501': { type: 'server', key: 'unsupportedOperation' },
  '502': { type: 'server', key: 'gatewayError' },
  '503': { type: 'server', key: 'serviceUnavailable' },
  '504': { type: 'server', key: 'gatewayError' },
  
  // 业务错误码
  'AUTH_INVALID_TOKEN': { type: 'auth', key: 'invalidToken' },
  'AUTH_EXPIRED_TOKEN': { type: 'auth', key: 'expiredToken' },
  'AUTH_INVALID_CREDENTIALS': { type: 'auth', key: 'invalidCredentials' },
  'AUTH_ACCOUNT_DISABLED': { type: 'auth', key: 'accountDisabled' },
  'AUTH_ACCOUNT_LOCKED': { type: 'auth', key: 'accountLocked' },
  'AUTH_ACCOUNT_EXPIRED': { type: 'auth', key: 'accountExpired' },
  'AUTH_PASSWORD_EXPIRED': { type: 'auth', key: 'passwordExpired' },
  'AUTH_INVALID_CAPTCHA': { type: 'auth', key: 'invalidCaptcha' },
  'AUTH_INVALID_TENANT': { type: 'auth', key: 'invalidTenant' },
  'AUTH_TENANT_DISABLED': { type: 'auth', key: 'tenantDisabled' },
  'AUTH_TENANT_EXPIRED': { type: 'auth', key: 'tenantExpired' },
  'AUTH_TOO_MANY_ATTEMPTS': { type: 'auth', key: 'tooManyAttempts' },
  'PERMISSION_DENIED': { type: 'permission', key: 'denied' },
  'RESOURCE_NOT_FOUND': { type: 'business', key: 'resourceNotFound' },
  'RESOURCE_ALREADY_EXISTS': { type: 'business', key: 'resourceAlreadyExists' },
  'RESOURCE_CONFLICT': { type: 'business', key: 'resourceConflict' },
  'VALIDATION_ERROR': { type: 'common', key: 'validationError' },
  'BUSINESS_ERROR': { type: 'common', key: 'operationFailed' },
  'SYSTEM_ERROR': { type: 'common', key: 'systemError' },
  'DATABASE_ERROR': { type: 'server', key: 'databaseError' },
  'EXTERNAL_SERVICE_ERROR': { type: 'server', key: 'externalServiceError' }
}

// 错误国际化服务类
class ErrorI18nService {
  // 获取错误类型的国际化文本
  public getErrorTypeText(type: string): string {
    try {
      const i18n = useI18n()
      return i18n.t(`error.errorType.${type}`)
    } catch (error) {
      // 如果i18n不可用，返回默认文本
      switch (type) {
        case ErrorType.NETWORK:
          return '网络错误'
        case ErrorType.AUTH:
          return '认证错误'
        case ErrorType.PERMISSION:
          return '权限错误'
        case ErrorType.BUSINESS:
          return '业务错误'
        case ErrorType.SERVER:
          return '服务器错误'
        case ErrorType.CLIENT:
          return '客户端错误'
        default:
          return '未知错误'
      }
    }
  }
  
  // 获取错误消息的国际化文本
  public getErrorMessage(code: string, defaultMessage: string, params?: Record<string, any>): string {
    try {
      const i18n = useI18n()
      
      // 查找错误码映射
      const errorMapping = errorCodeMap[code]
      
      if (errorMapping) {
        // 使用映射的类型和键构建i18n路径
        const path = `error.${errorMapping.type}.${errorMapping.key}`
        
        // 合并参数
        const mergedParams = { ...errorMapping.params, ...params }
        
        // 尝试获取国际化文本
        const message = i18n.t(path, mergedParams)
        
        // 如果返回的是对象或者与路径相同，说明没有找到对应的翻译
        if (typeof message === 'object' || message === path) {
          return defaultMessage
        }
        
        return message as string
      }
      
      // 如果没有找到映射，返回默认消息
      return defaultMessage
    } catch (error) {
      // 如果i18n不可用，返回默认消息
      return defaultMessage
    }
  }
  
  // 获取验证错误消息
  public getValidationMessage(key: string, field: string, params?: Record<string, any>): string {
    try {
      const i18n = useI18n()
      
      // 构建i18n路径
      const path = `error.validation.${key}`
      
      // 合并参数
      const mergedParams = { field, ...params }
      
      // 尝试获取国际化文本
      const message = i18n.t(path, mergedParams)
      
      // 如果返回的是对象或者与路径相同，说明没有找到对应的翻译
      if (typeof message === 'object' || message === path) {
        // 返回默认验证消息
        switch (key) {
          case 'required':
            return `${field}不能为空`
          case 'minLength':
            return `${field}长度不能小于${params?.min || 0}个字符`
          case 'maxLength':
            return `${field}长度不能超过${params?.max || 0}个字符`
          case 'pattern':
            return `${field}格式不正确`
          case 'email':
            return '请输入有效的邮箱地址'
          case 'url':
            return '请输入有效的URL'
          case 'number':
            return '请输入有效的数字'
          case 'integer':
            return '请输入整数'
          case 'min':
            return `${field}不能小于${params?.min || 0}`
          case 'max':
            return `${field}不能大于${params?.max || 0}`
          case 'confirmPassword':
            return '两次输入的密码不一致'
          default:
            return `${field}验证失败`
        }
      }
      
      return message as string
    } catch (error) {
      // 如果i18n不可用，返回默认验证消息
      return `${field}验证失败`
    }
  }
  
  // 获取当前语言
  public getCurrentLanguage(): string {
    try {
      const appStore = useAppStore()
      return appStore.language
    } catch (error) {
      // 如果store不可用，返回默认语言
      return 'zh-CN'
    }
  }
}

// 导出单例实例
export const errorI18nService = new ErrorI18nService()

// 导出默认实例
export default errorI18nService