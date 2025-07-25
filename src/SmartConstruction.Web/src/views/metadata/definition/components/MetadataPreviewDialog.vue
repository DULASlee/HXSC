<template>
  <el-dialog
    v-model="dialogVisible"
    title="元数据预览"
    width="600px"
    :close-on-click-modal="false"
  >
    <div v-if="metadata" class="metadata-preview">
      <!-- 基本信息 -->
      <div class="preview-section">
        <h4>基本信息</h4>
        <el-descriptions :column="2" border>
          <el-descriptions-item label="实体类型">
            <el-tag>{{ metadata.entityType }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="键名">
            <el-tag type="primary">{{ metadata.metaKey }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="显示名称">
            {{ metadata.displayName }}
          </el-descriptions-item>
          <el-descriptions-item label="值类型">
            <el-tag :type="getValueTypeTag(metadata.valueType)">
              {{ metadata.valueType }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="默认值">
            <code v-if="metadata.defaultValue">{{ metadata.defaultValue }}</code>
            <span v-else class="text-muted">无</span>
          </el-descriptions-item>
          <el-descriptions-item label="必填">
            <el-icon v-if="metadata.required" style="color: #f56c6c">
              <CircleCheckFilled />
            </el-icon>
            <el-icon v-else style="color: #909399">
              <CircleClose />
            </el-icon>
          </el-descriptions-item>
          <el-descriptions-item label="UI组件">
            {{ metadata.uiComponent || '默认' }}
          </el-descriptions-item>
          <el-descriptions-item label="排序">
            {{ metadata.sortOrder }}
          </el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="metadata.isActive ? 'success' : 'danger'">
              {{ metadata.isActive ? '启用' : '禁用' }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="创建时间">
            {{ formatDate(metadata.createdAt) }}
          </el-descriptions-item>
        </el-descriptions>
      </div>

      <!-- 描述信息 -->
      <div v-if="metadata.description" class="preview-section">
        <h4>描述信息</h4>
        <div class="description-content">
          {{ metadata.description }}
        </div>
      </div>

      <!-- 验证规则 -->
      <div v-if="metadata.validationRule" class="preview-section">
        <h4>验证规则</h4>
        <div class="validation-rule">
          <code>{{ metadata.validationRule }}</code>
        </div>
      </div>

      <!-- 示例值 -->
      <div v-if="metadata.example" class="preview-section">
        <h4>示例值</h4>
        <div class="example-value">
          <code>{{ metadata.example }}</code>
        </div>
      </div>

      <!-- 选项配置 -->
      <div v-if="metadata.options && metadata.options.length > 0" class="preview-section">
        <h4>选项配置</h4>
        <el-table :data="metadata.options" border size="small">
          <el-table-column prop="value" label="选项值" width="120" />
          <el-table-column prop="label" label="显示标签" width="150" />
          <el-table-column label="颜色" width="80" align="center">
            <template #default="{ row }">
              <div
                v-if="row.color"
                class="color-preview"
                :style="{ backgroundColor: row.color }"
              ></div>
              <span v-else class="text-muted">无</span>
            </template>
          </el-table-column>
          <el-table-column prop="icon" label="图标" width="100">
            <template #default="{ row }">
              <span v-if="row.icon">{{ row.icon }}</span>
              <span v-else class="text-muted">无</span>
            </template>
          </el-table-column>
          <el-table-column prop="sortOrder" label="排序" width="80" align="center" />
        </el-table>
      </div>

      <!-- 表单预览 -->
      <div class="preview-section">
        <h4>表单预览</h4>
        <div class="form-preview">
          <el-form label-width="120px">
            <el-form-item :label="metadata.displayName" :required="metadata.required">
              <component
                :is="getPreviewComponent()"
                v-model="previewValue"
                :placeholder="getPreviewPlaceholder()"
                v-bind="getPreviewProps()"
                style="width: 300px"
              />
            </el-form-item>
          </el-form>
        </div>
      </div>

      <!-- JSON配置 -->
      <div class="preview-section">
        <h4>JSON配置</h4>
        <div class="json-config">
          <el-input
            :model-value="formatJSON(metadata)"
            type="textarea"
            :rows="10"
            readonly
          />
        </div>
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="dialogVisible = false">关闭</el-button>
        <el-button type="primary" @click="handleCopyConfig">
          <el-icon><DocumentCopy /></el-icon>
          复制配置
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { CircleCheckFilled, CircleClose, DocumentCopy } from '@element-plus/icons-vue'
import { formatDate } from '@/utils/format'
import { useClipboard } from '@/composables/useClipboard'

interface Props {
  modelValue: boolean
  metadata: MetadataDefinition | null
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const { copy } = useClipboard()
const previewValue = ref('')

// 弹窗显示状态
const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 获取值类型标签样式
function getValueTypeTag(type: string) {
  const tagMap: Record<string, string> = {
    'String': 'primary',
    'Number': 'success',
    'Boolean': 'warning',
    'Date': 'info',
    'JSON': 'danger'
  }
  return tagMap[type] || 'info'
}

// 获取预览组件
function getPreviewComponent() {
  if (!props.metadata) return 'el-input'
  
  const componentMap: Record<string, string> = {
    'input': 'el-input',
    'textarea': 'el-input',
    'input-number': 'el-input-number',
    'switch': 'el-switch',
    'date-picker': 'el-date-picker',
    'time-picker': 'el-time-picker',
    'datetime-picker': 'el-date-picker',
    'select': 'el-select',
    'radio-group': 'el-radio-group',
    'checkbox-group': 'el-checkbox-group',
    'color-picker': 'el-color-picker'
  }
  
  return componentMap[props.metadata.uiComponent || 'input'] || 'el-input'
}

// 获取预览占位符
function getPreviewPlaceholder() {
  if (!props.metadata) return ''
  
  return props.metadata.example || `请输入${props.metadata.displayName}`
}

// 获取预览组件属性
function getPreviewProps() {
  if (!props.metadata) return {}
  
  const props_: any = {}
  
  switch (props.metadata.uiComponent) {
    case 'textarea':
      props_.type = 'textarea'
      props_.rows = 3
      break
    case 'input-number':
      props_.controlsPosition = 'right'
      break
    case 'date-picker':
      props_.type = 'date'
      break
    case 'datetime-picker':
      props_.type = 'datetime'
      break
    case 'select':
      if (props.metadata.options && props.metadata.options.length > 0) {
        props_.placeholder = '请选择'
      }
      break
  }
  
  return props_
}

// 格式化JSON
function formatJSON(data: any) {
  return JSON.stringify(data, null, 2)
}

// 复制配置
async function handleCopyConfig() {
  if (!props.metadata) return
  
  const config = formatJSON(props.metadata)
  const success = await copy(config)
  
  if (success) {
    ElMessage.success('配置已复制到剪贴板')
  }
}
</script>

<style lang="scss" scoped>
.metadata-preview {
  .preview-section {
    margin-bottom: 24px;
    
    &:last-child {
      margin-bottom: 0;
    }
    
    h4 {
      margin: 0 0 12px 0;
      font-size: 16px;
      font-weight: 600;
      color: var(--el-text-color-primary);
      border-bottom: 1px solid var(--el-border-color-lighter);
      padding-bottom: 8px;
    }
  }
  
  .description-content {
    padding: 12px;
    background-color: var(--el-fill-color-lighter);
    border-radius: 4px;
    line-height: 1.6;
  }
  
  .validation-rule,
  .example-value {
    code {
      display: block;
      padding: 12px;
      background-color: var(--el-fill-color-light);
      border-radius: 4px;
      font-family: 'Courier New', monospace;
      font-size: 14px;
      color: var(--el-color-primary);
    }
  }
  
  .color-preview {
    width: 20px;
    height: 20px;
    border-radius: 50%;
    border: 1px solid var(--el-border-color);
    margin: 0 auto;
  }
  
  .form-preview {
    padding: 20px;
    background-color: var(--el-fill-color-lighter);
    border-radius: 4px;
    border: 1px dashed var(--el-border-color);
  }
  
  .json-config {
    .el-textarea {
      :deep(.el-textarea__inner) {
        font-family: 'Courier New', monospace;
        font-size: 12px;
        line-height: 1.4;
      }
    }
  }
  
  .text-muted {
    color: var(--el-text-color-placeholder);
    font-style: italic;
  }
}

.dialog-footer {
  text-align: right;
}
</style>