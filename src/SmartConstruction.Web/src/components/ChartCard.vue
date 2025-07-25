<template>
  <el-card class="chart-card" :body-style="{ padding: '0' }">
    <template #header>
      <div class="card-header">
        <span>{{ title }}</span>
        <div class="header-actions">
          <slot name="header-actions"></slot>
        </div>
      </div>
    </template>
    
    <div class="chart-content" :class="{ 'is-loading': loading }">
      <div v-if="loading" class="chart-loading">
        <el-skeleton animated :rows="6">
          <template #template>
            <div style="padding: 20px">
              <el-skeleton-item variant="text" style="width: 30%" />
              <div style="margin-top: 20px; height: 200px">
                <el-skeleton-item variant="p" style="height: 100%" />
              </div>
            </div>
          </template>
        </el-skeleton>
      </div>
      
      <div v-else ref="chartRef" class="chart-container" :style="{ height: height }"></div>
    </div>
    
    <div class="chart-footer" v-if="$slots.footer">
      <slot name="footer"></slot>
    </div>
  </el-card>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount, watch } from 'vue'
import * as echarts from 'echarts'
import type { EChartsOption } from 'echarts'

const props = defineProps({
  title: {
    type: String,
    required: true
  },
  options: {
    type: Object as () => EChartsOption,
    required: true
  },
  height: {
    type: String,
    default: '300px'
  },
  loading: {
    type: Boolean,
    default: false
  },
  theme: {
    type: String,
    default: ''
  },
  autoResize: {
    type: Boolean,
    default: true
  }
})

const emit = defineEmits(['chart-ready', 'chart-click', 'chart-legend-select'])

const chartRef = ref<HTMLElement>()
let chart: echarts.ECharts | null = null

// 初始化图表
const initChart = () => {
  if (!chartRef.value) return
  
  // 销毁旧图表
  if (chart) {
    chart.dispose()
  }
  
  // 创建新图表
  chart = echarts.init(chartRef.value, props.theme)
  
  // 设置配置项
  chart.setOption(props.options)
  
  // 绑定事件
  chart.on('click', (params) => {
    emit('chart-click', params)
  })
  
  chart.on('legendselectchanged', (params) => {
    emit('chart-legend-select', params)
  })
  
  // 通知图表已准备好
  emit('chart-ready', chart)
}

// 监听options变化
watch(
  () => props.options,
  (newOptions) => {
    if (chart) {
      chart.setOption(newOptions)
    }
  },
  { deep: true }
)

// 监听theme变化
watch(
  () => props.theme,
  () => {
    initChart()
  }
)

// 监听loading变化
watch(
  () => props.loading,
  (newLoading) => {
    if (!newLoading && chartRef.value && !chart) {
      initChart()
    }
  }
)

// 窗口大小变化时重绘图表
const handleResize = () => {
  if (chart) {
    chart.resize()
  }
}

onMounted(() => {
  if (!props.loading) {
    initChart()
  }
  
  if (props.autoResize) {
    window.addEventListener('resize', handleResize)
  }
})

onBeforeUnmount(() => {
  if (chart) {
    chart.dispose()
    chart = null
  }
  
  if (props.autoResize) {
    window.removeEventListener('resize', handleResize)
  }
})

// 暴露方法
defineExpose({
  chart: () => chart,
  resize: handleResize,
  setOption: (options: EChartsOption) => {
    if (chart) {
      chart.setOption(options)
    }
  }
})
</script>

<style lang="scss" scoped>
.chart-card {
  margin-bottom: 20px;
  
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    .header-actions {
      display: flex;
      gap: 10px;
    }
  }
  
  .chart-content {
    position: relative;
    
    &.is-loading {
      min-height: 200px;
    }
    
    .chart-loading {
      padding: 20px;
    }
    
    .chart-container {
      width: 100%;
    }
  }
  
  .chart-footer {
    padding: 15px 20px;
    border-top: 1px solid var(--el-border-color-lighter);
  }
}
</style>