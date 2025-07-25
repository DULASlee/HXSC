<template>
  <div class="metadata-form">
    <el-row :gutter="20">
      <el-col
        v-for="field in fields"
        :key="field.key"
        :span="getFieldSpan(field)"
      >
        <el-form-item
          :label="field.label"
          :prop="`metadata.${field.key}`"
          :required="field.required"
        >
          <!-- 文本输入 -->
          <el-input
            v-if="field.type === 'input'"
            v-model="formData[field.key]"
            :placeholder="field.placeholder"
            :disabled="disabled"
            clearable
          />
          
          <!-- 数字输入 -->
          <el-input-number
            v-else-if="field.type === 'input-number'"
            v-model="formData[field.key]"
            :placeholder="field.placeholder"
            :disabled="disabled"
            style="width: 100%"
          />
          
          <!-- 文本域 -->
          <el-input
            v-else-if="field.type === 'textarea'"
            v-model="formData[field.key]"
            type="textarea"
            :placeholder="field.placeholder"
            :disabled="disabled"
            :rows="4"
          />
          
          <!-- 选择器 -->
          <el-select
            v-else-if="field.type === 'select'"
            v-model="formData[field.key]"
            :placeholder="field.placeholder"
            :disabled="disabled"
            style="width: 100%"
            clearable
          >
            <el-option
              v-for="option in field.options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>
          
          <!-- 多选器 -->
          <el-select
            v-else-if="field.type === 'multi-select'"
            v-model="formData[field.key]"
            :placeholder="field.placeholder"
            :disabled="disabled"
            multiple
            style="width: 100%"
            clearable
          >
            <el-option
              v-for="option in field.options"
              :key="option.value"
              :label="option.label"
              :value="option.value"
            />
          </el-select>
          
          <!-- 开关 -->
          <el-switch
            v-else-if="field.type === 'switch'"
            v-model="formData[field.key]"
            :disabled="disabled"
          />
          
          <!-- 日期选择器 -->
          <el-date-picker
            v-else-if="field.type === 'date-picker'"
            v-model="formData[field.key]"
            type="date"
            :placeholder="field.placeholder"
            :disabled="disabled"
            style="width: 100%"
          />
          
          <!-- 日期时间选择器 -->
          <el-date-picker
            v-else-if="field.type === 'datetime-picker'"
            v-model="formData[field.key]"
            type="datetime"
            :placeholder="field.placeholder"
            :disabled="disabled"
            style="width: 100%"
          />
          
          <!-- 单选组 -->
          <el-radio-group
            v-else-if="field.type === 'radio'"
            v-model="formData[field.key]"
            :disabled="disabled"
          >
            <el-radio
              v-for="option in field.options"
              :key="option.value"
              :label="option.value"
            >
              {{ option.label }}
            </el-radio>
          </el-radio-group>
          
          <!-- 复选框组 -->
          <el-checkbox-group
            v-else-if="field.type === 'checkbox'"
            v-model="formData[field.key]"
            :disabled="disabled"
          >
            <el-checkbox
              v-for="option in field.options"
              :key="option.value"
              :label="option.value"
            >
              {{ option.label }}
            </el-checkbox>
          </el-checkbox-group>
          
          <!-- 默认文本输入 -->
          <el-input
            v-else
            v-model="formData[field.key]"
            :placeholder="field.placeholder"
            :disabled="disabled"
            clearable
          />
        </el-form-item>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { reactive, computed, watch } from 'vue'

interface MetadataField {
  key: string
  label: string
  type: string
  required?: boolean
  placeholder?: string
  options?: Array<{ value: any; label: string }>
  span?: number
}

interface Props {
  modelValue: Record<string, any>
  fields: MetadataField[]
  disabled?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: Record<string, any>): void
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false
})

const emit = defineEmits<Emits>()

// 表单数据
const formData = reactive<Record<string, any>>({})

// 计算属性
const metadataValue = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 获取字段跨度
function getFieldSpan(field: MetadataField): number {
  if (field.span) return field.span
  
  // 根据字段类型自动设置跨度
  switch (field.type) {
    case 'textarea':
      return 24
    case 'checkbox':
    case 'radio':
      return 24
    default:
      return 12
  }
}

// 监听表单数据变化
watch(
  formData,
  (newValue) => {
    metadataValue.value = { ...newValue }
  },
  { deep: true }
)

// 监听外部数据变化
watch(
  () => props.modelValue,
  (newValue) => {
    Object.assign(formData, newValue)
  },
  { immediate: true, deep: true }
)

// 初始化表单数据
function initFormData() {
  props.fields.forEach(field => {
    if (!(field.key in formData)) {
      formData[field.key] = getDefaultValue(field)
    }
  })
}

// 获取字段默认值
function getDefaultValue(field: MetadataField): any {
  switch (field.type) {
    case 'input-number':
      return 0
    case 'switch':
      return false
    case 'multi-select':
    case 'checkbox':
      return []
    case 'date-picker':
    case 'datetime-picker':
      return null
    default:
      return ''
  }
}

// 初始化
initFormData()
</script>

<style lang="scss" scoped>
.metadata-form {
  :deep(.el-form-item__label) {
    font-weight: 500;
  }
}
</style>