<template>
  <div v-if="visible" :class="['error-message', type]">
    <div class="error-icon">
      <el-icon v-if="type === 'error'"><CircleClose /></el-icon>
      <el-icon v-else-if="type === 'warning'"><Warning /></el-icon>
      <el-icon v-else-if="type === 'info'"><InfoFilled /></el-icon>
    </div>
    <div class="error-content">
      <div v-if="title" class="error-title">{{ title }}</div>
      <div class="error-text">{{ message }}</div>
      <div v-if="showDetails && details" class="error-details">
        <el-collapse>
          <el-collapse-item title="查看详细信息">
            <pre>{{ details }}</pre>
          </el-collapse-item>
        </el-collapse>
      </div>
      <div v-if="showActions" class="error-actions">
        <el-button v-if="showRetry" size="small" @click="handleRetry">重试</el-button>
        <el-button v-if="showClose" size="small" @click="handleClose">关闭</el-button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { CircleClose, Warning, InfoFilled } from '@element-plus/icons-vue'

const props = defineProps({
  // 错误类型: error, warning, info
  type: {
    type: String,
    default: 'error',
    validator: (value: string) => ['error', 'warning', 'info'].includes(value)
  },
  // 错误标题
  title: {
    type: String,
    default: ''
  },
  // 错误消息
  message: {
    type: String,
    required: true
  },
  // 错误详情
  details: {
    type: String,
    default: ''
  },
  // 是否显示详情
  showDetails: {
    type: Boolean,
    default: false
  },
  // 是否显示操作按钮
  showActions: {
    type: Boolean,
    default: false
  },
  // 是否显示重试按钮
  showRetry: {
    type: Boolean,
    default: false
  },
  // 是否显示关闭按钮
  showClose: {
    type: Boolean,
    default: true
  },
  // 自动关闭时间(ms)，0表示不自动关闭
  duration: {
    type: Number,
    default: 0
  }
})

const emit = defineEmits(['retry', 'close'])

// 是否可见
const visible = ref(true)

// 处理重试
const handleRetry = () => {
  emit('retry')
}

// 处理关闭
const handleClose = () => {
  visible.value = false
  emit('close')
}

// 自动关闭
watch(() => props.duration, (newVal) => {
  if (newVal > 0) {
    setTimeout(() => {
      handleClose()
    }, newVal)
  }
}, { immediate: true })
</script>

<style lang="scss" scoped>
.error-message {
  display: flex;
  padding: 12px 16px;
  border-radius: 4px;
  margin-bottom: 16px;
  
  &.error {
    background-color: var(--el-color-danger-light-9);
    border-left: 4px solid var(--el-color-danger);
    
    .error-icon {
      color: var(--el-color-danger);
    }
  }
  
  &.warning {
    background-color: var(--el-color-warning-light-9);
    border-left: 4px solid var(--el-color-warning);
    
    .error-icon {
      color: var(--el-color-warning);
    }
  }
  
  &.info {
    background-color: var(--el-color-info-light-9);
    border-left: 4px solid var(--el-color-info);
    
    .error-icon {
      color: var(--el-color-info);
    }
  }
  
  .error-icon {
    margin-right: 12px;
    font-size: 20px;
    display: flex;
    align-items: flex-start;
  }
  
  .error-content {
    flex: 1;
    
    .error-title {
      font-weight: bold;
      margin-bottom: 4px;
    }
    
    .error-text {
      line-height: 1.5;
    }
    
    .error-details {
      margin-top: 8px;
      
      pre {
        white-space: pre-wrap;
        word-break: break-word;
        background-color: var(--el-fill-color-light);
        padding: 8px;
        border-radius: 4px;
        font-family: monospace;
        font-size: 12px;
        max-height: 200px;
        overflow: auto;
      }
    }
    
    .error-actions {
      margin-top: 12px;
      display: flex;
      justify-content: flex-end;
      gap: 8px;
    }
  }
}
</style>