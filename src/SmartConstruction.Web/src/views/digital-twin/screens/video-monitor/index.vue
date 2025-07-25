<template>
  <div class="video-monitor-screen">
    <div class="screen-header">
      <h2 class="screen-title">视频监控中心</h2>
      <div class="screen-controls">
        <el-button type="primary" size="small" @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新数据
        </el-button>
      </div>
    </div>

    <div class="screen-content">
      <!-- 摄像头网格 -->
      <div class="camera-grid">
        <div 
          v-for="camera in cameras" 
          :key="camera.id"
          class="camera-item"
          :class="{ 'offline': !camera.isOnline }"
        >
          <div class="camera-header">
            <span class="camera-name">{{ camera.name }}</span>
            <span class="camera-status" :class="{ 'online': camera.isOnline, 'offline': !camera.isOnline }">
              {{ camera.isOnline ? '在线' : '离线' }}
            </span>
          </div>
          
          <div class="camera-video">
            <div class="video-placeholder">
              <el-icon size="48" color="#909399">
                <VideoCamera />
              </el-icon>
              <p>{{ camera.isOnline ? '视频流加载中...' : '设备离线' }}</p>
            </div>
          </div>
          
          <div class="camera-info">
            <div class="info-item">
              <span class="label">位置：</span>
              <span class="value">{{ camera.location }}</span>
            </div>
            <div class="info-item">
              <span class="label">分辨率：</span>
              <span class="value">{{ camera.resolution }}</span>
            </div>
            <div class="info-item">
              <span class="label">AI分析：</span>
              <span class="value">{{ camera.aiAnalysis }}</span>
            </div>
          </div>
        </div>
      </div>

      <!-- 统计信息 -->
      <div class="statistics-panel">
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#409eff"><VideoCamera /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ statistics.totalCameras }}</div>
            <div class="stat-label">总摄像头</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#67c23a"><CircleCheckFilled /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ statistics.onlineCameras }}</div>
            <div class="stat-label">在线摄像头</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#e6a23c"><User /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ statistics.detectedPeople }}</div>
            <div class="stat-label">检测人数</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#f56c6c"><Warning /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ statistics.alerts }}</div>
            <div class="stat-label">安全警报</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { 
  VideoCamera, 
  Refresh, 
  CircleCheckFilled, 
  User, 
  Warning 
} from '@element-plus/icons-vue'

interface Camera {
  id: string
  name: string
  location: string
  resolution: string
  isOnline: boolean
  aiAnalysis: string
}

interface Statistics {
  totalCameras: number
  onlineCameras: number
  detectedPeople: number
  alerts: number
}

// 响应式数据
const cameras = ref<Camera[]>([])
const statistics = ref<Statistics>({
  totalCameras: 0,
  onlineCameras: 0,
  detectedPeople: 0,
  alerts: 0
})

// 模拟摄像头数据
const mockCameras: Camera[] = [
  {
    id: '1',
    name: '大门监控',
    location: '项目大门',
    resolution: '1920x1080',
    isOnline: true,
    aiAnalysis: '人数统计、车辆识别'
  },
  {
    id: '2',
    name: '施工区监控A',
    location: 'A区施工现场',
    resolution: '1920x1080',
    isOnline: true,
    aiAnalysis: '安全帽检测、行为分析'
  },
  {
    id: '3',
    name: '施工区监控B',
    location: 'B区施工现场',
    resolution: '1920x1080',
    isOnline: true,
    aiAnalysis: '安全帽检测、行为分析'
  },
  {
    id: '4',
    name: '材料区监控',
    location: '材料堆放区',
    resolution: '1920x1080',
    isOnline: false,
    aiAnalysis: '物料监控、防盗检测'
  },
  {
    id: '5',
    name: '塔吊监控',
    location: '塔吊作业区',
    resolution: '1920x1080',
    isOnline: true,
    aiAnalysis: '作业安全监控'
  },
  {
    id: '6',
    name: '升降机监控',
    location: '升降机区域',
    resolution: '1920x1080',
    isOnline: true,
    aiAnalysis: '载重监控、安全检测'
  }
]

// 刷新数据
const refreshData = () => {
  loadCameraData()
  loadStatistics()
}

// 加载摄像头数据
const loadCameraData = () => {
  cameras.value = mockCameras
}

// 加载统计信息
const loadStatistics = () => {
  const onlineCount = mockCameras.filter(c => c.isOnline).length
  statistics.value = {
    totalCameras: mockCameras.length,
    onlineCameras: onlineCount,
    detectedPeople: Math.floor(Math.random() * 50) + 20,
    alerts: Math.floor(Math.random() * 5)
  }
}

// 初始化
onMounted(() => {
  loadCameraData()
  loadStatistics()
})
</script>

<style scoped>
.video-monitor-screen {
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
}

.camera-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 20px;
  flex: 1;
}

.camera-item {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 8px;
  overflow: hidden;
  transition: all 0.3s ease;
}

.camera-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.camera-item.offline {
  opacity: 0.6;
}

.camera-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.05);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.camera-name {
  font-weight: 600;
  color: #ffffff;
}

.camera-status {
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 500;
}

.camera-status.online {
  background: rgba(103, 194, 58, 0.2);
  color: #67c23a;
}

.camera-status.offline {
  background: rgba(245, 108, 108, 0.2);
  color: #f56c6c;
}

.camera-video {
  height: 200px;
  background: #000000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.video-placeholder {
  text-align: center;
  color: #909399;
}

.video-placeholder p {
  margin: 10px 0 0 0;
  font-size: 14px;
}

.camera-info {
  padding: 12px 16px;
  background: rgba(255, 255, 255, 0.02);
}

.info-item {
  display: flex;
  justify-content: space-between;
  margin-bottom: 8px;
  font-size: 14px;
}

.info-item:last-child {
  margin-bottom: 0;
}

.info-item .label {
  color: #909399;
}

.info-item .value {
  color: #ffffff;
  font-weight: 500;
}

.statistics-panel {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
  padding: 20px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
}

.stat-card {
  display: flex;
  align-items: center;
  padding: 16px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.stat-icon {
  margin-right: 12px;
  font-size: 24px;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 24px;
  font-weight: 600;
  color: #ffffff;
  line-height: 1;
}

.stat-label {
  font-size: 12px;
  color: #909399;
  margin-top: 4px;
}
</style> 