<template>
  <div class="error-boundary">
    <slot v-if="!hasError" />
    
    <!-- 错误状态显示 -->
    <div v-else class="error-container">
      <div class="error-content">
        <div class="error-icon">
          <el-icon size="64" color="var(--color-danger)">
            <Warning />
          </el-icon>
        </div>
        
        <h3 class="error-title">页面出现了问题</h3>
        
        <div class="error-description">
          <p v-if="userFriendlyMessage">{{ userFriendlyMessage }}</p>
          <p v-else>抱歉，页面遇到了意外错误，请稍后重试。</p>
        </div>
        
        <div class="error-actions">
          <el-button type="primary" @click="handleReload">
            刷新页面
          </el-button>
          <el-button @click="handleGoBack">
            返回上页
          </el-button>
          <el-button 
            v-if="!isProduction" 
            type="info" 
            @click="showErrorDetails = !showErrorDetails"
          >
            {{ showErrorDetails ? '隐藏' : '显示' }}错误详情
          </el-button>
        </div>
        
        <!-- 开发环境下显示错误详情 -->
        <div v-if="!isProduction && showErrorDetails" class="error-details">
          <el-collapse>
            <el-collapse-item title="错误信息" name="error">
              <pre class="error-message">{{ errorMessage }}</pre>
            </el-collapse-item>
            <el-collapse-item title="错误堆栈" name="stack">
              <pre class="error-stack">{{ errorStack }}</pre>
            </el-collapse-item>
            <el-collapse-item title="组件信息" name="component">
              <pre class="error-component">{{ componentStack }}</pre>
            </el-collapse-item>
          </el-collapse>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onErrorCaptured, computed } from 'vue'
import { useRouter } from 'vue-router'
import { Warning } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

interface Props {
  fallback?: string
  onError?: (error: Error, instance: any, info: string) => void
}

const props = withDefaults(defineProps<Props>(), {
  fallback: '页面出现异常'
})

const router = useRouter()

// 状态
const hasError = ref(false)
const errorMessage = ref('')
const errorStack = ref('')
const componentStack = ref('')
const showErrorDetails = ref(false)

// 计算属性
const isProduction = computed(() => process.env.NODE_ENV === 'production')

const userFriendlyMessage = computed(() => {
  const error = errorMessage.value.toLowerCase()
  
  if (error.includes('network') || error.includes('fetch')) {
    return '网络连接出现问题，请检查网络后重试'
  }
  
  if (error.includes('loading chunk')) {
    return '资源加载失败，请刷新页面重试'
  }
  
  if (error.includes('permission') || error.includes('unauthorized')) {
    return '权限验证失败，请重新登录'
  }
  
  if (error.includes('timeout')) {
    return '请求超时，请稍后重试'
  }
  
  return props.fallback
})

/// <summary>
/// 捕获错误
/// </summary>
onErrorCaptured((error: Error, instance: any, info: string) => {
  console.error('[错误边界] 捕获到错误:', error)
  console.error('[错误边界] 组件实例:', instance)
  console.error('[错误边界] 错误信息:', info)
  
  hasError.value = true
  errorMessage.value = error.message || '未知错误'
  errorStack.value = error.stack || '无堆栈信息'
  componentStack.value = info || '无组件信息'
  
  // 错误上报
  reportError(error, instance, info)
  
  // 调用自定义错误处理
  props.onError?.(error, instance, info)
  
  // 阻止错误向上传播
  return false
})

/// <summary>
/// 错误上报
/// </summary>
function reportError(error: Error, instance: any, info: string) {
  const errorReport = {
    message: error.message,
    stack: error.stack,
    componentInfo: info,
    userAgent: navigator.userAgent,
    url: window.location.href,
    timestamp: new Date().toISOString(),
    userId: '', // 可以从用户store获取
    version: process.env.VITE_APP_VERSION || '1.0.0'
  }
  
  // 在生产环境中，可以发送到错误监控服务
  if (isProduction.value) {
    // 这里可以集成 Sentry、Bugsnag 等错误监控服务
    console.log('[错误上报]', errorReport)
  } else {
    console.warn('[错误报告]', errorReport)
  }
}

/// <summary>
/// 处理重新加载
/// </summary>
function handleReload() {
  window.location.reload()
}

/// <summary>
/// 处理返回上页
/// </summary>
function handleGoBack() {
  if (window.history.length > 1) {
    router.go(-1)
  } else {
    router.push('/')
  }
}

/// <summary>
/// 重置错误状态
/// </summary>
function resetError() {
  hasError.value = false
  errorMessage.value = ''
  errorStack.value = ''
  componentStack.value = ''
  showErrorDetails.value = false
}

// 监听全局错误
window.addEventListener('error', (event) => {
  console.error('[全局错误]', event.error)
  ElMessage.error('页面出现异常，请刷新重试')
})

window.addEventListener('unhandledrejection', (event) => {
  console.error('[未处理的Promise拒绝]', event.reason)
  ElMessage.error('操作失败，请稍后重试')
  event.preventDefault()
})

// 暴露重置方法
defineExpose({
  resetError
})
</script>

<style lang="scss" scoped>
.error-boundary {
  min-height: 100%;
}

.error-container {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 400px;
  padding: var(--spacing-extra-large);
  background-color: var(--bg-color-page);
}

.error-content {
  text-align: center;
  max-width: 600px;
  
  .error-icon {
    margin-bottom: var(--spacing-large);
  }
  
  .error-title {
    font-size: var(--font-size-extra-large);
    font-weight: var(--font-weight-primary);
    color: var(--text-color-primary);
    margin-bottom: var(--spacing-medium);
  }
  
  .error-description {
    color: var(--text-color-regular);
    margin-bottom: var(--spacing-extra-large);
    line-height: 1.6;
    
    p {
      margin: 0 0 var(--spacing-small) 0;
    }
  }
  
  .error-actions {
    display: flex;
    justify-content: center;
    gap: var(--spacing-medium);
    margin-bottom: var(--spacing-extra-large);
    flex-wrap: wrap;
  }
  
  .error-details {
    text-align: left;
    margin-top: var(--spacing-large);
    
    .error-message,
    .error-stack,
    .error-component {
      background-color: var(--fill-color-light);
      padding: var(--spacing-medium);
      border-radius: var(--border-radius-base);
      font-family: var(--font-family-mono);
      font-size: var(--font-size-small);
      white-space: pre-wrap;
      word-break: break-all;
      max-height: 300px;
      overflow-y: auto;
    }
  }
}

// 响应式适配
@media (max-width: 768px) {
  .error-container {
    padding: var(--spacing-large);
    min-height: 300px;
  }
  
  .error-content {
    .error-title {
      font-size: var(--font-size-large);
    }
    
    .error-actions {
      flex-direction: column;
      align-items: center;
      
      .el-button {
        width: 100%;
        max-width: 200px;
      }
    }
  }
}

// 暗色主题适配
[data-theme="dark"] {
  .error-content {
    .error-details {
      .error-message,
      .error-stack,
      .error-component {
        background-color: var(--fill-color-dark);
        color: var(--text-color-primary);
      }
    }
  }
}
</style>
