<template>
  <div class="environment-data-visualizer">
    <div class="visualizer-header">
      <h3 class="title">{{ title }}</h3>
      <div class="header-actions">
        <el-select v-model="selectedTimeRange" placeholder="时间范围" size="small">
          <el-option v-for="item in timeRangeOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
        <el-select v-model="selectedDataType" placeholder="数据类型" size="small">
          <el-option v-for="item in dataTypeOptions" :key="item.value" :label="item.label" :value="item.value" />
        </el-select>
      </div>
    </div>
    
    <div class="visualizer-content">
      <!-- 实时数据仪表盘 -->
      <div class="gauges-container">
        <div class="gauge-item" v-for="(gauge, index) in gaugeData" :key="index">
          <div class="gauge-wrapper">
            <div class="gauge" ref="gaugeRefs"></div>
          </div>
          <div class="gauge-info">
            <div class="gauge-value">{{ gauge.value }}{{ gauge.unit }}</div>
            <div class="gauge-label">{{ gauge.label }}</div>
            <div class="gauge-status" :class="getStatusClass(gauge.status)">
              {{ getStatusText(gauge.status) }}
            </div>
          </div>
        </div>
      </div>
      
      <!-- 趋势图表 -->
      <div class="trend-chart">
        <DigitalChart
          :option="chartOption"
          :loading="chartLoading"
          height="280px"
        />
      </div>
      
      <!-- 告警信息 -->
      <div class="alerts-panel" v-if="alerts.length > 0">
        <div class="panel-header">
          <h4>环境告警 ({{ alerts.length }})</h4>
          <el-button type="primary" size="small" plain @click="handleViewAllAlerts">
            查看全部
          </el-button>
        </div>
        <div class="alerts-list">
          <div class="alert-item" v-for="(alert, index) in alerts.slice(0, 3)" :key="index">
            <div class="alert-icon" :class="getAlertLevelClass(alert.level)">
              <el-icon><Warning /></el-icon>
            </div>
            <div class="alert-content">
              <div class="alert-title">{{ alert.title }}</div>
              <div class="alert-message">{{ alert.message }}</div>
              <div class="alert-meta">
                <span class="alert-location">{{ alert.location }}</span>
                <span class="alert-time">{{ formatTime(alert.time) }}</span>
              </div>
            </div>
            <div class="alert-actions">
              <el-button type="primary" size="small" @click="handleAlertAction(alert)">处理</el-button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue'
import { Warning } from '@element-plus/icons-vue'
import * as echarts from 'echarts'
import DigitalChart from './DigitalChart.vue'

const props = defineProps({
  title: {
    type: String,
    default: '环境监测数据'
  },
  data: {
    type: Object,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['time-range-change', 'data-type-change', 'alert-action', 'view-all-alerts'])

// 状态变量
const selectedTimeRange = ref('today')
const selectedDataType = ref('all')
const chartLoading = ref(false)
const gaugeRefs = ref<HTMLElement[]>([])
const gaugeInstances = ref<echarts.ECharts[]>([])

// 选项
const timeRangeOptions = [
  { label: '今日', value: 'today' },
  { label: '本周', value: 'week' },
  { label: '本月', value: 'month' },
  { label: '季度', value: 'quarter' }
]

const dataTypeOptions = [
  { label: '全部', value: 'all' },
  { label: 'PM2.5', value: 'pm25' },
  { label: 'PM10', value: 'pm10' },
  { label: '噪音', value: 'noise' },
  { label: '温度', value: 'temperature' },
  { label: '湿度', value: 'humidity' }
]

// 仪表盘数据
const gaugeData = computed(() => {
  if (!props.data || !props.data.currentData) return []
  
  const { dustMonitoring, noiseMonitoring, weatherData } = props.data.currentData
  
  return [
    {
      label: 'PM2.5',
      value: dustMonitoring?.pm25 || 0,
      unit: 'μg/m³',
      min: 0,
      max: 150,
      status: getPm25Status(dustMonitoring?.pm25 || 0),
      thresholds: [35, 75, 115]
    },
    {
      label: 'PM10',
      value: dustMonitoring?.pm10 || 0,
      unit: 'μg/m³',
      min: 0,
      max: 250,
      status: getPm10Status(dustMonitoring?.pm10 || 0),
      thresholds: [50, 150, 250]
    },
    {
      label: '噪音',
      value: noiseMonitoring?.level || 0,
      unit: 'dB',
      min: 0,
      max: 120,
      status: getNoiseStatus(noiseMonitoring?.level || 0),
      thresholds: [55, 70, 85]
    },
    {
      label: '温度',
      value: weatherData?.temperature || 0,
      unit: '°C',
      min: -10,
      max: 50,
      status: 'normal',
      thresholds: [0, 30, 40]
    }
  ]
})

// 告警数据
const alerts = computed(() => {
  if (!props.data || !props.data.alerts) return []
  return props.data.alerts
})

// 图表配置
const chartOption = computed(() => {
  if (!props.data || !props.data.trends) return {}
  
  const { dates, pm25, pm10, noise, temperature, humidity } = props.data.trends
  
  // 根据选择的数据类型过滤系列
  let series = []
  
  if (selectedDataType.value === 'all' || selectedDataType.value === 'pm25') {
    series.push({
      name: 'PM2.5',
      type: 'line',
      data: pm25,
      smooth: true,
      lineStyle: { color: '#3498db', width: 2 },
      itemStyle: { color: '#3498db' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(52, 152, 219, 0.3)' },
            { offset: 1, color: 'rgba(52, 152, 219, 0.1)' }
          ]
        }
      }
    })
  }
  
  if (selectedDataType.value === 'all' || selectedDataType.value === 'pm10') {
    series.push({
      name: 'PM10',
      type: 'line',
      data: pm10,
      smooth: true,
      lineStyle: { color: '#e74c3c', width: 2 },
      itemStyle: { color: '#e74c3c' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(231, 76, 60, 0.3)' },
            { offset: 1, color: 'rgba(231, 76, 60, 0.1)' }
          ]
        }
      }
    })
  }
  
  if (selectedDataType.value === 'all' || selectedDataType.value === 'noise') {
    series.push({
      name: '噪音',
      type: 'line',
      data: noise,
      smooth: true,
      lineStyle: { color: '#f39c12', width: 2 },
      itemStyle: { color: '#f39c12' },
      areaStyle: {
        color: {
          type: 'linear',
          x: 0, y: 0, x2: 0, y2: 1,
          colorStops: [
            { offset: 0, color: 'rgba(243, 156, 18, 0.3)' },
            { offset: 1, color: 'rgba(243, 156, 18, 0.1)' }
          ]
        }
      }
    })
  }
  
  if (selectedDataType.value === 'all' || selectedDataType.value === 'temperature') {
    series.push({
      name: '温度',
      type: 'line',
      data: temperature,
      smooth: true,
      lineStyle: { color: '#2ecc71', width: 2 },
      itemStyle: { color: '#2ecc71' }
    })
  }
  
  if (selectedDataType.value === 'all' || selectedDataType.value === 'humidity') {
    series.push({
      name: '湿度',
      type: 'line',
      data: humidity,
      smooth: true,
      lineStyle: { color: '#9b59b6', width: 2 },
      itemStyle: { color: '#9b59b6' }
    })
  }
  
  return {
    tooltip: {
      trigger: 'axis',
      backgroundColor: 'rgba(30, 42, 58, 0.9)',
      borderColor: '#3498db',
      textStyle: { color: '#ffffff' }
    },
    legend: {
      data: series.map(s => s.name),
      textStyle: { color: '#7f8c8d' },
      right: '5%'
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: dates,
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } }
    },
    yAxis: {
      type: 'value',
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } },
      splitLine: { lineStyle: { color: '#34495e', type: 'dashed' } }
    },
    series
  }
})

// 初始化仪表盘
const initGauges = () => {
  // 清除旧的实例
  gaugeInstances.value.forEach(instance => {
    instance.dispose()
  })
  gaugeInstances.value = []
  
  // 创建新的仪表盘
  if (gaugeRefs.value && gaugeRefs.value.length > 0) {
    gaugeData.value.forEach((gauge, index) => {
      if (gaugeRefs.value[index]) {
        const instance = echarts.init(gaugeRefs.value[index])
        
        const option = {
          series: [
            {
              type: 'gauge',
              startAngle: 180,
              endAngle: 0,
              min: gauge.min,
              max: gauge.max,
              radius: '100%',
              splitNumber: 5,
              axisLine: {
                lineStyle: {
                  width: 8,
                  color: [
                    [gauge.thresholds[0] / gauge.max, '#2ecc71'],
                    [gauge.thresholds[1] / gauge.max, '#f39c12'],
                    [gauge.thresholds[2] / gauge.max, '#e74c3c'],
                    [1, '#c0392b']
                  ]
                }
              },
              pointer: {
                icon: 'path://M12.8,0.7l12,40.1H0.7L12.8,0.7z',
                length: '12%',
                width: 6,
                offsetCenter: [0, '-60%'],
                itemStyle: {
                  color: 'auto'
                }
              },
              axisTick: {
                length: 6,
                lineStyle: {
                  color: 'auto',
                  width: 1
                }
              },
              splitLine: {
                length: 12,
                lineStyle: {
                  color: 'auto',
                  width: 2
                }
              },
              axisLabel: {
                color: '#7f8c8d',
                fontSize: 10,
                distance: -30
              },
              title: {
                offsetCenter: [0, '-20%'],
                fontSize: 12,
                color: '#7f8c8d'
              },
              detail: {
                fontSize: 18,
                offsetCenter: [0, '0%'],
                valueAnimation: true,
                formatter: function(value: number) {
                  return value + gauge.unit
                },
                color: 'auto'
              },
              data: [
                {
                  value: gauge.value,
                  name: gauge.label
                }
              ]
            }
          ]
        }
        
        instance.setOption(option)
        gaugeInstances.value.push(instance)
      }
    })
  }
}

// 工具函数
const getPm25Status = (value: number) => {
  if (value > 115) return 'critical'
  if (value > 75) return 'warning'
  if (value > 35) return 'moderate'
  return 'good'
}

const getPm10Status = (value: number) => {
  if (value > 250) return 'critical'
  if (value > 150) return 'warning'
  if (value > 50) return 'moderate'
  return 'good'
}

const getNoiseStatus = (value: number) => {
  if (value > 85) return 'critical'
  if (value > 70) return 'warning'
  if (value > 55) return 'moderate'
  return 'good'
}

const getStatusClass = (status: string) => {
  return `status-${status}`
}

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'good': '优',
    'moderate': '中',
    'warning': '差',
    'critical': '严重',
    'normal': '正常'
  }
  return statusMap[status] || status
}

const getAlertLevelClass = (level: string) => {
  return `alert-level-${level.toLowerCase()}`
}

const formatTime = (time: string) => {
  if (!time) return ''
  
  const date = new Date(time)
  return date.toLocaleString()
}

// 事件处理
const handleAlertAction = (alert: any) => {
  emit('alert-action', alert)
}

const handleViewAllAlerts = () => {
  emit('view-all-alerts')
}

// 监听选择变化
watch(selectedTimeRange, (newValue) => {
  emit('time-range-change', newValue)
})

watch(selectedDataType, (newValue) => {
  emit('data-type-change', newValue)
})

// 监听数据变化
watch(() => props.data, () => {
  initGauges()
}, { deep: true })

// 生命周期钩子
onMounted(() => {
  // 初始化仪表盘
  setTimeout(() => {
    initGauges()
  }, 100)
  
  // 监听窗口大小变化
  window.addEventListener('resize', handleResize)
})

const handleResize = () => {
  gaugeInstances.value.forEach(instance => {
    instance.resize()
  })
}

onUnmounted(() => {
  // 清理资源
  gaugeInstances.value.forEach(instance => {
    instance.dispose()
  })
  
  window.removeEventListener('resize', handleResize)
})
</script>

<style lang="scss" scoped>
.environment-data-visualizer {
  background-color: rgba(26, 26, 46, 0.7);
  border-radius: 8px;
  padding: 16px;
  
  .visualizer-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;
    
    .title {
      margin: 0;
      color: #ffffff;
      font-size: 18px;
      font-weight: 600;
    }
    
    .header-actions {
      display: flex;
      gap: 12px;
    }
  }
  
  .visualizer-content {
    display: flex;
    flex-direction: column;
    gap: 20px;
    
    .gauges-container {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
      
      .gauge-item {
        background-color: rgba(255, 255, 255, 0.05);
        border-radius: 8px;
        padding: 16px;
        display: flex;
        flex-direction: column;
        align-items: center;
        
        .gauge-wrapper {
          width: 100%;
          height: 120px;
          
          .gauge {
            width: 100%;
            height: 100%;
          }
        }
        
        .gauge-info {
          margin-top: 8px;
          text-align: center;
          
          .gauge-value {
            font-size: 20px;
            font-weight: 600;
            color: #ffffff;
          }
          
          .gauge-label {
            font-size: 14px;
            color: #7f8c8d;
            margin: 4px 0;
          }
          
          .gauge-status {
            display: inline-block;
            padding: 2px 8px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            
            &.status-good {
              background-color: rgba(46, 204, 113, 0.2);
              color: #2ecc71;
            }
            
            &.status-moderate {
              background-color: rgba(52, 152, 219, 0.2);
              color: #3498db;
            }
            
            &.status-warning {
              background-color: rgba(243, 156, 18, 0.2);
              color: #f39c12;
            }
            
            &.status-critical {
              background-color: rgba(231, 76, 60, 0.2);
              color: #e74c3c;
            }
            
            &.status-normal {
              background-color: rgba(52, 152, 219, 0.2);
              color: #3498db;
            }
          }
        }
      }
    }
    
    .trend-chart {
      background-color: rgba(255, 255, 255, 0.05);
      border-radius: 8px;
      padding: 16px;
    }
    
    .alerts-panel {
      background-color: rgba(255, 255, 255, 0.05);
      border-radius: 8px;
      padding: 16px;
      
      .panel-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 16px;
        
        h4 {
          margin: 0;
          color: #ffffff;
          font-size: 16px;
        }
      }
      
      .alerts-list {
        display: flex;
        flex-direction: column;
        gap: 12px;
        
        .alert-item {
          display: flex;
          align-items: flex-start;
          gap: 12px;
          padding: 12px;
          border-radius: 8px;
          background-color: rgba(255, 255, 255, 0.05);
          
          .alert-icon {
            width: 32px;
            height: 32px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            
            &.alert-level-critical {
              background-color: rgba(231, 76, 60, 0.2);
              color: #e74c3c;
            }
            
            &.alert-level-warning {
              background-color: rgba(243, 156, 18, 0.2);
              color: #f39c12;
            }
            
            &.alert-level-info {
              background-color: rgba(52, 152, 219, 0.2);
              color: #3498db;
            }
          }
          
          .alert-content {
            flex: 1;
            
            .alert-title {
              font-size: 14px;
              font-weight: 600;
              color: #ffffff;
              margin-bottom: 4px;
            }
            
            .alert-message {
              font-size: 12px;
              color: #bdc3c7;
              margin-bottom: 8px;
            }
            
            .alert-meta {
              display: flex;
              justify-content: space-between;
              font-size: 12px;
              color: #7f8c8d;
            }
          }
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 768px) {
  .environment-data-visualizer {
    .visualizer-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 12px;
      
      .header-actions {
        width: 100%;
      }
    }
    
    .gauges-container {
      grid-template-columns: 1fr 1fr;
    }
  }
}

@media (max-width: 480px) {
  .environment-data-visualizer {
    .gauges-container {
      grid-template-columns: 1fr;
    }
    
    .alert-item {
      flex-direction: column;
      
      .alert-icon {
        margin-bottom: 8px;
      }
    }
  }
}
</style>