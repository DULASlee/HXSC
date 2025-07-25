<template>
  <div class="digital-twin-dashboard">
    <div class="dashboard-header">
      <h2>{{ title }}</h2>
      <div class="header-actions">
        <el-select v-model="selectedProject" placeholder="选择项目" size="small" @change="handleProjectChange">
          <el-option
            v-for="project in projects"
            :key="project.id"
            :label="project.name"
            :value="project.id"
          />
        </el-select>
        <el-button-group>
          <el-button type="primary" size="small" @click="handleRefresh">
            <el-icon><Refresh /></el-icon>
            刷新
          </el-button>
          <el-button type="primary" size="small" @click="handleFullscreen">
            <el-icon><FullScreen /></el-icon>
            全屏
          </el-button>
        </el-button-group>
      </div>
    </div>
    
    <div class="dashboard-content">
      <div class="dashboard-grid">
        <!-- 3D设备监控 -->
        <div class="dashboard-card device-monitor-card">
          <div class="card-header">
            <h3>设备3D监控</h3>
            <el-button type="text" @click="handleViewDeviceDetail">查看详情</el-button>
          </div>
          <div class="card-content">
            <Device3DMonitor 
              :devices="devices" 
              :project-id="selectedProject"
              @device-click="handleDeviceClick"
              @view-detail="handleDeviceDetailView"
              @maintenance="handleDeviceMaintenance"
            />
          </div>
        </div>
        
        <!-- 环境数据可视化 -->
        <div class="dashboard-card environment-card">
          <div class="card-content">
            <EnvironmentDataVisualizer 
              title="环境监测数据"
              :project-id="selectedProject"
              @view-all-alerts="handleViewAllAlerts"
              @point-change="handleMonitorPointChange"
            />
          </div>
        </div>
        
        <!-- 项目进度时间轴 -->
        <div class="dashboard-card progress-card">
          <div class="card-content">
            <ProjectProgressTimeline 
              title="项目进度"
              :project-id="selectedProject"
              @project-change="handleProjectProgressChange"
              @view-detail="handleViewProjectDetail"
            />
          </div>
        </div>
      </div>
    </div>
    
    <!-- 实时告警通知 -->
    <RealTimeAlertNotifier ref="alertNotifier" />
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Refresh, FullScreen } from '@element-plus/icons-vue'

import Device3DMonitor from './Device3DMonitor.vue'
import EnvironmentDataVisualizer from './EnvironmentDataVisualizer.vue'
import ProjectProgressTimeline from './ProjectProgressTimeline.vue'
import RealTimeAlertNotifier from './RealTimeAlertNotifier.vue'

import { digitalTwinService } from '@/api/services/digital-twin.service'

const props = defineProps({
  title: {
    type: String,
    default: '智慧工地数字孪生中心'
  }
})

// 状态
const projects = ref<any[]>([])
const selectedProject = ref('')
const devices = ref<any[]>([])
const alertNotifier = ref<InstanceType<typeof RealTimeAlertNotifier> | null>(null)
let refreshTimer: NodeJS.Timeout | null = null

// 加载项目列表
const loadProjects = async () => {
  try {
    const response = await digitalTwinService.getProjectList()
    if (response.success) {
      projects.value = response.data
      if (projects.value.length > 0) {
        selectedProject.value = projects.value[0].id
      }
    }
  } catch (error) {
    console.error('Failed to load projects:', error)
    ElMessage.error('加载项目列表失败')
  }
}

// 加载设备数据
const loadDevices = async () => {
  try {
    const response = await digitalTwinService.getCraneElevatorDevices(selectedProject.value)
    if (response.success) {
      devices.value = response.data
    }
  } catch (error) {
    console.error('Failed to load devices:', error)
    ElMessage.error('加载设备数据失败')
  }
}

// 处理项目变更
const handleProjectChange = () => {
  loadDevices()
}

// 处理刷新
const handleRefresh = () => {
  loadDevices()
  ElMessage.success('数据已刷新')
}

// 处理全屏
const handleFullscreen = () => {
  const dashboard = document.querySelector('.digital-twin-dashboard')
  if (!dashboard) return
  
  if (document.fullscreenElement) {
    document.exitFullscreen()
  } else {
    dashboard.requestFullscreen()
  }
}

// 处理设备点击
const handleDeviceClick = (device: any) => {
  console.log('Device clicked:', device)
}

// 处理查看设备详情
const handleDeviceDetailView = (device: any) => {
  ElMessage.info(`查看设备详情: ${device.name}`)
}

// 处理设备维护
const handleDeviceMaintenance = (device: any) => {
  ElMessage.info(`设备维护: ${device.name}`)
}

// 处理查看设备详情
const handleViewDeviceDetail = () => {
  ElMessage.info('查看所有设备详情')
}

// 处理查看所有告警
const handleViewAllAlerts = () => {
  ElMessage.info('查看所有环境告警')
}

// 处理监测点变更
const handleMonitorPointChange = (pointId: string) => {
  console.log('Monitor point changed:', pointId)
}

// 处理项目进度变更
const handleProjectProgressChange = (projectId: string) => {
  console.log('Project progress changed:', projectId)
}

// 处理查看项目详情
const handleViewProjectDetail = (project: any) => {
  ElMessage.info(`查看项目详情: ${project.projectName}`)
}

// 模拟告警
const simulateAlerts = () => {
  // 随机生成告警
  if (Math.random() > 0.8 && alertNotifier.value) {
    const alertTypes = [
      {
        id: `alert-${Date.now()}`,
        level: 'warning',
        source: '扬尘监测系统',
        message: `PM10浓度超标，当前值: ${Math.floor(150 + Math.random() * 50)}μg/m³，阈值: 150μg/m³`,
        location: 'A区-东北角监测点',
        time: new Date().toISOString()
      },
      {
        id: `alert-${Date.now()}`,
        level: 'danger',
        source: '噪音监测系统',
        message: `噪音超标，当前值: ${Math.floor(85 + Math.random() * 15)}dB，阈值: 85dB`,
        location: 'B区-施工现场',
        time: new Date().toISOString()
      },
      {
        id: `alert-${Date.now()}`,
        level: 'danger',
        source: '塔吊监控系统',
        message: '塔吊超载警告，请立即检查',
        location: 'A区-1号塔吊',
        time: new Date().toISOString()
      }
    ]
    
    const randomAlert = alertTypes[Math.floor(Math.random() * alertTypes.length)]
    alertNotifier.value.addAlert(randomAlert)
  }
}

// 启动自动刷新
const startAutoRefresh = () => {
  refreshTimer = setInterval(() => {
    // 模拟告警
    simulateAlerts()
    
    // 每5分钟刷新一次设备数据
    if (Date.now() % (5 * 60 * 1000) < 30000) {
      loadDevices()
    }
  }, 30000) // 30秒检查一次
}

// 停止自动刷新
const stopAutoRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
}

// 生命周期钩子
onMounted(async () => {
  await loadProjects()
  await loadDevices()
  
  // 启动自动刷新
  startAutoRefresh()
})

onUnmounted(() => {
  // 停止自动刷新
  stopAutoRefresh()
})
</script>

<style lang="scss" scoped>
.digital-twin-dashboard {
  width: 100%;
  min-height: 100vh;
  padding: 20px;
  background: #1a1a2e;
  color: #ffffff;
  
  .dashboard-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 20px;
    
    h2 {
      margin: 0;
      font-size: 24px;
      font-weight: 600;
      background: linear-gradient(90deg, #3498db, #2ecc71);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
    }
    
    .header-actions {
      display: flex;
      gap: 12px;
    }
  }
  
  .dashboard-content {
    .dashboard-grid {
      display: grid;
      grid-template-columns: 1fr 1fr;
      grid-template-rows: 500px 1fr;
      gap: 20px;
      
      .dashboard-card {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 8px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        overflow: hidden;
        
        &.device-monitor-card {
          grid-column: 1;
          grid-row: 1 / 3;
        }
        
        &.environment-card {
          grid-column: 2;
          grid-row: 1;
        }
        
        &.progress-card {
          grid-column: 2;
          grid-row: 2;
        }
        
        .card-header {
          display: flex;
          align-items: center;
          justify-content: space-between;
          padding: 12px 16px;
          border-bottom: 1px solid rgba(255, 255, 255, 0.1);
          
          h3 {
            margin: 0;
            font-size: 16px;
            font-weight: 600;
            color: #ffffff;
          }
        }
        
        .card-content {
          height: calc(100% - 50px);
          padding: 16px;
        }
        
        &.device-monitor-card .card-content {
          padding: 0;
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 1200px) {
  .digital-twin-dashboard {
    .dashboard-content {
      .dashboard-grid {
        grid-template-columns: 1fr;
        grid-template-rows: auto;
        
        .dashboard-card {
          &.device-monitor-card,
          &.environment-card,
          &.progress-card {
            grid-column: 1;
            grid-row: auto;
          }
          
          &.device-monitor-card {
            height: 500px;
          }
          
          &.environment-card,
          &.progress-card {
            height: auto;
          }
        }
      }
    }
  }
}

@media (max-width: 768px) {
  .digital-twin-dashboard {
    padding: 12px;
    
    .dashboard-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 12px;
      
      .header-actions {
        width: 100%;
      }
    }
  }
}

// 全屏样式
:deep(.digital-twin-dashboard:fullscreen) {
  background: #1a1a2e;
  padding: 20px;
  overflow: auto;
}
</style>