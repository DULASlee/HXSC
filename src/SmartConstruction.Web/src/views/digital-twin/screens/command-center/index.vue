<template>
  <div class="command-center-screen">
    <div class="screen-header">
      <h2 class="screen-title">指挥中心</h2>
      <div class="screen-controls">
        <el-button type="primary" size="small" @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新数据
        </el-button>
      </div>
    </div>

    <div class="screen-content">
      <!-- 主要指标卡片 -->
      <div class="metrics-grid">
        <div class="metric-card">
          <div class="metric-icon">
            <el-icon color="#409eff"><User /></el-icon>
          </div>
          <div class="metric-content">
            <div class="metric-value">{{ metrics.totalWorkers }}</div>
            <div class="metric-label">在场工人</div>
          </div>
        </div>
        
        <div class="metric-card">
          <div class="metric-icon">
            <el-icon color="#67c23a"><CircleCheckFilled /></el-icon>
          </div>
          <div class="metric-content">
            <div class="metric-value">{{ metrics.onlineDevices }}</div>
            <div class="metric-label">在线设备</div>
          </div>
        </div>
        
        <div class="metric-card">
          <div class="metric-icon">
            <el-icon color="#e6a23c"><Warning /></el-icon>
          </div>
          <div class="metric-content">
            <div class="metric-value">{{ metrics.activeAlerts }}</div>
            <div class="metric-label">活跃警报</div>
          </div>
        </div>
        
        <div class="metric-card">
          <div class="metric-icon">
            <el-icon color="#f56c6c"><TrendCharts /></el-icon>
          </div>
          <div class="metric-content">
            <div class="metric-value">{{ metrics.completionRate }}%</div>
            <div class="metric-label">项目进度</div>
          </div>
        </div>
      </div>

      <!-- 实时监控区域 -->
      <div class="monitoring-section">
        <div class="section-header">
          <h3>实时监控</h3>
        </div>
        
        <div class="monitoring-grid">
          <!-- 环境监测 -->
          <div class="monitoring-card">
            <div class="card-header">
              <span class="card-title">环境监测</span>
              <span class="card-status" :class="{ 'good': environmentStatus === 'good', 'warning': environmentStatus === 'warning', 'danger': environmentStatus === 'danger' }">
                {{ getStatusText(environmentStatus) }}
              </span>
            </div>
            <div class="card-content">
              <div class="env-item">
                <span class="env-label">PM2.5:</span>
                <span class="env-value">{{ environmentData.pm25 }} μg/m³</span>
              </div>
              <div class="env-item">
                <span class="env-label">温度:</span>
                <span class="env-value">{{ environmentData.temperature }}°C</span>
              </div>
              <div class="env-item">
                <span class="env-label">湿度:</span>
                <span class="env-value">{{ environmentData.humidity }}%</span>
              </div>
              <div class="env-item">
                <span class="env-label">噪音:</span>
                <span class="env-value">{{ environmentData.noise }} dB</span>
              </div>
            </div>
          </div>

          <!-- 设备状态 -->
          <div class="monitoring-card">
            <div class="card-header">
              <span class="card-title">设备状态</span>
              <span class="card-status" :class="{ 'good': deviceStatus === 'good', 'warning': deviceStatus === 'warning', 'danger': deviceStatus === 'danger' }">
                {{ getStatusText(deviceStatus) }}
              </span>
            </div>
            <div class="card-content">
              <div class="device-item">
                <span class="device-label">塔吊:</span>
                <span class="device-value">{{ deviceData.cranes }}/{{ deviceData.totalCranes }} 台运行</span>
              </div>
              <div class="device-item">
                <span class="device-label">升降机:</span>
                <span class="device-value">{{ deviceData.elevators }}/{{ deviceData.totalElevators }} 台运行</span>
              </div>
              <div class="device-item">
                <span class="device-label">摄像头:</span>
                <span class="device-value">{{ deviceData.cameras }}/{{ deviceData.totalCameras }} 台在线</span>
              </div>
              <div class="device-item">
                <span class="device-label">传感器:</span>
                <span class="device-value">{{ deviceData.sensors }}/{{ deviceData.totalSensors }} 个正常</span>
              </div>
            </div>
          </div>

          <!-- 安全状态 -->
          <div class="monitoring-card">
            <div class="card-header">
              <span class="card-title">安全状态</span>
              <span class="card-status" :class="{ 'good': safetyStatus === 'good', 'warning': safetyStatus === 'warning', 'danger': safetyStatus === 'danger' }">
                {{ getStatusText(safetyStatus) }}
              </span>
            </div>
            <div class="card-content">
              <div class="safety-item">
                <span class="safety-label">安全帽佩戴:</span>
                <span class="safety-value">{{ safetyData.helmetRate }}%</span>
              </div>
              <div class="safety-item">
                <span class="safety-label">安全带使用:</span>
                <span class="safety-value">{{ safetyData.safetyBeltRate }}%</span>
              </div>
              <div class="safety-item">
                <span class="safety-label">违规行为:</span>
                <span class="safety-value">{{ safetyData.violations }} 起</span>
              </div>
              <div class="safety-item">
                <span class="safety-label">安全评分:</span>
                <span class="safety-value">{{ safetyData.safetyScore }}/100</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 项目进度 -->
      <div class="progress-section">
        <div class="section-header">
          <h3>项目进度</h3>
        </div>
        
        <div class="progress-grid">
          <div class="progress-card">
            <div class="progress-header">
              <span class="progress-title">A区施工</span>
              <span class="progress-percentage">{{ progressData.areaA }}%</span>
            </div>
            <el-progress 
              :percentage="progressData.areaA" 
              :color="getProgressColor(progressData.areaA)"
              :stroke-width="8"
            />
          </div>
          
          <div class="progress-card">
            <div class="progress-header">
              <span class="progress-title">B区施工</span>
              <span class="progress-percentage">{{ progressData.areaB }}%</span>
            </div>
            <el-progress 
              :percentage="progressData.areaB" 
              :color="getProgressColor(progressData.areaB)"
              :stroke-width="8"
            />
          </div>
          
          <div class="progress-card">
            <div class="progress-header">
              <span class="progress-title">C区施工</span>
              <span class="progress-percentage">{{ progressData.areaC }}%</span>
            </div>
            <el-progress 
              :percentage="progressData.areaC" 
              :color="getProgressColor(progressData.areaC)"
              :stroke-width="8"
            />
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
  User, 
  CircleCheckFilled, 
  Warning, 
  TrendCharts 
} from '@element-plus/icons-vue'

interface Metrics {
  totalWorkers: number
  onlineDevices: number
  activeAlerts: number
  completionRate: number
}

interface EnvironmentData {
  pm25: number
  temperature: number
  humidity: number
  noise: number
}

interface DeviceData {
  cranes: number
  totalCranes: number
  elevators: number
  totalElevators: number
  cameras: number
  totalCameras: number
  sensors: number
  totalSensors: number
}

interface SafetyData {
  helmetRate: number
  safetyBeltRate: number
  violations: number
  safetyScore: number
}

interface ProgressData {
  areaA: number
  areaB: number
  areaC: number
}

// 响应式数据
const metrics = ref<Metrics>({
  totalWorkers: 0,
  onlineDevices: 0,
  activeAlerts: 0,
  completionRate: 0
})

const environmentData = ref<EnvironmentData>({
  pm25: 0,
  temperature: 0,
  humidity: 0,
  noise: 0
})

const deviceData = ref<DeviceData>({
  cranes: 0,
  totalCranes: 0,
  elevators: 0,
  totalElevators: 0,
  cameras: 0,
  totalCameras: 0,
  sensors: 0,
  totalSensors: 0
})

const safetyData = ref<SafetyData>({
  helmetRate: 0,
  safetyBeltRate: 0,
  violations: 0,
  safetyScore: 0
})

const progressData = ref<ProgressData>({
  areaA: 0,
  areaB: 0,
  areaC: 0
})

const environmentStatus = ref<'good' | 'warning' | 'danger'>('good')
const deviceStatus = ref<'good' | 'warning' | 'danger'>('good')
const safetyStatus = ref<'good' | 'warning' | 'danger'>('good')

// 刷新数据
const refreshData = () => {
  loadMetrics()
  loadEnvironmentData()
  loadDeviceData()
  loadSafetyData()
  loadProgressData()
}

// 加载主要指标
const loadMetrics = () => {
  metrics.value = {
    totalWorkers: Math.floor(Math.random() * 100) + 50,
    onlineDevices: Math.floor(Math.random() * 20) + 15,
    activeAlerts: Math.floor(Math.random() * 5),
    completionRate: Math.floor(Math.random() * 30) + 60
  }
}

// 加载环境数据
const loadEnvironmentData = () => {
  const pm25 = Math.floor(Math.random() * 50) + 20
  environmentData.value = {
    pm25,
    temperature: Math.floor(Math.random() * 15) + 20,
    humidity: Math.floor(Math.random() * 30) + 50,
    noise: Math.floor(Math.random() * 20) + 60
  }
  
  // 根据PM2.5值判断环境状态
  if (pm25 < 35) {
    environmentStatus.value = 'good'
  } else if (pm25 < 75) {
    environmentStatus.value = 'warning'
  } else {
    environmentStatus.value = 'danger'
  }
}

// 加载设备数据
const loadDeviceData = () => {
  const totalCranes = 12
  const totalElevators = 6
  const totalCameras = 18
  const totalSensors = 9
  
  deviceData.value = {
    cranes: Math.floor(Math.random() * 3) + 10,
    totalCranes: totalCranes,
    elevators: Math.floor(Math.random() * 2) + 5,
    totalElevators: totalElevators,
    cameras: Math.floor(Math.random() * 3) + 16,
    totalCameras: totalCameras,
    sensors: Math.floor(Math.random() * 2) + 8,
    totalSensors: totalSensors
  }
  
  // 根据在线率判断设备状态
  const onlineRate = (deviceData.value.cranes + deviceData.value.elevators + deviceData.value.cameras + deviceData.value.sensors) / 
                     (totalCranes + totalElevators + totalCameras + totalSensors)
  
  if (onlineRate > 0.9) {
    deviceStatus.value = 'good'
  } else if (onlineRate > 0.7) {
    deviceStatus.value = 'warning'
  } else {
    deviceStatus.value = 'danger'
  }
}

// 加载安全数据
const loadSafetyData = () => {
  safetyData.value = {
    helmetRate: Math.floor(Math.random() * 20) + 80,
    safetyBeltRate: Math.floor(Math.random() * 15) + 85,
    violations: Math.floor(Math.random() * 3),
    safetyScore: Math.floor(Math.random() * 20) + 80
  }
  
  // 根据安全评分判断安全状态
  if (safetyData.value.safetyScore > 90) {
    safetyStatus.value = 'good'
  } else if (safetyData.value.safetyScore > 70) {
    safetyStatus.value = 'warning'
  } else {
    safetyStatus.value = 'danger'
  }
}

// 加载进度数据
const loadProgressData = () => {
  progressData.value = {
    areaA: Math.floor(Math.random() * 20) + 70,
    areaB: Math.floor(Math.random() * 15) + 60,
    areaC: Math.floor(Math.random() * 10) + 50
  }
}

// 获取状态文本
const getStatusText = (status: string) => {
  switch (status) {
    case 'good': return '良好'
    case 'warning': return '注意'
    case 'danger': return '危险'
    default: return '未知'
  }
}

// 获取进度条颜色
const getProgressColor = (percentage: number) => {
  if (percentage >= 80) return '#67c23a'
  if (percentage >= 60) return '#e6a23c'
  return '#f56c6c'
}

// 初始化
onMounted(() => {
  refreshData()
})
</script>

<style scoped>
.command-center-screen {
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

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.metric-card {
  display: flex;
  align-items: center;
  padding: 20px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.metric-icon {
  margin-right: 16px;
  font-size: 32px;
}

.metric-content {
  flex: 1;
}

.metric-value {
  font-size: 28px;
  font-weight: 600;
  color: #ffffff;
  line-height: 1;
}

.metric-label {
  font-size: 14px;
  color: #909399;
  margin-top: 4px;
}

.monitoring-section,
.progress-section {
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

.monitoring-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.monitoring-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  overflow: hidden;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.05);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.card-title {
  font-weight: 600;
  color: #ffffff;
}

.card-status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.card-status.good {
  background: rgba(103, 194, 58, 0.2);
  color: #67c23a;
}

.card-status.warning {
  background: rgba(230, 162, 60, 0.2);
  color: #e6a23c;
}

.card-status.danger {
  background: rgba(245, 108, 108, 0.2);
  color: #f56c6c;
}

.card-content {
  padding: 16px;
}

.env-item,
.device-item,
.safety-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.env-item:last-child,
.device-item:last-child,
.safety-item:last-child {
  margin-bottom: 0;
}

.env-label,
.device-label,
.safety-label {
  color: #909399;
}

.env-value,
.device-value,
.safety-value {
  color: #ffffff;
  font-weight: 500;
}

.progress-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.progress-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.progress-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.progress-title {
  font-weight: 600;
  color: #ffffff;
}

.progress-percentage {
  font-weight: 600;
  color: #409eff;
}
</style> 