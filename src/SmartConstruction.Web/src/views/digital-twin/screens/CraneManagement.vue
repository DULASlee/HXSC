<template>
  <div class="crane-management-screen">
    <!-- 顶部设备统计 -->
    <div class="stats-section">
      <div class="stats-grid">
        <StatisticCard
          :value="deviceStatistics.deviceSummary?.totalDevices || 0"
          label="设备总数"
          description="塔吊升降机总数"
          icon="Monitor"
          type="primary"
          status="normal"
        />
        <StatisticCard
          :value="deviceStatistics.deviceSummary?.runningDevices || 0"
          label="运行设备"
          description="正在运行的设备"
          icon="Warning"
          type="success"
          :trend="2.3"
          status="online"
        />
        <StatisticCard
          :value="deviceStatistics.deviceSummary?.faultDevices || 0"
          label="故障设备"
          description="需要维修的设备"
          icon="Warning"
          type="danger"
          :trend="-1.2"
          status="offline"
        />
        <StatisticCard
          :value="deviceStatistics.deviceSummary?.utilizationRate || 0"
          label="利用率"
          description="设备综合利用率"
          icon="PieChart"
          type="info"
          suffix="%"
          :show-progress="true"
          :percentage="deviceStatistics.deviceSummary?.utilizationRate || 0"
          :trend="1.8"
          status="normal"
        />
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <div class="content-grid">
        <!-- 设备实时状态 -->
        <div class="devices-section">
          <DataCard title="设备实时状态" class="devices-card">
            <template #actions>
              <el-button size="small" @click="refreshDevices">
                <el-icon><Refresh /></el-icon>
                刷新
              </el-button>
            </template>
            <div class="devices-grid">
              <div 
                class="device-item"
                v-for="device in craneElevatorDevices"
                :key="device.id"
                @click="selectDevice(device)"
                :class="{ 
                  active: selectedDevice?.id === device.id,
                  [`device--${device.status.toLowerCase()}`]: true
                }"
              >
                <div class="device-header">
                  <div class="device-icon">
                    <el-icon>
                      <component :is="getDeviceIcon(device.type)" />
                    </el-icon>
                  </div>
                  <div class="device-info">
                    <div class="device-name">{{ device.name }}</div>
                    <div class="device-model">{{ device.model }}</div>
                  </div>
                  <div class="device-status" :class="`status--${device.status.toLowerCase()}`">
                    {{ getStatusText(device.status) }}
                  </div>
                </div>
                
                <div class="device-metrics">
                  <div class="metric-row">
                    <div class="metric-item">
                      <span class="metric-label">载重</span>
                      <span class="metric-value">
                        {{ device.realTimeData?.load || 0 }}kg
                      </span>
                      <div class="metric-bar">
                        <div 
                          class="metric-fill"
                          :style="{ 
                            width: `${getLoadPercentage(device)}%`,
                            background: getLoadColor(device)
                          }"
                        ></div>
                      </div>
                    </div>
                    
                    <div class="metric-item">
                      <span class="metric-label">高度</span>
                      <span class="metric-value">
                        {{ device.realTimeData?.height || 0 }}m
                      </span>
                      <div class="metric-bar">
                        <div 
                          class="metric-fill"
                          :style="{ 
                            width: `${getHeightPercentage(device)}%`,
                            background: '#3498db'
                          }"
                        ></div>
                      </div>
                    </div>
                  </div>
                  
                  <div class="safety-score">
                    <span class="score-label">安全评分</span>
                    <span class="score-value" :class="getScoreClass(device.safetyStatus?.safetyScore)">
                      {{ device.safetyStatus?.safetyScore || 0 }}分
                    </span>
                  </div>
                </div>
              </div>
            </div>
          </DataCard>
        </div>

        <!-- 右侧信息面板 -->
        <div class="info-panel">
          <!-- 选中设备详情 -->
          <DataCard 
            :title="`${selectedDevice?.name || '请选择设备'} 详情`" 
            class="device-detail-card"
            v-if="selectedDevice"
          >
            <div class="device-detail">
              <div class="detail-section">
                <h4 class="section-title">基本信息</h4>
                <div class="detail-grid">
                  <div class="detail-item">
                    <span class="detail-label">设备型号</span>
                    <span class="detail-value">{{ selectedDevice.model }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-label">所在区域</span>
                    <span class="detail-value">{{ selectedDevice.location?.zone }}</span>
                  </div>
                  <div class="detail-item">
                    <span class="detail-label">工作时长</span>
                    <span class="detail-value">{{ selectedDevice.realTimeData?.workingHours }}h</span>
                  </div>
                </div>
              </div>

              <div class="detail-section">
                <h4 class="section-title">实时数据</h4>
                <div class="realtime-grid">
                  <div class="realtime-item">
                    <div class="realtime-icon">
                      <el-icon><Platform /></el-icon>
                    </div>
                    <div class="realtime-content">
                      <div class="realtime-label">当前载重</div>
                      <div class="realtime-value">
                        {{ selectedDevice.realTimeData?.load }}kg
                        <span class="realtime-max">/ {{ selectedDevice.realTimeData?.maxLoad }}kg</span>
                      </div>
                    </div>
                  </div>
                  
                  <div class="realtime-item">
                    <div class="realtime-icon">
                      <el-icon><Top /></el-icon>
                    </div>
                    <div class="realtime-content">
                      <div class="realtime-label">当前高度</div>
                      <div class="realtime-value">
                        {{ selectedDevice.realTimeData?.height }}m
                        <span class="realtime-max">/ {{ selectedDevice.realTimeData?.maxHeight }}m</span>
                      </div>
                    </div>
                  </div>
                  
                  <div class="realtime-item">
                    <div class="realtime-icon">
                      <el-icon><Monitor /></el-icon>
                    </div>
                    <div class="realtime-content">
                      <div class="realtime-label">风速</div>
                      <div class="realtime-value">
                        {{ selectedDevice.realTimeData?.windSpeed }}m/s
                        <span class="realtime-max">/ {{ selectedDevice.realTimeData?.maxWindSpeed }}m/s</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <div class="detail-section">
                <h4 class="section-title">安全状态</h4>
                <div class="safety-indicators">
                  <div 
                    class="safety-item"
                    v-for="(value, key) in selectedDevice.safetyStatus"
                    :key="key"
                    v-if="key !== 'safetyScore'"
                  >
                    <div class="safety-icon" :class="{ danger: value }">
                      <el-icon><component :is="getSafetyIcon(key)" /></el-icon>
                    </div>
                    <div class="safety-label">{{ getSafetyLabel(key) }}</div>
                    <div class="safety-status" :class="{ danger: value }">
                      {{ value ? '异常' : '正常' }}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </DataCard>

          <!-- 安全监控 -->
          <DataCard title="安全监控" class="safety-monitoring-card">
            <div class="safety-summary">
              <div class="safety-score-display">
                <div class="score-circle" :class="getOverallScoreClass()">
                  <span class="score-number">{{ safetyMonitoring.safetyMetrics?.overallSafetyScore || 0 }}</span>
                  <span class="score-unit">分</span>
                </div>
                <div class="score-text">综合安全评分</div>
              </div>
              
              <div class="alert-counts">
                <div class="alert-count-item critical">
                  <span class="count">{{ safetyMonitoring.safetyMetrics?.criticalAlerts || 0 }}</span>
                  <span class="label">严重告警</span>
                </div>
                <div class="alert-count-item warning">
                  <span class="count">{{ safetyMonitoring.safetyMetrics?.warningAlerts || 0 }}</span>
                  <span class="label">警告</span>
                </div>
                <div class="alert-count-item info">
                  <span class="count">{{ safetyMonitoring.safetyMetrics?.infoAlerts || 0 }}</span>
                  <span class="label">提示</span>
                </div>
              </div>
            </div>

            <div class="current-alerts" v-if="safetyMonitoring.currentAlerts?.length > 0">
              <h5 class="alerts-title">当前告警</h5>
              <div class="alerts-list">
                <div 
                  class="alert-item"
                  v-for="alert in safetyMonitoring.currentAlerts"
                  :key="alert.deviceId + alert.alertType"
                >
                  <div class="alert-icon" :class="`alert--${alert.alertLevel.toLowerCase()}`">
                    <el-icon><Warning /></el-icon>
                  </div>
                  <div class="alert-content">
                    <div class="alert-message">{{ alert.message }}</div>
                    <div class="alert-device">{{ alert.deviceName }}</div>
                    <div class="alert-value">
                      当前值: {{ alert.currentValue }} (阈值: {{ alert.threshold }})
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </DataCard>
        </div>
      </div>
    </div>

    <!-- 底部效率分析 -->
    <div class="efficiency-section">
      <DataCard title="工作效率分析" class="efficiency-card">
        <div class="efficiency-content">
          <div class="efficiency-chart">
            <DigitalChart
              :option="efficiencyChartOption"
              :loading="chartLoading"
              height="200px"
            />
          </div>
          
          <div class="efficiency-ranking">
            <h5 class="ranking-title">设备效率排行</h5>
            <div class="ranking-list">
              <div 
                class="ranking-item"
                v-for="device in efficiencyAnalysis.deviceRanking"
                :key="device.deviceId"
                :class="{ 'top-rank': device.rank <= 3 }"
              >
                <div class="rank-number" :class="`rank--${device.rank}`">
                  {{ device.rank }}
                </div>
                <div class="device-info">
                  <div class="device-name">{{ device.deviceName }}</div>
                  <div class="device-stats">
                    工时: {{ device.workingHours }}h | 作业: {{ device.operationCount }}次
                  </div>
                </div>
                <div class="efficiency-score">
                  <span class="score">{{ device.efficiency }}%</span>
                  <el-progress
                    :percentage="device.efficiency"
                    :stroke-width="4"
                    :show-text="false"
                    :color="getEfficiencyColor(device.efficiency)"
                  />
                </div>
              </div>
            </div>
          </div>
        </div>
      </DataCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import {
  Warning,
  PieChart,
  Refresh,
  Platform,
  Top,
  Monitor
} from '@element-plus/icons-vue'

import StatisticCard from '../components/StatisticCard.vue'
import DataCard from '../components/DataCard.vue'
import DigitalChart from '../components/DigitalChart.vue'

import { digitalTwinService } from '@/api/services/digital-twin.service'

// 数据状态
const craneElevatorDevices = ref<any[]>([])
const deviceStatistics = ref<any>({})
const safetyMonitoring = ref<any>({})
const efficiencyAnalysis = ref<any>({})
const selectedDevice = ref<any>(null)
const chartLoading = ref(false)

// 定时器
let dataTimer: NodeJS.Timeout | null = null

// 加载设备列表
const loadCraneElevatorDevices = async () => {
  try {
    const response = await digitalTwinService.getCraneElevatorDevices()
    if (response.success) {
      craneElevatorDevices.value = response.data
      if (response.data.length > 0 && !selectedDevice.value) {
        selectedDevice.value = response.data[0]
      }
    }
  } catch (error) {
    console.error('Failed to load crane elevator devices:', error)
    ElMessage.error('加载设备列表失败')
  }
}

// 加载设备统计
const loadDeviceStatistics = async () => {
  try {
    const response = await digitalTwinService.getCraneElevatorStatistics()
    if (response.success) {
      deviceStatistics.value = response.data
    }
  } catch (error) {
    console.error('Failed to load device statistics:', error)
    ElMessage.error('加载设备统计失败')
  }
}

// 加载安全监控
const loadSafetyMonitoring = async () => {
  try {
    const response = await digitalTwinService.getSafetyMonitoring()
    if (response.success) {
      safetyMonitoring.value = response.data
    }
  } catch (error) {
    console.error('Failed to load safety monitoring:', error)
    ElMessage.error('加载安全监控失败')
  }
}

// 加载效率分析
const loadEfficiencyAnalysis = async () => {
  try {
    chartLoading.value = true
    const response = await digitalTwinService.getEfficiencyAnalysis()
    if (response.success) {
      efficiencyAnalysis.value = response.data
    }
  } catch (error) {
    console.error('Failed to load efficiency analysis:', error)
    ElMessage.error('加载效率分析失败')
  } finally {
    chartLoading.value = false
  }
}

// 效率趋势图表配置
const efficiencyChartOption = computed(() => {
  const data = efficiencyAnalysis.value.efficiencyTrend
  if (!data) return {}

  return {
    tooltip: {
      trigger: 'axis',
      backgroundColor: 'rgba(30, 42, 58, 0.9)',
      borderColor: '#3498db',
      textStyle: { color: '#ffffff' }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: data.dates || [],
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } }
    },
    yAxis: {
      type: 'value',
      name: '效率(%)',
      axisLabel: { color: '#7f8c8d', formatter: '{value}%' },
      axisLine: { lineStyle: { color: '#34495e' } },
      splitLine: { lineStyle: { color: '#34495e', type: 'dashed' } }
    },
    series: [
      {
        name: '工作效率',
        type: 'line',
        data: data.efficiency || [],
        smooth: true,
        lineStyle: { color: '#2ecc71', width: 3 },
        itemStyle: { color: '#2ecc71' },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              { offset: 0, color: 'rgba(46, 204, 113, 0.3)' },
              { offset: 1, color: 'rgba(46, 204, 113, 0.1)' }
            ]
          }
        }
      }
    ]
  }
})

// 选择设备
const selectDevice = (device: any) => {
  selectedDevice.value = device
}

// 刷新设备
const refreshDevices = () => {
  loadCraneElevatorDevices()
  ElMessage.success('设备数据已刷新')
}

// 工具函数
const getDeviceIcon = (type: string) => {
  return type === 'TowerCrane' ? 'Monitor' : 'Top'
}

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Running': '运行中',
    'Idle': '空闲',
    'Maintenance': '维护中',
    'Fault': '故障'
  }
  return statusMap[status] || status
}

const getLoadPercentage = (device: any) => {
  const load = device.realTimeData?.load || 0
  const maxLoad = device.realTimeData?.maxLoad || 1
  return Math.min(100, (load / maxLoad) * 100)
}

const getHeightPercentage = (device: any) => {
  const height = device.realTimeData?.height || 0
  const maxHeight = device.realTimeData?.maxHeight || 1
  return Math.min(100, (height / maxHeight) * 100)
}

const getLoadColor = (device: any) => {
  const percentage = getLoadPercentage(device)
  if (percentage >= 90) return '#e74c3c'
  if (percentage >= 70) return '#f39c12'
  return '#2ecc71'
}

const getScoreClass = (score: number) => {
  if (score >= 90) return 'score-excellent'
  if (score >= 70) return 'score-good'
  if (score >= 50) return 'score-average'
  return 'score-poor'
}

const getOverallScoreClass = () => {
  const score = safetyMonitoring.value.safetyMetrics?.overallSafetyScore || 0
  if (score >= 90) return 'excellent'
  if (score >= 70) return 'good'
  if (score >= 50) return 'average'
  return 'poor'
}

const getSafetyIcon = (key: string) => {
  const iconMap: Record<string, string> = {
    'overload': 'Platform',
    'overHeight': 'Top',
    'highWind': 'Monitor',
    'collision': 'Warning',
    'doorStatus': 'Lock'
  }
  return iconMap[key] || 'Warning'
}

const getSafetyLabel = (key: string) => {
  const labelMap: Record<string, string> = {
    'overload': '超载检测',
    'overHeight': '超高检测',
    'highWind': '强风检测',
    'collision': '碰撞检测',
    'doorStatus': '门状态检测'
  }
  return labelMap[key] || key
}

const getEfficiencyColor = (efficiency: number) => {
  if (efficiency >= 90) return '#2ecc71'
  if (efficiency >= 70) return '#3498db'
  if (efficiency >= 50) return '#f39c12'
  return '#e74c3c'
}

// 初始化数据
const initData = async () => {
  await Promise.all([
    loadCraneElevatorDevices(),
    loadDeviceStatistics(),
    loadSafetyMonitoring(),
    loadEfficiencyAnalysis()
  ])
}

// 启动数据更新
const startDataUpdate = () => {
  dataTimer = setInterval(() => {
    loadCraneElevatorDevices()
    loadSafetyMonitoring()
  }, 15000) // 15秒更新一次
}

// 停止数据更新
const stopDataUpdate = () => {
  if (dataTimer) {
    clearInterval(dataTimer)
    dataTimer = null
  }
}

onMounted(() => {
  initData()
  startDataUpdate()
})

onUnmounted(() => {
  stopDataUpdate()
})
</script>

<style lang="scss" scoped>
.crane-management-screen {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  min-height: 100vh;
}

// 统计区域
.stats-section {
  .stats-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 16px;
  }
}

// 主要内容区域
.main-content {
  .content-grid {
    display: grid;
    grid-template-columns: 1fr 400px;
    gap: 20px;
  }
}

// 设备区域
.devices-section {
  .devices-card {
    .devices-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 16px;
      max-height: 500px;
      overflow-y: auto;
      
      .device-item {
        padding: 16px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        cursor: pointer;
        transition: all 0.3s ease;
        
        &:hover {
          background: rgba(255, 255, 255, 0.08);
          border-color: #3498db;
          transform: translateY(-2px);
        }
        
        &.active {
          background: rgba(52, 152, 219, 0.2);
          border-color: #3498db;
        }
        
        &.device--running {
          border-left: 4px solid #2ecc71;
        }
        
        &.device--idle {
          border-left: 4px solid #95a5a6;
        }
        
        &.device--fault {
          border-left: 4px solid #e74c3c;
        }
        
        .device-header {
          display: flex;
          align-items: center;
          gap: 12px;
          margin-bottom: 12px;
          
          .device-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            background: rgba(52, 152, 219, 0.2);
            display: flex;
            align-items: center;
            justify-content: center;
            
            .el-icon {
              font-size: 18px;
              color: #3498db;
            }
          }
          
          .device-info {
            flex: 1;
            
            .device-name {
              font-size: 14px;
              font-weight: 600;
              color: #ffffff;
              margin-bottom: 2px;
            }
            
            .device-model {
              font-size: 12px;
              color: #7f8c8d;
            }
          }
          
          .device-status {
            padding: 4px 8px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 500;
            
            &.status--running {
              background: rgba(46, 204, 113, 0.2);
              color: #2ecc71;
            }
            
            &.status--idle {
              background: rgba(149, 165, 166, 0.2);
              color: #95a5a6;
            }
            
            &.status--fault {
              background: rgba(231, 76, 60, 0.2);
              color: #e74c3c;
            }
          }
        }
        
        .device-metrics {
          .metric-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
            margin-bottom: 12px;
            
            .metric-item {
              .metric-label {
                font-size: 11px;
                color: #7f8c8d;
                display: block;
                margin-bottom: 4px;
              }
              
              .metric-value {
                font-size: 13px;
                font-weight: 600;
                color: #ffffff;
                display: block;
                margin-bottom: 4px;
              }
              
              .metric-bar {
                height: 4px;
                background: rgba(255, 255, 255, 0.1);
                border-radius: 2px;
                overflow: hidden;
                
                .metric-fill {
                  height: 100%;
                  transition: width 0.3s ease;
                }
              }
            }
          }
          
          .safety-score {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding-top: 8px;
            border-top: 1px solid rgba(255, 255, 255, 0.1);
            
            .score-label {
              font-size: 11px;
              color: #7f8c8d;
            }
            
            .score-value {
              font-size: 14px;
              font-weight: 700;
              
              &.score-excellent {
                color: #2ecc71;
              }
              
              &.score-good {
                color: #3498db;
              }
              
              &.score-average {
                color: #f39c12;
              }
              
              &.score-poor {
                color: #e74c3c;
              }
            }
          }
        }
      }
    }
  }
}

// 信息面板
.info-panel {
  display: flex;
  flex-direction: column;
  gap: 16px;
  
  .device-detail-card {
    .device-detail {
      .detail-section {
        margin-bottom: 20px;
        
        &:last-child {
          margin-bottom: 0;
        }
        
        .section-title {
          font-size: 14px;
          font-weight: 600;
          color: #3498db;
          margin: 0 0 12px 0;
          padding-bottom: 4px;
          border-bottom: 1px solid rgba(52, 152, 219, 0.3);
        }
        
        .detail-grid {
          display: grid;
          grid-template-columns: 1fr 1fr;
          gap: 8px;
          
          .detail-item {
            padding: 8px;
            border-radius: 4px;
            background: rgba(255, 255, 255, 0.03);
            
            .detail-label {
              font-size: 11px;
              color: #7f8c8d;
              display: block;
              margin-bottom: 2px;
            }
            
            .detail-value {
              font-size: 12px;
              font-weight: 600;
              color: #ffffff;
            }
          }
        }
        
        .realtime-grid {
          display: flex;
          flex-direction: column;
          gap: 12px;
          
          .realtime-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 8px;
            border-radius: 6px;
            background: rgba(255, 255, 255, 0.03);
            
            .realtime-icon {
              width: 32px;
              height: 32px;
              border-radius: 50%;
              background: rgba(52, 152, 219, 0.2);
              display: flex;
              align-items: center;
              justify-content: center;
              
              .el-icon {
                font-size: 14px;
                color: #3498db;
              }
            }
            
            .realtime-content {
              flex: 1;
              
              .realtime-label {
                font-size: 11px;
                color: #7f8c8d;
                margin-bottom: 2px;
              }
              
              .realtime-value {
                font-size: 13px;
                font-weight: 600;
                color: #ffffff;
                
                .realtime-max {
                  font-size: 10px;
                  color: #7f8c8d;
                  font-weight: normal;
                }
              }
            }
          }
        }
        
        .safety-indicators {
          display: flex;
          flex-direction: column;
          gap: 8px;
          
          .safety-item {
            display: flex;
            align-items: center;
            gap: 8px;
            padding: 6px 8px;
            border-radius: 4px;
            background: rgba(255, 255, 255, 0.03);
            
            .safety-icon {
              width: 24px;
              height: 24px;
              border-radius: 50%;
              background: rgba(46, 204, 113, 0.2);
              display: flex;
              align-items: center;
              justify-content: center;
              
              &.danger {
                background: rgba(231, 76, 60, 0.2);
                
                .el-icon {
                  color: #e74c3c;
                }
              }
              
              .el-icon {
                font-size: 12px;
                color: #2ecc71;
              }
            }
            
            .safety-label {
              flex: 1;
              font-size: 11px;
              color: #ecf0f1;
            }
            
            .safety-status {
              font-size: 11px;
              font-weight: 600;
              color: #2ecc71;
              
              &.danger {
                color: #e74c3c;
              }
            }
          }
        }
      }
    }
  }
  
  .safety-monitoring-card {
    .safety-summary {
      display: flex;
      flex-direction: column;
      gap: 16px;
      margin-bottom: 16px;
      
      .safety-score-display {
        text-align: center;
        
        .score-circle {
          width: 80px;
          height: 80px;
          border-radius: 50%;
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          margin: 0 auto 8px;
          border: 3px solid;
          
          &.excellent {
            border-color: #2ecc71;
            background: rgba(46, 204, 113, 0.1);
          }
          
          &.good {
            border-color: #3498db;
            background: rgba(52, 152, 219, 0.1);
          }
          
          &.average {
            border-color: #f39c12;
            background: rgba(243, 156, 18, 0.1);
          }
          
          &.poor {
            border-color: #e74c3c;
            background: rgba(231, 76, 60, 0.1);
          }
          
          .score-number {
            font-size: 20px;
            font-weight: 700;
            color: #ffffff;
            line-height: 1;
          }
          
          .score-unit {
            font-size: 10px;
            color: #7f8c8d;
          }
        }
        
        .score-text {
          font-size: 12px;
          color: #7f8c8d;
        }
      }
      
      .alert-counts {
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        gap: 8px;
        
        .alert-count-item {
          text-align: center;
          padding: 8px;
          border-radius: 6px;
          
          &.critical {
            background: rgba(231, 76, 60, 0.1);
            border: 1px solid rgba(231, 76, 60, 0.3);
          }
          
          &.warning {
            background: rgba(243, 156, 18, 0.1);
            border: 1px solid rgba(243, 156, 18, 0.3);
          }
          
          &.info {
            background: rgba(52, 152, 219, 0.1);
            border: 1px solid rgba(52, 152, 219, 0.3);
          }
          
          .count {
            font-size: 16px;
            font-weight: 700;
            color: #ffffff;
            display: block;
          }
          
          .label {
            font-size: 10px;
            color: #7f8c8d;
          }
        }
      }
    }
    
    .current-alerts {
      .alerts-title {
        font-size: 12px;
        font-weight: 600;
        color: #3498db;
        margin: 0 0 8px 0;
      }
      
      .alerts-list {
        .alert-item {
          display: flex;
          align-items: flex-start;
          gap: 8px;
          padding: 8px;
          margin-bottom: 8px;
          border-radius: 6px;
          background: rgba(255, 255, 255, 0.03);
          
          .alert-icon {
            width: 20px;
            height: 20px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            
            &.alert--warning {
              background: rgba(243, 156, 18, 0.3);
              
              .el-icon {
                font-size: 10px;
                color: #f39c12;
              }
            }
            
            &.alert--critical {
              background: rgba(231, 76, 60, 0.3);
              
              .el-icon {
                font-size: 10px;
                color: #e74c3c;
              }
            }
          }
          
          .alert-content {
            flex: 1;
            
            .alert-message {
              font-size: 11px;
              font-weight: 600;
              color: #ffffff;
              margin-bottom: 2px;
            }
            
            .alert-device {
              font-size: 10px;
              color: #3498db;
              margin-bottom: 2px;
            }
            
            .alert-value {
              font-size: 9px;
              color: #7f8c8d;
            }
          }
        }
      }
    }
  }
}

// 效率区域
.efficiency-section {
  .efficiency-card {
    .efficiency-content {
      display: grid;
      grid-template-columns: 1fr 350px;
      gap: 20px;
      
      .efficiency-ranking {
        .ranking-title {
          font-size: 14px;
          font-weight: 600;
          color: #3498db;
          margin: 0 0 12px 0;
        }
        
        .ranking-list {
          .ranking-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 8px;
            margin-bottom: 8px;
            border-radius: 6px;
            background: rgba(255, 255, 255, 0.03);
            
            &.top-rank {
              background: rgba(241, 196, 15, 0.1);
              border: 1px solid rgba(241, 196, 15, 0.3);
            }
            
            .rank-number {
              width: 24px;
              height: 24px;
              border-radius: 50%;
              display: flex;
              align-items: center;
              justify-content: center;
              font-weight: 700;
              font-size: 11px;
              
              &.rank--1 {
                background: linear-gradient(135deg, #f1c40f, #f39c12);
                color: #ffffff;
              }
              
              &.rank--2 {
                background: linear-gradient(135deg, #95a5a6, #7f8c8d);
                color: #ffffff;
              }
              
              &.rank--3 {
                background: linear-gradient(135deg, #e67e22, #d35400);
                color: #ffffff;
              }
              
              &:not(.rank--1):not(.rank--2):not(.rank--3) {
                background: rgba(52, 152, 219, 0.2);
                color: #3498db;
              }
            }
            
            .device-info {
              flex: 1;
              
              .device-name {
                font-size: 12px;
                font-weight: 600;
                color: #ffffff;
                margin-bottom: 2px;
              }
              
              .device-stats {
                font-size: 10px;
                color: #7f8c8d;
              }
            }
            
            .efficiency-score {
              width: 60px;
              text-align: right;
              
              .score {
                font-size: 12px;
                font-weight: 700;
                color: #ffffff;
                display: block;
                margin-bottom: 4px;
              }
            }
          }
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 1400px) {
  .main-content .content-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .efficiency-section .efficiency-content {
    grid-template-columns: 1fr;
    gap: 16px;
  }
}

@media (max-width: 768px) {
  .crane-management-screen {
    padding: 12px;
  }
  
  .stats-section .stats-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  
  .devices-section .devices-grid {
    grid-template-columns: 1fr;
  }
}
</style>