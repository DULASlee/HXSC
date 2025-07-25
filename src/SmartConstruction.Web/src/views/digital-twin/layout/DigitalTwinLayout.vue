<template>
  <div class="digital-twin-layout" :class="{ fullscreen: isFullscreen }">
    <!-- 顶部Banner -->
    <header class="dt-header">
      <div class="header-content">
        <!-- 左侧：项目名称 -->
        <div class="project-info">
          <div class="project-logo">
            <el-icon><OfficeBuilding /></el-icon>
          </div>
          <div class="project-details">
            <h1 class="project-name">{{ projectName }}</h1>
            <span class="project-subtitle">数字孪生监控中心</span>
          </div>
        </div>

        <!-- 中间：导航TAB -->
        <nav class="nav-tabs">
          <div 
            v-for="tab in tabs" 
            :key="tab.key"
            class="nav-tab"
            :class="{ active: activeTab === tab.key }"
            @click="switchTab(tab.key)"
          >
            <el-icon class="tab-icon" :size="18">
              <component :is="tab.icon" />
            </el-icon>
            <span class="tab-text">{{ tab.label }}</span>
          </div>
        </nav>

        <!-- 右侧：操作按钮 -->
        <div class="header-actions">
          <div class="time-info">
            <div class="current-time">{{ currentTime }}</div>
            <div class="current-date">{{ currentDate }}</div>
          </div>
          
          <div class="action-buttons">
            <el-tooltip content="全屏显示" placement="bottom">
              <el-button 
                type="primary" 
                :icon="isFullscreen ? 'FullScreen' : 'Aim'" 
                circle 
                @click="toggleFullscreen"
              />
            </el-tooltip>
            
            <el-tooltip content="返回PC端" placement="bottom">
              <el-button 
                type="success" 
                :icon="'Monitor'" 
                circle 
                @click="backToPCView"
              />
            </el-tooltip>
          </div>
        </div>
      </div>
    </header>

    <!-- 主内容区域 -->
    <main class="dt-main">
      <div class="screen-container" ref="screenContainer">
        <transition name="screen-fade" mode="out-in">
          <component 
            :is="currentScreenComponent" 
            :key="activeTab"
            :screen-size="screenSize"
          />
        </transition>
      </div>
    </main>

    <!-- 底部状态栏 -->
    <footer class="dt-footer">
      <div class="status-bar">
        <div class="system-status">
          <span class="status-item">
            <el-icon color="#67c23a"><CircleCheckFilled /></el-icon>
            系统正常
          </span>
          <span class="status-item">
            <el-icon color="#409eff"><Connection /></el-icon>
            数据连接：正常
          </span>
          <span class="status-item">
            <el-icon color="#e6a23c"><Refresh /></el-icon>
            最后更新：{{ lastUpdateTime }}
          </span>
        </div>
        
        <div class="footer-info">
          <span>智慧工地数字孪生系统 v1.0</span>
          <span class="separator">|</span>
          <span>分辨率：{{ screenSize.width }} × {{ screenSize.height }}</span>
        </div>
      </div>
    </footer>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { 
  OfficeBuilding, 
  DataAnalysis, 
  Calendar, 
  VideoCamera, 
  UploadFilled, 
  Cloudy,
  FullScreen,
  Aim,
  Monitor,
  CircleCheckFilled,
  Connection,
  Refresh
} from '@element-plus/icons-vue'

// 导入各个屏幕组件
import CommandCenterScreen from '../screens/command-center/index.vue'
import AttendanceScreen from '../screens/attendance/index.vue'
import VideoMonitorScreen from '../screens/video-monitor/index.vue'
import CraneManagementScreen from '../screens/crane-management/index.vue'
import EnvironmentMonitorScreen from '../screens/environment-monitor/index.vue'

const router = useRouter()

// 响应式数据
const isFullscreen = ref(false)
const activeTab = ref('command-center')
const currentTime = ref('')
const currentDate = ref('')
const lastUpdateTime = ref('')
const screenContainer = ref()

// 屏幕尺寸
const screenSize = reactive({
  width: 1920,
  height: 1080
})

// 项目信息
const projectName = ref('智慧工地示范项目')

// 导航标签页配置
const tabs = [
  { 
    key: 'command-center', 
    label: '指挥中心', 
    icon: 'DataAnalysis' 
  },
  { 
    key: 'attendance', 
    label: '项目考勤', 
    icon: 'Calendar' 
  },
  { 
    key: 'video-monitor', 
    label: '视频监控', 
    icon: 'VideoCamera' 
  },
  { 
    key: 'crane-management', 
    label: '塔吊升降机', 
    icon: 'UploadFilled' 
  },
  { 
    key: 'environment-monitor', 
    label: '扬尘噪音', 
    icon: 'Cloudy' 
  }
]

// 当前屏幕组件
const currentScreenComponent = computed(() => {
  const componentMap = {
    'command-center': CommandCenterScreen,
    'attendance': AttendanceScreen,
    'video-monitor': VideoMonitorScreen,
    'crane-management': CraneManagementScreen,
    'environment-monitor': EnvironmentMonitorScreen
  }
  return componentMap[activeTab.value] || CommandCenterScreen
})

// 定时器
let timeTimer: any = null
let updateTimer: any = null

// 初始化
onMounted(() => {
  updateScreenSize()
  startTimeUpdate()
  startDataUpdate()
  
  window.addEventListener('resize', updateScreenSize)
  
  // 监听ESC键退出全屏
  document.addEventListener('keydown', handleKeydown)
})

onUnmounted(() => {
  if (timeTimer) clearInterval(timeTimer)
  if (updateTimer) clearInterval(updateTimer)
  
  window.removeEventListener('resize', updateScreenSize)
  document.removeEventListener('keydown', handleKeydown)
})

// 更新屏幕尺寸
const updateScreenSize = () => {
  if (screenContainer.value) {
    screenSize.width = window.innerWidth
    screenSize.height = window.innerHeight - 120 // 减去头部和底部高度
  }
}

// 开始时间更新
const startTimeUpdate = () => {
  const updateTime = () => {
    const now = new Date()
    currentTime.value = now.toLocaleTimeString('zh-CN', { 
      hour12: false,
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit'
    })
    currentDate.value = now.toLocaleDateString('zh-CN', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      weekday: 'long'
    })
  }
  
  updateTime()
  timeTimer = setInterval(updateTime, 1000)
}

// 开始数据更新
const startDataUpdate = () => {
  const updateData = () => {
    lastUpdateTime.value = new Date().toLocaleTimeString('zh-CN', { hour12: false })
  }
  
  updateData()
  updateTimer = setInterval(updateData, 30000) // 30秒更新一次
}

// 切换标签页
const switchTab = (tabKey: string) => {
  activeTab.value = tabKey
}

// 切换全屏
const toggleFullscreen = () => {
  if (!isFullscreen.value) {
    document.documentElement.requestFullscreen()
    isFullscreen.value = true
  } else {
    document.exitFullscreen()
    isFullscreen.value = false
  }
}

// 返回PC端
const backToPCView = () => {
  router.push('/dashboard')
}

// 键盘事件处理
const handleKeydown = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && isFullscreen.value) {
    isFullscreen.value = false
  }
}
</script>

<style lang="scss" scoped>
.digital-twin-layout {
  width: 100vw;
  height: 100vh;
  background: linear-gradient(135deg, #0c1426 0%, #1a2332 100%);
  color: #ffffff;
  overflow: hidden;
  position: relative;
  
  &.fullscreen {
    .dt-header,
    .dt-footer {
      opacity: 0.1;
      transition: opacity 0.3s ease;
      
      &:hover {
        opacity: 1;
      }
    }
  }
}

// 顶部Banner
.dt-header {
  height: 80px;
  background: linear-gradient(90deg, #1e2a3a 0%, #2d3e50 100%);
  border-bottom: 2px solid #34495e;
  position: relative;
  z-index: 100;
  
  &::after {
    content: '';
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    height: 2px;
    background: linear-gradient(90deg, #3498db, #2ecc71, #f39c12, #e74c3c);
    opacity: 0.6;
  }
}

.header-content {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 30px;
}

.project-info {
  display: flex;
  align-items: center;
  gap: 15px;
  
  .project-logo {
    width: 50px;
    height: 50px;
    background: linear-gradient(135deg, #3498db, #2ecc71);
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 24px;
  }
  
  .project-details {
    .project-name {
      font-size: 24px;
      font-weight: 600;
      margin: 0;
      background: linear-gradient(90deg, #3498db, #2ecc71);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }
    
    .project-subtitle {
      font-size: 12px;
      color: #7f8c8d;
      opacity: 0.8;
    }
  }
}

// 导航TAB
.nav-tabs {
  display: flex;
  gap: 8px;
}

.nav-tab {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 12px 20px;
  background: rgba(52, 73, 94, 0.3);
  border: 1px solid rgba(52, 73, 94, 0.5);
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.3s ease;
  position: relative;
  
  &:hover {
    background: rgba(52, 152, 219, 0.2);
    border-color: #3498db;
    transform: translateY(-2px);
  }
  
  &.active {
    background: linear-gradient(135deg, #3498db, #2ecc71);
    border-color: transparent;
    box-shadow: 0 4px 12px rgba(52, 152, 219, 0.3);
    
    &::after {
      content: '';
      position: absolute;
      bottom: -2px;
      left: 50%;
      transform: translateX(-50%);
      width: 30px;
      height: 3px;
      background: #ffffff;
      border-radius: 2px;
    }
  }
  
  .tab-icon {
    font-size: 18px;
  }
  
  .tab-text {
    font-size: 14px;
    font-weight: 500;
  }
}

// 右侧操作区
.header-actions {
  display: flex;
  align-items: center;
  gap: 20px;
}

.time-info {
  text-align: right;
  
  .current-time {
    font-size: 20px;
    font-weight: 600;
    color: #3498db;
  }
  
  .current-date {
    font-size: 12px;
    color: #7f8c8d;
    margin-top: 2px;
  }
}

.action-buttons {
  display: flex;
  gap: 10px;
}

// 主内容区
.dt-main {
  height: calc(100vh - 120px);
  position: relative;
  overflow: hidden;
}

.screen-container {
  width: 100%;
  height: 100%;
  position: relative;
}

// 过渡动画
.screen-fade-enter-active,
.screen-fade-leave-active {
  transition: all 0.5s ease;
}

.screen-fade-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.screen-fade-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}

// 底部状态栏
.dt-footer {
  height: 40px;
  background: rgba(30, 42, 58, 0.9);
  border-top: 1px solid #34495e;
  position: relative;
  z-index: 100;
}

.status-bar {
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 30px;
}

.system-status {
  display: flex;
  gap: 20px;
  
  .status-item {
    display: flex;
    align-items: center;
    gap: 5px;
    font-size: 12px;
    color: #bdc3c7;
  }
}

.footer-info {
  display: flex;
  gap: 15px;
  font-size: 12px;
  color: #7f8c8d;
  
  .separator {
    color: #34495e;
  }
}

// 响应式设计
@media (max-width: 1600px) {
  .project-info {
    .project-name {
      font-size: 20px;
    }
  }
  
  .nav-tab {
    padding: 10px 16px;
    
    .tab-text {
      font-size: 13px;
    }
  }
}

@media (max-width: 1200px) {
  .header-content {
    padding: 0 20px;
  }
  
  .nav-tabs {
    gap: 4px;
  }
  
  .nav-tab {
    padding: 8px 12px;
    
    .tab-text {
      display: none;
    }
  }
}
</style>