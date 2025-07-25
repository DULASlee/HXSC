<template>
  <div class="video-monitor-screen">
    <!-- 顶部监控统计 -->
    <div class="stats-section">
      <div class="stats-grid">
        <StatisticCard
          :value="videoStatistics.cameraSummary?.totalCameras || 0"
          label="监控总数"
          description="所有监控摄像头"
          icon="VideoCamera"
          type="primary"
          status="normal"
        />
        <StatisticCard
          :value="videoStatistics.cameraSummary?.onlineCameras || 0"
          label="在线监控"
          description="正常工作摄像头"
          icon="View"
          type="success"
          :trend="1.2"
          status="online"
        />
        <StatisticCard
          :value="videoStatistics.cameraSummary?.faultCameras || 0"
          label="故障监控"
          description="需要维修摄像头"
          icon="Warning"
          type="danger"
          :trend="-0.5"
          status="offline"
        />
        <StatisticCard
          :value="videoStatistics.cameraSummary?.onlineRate || 0"
          label="在线率"
          description="设备在线率统计"
          icon="PieChart"
          type="info"
          suffix="%"
          :show-progress="true"
          :percentage="videoStatistics.cameraSummary?.onlineRate || 0"
          :trend="0.8"
          status="normal"
        />
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <div class="content-grid">
        <!-- 视频监控网格 -->
        <div class="video-grid-section">
          <DataCard title="实时视频监控" class="video-grid-card">
            <template #actions>
              <el-button size="small" @click="toggleFullscreen">
                <el-icon><FullScreen /></el-icon>
                全屏
              </el-button>
            </template>
            <div class="video-grid">
              <div 
                class="video-item"
                v-for="camera in videoCameras.slice(0, 9)"
                :key="camera.id"
                @click="selectCamera(camera)"
                :class="{ active: selectedCamera?.id === camera.id }"
              >
                <div class="video-container">
                  <div class="video-placeholder">
                    <el-icon class="video-icon"><VideoCamera /></el-icon>
                    <div class="camera-info">
                      <div class="camera-name">{{ camera.name }}</div>
                      <div class="camera-status" :class="`status--${camera.status.toLowerCase()}`">
                        {{ camera.status }}
                      </div>
                    </div>
                  </div>
                  <div class="video-overlay">
                    <div class="overlay-info">
                      <span class="resolution">{{ camera.resolution }}</span>
                      <span class="fps">{{ camera.fps }}fps</span>
                    </div>
                    <div class="recording-indicator" v-if="camera.recordingStatus === 'Recording'">
                      <el-icon><VideoCameraFilled /></el-icon>
                      REC
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </DataCard>
        </div>

        <!-- 右侧信息面板 -->
        <div class="info-panel">
          <!-- 智能分析 -->
          <DataCard title="AI智能分析" class="ai-analysis-card">
            <div class="analysis-grid">
              <div class="analysis-item">
                <div class="analysis-icon">
                  <el-icon><User /></el-icon>
                </div>
                <div class="analysis-content">
                  <div class="analysis-title">人员检测</div>
                  <div class="analysis-value">{{ aiAnalysis.personDetection?.totalPersons || 0 }}人</div>
                  <div class="analysis-detail">
                    佩戴安全帽：{{ aiAnalysis.personDetection?.helmetDetection?.withHelmets || 0 }}人<br>
                    未佩戴：{{ aiAnalysis.personDetection?.helmetDetection?.withoutHelmets || 0 }}人
                  </div>
                </div>
              </div>
              
              <div class="analysis-item">
                <div class="analysis-icon">
                  <el-icon><Van /></el-icon>
                </div>
                <div class="analysis-content">
                  <div class="analysis-title">车辆检测</div>
                  <div class="analysis-value">{{ aiAnalysis.vehicleDetection?.totalVehicles || 0 }}辆</div>
                  <div class="analysis-detail">
                    货车：{{ aiAnalysis.vehicleDetection?.vehicleTypes?.truck || 0 }}辆<br>
                    小车：{{ aiAnalysis.vehicleDetection?.vehicleTypes?.car || 0 }}辆
                  </div>
                </div>
              </div>

              <div class="analysis-item">
                <div class="analysis-icon">
                  <el-icon><Warning /></el-icon>
                </div>
                <div class="analysis-content">
                  <div class="analysis-title">行为分析</div>
                  <div class="analysis-value">{{ getTotalBehaviorAlerts() }}项</div>
                  <div class="analysis-detail">
                    吸烟检测：{{ aiAnalysis.behaviorAnalysis?.smokingDetection || 0 }}次<br>
                    人群聚集：{{ aiAnalysis.behaviorAnalysis?.crowdGathering || 0 }}次
                  </div>
                </div>
              </div>
            </div>
          </DataCard>

          <!-- 存储统计 -->
          <DataCard title="存储统计" class="storage-card">
            <div class="storage-info">
              <div class="storage-usage">
                <div class="usage-label">存储空间使用情况</div>
                <div class="usage-bar">
                  <el-progress
                    :percentage="videoStatistics.recordingSummary?.usageRate || 0"
                    :stroke-width="12"
                    :color="getStorageColor(videoStatistics.recordingSummary?.usageRate || 0)"
                  />
                </div>
                <div class="usage-text">
                  {{ videoStatistics.recordingSummary?.usedStorage || '0GB' }} / 
                  {{ videoStatistics.recordingSummary?.totalStorage || '0GB' }}
                </div>
              </div>
              
              <div class="storage-stats">
                <div class="stat-row">
                  <span class="stat-label">保留天数</span>
                  <span class="stat-value">{{ videoStatistics.recordingSummary?.retentionDays || 0 }}天</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">今日告警</span>
                  <span class="stat-value">{{ videoStatistics.alertSummary?.todayAlerts || 0 }}次</span>
                </div>
                <div class="stat-row">
                  <span class="stat-label">待处理</span>
                  <span class="stat-value">{{ videoStatistics.alertSummary?.unresolvedAlerts || 0 }}次</span>
                </div>
              </div>
            </div>
          </DataCard>

          <!-- 监控点位列表 -->
          <DataCard title="监控点位" class="camera-list-card">
            <div class="camera-list">
              <div 
                class="camera-item"
                v-for="camera in videoCameras"
                :key="camera.id"
                @click="selectCamera(camera)"
                :class="{ active: selectedCamera?.id === camera.id }"
              >
                <div class="camera-icon">
                  <el-icon><VideoCamera /></el-icon>
                </div>
                <div class="camera-details">
                  <div class="camera-name">{{ camera.name }}</div>
                  <div class="camera-location">{{ camera.location?.area }} - {{ camera.location?.position }}</div>
                </div>
                <div class="camera-status" :class="`status--${camera.status.toLowerCase()}`">
                  <div class="status-dot"></div>
                </div>
              </div>
            </div>
          </DataCard>
        </div>
      </div>
    </div>

    <!-- 底部告警信息 -->
    <div class="alerts-section" v-if="videoStatistics.alertSummary?.todayAlerts > 0">
      <DataCard title="今日告警信息" class="alerts-card">
        <div class="alerts-list">
          <div class="alert-item" v-for="(count, type) in videoStatistics.alertSummary?.alertTypes" :key="type">
            <div class="alert-type">{{ getAlertTypeName(type) }}</div>
            <div class="alert-count">{{ count }}次</div>
            <div class="alert-bar">
              <div 
                class="alert-fill" 
                :style="{ 
                  width: `${(count / Math.max(...Object.values(videoStatistics.alertSummary?.alertTypes || {}))) * 100}%`,
                  background: getAlertTypeColor(type)
                }"
              ></div>
            </div>
          </div>
        </div>
      </DataCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'
import ScreenHeader from '@/components/PageHeader.vue'
import StatisticCard from '@/views/digital-twin/components/StatisticCard.vue'
import DataCard from '@/views/digital-twin/components/DataCard.vue'
import {
  VideoCamera,
  VideoCameraFilled,
  View,
  Warning,
  PieChart,
  FullScreen,
  User,
  Van
} from '@element-plus/icons-vue'

// 数据状态
const loading = ref(true)
const videoCameras = ref<any[]>([])
const videoStatistics = ref<any>({})
const aiAnalysis = ref<any>({})
const selectedCamera = ref<any>(null)

// 定时器
let dataTimer: number | null = null

// 加载监控摄像头
const loadVideoCameras = async () => {
  //  此处需要替换为真实的服务调用
}

// 加载视频统计
const loadVideoStatistics = async () => {
  //  此处需要替换为真实的服务调用
}

// 加载AI分析数据
const loadAiAnalysis = async () => {
  //  此处需要替换为真实的服务调用
}


// 切换摄像头
const selectCamera = (camera: any) => {
  selectedCamera.value = camera
}

// 全屏切换
const toggleFullscreen = () => {
  // 实现全屏逻辑
}

// 工具函数
const getTotalBehaviorAlerts = () => {
  const behavior = aiAnalysis.value.behaviorAnalysis
  if (!behavior) return 0
  return (behavior.smokingDetection || 0) + (behavior.fightingDetection || 0) + (behavior.crowdGathering || 0)
}

const getStorageColor = (percentage: number) => {
  if (percentage >= 90) return '#e74c3c'
  if (percentage >= 70) return '#f39c12'
  return '#2ecc71'
}

const getAlertTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'motion': '移动检测',
    'intrusion': '入侵检测',
    'equipment': '设备异常'
  }
  return typeMap[type] || type
}

const getAlertTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    'motion': '#3498db',
    'intrusion': '#e74c3c',
    'equipment': '#f39c12'
  }
  return colorMap[type] || '#95a5a6'
}

// 初始化数据
const initData = async () => {
  loading.value = true
  await Promise.all([
    loadVideoCameras(),
    loadVideoStatistics(),
    loadAiAnalysis()
  ])
  loading.value = false
}

// 启动数据更新
const startDataUpdate = () => {
  dataTimer = window.setInterval(() => {
    loadVideoStatistics()
    loadAiAnalysis()
  }, 30000)
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
.video-monitor-screen {
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
  flex: 1;
  
  .content-grid {
    display: grid;
    grid-template-columns: 1fr 350px;
    gap: 20px;
    height: 600px;
  }
}

// 视频网格
.video-grid-section {
  .video-grid-card {
    height: 100%;
    
    .video-grid {
      display: grid;
      grid-template-columns: repeat(3, 1fr);
      gap: 12px;
      height: 520px;
      
      .video-item {
        position: relative;
        border-radius: 8px;
        overflow: hidden;
        cursor: pointer;
        border: 2px solid transparent;
        transition: all 0.3s ease;
        
        &:hover {
          border-color: #3498db;
          transform: scale(1.02);
        }
        
        &.active {
          border-color: #2ecc71;
          box-shadow: 0 0 15px rgba(46, 204, 113, 0.3);
        }
        
        .video-container {
          position: relative;
          width: 100%;
          height: 100%;
          background: #1a1a1a;
          
          .video-placeholder {
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            height: 100%;
            color: #7f8c8d;
            
            .video-icon {
              font-size: 32px;
              margin-bottom: 8px;
            }
            
            .camera-info {
              text-align: center;
              
              .camera-name {
                font-size: 12px;
                font-weight: 600;
                color: #ffffff;
                margin-bottom: 4px;
              }
              
              .camera-status {
                font-size: 10px;
                padding: 2px 6px;
                border-radius: 10px;
                
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
          }
          
          .video-overlay {
            position: absolute;
            top: 8px;
            left: 8px;
            right: 8px;
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            
            .overlay-info {
              display: flex;
              flex-direction: column;
              gap: 4px;
              
              span {
                background: rgba(0, 0, 0, 0.7);
                color: #ffffff;
                padding: 2px 6px;
                border-radius: 4px;
                font-size: 10px;
              }
            }
            
            .recording-indicator {
              display: flex;
              align-items: center;
              gap: 4px;
              background: rgba(231, 76, 60, 0.9);
              color: #ffffff;
              padding: 4px 8px;
              border-radius: 12px;
              font-size: 10px;
              font-weight: 600;
              animation: pulse 2s infinite;
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
  
  .ai-analysis-card {
    .analysis-grid {
      display: flex;
      flex-direction: column;
      gap: 16px;
      
      .analysis-item {
        display: flex;
        align-items: flex-start;
        gap: 12px;
        padding: 12px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        
        .analysis-icon {
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
        
        .analysis-content {
          flex: 1;
          
          .analysis-title {
            font-size: 14px;
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 4px;
          }
          
          .analysis-value {
            font-size: 18px;
            font-weight: 700;
            color: #3498db;
            margin-bottom: 4px;
          }
          
          .analysis-detail {
            font-size: 11px;
            color: #7f8c8d;
            line-height: 1.4;
          }
        }
      }
    }
  }
  
  .storage-card {
    .storage-info {
      .storage-usage {
        margin-bottom: 16px;
        
        .usage-label {
          font-size: 12px;
          color: #7f8c8d;
          margin-bottom: 8px;
        }
        
        .usage-bar {
          margin-bottom: 8px;
        }
        
        .usage-text {
          font-size: 12px;
          color: #ffffff;
          text-align: center;
        }
      }
      
      .storage-stats {
        .stat-row {
          display: flex;
          justify-content: space-between;
          align-items: center;
          padding: 6px 0;
          border-bottom: 1px solid rgba(255, 255, 255, 0.1);
          
          &:last-child {
            border-bottom: none;
          }
          
          .stat-label {
            font-size: 12px;
            color: #7f8c8d;
          }
          
          .stat-value {
            font-size: 12px;
            font-weight: 600;
            color: #ffffff;
          }
        }
      }
    }
  }
  
  .camera-list-card {
    flex: 1;
    
    .camera-list {
      max-height: 200px;
      overflow-y: auto;
      
      .camera-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 8px;
        margin-bottom: 4px;
        border-radius: 6px;
        cursor: pointer;
        transition: all 0.3s ease;
        
        &:hover {
          background: rgba(255, 255, 255, 0.05);
        }
        
        &.active {
          background: rgba(52, 152, 219, 0.2);
          border: 1px solid #3498db;
        }
        
        .camera-icon {
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
        
        .camera-details {
          flex: 1;
          
          .camera-name {
            font-size: 12px;
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 2px;
          }
          
          .camera-location {
            font-size: 10px;
            color: #7f8c8d;
          }
        }
        
        .camera-status {
          .status-dot {
            width: 8px;
            height: 8px;
            border-radius: 50%;
            
            &.status--online .status-dot {
              background: #2ecc71;
              box-shadow: 0 0 8px rgba(46, 204, 113, 0.6);
            }
            
            &.status--offline .status-dot {
              background: #e74c3c;
              box-shadow: 0 0 8px rgba(231, 76, 60, 0.6);
            }
          }
        }
      }
    }
  }
}

// 告警区域
.alerts-section {
  .alerts-card {
    .alerts-list {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 16px;
      
      .alert-item {
        display: flex;
        flex-direction: column;
        gap: 8px;
        padding: 16px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        
        .alert-type {
          font-size: 14px;
          font-weight: 600;
          color: #ffffff;
        }
        
        .alert-count {
          font-size: 20px;
          font-weight: 700;
          color: #f39c12;
        }
        
        .alert-bar {
          height: 4px;
          background: rgba(255, 255, 255, 0.1);
          border-radius: 2px;
          overflow: hidden;
          
          .alert-fill {
            height: 100%;
            transition: width 0.3s ease;
          }
        }
      }
    }
  }
}

// 动画效果
@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.6; }
}

// 响应式设计
@media (max-width: 1200px) {
  .main-content .content-grid {
    grid-template-columns: 1fr;
    height: auto;
    gap: 16px;
  }
  
  .video-grid-section .video-grid {
    grid-template-columns: repeat(2, 1fr);
    height: 400px;
  }
}

@media (max-width: 768px) {
  .video-monitor-screen {
    padding: 12px;
  }
  
  .stats-section .stats-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  
  .video-grid-section .video-grid {
    grid-template-columns: 1fr;
    height: 300px;
  }
  
  .alerts-section .alerts-list {
    grid-template-columns: 1fr;
    gap: 12px;
  }
}
</style>