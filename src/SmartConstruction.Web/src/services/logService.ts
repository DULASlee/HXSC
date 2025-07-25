// =============================================
// 日志服务 - 用于收集和上报错误日志
// =============================================
import { ErrorType, type ErrorInfo } from '@/utils/error'

// 日志级别
export enum LogLevel {
  DEBUG = 'debug',
  INFO = 'info',
  WARN = 'warn',
  ERROR = 'error',
  FATAL = 'fatal'
}

// 日志条目接口
export interface LogEntry {
  id: string
  timestamp: number
  level: LogLevel
  message: string
  details?: string
  tags?: string[]
  data?: any
  source?: string
  user?: {
    id?: string
    username?: string
  }
  session?: string
  url?: string
  userAgent?: string
}

// 日志服务类
class LogService {
  // 是否为开发环境
  private isDev = import.meta.env.DEV
  
  // 日志缓存
  private logCache: LogEntry[] = []
  
  // 最大缓存大小
  private maxCacheSize = 100
  
  // 是否启用自动上报
  private autoReport = !this.isDev
  
  // 上报URL
  private reportUrl = import.meta.env.VITE_LOG_REPORT_URL || '/api/logs'
  
  // 上报间隔（毫秒）
  private reportInterval = 60000 // 1分钟
  
  // 上报定时器
  private reportTimer: number | null = null
  
  // 初始化
  constructor() {
    if (this.autoReport) {
      this.startAutoReport()
    }
    
    // 页面卸载前尝试上报
    window.addEventListener('beforeunload', () => {
      if (this.logCache.length > 0) {
        this.reportLogs(true)
      }
    })
  }
  
  // 开始自动上报
  private startAutoReport() {
    this.reportTimer = window.setInterval(() => {
      if (this.logCache.length > 0) {
        this.reportLogs()
      }
    }, this.reportInterval)
  }
  
  // 停止自动上报
  private stopAutoReport() {
    if (this.reportTimer) {
      window.clearInterval(this.reportTimer)
      this.reportTimer = null
    }
  }
  
  // 生成日志ID
  private generateId(): string {
    return Date.now().toString(36) + Math.random().toString(36).substr(2, 5)
  }
  
  // 创建日志条目
  private createLogEntry(
    level: LogLevel,
    message: string,
    details?: string,
    data?: any,
    tags?: string[]
  ): LogEntry {
    // 获取当前用户信息
    const userStore = window.$pinia?.state.value?.user
    const user = userStore ? {
      id: userStore.userInfo?.id,
      username: userStore.userInfo?.username
    } : undefined
    
    // 创建日志条目
    return {
      id: this.generateId(),
      timestamp: Date.now(),
      level,
      message,
      details,
      tags,
      data,
      source: 'browser',
      user,
      session: localStorage.getItem('sessionId') || undefined,
      url: window.location.href,
      userAgent: navigator.userAgent
    }
  }
  
  // 添加日志
  private addLog(entry: LogEntry) {
    // 添加到缓存
    this.logCache.unshift(entry)
    
    // 限制缓存大小
    if (this.logCache.length > this.maxCacheSize) {
      this.logCache = this.logCache.slice(0, this.maxCacheSize)
    }
    
    // 开发环境下打印到控制台
    if (this.isDev) {
      this.printToConsole(entry)
    }
    
    // 如果缓存达到一定大小，立即上报
    if (this.autoReport && this.logCache.length >= 20) {
      this.reportLogs()
    }
  }
  
  // 打印到控制台
  private printToConsole(entry: LogEntry) {
    const { level, message, details, data } = entry
    
    switch (level) {
      case LogLevel.DEBUG:
        console.debug(`[DEBUG] ${message}`, details, data)
        break
      case LogLevel.INFO:
        console.info(`[INFO] ${message}`, details, data)
        break
      case LogLevel.WARN:
        console.warn(`[WARN] ${message}`, details, data)
        break
      case LogLevel.ERROR:
      case LogLevel.FATAL:
        console.error(`[${level.toUpperCase()}] ${message}`, details, data)
        break
    }
  }
  
  // 上报日志
  private async reportLogs(sync = false) {
    if (this.logCache.length === 0) return
    
    try {
      const logs = [...this.logCache]
      
      // 使用Beacon API进行同步上报（页面卸载时）
      if (sync && navigator.sendBeacon) {
        const blob = new Blob([JSON.stringify({ logs })], { type: 'application/json' })
        const success = navigator.sendBeacon(this.reportUrl, blob)
        
        if (success) {
          this.logCache = []
        }
        return
      }
      
      // 使用fetch进行异步上报
      const response = await fetch(this.reportUrl, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ logs }),
        // 不发送凭证，避免CORS问题
        credentials: 'omit'
      })
      
      if (response.ok) {
        // 上报成功，清空缓存
        this.logCache = []
      }
    } catch (error) {
      console.error('Failed to report logs:', error)
    }
  }
  
  // 记录调试日志
  public debug(message: string, details?: string, data?: any, tags?: string[]) {
    const entry = this.createLogEntry(LogLevel.DEBUG, message, details, data, tags)
    this.addLog(entry)
  }
  
  // 记录信息日志
  public info(message: string, details?: string, data?: any, tags?: string[]) {
    const entry = this.createLogEntry(LogLevel.INFO, message, details, data, tags)
    this.addLog(entry)
  }
  
  // 记录警告日志
  public warn(message: string, details?: string, data?: any, tags?: string[]) {
    const entry = this.createLogEntry(LogLevel.WARN, message, details, data, tags)
    this.addLog(entry)
  }
  
  // 记录错误日志
  public error(message: string, details?: string, data?: any, tags?: string[]) {
    const entry = this.createLogEntry(LogLevel.ERROR, message, details, data, tags)
    this.addLog(entry)
  }
  
  // 记录致命错误日志
  public fatal(message: string, details?: string, data?: any, tags?: string[]) {
    const entry = this.createLogEntry(LogLevel.FATAL, message, details, data, tags)
    this.addLog(entry)
  }
  
  // 记录错误对象
  public logError(error: any, level: LogLevel = LogLevel.ERROR) {
    let message = 'Unknown error'
    let details = ''
    let data = null
    
    if (error instanceof Error) {
      message = error.message
      details = error.stack || ''
    } else if (typeof error === 'string') {
      message = error
    } else if (typeof error === 'object' && error !== null) {
      message = error.message || 'Object error'
      details = error.details || error.stack || JSON.stringify(error)
      data = error
    }
    
    const entry = this.createLogEntry(level, message, details, data, ['error'])
    this.addLog(entry)
  }
  
  // 记录错误信息
  public logErrorInfo(errorInfo: ErrorInfo) {
    // 映射错误类型到日志级别
    let level = LogLevel.ERROR
    
    switch (errorInfo.type) {
      case ErrorType.NETWORK:
        level = LogLevel.WARN
        break
      case ErrorType.AUTH:
      case ErrorType.PERMISSION:
        level = LogLevel.INFO
        break
      case ErrorType.SERVER:
        level = LogLevel.ERROR
        break
      case ErrorType.BUSINESS:
        level = LogLevel.WARN
        break
      case ErrorType.CLIENT:
        level = LogLevel.ERROR
        break
      case ErrorType.UNKNOWN:
        level = LogLevel.ERROR
        break
    }
    
    const entry = this.createLogEntry(
      level,
      `[${errorInfo.type}] ${errorInfo.code}: ${errorInfo.message}`,
      errorInfo.details,
      errorInfo,
      ['error', errorInfo.type.toLowerCase()]
    )
    
    this.addLog(entry)
  }
  
  // 获取日志缓存
  public getLogs(): LogEntry[] {
    return [...this.logCache]
  }
  
  // 清空日志缓存
  public clearLogs() {
    this.logCache = []
  }
  
  // 手动上报日志
  public async reportLogsNow() {
    return this.reportLogs()
  }
  
  // 设置自动上报
  public setAutoReport(enabled: boolean) {
    this.autoReport = enabled
    
    if (enabled) {
      this.startAutoReport()
    } else {
      this.stopAutoReport()
    }
  }
  
  // 设置上报URL
  public setReportUrl(url: string) {
    this.reportUrl = url
  }
  
  // 设置上报间隔
  public setReportInterval(interval: number) {
    this.reportInterval = interval
    
    if (this.autoReport) {
      this.stopAutoReport()
      this.startAutoReport()
    }
  }
}

// 导出单例实例
export const logService = new LogService()

// 导出默认实例
export default logService