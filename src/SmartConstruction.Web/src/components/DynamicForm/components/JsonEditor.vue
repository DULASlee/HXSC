<template>
  <div class="json-editor">
    <div class="editor-toolbar">
      <el-button-group size="small">
        <el-button @click="formatJson">
          <el-icon><Document /></el-icon>
          格式化
        </el-button>
        <el-button @click="compressJson">
          <el-icon><Compress /></el-icon>
          压缩
        </el-button>
        <el-button @click="validateJson">
          <el-icon><Check /></el-icon>
          验证
        </el-button>
      </el-button-group>
      <div class="editor-status">
        <el-tag v-if="isValid" type="success" size="small">JSON 有效</el-tag>
        <el-tag v-else-if="errorMessage" type="danger" size="small">{{ errorMessage }}</el-tag>
      </div>
    </div>
    
    <el-input
      v-model="jsonContent"
      type="textarea"
      :rows="rows"
      :placeholder="placeholder"
      :disabled="disabled"
      :readonly="readonly"
      class="json-textarea"
      @input="handleInput"
      @blur="handleBlur"
    />
    
    <div v-if="showPreview && isValid" class="json-preview">
      <el-divider content-position="left">预览</el-divider>
      <div class="preview-content">
        <json-viewer :value="parsedJson" :expand-depth="2" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick } from 'vue'
import { ElMessage } from 'element-plus'
import { Document, Compress, Check } from '@element-plus/icons-vue'
import JsonViewer from './JsonViewer.vue'

interface Props {
  modelValue?: any
  placeholder?: string
  rows?: number
  disabled?: boolean
  readonly?: boolean
  showPreview?: boolean
  autoFormat?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请输入JSON格式数据',
  rows: 6,
  disabled: false,
  readonly: false,
  showPreview: true,
  autoFormat: false
})

const emit = defineEmits(['update:modelValue', 'change', 'error'])

const jsonContent = ref('')
const isValid = ref(true)
const errorMessage = ref('')

// 解析后的JSON对象
const parsedJson = computed(() => {
  try {
    return JSON.parse(jsonContent.value || '{}')
  } catch {
    return {}
  }
})

// 初始化内容
function initContent() {
  if (props.modelValue) {
    if (typeof props.modelValue === 'string') {
      jsonContent.value = props.modelValue
    } else {
      jsonContent.value = JSON.stringify(props.modelValue, null, 2)
    }
  } else {
    jsonContent.value = ''
  }
  validateJson()
}

// 处理输入
function handleInput() {
  if (props.autoFormat) {
    nextTick(() => {
      formatJson(false)
    })
  }
  validateJson()
  emitChange()
}

// 处理失焦
function handleBlur() {
  if (props.autoFormat && isValid.value) {
    formatJson()
  }
}

// 验证JSON
function validateJson() {
  if (!jsonContent.value.trim()) {
    isValid.value = true
    errorMessage.value = ''
    return true
  }

  try {
    JSON.parse(jsonContent.value)
    isValid.value = true
    errorMessage.value = ''
    return true
  } catch (error) {
    isValid.value = false
    errorMessage.value = error instanceof Error ? error.message : 'JSON格式错误'
    emit('error', errorMessage.value)
    return false
  }
}

// 格式化JSON
function formatJson(showMessage = true) {
  if (!jsonContent.value.trim()) return

  try {
    const parsed = JSON.parse(jsonContent.value)
    jsonContent.value = JSON.stringify(parsed, null, 2)
    isValid.value = true
    errorMessage.value = ''
    emitChange()
    if (showMessage) {
      ElMessage.success('JSON格式化成功')
    }
  } catch (error) {
    ElMessage.error('JSON格式错误，无法格式化')
  }
}

// 压缩JSON
function compressJson() {
  if (!jsonContent.value.trim()) return

  try {
    const parsed = JSON.parse(jsonContent.value)
    jsonContent.value = JSON.stringify(parsed)
    isValid.value = true
    errorMessage.value = ''
    emitChange()
    ElMessage.success('JSON压缩成功')
  } catch (error) {
    ElMessage.error('JSON格式错误，无法压缩')
  }
}

// 发出变化事件
function emitChange() {
  let value: any = jsonContent.value

  if (isValid.value && jsonContent.value.trim()) {
    try {
      value = JSON.parse(jsonContent.value)
    } catch {
      // 保持字符串格式
    }
  }

  emit('update:modelValue', value)
  emit('change', value)
}

// 监听外部值变化
watch(() => props.modelValue, () => {
  initContent()
}, { immediate: true })

// 初始化
initContent()
</script>

<style lang="scss" scoped>
.json-editor {
  .editor-toolbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 8px;

    .editor-status {
      .el-tag {
        font-size: 12px;
      }
    }
  }

  .json-textarea {
    font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
    
    :deep(.el-textarea__inner) {
      font-family: inherit;
      line-height: 1.5;
    }
  }

  .json-preview {
    margin-top: 16px;

    .preview-content {
      max-height: 300px;
      overflow-y: auto;
      border: 1px solid var(--el-border-color);
      border-radius: 4px;
      padding: 12px;
      background-color: var(--el-bg-color-page);
    }
  }
}
</style>