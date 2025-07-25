<template>
  <el-form
    ref="formRef"
    :model="formData"
    :rules="formRules"
    :label-width="labelWidth"
    :label-position="labelPosition"
    :size="size"
    :disabled="disabled"
  >
    <el-row :gutter="20">
      <el-col
        v-for="field in visibleFields"
        :key="field.metaKey"
        :span="field.span || 12"
      >
        <el-form-item
          :label="field.displayName"
          :prop="field.metaKey"
          :required="field.required"
        >
          <component
            :is="getComponent(field)"
            v-model="formData[field.metaKey]"
            v-bind="getComponentProps(field)"
            @change="handleFieldChange(field, $event)"
          />
          <div v-if="field.description" class="field-description">
            {{ field.description }}
          </div>
        </el-form-item>
      </el-col>
    </el-row>
  </el-form>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, PropType } from 'vue'
import { ElInput, ElInputNumber, ElSelect, ElDatePicker, ElSwitch, ElRadioGroup, ElCheckboxGroup } from 'element-plus'
import { MetadataDefinition } from '@/types/global'
import { useFormValidation } from '@/composables/useFormValidation'
import DynamicSelect from './components/DynamicSelect.vue'
import JsonEditor from './components/JsonEditor.vue'
import FileUpload from './components/FileUpload.vue'
import RichTextEditor from './components/RichTextEditor.vue'
import OrgTreeSelect from '@/components/OrgTreeSelect.vue'
import UserSelect from '@/components/UserSelect.vue'

const props = defineProps({
  // 元数据定义列表
  fields: {
    type: Array as PropType<MetadataDefinition[]>,
    required: true
  },
  // 表单数据
  modelValue: {
    type: Object as PropType<Record<string, any>>,
    default: () => ({})
  },
  // 标签宽度
  labelWidth: {
    type: String,
    default: '120px'
  },
  // 标签位置
  labelPosition: {
    type: String as PropType<'left' | 'right' | 'top'>,
    default: 'right'
  },
  // 表单尺寸
  size: {
    type: String as PropType<'large' | 'default' | 'small'>,
    default: 'default'
  },
  // 是否禁用
  disabled: {
    type: Boolean,
    default: false
  },
  // 是否只读
  readonly: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['update:modelValue', 'change', 'field-change'])

const formRef = ref()
const formData = reactive<Record<string, any>>({})

// 可见字段（根据条件过滤）
const visibleFields = computed(() => {
  return props.fields.filter(field => {
    if (!field.isActive) return false
    // TODO: 根据条件表达式判断是否显示
    return true
  })
})

// 表单验证规则
const formRules = computed(() => {
  const rules: Record<string, any[]> = {}
  
  visibleFields.value.forEach(field => {
    const fieldRules: any[] = []
    
    // 必填验证
    if (field.required) {
      fieldRules.push({
        required: true,
        message: `请输入${field.displayName}`,
        trigger: field.valueType === 'String' ? 'blur' : 'change'
      })
    }
    
    // 自定义验证规则
    if (field.validationRule) {
      try {
        const customRule = JSON.parse(field.validationRule)
        fieldRules.push(...useFormValidation(customRule))
      } catch (error) {
        console.error('Invalid validation rule:', field.validationRule)
      }
    }
    
    if (fieldRules.length > 0) {
      rules[field.metaKey] = fieldRules
    }
  })
  
  return rules
})

// 组件映射
const componentMap: Record<string, any> = {
  // 基础组件
  'input': ElInput,
  'textarea': ElInput,
  'number': ElInputNumber,
  'select': ElSelect,
  'date': ElDatePicker,
  'datetime': ElDatePicker,
  'switch': ElSwitch,
  'radio': ElRadioGroup,
  'checkbox': ElCheckboxGroup,
  
  // 自定义组件
  'dynamic-select': DynamicSelect,
  'json-editor': JsonEditor,
  'file-upload': FileUpload,
  'rich-text': RichTextEditor,
  'org-tree-select': OrgTreeSelect,
  'user-select': UserSelect
}

// 获取组件
function getComponent(field: MetadataDefinition) {
  const component = field.uiComponent || getDefaultComponent(field.valueType)
  return componentMap[component] || ElInput
}

// 获取默认组件
function getDefaultComponent(valueType: string): string {
  const defaultMap: Record<string, string> = {
    'String': 'input',
    'Number': 'number',
    'Boolean': 'switch',
    'Date': 'date',
    'JSON': 'json-editor'
  }
  return defaultMap[valueType] || 'input'
}

// 获取组件属性
function getComponentProps(field: MetadataDefinition) {
  const componentProps: Record<string, any> = {
    placeholder: `请输入${field.displayName}`,
    disabled: props.disabled || props.readonly,
    clearable: true
  }
  
  // 根据组件类型设置特定属性
  switch (field.uiComponent) {
    case 'textarea':
      componentProps.type = 'textarea'
      componentProps.rows = 4
      break
    case 'select':
    case 'dynamic-select':
      componentProps.multiple = field.uiConfig?.multiple || false
      componentProps.options = field.options || []
      break
    case 'date':
      componentProps.type = 'date'
      componentProps.format = 'YYYY-MM-DD'
      componentProps.valueFormat = 'YYYY-MM-DD'
      break
    case 'datetime':
      componentProps.type = 'datetime'
      componentProps.format = 'YYYY-MM-DD HH:mm:ss'
      componentProps.valueFormat = 'YYYY-MM-DD HH:mm:ss'
      break
    case 'number':
      componentProps.min = field.uiConfig?.min
      componentProps.max = field.uiConfig?.max
      componentProps.step = field.uiConfig?.step || 1
      componentProps.precision = field.uiConfig?.precision
      break
  }
  
  // 合并自定义配置
  if (field.uiConfig) {
    Object.assign(componentProps, field.uiConfig)
  }
  
  return componentProps
}

// 字段值变化
function handleFieldChange(field: MetadataDefinition, value: any) {
  emit('field-change', {
    field: field.metaKey,
    value,
    metadata: field
  })
}

// 初始化表单数据
function initFormData() {
  // 从modelValue初始化
  Object.assign(formData, props.modelValue)
  
  // 设置默认值
  visibleFields.value.forEach(field => {
    if (formData[field.metaKey] === undefined && field.defaultValue !== undefined) {
      formData[field.metaKey] = field.defaultValue
    }
  })
}

// 监听数据变化
watch(() => formData, 
  (newData) => {
    emit('update:modelValue', { ...newData })
    emit('change', newData)
  },
  { deep: true }
)

// 监听外部数据变化
watch(() => props.modelValue,
  (newValue) => {
    Object.assign(formData, newValue)
  },
  { deep: true }
)

// 表单验证
async function validate() {
  return await formRef.value?.validate()
}

// 重置表单
function resetFields() {
  formRef.value?.resetFields()
}

// 清除验证
function clearValidate(props?: string | string[]) {
  formRef.value?.clearValidate(props)
}

// 初始化
initFormData()

// 暴露方法
defineExpose({
  validate,
  resetFields,
  clearValidate
})
</script>

<style lang="scss" scoped>
.field-description {
  margin-top: 4px;
  font-size: 12px;
  color: var(--el-text-color-secondary);
  line-height: 1.5;
}
</style>