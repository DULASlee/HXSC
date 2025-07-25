<template>
  <div class="app-layout" :class="{ 'is-mobile': isMobile, 'sidebar-collapsed': isSidebarCollapsed }">
    <!-- 头部 -->
    <header class="app-header">
      <div class="app-header__left">
        <!-- 菜单切换按钮 -->
        <el-button
          class="menu-toggle"
          :icon="isSidebarCollapsed ? Expand : Fold"
          @click="toggleSidebar"
          text
        />
        
        <!-- Logo -->
        <div class="app-header__logo">
          <img src="/logo.png" alt="Logo" />
          <h1 class="app-header__title" v-if="!isMobile">{{ appTitle }}</h1>
        </div>
      </div>
      
      <!-- 导航区域 -->
      <nav class="app-header__nav">
        <breadcrumb />
      </nav>
      
      <!-- 右侧操作区域 -->
      <div class="app-header__actions">
        <!-- 语言切换 -->
        <language-switcher />
        
        <!-- 搜索 -->
        <header-search v-if="!isMobile" />
        
        <!-- 通知 -->
        <header-notification />
        
        <!-- 全屏 -->
        <header-fullscreen v-if="!isMobile" />
        
        <!-- 主题切换 -->
        <theme-switcher />
        
        <!-- 用户菜单 -->
        <user-dropdown />
      </div>
    </header>
    
    <!-- 主体内容 -->
    <main class="app-main">
      <!-- 侧边栏 -->
      <aside 
        class="app-sidebar" 
        :class="{ 
          'is-collapsed': isSidebarCollapsed,
          'is-open': isMobile && isSidebarOpen 
        }"
      >
        <div class="app-sidebar__wrapper">
          <sidebar-menu :menus="menuStore.menus" :collapse="isSidebarCollapsed" />
        </div>
      </aside>
      
      <!-- 内容区域 -->
      <section class="app-content">
        <!-- 标签页 -->
        <div class="app-tabs" v-if="showTabs">
          <tab-bar />
        </div>
        
        <!-- 页面内容 -->
        <div class="app-page">
          <div class="app-page__wrapper">
            <global-error-handler>
              <router-view v-slot="{ Component, route }">
                <transition name="fade" mode="out-in">
                  <keep-alive :include="cachedViews">
                    <component :is="Component" :key="route.path" />
                  </keep-alive>
                </transition>
              </router-view>
            </global-error-handler>
          </div>
        </div>
      </section>
    </main>
    
    <!-- 移动端遮罩 -->
    <div 
      v-if="isMobile && isSidebarOpen" 
      class="app-overlay"
      @click="closeSidebar"
    ></div>
    
    <!-- 底部 -->
    <footer class="app-footer" v-if="showFooter && !isMobile">
      <app-footer />
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'
import { useRoute } from 'vue-router'
import { Expand, Fold } from '@element-plus/icons-vue'
import { useAppStore } from '@/stores/app'
import { useUserStore } from '@/stores/user'
import { useSettingsStore } from '@/stores/settings'
import { useMenuStore } from '@/stores/menu'

// 组件导入
import Breadcrumb from '@/components/layout/Breadcrumb.vue'
import HeaderSearch from '@/components/layout/HeaderSearch.vue'
import HeaderNotification from '@/components/layout/HeaderNotification.vue'
import HeaderFullscreen from '@/components/layout/HeaderFullscreen.vue'
import ThemeSwitcher from '@/components/layout/ThemeSwitcher.vue'
import LanguageSwitcher from '@/components/layout/LanguageSwitcher.vue'
import UserDropdown from '@/components/layout/UserDropdown.vue'
import SidebarMenu from '@/components/layout/SidebarMenu.vue'
import TabBar from '@/components/layout/TabBar.vue'
import AppFooter from '@/components/layout/AppFooter.vue'
import GlobalErrorHandler from '@/components/GlobalErrorHandler.vue'

const route = useRoute()
const appStore = useAppStore()
const userStore = useUserStore()
const settingsStore = useSettingsStore()
const menuStore = useMenuStore()

// 响应式状态
const isMobile = ref(false)
const isSidebarCollapsed = ref(false)
const isSidebarOpen = ref(false)

// 计算属性
const appTitle = computed(() => import.meta.env.VITE_APP_TITLE || '智慧建设管理平台')
const showTabs = computed(() => settingsStore.showTabs)
const showFooter = computed(() => settingsStore.showFooter)
const cachedViews = computed(() => appStore.cachedViews)

// CSS 变量计算
const cssVars = computed(() => {
  const sidebarWidth = isSidebarCollapsed.value ? 'var(--sidebar-collapsed-width)' : 'var(--sidebar-width)'
  const contentMargin = isMobile.value ? '0' : sidebarWidth
  
  return {
    '--dynamic-sidebar-width': sidebarWidth,
    '--dynamic-content-margin': contentMargin
  }
})

// 检测移动端
const checkMobile = () => {
  const width = window.innerWidth
  const wasMobile = isMobile.value
  isMobile.value = width < 768
  
  // 移动端状态变化处理
  if (isMobile.value !== wasMobile) {
    if (isMobile.value) {
      // 切换到移动端
      isSidebarOpen.value = false
      isSidebarCollapsed.value = true
    } else {
      // 切换到桌面端
      isSidebarCollapsed.value = settingsStore.sidebarCollapsed
      isSidebarOpen.value = false
    }
    
    // 强制重新计算布局
    nextTick(() => {
      window.dispatchEvent(new Event('resize'))
    })
  }
}

// 切换侧边栏
const toggleSidebar = () => {
  if (isMobile.value) {
    isSidebarOpen.value = !isSidebarOpen.value
  } else {
    isSidebarCollapsed.value = !isSidebarCollapsed.value
    settingsStore.setSidebarCollapsed(isSidebarCollapsed.value)
  }
  
  // 延迟触发resize事件，确保布局更新
  nextTick(() => {
    setTimeout(() => {
      window.dispatchEvent(new Event('resize'))
    }, 300)
  })
}

// 关闭侧边栏（移动端）
const closeSidebar = () => {
  if (isMobile.value) {
    isSidebarOpen.value = false
  }
}

// 防抖的窗口大小变化处理
let resizeTimer: NodeJS.Timeout
const handleResize = () => {
  clearTimeout(resizeTimer)
  resizeTimer = setTimeout(() => {
    checkMobile()
  }, 100)
}

// 键盘快捷键处理
const handleKeydown = (event: KeyboardEvent) => {
  // Ctrl/Cmd + B 切换侧边栏
  if ((event.ctrlKey || event.metaKey) && event.key === 'b') {
    event.preventDefault()
    toggleSidebar()
  }
  
  // Esc 关闭移动端侧边栏
  if (event.key === 'Escape' && isMobile.value && isSidebarOpen.value) {
    closeSidebar()
  }
}

// 监听设置变化
watch(
  () => settingsStore.sidebarCollapsed,
  (newVal) => {
    if (!isMobile.value) {
      isSidebarCollapsed.value = newVal
    }
  },
  { immediate: true }
)

// 组件挂载
onMounted(() => {
  checkMobile()
  window.addEventListener('resize', handleResize, { passive: true })
  window.addEventListener('keydown', handleKeydown)
  
  // 初始化设置
  if (!isMobile.value) {
    isSidebarCollapsed.value = settingsStore.sidebarCollapsed
  }
  
  // 确保初始布局正确
  nextTick(() => {
    window.dispatchEvent(new Event('resize'))
  })
})

// 组件卸载
onUnmounted(() => {
  window.removeEventListener('resize', handleResize)
  window.removeEventListener('keydown', handleKeydown)
  clearTimeout(resizeTimer)
})
</script>

<style lang="scss" scoped>
// CSS变量定义
:root {
  --header-height: 60px;
  --sidebar-width: 260px;
  --sidebar-collapsed-width: 64px;
  --footer-height: 50px;
  --z-index-header: 1000;
  --z-index-sidebar: 100;
  --z-index-overlay: 999;
  --transition-base: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  --transition-fade: opacity 0.3s ease;
}

.app-layout {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: var(--bg-color-page);
  position: relative;
  
  // 动态CSS变量
  --dynamic-sidebar-width: var(--sidebar-width);
  --dynamic-content-margin: var(--sidebar-width);
  
  // 头部样式
  .app-header {
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    height: var(--header-height);
    background-color: var(--bg-color-overlay);
    border-bottom: 1px solid var(--border-color-lighter);
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    z-index: var(--z-index-header);
    display: flex;
    align-items: center;
    padding: 0 var(--spacing-large);
    transition: var(--transition-base);
    
    .app-header__left {
      display: flex;
      align-items: center;
      flex-shrink: 0;
      
      .menu-toggle {
        margin-right: var(--spacing-medium);
        font-size: var(--font-size-large);
        transition: var(--transition-base);
        
        &:hover {
          background-color: var(--fill-color-light);
        }
      }
      
      .app-header__logo {
        display: flex;
        align-items: center;
        margin-right: var(--spacing-extra-large);
        
        img {
          height: 32px;
          margin-right: var(--spacing-small);
        }
        
        .app-header__title {
          font-size: var(--font-size-large);
          font-weight: var(--font-weight-primary);
          color: var(--text-color-primary);
          margin: 0;
          white-space: nowrap;
        }
      }
    }
    
    .app-header__nav {
      flex: 1;
      display: flex;
      align-items: center;
      min-width: 0;
      margin: 0 var(--spacing-large);
    }
    
    .app-header__actions {
      display: flex;
      align-items: center;
      gap: var(--spacing-small);
      flex-shrink: 0;
    }
  }
  
  // 主体内容样式
  .app-main {
    display: flex;
    flex: 1;
    margin-top: var(--header-height);
    position: relative;
    overflow: hidden;
    
    // 侧边栏样式
    .app-sidebar {
      width: var(--dynamic-sidebar-width);
      background-color: var(--bg-color-overlay);
      border-right: 1px solid var(--border-color-lighter);
      transition: var(--transition-base);
      position: fixed;
      top: var(--header-height);
      bottom: 0;
      left: 0;
      z-index: var(--z-index-sidebar);
      display: flex;
      flex-direction: column;
      
      &.is-collapsed {
        width: var(--sidebar-collapsed-width);
      }
      
      .app-sidebar__wrapper {
        flex: 1;
        overflow-y: auto;
        overflow-x: hidden;
        @include scrollbar();
      }
    }
    
    // 内容区域样式
    .app-content {
      flex: 1;
      display: flex;
      flex-direction: column;
      min-width: 0;
      margin-left: var(--dynamic-content-margin);
      transition: var(--transition-base);
      position: relative;
      
      // 标签页样式
      .app-tabs {
        background-color: var(--bg-color-overlay);
        border-bottom: 1px solid var(--border-color-lighter);
        min-height: 40px;
        flex-shrink: 0;
        z-index: 10;
      }
      
      // 页面内容样式
      .app-page {
        flex: 1;
        overflow: hidden;
        background-color: var(--bg-color-page);
        
        .app-page__wrapper {
          height: 100%;
          padding: var(--spacing-large);
          overflow-y: auto;
          overflow-x: hidden;
          @include scrollbar();
          
          // 清除浮动
          &::after {
            content: "";
            display: table;
            clear: both;
          }
          
          // 确保内容容器不会超出
          & > * {
            max-width: 100%;
            box-sizing: border-box;
          }
        }
      }
    }
  }
  
  // 底部样式
  .app-footer {
    height: var(--footer-height);
    background-color: var(--bg-color-overlay);
    border-top: 1px solid var(--border-color-lighter);
    display: flex;
    align-items: center;
    justify-content: center;
    flex-shrink: 0;
    z-index: 10;
  }
  
  // 遮罩层样式
  .app-overlay {
    position: fixed;
    top: var(--header-height);
    left: 0;
    width: 100%;
    height: calc(100vh - var(--header-height));
    background-color: rgba(0, 0, 0, 0.5);
    z-index: var(--z-index-overlay);
    transition: var(--transition-fade);
  }
  
  // 响应式布局 - 移动端适配
  &.is-mobile {
    .app-header {
      padding: 0 var(--spacing-medium);
      
      .app-header__left {
        .app-header__logo {
          margin-right: var(--spacing-medium);
          
          .app-header__title {
            display: none;
          }
        }
      }
      
      .app-header__nav {
        margin: 0 var(--spacing-small);
      }
    }
    
    .app-main {
      .app-sidebar {
        width: var(--sidebar-width);
        left: -100%;
        transition: left var(--transition-base);
        
        &.is-open {
          left: 0;
        }
      }
      
      .app-content {
        margin-left: 0;
        width: 100%;
        
        .app-page {
          .app-page__wrapper {
            padding: var(--spacing-medium);
          }
        }
      }
    }
  }
  
  // 侧边栏折叠状态的内容区域调整
  &.sidebar-collapsed:not(.is-mobile) {
    .app-main .app-content {
      margin-left: var(--sidebar-collapsed-width);
    }
  }
}

// 过渡动画
.fade-enter-active,
.fade-leave-active {
  transition: var(--transition-fade);
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

// 滚动条样式混入
@mixin scrollbar() {
  &::-webkit-scrollbar {
    width: 6px;
    height: 6px;
  }
  
  &::-webkit-scrollbar-track {
    background: transparent;
  }
  
  &::-webkit-scrollbar-thumb {
    background-color: var(--border-color-light);
    border-radius: 3px;
    
    &:hover {
      background-color: var(--border-color);
    }
  }
}

// 强制重新计算布局的辅助类
.force-layout-recalc {
  transform: translateZ(0);
}
</style>