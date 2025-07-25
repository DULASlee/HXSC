<template>
  <div class="video-security-dashboard">
    <!-- 顶部控制栏 -->
    <div class="control-bar">
      <div class="control-left">
        <h2 class="page-title">📹 视频安防监控大屏</h2>
        <div class="stream-status">
          <div class="status-indicator" :class="streamStatus.webrtc ? 'online' : 'offline'">
            <el-icon><VideoCamera /></el-icon>
            WebRTC: {{ streamStatus.webrtc ? '已连接' : '连接中' }}
          </div>
          <div class="latency-info">
            延迟: {{ streamStatus.latency }}ms
          </div>
        </div>
      </div>
      
      <div class="control-right">
        <div class="ai-status">
          <div class="ai-indicator" :class="aiAnalysis.enabled ? 'active' : 'inactive'">
            <el-icon><View /></el-icon>
            AI分析: {{ aiAnalysis.enabled ? '运行中' : '已停止' }}
          </div>
          <div class="detection-count">
            今日检测: {{ aiAnalysis.todayDetections }}次
          </div>
        </div>
        
        <div class="action-buttons">
          <el-button size="small" @click="toggleVideoMapping">
            <el-icon><Monitor /></el-icon>
            {{ videoMapping ? '关闭映射' : '开启映射' }}
          </el-button>
          <el-button size="small" @click="startCrossTracking">
            <el-icon><Connection /></el-icon>
            跨镜头追踪
          </el-button>
        </div>
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 3D场景区域 -->
      <div class="scene-section">
        <div ref="threejsContainer" class="threejs-container"></div>
        
        <!-- 摄像头状态面板 -->
        <div class="camera-status-panel">
          <h4>摄像头状态</h4>
          <div class="camera-grid">
            <div 
              v-for="camera in cameras" 
              :key="camera.id"
              class="camera-item"
              :class="{ 
                active: selectedCamera?.id === camera.id,
                online: camera.status === 'online',
                offline: camera.status === 'offline',
                alert: camera.hasAlert
              }"
              @click="selectCamera(camera)"
            >
              <div class="camera-preview">
                <video 
                  v-if="camera.status === 'online'"
                  :ref="`video_${camera.id}`"
                  autoplay 
                  muted 
                  playsinline
                  class="preview-video"
                ></video>
                <div v-else class="offline-placeholder">
                  <el-icon><VideoCamera /></el-icon>
                  <span>离线</span>
                </div>
              </div>
              <div class="camera-info">
                <div class="camera-name">{{ camera.name }}</div>
                <div class="camera-location">{{ camera.location }}</div>
                <div class="camera-metrics">
                  <span class="resolution">{{ camera.resolution }}</span>
                  <span class="fps">{{ camera.fps }}fps</span>
                </div>
              </div>
              <div v-if="camera.hasAlert" class="alert-badge">
                <el-icon><Warning /></el-icon>
              </div>
            </div>
          </div>
        </div>
        
        <!-- AI报警弹窗 -->
        <div v-if="activeAlert" class="ai-alert-popup">
          <div class="alert-header">
            <h3>🚨 AI安全预警</h3>
            <el-button text @click="dismissAlert">
              <el-icon><Close /></el-icon>
            </el-button>
          </div>
          <div class="alert-content">
            <div class="alert-video">
              <video 
                :src="activeAlert.videoUrl" 
                autoplay 
                loop 
                muted
                class="alert-video-player"
              ></video>
            </div>
            <div class="alert-details">
              <div class="alert-type">{{ activeAlert.type }}</div>
              <div class="alert-description">{{ activeAlert.description }}</div>
              <div class="alert-location">
                <el-icon><Location /></el-icon>
                {{ activeAlert.location }}
              </div>
              <div class="alert-time">
                {{ formatTime(activeAlert.timestamp) }}
              </div>
            </div>
          </div>
          <div class="alert-actions">
            <el-button type="primary" @click="handleAlertAction('confirm')">
              确认处理
            </el-button>
            <el-button @click="handleAlertAction('ignore')">
              忽略
            </el-button>
            <el-button @click="handleAlertAction('locate')">
              <el-icon><Location /></el-icon>
              定位现场
            </el-button>
          </div>
        </div>
      </div>
      
      <!-- 控制面板 -->
      <div class="control-panel">
        <!-- 实时视频流 -->
        <div class="video-stream-section">
          <h3>实时视频流</h3>
          <div class="main-video-container">
            <video 
              v-if="selectedCamera"
              ref="mainVideo"
              autoplay 
              muted 
              playsinline
              class="main-video"
              @loadstart="onVideoLoadStart"
              @canplay="onVideoCanPlay"
            ></video>
            <div v-else class="no-selection">
              <el-icon><VideoCamera /></el-icon>
              <span>请选择摄像头</span>
            </div>
            
            <!-- 视频控制器 -->
            <div v-if="selectedCamera" class="video-controls">
              <div class="control-row">
                <el-button size="small" @click="toggleRecording">
                  <el-icon><VideoCameraFilled /></el-icon>
                  {{ isRecording ? '停止录制' : '开始录制' }}
                </el-button>
                <el-button size="small" @click="takeSnapshot">
                  <el-icon><Camera /></el-icon>
                  截图
                </el-button>
                <el-button size="small" @click="toggleFullscreen">
                  <el-icon><FullScreen /></el-icon>
                  全屏
                </el-button>
              </div>
              
              <div class="quality-controls">
                <span>画质:</span>
                <el-select v-model="videoQuality" size="small" @change="changeVideoQuality">
                  <el-option label="高清(1080p)" value="1080p"></el-option>
                  <el-option label="标清(720p)" value="720p"></el-option>
                  <el-option label="流畅(480p)" value="480p"></el-option>
                </el-select>
              </div>
            </div>
          </div>
        </div>
        
        <!-- AI分析结果 -->
        <div class="ai-analysis-section">
          <h3>AI智能分析</h3>
          <div class="analysis-stats">
            <div class="stat-item">
              <div class="stat-icon safety">
                <el-icon><ShieldCheck /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ aiStats.safetyCompliance }}%</div>
                <div class="stat-label">安全合规率</div>
              </div>
            </div>
            
            <div class="stat-item">
              <div class="stat-icon detection">
                <el-icon><View /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ aiStats.detectionCount }}</div>
                <div class="stat-label">今日检测数</div>
              </div>
            </div>
          </div>
          
          <!-- 实时检测结果 -->
          <div class="detection-results">
            <h4>实时检测结果</h4>
            <div class="detection-list">
              <div 
                v-for="detection in realtimeDetections" 
                :key="detection.id"
                class="detection-item"
                :class="detection.severity"
              >
                <div class="detection-icon">
                  <el-icon><Warning /></el-icon>
                </div>
                <div class="detection-content">
                  <div class="detection-type">{{ detection.type }}</div>
                  <div class="detection-confidence">置信度: {{ detection.confidence }}%</div>
                  <div class="detection-camera">{{ detection.cameraName }}</div>
                </div>
                <div class="detection-time">
                  {{ formatTime(detection.timestamp) }}
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 跨镜头追踪 -->
        <div class="cross-tracking-section">
          <h3>跨镜头追踪</h3>
          <div class="tracking-controls">
            <el-input 
              v-model="trackingTarget" 
              placeholder="输入人员ID或选择目标"
              size="small"
            >
              <template #append>
                <el-button @click="startTracking">开始追踪</el-button>
              </template>
            </el-input>
          </div>
          
          <!-- 追踪结果 -->
          <div v-if="trackingResults.length > 0" class="tracking-results">
            <h4>追踪路径</h4>
            <div class="tracking-timeline">
              <div 
                v-for="(result, index) in trackingResults" 
                :key="index"
                class="tracking-item"
                @click="jumpToCamera(result.cameraId, result.timestamp)"
              >
                <div class="tracking-time">{{ formatTime(result.timestamp) }}</div>
                <div class="tracking-camera">{{ result.cameraName }}</div>
                <div class="tracking-confidence">{{ result.confidence }}%</div>
                <div class="tracking-thumbnail">
                  <img :src="result.thumbnail" alt="追踪截图" />
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 系统性能监控 -->
        <div class="performance-section">
          <h3>系统性能</h3>
          <div class="performance-metrics">
            <div class="metric-item">
              <span class="metric-label">WebRTC延迟:</span>
              <span class="metric-value" :class="getLatencyClass(streamStatus.latency)">
                {{ streamStatus.latency }}ms
              </span>
            </div>
            <div class="metric-item">
              <span class="metric-label">AI处理延迟:</span>
              <span class="metric-value">{{ aiStats.processingLatency }}ms</span>
            </div>
            <div class="metric-item">
              <span class="metric-label">网络带宽:</span>
              <span class="metric-value">{{ networkStats.bandwidth }}Mbps</span>
            </div>
            <div class="metric-item">
              <span class="metric-label">CPU使用率:</span>
              <span class="metric-value">{{ systemStats.cpuUsage }}%</span>
            </div>
            <div class="metric-item">
              <span class="metric-label">内存使用:</span>
              <span class="metric-value">{{ systemStats.memoryUsage }}%</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, nextTick } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  VideoCamera, View, Monitor, Connection, Warning, Close, 
  Location, VideoCameraFilled, Camera, FullScreen, ShieldCheck
} from '@element-plus/icons-vue'
import { VideoSecurityEngine } from './engine/VideoSecurityEngine'

// 组件状态
const threejsContainer = ref<HTMLElement>()
const mainVideo = ref<HTMLVideoElement>()
const selectedCamera = ref<any>(null)
const trackingTarget = ref('')
const videoQuality = ref('1080p')
const isRecording = ref(false)
const videoMapping = ref(true)

// 3D引擎实例
let engine: VideoSecurityEngine | null = null

// 流媒体状态
const streamStatus = ref({
  webrtc: false,
  latency: 0,
  quality: '1080p',
  bitrate: 0
})

// AI分析状态
const aiAnalysis = ref({
  enabled: true,
  accuracy: 95.2,
  todayDetections: 247
})

// 摄像头数据
const cameras = ref([
  {
    id: 'cam001',
    name: '主入口摄像头',
    location: '工地大门',
    position: { x: 0, y: 5, z: 50 },
    status: 'online',
    resolution: '1080p',
    fps: 30,
    hasAlert: false,
    streamUrl: 'wss://localhost:8080/stream/cam001'
  },
  {
    id: 'cam002', 
    name: '塔吊监控',
    location: '1号塔吊',
    position: { x: 30, y: 25, z: 30 },
    status: 'online',
    resolution: '1080p',
    fps: 25,
    hasAlert: true,
    streamUrl: 'wss://localhost:8080/stream/cam002'
  },
  {
    id: 'cam003',
    name: '材料堆放区',
    location: '材料仓库',
    position: { x: -40, y: 3, z: 20 },
    status: 'online',
    resolution: '720p',
    fps: 30,
    hasAlert: false,
    streamUrl: 'wss://localhost:8080/stream/cam003'
  },
  {
    id: 'cam004',
    name: '办公区监控',
    location: '办公楼',
    position: { x: -30, y: 8, z: -40 },
    status: 'offline',
    resolution: '1080p', 
    fps: 0,
    hasAlert: false,
    streamUrl: 'wss://localhost:8080/stream/cam004'
  }
])

// AI统计数据
const aiStats = ref({
  safetyCompliance: 94.2,
  detectionCount: 247,
  processingLatency: 156,
  accuracy: 96.8
})

// 实时检测结果
const realtimeDetections = ref([
  {
    id: 'det001',
    type: '未佩戴安全帽',
    confidence: 94,
    severity: 'high',
    cameraName: '主入口摄像头',
    timestamp: Date.now() - 30000
  },
  {
    id: 'det002',
    type: '人员越界',
    confidence: 88,
    severity: 'medium',
    cameraName: '塔吊监控',
    timestamp: Date.now() - 120000
  }
])

// 活跃警报
const activeAlert = ref<any>(null)

// 跨镜头追踪结果
const trackingResults = ref<any[]>([])

// 网络统计
const networkStats = ref({
  bandwidth: 45.6,
  packetLoss: 0.2,
  jitter: 12
})

// 系统统计
const systemStats = ref({
  cpuUsage: 68,
  memoryUsage: 72,
  gpuUsage: 45
})

// WebRTC连接管理
const webrtcConnections = new Map<string, RTCPeerConnection>()

/// <summary>
/// 初始化3D可视化引擎
/// </summary>
const initVisualization = async () => {
  if (!threejsContainer.value) return
  
  try {
    engine = new VideoSecurityEngine(threejsContainer.value)
    await engine.init()
    
    // 设置事件监听
    engine.on('cameraClick', handleCameraClick)
    engine.on('alertTrigger', handleAlertTrigger)
    
    // 初始化摄像头
    cameras.value.forEach(camera => {
      engine?.addCamera(camera)
    })
    
    // 开始渲染
    engine.startRender()
    
    ElMessage.success('视频监控系统初始化成功')
    
  } catch (error) {
    console.error('初始化失败:', error)
    ElMessage.error('系统初始化失败')
  }
}

/// <summary>
/// 初始化WebRTC连接
/// </summary>
const initWebRTC = async () => {
  try {
    // 为每个在线摄像头建立WebRTC连接
    for (const camera of cameras.value) {
      if (camera.status === 'online') {
        await establishWebRTCConnection(camera)
      }
    }
    
    streamStatus.value.webrtc = true
    
  } catch (error) {
    console.error('WebRTC初始化失败:', error)
    streamStatus.value.webrtc = false
  }
}

/// <summary>
/// 建立WebRTC连接 - 200ms延迟目标
/// </summary>
const establishWebRTCConnection = async (camera: any): Promise<void> => {
  const startTime = performance.now()
  
  try {
    const pc = new RTCPeerConnection({
      iceServers: [
        { urls: 'stun:stun.l.google.com:19302' }
      ]
    })
    
    // 接收视频流
    pc.ontrack = (event) => {
      const stream = event.streams[0]
      const videoElement = document.querySelector(`[ref="video_${camera.id}"]`) as HTMLVideoElement
      
      if (videoElement) {
        videoElement.srcObject = stream
        
        // 测量延迟
        const endTime = performance.now()
        const latency = Math.round(endTime - startTime)
        streamStatus.value.latency = latency
        
        console.log(`摄像头 ${camera.id} WebRTC连接建立，延迟: ${latency}ms`)
        
        if (latency > 200) {
          ElMessage.warning(`摄像头 ${camera.name} 延迟过高: ${latency}ms`)
        }
      }
    }
    
    // ICE连接状态监控
    pc.oniceconnectionstatechange = () => {
      console.log(`摄像头 ${camera.id} ICE状态:`, pc.iceConnectionState)
      
      if (pc.iceConnectionState === 'failed') {
        ElMessage.error(`摄像头 ${camera.name} 连接失败`)
        camera.status = 'offline'
      }
    }
    
    // 创建offer并发送信令
    const offer = await pc.createOffer()
    await pc.setLocalDescription(offer)
    
    // 模拟信令交换 - 实际应用中需要通过WebSocket或HTTP发送
    // await sendSignaling(camera.id, offer)
    
    webrtcConnections.set(camera.id, pc)
    
  } catch (error) {
    console.error(`建立WebRTC连接失败 (${camera.id}):`, error)
    camera.status = 'offline'
  }
}

/// <summary>
/// 选择摄像头
/// </summary>
const selectCamera = (camera: any) => {
  selectedCamera.value = camera
  
  // 切换主视频流
  if (camera.status === 'online' && mainVideo.value) {
    const pc = webrtcConnections.get(camera.id)
    if (pc) {
      // 获取视频流并显示
      // 实际实现中需要从RTCPeerConnection获取远程流
    }
  }
  
  // 在3D场景中高亮摄像头
  engine?.highlightCamera(camera.id)
  
  // 如果开启了视频映射，将视频投影到建筑物上
  if (videoMapping.value) {
    engine?.mapVideoToBuilding(camera.id, camera.position)
  }
}

/// <summary>
/// 摄像头点击事件
/// </summary>
const handleCameraClick = (cameraData: any) => {
  const camera = cameras.value.find(c => c.id === cameraData.id)
  if (camera) {
    selectCamera(camera)
  }
}

/// <summary>
/// AI报警触发
/// </summary>
const handleAlertTrigger = (alertData: any) => {
  // 创建AI报警弹窗
  activeAlert.value = {
    id: `alert_${Date.now()}`,
    type: alertData.type,
    description: alertData.description,
    location: alertData.location,
    videoUrl: alertData.videoUrl,
    timestamp: Date.now(),
    cameraId: alertData.cameraId
  }
  
  // 更新摄像头警报状态
  const camera = cameras.value.find(c => c.id === alertData.cameraId)
  if (camera) {
    camera.hasAlert = true
  }
  
  // 在3D场景中显示警报动画
  engine?.showAlert(alertData.cameraId, alertData.type)
  
  ElMessage.warning(`AI检测到异常: ${alertData.type}`)
}

/// <summary>
/// 处理警报
/// </summary>
const handleAlertAction = (action: string) => {
  if (!activeAlert.value) return
  
  switch (action) {
    case 'confirm':
      ElMessage.success('警报已确认处理')
      break
    case 'ignore':
      ElMessage.info('警报已忽略')
      break
    case 'locate':
      // 定位到警报位置
      const camera = cameras.value.find(c => c.id === activeAlert.value.cameraId)
      if (camera && engine) {
        engine.focusOnCamera(camera.id)
      }
      break
  }
  
  dismissAlert()
}

/// <summary>
/// 关闭警报
/// </summary>
const dismissAlert = () => {
  if (activeAlert.value) {
    // 清除摄像头警报状态
    const camera = cameras.value.find(c => c.id === activeAlert.value.cameraId)
    if (camera) {
      camera.hasAlert = false
    }
    
    // 清除3D场景中的警报显示
    engine?.clearAlert(activeAlert.value.cameraId)
  }
  
  activeAlert.value = null
}

/// <summary>
/// 切换视频映射
/// </summary>
const toggleVideoMapping = () => {
  videoMapping.value = !videoMapping.value
  
  if (videoMapping.value && selectedCamera.value) {
    engine?.mapVideoToBuilding(selectedCamera.value.id, selectedCamera.value.position)
  } else {
    engine?.clearVideoMapping()
  }
  
  ElMessage.info(`视频纹理映射${videoMapping.value ? '已开启' : '已关闭'}`)
}

/// <summary>
/// 开始跨镜头追踪
/// </summary>
const startCrossTracking = () => {
  if (!trackingTarget.value) {
    ElMessage.warning('请输入追踪目标')
    return
  }
  
  // 模拟跨镜头追踪结果
  trackingResults.value = [
    {
      cameraId: 'cam001',
      cameraName: '主入口摄像头',
      timestamp: Date.now() - 300000,
      confidence: 95,
      thumbnail: '/api/snapshots/cam001_tracking.jpg'
    },
    {
      cameraId: 'cam002',
      cameraName: '塔吊监控',
      timestamp: Date.now() - 180000,
      confidence: 88,
      thumbnail: '/api/snapshots/cam002_tracking.jpg'
    },
    {
      cameraId: 'cam003',
      cameraName: '材料堆放区',
      timestamp: Date.now() - 60000,
      confidence: 92,
      thumbnail: '/api/snapshots/cam003_tracking.jpg'
    }
  ]
  
  // 在3D场景中显示追踪路径
  engine?.showTrackingPath(trackingResults.value)
  
  ElMessage.success(`开始追踪目标: ${trackingTarget.value}`)
}

/// <summary>
/// 开始追踪
/// </summary>
const startTracking = () => {
  startCrossTracking()
}

/// <summary>
/// 跳转到指定摄像头
/// </summary>
const jumpToCamera = (cameraId: string, timestamp: number) => {
  const camera = cameras.value.find(c => c.id === cameraId)
  if (camera) {
    selectCamera(camera)
    // TODO: 跳转到指定时间戳的录像
  }
}

/// <summary>
/// 切换录制状态
/// </summary>
const toggleRecording = () => {
  isRecording.value = !isRecording.value
  
  if (isRecording.value) {
    ElMessage.success('开始录制')
    // TODO: 实现录制功能
  } else {
    ElMessage.info('停止录制')
  }
}

/// <summary>
/// 截图
/// </summary>
const takeSnapshot = () => {
  if (selectedCamera.value && mainVideo.value) {
    // TODO: 实现截图功能
    ElMessage.success('截图已保存')
  }
}

/// <summary>
/// 切换全屏
/// </summary>
const toggleFullscreen = () => {
  if (mainVideo.value) {
    if (document.fullscreenElement) {
      document.exitFullscreen()
    } else {
      mainVideo.value.requestFullscreen()
    }
  }
}

/// <summary>
/// 改变视频质量
/// </summary>
const changeVideoQuality = (quality: string) => {
  // TODO: 实现画质切换
  ElMessage.info(`画质已切换到: ${quality}`)
}

/// <summary>
/// 视频加载开始
/// </summary>
const onVideoLoadStart = () => {
  console.log('视频开始加载')
}

/// <summary>
/// 视频可以播放
/// </summary>
const onVideoCanPlay = () => {
  console.log('视频可以播放')
}

/// <summary>
/// 获取延迟等级样式
/// </summary>
const getLatencyClass = (latency: number): string => {
  if (latency <= 100) return 'excellent'
  if (latency <= 200) return 'good'
  if (latency <= 500) return 'fair'
  return 'poor'
}

/// <summary>
/// 格式化时间
/// </summary>
const formatTime = (timestamp: number): string => {
  return new Date(timestamp).toLocaleTimeString()
}

// 实时数据更新
let updateInterval: NodeJS.Timeout
let latencyMonitorInterval: NodeJS.Timeout

onMounted(async () => {
  await nextTick()
  
  // 初始化可视化引擎
  await initVisualization()
  
  // 初始化WebRTC连接
  await initWebRTC()
  
  // 模拟AI检测数据更新
  updateInterval = setInterval(() => {
    // 随机生成新的检测结果
    if (Math.random() < 0.3) {
      const detectionTypes = ['未佩戴安全帽', '人员越界', '危险行为', '设备异常']
      const newDetection = {
        id: `det_${Date.now()}`,
        type: detectionTypes[Math.floor(Math.random() * detectionTypes.length)],
        confidence: Math.floor(80 + Math.random() * 20),
        severity: Math.random() > 0.7 ? 'high' : 'medium',
        cameraName: cameras.value[Math.floor(Math.random() * cameras.value.length)].name,
        timestamp: Date.now()
      }
      
      realtimeDetections.value.unshift(newDetection)
      
      // 限制检测结果数量
      if (realtimeDetections.value.length > 10) {
        realtimeDetections.value = realtimeDetections.value.slice(0, 10)
      }
      
      // 高危险等级触发弹窗警报
      if (newDetection.severity === 'high' && Math.random() < 0.5) {
        handleAlertTrigger({
          type: newDetection.type,
          description: `在${newDetection.cameraName}检测到${newDetection.type}`,
          location: newDetection.cameraName,
          videoUrl: '/mock-video/alert.mp4',
          cameraId: cameras.value[0].id
        })
      }
    }
  }, 5000)
  
  // 延迟监控
  latencyMonitorInterval = setInterval(() => {
    // 模拟延迟波动
    streamStatus.value.latency = Math.floor(150 + Math.random() * 100)
    
    // 更新网络统计
    networkStats.value.bandwidth = Math.round((40 + Math.random() * 20) * 10) / 10
    networkStats.value.packetLoss = Math.round(Math.random() * 0.5 * 100) / 100
    
    // 更新系统统计
    systemStats.value.cpuUsage = Math.floor(60 + Math.random() * 30)
    systemStats.value.memoryUsage = Math.floor(65 + Math.random() * 25)
  }, 2000)
})

onUnmounted(() => {
  if (updateInterval) clearInterval(updateInterval)
  if (latencyMonitorInterval) clearInterval(latencyMonitorInterval)
  
  // 关闭WebRTC连接
  webrtcConnections.forEach(pc => {
    pc.close()
  })
  webrtcConnections.clear()
  
  engine?.destroy()
})
</script>

<style lang="scss" scoped>
.video-security-dashboard {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #0a0a0a;
  color: #ffffff;
  
  .control-bar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 16px 20px;
    background: rgba(255, 255, 255, 0.05);
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    backdrop-filter: blur(10px);
    
    .control-left {
      display: flex;
      align-items: center;
      gap: 20px;
      
      .page-title {
        margin: 0;
        font-size: 20px;
        font-weight: 600;
      }
      
      .stream-status {
        display: flex;
        align-items: center;
        gap: 16px;
        
        .status-indicator {
          display: flex;
          align-items: center;
          gap: 6px;
          font-size: 14px;
          
          &.online {
            color: #67c23a;
          }
          
          &.offline {
            color: #f56c6c;
          }
        }
        
        .latency-info {
          font-size: 12px;
          color: rgba(255, 255, 255, 0.7);
        }
      }
    }
    
    .control-right {
      display: flex;
      align-items: center;
      gap: 20px;
      
      .ai-status {
        display: flex;
        flex-direction: column;
        align-items: flex-end;
        
        .ai-indicator {
          display: flex;
          align-items: center;
          gap: 6px;
          font-size: 14px;
          
          &.active {
            color: #409eff;
          }
          
          &.inactive {
            color: #909399;
          }
        }
        
        .detection-count {
          font-size: 12px;
          color: rgba(255, 255, 255, 0.6);
        }
      }
      
      .action-buttons {
        display: flex;
        gap: 8px;
      }
    }
  }
  
  .main-content {
    flex: 1;
    display: flex;
    gap: 20px;
    padding: 20px;
    overflow: hidden;
    
    .scene-section {
      flex: 1;
      position: relative;
      background: linear-gradient(135deg, #1e3c72 0%, #2a5298 100%);
      border-radius: 12px;
      overflow: hidden;
      
      .threejs-container {
        width: 100%;
        height: 100%;
      }
      
      .camera-status-panel {
        position: absolute;
        top: 20px;
        left: 20px;
        width: 300px;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 8px;
        padding: 16px;
        backdrop-filter: blur(10px);
        
        h4 {
          margin: 0 0 12px 0;
          color: white;
          font-size: 14px;
        }
        
        .camera-grid {
          display: grid;
          grid-template-columns: 1fr;
          gap: 8px;
          max-height: 300px;
          overflow-y: auto;
          
          .camera-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 8px;
            border-radius: 6px;
            background: rgba(255, 255, 255, 0.05);
            border: 1px solid transparent;
            cursor: pointer;
            transition: all 0.3s;
            position: relative;
            
            &:hover {
              background: rgba(255, 255, 255, 0.1);
            }
            
            &.active {
              border-color: #409eff;
              background: rgba(64, 158, 255, 0.1);
            }
            
            &.offline {
              opacity: 0.6;
            }
            
            &.alert {
              border-color: #f56c6c;
              animation: pulse 2s infinite;
            }
            
            .camera-preview {
              width: 48px;
              height: 36px;
              border-radius: 4px;
              overflow: hidden;
              background: #333;
              display: flex;
              align-items: center;
              justify-content: center;
              
              .preview-video {
                width: 100%;
                height: 100%;
                object-fit: cover;
              }
              
              .offline-placeholder {
                display: flex;
                flex-direction: column;
                align-items: center;
                gap: 2px;
                color: #666;
                font-size: 10px;
              }
            }
            
            .camera-info {
              flex: 1;
              
              .camera-name {
                font-size: 12px;
                font-weight: 600;
                color: white;
              }
              
              .camera-location {
                font-size: 11px;
                color: rgba(255, 255, 255, 0.7);
              }
              
              .camera-metrics {
                display: flex;
                gap: 8px;
                margin-top: 2px;
                
                .resolution, .fps {
                  font-size: 10px;
                  color: rgba(255, 255, 255, 0.5);
                }
              }
            }
            
            .alert-badge {
              position: absolute;
              top: 4px;
              right: 4px;
              width: 16px;
              height: 16px;
              background: #f56c6c;
              border-radius: 50%;
              display: flex;
              align-items: center;
              justify-content: center;
              font-size: 10px;
              color: white;
            }
          }
        }
      }
      
      .ai-alert-popup {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 480px;
        background: rgba(0, 0, 0, 0.95);
        border-radius: 12px;
        border: 2px solid #f56c6c;
        backdrop-filter: blur(20px);
        z-index: 1000;
        
        .alert-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          padding: 16px 20px;
          border-bottom: 1px solid rgba(255, 255, 255, 0.1);
          
          h3 {
            margin: 0;
            color: #f56c6c;
            font-size: 16px;
          }
        }
        
        .alert-content {
          padding: 20px;
          
          .alert-video {
            width: 100%;
            margin-bottom: 16px;
            
            .alert-video-player {
              width: 100%;
              height: 200px;
              border-radius: 8px;
              object-fit: cover;
            }
          }
          
          .alert-details {
            .alert-type {
              font-size: 18px;
              font-weight: 600;
              color: #f56c6c;
              margin-bottom: 8px;
            }
            
            .alert-description {
              font-size: 14px;
              color: rgba(255, 255, 255, 0.8);
              margin-bottom: 12px;
            }
            
            .alert-location {
              display: flex;
              align-items: center;
              gap: 6px;
              font-size: 12px;
              color: rgba(255, 255, 255, 0.6);
              margin-bottom: 8px;
            }
            
            .alert-time {
              font-size: 12px;
              color: rgba(255, 255, 255, 0.5);
            }
          }
        }
        
        .alert-actions {
          display: flex;
          gap: 12px;
          padding: 16px 20px;
          border-top: 1px solid rgba(255, 255, 255, 0.1);
        }
      }
    }
    
    .control-panel {
      width: 420px;
      display: flex;
      flex-direction: column;
      gap: 20px;
      
      .video-stream-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .main-video-container {
          position: relative;
          
          .main-video {
            width: 100%;
            height: 200px;
            border-radius: 8px;
            object-fit: cover;
            background: #333;
          }
          
          .no-selection {
            width: 100%;
            height: 200px;
            border-radius: 8px;
            background: #333;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 8px;
            color: #666;
          }
          
          .video-controls {
            margin-top: 12px;
            
            .control-row {
              display: flex;
              gap: 8px;
              margin-bottom: 8px;
            }
            
            .quality-controls {
              display: flex;
              align-items: center;
              gap: 8px;
              font-size: 12px;
              color: rgba(255, 255, 255, 0.7);
            }
          }
        }
      }
      
      .ai-analysis-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .analysis-stats {
          display: flex;
          gap: 16px;
          margin-bottom: 20px;
          
          .stat-item {
            flex: 1;
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 8px;
            
            .stat-icon {
              width: 32px;
              height: 32px;
              border-radius: 6px;
              display: flex;
              align-items: center;
              justify-content: center;
              
              &.safety {
                background: linear-gradient(135deg, #67c23a, #85ce61);
              }
              
              &.detection {
                background: linear-gradient(135deg, #409eff, #66b1ff);
              }
            }
            
            .stat-content {
              .stat-value {
                font-size: 16px;
                font-weight: 700;
                color: white;
              }
              
              .stat-label {
                font-size: 11px;
                color: rgba(255, 255, 255, 0.6);
              }
            }
          }
        }
        
        .detection-results {
          h4 {
            margin: 0 0 12px 0;
            color: white;
            font-size: 14px;
          }
          
          .detection-list {
            max-height: 200px;
            overflow-y: auto;
            
            .detection-item {
              display: flex;
              align-items: center;
              gap: 12px;
              padding: 8px;
              border-radius: 6px;
              margin-bottom: 6px;
              
              &.high {
                background: rgba(245, 108, 108, 0.1);
                border-left: 3px solid #f56c6c;
              }
              
              &.medium {
                background: rgba(230, 162, 60, 0.1);
                border-left: 3px solid #e6a23c;
              }
              
              .detection-icon {
                color: #f56c6c;
                font-size: 14px;
              }
              
              .detection-content {
                flex: 1;
                
                .detection-type {
                  font-size: 12px;
                  font-weight: 600;
                  color: white;
                }
                
                .detection-confidence {
                  font-size: 11px;
                  color: rgba(255, 255, 255, 0.6);
                }
                
                .detection-camera {
                  font-size: 10px;
                  color: rgba(255, 255, 255, 0.5);
                }
              }
              
              .detection-time {
                font-size: 10px;
                color: rgba(255, 255, 255, 0.4);
              }
            }
          }
        }
      }
      
      .cross-tracking-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .tracking-controls {
          margin-bottom: 16px;
        }
        
        .tracking-results {
          h4 {
            margin: 0 0 12px 0;
            color: white;
            font-size: 14px;
          }
          
          .tracking-timeline {
            .tracking-item {
              display: flex;
              align-items: center;
              gap: 8px;
              padding: 8px;
              border-radius: 6px;
              background: rgba(255, 255, 255, 0.05);
              margin-bottom: 6px;
              cursor: pointer;
              transition: background-color 0.3s;
              
              &:hover {
                background: rgba(255, 255, 255, 0.1);
              }
              
              .tracking-time {
                font-size: 11px;
                color: rgba(255, 255, 255, 0.6);
                min-width: 60px;
              }
              
              .tracking-camera {
                flex: 1;
                font-size: 12px;
                color: white;
              }
              
              .tracking-confidence {
                font-size: 11px;
                color: #67c23a;
                min-width: 40px;
              }
              
              .tracking-thumbnail {
                width: 32px;
                height: 24px;
                border-radius: 3px;
                overflow: hidden;
                
                img {
                  width: 100%;
                  height: 100%;
                  object-fit: cover;
                }
              }
            }
          }
        }
      }
      
      .performance-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .performance-metrics {
          .metric-item {
            display: flex;
            justify-content: space-between;
            margin-bottom: 8px;
            
            .metric-label {
              font-size: 12px;
              color: rgba(255, 255, 255, 0.7);
            }
            
            .metric-value {
              font-size: 12px;
              font-weight: 600;
              
              &.excellent {
                color: #67c23a;
              }
              
              &.good {
                color: #e6a23c;
              }
              
              &.fair {
                color: #f56c6c;
              }
              
              &.poor {
                color: #f56c6c;
                animation: blink 2s infinite;
              }
            }
          }
        }
      }
    }
  }
}

// 动画
@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

@keyframes blink {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.3;
  }
}
</style> 