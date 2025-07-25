<template>
  <div class="app-footer">
    <div class="footer-content">
      <span class="copyright">
        © 2024 智慧建设巴伯框架 All Rights Reserved
      </span>
      <div class="footer-links">
        <a href="/docs" target="_blank">帮助文档</a>
        <span class="divider">|</span>
        <a href="/privacy" target="_blank">隐私政策</a>
        <span class="divider">|</span>
        <a href="/terms" target="_blank">服务条款</a>
      </div>
      <div class="system-info">
        <span>版本 v{{ version }}</span>
        <span class="divider">|</span>
        <span>{{ buildTime }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

// 系统信息
const version = computed(() => import.meta.env.VITE_APP_VERSION || '1.0.0')
const buildTime = computed(() => {
  const buildDate = import.meta.env.VITE_BUILD_TIME
  return buildDate ? new Date(buildDate).toLocaleDateString() : new Date().toLocaleDateString()
})
</script>

<style lang="scss" scoped>
.app-footer {
  display: flex;
  align-items: center;
  justify-content: center;
  height: var(--footer-height);
  background-color: var(--bg-color-overlay);
  border-top: 1px solid var(--border-color-lighter);
  color: var(--text-color-secondary);
  font-size: var(--font-size-small);
  
  .footer-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    width: 100%;
    max-width: 1200px;
    padding: 0 var(--spacing-large);
    
    .copyright {
      font-weight: var(--font-weight-primary);
    }
    
    .footer-links {
      display: flex;
      align-items: center;
      gap: var(--spacing-small);
      
      a {
        color: var(--text-color-secondary);
        text-decoration: none;
        transition: var(--transition-color);
        
        &:hover {
          color: var(--primary-color);
        }
      }
      
      .divider {
        color: var(--text-color-placeholder);
      }
    }
    
    .system-info {
      display: flex;
      align-items: center;
      gap: var(--spacing-small);
      color: var(--text-color-placeholder);
      
      .divider {
        color: var(--text-color-placeholder);
      }
    }
  }
}

// 移动端适配
@include respond-to(xs) {
  .app-footer {
    .footer-content {
      flex-direction: column;
      gap: var(--spacing-small);
      padding: var(--spacing-small) var(--spacing-medium);
      
      .footer-links,
      .system-info {
        font-size: var(--font-size-extra-small);
      }
    }
  }
}

// 暗色主题适配
[data-theme="dark"] {
  .app-footer {
    .footer-content {
      .footer-links {
        a {
          &:hover {
            color: var(--primary-light);
          }
        }
      }
    }
  }
}
</style>