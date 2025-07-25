// =============================================
// 表单通用组合式函数
// =============================================
import { ref, reactive, nextTick } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'

export interface FormField {
  key: string
  label: string
  type: string
  required?: boolean
  defaultValue?: any
  options?: any[]
  placeholder?: string
  rules?: any[]
  props?: Record<string, any>
}

export function useForm<T = Record<string, any>>() {
  const formRef = ref<FormInstance>()
  const loading = ref(false)
  const formData = reactive<T>({} as T)
  const originalData = ref<T>({} as T)

  // 表单规则
  const rules = ref<FormRules>({})

  // 设置表单数据
  const setFormData = (data: T) => {
    Object.assign(formData, data)
    originalData.value = JSON.parse(JSON.stringify(data))
  }

  // 重置表单
  const resetForm = () => {
    formRef.value?.resetFields()
    Object.assign(formData, originalData.value)
  }

  // 清空表单
  const clearForm = () => {
    formRef.value?.clearValidate()
    Object.keys(formData).forEach(key => {
      delete (formData as any)[key]
    })
  }

  // 验证表单
  const validateForm = async (): Promise<boolean> => {
    if (!formRef.value) return false
    
    try {
      await formRef.value.validate()
      return true
    } catch (error) {
      return false
    }
  }

  // 验证指定字段
  const validateField = async (prop: string): Promise<boolean> => {
    if (!formRef.value) return false
    
    try {
      await formRef.value.validateField(prop)
      return true
    } catch (error) {
      return false
    }
  }

  // 清除验证
  const clearValidate = (props?: string | string[]) => {
    formRef.value?.clearValidate(props)
  }

  // 设置字段值
  const setFieldValue = (key: keyof T, value: any) => {
    ;(formData as any)[key] = value
  }

  // 获取字段值
  const getFieldValue = (key: keyof T) => {
    return (formData as any)[key]
  }

  // 设置表单规则
  const setRules = (formRules: FormRules) => {
    rules.value = formRules
  }

  // 添加字段规则
  const addFieldRule = (field: string, rule: any) => {
    if (!rules.value[field]) {
      rules.value[field] = []
    }
    if (Array.isArray(rules.value[field])) {
      ;(rules.value[field] as any[]).push(rule)
    }
  }

  // 构建表单规则
  const buildRules = (fields: FormField[]): FormRules => {
    const formRules: FormRules = {}
    
    fields.forEach(field => {
      const fieldRules: any[] = []
      
      // 必填规则
      if (field.required) {
        fieldRules.push({
          required: true,
          message: `请输入${field.label}`,
          trigger: ['blur', 'change']
        })
      }
      
      // 自定义规则
      if (field.rules && field.rules.length > 0) {
        fieldRules.push(...field.rules)
      }
      
      if (fieldRules.length > 0) {
        formRules[field.key] = fieldRules
      }
    })
    
    return formRules
  }

  // 初始化表单
  const initForm = (fields: FormField[], data?: T) => {
    // 设置默认值
    const defaultData: any = {}
    fields.forEach(field => {
      if (field.defaultValue !== undefined) {
        defaultData[field.key] = field.defaultValue
      }
    })
    
    // 合并数据
    const finalData = { ...defaultData, ...data }
    setFormData(finalData as T)
    
    // 构建规则
    const formRules = buildRules(fields)
    setRules(formRules)
  }

  // 检查表单是否有变化
  const hasChanges = (): boolean => {
    return JSON.stringify(formData) !== JSON.stringify(originalData.value)
  }

  // 获取变化的字段
  const getChangedFields = (): Partial<T> => {
    const changes: any = {}
    const original = originalData.value as any
    const current = formData as any
    
    Object.keys(current).forEach(key => {
      if (current[key] !== original[key]) {
        changes[key] = current[key]
      }
    })
    
    return changes
  }

  // 设置加载状态
  const setLoading = (state: boolean) => {
    loading.value = state
  }

  // 滚动到第一个错误字段
  const scrollToError = async () => {
    await nextTick()
    const errorField = document.querySelector('.el-form-item.is-error')
    if (errorField) {
      errorField.scrollIntoView({ behavior: 'smooth', block: 'center' })
    }
  }

  return {
    // 状态
    formRef,
    loading,
    formData,
    originalData,
    rules,

    // 方法
    setFormData,
    resetForm,
    clearForm,
    validateForm,
    validateField,
    clearValidate,
    setFieldValue,
    getFieldValue,
    setRules,
    addFieldRule,
    buildRules,
    initForm,
    hasChanges,
    getChangedFields,
    setLoading,
    scrollToError
  }
}