<template>
  <div 
    class="data-card" 
    :class="[`data-card--${type}`, { 'data-card--bordered': bordered }]"
    :style="cardStyle"
  >
    <!-- 装饰边框 -->
    <div class="card-border" v-if="bordered">
      <div class="corner corner--top-left"></div>
      <div class="corner corner--top-right"></div>
      <div class="corner corner--bottom-left"></div>
      <div class="corner corner--bottom-right"></div>
    </div>
    
    <!-- 卡片头部 -->
    <header class="card-header" v-if="title || $slots.header">
      <slot name="header">
        <div class="header-left">
          <el-icon v-if="icon" class="header-icon" :size="iconSize">
            <component :is="icon" />
          </el-icon>
          <h3 class="card-title">{{ title }}</h3>
          <span v-if="subtitle" class="card-subtitle">{{ subtitle }}</span>
        </div>
        <div class="header-right" v-if="$slots.extra">
          <slot name="extra" />
        </div>
      </slot>
    </header>
    
    <!-- 卡片内容 -->
    <main class="card-body">
      <slot />
    </main>
    
    <!-- 卡片底部 -->
    <footer class="card-footer" v-if="$slots.footer">
      <slot name="footer" />
    </footer>
    
    <!-- 动态效果 -->
    <div class="card-glow" v-if="glow"></div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'

interface Props {
  title?: string
  subtitle?: string
  icon?: string
  iconSize?: number
  type?: 'primary' | 'success' | 'warning' | 'danger' | 'info' | 'dark'
  bordered?: boolean
  glow?: boolean
  width?: string | number
  height?: string | number
}

const props = withDefaults(defineProps<Props>(), {
  type: 'dark',
  iconSize: 20,
  bordered: true,
  glow: false
})

const cardStyle = computed(() => ({
  width: typeof props.width === 'number' ? `${props.width}px` : props.width,
  height: typeof props.height === 'number' ? `${props.height}px` : props.height
}))
</script>

<style lang="scss" scoped>
.data-card {
  background: rgba(30, 42, 58, 0.85);
  border-radius: 8px;
  backdrop-filter: blur(10px);
  position: relative;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  transition: all 0.3s ease;
  
  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  }
  
  &--primary {
    background: rgba(52, 152, 219, 0.1);
    border: 1px solid rgba(52, 152, 219, 0.3);
  }
  
  &--success {
    background: rgba(46, 204, 113, 0.1);
    border: 1px solid rgba(46, 204, 113, 0.3);
  }
  
  &--warning {
    background: rgba(243, 156, 18, 0.1);
    border: 1px solid rgba(243, 156, 18, 0.3);
  }
  
  &--danger {
    background: rgba(231, 76, 60, 0.1);
    border: 1px solid rgba(231, 76, 60, 0.3);
  }
  
  &--info {
    background: rgba(155, 89, 182, 0.1);
    border: 1px solid rgba(155, 89, 182, 0.3);
  }
  
  &--dark {
    background: rgba(44, 62, 80, 0.8);
    border: 1px solid rgba(127, 140, 141, 0.2);
  }
}

// 装饰边框
.card-border {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  pointer-events: none;
  z-index: 1;
}

.corner {
  position: absolute;
  width: 15px;
  height: 15px;
  
  &--top-left {
    top: -1px;
    left: -1px;
    border-top: 2px solid #3498db;
    border-left: 2px solid #3498db;
  }
  
  &--top-right {
    top: -1px;
    right: -1px;
    border-top: 2px solid #3498db;
    border-right: 2px solid #3498db;
  }
  
  &--bottom-left {
    bottom: -1px;
    left: -1px;
    border-bottom: 2px solid #3498db;
    border-left: 2px solid #3498db;
  }
  
  &--bottom-right {
    bottom: -1px;
    right: -1px;
    border-bottom: 2px solid #3498db;
    border-right: 2px solid #3498db;
  }
}

// 卡片头部
.card-header {
  padding: 16px 20px;
  border-bottom: 1px solid rgba(127, 140, 141, 0.1);
  display: flex;
  align-items: center;
  justify-content: space-between;
  position: relative;
  z-index: 2;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 10px;
}

.header-icon {
  color: #3498db;
}

.card-title {
  margin: 0;
  font-size: 16px;
  font-weight: 600;
  color: #ffffff;
}

.card-subtitle {
  font-size: 12px;
  color: #7f8c8d;
  margin-left: 8px;
}

// 卡片内容
.card-body {
  flex: 1;
  padding: 20px;
  position: relative;
  z-index: 2;
  color: #ffffff;
}

// 卡片底部
.card-footer {
  padding: 12px 20px;
  border-top: 1px solid rgba(127, 140, 141, 0.1);
  position: relative;
  z-index: 2;
}

// 发光效果
.card-glow {
  position: absolute;
  top: -2px;
  left: -2px;
  right: -2px;
  bottom: -2px;
  background: linear-gradient(45deg, #3498db, #2ecc71, #f39c12, #e74c3c);
  border-radius: 10px;
  opacity: 0.3;
  z-index: 0;
  animation: rotate 4s linear infinite;
}

@keyframes rotate {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

// 响应式设计
@media (max-width: 768px) {
  .card-header {
    padding: 12px 16px;
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
  }
  
  .card-body {
    padding: 16px;
  }
  
  .card-footer {
    padding: 10px 16px;
  }
}
</style>