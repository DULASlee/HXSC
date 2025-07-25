<template>
  <div class="statistic-card" :class="[`statistic-card--${type}`, { 'card-animated': animated }]">
    <!-- 背景装饰 -->
    <div class="card-background">
      <div class="bg-pattern"></div>
      <div class="bg-glow"></div>
    </div>
    
    <!-- 内容区域 -->
    <div class="card-content">
      <!-- 图标区域 -->
      <div class="icon-section" v-if="icon">
        <div class="icon-wrapper">
          <el-icon :size="iconSize" class="stat-icon">
            <component :is="icon" />
          </el-icon>
        </div>
      </div>
      
      <!-- 数据区域 -->
      <div class="data-section">
        <div class="value-container">
          <span class="stat-value" :class="{ 'value-animated': animated }">
            {{ formattedValue }}
          </span>
          <span v-if="suffix" class="stat-suffix">{{ suffix }}</span>
        </div>
        
        <div class="label-container">
          <h4 class="stat-label">{{ label }}</h4>
          <p v-if="description" class="stat-description">{{ description }}</p>
        </div>
        
        <!-- 趋势指示器 -->
        <div class="trend-container" v-if="trend !== undefined">
          <div class="trend-indicator" :class="`trend--${trendType}`">
            <el-icon :size="12">
              <component :is="trendIcon" />
            </el-icon>
            <span class="trend-value">{{ Math.abs(trend) }}%</span>
          </div>
        </div>
      </div>
    </div>
    
    <!-- 进度条 -->
    <div class="progress-bar" v-if="showProgress && percentage !== undefined">
      <div 
        class="progress-fill" 
        :style="{ 
          width: `${Math.min(100, Math.max(0, percentage))}%`,
          background: progressColor
        }"
      ></div>
    </div>
    
    <!-- 状态点 -->
    <div class="status-dot" :class="`status--${status}`" v-if="status"></div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { ArrowUp, ArrowDown, Minus } from '@element-plus/icons-vue'

interface Props {
  value: number | string
  label: string
  description?: string
  icon?: string
  iconSize?: number
  suffix?: string
  type?: 'primary' | 'success' | 'warning' | 'danger' | 'info'
  animated?: boolean
  trend?: number // 正数表示上升，负数表示下降
  showProgress?: boolean
  percentage?: number
  status?: 'online' | 'offline' | 'warning' | 'normal'
}

const props = withDefaults(defineProps<Props>(), {
  iconSize: 32,
  type: 'primary',
  animated: true,
  showProgress: false
})

// 格式化数值
const formattedValue = computed(() => {
  if (typeof props.value === 'number') {
    if (props.value >= 10000) {
      return (props.value / 10000).toFixed(1) + '万'
    } else if (props.value >= 1000) {
      return (props.value / 1000).toFixed(1) + 'k'
    } else {
      return props.value.toLocaleString()
    }
  }
  return props.value
})

// 趋势类型
const trendType = computed(() => {
  if (props.trend === undefined) return 'none'
  if (props.trend > 0) return 'up'
  if (props.trend < 0) return 'down'
  return 'stable'
})

// 趋势图标
const trendIcon = computed(() => {
  switch (trendType.value) {
    case 'up': return ArrowUp
    case 'down': return ArrowDown
    default: return Minus
  }
})

// 进度条颜色
const progressColor = computed(() => {
  const colors = {
    primary: 'linear-gradient(90deg, #3498db, #5dade2)',
    success: 'linear-gradient(90deg, #2ecc71, #58d68d)',
    warning: 'linear-gradient(90deg, #f39c12, #f8c471)',
    danger: 'linear-gradient(90deg, #e74c3c, #ec7063)',
    info: 'linear-gradient(90deg, #9b59b6, #bb8fce)'
  }
  return colors[props.type]
})
</script>

<style lang="scss" scoped>
.statistic-card {
  position: relative;
  padding: 20px;
  border-radius: 12px;
  overflow: hidden;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
  cursor: pointer;
  
  &:hover {
    transform: translateY(-3px) scale(1.02);
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  }
  
  &--primary {
    background: linear-gradient(135deg, rgba(52, 152, 219, 0.15) 0%, rgba(41, 128, 185, 0.1) 100%);
    border: 1px solid rgba(52, 152, 219, 0.3);
  }
  
  &--success {
    background: linear-gradient(135deg, rgba(46, 204, 113, 0.15) 0%, rgba(39, 174, 96, 0.1) 100%);
    border: 1px solid rgba(46, 204, 113, 0.3);
  }
  
  &--warning {
    background: linear-gradient(135deg, rgba(243, 156, 18, 0.15) 0%, rgba(211, 84, 0, 0.1) 100%);
    border: 1px solid rgba(243, 156, 18, 0.3);
  }
  
  &--danger {
    background: linear-gradient(135deg, rgba(231, 76, 60, 0.15) 0%, rgba(192, 57, 43, 0.1) 100%);
    border: 1px solid rgba(231, 76, 60, 0.3);
  }
  
  &--info {
    background: linear-gradient(135deg, rgba(155, 89, 182, 0.15) 0%, rgba(142, 68, 173, 0.1) 100%);
    border: 1px solid rgba(155, 89, 182, 0.3);
  }
}

// 背景装饰
.card-background {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  opacity: 0.1;
}

.bg-pattern {
  position: absolute;
  top: -50%;
  right: -50%;
  width: 100%;
  height: 100%;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1) 1px, transparent 1px);
  background-size: 20px 20px;
  animation: pattern-move 20s linear infinite;
}

.bg-glow {
  position: absolute;
  top: -10px;
  right: -10px;
  width: 60px;
  height: 60px;
  background: radial-gradient(circle, rgba(255, 255, 255, 0.1) 0%, transparent 70%);
  border-radius: 50%;
  animation: glow-pulse 3s ease-in-out infinite;
}

@keyframes pattern-move {
  0% { transform: translateX(0) translateY(0); }
  100% { transform: translateX(20px) translateY(20px); }
}

@keyframes glow-pulse {
  0%, 100% { opacity: 0.3; transform: scale(1); }
  50% { opacity: 0.6; transform: scale(1.1); }
}

// 内容区域
.card-content {
  position: relative;
  z-index: 2;
  display: flex;
  align-items: flex-start;
  gap: 16px;
}

// 图标区域
.icon-section {
  flex-shrink: 0;
}

.icon-wrapper {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
}

.stat-icon {
  color: #ffffff;
}

// 数据区域
.data-section {
  flex: 1;
}

.value-container {
  display: flex;
  align-items: baseline;
  gap: 4px;
  margin-bottom: 8px;
}

.stat-value {
  font-size: 2.2rem;
  font-weight: 700;
  color: #ffffff;
  line-height: 1;
  
  &.value-animated {
    animation: value-slide-in 0.8s ease-out;
  }
}

.stat-suffix {
  font-size: 1rem;
  color: #bdc3c7;
  font-weight: 500;
}

.label-container {
  margin-bottom: 8px;
}

.stat-label {
  font-size: 1rem;
  font-weight: 600;
  color: #ecf0f1;
  margin: 0 0 4px 0;
}

.stat-description {
  font-size: 0.75rem;
  color: #7f8c8d;
  margin: 0;
  line-height: 1.2;
}

// 趋势指示器
.trend-container {
  display: flex;
  align-items: center;
}

.trend-indicator {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 0.75rem;
  font-weight: 600;
  padding: 2px 8px;
  border-radius: 12px;
  backdrop-filter: blur(5px);
  
  &.trend--up {
    color: #2ecc71;
    background: rgba(46, 204, 113, 0.2);
  }
  
  &.trend--down {
    color: #e74c3c;
    background: rgba(231, 76, 60, 0.2);
  }
  
  &.trend--stable {
    color: #95a5a6;
    background: rgba(149, 165, 166, 0.2);
  }
}

// 进度条
.progress-bar {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: rgba(255, 255, 255, 0.1);
}

.progress-fill {
  height: 100%;
  transition: width 1s ease-out;
  border-radius: 0 2px 2px 0;
}

// 状态点
.status-dot {
  position: absolute;
  top: 16px;
  right: 16px;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  
  &.status--online {
    background: #2ecc71;
    box-shadow: 0 0 8px rgba(46, 204, 113, 0.6);
  }
  
  &.status--offline {
    background: #e74c3c;
    box-shadow: 0 0 8px rgba(231, 76, 60, 0.6);
  }
  
  &.status--warning {
    background: #f39c12;
    box-shadow: 0 0 8px rgba(243, 156, 18, 0.6);
  }
  
  &.status--normal {
    background: #3498db;
    box-shadow: 0 0 8px rgba(52, 152, 219, 0.6);
  }
}

@keyframes value-slide-in {
  0% {
    opacity: 0;
    transform: translateY(20px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

// 动画效果
.card-animated {
  animation: card-entrance 0.6s ease-out;
}

@keyframes card-entrance {
  0% {
    opacity: 0;
    transform: translateY(30px) scale(0.95);
  }
  100% {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

// 响应式设计
@media (max-width: 768px) {
  .statistic-card {
    padding: 16px;
  }
  
  .card-content {
    gap: 12px;
  }
  
  .icon-wrapper {
    width: 48px;
    height: 48px;
  }
  
  .stat-value {
    font-size: 1.8rem;
  }
}
</style>