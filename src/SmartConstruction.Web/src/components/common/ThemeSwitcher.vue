<template>
  <div class="theme-switcher">
    <!-- 主题切换按钮 -->
    <el-dropdown 
      trigger="click" 
      placement="bottom-end"
      @command="handleThemeChange"
    >
      <el-button 
        :icon="Brush" 
        circle 
        :title="$t('system.theme')"
        class="theme-button"
      />
      
      <template #dropdown>
        <el-dropdown-menu class="theme-dropdown">
          <div class="theme-header">
            <el-icon><Brush /></el-icon>
            <span>{{ $t('system.theme') }}</span>
          </div>
          
          <div class="theme-grid">
            <div
              v-for="theme in themes"
              :key="theme.name"
              :class="[
                'theme-item',
                { 'theme-item--active': currentTheme === theme.name }
              ]"
              @click="handleThemeChange(theme.name)"
            >
              <div 
                class="theme-preview"
                :style="{ background: theme.preview }"
              >
                <div class="theme-icon">{{ theme.icon }}</div>
              </div>
              <div class="theme-info">
                <div class="theme-label">{{ theme.label }}</div>
                <div class="theme-description">{{ theme.description }}</div>
              </div>
              <el-icon v-if="currentTheme === theme.name" class="theme-check">
                <Check />
              </el-icon>
            </div>
          </div>
          
          <el-divider style="margin: 8px 0;" />
          
          <el-dropdown-item 
            command="reset"
            class="theme-reset"
          >
            <el-icon><RefreshLeft /></el-icon>
            <span>重置为默认主题</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Brush, Check, RefreshLeft } from '@element-plus/icons-vue'
import { themeService, THEME_CONFIGS, type ThemeType } from '@/services/themeService'
import { useAppStore } from '@/stores/app'
import { useI18nStore } from '@/stores/i18n'

const appStore = useAppStore()
const i18nStore = useI18nStore()

// 当前主题
const currentTheme = ref<ThemeType>(themeService.getCurrentTheme())

// 所有主题配置
const themes = computed(() => themeService.getAllThemes())

// 处理主题变化
const handleThemeChange = async (command: string) => {
  if (command === 'reset') {
    await themeService.resetTheme()
    ElMessage.success('主题已重置为默认')
    return
  }
  
  const theme = command as ThemeType
  if (theme === currentTheme.value) return
  
  try {
    // 切换主题
    await themeService.setTheme(theme)
    
    // 更新store
    appStore.setTheme(theme)
    
    // 显示成功消息
    ElMessage.success(`已切换到${THEME_CONFIGS[theme].label}`)
    
  } catch (error) {
    console.error('主题切换失败:', error)
    ElMessage.error('主题切换失败，请重试')
  }
}

// 监听主题变化事件
const handleThemeChangeEvent = (event: CustomEvent) => {
  currentTheme.value = event.detail.theme
}

onMounted(() => {
  // 监听主题变化
  window.addEventListener('theme-change', handleThemeChangeEvent as EventListener)
  
  // 同步当前主题
  currentTheme.value = themeService.getCurrentTheme()
})

onUnmounted(() => {
  window.removeEventListener('theme-change', handleThemeChangeEvent as EventListener)
})
</script>

<style lang="scss" scoped>
.theme-switcher {
  .theme-button {
    border: none;
    background: transparent;
    color: var(--navbar-text);
    
    &:hover {
      background: var(--sidebar-hover-bg);
    }
  }
}

.theme-dropdown {
  width: 320px;
  padding: 0;
  
  .theme-header {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 12px 16px;
    font-weight: 500;
    color: var(--text-primary);
    border-bottom: 1px solid var(--border-color-light);
    background: var(--bg-elevated);
  }
  
  .theme-grid {
    padding: 12px;
    display: flex;
    flex-direction: column;
    gap: 8px;
  }
  
  .theme-item {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 12px;
    border-radius: var(--radius-md);
    cursor: pointer;
    transition: all 0.2s ease;
    border: 2px solid transparent;
    
    &:hover {
      background: var(--bg-elevated);
      transform: translateY(-1px);
    }
    
    &--active {
      border-color: var(--primary-color);
      background: var(--primary-bg);
    }
  }
  
  .theme-preview {
    width: 40px;
    height: 40px;
    border-radius: var(--radius-md);
    display: flex;
    align-items: center;
    justify-content: center;
    position: relative;
    overflow: hidden;
    border: 1px solid var(--border-color-light);
    
    .theme-icon {
      font-size: 18px;
      z-index: 1;
    }
  }
  
  .theme-info {
    flex: 1;
    
    .theme-label {
      font-weight: 500;
      color: var(--text-primary);
      margin-bottom: 2px;
    }
    
    .theme-description {
      font-size: 12px;
      color: var(--text-secondary);
      line-height: 1.4;
    }
  }
  
  .theme-check {
    color: var(--primary-color);
    font-size: 16px;
  }
  
  .theme-reset {
    display: flex;
    align-items: center;
    gap: 8px;
    color: var(--text-secondary);
    
    &:hover {
      color: var(--primary-color);
    }
  }
}

// 响应式适配
@media (max-width: 768px) {
  .theme-dropdown {
    width: 280px;
    
    .theme-item {
      padding: 10px;
      gap: 10px;
    }
    
    .theme-preview {
      width: 36px;
      height: 36px;
      
      .theme-icon {
        font-size: 16px;
      }
    }
    
    .theme-info {
      .theme-label {
        font-size: 14px;
      }
      
      .theme-description {
        font-size: 11px;
      }
    }
  }
}
</style>