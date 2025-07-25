<template>
  <el-select
    v-model="selectedValue"
    :placeholder="placeholder"
    :multiple="multiple"
    :clearable="clearable"
    :filterable="filterable"
    :disabled="disabled"
    :loading="loading"
    @change="handleChange"
  >
    <el-option
      v-for="option in computedOptions"
      :key="option.value"
      :label="option.label"
      :value="option.value"
      :disabled="option.disabled"
    >
      <div class="option-item">
        <el-icon v-if="option.icon" class="option-icon">
          <component :is="option.icon" />
        </el-icon>
        <span>{{ option.label }}</span>
        <el-tag
          v-if="option.color"
          :color="option.color"
          size="small"
          class="option-tag"
        />
      </div>
    </el-option>
  </el-select>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { MetadataOption } from '@/types/global'

interface Props {
  modelValue?: any
  options?: MetadataOption[]
  placeholder?: string
  multiple?: boolean
  clearable?: boolean
  filterable?: boolean
  disabled?: boolean
  // 动态数据源配置
  dataSource?: {
    url?: string
    method?: 'GET' | 'POST'
    params?: Record<string, any>
    valueField?: string
    labelField?: string
    transform?: (data: any) => MetadataOption[]
  }
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请选择',
  multiple: false,
  clearable: true,
  filterable: false,
  disabled: false
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedValue = ref(props.modelValue)
const loading = ref(false)
const dynamicOptions = ref<MetadataOption[]>([])

// 计算选项列表
const computedOptions = computed(() => {
  if (props.dataSource && dynamicOptions.value.length > 0) {
    return dynamicOptions.value
  }
  return props.options || []
})

// 处理值变化
function handleChange(value: any) {
  emit('update:modelValue', value)
  emit('change', value)
}

// 加载动态数据
async function loadDynamicData() {
  if (!props.dataSource?.url) return

  loading.value = true
  try {
    const response = await fetch(props.dataSource.url, {
      method: props.dataSource.method || 'GET',
      headers: {
        'Content-Type': 'application/json'
      },
      body: props.dataSource.method === 'POST' 
        ? JSON.stringify(props.dataSource.params || {})
        : undefined
    })

    const data = await response.json()
    
    if (props.dataSource.transform) {
      dynamicOptions.value = props.dataSource.transform(data)
    } else {
      // 默认转换逻辑
      const items = Array.isArray(data) ? data : data.items || data.data || []
      dynamicOptions.value = items.map((item: any) => ({
        value: item[props.dataSource?.valueField || 'value'],
        label: item[props.dataSource?.labelField || 'label'],
        color: item.color,
        icon: item.icon,
        sortOrder: item.sortOrder
      }))
    }
  } catch (error) {
    console.error('Failed to load dynamic options:', error)
  } finally {
    loading.value = false
  }
}

// 监听modelValue变化
watch(() => props.modelValue, (newValue) => {
  selectedValue.value = newValue
})

// 监听selectedValue变化
watch(selectedValue, (newValue) => {
  if (newValue !== props.modelValue) {
    handleChange(newValue)
  }
})

// 监听数据源变化
watch(() => props.dataSource, () => {
  if (props.dataSource?.url) {
    loadDynamicData()
  }
}, { deep: true, immediate: true })

onMounted(() => {
  if (props.dataSource?.url) {
    loadDynamicData()
  }
})
</script>

<style lang="scss" scoped>
.option-item {
  display: flex;
  align-items: center;
  gap: 8px;

  .option-icon {
    font-size: 14px;
  }

  .option-tag {
    margin-left: auto;
    width: 12px;
    height: 12px;
    border-radius: 50%;
  }
}
</style>