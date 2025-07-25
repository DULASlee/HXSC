<template>
  <div>
    <div v-if="error" class="error-boundary">
      <el-alert
        title="组件渲染错误"
        type="error"
        description="页面渲染过程中发生错误，请尝试刷新页面或联系管理员"
        show-icon
        :closable="false"
      >
        <template #default>
          <div class="error-details">
            <p class="error-message">{{ error.message }}</p>
            <div class="error-actions">
              <el-button type="primary" size="small" @click="reload">
                <el-icon><Refresh /></el-icon>
                刷新页面
              </el-button>
              <el-button size="small" @click="goHome">
                <el-icon><House /></el-icon>
                返回首页
              </el-button>
            </div>
          </div>
        </template>
      </el-alert>
    </div>
    <slot v-else></slot>
  </div>
</template>

<script setup lang="ts">
import { ref, onErrorCaptured, provide } from 'vue'
import { useRouter } from 'vue-router'
import { Refresh, House } from '@element-plus/icons-vue'

const error = ref<Error | null>(null)
const router = useRouter()

// 捕获子组件错误
onErrorCaptured((err, instance, info) => {
  console.error('[错误边界捕获]', err, instance, info)
  error.value = err as Error
  
  // 上报错误（可以集成错误监控系统）
  reportError(err, info)
  
  // 阻止错误向上传播
  return false
})

// 提供错误处理方法给子组件
provide('resetError', () => {
  error.value = null
})

// 上报错误
function reportError(err: unknown, info: string) {
  // 这里可以集成错误监控系统，如Sentry
  console.error('[错误上报]', {
    error: err,
    info,
    url: window.location.href,
    time: new Date().toISOString()
  })
}

// 刷新页面
function reload() {
  window.location.reload()
}

// 返回首页
function goHome() {
  error.value = null
  router.push('/')
}
</script>

<style lang="scss" scoped>
.error-boundary {
  padding: 20px;
  
  .error-details {
    margin-top: 12px;
    
    .error-message {
      font-family: monospace;
      background-color: var(--fill-color-dark);
      padding: 8px;
      border-radius: 4px;
      margin-bottom: 12px;
      word-break: break-word;
    }
    
    .error-actions {
      display: flex;
      gap: 8px;
    }
  }
}
</style>