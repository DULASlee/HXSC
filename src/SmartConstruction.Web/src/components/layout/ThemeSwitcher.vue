<template>
  <el-dropdown 
    class="theme-switcher"
    trigger="click"
    @command="handleThemeChange"
  >
    <el-button text class="theme-button">
      <el-icon :size="18">
        <component :is="currentThemeIcon" />
      </el-icon>
    </el-button>
    
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item
          v-for="theme in themes"
          :key="theme.value"
          :command="theme.value"
          :class="{ 'is-active': currentTheme === theme.value }"
        >
          <div class="theme-option">
            <el-icon class="theme-icon">
              <component :is="theme.icon" />
            </el-icon>
            <span class="theme-label">{{ theme.label }}</span>
            <el-icon v-if="currentTheme === theme.value" class="check-icon">
              <Check />
            </el-icon>
          </div>
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { Sunny, Moon, OfficeBuilding, Check } from '@element-plus/icons-vue';
import { useI18n } from 'vue-i18n';
import { useTheme } from '@/composables/useTheme';

const { t } = useI18n();
const { currentTheme, setTheme } = useTheme();

// 主题选项
const themes = [
  {
    value: 'light',
    label: t('theme.light', '亮色主题'),
    icon: Sunny
  },
  {
    value: 'dark',
    label: t('theme.dark', '暗色主题'),
    icon: Moon
  },
  {
    value: 'business-blue',
    label: t('theme.businessBlue', '商务蓝主题'),
    icon: OfficeBuilding
  }
];

// 当前主题图标
const currentThemeIcon = computed(() => {
  const theme = themes.find(t => t.value === currentTheme.value);
  return theme ? theme.icon : Sunny;
});

// 处理主题切换
const handleThemeChange = (themeValue: string) => {
  setTheme(themeValue);
};
</script>

<style lang="scss" scoped>
.theme-switcher {
  .theme-button {
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
  }
}

:deep(.el-dropdown-menu) {
  .theme-option {
    display: flex;
    align-items: center;
    width: 100%;
    padding: 0;
    
    .theme-icon {
      margin-right: var(--spacing-small);
      font-size: var(--font-size-base);
      color: var(--text-color-secondary);
    }
    
    .theme-label {
      flex: 1;
      font-size: var(--font-size-small);
    }
    
    .check-icon {
      font-size: var(--font-size-small);
      color: var(--primary-color);
    }
  }
  
  .el-dropdown-menu__item {
    &.is-active {
      background-color: var(--primary-color-light-9, #ecf5ff);
      color: var(--primary-color);
      
      .theme-icon {
        color: var(--primary-color);
      }
    }
    
    &:hover {
      background-color: var(--fill-color-light);
    }
  }
}

// 主题切换动画
.theme-switcher {
  .theme-button {
    position: relative;
    overflow: hidden;
    
    &::before {
      content: '';
      position: absolute;
      top: 50%;
      left: 50%;
      width: 0;
      height: 0;
      background-color: var(--primary-color);
      border-radius: 50%;
      transform: translate(-50%, -50%);
      transition: all 0.3s ease;
      opacity: 0.1;
    }
    
    &:active::before {
      width: 100px;
      height: 100px;
    }
  }
}
</style>