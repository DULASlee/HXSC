<template>
  <div class="environment-monitor-screen">
    <div class="screen-header">
      <h2 class="screen-title">环境监测管理</h2>
      <div class="screen-controls">
        <el-button type="primary" size="small" @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新数据
        </el-button>
      </div>
    </div>

    <div class="screen-content">
      <!-- 环境质量概览 -->
      <div class="quality-overview">
        <div class="overview-card" :class="getQualityLevel(environmentData.aqi)">
          <div class="overview-header">
            <h3>空气质量指数</h3>
            <span class="quality-level">{{ getQualityText(environmentData.aqi) }}</span>
          </div>
          <div class="overview-value">{{ environmentData.aqi }}</div>
          <div class="overview-label">AQI</div>
        </div>
      </div>

      <!-- 实时监测数据 -->
      <div class="monitoring-grid">
        <div class="monitor-card">
          <div class="monitor-header">
            <el-icon color="#409eff"><Cloudy /></el-icon>
            <span class="monitor-title">PM2.5</span>
          </div>
          <div class="monitor-value" :class="getPMLevel(environmentData.pm25)">
            {{ environmentData.pm25 }} μg/m³
          </div>
          <div class="monitor-status">
            <span class="status-text">{{ getPMStatus(environmentData.pm25) }}</span>
          </div>
        </div>

        <div class="monitor-card">
          <div class="monitor-header">
            <el-icon color="#67c23a"><Cloudy /></el-icon>
            <span class="monitor-title">PM10</span>
          </div>
          <div class="monitor-value" :class="getPMLevel(environmentData.pm10)">
            {{ environmentData.pm10 }} μg/m³
          </div>
          <div class="monitor-status">
            <span class="status-text">{{ getPMStatus(environmentData.pm10) }}</span>
          </div>
        </div>

        <div class="monitor-card">
          <div class="monitor-header">
            <el-icon color="#e6a23c"><Sunny /></el-icon>
            <span class="monitor-title">温度</span>
          </div>
          <div class="monitor-value">
            {{ environmentData.temperature }}°C
          </div>
          <div class="monitor-status">
            <span class="status-text">{{ getTemperatureStatus(environmentData.temperature) }}</span>
          </div>
        </div>

        <div class="monitor-card">
          <div class="monitor-header">
            <el-icon color="#409eff"><Drizzling /></el-icon>
            <span class="monitor-title">湿度</span>
          </div>
          <div class="monitor-value">
            {{ environmentData.humidity }}%
          </div>
          <div class="monitor-status">
            <span class="status-text">{{ getHumidityStatus(environmentData.humidity) }}</span>
          </div>
        </div>

        <div class="monitor-card">
          <div class="monitor-header">
            <el-icon color="#f56c6c"><Bell /></el-icon>
            <span class="monitor-title">噪音</span>
          </div>
          <div class="monitor-value" :class="getNoiseLevel(environmentData.noise)">
            {{ environmentData.noise }} dB
          </div>
          <div class="monitor-status">
            <span class="status-text">{{ getNoiseStatus(environmentData.noise) }}</span>
          </div>
        </div>

        <div class="monitor-card">
          <div class="monitor-header">
            <el-icon color="#909399"><WindPower /></el-icon>
            <span class="monitor-title">风速</span>
          </div>
          <div class="monitor-value">
            {{ environmentData.windSpeed }} m/s
          </div>
          <div class="monitor-status">
            <span class="status-text">{{ getWindStatus(environmentData.windSpeed) }}</span>
          </div>
        </div>
      </div>

      <!-- 监测点分布 -->
      <div class="monitoring-points">
        <div class="section-header">
          <h3>监测点状态</h3>
        </div>
        
        <div class="points-grid">
          <div 
            v-for="point in monitoringPoints" 
            :key="point.id"
            class="point-card"
            :class="{ 'offline': !point.isOnline }"
          >
            <div class="point-header">
              <span class="point-name">{{ point.name }}</span>
              <span class="point-status" :class="{ 'online': point.isOnline, 'offline': !point.isOnline }">
                {{ point.isOnline ? '在线' : '离线' }}
              </span>
            </div>
            
            <div class="point-location">
              <el-icon><Location /></el-icon>
              <span>{{ point.location }}</span>
            </div>
            
            <div class="point-data" v-if="point.isOnline">
              <div class="data-item">
                <span class="data-label">PM2.5:</span>
                <span class="data-value">{{ point.data.pm25 }} μg/m³</span>
              </div>
              <div class="data-item">
                <span class="data-label">温度:</span>
                <span class="data-value">{{ point.data.temperature }}°C</span>
              </div>
              <div class="data-item">
                <span class="data-label">湿度:</span>
                <span class="data-value">{{ point.data.humidity }}%</span>
              </div>
              <div class="data-item">
                <span class="data-label">噪音:</span>
                <span class="data-value">{{ point.data.noise }} dB</span>
              </div>
            </div>
            
            <div class="point-offline" v-else>
              <el-icon color="#f56c6c"><Warning /></el-icon>
              <span>设备离线</span>
            </div>
          </div>
        </div>
      </div>

      <!-- 历史趋势 -->
      <div class="trend-section">
        <div class="section-header">
          <h3>24小时趋势</h3>
        </div>
        
        <div class="trend-cards">
          <div class="trend-card">
            <div class="trend-header">
              <span class="trend-title">PM2.5趋势</span>
            </div>
            <div class="trend-chart">
              <div class="chart-placeholder">
                <el-icon size="32" color="#909399"><TrendCharts /></el-icon>
                <p>图表数据加载中...</p>
              </div>
            </div>
          </div>
          
          <div class="trend-card">
            <div class="trend-header">
              <span class="trend-title">温湿度趋势</span>
            </div>
            <div class="trend-chart">
              <div class="chart-placeholder">
                <el-icon size="32" color="#909399"><TrendCharts /></el-icon>
                <p>图表数据加载中...</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { 
  Refresh, 
  Cloudy, 
  Sunny, 
  Drizzling, 
  Bell, 
  WindPower, 
  Location, 
  Warning, 
  TrendCharts 
} from '@element-plus/icons-vue'

interface EnvironmentData {
  aqi: number
  pm25: number
  pm10: number
  temperature: number
  humidity: number
  noise: number
  windSpeed: number
}

interface MonitoringPoint {
  id: string
  name: string
  location: string
  isOnline: boolean
  data: {
    pm25: number
    temperature: number
    humidity: number
    noise: number
  }
}

// 响应式数据
const environmentData = ref<EnvironmentData>({
  aqi: 0,
  pm25: 0,
  pm10: 0,
  temperature: 0,
  humidity: 0,
  noise: 0,
  windSpeed: 0
})

const monitoringPoints = ref<MonitoringPoint[]>([])

// 模拟监测点数据
const mockMonitoringPoints: MonitoringPoint[] = [
  {
    id: '1',
    name: '监测点A',
    location: 'A区施工现场入口',
    isOnline: true,
    data: {
      pm25: 45,
      temperature: 28,
      humidity: 65,
      noise: 72
    }
  },
  {
    id: '2',
    name: '监测点B',
    location: 'B区施工现场中心',
    isOnline: true,
    data: {
      pm25: 52,
      temperature: 27,
      humidity: 68,
      noise: 78
    }
  },
  {
    id: '3',
    name: '监测点C',
    location: 'C区施工现场出口',
    isOnline: false,
    data: {
      pm25: 0,
      temperature: 0,
      humidity: 0,
      noise: 0
    }
  },
  {
    id: '4',
    name: '监测点D',
    location: '材料堆放区',
    isOnline: true,
    data: {
      pm25: 38,
      temperature: 26,
      humidity: 62,
      noise: 68
    }
  }
]

// 刷新数据
const refreshData = () => {
  loadEnvironmentData()
  loadMonitoringPoints()
}

// 加载环境数据
const loadEnvironmentData = () => {
  const pm25 = Math.floor(Math.random() * 50) + 30
  const pm10 = Math.floor(Math.random() * 80) + 50
  const temperature = Math.floor(Math.random() * 15) + 20
  const humidity = Math.floor(Math.random() * 30) + 50
  const noise = Math.floor(Math.random() * 20) + 65
  const windSpeed = Math.floor(Math.random() * 10) + 5
  
  environmentData.value = {
    aqi: Math.floor((pm25 + pm10) / 2),
    pm25,
    pm10,
    temperature,
    humidity,
    noise,
    windSpeed
  }
}

// 加载监测点数据
const loadMonitoringPoints = () => {
  monitoringPoints.value = mockMonitoringPoints.map(point => ({
    ...point,
    data: point.isOnline ? {
      pm25: Math.floor(Math.random() * 30) + 30,
      temperature: Math.floor(Math.random() * 10) + 22,
      humidity: Math.floor(Math.random() * 20) + 55,
      noise: Math.floor(Math.random() * 15) + 65
    } : point.data
  }))
}

// 获取空气质量等级
const getQualityLevel = (aqi: number) => {
  if (aqi <= 50) return 'excellent'
  if (aqi <= 100) return 'good'
  if (aqi <= 150) return 'moderate'
  if (aqi <= 200) return 'poor'
  return 'hazardous'
}

// 获取空气质量文本
const getQualityText = (aqi: number) => {
  if (aqi <= 50) return '优'
  if (aqi <= 100) return '良'
  if (aqi <= 150) return '轻度污染'
  if (aqi <= 200) return '中度污染'
  return '重度污染'
}

// 获取PM等级
const getPMLevel = (pm: number) => {
  if (pm <= 35) return 'good'
  if (pm <= 75) return 'moderate'
  return 'poor'
}

// 获取PM状态
const getPMStatus = (pm: number) => {
  if (pm <= 35) return '优良'
  if (pm <= 75) return '轻度污染'
  return '重度污染'
}

// 获取温度状态
const getTemperatureStatus = (temp: number) => {
  if (temp < 10) return '较低'
  if (temp < 25) return '适宜'
  if (temp < 35) return '较高'
  return '过高'
}

// 获取湿度状态
const getHumidityStatus = (humidity: number) => {
  if (humidity < 40) return '干燥'
  if (humidity < 70) return '适宜'
  return '潮湿'
}

// 获取噪音等级
const getNoiseLevel = (noise: number) => {
  if (noise <= 60) return 'good'
  if (noise <= 80) return 'moderate'
  return 'poor'
}

// 获取噪音状态
const getNoiseStatus = (noise: number) => {
  if (noise <= 60) return '安静'
  if (noise <= 80) return '适中'
  return '嘈杂'
}

// 获取风速状态
const getWindStatus = (windSpeed: number) => {
  if (windSpeed < 5) return '微风'
  if (windSpeed < 10) return '和风'
  if (windSpeed < 15) return '强风'
  return '大风'
}

// 初始化
onMounted(() => {
  refreshData()
})
</script>

<style scoped>
.environment-monitor-screen {
  height: 100%;
  display: flex;
  flex-direction: column;
  background: #0a0a0a;
  color: #ffffff;
}

.screen-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 20px;
  background: rgba(255, 255, 255, 0.05);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.screen-title {
  margin: 0;
  font-size: 24px;
  font-weight: 600;
  color: #ffffff;
}

.screen-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 20px;
  gap: 20px;
  overflow-y: auto;
}

.quality-overview {
  display: flex;
  justify-content: center;
  margin-bottom: 20px;
}

.overview-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 12px;
  padding: 30px;
  text-align: center;
  border: 2px solid;
  min-width: 200px;
}

.overview-card.excellent {
  border-color: #67c23a;
  background: rgba(103, 194, 58, 0.1);
}

.overview-card.good {
  border-color: #409eff;
  background: rgba(64, 158, 255, 0.1);
}

.overview-card.moderate {
  border-color: #e6a23c;
  background: rgba(230, 162, 60, 0.1);
}

.overview-card.poor {
  border-color: #f56c6c;
  background: rgba(245, 108, 108, 0.1);
}

.overview-card.hazardous {
  border-color: #909399;
  background: rgba(144, 147, 153, 0.1);
}

.overview-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.overview-header h3 {
  margin: 0;
  font-size: 16px;
  color: #ffffff;
}

.quality-level {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
  background: rgba(255, 255, 255, 0.2);
}

.overview-value {
  font-size: 48px;
  font-weight: 600;
  color: #ffffff;
  line-height: 1;
  margin-bottom: 8px;
}

.overview-label {
  font-size: 14px;
  color: #909399;
}

.monitoring-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.monitor-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 20px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  text-align: center;
}

.monitor-header {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 16px;
  gap: 8px;
}

.monitor-title {
  font-weight: 600;
  color: #ffffff;
}

.monitor-value {
  font-size: 28px;
  font-weight: 600;
  color: #ffffff;
  margin-bottom: 8px;
}

.monitor-value.good {
  color: #67c23a;
}

.monitor-value.moderate {
  color: #e6a23c;
}

.monitor-value.poor {
  color: #f56c6c;
}

.monitor-status {
  font-size: 14px;
  color: #909399;
}

.status-text {
  padding: 4px 8px;
  border-radius: 4px;
  background: rgba(255, 255, 255, 0.1);
}

.monitoring-points,
.trend-section {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 20px;
}

.section-header {
  margin-bottom: 20px;
}

.section-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #ffffff;
}

.points-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.point-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.point-card.offline {
  opacity: 0.6;
  border-color: #f56c6c;
}

.point-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.point-name {
  font-weight: 600;
  color: #ffffff;
}

.point-status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.point-status.online {
  background: rgba(103, 194, 58, 0.2);
  color: #67c23a;
}

.point-status.offline {
  background: rgba(245, 108, 108, 0.2);
  color: #f56c6c;
}

.point-location {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 12px;
  font-size: 14px;
  color: #909399;
}

.point-data {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 8px;
}

.data-item {
  display: flex;
  justify-content: space-between;
  font-size: 14px;
}

.data-label {
  color: #909399;
}

.data-value {
  color: #ffffff;
  font-weight: 500;
}

.point-offline {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  color: #f56c6c;
  font-size: 14px;
}

.trend-cards {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
}

.trend-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.trend-header {
  margin-bottom: 16px;
}

.trend-title {
  font-weight: 600;
  color: #ffffff;
}

.trend-chart {
  height: 200px;
  background: rgba(255, 255, 255, 0.02);
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.chart-placeholder {
  text-align: center;
  color: #909399;
}

.chart-placeholder p {
  margin: 10px 0 0 0;
  font-size: 14px;
}
</style> 