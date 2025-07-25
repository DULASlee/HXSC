<template>
  <div id="app">
    <!-- 登录页面使用简单布局 -->
    <template v-if="isLoginPage">
      <router-view />
    </template>
    
    <!-- 其他页面使用默认布局 -->
    <template v-else>
      <router-view />
    </template>
    
    <!-- 全局错误处理 -->
    <global-error-handler ref="errorHandlerRef" />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, provide } from 'vue'
import { useRoute } from 'vue-router'
import { useAppStore } from './stores/app'
import { themeService } from './services/themeService'
import DefaultLayout from './layouts/DefaultLayout.vue'
import GlobalErrorHandler from './components/GlobalErrorHandler.vue'

const route = useRoute()
const appStore = useAppStore()
const errorHandlerRef = ref<InstanceType<typeof GlobalErrorHandler> | null>(null)

// 计算属性
const isLoginPage = computed(() => {
  const loginRoutes = ['/login', '/register', '/forgot-password']
  return loginRoutes.includes(route.path)
})

// 提供全局错误处理函数给子组件
const handleGlobalError = (error: any, options?: any) => {
  errorHandlerRef.value?.addError(error, options)
}

provide('handleGlobalError', handleGlobalError)

onMounted(() => {
  // 初始化应用设置
  appStore.initAppSettings()
  
  // 主题服务已在构造函数中自动初始化
  // 确保主题正确应用
  const currentTheme = themeService.getCurrentTheme()
  document.documentElement.setAttribute('data-theme', currentTheme)
  
  // 设置全局错误处理
  window.$handleError = handleGlobalError
})
</script>

<style lang="scss">
// 全局样式已在 main.ts 中导入
</style>