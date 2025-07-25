<template>
  <div class="command-center">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>数字孪生指挥中心</span>
          <el-button type="primary" @click="refreshData">刷新数据</el-button>
        </div>
      </template>

      <el-row :gutter="20">
        <!-- 实时监控面板 -->
        <el-col :span="16">
          <el-card class="monitor-panel">
            <template #header>
              <span>实时监控面板</span>
            </template>
            <div class="monitor-grid">
              <div class="monitor-item">
                <div class="monitor-title">设备状态</div>
                <div class="monitor-value">
                  <el-tag type="success">{{ deviceStatus.online }}</el-tag>
                  <span class="monitor-label">在线</span>
                </div>
                <div class="monitor-value">
                  <el-tag type="danger">{{ deviceStatus.offline }}</el-tag>
                  <span class="monitor-label">离线</span>
                </div>
              </div>
              
              <div class="monitor-item">
                <div class="monitor-title">环境指标</div>
                <div class="monitor-value">
                  <span class="value">{{ environment.temperature }}°C</span>
                  <span class="monitor-label">温度</span>
                </div>
                <div class="monitor-value">
                  <span class="value">{{ environment.humidity }}%</span>
                  <span class="monitor-label">湿度</span>
                </div>
              </div>
              
              <div class="monitor-item">
                <div class="monitor-title">安全状态</div>
                <div class="monitor-value">
                  <el-tag type="success">{{ safetyStatus.normal }}</el-tag>
                  <span class="monitor-label">正常</span>
                </div>
                <div class="monitor-value">
                  <el-tag type="warning">{{ safetyStatus.warning }}</el-tag>
                  <span class="monitor-label">警告</span>
                </div>
              </div>
            </div>
          </el-card>
        </el-col>

        <!-- 控制面板 -->
        <el-col :span="8">
          <el-card class="control-panel">
            <template #header>
              <span>控制面板</span>
            </template>
            <div class="control-buttons">
              <el-button type="primary" @click="emergencyStop" :loading="emergencyLoading">
                紧急停止
              </el-button>
              <el-button type="success" @click="startSystem" :loading="startLoading">
                启动系统
              </el-button>
              <el-button type="warning" @click="pauseSystem" :loading="pauseLoading">
                暂停系统
              </el-button>
            </div>
            
            <el-divider />
            
            <div class="system-status">
              <h4>系统状态</h4>
              <div class="status-item">
                <span class="label">主系统：</span>
                <el-tag :type="systemStatus.main === 'running' ? 'success' : 'danger'">
                  {{ systemStatus.main === 'running' ? '运行中' : '已停止' }}
                </el-tag>
              </div>
              <div class="status-item">
                <span class="label">数据采集：</span>
                <el-tag :type="systemStatus.dataCollection === 'running' ? 'success' : 'danger'">
                  {{ systemStatus.dataCollection === 'running' ? '运行中' : '已停止' }}
                </el-tag>
              </div>
              <div class="status-item">
                <span class="label">AI分析：</span>
                <el-tag :type="systemStatus.aiAnalysis === 'running' ? 'success' : 'danger'">
                  {{ systemStatus.aiAnalysis === 'running' ? '运行中' : '已停止' }}
                </el-tag>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- 数据可视化 -->
      <el-card style="margin-top: 20px;">
        <template #header>
          <span>数据可视化</span>
        </template>
        <div class="chart-container">
          <div ref="dataChart" class="chart"></div>
        </div>
      </el-card>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import * as echarts from 'echarts'

// 响应式数据
const deviceStatus = ref({
  online: 156,
  offline: 12
})

const environment = ref({
  temperature: 24.5,
  humidity: 65.2
})

const safetyStatus = ref({
  normal: 145,
  warning: 3
})

const systemStatus = ref({
  main: 'running',
  dataCollection: 'running',
  aiAnalysis: 'running'
})

// 加载状态
const emergencyLoading = ref(false)
const startLoading = ref(false)
const pauseLoading = ref(false)

// 图表实例
let dataChart: echarts.ECharts | null = null
let timer: NodeJS.Timeout | null = null

// 刷新数据
const refreshData = () => {
  // 模拟数据更新
  deviceStatus.value.online = Math.floor(Math.random() * 200) + 100
  deviceStatus.value.offline = Math.floor(Math.random() * 20) + 5
  environment.value.temperature = Math.random() * 10 + 20
  environment.value.humidity = Math.random() * 30 + 50
  safetyStatus.value.normal = Math.floor(Math.random() * 50) + 120
  safetyStatus.value.warning = Math.floor(Math.random() * 10) + 1
  
  updateChart()
}

// 紧急停止
const emergencyStop = async () => {
  emergencyLoading.value = true
  try {
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 1000))
    systemStatus.value.main = 'stopped'
    systemStatus.value.dataCollection = 'stopped'
    systemStatus.value.aiAnalysis = 'stopped'
    ElMessage.success('系统已紧急停止')
  } catch (error) {
    ElMessage.error('紧急停止失败')
  } finally {
    emergencyLoading.value = false
  }
}

// 启动系统
const startSystem = async () => {
  startLoading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 1000))
    systemStatus.value.main = 'running'
    systemStatus.value.dataCollection = 'running'
    systemStatus.value.aiAnalysis = 'running'
    ElMessage.success('系统已启动')
  } catch (error) {
    ElMessage.error('系统启动失败')
  } finally {
    startLoading.value = false
  }
}

// 暂停系统
const pauseSystem = async () => {
  pauseLoading.value = true
  try {
    await new Promise(resolve => setTimeout(resolve, 1000))
    systemStatus.value.dataCollection = 'stopped'
    ElMessage.warning('数据采集已暂停')
  } catch (error) {
    ElMessage.error('系统暂停失败')
  } finally {
    pauseLoading.value = false
  }
}

// 初始化图表
const initChart = () => {
  const chartDom = document.querySelector('.chart') as HTMLElement
  if (!chartDom) return
  
  dataChart = echarts.init(chartDom)
  const option = {
    title: {
      text: '实时数据趋势',
      left: 'center'
    },
    tooltip: {
      trigger: 'axis'
    },
    legend: {
      data: ['设备在线数', '温度', '湿度'],
      top: 30
    },
    xAxis: {
      type: 'category',
      data: Array.from({ length: 20 }, (_, i) => i)
    },
    yAxis: [
      {
        type: 'value',
        name: '设备数量',
        position: 'left'
      },
      {
        type: 'value',
        name: '环境指标',
        position: 'right'
      }
    ],
    series: [
      {
        name: '设备在线数',
        type: 'line',
        data: Array.from({ length: 20 }, () => Math.floor(Math.random() * 200) + 100)
      },
      {
        name: '温度',
        type: 'line',
        yAxisIndex: 1,
        data: Array.from({ length: 20 }, () => Math.random() * 10 + 20)
      },
      {
        name: '湿度',
        type: 'line',
        yAxisIndex: 1,
        data: Array.from({ length: 20 }, () => Math.random() * 30 + 50)
      }
    ]
  }
  dataChart.setOption(option)
}

// 更新图表
const updateChart = () => {
  if (!dataChart) return
  
  const option = dataChart.getOption()
  const series = option.series as any[]
  
  // 更新数据
  series[0].data.shift()
  series[0].data.push(deviceStatus.value.online)
  
  series[1].data.shift()
  series[1].data.push(environment.value.temperature)
  
  series[2].data.shift()
  series[2].data.push(environment.value.humidity)
  
  dataChart.setOption({
    series: series
  })
}

// 组件挂载
onMounted(() => {
  initChart()
  
  // 定时更新数据
  timer = setInterval(() => {
    refreshData()
  }, 5000)
})

// 组件卸载
onUnmounted(() => {
  if (timer) {
    clearInterval(timer)
  }
  if (dataChart) {
    dataChart.dispose()
  }
})
</script>

<style lang="scss" scoped>
.command-center {
  padding: 20px;
  
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .monitor-panel {
    .monitor-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 20px;
      
      .monitor-item {
        text-align: center;
        padding: 15px;
        border: 1px solid var(--el-border-color-light);
        border-radius: 8px;
        
        .monitor-title {
          font-weight: 600;
          margin-bottom: 10px;
          color: var(--el-text-color-primary);
        }
        
        .monitor-value {
          display: flex;
          flex-direction: column;
          align-items: center;
          margin-bottom: 8px;
          
          .value {
            font-size: 18px;
            font-weight: 600;
            color: var(--el-color-primary);
          }
          
          .monitor-label {
            font-size: 12px;
            color: var(--el-text-color-secondary);
            margin-top: 4px;
          }
        }
      }
    }
  }
  
  .control-panel {
    .control-buttons {
      display: flex;
      flex-direction: column;
      gap: 10px;
      
      .el-button {
        width: 100%;
      }
    }
    
    .system-status {
      h4 {
        margin: 0 0 15px 0;
        color: var(--el-text-color-primary);
      }
      
      .status-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 10px;
        
        .label {
          color: var(--el-text-color-regular);
        }
      }
    }
  }
  
  .chart-container {
    .chart {
      height: 400px;
    }
  }
}
</style> 