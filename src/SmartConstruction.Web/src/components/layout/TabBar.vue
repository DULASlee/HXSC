<template>
  <div class="tab-bar">
    <div class="tab-list" ref="tabListRef">
      <div
        v-for="tab in tabs"
        :key="tab.path"
        class="tab-item"
        :class="{ 'is-active': tab.path === activeTab }"
        @click="handleTabClick(tab)"
        @contextmenu.prevent="handleContextMenu(tab, $event)"
      >
        <el-icon v-if="tab.icon" class="tab-icon">
          <component :is="getIconComponent(tab.icon)" />
        </el-icon>
        <span class="tab-title">{{ tab.title }}</span>
        <el-icon 
          v-if="!tab.affix" 
          class="tab-close"
          @click.stop="handleTabClose(tab)"
        >
          <Close />
        </el-icon>
      </div>
    </div>
    
    <div class="tab-actions">
      <el-dropdown @command="handleActionCommand">
        <el-button text size="small">
          <el-icon><ArrowDown /></el-icon>
        </el-button>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="refresh">
              <el-icon><Refresh /></el-icon>
              刷新当前页
            </el-dropdown-item>
            <el-dropdown-item command="close-current" :disabled="currentTab?.affix">
              <el-icon><Close /></el-icon>
              关闭当前页
            </el-dropdown-item>
            <el-dropdown-item command="close-others">
              <el-icon><Remove /></el-icon>
              关闭其他页
            </el-dropdown-item>
            <el-dropdown-item command="close-all">
              <el-icon><CircleClose /></el-icon>
              关闭所有页
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
    
    <!-- 右键菜单 -->
    <div
      v-if="contextMenuVisible"
      class="context-menu"
      :style="{ left: contextMenuPosition.x + 'px', top: contextMenuPosition.y + 'px' }"
      @click.stop
    >
      <div class="context-menu-item" @click="refreshTab(contextMenuTab)">
        <el-icon><Refresh /></el-icon>
        刷新
      </div>
      <div 
        class="context-menu-item" 
        :class="{ disabled: contextMenuTab?.affix }"
        @click="closeTab(contextMenuTab)"
      >
        <el-icon><Close /></el-icon>
        关闭
      </div>
      <div class="context-menu-item" @click="closeOtherTabs(contextMenuTab)">
        <el-icon><Remove /></el-icon>
        关闭其他
      </div>
      <div class="context-menu-item" @click="closeAllTabs">
        <el-icon><CircleClose /></el-icon>
        关闭所有
      </div>
    </div>
    
    <!-- 遮罩层 -->
    <div
      v-if="contextMenuVisible"
      class="context-menu-overlay"
      @click="hideContextMenu"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, nextTick, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { 
  Close, 
  ArrowDown, 
  Refresh, 
  Remove, 
  CircleClose,
  Document
} from '@element-plus/icons-vue'
import { useAppStore } from '@/stores/app'

const route = useRoute()
const router = useRouter()
const appStore = useAppStore()

// 标签页接口
interface TabItem {
  path: string
  title: string
  icon?: string
  affix?: boolean
}

// 响应式数据
const tabListRef = ref<HTMLElement>()
const contextMenuVisible = ref(false)
const contextMenuPosition = ref({ x: 0, y: 0 })
const contextMenuTab = ref<TabItem | null>(null)

// 计算属性
const tabs = computed(() => appStore.visitedViews)
const activeTab = computed(() => route.path)
const currentTab = computed(() => tabs.value.find(tab => tab.path === activeTab.value))

// 图标组件映射
const iconMap: Record<string, any> = {
  Document,
  // 添加更多图标映射
}

const getIconComponent = (iconName: string) => {
  return iconMap[iconName] || Document
}

// 处理标签页点击
const handleTabClick = (tab: TabItem) => {
  if (tab.path !== activeTab.value) {
    router.push(tab.path)
  }
}

// 处理标签页关闭
const handleTabClose = (tab: TabItem) => {
  closeTab(tab)
}

// 处理右键菜单
const handleContextMenu = (tab: TabItem, event: MouseEvent) => {
  contextMenuTab.value = tab
  contextMenuPosition.value = {
    x: event.clientX,
    y: event.clientY
  }
  contextMenuVisible.value = true
}

// 隐藏右键菜单
const hideContextMenu = () => {
  contextMenuVisible.value = false
  contextMenuTab.value = null
}

// 处理操作命令
const handleActionCommand = (command: string) => {
  switch (command) {
    case 'refresh':
      refreshTab(currentTab.value)
      break
    case 'close-current':
      closeTab(currentTab.value)
      break
    case 'close-others':
      closeOtherTabs(currentTab.value)
      break
    case 'close-all':
      closeAllTabs()
      break
  }
}

// 刷新标签页
const refreshTab = (tab: TabItem | null | undefined) => {
  if (!tab) return
  
  // 如果是当前标签页，刷新页面
  if (tab.path === activeTab.value) {
    window.location.reload()
  }
}

// 关闭标签页
const closeTab = (tab: TabItem | null | undefined) => {
  if (!tab || tab.affix) return
  
  appStore.delVisitedView(tab)
  
  // 如果关闭的是当前标签页，跳转到最后一个标签页
  if (tab.path === activeTab.value) {
    const remainingTabs = tabs.value
    if (remainingTabs.length > 0) {
      const lastTab = remainingTabs[remainingTabs.length - 1]
      router.push(lastTab.path)
    } else {
      router.push('/dashboard')
    }
  }
  
  hideContextMenu()
}

// 关闭其他标签页
const closeOtherTabs = (tab: TabItem | null | undefined) => {
  if (!tab) return
  
  appStore.delOthersVisitedViews(tab)
  
  // 如果当前标签页被关闭，跳转到指定标签页
  if (tab.path !== activeTab.value) {
    router.push(tab.path)
  }
  
  hideContextMenu()
}

// 关闭所有标签页
const closeAllTabs = () => {
  appStore.delAllVisitedViews()
  router.push('/dashboard')
  hideContextMenu()
}

// 监听点击事件，隐藏右键菜单
const handleDocumentClick = () => {
  hideContextMenu()
}

onMounted(() => {
  document.addEventListener('click', handleDocumentClick)
})

onUnmounted(() => {
  document.removeEventListener('click', handleDocumentClick)
})
</script>

<style lang="scss" scoped>
.tab-bar {
  display: flex;
  align-items: center;
  height: 40px;
  background-color: var(--bg-color-overlay);
  border-bottom: 1px solid var(--border-color-lighter);
  position: relative;
  
  .tab-list {
    flex: 1;
    display: flex;
    align-items: center;
    overflow-x: auto;
    @include scrollbar(4px);
    padding: 0 var(--spacing-medium);
    
    .tab-item {
      display: flex;
      align-items: center;
      height: 32px;
      padding: 0 var(--spacing-medium);
      margin-right: var(--spacing-small);
      background-color: var(--fill-color-light);
      border: 1px solid var(--border-color-lighter);
      border-radius: var(--border-radius-base);
      cursor: pointer;
      transition: var(--transition-base);
      white-space: nowrap;
      user-select: none;
      
      &:hover {
        background-color: var(--fill-color);
        border-color: var(--border-color-base);
      }
      
      &.is-active {
        background-color: var(--primary-color);
        border-color: var(--primary-color);
        color: #ffffff;
        
        .tab-icon,
        .tab-close {
          color: #ffffff;
        }
      }
      
      .tab-icon {
        margin-right: var(--spacing-extra-small);
        font-size: var(--font-size-small);
        color: var(--text-color-secondary);
      }
      
      .tab-title {
        font-size: var(--font-size-small);
        margin-right: var(--spacing-extra-small);
      }
      
      .tab-close {
        font-size: var(--font-size-extra-small);
        color: var(--text-color-placeholder);
        border-radius: 50%;
        padding: 2px;
        transition: var(--transition-base);
        
        &:hover {
          background-color: rgba(0, 0, 0, 0.1);
          color: var(--text-color-regular);
        }
      }
    }
  }
  
  .tab-actions {
    padding: 0 var(--spacing-medium);
    border-left: 1px solid var(--border-color-lighter);
  }
  
  .context-menu {
    position: fixed;
    background-color: var(--bg-color-overlay);
    border: 1px solid var(--border-color-lighter);
    border-radius: var(--border-radius-base);
    box-shadow: var(--box-shadow-base);
    z-index: 2000;
    min-width: 120px;
    
    .context-menu-item {
      display: flex;
      align-items: center;
      padding: var(--spacing-small) var(--spacing-medium);
      cursor: pointer;
      transition: var(--transition-base);
      
      &:hover:not(.disabled) {
        background-color: var(--fill-color-light);
        color: var(--primary-color);
      }
      
      &.disabled {
        color: var(--text-color-placeholder);
        cursor: not-allowed;
      }
      
      .el-icon {
        margin-right: var(--spacing-small);
        font-size: var(--font-size-small);
      }
    }
  }
  
  .context-menu-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1999;
  }
}

// 移动端适配
@include respond-to(xs) {
  .tab-bar {
    .tab-list {
      padding: 0 var(--spacing-small);
      
      .tab-item {
        padding: 0 var(--spacing-small);
        margin-right: var(--spacing-extra-small);
        
        .tab-icon {
          display: none;
        }
        
        .tab-title {
          font-size: var(--font-size-extra-small);
        }
      }
    }
    
    .tab-actions {
      padding: 0 var(--spacing-small);
    }
  }
}

// 暗色主题适配
[data-theme="dark"] {
  .tab-bar {
    .tab-list {
      .tab-item {
        background-color: var(--fill-color-dark);
        border-color: var(--border-color-base);
        
        &:hover {
          background-color: var(--fill-color);
        }
      }
    }
    
    .context-menu {
      .context-menu-item {
        &:hover:not(.disabled) {
          background-color: var(--fill-color-dark);
        }
      }
    }
  }
}
</style>