<template>
  <el-button 
    text 
    class="fullscreen-button"
    @click="toggleFullscreen"
    :title="isFullscreen ? '退出全屏' : '进入全屏'"
  >
    <el-icon :size="18">
      <component :is="isFullscreen ? 'FullScreen' : 'Rank'" />
    </el-icon>
  </el-button>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { FullScreen, Rank } from '@element-plus/icons-vue'
import { ElMessage } from 'element-plus'

const isFullscreen = ref(false)

// 切换全屏
const toggleFullscreen = () => {
  if (!document.fullscreenEnabled) {
    ElMessage.warning('您的浏览器不支持全屏功能')
    return
  }
  
  if (isFullscreen.value) {
    exitFullscreen()
  } else {
    enterFullscreen()
  }
}

// 进入全屏
const enterFullscreen = () => {
  const element = document.documentElement
  
  if (element.requestFullscreen) {
    element.requestFullscreen()
  } else if ((element as any).webkitRequestFullscreen) {
    (element as any).webkitRequestFullscreen()
  } else if ((element as any).mozRequestFullScreen) {
    (element as any).mozRequestFullScreen()
  } else if ((element as any).msRequestFullscreen) {
    (element as any).msRequestFullscreen()
  }
}

// 退出全屏
const exitFullscreen = () => {
  if (document.exitFullscreen) {
    document.exitFullscreen()
  } else if ((document as any).webkitExitFullscreen) {
    (document as any).webkitExitFullscreen()
  } else if ((document as any).mozCancelFullScreen) {
    (document as any).mozCancelFullScreen()
  } else if ((document as any).msExitFullscreen) {
    (document as any).msExitFullscreen()
  }
}

// 监听全屏状态变化
const handleFullscreenChange = () => {
  isFullscreen.value = !!(
    document.fullscreenElement ||
    (document as any).webkitFullscreenElement ||
    (document as any).mozFullScreenElement ||
    (document as any).msFullscreenElement
  )
}

// 监听键盘事件
const handleKeydown = (event: KeyboardEvent) => {
  // F11 键切换全屏
  if (event.key === 'F11') {
    event.preventDefault()
    toggleFullscreen()
  }
  
  // Esc 键退出全屏
  if (event.key === 'Escape' && isFullscreen.value) {
    exitFullscreen()
  }
}

onMounted(() => {
  // 监听全屏状态变化
  document.addEventListener('fullscreenchange', handleFullscreenChange)
  document.addEventListener('webkitfullscreenchange', handleFullscreenChange)
  document.addEventListener('mozfullscreenchange', handleFullscreenChange)
  document.addEventListener('MSFullscreenChange', handleFullscreenChange)
  
  // 监听键盘事件
  document.addEventListener('keydown', handleKeydown)
  
  // 初始化状态
  handleFullscreenChange()
})

onUnmounted(() => {
  // 移除事件监听
  document.removeEventListener('fullscreenchange', handleFullscreenChange)
  document.removeEventListener('webkitfullscreenchange', handleFullscreenChange)
  document.removeEventListener('mozfullscreenchange', handleFullscreenChange)
  document.removeEventListener('MSFullscreenChange', handleFullscreenChange)
  document.removeEventListener('keydown', handleKeydown)
})
</script>

<style lang="scss" scoped>
.fullscreen-button {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  height: 36px;
  border-radius: var(--border-radius-base);
  color: var(--text-color-regular);
  transition: var(--transition-base);
  
  &:hover {
    background-color: var(--fill-color-light);
    color: var(--primary-color);
  }
  
  &:active {
    transform: scale(0.95);
  }
}

// 全屏状态下的样式调整
:fullscreen {
  .fullscreen-button {
    color: var(--primary-color);
  }
}

// 暗色主题适配
[data-theme="dark"] {
  .fullscreen-button {
    &:hover {
      background-color: var(--fill-color-dark);
    }
  }
}
</style>