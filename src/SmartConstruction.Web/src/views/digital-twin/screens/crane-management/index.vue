<template>
  <div class="crane-management-screen">
    <div class="screen-header">
      <h2 class="screen-title">塔吊升降机管理</h2>
      <div class="screen-controls">
        <el-button type="primary" size="small" @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新数据
        </el-button>
      </div>
    </div>

    <div class="screen-content">
      <!-- 设备概览 -->
      <div class="overview-grid">
        <div class="overview-card">
          <div class="overview-icon">
                         <el-icon color="#409eff"><Warning /></el-icon>
          </div>
          <div class="overview-content">
            <div class="overview-value">{{ overview.totalCranes }}</div>
            <div class="overview-label">塔吊总数</div>
          </div>
        </div>
        
        <div class="overview-card">
          <div class="overview-icon">
            <el-icon color="#67c23a"><CircleCheckFilled /></el-icon>
          </div>
          <div class="overview-content">
            <div class="overview-value">{{ overview.runningCranes }}</div>
            <div class="overview-label">运行中</div>
          </div>
        </div>
        
        <div class="overview-card">
          <div class="overview-icon">
            <el-icon color="#e6a23c"><Warning /></el-icon>
          </div>
          <div class="overview-content">
            <div class="overview-value">{{ overview.warningCranes }}</div>
            <div class="overview-label">预警设备</div>
          </div>
        </div>
        
        <div class="overview-card">
          <div class="overview-icon">
            <el-icon color="#f56c6c"><AlarmClock /></el-icon>
          </div>
          <div class="overview-content">
            <div class="overview-value">{{ overview.maintenanceCranes }}</div>
            <div class="overview-label">维护中</div>
          </div>
        </div>
      </div>

      <!-- 设备状态监控 -->
      <div class="device-monitoring">
        <div class="section-header">
          <h3>设备状态监控</h3>
        </div>
        
        <div class="device-grid">
          <div 
            v-for="device in devices" 
            :key="device.id"
            class="device-card"
            :class="{ 
              'warning': device.status === 'warning',
              'danger': device.status === 'danger',
              'maintenance': device.status === 'maintenance'
            }"
          >
            <div class="device-header">
              <span class="device-name">{{ device.name }}</span>
              <span class="device-status" :class="device.status">
                {{ getStatusText(device.status) }}
              </span>
            </div>
            
            <div class="device-info">
              <div class="info-row">
                <span class="info-label">位置:</span>
                <span class="info-value">{{ device.location }}</span>
              </div>
              <div class="info-row">
                <span class="info-label">负载:</span>
                <span class="info-value">{{ device.load }}%</span>
              </div>
              <div class="info-row">
                <span class="info-label">高度:</span>
                <span class="info-value">{{ device.height }}m</span>
              </div>
              <div class="info-row">
                <span class="info-label">风速:</span>
                <span class="info-value">{{ device.windSpeed }}m/s</span>
              </div>
            </div>
            
            <div class="device-alerts" v-if="device.alerts.length > 0">
              <div class="alert-item" v-for="alert in device.alerts" :key="alert.id">
                <el-icon color="#f56c6c"><Warning /></el-icon>
                <span class="alert-text">{{ alert.message }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 安全监控 -->
      <div class="safety-monitoring">
        <div class="section-header">
          <h3>安全监控</h3>
        </div>
        
        <div class="safety-grid">
          <div class="safety-card">
            <div class="safety-header">
              <span class="safety-title">超载监控</span>
              <span class="safety-count">{{ safetyData.overloadCount }}起</span>
            </div>
            <div class="safety-progress">
              <el-progress 
                :percentage="safetyData.overloadRate" 
                :color="getSafetyColor(safetyData.overloadRate)"
                :stroke-width="8"
              />
            </div>
          </div>
          
          <div class="safety-card">
            <div class="safety-header">
              <span class="safety-title">超高监控</span>
              <span class="safety-count">{{ safetyData.overheightCount }}起</span>
            </div>
            <div class="safety-progress">
              <el-progress 
                :percentage="safetyData.overheightRate" 
                :color="getSafetyColor(safetyData.overheightRate)"
                :stroke-width="8"
              />
            </div>
          </div>
          
          <div class="safety-card">
            <div class="safety-header">
              <span class="safety-title">强风预警</span>
              <span class="safety-count">{{ safetyData.windAlertCount }}起</span>
            </div>
            <div class="safety-progress">
              <el-progress 
                :percentage="safetyData.windAlertRate" 
                :color="getSafetyColor(safetyData.windAlertRate)"
                :stroke-width="8"
              />
            </div>
          </div>
        </div>
      </div>

      <!-- 工作效率 -->
      <div class="efficiency-section">
        <div class="section-header">
          <h3>工作效率</h3>
        </div>
        
        <div class="efficiency-grid">
          <div class="efficiency-card">
            <div class="efficiency-header">
              <span class="efficiency-title">平均工作时长</span>
            </div>
            <div class="efficiency-value">{{ efficiencyData.avgWorkHours }}小时</div>
          </div>
          
          <div class="efficiency-card">
            <div class="efficiency-header">
              <span class="efficiency-title">设备利用率</span>
            </div>
            <div class="efficiency-value">{{ efficiencyData.utilizationRate }}%</div>
          </div>
          
          <div class="efficiency-card">
            <div class="efficiency-header">
              <span class="efficiency-title">安全评分</span>
            </div>
            <div class="efficiency-value">{{ efficiencyData.safetyScore }}/100</div>
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
  CircleCheckFilled, 
  Warning, 
  AlarmClock 
} from '@element-plus/icons-vue'

interface DeviceOverview {
  totalCranes: number
  runningCranes: number
  warningCranes: number
  maintenanceCranes: number
}

interface Device {
  id: string
  name: string
  location: string
  status: 'normal' | 'warning' | 'danger' | 'maintenance'
  load: number
  height: number
  windSpeed: number
  alerts: Array<{
    id: string
    message: string
  }>
}

interface SafetyData {
  overloadCount: number
  overloadRate: number
  overheightCount: number
  overheightRate: number
  windAlertCount: number
  windAlertRate: number
}

interface EfficiencyData {
  avgWorkHours: number
  utilizationRate: number
  safetyScore: number
}

// 响应式数据
const overview = ref<DeviceOverview>({
  totalCranes: 0,
  runningCranes: 0,
  warningCranes: 0,
  maintenanceCranes: 0
})

const devices = ref<Device[]>([])
const safetyData = ref<SafetyData>({
  overloadCount: 0,
  overloadRate: 0,
  overheightCount: 0,
  overheightRate: 0,
  windAlertCount: 0,
  windAlertRate: 0
})

const efficiencyData = ref<EfficiencyData>({
  avgWorkHours: 0,
  utilizationRate: 0,
  safetyScore: 0
})

// 模拟设备数据
const mockDevices: Device[] = [
  {
    id: '1',
    name: '塔吊A-01',
    location: 'A区施工现场',
    status: 'normal',
    load: 65,
    height: 120,
    windSpeed: 8.5,
    alerts: []
  },
  {
    id: '2',
    name: '塔吊A-02',
    location: 'A区施工现场',
    status: 'warning',
    load: 85,
    height: 135,
    windSpeed: 12.0,
    alerts: [
      { id: '1', message: '负载接近安全上限' },
      { id: '2', message: '风速超过安全范围' }
    ]
  },
  {
    id: '3',
    name: '塔吊B-01',
    location: 'B区施工现场',
    status: 'normal',
    load: 45,
    height: 110,
    windSpeed: 6.2,
    alerts: []
  },
  {
    id: '4',
    name: '升降机C-01',
    location: 'C区施工现场',
    status: 'maintenance',
    load: 0,
    height: 0,
    windSpeed: 0,
    alerts: [
      { id: '3', message: '设备维护中' }
    ]
  },
  {
    id: '5',
    name: '塔吊B-02',
    location: 'B区施工现场',
    status: 'danger',
    load: 95,
    height: 150,
    windSpeed: 15.5,
    alerts: [
      { id: '4', message: '严重超载警告' },
      { id: '5', message: '风速过高，建议停止作业' }
    ]
  },
  {
    id: '6',
    name: '升降机A-01',
    location: 'A区施工现场',
    status: 'normal',
    load: 70,
    height: 80,
    windSpeed: 7.8,
    alerts: []
  }
]

// 刷新数据
const refreshData = () => {
  loadDeviceData()
  loadSafetyData()
  loadEfficiencyData()
}

// 加载设备数据
const loadDeviceData = () => {
  devices.value = mockDevices
  
  // 计算概览数据
  const totalCranes = devices.value.length
  const runningCranes = devices.value.filter(d => d.status === 'normal').length
  const warningCranes = devices.value.filter(d => d.status === 'warning').length
  const maintenanceCranes = devices.value.filter(d => d.status === 'maintenance').length
  
  overview.value = {
    totalCranes,
    runningCranes,
    warningCranes,
    maintenanceCranes
  }
}

// 加载安全数据
const loadSafetyData = () => {
  const totalAlerts = devices.value.reduce((sum, device) => sum + device.alerts.length, 0)
  const overloadAlerts = devices.value.filter(d => d.load > 80).length
  const overheightAlerts = devices.value.filter(d => d.height > 140).length
  const windAlerts = devices.value.filter(d => d.windSpeed > 12).length
  
  safetyData.value = {
    overloadCount: overloadAlerts,
    overloadRate: Math.round((overloadAlerts / devices.value.length) * 100),
    overheightCount: overheightAlerts,
    overheightRate: Math.round((overheightAlerts / devices.value.length) * 100),
    windAlertCount: windAlerts,
    windAlertRate: Math.round((windAlerts / devices.value.length) * 100)
  }
}

// 加载效率数据
const loadEfficiencyData = () => {
  const normalDevices = devices.value.filter(d => d.status === 'normal')
  const avgWorkHours = normalDevices.length > 0 
    ? Math.round(normalDevices.reduce((sum, d) => sum + d.load, 0) / normalDevices.length)
    : 0
  
  const utilizationRate = Math.round((devices.value.filter(d => d.status === 'normal').length / devices.value.length) * 100)
  
  const safetyScore = 100 - (safetyData.value.overloadRate + safetyData.value.overheightRate + safetyData.value.windAlertRate)
  
  efficiencyData.value = {
    avgWorkHours,
    utilizationRate,
    safetyScore: Math.max(0, safetyScore)
  }
}

// 获取状态文本
const getStatusText = (status: string) => {
  switch (status) {
    case 'normal': return '正常'
    case 'warning': return '预警'
    case 'danger': return '危险'
    case 'maintenance': return '维护'
    default: return '未知'
  }
}

// 获取安全颜色
const getSafetyColor = (percentage: number) => {
  if (percentage <= 20) return '#67c23a'
  if (percentage <= 50) return '#e6a23c'
  return '#f56c6c'
}

// 初始化
onMounted(() => {
  refreshData()
})
</script>

<style scoped>
.crane-management-screen {
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

.overview-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.overview-card {
  display: flex;
  align-items: center;
  padding: 20px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.overview-icon {
  margin-right: 16px;
  font-size: 32px;
}

.overview-content {
  flex: 1;
}

.overview-value {
  font-size: 28px;
  font-weight: 600;
  color: #ffffff;
  line-height: 1;
}

.overview-label {
  font-size: 14px;
  color: #909399;
  margin-top: 4px;
}

.device-monitoring,
.safety-monitoring,
.efficiency-section {
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

.device-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
  gap: 20px;
}

.device-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.3s ease;
}

.device-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.device-card.warning {
  border-color: #e6a23c;
  background: rgba(230, 162, 60, 0.1);
}

.device-card.danger {
  border-color: #f56c6c;
  background: rgba(245, 108, 108, 0.1);
}

.device-card.maintenance {
  border-color: #909399;
  background: rgba(144, 147, 153, 0.1);
}

.device-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.device-name {
  font-weight: 600;
  color: #ffffff;
  font-size: 16px;
}

.device-status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.device-status.normal {
  background: rgba(103, 194, 58, 0.2);
  color: #67c23a;
}

.device-status.warning {
  background: rgba(230, 162, 60, 0.2);
  color: #e6a23c;
}

.device-status.danger {
  background: rgba(245, 108, 108, 0.2);
  color: #f56c6c;
}

.device-status.maintenance {
  background: rgba(144, 147, 153, 0.2);
  color: #909399;
}

.device-info {
  margin-bottom: 12px;
}

.info-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.info-row:last-child {
  margin-bottom: 0;
}

.info-label {
  color: #909399;
}

.info-value {
  color: #ffffff;
  font-weight: 500;
}

.device-alerts {
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  padding-top: 12px;
}

.alert-item {
  display: flex;
  align-items: center;
  margin-bottom: 8px;
  font-size: 12px;
  color: #f56c6c;
}

.alert-item:last-child {
  margin-bottom: 0;
}

.alert-text {
  margin-left: 8px;
}

.safety-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.safety-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.safety-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 12px;
}

.safety-title {
  font-weight: 600;
  color: #ffffff;
}

.safety-count {
  color: #f56c6c;
  font-weight: 500;
}

.safety-progress {
  margin-top: 8px;
}

.efficiency-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.efficiency-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 20px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  text-align: center;
}

.efficiency-header {
  margin-bottom: 12px;
}

.efficiency-title {
  font-size: 14px;
  color: #909399;
}

.efficiency-value {
  font-size: 24px;
  font-weight: 600;
  color: #409eff;
}
</style> 