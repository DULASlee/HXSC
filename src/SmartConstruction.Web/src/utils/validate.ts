// =============================================
// 验证工具函数
// =============================================

/**
 * 邮箱验证
 */
export function isEmail(email: string): boolean {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  return emailRegex.test(email)
}

/**
 * 手机号验证（中国大陆）
 */
export function isMobile(mobile: string): boolean {
  const mobileRegex = /^1[3-9]\d{9}$/
  return mobileRegex.test(mobile)
}

/**
 * 身份证号验证（中国大陆）
 */
export function isIdCard(idCard: string): boolean {
  const idCardRegex = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/
  return idCardRegex.test(idCard)
}

/**
 * URL验证
 */
export function isUrl(url: string): boolean {
  try {
    new URL(url)
    return true
  } catch {
    return false
  }
}

/**
 * IP地址验证
 */
export function isIP(ip: string): boolean {
  const ipRegex = /^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/
  return ipRegex.test(ip)
}

/**
 * 密码强度验证
 */
export function validatePassword(password: string): {
  valid: boolean
  strength: 'weak' | 'medium' | 'strong'
  message: string
} {
  if (password.length < 6) {
    return { valid: false, strength: 'weak', message: '密码长度至少6位' }
  }
  
  let score = 0
  const checks = [
    /[a-z]/.test(password), // 小写字母
    /[A-Z]/.test(password), // 大写字母
    /\d/.test(password),    // 数字
    /[!@#$%^&*(),.?":{}|<>]/.test(password), // 特殊字符
    password.length >= 8    // 长度大于等于8
  ]
  
  score = checks.filter(Boolean).length
  
  if (score < 3) {
    return { valid: false, strength: 'weak', message: '密码强度太弱，建议包含大小写字母、数字和特殊字符' }
  } else if (score < 4) {
    return { valid: true, strength: 'medium', message: '密码强度中等' }
  } else {
    return { valid: true, strength: 'strong', message: '密码强度很强' }
  }
}

/**
 * 用户名验证
 */
export function isUsername(username: string): boolean {
  // 4-20位，字母开头，可包含字母、数字、下划线
  const usernameRegex = /^[a-zA-Z][a-zA-Z0-9_]{3,19}$/
  return usernameRegex.test(username)
}

/**
 * 中文姓名验证
 */
export function isChineseName(name: string): boolean {
  const chineseNameRegex = /^[\u4e00-\u9fa5]{2,8}$/
  return chineseNameRegex.test(name)
}

/**
 * 银行卡号验证
 */
export function isBankCard(cardNumber: string): boolean {
  // 简单的银行卡号验证（13-19位数字）
  const bankCardRegex = /^\d{13,19}$/
  return bankCardRegex.test(cardNumber)
}

/**
 * 车牌号验证
 */
export function isLicensePlate(plate: string): boolean {
  // 中国车牌号验证
  const plateRegex = /^[京津沪渝冀豫云辽黑湘皖鲁新苏浙赣鄂桂甘晋蒙陕吉闽贵粤青藏川宁琼使领A-Z]{1}[A-Z]{1}[A-Z0-9]{4}[A-Z0-9挂学警港澳]{1}$/
  return plateRegex.test(plate)
}

/**
 * 数字验证
 */
export function isNumber(value: any): boolean {
  return !isNaN(Number(value)) && isFinite(Number(value))
}

/**
 * 整数验证
 */
export function isInteger(value: any): boolean {
  return Number.isInteger(Number(value))
}

/**
 * 正整数验证
 */
export function isPositiveInteger(value: any): boolean {
  const num = Number(value)
  return Number.isInteger(num) && num > 0
}

/**
 * 非负整数验证
 */
export function isNonNegativeInteger(value: any): boolean {
  const num = Number(value)
  return Number.isInteger(num) && num >= 0
}

/**
 * 小数验证
 */
export function isDecimal(value: any, decimalPlaces?: number): boolean {
  const num = Number(value)
  if (!isNumber(num)) return false
  
  if (decimalPlaces !== undefined) {
    const str = num.toString()
    const decimalIndex = str.indexOf('.')
    if (decimalIndex === -1) return decimalPlaces === 0
    return str.length - decimalIndex - 1 <= decimalPlaces
  }
  
  return true
}

/**
 * 范围验证
 */
export function isInRange(value: any, min: number, max: number): boolean {
  const num = Number(value)
  return isNumber(num) && num >= min && num <= max
}

/**
 * 长度验证
 */
export function isLengthInRange(str: string, min: number, max: number): boolean {
  return str.length >= min && str.length <= max
}

/**
 * 正则表达式验证
 */
export function matchesPattern(value: string, pattern: RegExp): boolean {
  return pattern.test(value)
}

/**
 * 必填验证
 */
export function isRequired(value: any): boolean {
  if (value === null || value === undefined) return false
  if (typeof value === 'string') return value.trim() !== ''
  if (Array.isArray(value)) return value.length > 0
  if (typeof value === 'object') return Object.keys(value).length > 0
  return true
}

/**
 * 日期验证
 */
export function isValidDate(date: any): boolean {
  if (date instanceof Date) {
    return !isNaN(date.getTime())
  }
  
  if (typeof date === 'string') {
    const parsedDate = new Date(date)
    return !isNaN(parsedDate.getTime())
  }
  
  return false
}

/**
 * 日期范围验证
 */
export function isDateInRange(date: Date | string, startDate: Date | string, endDate: Date | string): boolean {
  const targetDate = new Date(date)
  const start = new Date(startDate)
  const end = new Date(endDate)
  
  return targetDate >= start && targetDate <= end
}

/**
 * JSON字符串验证
 */
export function isValidJSON(str: string): boolean {
  try {
    JSON.parse(str)
    return true
  } catch {
    return false
  }
}

/**
 * 文件类型验证
 */
export function isValidFileType(fileName: string, allowedTypes: string[]): boolean {
  const extension = fileName.split('.').pop()?.toLowerCase()
  return extension ? allowedTypes.includes(extension) : false
}

/**
 * 文件大小验证
 */
export function isValidFileSize(fileSize: number, maxSizeInMB: number): boolean {
  const maxSizeInBytes = maxSizeInMB * 1024 * 1024
  return fileSize <= maxSizeInBytes
}

/**
 * 组合验证器
 */
export function createValidator(rules: Array<(value: any) => boolean | string>) {
  return (value: any): { valid: boolean; message?: string } => {
    for (const rule of rules) {
      const result = rule(value)
      if (result !== true) {
        return {
          valid: false,
          message: typeof result === 'string' ? result : '验证失败'
        }
      }
    }
    return { valid: true }
  }
}

/**
 * 常用验证规则
 */
export const validationRules = {
  required: (message = '此字段为必填项') => (value: any) => isRequired(value) || message,
  email: (message = '请输入有效的邮箱地址') => (value: string) => !value || isEmail(value) || message,
  mobile: (message = '请输入有效的手机号') => (value: string) => !value || isMobile(value) || message,
  username: (message = '用户名格式不正确') => (value: string) => !value || isUsername(value) || message,
  minLength: (min: number, message?: string) => (value: string) => 
    !value || value.length >= min || message || `最少输入${min}个字符`,
  maxLength: (max: number, message?: string) => (value: string) => 
    !value || value.length <= max || message || `最多输入${max}个字符`,
  pattern: (regex: RegExp, message = '格式不正确') => (value: string) => 
    !value || regex.test(value) || message
}

/**
 * @param {string} path
 * @returns {Boolean}
 */
export function isExternal(path: string): boolean {
  return /^(https?:|mailto:|tel:)/.test(path)
}