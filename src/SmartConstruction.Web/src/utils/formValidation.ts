// =============================================
// 表单验证错误处理工具
// =============================================
import { ElMessage } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'
import { errorI18nService } from '@/services/errorI18nService'

// 表单验证错误
export interface FormValidationError {
  field: string
  message: string
  label?: string
}

// 表单验证结果
export interface FormValidationResult {
  valid: boolean
  errors: FormValidationError[]
  firstError?: FormValidationError
  errorMessage: string
}

// 表单验证选项
export interface FormValidationOptions {
  showMessage?: boolean
  scrollToError?: boolean
}

// 默认选项
const defaultOptions: FormValidationOptions = {
  showMessage: true,
  scrollToError: true
}

/**
 * 验证表单
 * @param form 表单实例
 * @param options 验证选项
 * @returns 验证结果
 */
export async function validateForm(
  form: FormInstance,
  options: FormValidationOptions = {}
): Promise<FormValidationResult> {
  const mergedOptions = { ...defaultOptions, ...options }
  
  try {
    // 验证表单
    await form.validate()
    
    // 验证通过
    return {
      valid: true,
      errors: [],
      errorMessage: ''
    }
  } catch (error: any) {
    // 验证失败
    const errors: FormValidationError[] = []
    
    // 处理验证错误
    if (error.fields) {
      Object.keys(error.fields).forEach(field => {
        const fieldErrors = error.fields[field]
        const formItem = form.formItemInstances[field]
        const label = formItem?.labelRef?.value || field
        
        fieldErrors.forEach((message: string) => {
          errors.push({
            field,
            message,
            label
          })
        })
      })
    }
    
    // 获取第一个错误
    const firstError = errors[0]
    
    // 构建错误消息
    const errorMessage = firstError ? firstError.message : '表单验证失败'
    
    // 显示错误消息
    if (mergedOptions.showMessage && firstError) {
      ElMessage.error(errorMessage)
    }
    
    // 滚动到错误位置
    if (mergedOptions.scrollToError && firstError) {
      const errorField = document.querySelector(`[data-field="${firstError.field}"]`)
      if (errorField) {
        errorField.scrollIntoView({ behavior: 'smooth', block: 'center' })
      }
    }
    
    return {
      valid: false,
      errors,
      firstError,
      errorMessage
    }
  }
}

/**
 * 创建表单验证规则
 * @param rules 验证规则
 * @returns 国际化的验证规则
 */
export function createFormRules(rules: FormRules): FormRules {
  const i18nRules: FormRules = {}
  
  // 处理每个字段的规则
  Object.keys(rules).forEach(field => {
    const fieldRules = rules[field]
    
    if (Array.isArray(fieldRules)) {
      i18nRules[field] = fieldRules.map(rule => {
        // 如果规则有自定义消息，直接使用
        if (rule.message) {
          return rule
        }
        
        // 根据规则类型生成国际化消息
        let key = 'required'
        let params: Record<string, any> = {}
        
        if (rule.required) {
          key = 'required'
        } else if (rule.min !== undefined) {
          key = 'min'
          params = { min: rule.min }
        } else if (rule.max !== undefined) {
          key = 'max'
          params = { max: rule.max }
        } else if (rule.len !== undefined) {
          key = rule.len === 0 ? 'required' : 'length'
          params = { len: rule.len }
        } else if (rule.pattern) {
          key = 'pattern'
        } else if (rule.type) {
          key = rule.type as string
        }
        
        // 获取字段标签
        const label = rule.label || field
        
        // 获取国际化消息
        const message = errorI18nService.getValidationMessage(key, label, params)
        
        return {
          ...rule,
          message
        }
      })
    } else {
      i18nRules[field] = fieldRules
    }
  })
  
  return i18nRules
}

/**
 * 处理表单验证错误
 * @param error 验证错误
 * @returns 格式化的错误信息
 */
export function handleFormValidationError(error: any): string {
  if (!error) return ''
  
  // 如果是表单验证错误
  if (error.fields) {
    const errors: string[] = []
    
    Object.keys(error.fields).forEach(field => {
      const fieldErrors = error.fields[field]
      fieldErrors.forEach((message: string) => {
        errors.push(message)
      })
    })
    
    return errors.join('；')
  }
  
  // 其他错误
  return error.message || '表单验证失败'
}

/**
 * 重置表单验证
 * @param form 表单实例
 * @param fields 要重置的字段，不指定则重置所有字段
 */
export function resetFormValidation(form: FormInstance, fields?: string[]): void {
  if (fields && fields.length > 0) {
    fields.forEach(field => {
      form.clearValidate(field)
    })
  } else {
    form.clearValidate()
  }
}

/**
 * 验证表单字段
 * @param form 表单实例
 * @param field 字段名
 * @returns 验证结果
 */
export async function validateFormField(
  form: FormInstance,
  field: string
): Promise<boolean> {
  try {
    await form.validateField(field)
    return true
  } catch (error) {
    return false
  }
}

/**
 * 创建必填规则
 * @param message 错误消息
 * @returns 必填规则
 */
export function requiredRule(message?: string): any {
  return {
    required: true,
    message: message || '该字段不能为空',
    trigger: ['blur', 'change']
  }
}

/**
 * 创建长度规则
 * @param min 最小长度
 * @param max 最大长度
 * @param message 错误消息
 * @returns 长度规则
 */
export function lengthRule(min: number, max: number, message?: string): any {
  return {
    min,
    max,
    message: message || `长度应在${min}到${max}个字符之间`,
    trigger: 'blur'
  }
}

/**
 * 创建正则表达式规则
 * @param pattern 正则表达式
 * @param message 错误消息
 * @returns 正则表达式规则
 */
export function patternRule(pattern: RegExp, message: string): any {
  return {
    pattern,
    message,
    trigger: 'blur'
  }
}

/**
 * 创建邮箱规则
 * @param message 错误消息
 * @returns 邮箱规则
 */
export function emailRule(message?: string): any {
  return {
    type: 'email',
    message: message || '请输入有效的邮箱地址',
    trigger: 'blur'
  }
}

/**
 * 创建URL规则
 * @param message 错误消息
 * @returns URL规则
 */
export function urlRule(message?: string): any {
  return {
    type: 'url',
    message: message || '请输入有效的URL',
    trigger: 'blur'
  }
}

/**
 * 创建数字规则
 * @param message 错误消息
 * @returns 数字规则
 */
export function numberRule(message?: string): any {
  return {
    type: 'number',
    message: message || '请输入有效的数字',
    trigger: 'blur'
  }
}

/**
 * 创建整数规则
 * @param message 错误消息
 * @returns 整数规则
 */
export function integerRule(message?: string): any {
  return patternRule(/^-?\d+$/, message || '请输入整数')
}

/**
 * 创建手机号规则
 * @param message 错误消息
 * @returns 手机号规则
 */
export function mobileRule(message?: string): any {
  return patternRule(/^1[3-9]\d{9}$/, message || '请输入有效的手机号')
}

/**
 * 创建身份证号规则
 * @param message 错误消息
 * @returns 身份证号规则
 */
export function idCardRule(message?: string): any {
  return patternRule(/(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/, message || '请输入有效的身份证号')
}

/**
 * 创建密码规则
 * @param message 错误消息
 * @returns 密码规则
 */
export function passwordRule(message?: string): any {
  return patternRule(
    /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/,
    message || '密码必须包含大小写字母和数字，且长度不少于8位'
  )
}

/**
 * 创建确认密码规则
 * @param passwordField 密码字段名
 * @param message 错误消息
 * @returns 确认密码规则
 */
export function confirmPasswordRule(passwordField: string, message?: string): any {
  return {
    validator: (rule: any, value: string, callback: Function) => {
      const form = rule.form
      const password = form[passwordField]
      
      if (value !== password) {
        callback(new Error(message || '两次输入的密码不一致'))
      } else {
        callback()
      }
    },
    trigger: ['blur', 'change']
  }
}

/**
 * 创建自定义验证规则
 * @param validator 验证函数
 * @param trigger 触发方式
 * @returns 自定义验证规则
 */
export function customRule(
  validator: (rule: any, value: any, callback: Function) => void,
  trigger: string | string[] = 'blur'
): any {
  return {
    validator,
    trigger
  }
}

// 导出默认对象
export default {
  validateForm,
  createFormRules,
  handleFormValidationError,
  resetFormValidation,
  validateFormField,
  requiredRule,
  lengthRule,
  patternRule,
  emailRule,
  urlRule,
  numberRule,
  integerRule,
  mobileRule,
  idCardRule,
  passwordRule,
  confirmPasswordRule,
  customRule
}