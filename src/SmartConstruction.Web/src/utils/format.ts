// =============================================
// 格式化工具函数
// =============================================

/**
 * 格式化日期
 */
export function formatDate(
  date: Date | string | number,
  format: string = 'YYYY-MM-DD HH:mm:ss'
): string {
  const d = new Date(date)
  
  if (isNaN(d.getTime())) {
    return '-'
  }
  
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const hours = String(d.getHours()).padStart(2, '0')
  const minutes = String(d.getMinutes()).padStart(2, '0')
  const seconds = String(d.getSeconds()).padStart(2, '0')
  
  return format
    .replace('YYYY', String(year))
    .replace('MM', month)
    .replace('DD', day)
    .replace('HH', hours)
    .replace('mm', minutes)
    .replace('ss', seconds)
}

/**
 * 格式化日期时间
 */
export function formatDateTime(
  date: Date | string | number,
  format: string = 'YYYY-MM-DD HH:mm:ss'
): string {
  return formatDate(date, format)
}

/**
 * 格式化时间
 */
export function formatTime(
  date: Date | string | number,
  format: string = 'HH:mm:ss'
): string {
  return formatDate(date, format)
}

/**
 * 格式化相对时间
 */
export function formatRelativeTime(date: Date | string | number): string {
  const now = new Date()
  const target = new Date(date)
  const diff = now.getTime() - target.getTime()
  
  const minute = 60 * 1000
  const hour = 60 * minute
  const day = 24 * hour
  const week = 7 * day
  const month = 30 * day
  const year = 365 * day
  
  if (diff < minute) {
    return '刚刚'
  } else if (diff < hour) {
    return `${Math.floor(diff / minute)}分钟前`
  } else if (diff < day) {
    return `${Math.floor(diff / hour)}小时前`
  } else if (diff < week) {
    return `${Math.floor(diff / day)}天前`
  } else if (diff < month) {
    return `${Math.floor(diff / week)}周前`
  } else if (diff < year) {
    return `${Math.floor(diff / month)}个月前`
  } else {
    return `${Math.floor(diff / year)}年前`
  }
}

/**
 * 格式化文件大小
 */
export function formatFileSize(bytes: number): string {
  if (bytes === 0) return '0 B'
  
  const k = 1024
  const sizes = ['B', 'KB', 'MB', 'GB', 'TB', 'PB']
  const i = Math.floor(Math.log(bytes) / Math.log(k))
  
  return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
}

/**
 * 格式化数字（千分位分隔）
 */
export function formatNumber(
  num: number | string,
  options: {
    decimals?: number
    separator?: string
    prefix?: string
    suffix?: string
  } = {}
): string {
  const {
    decimals = 0,
    separator = ',',
    prefix = '',
    suffix = ''
  } = options
  
  const number = Number(num)
  if (isNaN(number)) return '-'
  
  const fixed = number.toFixed(decimals)
  const parts = fixed.split('.')
  
  // 添加千分位分隔符
  parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, separator)
  
  return prefix + parts.join('.') + suffix
}

/**
 * 格式化货币
 */
export function formatCurrency(
  amount: number | string,
  options: {
    currency?: string
    locale?: string
    minimumFractionDigits?: number
    maximumFractionDigits?: number
  } = {}
): string {
  const {
    currency = 'CNY',
    locale = 'zh-CN',
    minimumFractionDigits = 2,
    maximumFractionDigits = 2
  } = options
  
  const number = Number(amount)
  if (isNaN(number)) return '-'
  
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency,
    minimumFractionDigits,
    maximumFractionDigits
  }).format(number)
}

/**
 * 格式化百分比
 */
export function formatPercentage(
  value: number | string,
  decimals: number = 2
): string {
  const number = Number(value)
  if (isNaN(number)) return '-'
  
  return (number * 100).toFixed(decimals) + '%'
}

/**
 * 格式化手机号（隐藏中间4位）
 */
export function formatMobile(mobile: string): string {
  if (!mobile || mobile.length !== 11) return mobile
  return mobile.replace(/(\d{3})\d{4}(\d{4})/, '$1****$2')
}

/**
 * 格式化身份证号（隐藏中间部分）
 */
export function formatIdCard(idCard: string): string {
  if (!idCard) return idCard
  
  if (idCard.length === 15) {
    return idCard.replace(/(\d{6})\d{6}(\d{3})/, '$1******$2')
  } else if (idCard.length === 18) {
    return idCard.replace(/(\d{6})\d{8}(\d{4})/, '$1********$2')
  }
  
  return idCard
}

/**
 * 格式化银行卡号（每4位一组）
 */
export function formatBankCard(cardNumber: string): string {
  if (!cardNumber) return cardNumber
  return cardNumber.replace(/\s/g, '').replace(/(\d{4})(?=\d)/g, '$1 ')
}

/**
 * 格式化地址（省略中间部分）
 */
export function formatAddress(address: string, maxLength: number = 20): string {
  if (!address || address.length <= maxLength) return address
  
  const start = Math.floor(maxLength / 2) - 1
  const end = address.length - Math.floor(maxLength / 2) + 2
  
  return address.substring(0, start) + '...' + address.substring(end)
}

/**
 * 格式化JSON（美化显示）
 */
export function formatJSON(obj: any, indent: number = 2): string {
  try {
    return JSON.stringify(obj, null, indent)
  } catch {
    return String(obj)
  }
}

/**
 * 格式化枚举值（转换为可读文本）
 */
export function formatEnum(
  value: string | number,
  enumMap: Record<string | number, string>
): string {
  return enumMap[value] || String(value)
}

/**
 * 格式化状态（带颜色标识）
 */
export function formatStatus(
  status: number | string,
  statusMap: Record<string | number, { text: string; color: string }>
): { text: string; color: string } {
  return statusMap[status] || { text: String(status), color: 'default' }
}

/**
 * 格式化数组为字符串
 */
export function formatArray(
  arr: any[],
  options: {
    separator?: string
    maxItems?: number
    formatter?: (item: any) => string
  } = {}
): string {
  const {
    separator = ', ',
    maxItems = 5,
    formatter = (item) => String(item)
  } = options
  
  if (!Array.isArray(arr) || arr.length === 0) return '-'
  
  const formatted = arr.slice(0, maxItems).map(formatter)
  const result = formatted.join(separator)
  
  if (arr.length > maxItems) {
    return result + ` 等${arr.length}项`
  }
  
  return result
}

/**
 * 格式化时长（秒转换为可读格式）
 */
export function formatDuration(seconds: number): string {
  if (seconds < 60) {
    return `${seconds}秒`
  } else if (seconds < 3600) {
    const minutes = Math.floor(seconds / 60)
    const remainingSeconds = seconds % 60
    return remainingSeconds > 0 ? `${minutes}分${remainingSeconds}秒` : `${minutes}分钟`
  } else if (seconds < 86400) {
    const hours = Math.floor(seconds / 3600)
    const remainingMinutes = Math.floor((seconds % 3600) / 60)
    return remainingMinutes > 0 ? `${hours}小时${remainingMinutes}分钟` : `${hours}小时`
  } else {
    const days = Math.floor(seconds / 86400)
    const remainingHours = Math.floor((seconds % 86400) / 3600)
    return remainingHours > 0 ? `${days}天${remainingHours}小时` : `${days}天`
  }
}

/**
 * 格式化URL参数
 */
export function formatUrlParams(params: Record<string, any>): string {
  const searchParams = new URLSearchParams()
  
  Object.entries(params).forEach(([key, value]) => {
    if (value !== null && value !== undefined && value !== '') {
      searchParams.append(key, String(value))
    }
  })
  
  return searchParams.toString()
}

/**
 * 格式化HTML（移除标签）
 */
export function formatPlainText(html: string): string {
  if (!html) return ''
  return html.replace(/<[^>]*>/g, '').trim()
}

/**
 * 格式化文本省略
 */
export function formatEllipsis(
  text: string,
  maxLength: number,
  suffix: string = '...'
): string {
  if (!text || text.length <= maxLength) return text
  return text.substring(0, maxLength - suffix.length) + suffix
}