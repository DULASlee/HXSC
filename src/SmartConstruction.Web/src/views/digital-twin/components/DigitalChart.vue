<template>
  <div 
    class="digital-chart" 
    :style="{ width: width, height: height }"
    ref="chartContainer"
  >
    <div class="chart-loading" v-if="loading">
      <el-icon class="loading-icon"><Loading /></el-icon>
      <span>加载中...</span>
    </div>
    <div 
      class="chart-instance" 
      ref="chartRef"
      :style="{ width: '100%', height: '100%' }"
    ></div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch, nextTick } from 'vue'
import * as echarts from 'echarts'
import { Loading } from '@element-plus/icons-vue'

interface Props {
  option: any
  loading?: boolean
  width?: string
  height?: string
  autoResize?: boolean
  theme?: 'dark' | 'light'
}

const props = withDefaults(defineProps<Props>(), {
  loading: false,
  width: '100%',
  height: '300px',
  autoResize: true,
  theme: 'dark'
})

const emit = defineEmits<{
  chartReady: [chart: echarts.EChartsType]
  chartClick: [params: any]
}>()

const chartRef = ref<HTMLDivElement>()
const chartContainer = ref<HTMLDivElement>()
let chartInstance: echarts.EChartsType | null = null

// 注册自定义主题
const registerTheme = () => {
  if (!echarts.getMap('digitalTwin')) {
    echarts.registerTheme('digitalTwin', {
      backgroundColor: 'transparent',
      textStyle: {
        color: '#ffffff',
        fontFamily: 'PingFang SC, Helvetica Neue, Arial, sans-serif'
      },
      title: {
        textStyle: {
          color: '#ffffff'
        }
      },
      tooltip: {
        backgroundColor: 'rgba(30, 42, 58, 0.9)',
        borderColor: '#3498db',
        textStyle: {
          color: '#ffffff'
        }
      },
      legend: {
        textStyle: {
          color: '#7f8c8d'
        }
      },
      categoryAxis: {
        axisLine: {
          lineStyle: {
            color: '#34495e'
          }
        },
        axisTick: {
          lineStyle: {
            color: '#34495e'
          }
        },
        axisLabel: {
          color: '#7f8c8d'
        },
        splitLine: {
          lineStyle: {
            color: '#34495e',
            type: 'dashed'
          }
        }
      },
      valueAxis: {
        axisLine: {
          lineStyle: {
            color: '#34495e'
          }
        },
        axisTick: {
          lineStyle: {
            color: '#34495e'
          }
        },
        axisLabel: {
          color: '#7f8c8d'
        },
        splitLine: {
          lineStyle: {
            color: '#34495e',
            type: 'dashed'
          }
        }
      },
      grid: {
        borderColor: '#34495e'
      },
      color: [
        '#3498db', '#2ecc71', '#f39c12', '#e74c3c', 
        '#9b59b6', '#1abc9c', '#34495e', '#f1c40f',
        '#e67e22', '#95a5a6', '#16a085', '#27ae60'
      ]
    })
  }
}

// 初始化图表
const initChart = () => {
  if (!chartRef.value) return
  
  registerTheme()
  
  chartInstance = echarts.init(chartRef.value, 'digitalTwin')
  
  // 绑定事件
  chartInstance.on('click', (params) => {
    emit('chartClick', params)
  })
  
  emit('chartReady', chartInstance)
  
  updateChart()
}

// 更新图表
const updateChart = () => {
  if (!chartInstance || !props.option) return
  
  try {
    chartInstance.setOption(props.option, true)
  } catch (error) {
    console.error('Chart update error:', error)
  }
}

// 调整图表尺寸
const resizeChart = () => {
  if (chartInstance) {
    chartInstance.resize()
  }
}

// 销毁图表
const destroyChart = () => {
  if (chartInstance) {
    chartInstance.dispose()
    chartInstance = null
  }
}

// 监听选项变化
watch(() => props.option, () => {
  updateChart()
}, { deep: true })

// 监听加载状态
watch(() => props.loading, (loading) => {
  if (chartInstance) {
    if (loading) {
      chartInstance.showLoading({
        text: '加载中...',
        color: '#3498db',
        textColor: '#ffffff',
        maskColor: 'rgba(44, 62, 80, 0.8)'
      })
    } else {
      chartInstance.hideLoading()
    }
  }
})

onMounted(async () => {
  await nextTick()
  initChart()
  
  if (props.autoResize) {
    window.addEventListener('resize', resizeChart)
  }
})

onUnmounted(() => {
  if (props.autoResize) {
    window.removeEventListener('resize', resizeChart)
  }
  destroyChart()
})

// 暴露方法
defineExpose({
  getInstance: () => chartInstance,
  resize: resizeChart,
  refresh: updateChart
})
</script>

<style lang="scss" scoped>
.digital-chart {
  position: relative;
  overflow: hidden;
}

.chart-loading {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  color: #7f8c8d;
  font-size: 14px;
  z-index: 10;
  
  .loading-icon {
    font-size: 24px;
    color: #3498db;
    animation: spin 1s linear infinite;
  }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.chart-instance {
  transition: opacity 0.3s ease;
}
</style>