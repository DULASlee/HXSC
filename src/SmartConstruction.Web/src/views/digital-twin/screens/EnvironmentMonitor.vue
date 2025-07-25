<template>
  <div class="environment-monitor-screen">
    <!-- 顶部环境数据概览 -->
    <div class="overview-section">
      <div class="overview-grid">
        <StatisticCard
          :value="environmentData.dustMonitoring?.pm25 || 0"
          label="PM2.5"
          description="细颗粒物浓度"
          icon="Sunny"
          type="primary"
          suffix="μg/m³"
          :trend="getTrendValue('pm25')"
          :status="getDustStatus('pm25')"
        />
        <StatisticCard
          :value="environmentData.dustMonitoring?.pm10 || 0"
          label="PM10"
          description="可吸入颗粒物"
          icon="Cloudy"
          type="warning"
          suffix="μg/m³"
          :trend="getTrendValue('pm10')"
          :status="getDustStatus('pm10')"
        />
        <StatisticCard
          :value="environmentData.noiseMonitoring?.currentLevel || 0"
          label="噪音等级"
          description="当前噪音分贝"
          icon="Monitor"
          type="info"
          suffix="dB"
          :trend="getTrendValue('noise')"
          :status="getNoiseStatus()"
        />
        <StatisticCard
          :value="environmentData.weatherData?.temperature || 0"
          label="温度"
          description="当前环境温度"
          icon="Camera"
          type="success"
          suffix="°C"
          :trend="getTrendValue('temperature')"
          status="normal"
        />
        <StatisticCard
          :value="environmentData.weatherData?.humidity || 0"
          label="湿度"
          description="空气相对湿度"
          icon="Drizzling"
          type="info"
          suffix="%"
          :trend="getTrendValue('humidity')"
          status="normal"
        />
        <StatisticCard
          :value="environmentData.weatherData?.windSpeed || 0"
          label="风速"
          description="当前风速"
          icon="Warning"
          type="primary"
          suffix="m/s"
          :trend="getTrendValue('windSpeed')"
          status="normal"
        />
      </div>
    </div>

    <!-- 主要监测内容 -->
    <div class="main-content">
      <div class="content-grid">
        <!-- 扬尘监测 -->
        <div class="dust-section">
          <DataCard title="扬尘监测" class="dust-card">
            <div class="dust-metrics">
              <div class="dust-gauge">
                <div class="gauge-container">
                  <div class="gauge-item">
                    <div class="gauge-circle" :class="getDustLevelClass('pm25')">
                      <div class="gauge-value">
                        <span class="value">{{ environmentData.dustMonitoring?.pm25 || 0 }}</span>
                        <span class="unit">μg/m³</span>
                      </div>
                      <div class="gauge-label">PM2.5</div>
                    </div>
                    <div class="gauge-status" :class="getDustStatusClass('pm25')">
                      {{ getDustLevelText('pm25') }}
                    </div>
                  </div>
                  
                  <div class="gauge-item">
                    <div class="gauge-circle" :class="getDustLevelClass('pm10')">
                      <div class="gauge-value">
                        <span class="value">{{ environmentData.dustMonitoring?.pm10 || 0 }}</span>
                        <span class="unit">μg/m³</span>
                      </div>
                      <div class="gauge-label">PM10</div>
                    </div>
                    <div class="gauge-status" :class="getDustStatusClass('pm10')">
                      {{ getDustLevelText('pm10') }}
                    </div>
                  </div>
                  
                  <div class="gauge-item">
                    <div class="gauge-circle" :class="getDustLevelClass('tsp')">
                      <div class="gauge-value">
                        <span class="value">{{ environmentData.dustMonitoring?.tsp || 0 }}</span>
                        <span class="unit">μg/m³</span>
                      </div>
                      <div class="gauge-label">TSP</div>
                    </div>
                    <div class="gauge-status" :class="getDustStatusClass('tsp')">
                      {{ getDustLevelText('tsp') }}
                    </div>
                  </div>
                </div>
                
                <div class="dust-summary">
                  <div class="summary-item">
                    <span class="summary-label">总体状态</span>
                    <span class="summary-value" :class="getDustStatusClass('overall')">
                      {{ environmentData.dustMonitoring?.status || 'Unknown' }}
                    </span>
                  </div>
                  <div class="summary-item">
                    <span class="summary-label">告警等级</span>
                    <span class="summary-value" :class="getAlertLevelClass()">
                      {{ environmentData.dustMonitoring?.alertLevel || 'Normal' }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="dust-trends">
                <h5 class="trends-title">24小时扬尘趋势</h5>
                <DigitalChart
                  :option="dustTrendOption"
                  :loading="chartLoading"
                  height="200px"
                />
              </div>
            </div>
          </DataCard>
        </div>

        <!-- 噪音监测 -->
        <div class="noise-section">
          <DataCard title="噪音监测" class="noise-card">
            <div class="noise-metrics">
              <div class="noise-gauge">
                <div class="circular-progress">
                  <svg class="progress-ring" width="120" height="120">
                    <circle
                      class="progress-ring-background"
                      stroke="#34495e"
                      stroke-width="8"
                      fill="transparent"
                      r="52"
                      cx="60"
                      cy="60"
                    />
                    <circle
                      class="progress-ring-progress"
                      :stroke="getNoiseColor()"
                      stroke-width="8"
                      fill="transparent"
                      r="52"
                      cx="60"
                      cy="60"
                      :stroke-dasharray="getNoiseProgress()"
                      :stroke-dashoffset="getNoiseOffset()"
                    />
                  </svg>
                  <div class="progress-content">
                    <span class="progress-value">{{ environmentData.noiseMonitoring?.currentLevel || 0 }}</span>
                    <span class="progress-unit">dB</span>
                  </div>
                </div>
                
                <div class="noise-info">
                  <div class="noise-levels">
                    <div class="level-item">
                      <span class="level-label">当前值</span>
                      <span class="level-value">{{ environmentData.noiseMonitoring?.currentLevel || 0 }} dB</span>
                    </div>
                    <div class="level-item">
                      <span class="level-label">平均值</span>
                      <span class="level-value">{{ environmentData.noiseMonitoring?.averageLevel || 0 }} dB</span>
                    </div>
                    <div class="level-item">
                      <span class="level-label">最大值</span>
                      <span class="level-value">{{ environmentData.noiseMonitoring?.maxLevel || 0 }} dB</span>
                    </div>
                  </div>
                  
                  <div class="noise-thresholds">
                    <div class="threshold-item">
                      <span class="threshold-label">白天标准</span>
                      <span class="threshold-value">
                        {{ environmentData.noiseMonitoring?.threshold?.day || 70 }} dB
                      </span>
                    </div>
                    <div class="threshold-item">
                      <span class="threshold-label">夜间标准</span>
                      <span class="threshold-value">
                        {{ environmentData.noiseMonitoring?.threshold?.night || 55 }} dB
                      </span>
                    </div>
                  </div>
                </div>
              </div>
              
              <div class="noise-trends">
                <h5 class="trends-title">24小时噪音趋势</h5>
                <DigitalChart
                  :option="noiseTrendOption"
                  :loading="chartLoading"
                  height="200px"
                />
              </div>
            </div>
          </DataCard>
        </div>

        <!-- 气象数据 -->
        <div class="weather-section">
          <DataCard title="气象监测" class="weather-card">
            <div class="weather-grid">
              <div class="weather-item">
                <div class="weather-icon">
                  <el-icon><Camera /></el-icon>
                </div>
                <div class="weather-content">
                  <div class="weather-label">温度</div>
                  <div class="weather-value">{{ environmentData.weatherData?.temperature || 0 }}°C</div>
                </div>
              </div>
              
              <div class="weather-item">
                <div class="weather-icon">
                  <el-icon><Drizzling /></el-icon>
                </div>
                <div class="weather-content">
                  <div class="weather-label">湿度</div>
                  <div class="weather-value">{{ environmentData.weatherData?.humidity || 0 }}%</div>
                </div>
              </div>
              
              <div class="weather-item">
                <div class="weather-icon">
                  <el-icon><Warning /></el-icon>
                </div>
                <div class="weather-content">
                  <div class="weather-label">风速</div>
                  <div class="weather-value">{{ environmentData.weatherData?.windSpeed || 0 }}m/s</div>
                </div>
              </div>
              
              <div class="weather-item">
                <div class="weather-icon">
                  <el-icon><Compass /></el-icon>
                </div>
                <div class="weather-content">
                  <div class="weather-label">风向</div>
                  <div class="weather-value">{{ environmentData.weatherData?.windDirection || '-' }}</div>
                </div>
              </div>
              
              <div class="weather-item">
                <div class="weather-icon">
                  <el-icon><Position /></el-icon>
                </div>
                <div class="weather-content">
                  <div class="weather-label">气压</div>
                  <div class="weather-value">{{ environmentData.weatherData?.airPressure || 0 }}hPa</div>
                </div>
              </div>
              
              <div class="weather-item">
                <div class="weather-icon">
                  <el-icon><View /></el-icon>
                </div>
                <div class="weather-content">
                  <div class="weather-label">能见度</div>
                  <div class="weather-value">{{ environmentData.weatherData?.visibility || 0 }}km</div>
                </div>
              </div>
            </div>
          </DataCard>
        </div>
      </div>
    </div>

    <!-- 监测点位和告警信息 -->
    <div class="bottom-section">
      <div class="bottom-grid">
        <!-- 监测点位 -->
        <DataCard title="监测点位" class="monitoring-points-card">
          <div class="points-list">
            <div 
              class="point-item"
              v-for="point in monitoringPoints"
              :key="point.id"
              @click="selectPoint(point)"
              :class="{ active: selectedPoint?.id === point.id }"
            >
              <div class="point-header">
                <div class="point-icon">
                  <el-icon><Location /></el-icon>
                </div>
                <div class="point-info">
                  <div class="point-name">{{ point.name }}</div>
                  <div class="point-zone">{{ point.location?.zone }}</div>
                </div>
                <div class="point-status" :class="`status--${point.status.toLowerCase()}`">
                  {{ point.status }}
                </div>
              </div>
              
              <div class="point-sensors">
                <div class="sensor-item" v-if="point.sensors?.dustSensor">
                  <span class="sensor-label">扬尘</span>
                  <span class="sensor-value">
                    PM2.5: {{ point.sensors.dustSensor.pm25 }}
                  </span>
                </div>
                <div class="sensor-item" v-if="point.sensors?.noiseSensor">
                  <span class="sensor-label">噪音</span>
                  <span class="sensor-value">
                    {{ point.sensors.noiseSensor.level }}dB
                  </span>
                </div>
                <div class="sensor-item" v-if="point.sensors?.weatherSensor">
                  <span class="sensor-label">气象</span>
                  <span class="sensor-value">
                    {{ point.sensors.weatherSensor.temperature }}°C
                  </span>
                </div>
              </div>
            </div>
          </div>
        </DataCard>

        <!-- 环境告警 -->
        <DataCard title="环境告警" class="alerts-card">
          <div class="alerts-content">
            <div class="alerts-summary">
              <div class="summary-stats">
                <div class="stat-item">
                  <span class="stat-value">{{ environmentAlerts.alertStatistics?.todayAlerts || 0 }}</span>
                  <span class="stat-label">今日告警</span>
                </div>
                <div class="stat-item">
                  <span class="stat-value">{{ environmentAlerts.alertStatistics?.activeAlerts || 0 }}</span>
                  <span class="stat-label">活跃告警</span>
                </div>
                <div class="stat-item">
                  <span class="stat-value">{{ environmentAlerts.alertStatistics?.resolvedAlerts || 0 }}</span>
                  <span class="stat-label">已处理</span>
                </div>
              </div>
              
              <div class="alert-types">
                <div class="type-item" v-for="(count, type) in environmentAlerts.alertStatistics?.alertTypes" :key="type">
                  <span class="type-label">{{ getAlertTypeText(type) }}</span>
                  <span class="type-count">{{ count }}</span>
                  <div class="type-bar">
                    <div 
                      class="type-fill"
                      :style="{ 
                        width: `${getAlertTypePercentage(count)}%`,
                        background: getAlertTypeColor(type)
                      }"
                    ></div>
                  </div>
                </div>
              </div>
            </div>
            
            <div class="current-alerts" v-if="environmentAlerts.currentAlerts?.length > 0">
              <h5 class="alerts-title">当前告警</h5>
              <div class="alerts-list">
                <div 
                  class="alert-item"
                  v-for="alert in environmentAlerts.currentAlerts"
                  :key="alert.id"
                >
                  <div class="alert-icon" :class="`alert--${alert.level.toLowerCase()}`">
                    <el-icon><Warning /></el-icon>
                  </div>
                  <div class="alert-content">
                    <div class="alert-message">{{ alert.message }}</div>
                    <div class="alert-location">{{ alert.location }}</div>
                    <div class="alert-value">
                      当前值: {{ alert.currentValue }}{{ alert.unit }} 
                      (阈值: {{ alert.threshold }}{{ alert.unit }})
                    </div>
                    <div class="alert-time">{{ formatTime(alert.timestamp) }}</div>
                  </div>
                  <div class="alert-status" :class="`status--${alert.status.toLowerCase()}`">
                    {{ alert.status }}
                  </div>
                </div>
              </div>
            </div>
          </div>
        </DataCard>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import {
  Sunny,
  Cloudy,
  Monitor,
  Camera,
  Drizzling,
  Warning,
  Compass,
  Position,
  View,
  Location
} from '@element-plus/icons-vue'

import StatisticCard from '../components/StatisticCard.vue'
import DataCard from '../components/DataCard.vue'
import DigitalChart from '../components/DigitalChart.vue'

import { digitalTwinService } from '@/api/services/digital-twin.service'
import { formatTime } from '@/utils/format'

// 数据状态
const environmentData = ref<any>({})
const monitoringPoints = ref<any[]>([])
const environmentTrends = ref<any>({})
const environmentAlerts = ref<any>({})
const selectedPoint = ref<any>(null)
const chartLoading = ref(false)

// 定时器
let dataTimer: NodeJS.Timeout | null = null

// 加载环境监测数据
const loadEnvironmentData = async () => {
  try {
    const response = await digitalTwinService.getEnvironmentMonitoring()
    if (response.success) {
      environmentData.value = response.data
    }
  } catch (error) {
    console.error('Failed to load environment data:', error)
    ElMessage.error('加载环境数据失败')
  }
}

// 加载监测点位
const loadMonitoringPoints = async () => {
  try {
    const response = await digitalTwinService.getMonitoringPoints()
    if (response.success) {
      monitoringPoints.value = response.data
      if (response.data.length > 0 && !selectedPoint.value) {
        selectedPoint.value = response.data[0]
      }
    }
  } catch (error) {
    console.error('Failed to load monitoring points:', error)
    ElMessage.error('加载监测点位失败')
  }
}

// 加载环境趋势
const loadEnvironmentTrends = async () => {
  try {
    chartLoading.value = true
    const response = await digitalTwinService.getEnvironmentTrends()
    if (response.success) {
      environmentTrends.value = response.data
    }
  } catch (error) {
    console.error('Failed to load environment trends:', error)
    ElMessage.error('加载环境趋势失败')
  } finally {
    chartLoading.value = false
  }
}

// 加载环境告警
const loadEnvironmentAlerts = async () => {
  try {
    const response = await digitalTwinService.getEnvironmentAlerts()
    if (response.success) {
      environmentAlerts.value = response.data
    }
  } catch (error) {
    console.error('Failed to load environment alerts:', error)
    ElMessage.error('加载环境告警失败')
  }
}

// 扬尘趋势图表配置
const dustTrendOption = computed(() => {
  const data = environmentTrends.value.dustTrend?.hourlyData
  if (!data) return {}

  return {
    tooltip: {
      trigger: 'axis',
      backgroundColor: 'rgba(30, 42, 58, 0.9)',
      borderColor: '#3498db',
      textStyle: { color: '#ffffff' }
    },
    legend: {
      data: ['PM2.5', 'PM10'],
      textStyle: { color: '#7f8c8d' }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: data.times || [],
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } }
    },
    yAxis: {
      type: 'value',
      name: 'μg/m³',
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } },
      splitLine: { lineStyle: { color: '#34495e', type: 'dashed' } }
    },
    series: [
      {
        name: 'PM2.5',
        type: 'line',
        data: data.pm25 || [],
        smooth: true,
        lineStyle: { color: '#e74c3c', width: 3 },
        itemStyle: { color: '#e74c3c' }
      },
      {
        name: 'PM10',
        type: 'line',
        data: data.pm10 || [],
        smooth: true,
        lineStyle: { color: '#f39c12', width: 3 },
        itemStyle: { color: '#f39c12' }
      }
    ]
  }
})

// 噪音趋势图表配置
const noiseTrendOption = computed(() => {
  const data = environmentTrends.value.noiseTrend?.hourlyData
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
      data: data.times || [],
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } }
    },
    yAxis: {
      type: 'value',
      name: 'dB',
      axisLabel: { color: '#7f8c8d' },
      axisLine: { lineStyle: { color: '#34495e' } },
      splitLine: { lineStyle: { color: '#34495e', type: 'dashed' } }
    },
    series: [
      {
        name: '噪音等级',
        type: 'line',
        data: data.levels || [],
        smooth: true,
        lineStyle: { color: '#9b59b6', width: 3 },
        itemStyle: { color: '#9b59b6' },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0, y: 0, x2: 0, y2: 1,
            colorStops: [
              { offset: 0, color: 'rgba(155, 89, 182, 0.3)' },
              { offset: 1, color: 'rgba(155, 89, 182, 0.1)' }
            ]
          }
        }
      }
    ]
  }
})

// 选择监测点
const selectPoint = (point: any) => {
  selectedPoint.value = point
  ElMessage.info(`切换到监测点：${point.name}`)
}

// 工具函数
const getTrendValue = (type: string) => {
  // 模拟趋势值
  const trends: Record<string, number> = {
    'pm25': -2.1,
    'pm10': 1.5,
    'noise': -0.8,
    'temperature': 0.5,
    'humidity': -1.2,
    'windSpeed': 2.3
  }
  return trends[type] || 0
}

const getDustStatus = (type: string) => {
  const dust = environmentData.value.dustMonitoring
  if (!dust) return 'normal'
  
  const value = dust[type]
  const threshold = dust.threshold?.[type]
  
  if (!value || !threshold) return 'normal'
  
  if (value > threshold * 0.8) return 'warning'
  if (value > threshold) return 'offline'
  return 'online'
}

const getNoiseStatus = () => {
  const noise = environmentData.value.noiseMonitoring
  if (!noise) return 'normal'
  
  const currentLevel = noise.currentLevel
  const dayThreshold = noise.threshold?.day || 70
  
  if (currentLevel > dayThreshold) return 'warning'
  return 'online'
}

const getDustLevelClass = (type: string) => {
  const value = environmentData.value.dustMonitoring?.[type] || 0
  const threshold = environmentData.value.dustMonitoring?.threshold?.[type] || 100
  
  const ratio = value / threshold
  if (ratio >= 1) return 'level-danger'
  if (ratio >= 0.8) return 'level-warning'
  if (ratio >= 0.6) return 'level-caution'
  return 'level-good'
}

const getDustStatusClass = (type: string) => {
  const status = getDustStatus(type)
  return `status--${status}`
}

const getDustLevelText = (type: string) => {
  const value = environmentData.value.dustMonitoring?.[type] || 0
  const threshold = environmentData.value.dustMonitoring?.threshold?.[type] || 100
  
  const ratio = value / threshold
  if (ratio >= 1) return '严重污染'
  if (ratio >= 0.8) return '重度污染'
  if (ratio >= 0.6) return '中度污染'
  if (ratio >= 0.4) return '轻度污染'
  return '优良'
}

const getAlertLevelClass = () => {
  const level = environmentData.value.dustMonitoring?.alertLevel || 'Normal'
  return level === 'Normal' ? 'status--online' : 'status--warning'
}

const getNoiseColor = () => {
  const level = environmentData.value.noiseMonitoring?.currentLevel || 0
  if (level >= 80) return '#e74c3c'
  if (level >= 70) return '#f39c12'
  if (level >= 60) return '#3498db'
  return '#2ecc71'
}

const getNoiseProgress = () => {
  const circumference = 2 * Math.PI * 52
  return circumference
}

const getNoiseOffset = () => {
  const level = environmentData.value.noiseMonitoring?.currentLevel || 0
  const maxLevel = 100 // 假设最大值为100dB
  const progress = Math.min(level / maxLevel, 1)
  const circumference = 2 * Math.PI * 52
  return circumference * (1 - progress)
}

const getAlertTypeText = (type: string) => {
  const typeMap: Record<string, string> = {
    'dust': '扬尘',
    'noise': '噪音',
    'weather': '气象'
  }
  return typeMap[type] || type
}

const getAlertTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    'dust': '#e74c3c',
    'noise': '#9b59b6',
    'weather': '#3498db'
  }
  return colorMap[type] || '#95a5a6'
}

const getAlertTypePercentage = (count: number) => {
  const total = Object.values(environmentAlerts.value.alertStatistics?.alertTypes || {}).reduce((sum: number, val: any) => sum + val, 0)
  return total > 0 ? (count / total) * 100 : 0
}

// 初始化数据
const initData = async () => {
  await Promise.all([
    loadEnvironmentData(),
    loadMonitoringPoints(),
    loadEnvironmentTrends(),
    loadEnvironmentAlerts()
  ])
}

// 启动数据更新
const startDataUpdate = () => {
  dataTimer = setInterval(() => {
    loadEnvironmentData()
    loadEnvironmentAlerts()
  }, 60000) // 60秒更新一次
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
.environment-monitor-screen {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  min-height: 100vh;
}

// 概览区域
.overview-section {
  .overview-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 16px;
  }
}

// 主要内容区域
.main-content {
  .content-grid {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 20px;
  }
  
  .dust-section {
    .dust-card {
      .dust-metrics {
        .dust-gauge {
          margin-bottom: 20px;
          
          .gauge-container {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 16px;
            margin-bottom: 16px;
            
            .gauge-item {
              text-align: center;
              
              .gauge-circle {
                width: 80px;
                height: 80px;
                border-radius: 50%;
                display: flex;
                flex-direction: column;
                align-items: center;
                justify-content: center;
                margin: 0 auto 8px;
                border: 3px solid;
                position: relative;
                
                &.level-good {
                  border-color: #2ecc71;
                  background: rgba(46, 204, 113, 0.1);
                }
                
                &.level-caution {
                  border-color: #3498db;
                  background: rgba(52, 152, 219, 0.1);
                }
                
                &.level-warning {
                  border-color: #f39c12;
                  background: rgba(243, 156, 18, 0.1);
                }
                
                &.level-danger {
                  border-color: #e74c3c;
                  background: rgba(231, 76, 60, 0.1);
                }
                
                .gauge-value {
                  .value {
                    font-size: 16px;
                    font-weight: 700;
                    color: #ffffff;
                    line-height: 1;
                  }
                  
                  .unit {
                    font-size: 8px;
                    color: #7f8c8d;
                  }
                }
                
                .gauge-label {
                  font-size: 10px;
                  color: #7f8c8d;
                  position: absolute;
                  bottom: 8px;
                }
              }
              
              .gauge-status {
                font-size: 11px;
                font-weight: 600;
                padding: 2px 6px;
                border-radius: 8px;
                
                &.status--online {
                  background: rgba(46, 204, 113, 0.2);
                  color: #2ecc71;
                }
                
                &.status--warning {
                  background: rgba(243, 156, 18, 0.2);
                  color: #f39c12;
                }
                
                &.status--offline {
                  background: rgba(231, 76, 60, 0.2);
                  color: #e74c3c;
                }
              }
            }
          }
          
          .dust-summary {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 12px;
            padding: 12px;
            border-radius: 6px;
            background: rgba(255, 255, 255, 0.05);
            
            .summary-item {
              display: flex;
              justify-content: space-between;
              align-items: center;
              
              .summary-label {
                font-size: 12px;
                color: #7f8c8d;
              }
              
              .summary-value {
                font-size: 12px;
                font-weight: 600;
              }
            }
          }
        }
        
        .dust-trends {
          .trends-title {
            font-size: 14px;
            font-weight: 600;
            color: #3498db;
            margin: 0 0 12px 0;
          }
        }
      }
    }
  }
  
  .noise-section {
    .noise-card {
      .noise-metrics {
        .noise-gauge {
          margin-bottom: 20px;
          
          .circular-progress {
            text-align: center;
            margin-bottom: 16px;
            position: relative;
            
            .progress-ring {
              transform: rotate(-90deg);
              
              .progress-ring-progress {
                transition: stroke-dashoffset 0.5s ease;
              }
            }
            
            .progress-content {
              position: absolute;
              top: 50%;
              left: 50%;
              transform: translate(-50%, -50%);
              
              .progress-value {
                font-size: 20px;
                font-weight: 700;
                color: #ffffff;
                line-height: 1;
                display: block;
              }
              
              .progress-unit {
                font-size: 12px;
                color: #7f8c8d;
              }
            }
          }
          
          .noise-info {
            .noise-levels {
              display: grid;
              grid-template-columns: repeat(3, 1fr);
              gap: 8px;
              margin-bottom: 12px;
              
              .level-item {
                text-align: center;
                padding: 8px;
                border-radius: 4px;
                background: rgba(255, 255, 255, 0.05);
                
                .level-label {
                  font-size: 10px;
                  color: #7f8c8d;
                  display: block;
                  margin-bottom: 4px;
                }
                
                .level-value {
                  font-size: 12px;
                  font-weight: 600;
                  color: #ffffff;
                }
              }
            }
            
            .noise-thresholds {
              display: grid;
              grid-template-columns: 1fr 1fr;
              gap: 8px;
              
              .threshold-item {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 6px 8px;
                border-radius: 4px;
                background: rgba(255, 255, 255, 0.03);
                
                .threshold-label {
                  font-size: 11px;
                  color: #7f8c8d;
                }
                
                .threshold-value {
                  font-size: 11px;
                  font-weight: 600;
                  color: #3498db;
                }
              }
            }
          }
        }
        
        .noise-trends {
          .trends-title {
            font-size: 14px;
            font-weight: 600;
            color: #3498db;
            margin: 0 0 12px 0;
          }
        }
      }
    }
  }
  
  .weather-section {
    .weather-card {
      .weather-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 12px;
        
        .weather-item {
          display: flex;
          align-items: center;
          gap: 12px;
          padding: 12px;
          border-radius: 6px;
          background: rgba(255, 255, 255, 0.05);
          border: 1px solid rgba(255, 255, 255, 0.1);
          
          .weather-icon {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            background: rgba(52, 152, 219, 0.2);
            display: flex;
            align-items: center;
            justify-content: center;
            
            .el-icon {
              font-size: 16px;
              color: #3498db;
            }
          }
          
          .weather-content {
            flex: 1;
            
            .weather-label {
              font-size: 11px;
              color: #7f8c8d;
              margin-bottom: 2px;
            }
            
            .weather-value {
              font-size: 14px;
              font-weight: 600;
              color: #ffffff;
            }
          }
        }
      }
    }
  }
}

// 底部区域
.bottom-section {
  .bottom-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 20px;
  }
  
  .monitoring-points-card {
    .points-list {
      max-height: 300px;
      overflow-y: auto;
      
      .point-item {
        padding: 12px;
        margin-bottom: 8px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        cursor: pointer;
        transition: all 0.3s ease;
        
        &:hover {
          background: rgba(255, 255, 255, 0.08);
          border-color: #3498db;
        }
        
        &.active {
          background: rgba(52, 152, 219, 0.2);
          border-color: #3498db;
        }
        
        .point-header {
          display: flex;
          align-items: center;
          gap: 12px;
          margin-bottom: 8px;
          
          .point-icon {
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
          
          .point-info {
            flex: 1;
            
            .point-name {
              font-size: 13px;
              font-weight: 600;
              color: #ffffff;
              margin-bottom: 2px;
            }
            
            .point-zone {
              font-size: 11px;
              color: #7f8c8d;
            }
          }
          
          .point-status {
            font-size: 10px;
            font-weight: 600;
            padding: 2px 6px;
            border-radius: 8px;
            
            &.status--online {
              background: rgba(46, 204, 113, 0.2);
              color: #2ecc71;
            }
            
            &.status--offline {
              background: rgba(231, 76, 60, 0.2);
              color: #e74c3c;
            }
          }
        }
        
        .point-sensors {
          display: grid;
          grid-template-columns: repeat(3, 1fr);
          gap: 8px;
          
          .sensor-item {
            text-align: center;
            padding: 4px;
            border-radius: 4px;
            background: rgba(255, 255, 255, 0.03);
            
            .sensor-label {
              font-size: 9px;
              color: #7f8c8d;
              display: block;
              margin-bottom: 2px;
            }
            
            .sensor-value {
              font-size: 10px;
              font-weight: 600;
              color: #ffffff;
            }
          }
        }
      }
    }
  }
  
  .alerts-card {
    .alerts-content {
      .alerts-summary {
        margin-bottom: 16px;
        
        .summary-stats {
          display: grid;
          grid-template-columns: repeat(3, 1fr);
          gap: 12px;
          margin-bottom: 12px;
          
          .stat-item {
            text-align: center;
            padding: 8px;
            border-radius: 6px;
            background: rgba(255, 255, 255, 0.05);
            
            .stat-value {
              font-size: 16px;
              font-weight: 700;
              color: #ffffff;
              display: block;
            }
            
            .stat-label {
              font-size: 10px;
              color: #7f8c8d;
            }
          }
        }
        
        .alert-types {
          .type-item {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 6px;
            
            .type-label {
              font-size: 11px;
              color: #7f8c8d;
              width: 40px;
            }
            
            .type-count {
              font-size: 11px;
              font-weight: 600;
              color: #ffffff;
              width: 20px;
            }
            
            .type-bar {
              flex: 1;
              height: 4px;
              background: rgba(255, 255, 255, 0.1);
              border-radius: 2px;
              overflow: hidden;
              
              .type-fill {
                height: 100%;
                transition: width 0.3s ease;
              }
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
          max-height: 150px;
          overflow-y: auto;
          
          .alert-item {
            display: flex;
            align-items: flex-start;
            gap: 8px;
            padding: 8px;
            margin-bottom: 6px;
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
              
              .alert-location {
                font-size: 10px;
                color: #3498db;
                margin-bottom: 2px;
              }
              
              .alert-value {
                font-size: 9px;
                color: #7f8c8d;
                margin-bottom: 2px;
              }
              
              .alert-time {
                font-size: 9px;
                color: #95a5a6;
              }
            }
            
            .alert-status {
              font-size: 9px;
              font-weight: 600;
              padding: 2px 4px;
              border-radius: 4px;
              
              &.status--active {
                background: rgba(231, 76, 60, 0.2);
                color: #e74c3c;
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
    grid-template-columns: 1fr 1fr;
  }
  
  .bottom-section .bottom-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
}

@media (max-width: 768px) {
  .environment-monitor-screen {
    padding: 12px;
  }
  
  .overview-section .overview-grid {
    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
    gap: 12px;
  }
  
  .main-content .content-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
  
  .weather-section .weather-grid {
    grid-template-columns: 1fr;
  }
}
</style>