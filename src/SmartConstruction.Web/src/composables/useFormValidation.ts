import { FormItemRule } from 'element-plus'

export interface ValidationRule {
  type?: 'required' | 'pattern' | 'min' | 'max' | 'minLength' | 'maxLength' | 'email' | 'phone' | 'custom'
  value?: any
  message?: string
  trigger?: 'blur' | 'change'
}

export function useFormValidation(rules: ValidationRule[]): FormItemRule[] {
  const formRules: FormItemRule[] = []

  rules.forEach(rule => {
    const formRule: FormItemRule = {
      trigger: rule.trigger || 'blur'
    }

    switch (rule.type) {
      case 'required':
        formRule.required = true
        formRule.message = rule.message || '此字段为必填项'
        break

      case 'pattern':
        formRule.pattern = new RegExp(rule.value)
        formRule.message = rule.message || '格式不正确'
        break

      case 'min':
        formRule.min = rule.value
        formRule.message = rule.message || `最小值为 ${rule.value}`
        break

      case 'max':
        formRule.max = rule.value
        formRule.message = rule.message || `最大值为 ${rule.value}`
        break

      case 'minLength':
        formRule.min = rule.value
        formRule.message = rule.message || `最少输入 ${rule.value} 个字符`
        break

      case 'maxLength':
        formRule.max = rule.value
        formRule.message = rule.message || `最多输入 ${rule.value} 个字符`
        break

      case 'email':
        formRule.type = 'email'
        formRule.message = rule.message || '请输入正确的邮箱地址'
        break

      case 'phone':
        formRule.pattern = /^1[3-9]\d{9}$/
        formRule.message = rule.message || '请输入正确的手机号码'
        break

      case 'custom':
        if (rule.value && typeof rule.value === 'function') {
          formRule.validator = rule.value
          formRule.message = rule.message
        }
        break
    }

    formRules.push(formRule)
  })

  return formRules
}

// 常用验证规则
export const commonValidationRules = {
  // 手机号验证
  phone: {
    pattern: /^1[3-9]\d{9}$/,
    message: '请输入正确的手机号码'
  },

  // 邮箱验证
  email: {
    type: 'email' as const,
    message: '请输入正确的邮箱地址'
  },

  // 身份证验证
  idCard: {
    pattern: /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/,
    message: '请输入正确的身份证号码'
  },

  // 密码强度验证
  strongPassword: {
    pattern: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/,
    message: '密码必须包含大小写字母、数字和特殊字符，且长度不少于8位'
  },

  // URL验证
  url: {
    pattern: /^https?:\/\/.+/,
    message: '请输入正确的URL地址'
  },

  // 中文姓名验证
  chineseName: {
    pattern: /^[\u4e00-\u9fa5]{2,10}$/,
    message: '请输入正确的中文姓名'
  },

  // 数字验证
  number: {
    pattern: /^\d+$/,
    message: '请输入数字'
  },

  // 正整数验证
  positiveInteger: {
    pattern: /^[1-9]\d*$/,
    message: '请输入正整数'
  },

  // 金额验证（保留两位小数）
  money: {
    pattern: /^\d+(\.\d{1,2})?$/,
    message: '请输入正确的金额格式'
  }
}