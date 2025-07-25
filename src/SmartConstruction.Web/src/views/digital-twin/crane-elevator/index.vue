<template>
  <div class="crane-monitoring-dashboard">
    <!-- 顶部控制栏 -->
    <div class="control-bar">
      <div class="control-left">
        <h2 class="page-title">🏗️ 塔吊升降机监测大屏</h2>
        <div class="sampling-status">
          <div class="status-indicator" :class="samplingStatus.active ? 'online' : 'offline'">
            <el-icon><DataAnalysis /></el-icon>
            采样频率: {{ samplingStatus.frequency }}Hz
          </div>
          <div class="sync-accuracy">
            姿态同步误差: {{ samplingStatus.syncError }}°
          </div>
        </div>
      </div>
      
      <div class="control-right">
        <div class="collision-status">
          <div class="collision-indicator" :class="collisionStatus.risk ? 'warning' : 'safe'">
            <el-icon><Warning /></el-icon>
            碰撞风险: {{ collisionStatus.risk ? '高危' : '安全' }}
          </div>
          <div class="safety-distance">
            最小距离: {{ collisionStatus.minDistance }}m
          </div>
        </div>
        
        <div class="action-buttons">
          <el-button size="small" @click="toggleStressHeatmap">
            <el-icon><TrendCharts /></el-icon>
            {{ stressHeatmap ? '关闭应力图' : '开启应力图' }}
          </el-button>
          <el-button size="small" @click="playCollisionSimulation">
            <el-icon><VideoPlay /></el-icon>
            碰撞预演
          </el-button>
          <el-button size="small" @click="toggleGestureControl">
            <el-icon><View /></el-icon>
            {{ gestureControl ? '关闭手势' : '开启手势' }}
          </el-button>
        </div>
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 3D场景区域 -->
      <div class="scene-section">
        <div ref="threejsContainer" class="threejs-container"></div>
        
        <!-- 塔吊状态面板 -->
        <div class="crane-status-panel">
          <h4>塔吊实时状态</h4>
          <div class="crane-grid">
            <div 
              v-for="crane in cranes" 
              :key="crane.id"
              class="crane-item"
              :class="{ 
                active: selectedCrane?.id === crane.id,
                online: crane.status === 'online',
                offline: crane.status === 'offline',
                warning: crane.hasWarning
              }"
              @click="selectCrane(crane)"
            >
              <div class="crane-header">
                <div class="crane-name">{{ crane.name }}</div>
                <div class="crane-status">{{ crane.status === 'online' ? '运行中' : '离线' }}</div>
              </div>
              
              <div class="crane-metrics">
                <div class="metric-row">
                  <span class="label">负载:</span>
                  <span class="value" :class="getLoadClass(crane.load, crane.maxLoad)">
                    {{ crane.load }}/{{ crane.maxLoad }}t
                  </span>
                </div>
                <div class="metric-row">
                  <span class="label">高度:</span>
                  <span class="value">{{ crane.height }}m</span>
                </div>
                <div class="metric-row">
                  <span class="label">角度:</span>
                  <span class="value">{{ crane.rotation }}°</span>
                </div>
                <div class="metric-row">
                  <span class="label">幅度:</span>
                  <span class="value">{{ crane.jibAngle }}°</span>
                </div>
              </div>
              
              <!-- 应力指示器 -->
              <div class="stress-indicator">
                <div class="stress-bar">
                  <div 
                    class="stress-fill" 
                    :style="{ 
                      width: `${crane.stressLevel}%`,
                      background: getStressColor(crane.stressLevel)
                    }"
                  ></div>
                </div>
                <span class="stress-label">应力: {{ crane.stressLevel }}%</span>
              </div>
              
              <div v-if="crane.hasWarning" class="warning-badge">
                <el-icon><Warning /></el-icon>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 碰撞预警弹窗 -->
        <div v-if="collisionAlert" class="collision-alert-popup">
          <div class="alert-header">
            <h3>⚠️ 碰撞风险预警</h3>
            <el-button text @click="dismissCollisionAlert">
              <el-icon><Close /></el-icon>
            </el-button>
          </div>
          <div class="alert-content">
            <div class="collision-info">
              <div class="collision-cranes">
                <span>{{ collisionAlert.crane1 }}</span>
                <el-icon><Right /></el-icon>
                <span>{{ collisionAlert.crane2 }}</span>
              </div>
              <div class="collision-distance">
                当前距离: <span class="distance">{{ collisionAlert.currentDistance }}m</span>
              </div>
              <div class="collision-time">
                预计碰撞时间: <span class="time">{{ collisionAlert.estimatedTime }}s</span>
              </div>
            </div>
            <div class="collision-simulation">
              <canvas ref="collisionCanvas" width="300" height="200"></canvas>
            </div>
          </div>
          <div class="alert-actions">
            <el-button type="danger" @click="emergencyStop">
              紧急停机
            </el-button>
            <el-button type="warning" @click="adjustCranePath">
              调整路径
            </el-button>
            <el-button @click="dismissCollisionAlert">
              忽略
            </el-button>
          </div>
        </div>
        
        <!-- 手势控制指示器 -->
        <div v-if="gestureControl" class="gesture-indicator">
          <div class="gesture-status">
            <el-icon><View /></el-icon>
            <span>手势控制已启用</span>
          </div>
          <div class="gesture-hints">
            <div class="hint-item">👆 单指上滑 - 上升</div>
            <div class="hint-item">👇 单指下滑 - 下降</div>
            <div class="hint-item">🔄 双指旋转 - 旋转</div>
            <div class="hint-item">👐 双指缩放 - 查看细节</div>
          </div>
        </div>
      </div>
      
      <!-- 控制面板 -->
      <div class="control-panel">
        <!-- 实时数据监控 -->
        <div class="realtime-monitoring-section">
          <h3>实时数据监控</h3>
          <div v-if="selectedCrane" class="monitoring-content">
            <!-- 关键指标卡片 -->
            <div class="metrics-cards">
              <div class="metric-card load">
                <div class="metric-icon">
                  <el-icon><Scale /></el-icon>
                </div>
                <div class="metric-content">
                  <div class="metric-value">{{ selectedCrane.load }}t</div>
                  <div class="metric-label">当前负载</div>
                  <div class="metric-progress">
                    <el-progress 
                      :percentage="(selectedCrane.load / selectedCrane.maxLoad) * 100"
                      :color="getLoadProgressColor(selectedCrane.load, selectedCrane.maxLoad)"
                      :show-text="false"
                      :stroke-width="6"
                    />
                  </div>
                </div>
              </div>
              
              <div class="metric-card height">
                <div class="metric-icon">
                  <el-icon><Top /></el-icon>
                </div>
                <div class="metric-content">
                  <div class="metric-value">{{ selectedCrane.height }}m</div>
                  <div class="metric-label">吊钩高度</div>
                  <div class="metric-trend">
                    <span :class="selectedCrane.heightTrend">
                      {{ selectedCrane.heightTrend === 'up' ? '↗' : selectedCrane.heightTrend === 'down' ? '↘' : '→' }}
                    </span>
                  </div>
                </div>
              </div>
              
              <div class="metric-card rotation">
                <div class="metric-icon">
                  <el-icon><RefreshRight /></el-icon>
                </div>
                <div class="metric-content">
                  <div class="metric-value">{{ selectedCrane.rotation }}°</div>
                  <div class="metric-label">回转角度</div>
                  <div class="metric-speed">{{ selectedCrane.rotationSpeed }}°/s</div>
                </div>
              </div>
            </div>
            
            <!-- 传感器数据图表 -->
            <div class="sensor-charts">
              <h4>传感器数据趋势 (10Hz采样)</h4>
              <div class="chart-container">
                <canvas ref="sensorChart" width="360" height="200"></canvas>
              </div>
              <div class="chart-legend">
                <div class="legend-item">
                  <span class="color-indicator load"></span>
                  <span>负载 (t)</span>
                </div>
                <div class="legend-item">
                  <span class="color-indicator stress"></span>
                  <span>应力 (%)</span>
                </div>
                <div class="legend-item">
                  <span class="color-indicator vibration"></span>
                  <span>振动 (mm/s)</span>
                </div>
              </div>
            </div>
          </div>
          <div v-else class="no-selection">
            <el-icon><Monitor /></el-icon>
            <span>请选择塔吊查看详细数据</span>
          </div>
        </div>
        
        <!-- 应力分析 -->
        <div class="stress-analysis-section">
          <h3>结构应力分析</h3>
          <div v-if="selectedCrane" class="stress-content">
            <div class="stress-overview">
              <div class="stress-summary">
                <div class="summary-item critical">
                  <span class="count">{{ stressAnalysis.critical }}</span>
                  <span class="label">高应力点</span>
                </div>
                <div class="summary-item warning">
                  <span class="count">{{ stressAnalysis.warning }}</span>
                  <span class="label">预警点</span>
                </div>
                <div class="summary-item normal">
                  <span class="count">{{ stressAnalysis.normal }}</span>
                  <span class="label">正常点</span>
                </div>
              </div>
            </div>
            
            <!-- 关键节点应力 -->
            <div class="key-nodes">
              <h4>关键节点应力</h4>
              <div class="nodes-list">
                <div 
                  v-for="node in stressNodes" 
                  :key="node.id"
                  class="node-item"
                  :class="node.level"
                  @click="focusOnNode(node)"
                >
                  <div class="node-name">{{ node.name }}</div>
                  <div class="node-stress">{{ node.stress }}MPa</div>
                  <div class="node-status">{{ node.status }}</div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 控制操作 -->
        <div class="crane-control-section">
          <h3>塔吊控制</h3>
          <div v-if="selectedCrane && selectedCrane.status === 'online'" class="control-content">
            <!-- 运动控制 -->
            <div class="motion-controls">
              <div class="control-group">
                <h4>起升控制</h4>
                <div class="control-buttons">
                  <el-button 
                    type="success" 
                    @click="controlCrane('lift', 'up')"
                    :disabled="isControlling"
                  >
                    <el-icon><Top /></el-icon>
                    上升
                  </el-button>
                  <el-button 
                    type="warning" 
                    @click="controlCrane('lift', 'down')"
                    :disabled="isControlling"
                  >
                    <el-icon><Bottom /></el-icon>
                    下降
                  </el-button>
                </div>
              </div>
              
              <div class="control-group">
                <h4>回转控制</h4>
                <div class="control-buttons">
                  <el-button 
                    type="primary" 
                    @click="controlCrane('rotate', 'left')"
                    :disabled="isControlling"
                  >
                    <el-icon><Back /></el-icon>
                    左转
                  </el-button>
                  <el-button 
                    type="primary" 
                    @click="controlCrane('rotate', 'right')"
                    :disabled="isControlling"
                  >
                    <el-icon><Right /></el-icon>
                    右转
                  </el-button>
                </div>
              </div>
              
              <div class="control-group">
                <h4>变幅控制</h4>
                <div class="control-buttons">
                  <el-button 
                    type="info" 
                    @click="controlCrane('jib', 'extend')"
                    :disabled="isControlling"
                  >
                    <el-icon><Right /></el-icon>
                    伸出
                  </el-button>
                  <el-button 
                    type="info" 
                    @click="controlCrane('jib', 'retract')"
                    :disabled="isControlling"
                  >
                    <el-icon><Back /></el-icon>
                    收回
                  </el-button>
                </div>
              </div>
            </div>
            
            <!-- 安全控制 -->
            <div class="safety-controls">
              <el-button 
                type="danger" 
                size="large" 
                @click="emergencyStop"
                class="emergency-stop"
              >
                <el-icon><Close /></el-icon>
                紧急停机
              </el-button>
            </div>
            
            <!-- 控制状态 -->
            <div v-if="isControlling" class="control-status">
              <div class="status-text">
                <el-icon><Loading /></el-icon>
                正在执行: {{ currentOperation }}
              </div>
              <div class="status-progress">
                <el-progress :percentage="operationProgress" />
              </div>
            </div>
          </div>
          <div v-else class="control-disabled">
            <el-icon><Lock /></el-icon>
            <span>{{ selectedCrane ? '塔吊离线，无法控制' : '请选择塔吊' }}</span>
          </div>
        </div>
        
        <!-- 系统性能 -->
        <div class="performance-section">
          <h3>系统性能</h3>
          <div class="performance-metrics">
            <div class="metric-item">
              <span class="metric-label">采样频率:</span>
              <span class="metric-value" :class="getSamplingClass(samplingStatus.frequency)">
                {{ samplingStatus.frequency }}Hz
              </span>
            </div>
            <div class="metric-item">
              <span class="metric-label">数据延迟:</span>
              <span class="metric-value">{{ performanceStats.dataLatency }}ms</span>
            </div>
            <div class="metric-item">
              <span class="metric-label">控制响应:</span>
              <span class="metric-value">{{ performanceStats.controlLatency }}ms</span>
            </div>
            <div class="metric-item">
              <span class="metric-label">同步精度:</span>
              <span class="metric-value" :class="getSyncClass(samplingStatus.syncError)">
                ±{{ samplingStatus.syncError }}°
              </span>
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
  DataAnalysis, Warning, TrendCharts, VideoPlay, View, Close, 
  Right, Scale, Top, RefreshRight, Monitor, Bottom, Back, 
  Lock, Loading
} from '@element-plus/icons-vue'
import { digitalTwinApi } from '@/api/modules/digitalTwin'
import { CraneVisualizationEngine } from './engine/CraneVisualizationEngine'

// 组件状态
const threejsContainer = ref<HTMLElement>()
const collisionCanvas = ref<HTMLCanvasElement>()
const sensorChart = ref<HTMLCanvasElement>()
const selectedCrane = ref<any>(null)
const isControlling = ref(false)
const currentOperation = ref('')
const operationProgress = ref(0)
const stressHeatmap = ref(true)
const gestureControl = ref(false)

// 3D引擎实例
let engine: CraneVisualizationEngine | null = null

// 采样状态
const samplingStatus = ref({
  active: true,
  frequency: 10.2,
  syncError: 0.3,
  dataQuality: 98.5
})

// 碰撞状态
const collisionStatus = ref({
  risk: false,
  minDistance: 15.8,
  safetyThreshold: 10.0
})

// 塔吊数据
const cranes = ref([
  {
    id: 'crane001',
    name: '1号塔吊',
    position: { x: 30, y: 0, z: 30 },
    status: 'online',
    load: 2.5,
    maxLoad: 8.0,
    height: 45.2,
    rotation: 135,
    jibAngle: 15,
    rotationSpeed: 0.5,
    heightTrend: 'up',
    stressLevel: 65,
    hasWarning: false,
    workRadius: 55,
    sensors: {
      load: 2.5,
      stress: 65,
      vibration: 2.1,
      temperature: 28
    }
  },
  {
    id: 'crane002',
    name: '2号塔吊',
    position: { x: -30, y: 0, z: -30 },
    status: 'online',
    load: 1.2,
    maxLoad: 6.0,
    height: 32.8,
    rotation: 45,
    jibAngle: 8,
    rotationSpeed: 0,
    heightTrend: 'stable',
    stressLevel: 42,
    hasWarning: true,
    workRadius: 50,
    sensors: {
      load: 1.2,
      stress: 42,
      vibration: 1.8,
      temperature: 26
    }
  },
  {
    id: 'crane003',
    name: '3号塔吊',
    position: { x: 60, y: 0, z: -10 },
    status: 'offline',
    load: 0,
    maxLoad: 5.0,
    height: 0,
    rotation: 0,
    jibAngle: 0,
    rotationSpeed: 0,
    heightTrend: 'stable',
    stressLevel: 0,
    hasWarning: false,
    workRadius: 45,
    sensors: {
      load: 0,
      stress: 0,
      vibration: 0,
      temperature: 0
    }
  }
])

// 应力分析数据
const stressAnalysis = ref({
  critical: 2,
  warning: 5,
  normal: 28
})

// 关键节点应力数据
const stressNodes = ref([
  { id: 'node1', name: '起重臂根部', stress: 145.6, status: '正常', level: 'normal' },
  { id: 'node2', name: '塔身中段', stress: 198.2, status: '预警', level: 'warning' },
  { id: 'node3', name: '回转支承', stress: 234.7, status: '高应力', level: 'critical' },
  { id: 'node4', name: '平衡臂', stress: 128.9, status: '正常', level: 'normal' },
  { id: 'node5', name: '起升机构', stress: 176.4, status: '预警', level: 'warning' }
])

// 碰撞警报
const collisionAlert = ref<any>(null)

// 性能统计
const performanceStats = ref({
  dataLatency: 45,
  controlLatency: 120,
  networkQuality: 95
})

// 传感器数据历史
const sensorDataHistory = ref<Array<{ timestamp: number; load: number; stress: number; vibration: number }>>([])

/// <summary>
/// 初始化3D可视化引擎
/// </summary>
const initVisualization = async () => {
  if (!threejsContainer.value) return
  
  try {
    engine = new CraneVisualizationEngine(threejsContainer.value)
    await engine.init()
    
    // 设置事件监听
    engine.on('craneClick', handleCraneClick)
    engine.on('collisionWarning', handleCollisionWarning)
    engine.on('gestureDetected', handleGesture)
    
    // 初始化塔吊
    cranes.value.forEach(crane => {
      engine?.addCrane(crane)
    })
    
    // 启用应力热力图
    if (stressHeatmap.value) {
      engine?.enableStressHeatmap(true)
    }
    
    // 开始渲染
    engine.startRender()
    
    ElMessage.success('塔吊监测系统初始化成功')
    
  } catch (error) {
    console.error('初始化失败:', error)
    ElMessage.error('系统初始化失败')
  }
}

/// <summary>
/// 初始化传感器数据采集 - 10Hz采样频率
/// </summary>
const initSensorDataCollection = () => {
  // 高频数据采集 - 100ms间隔 (10Hz)
  setInterval(async () => {
    if (selectedCrane.value && selectedCrane.value.status === 'online') {
      try {
        // 模拟传感器数据采集
        const newData = {
          timestamp: Date.now(),
          load: selectedCrane.value.load + (Math.random() - 0.5) * 0.1,
          stress: selectedCrane.value.stressLevel + (Math.random() - 0.5) * 2,
          vibration: selectedCrane.value.sensors.vibration + (Math.random() - 0.5) * 0.2
        }
        
        // 添加到历史数据
        sensorDataHistory.value.push(newData)
        
        // 保持最近100个数据点 (10秒历史)
        if (sensorDataHistory.value.length > 100) {
          sensorDataHistory.value = sensorDataHistory.value.slice(-100)
        }
        
        // 更新实时数据图表
        updateSensorChart()
        
        // 检测异常值
        detectAnomalies(newData)
        
      } catch (error) {
        console.error('传感器数据采集失败:', error)
      }
    }
  }, 100) // 100ms = 10Hz
}

/// <summary>
/// 选择塔吊
/// </summary>
const selectCrane = (crane: any) => {
  selectedCrane.value = crane
  
  // 在3D场景中高亮塔吊
  engine?.highlightCrane(crane.id)
  
  // 显示应力云图
  if (stressHeatmap.value && crane.status === 'online') {
    engine?.showStressDistribution(crane.id)
  }
  
  // 重置传感器数据历史
  sensorDataHistory.value = []
  
  ElMessage.info(`已选择: ${crane.name}`)
}

/// <summary>
/// 塔吊点击事件
/// </summary>
const handleCraneClick = (craneData: any) => {
  const crane = cranes.value.find(c => c.id === craneData.id)
  if (crane) {
    selectCrane(crane)
  }
}

/// <summary>
/// 碰撞预警处理
/// </summary>
const handleCollisionWarning = (warningData: any) => {
  collisionAlert.value = {
    crane1: warningData.crane1Name,
    crane2: warningData.crane2Name,
    currentDistance: warningData.distance.toFixed(1),
    estimatedTime: warningData.timeToCollision.toFixed(1)
  }
  
  // 更新碰撞状态
  collisionStatus.value.risk = true
  collisionStatus.value.minDistance = warningData.distance
  
  // 播放碰撞模拟动画
  playCollisionSimulationCanvas()
  
  ElMessage.error(`碰撞风险警报: ${warningData.crane1Name} 和 ${warningData.crane2Name}`)
}

/// <summary>
/// 手势控制处理
/// </summary>
const handleGesture = (gestureData: any) => {
  if (!gestureControl.value || !selectedCrane.value) return
  
  const { type, direction, intensity } = gestureData
  
  switch (type) {
    case 'swipe_up':
      controlCrane('lift', 'up', intensity)
      break
    case 'swipe_down':
      controlCrane('lift', 'down', intensity)
      break
    case 'rotate':
      controlCrane('rotate', direction > 0 ? 'right' : 'left', Math.abs(direction))
      break
    case 'pinch':
      // 缩放查看细节
      engine?.zoomToDetail(selectedCrane.value.id, intensity)
      break
  }
}

/// <summary>
/// 控制塔吊动作
/// </summary>
const controlCrane = async (action: string, direction: string, intensity: number = 1) => {
  if (!selectedCrane.value || selectedCrane.value.status !== 'online' || isControlling.value) return
  
  isControlling.value = true
  currentOperation.value = `${action}_${direction}`
  operationProgress.value = 0
  
  try {
    // 发送控制指令
    const response = await digitalTwinApi.controlCrane(selectedCrane.value.id, {
      action,
      direction,
      intensity
    })
    
    if (response.data) {
      // 模拟操作进度
      const progressInterval = setInterval(() => {
        operationProgress.value += 10
        if (operationProgress.value >= 100) {
          clearInterval(progressInterval)
          isControlling.value = false
          operationProgress.value = 0
          currentOperation.value = ''
          
          // 更新塔吊状态
          updateCraneState(action, direction, intensity)
          
          ElMessage.success('控制指令执行完成')
        }
      }, 200)
    }
    
  } catch (error) {
    console.error('控制指令失败:', error)
    ElMessage.error('控制指令执行失败')
    isControlling.value = false
  }
}

/// <summary>
/// 更新塔吊状态
/// </summary>
const updateCraneState = (action: string, direction: string, intensity: number) => {
  if (!selectedCrane.value) return
  
  const crane = selectedCrane.value
  
  switch (action) {
    case 'lift':
      if (direction === 'up') {
        crane.height = Math.min(crane.height + intensity * 2, 60)
        crane.heightTrend = 'up'
      } else {
        crane.height = Math.max(crane.height - intensity * 2, 0)
        crane.heightTrend = 'down'
      }
      break
    case 'rotate':
      if (direction === 'right') {
        crane.rotation = (crane.rotation + intensity * 10) % 360
      } else {
        crane.rotation = (crane.rotation - intensity * 10 + 360) % 360
      }
      crane.rotationSpeed = intensity
      break
    case 'jib':
      if (direction === 'extend') {
        crane.jibAngle = Math.min(crane.jibAngle + intensity * 5, 30)
      } else {
        crane.jibAngle = Math.max(crane.jibAngle - intensity * 5, 0)
      }
      break
  }
  
  // 更新3D模型
  engine?.updateCraneState(crane.id, crane)
  
  // 检查碰撞风险
  checkCollisionRisk()
}

/// <summary>
/// 检查碰撞风险
/// </summary>
const checkCollisionRisk = () => {
  const onlineCranes = cranes.value.filter(c => c.status === 'online')
  
  for (let i = 0; i < onlineCranes.length; i++) {
    for (let j = i + 1; j < onlineCranes.length; j++) {
      const crane1 = onlineCranes[i]
      const crane2 = onlineCranes[j]
      
      const distance = calculateDistance(crane1.position, crane2.position)
      const safeDistance = crane1.workRadius + crane2.workRadius + 5 // 5m安全缓冲
      
      if (distance < safeDistance) {
        const timeToCollision = estimateCollisionTime(crane1, crane2, distance)
        
        if (timeToCollision > 0 && timeToCollision < 30) { // 30秒内碰撞风险
          handleCollisionWarning({
            crane1Name: crane1.name,
            crane2Name: crane2.name,
            distance,
            timeToCollision
          })
        }
      }
    }
  }
}

/// <summary>
/// 计算距离
/// </summary>
const calculateDistance = (pos1: any, pos2: any): number => {
  const dx = pos1.x - pos2.x
  const dz = pos1.z - pos2.z
  return Math.sqrt(dx * dx + dz * dz)
}

/// <summary>
/// 估算碰撞时间
/// </summary>
const estimateCollisionTime = (crane1: any, crane2: any, currentDistance: number): number => {
  // 简化计算 - 基于当前旋转速度
  const relativeSpeed = Math.abs(crane1.rotationSpeed) + Math.abs(crane2.rotationSpeed)
  if (relativeSpeed === 0) return -1
  
  const dangerDistance = crane1.workRadius + crane2.workRadius
  const timeToCollision = (currentDistance - dangerDistance) / (relativeSpeed * 0.1) // 转换为秒
  
  return timeToCollision
}

/// <summary>
/// 紧急停机
/// </summary>
const emergencyStop = async () => {
  try {
    await ElMessageBox.confirm(
      '确认执行紧急停机操作？这将停止所有塔吊运动。',
      '紧急停机确认',
      {
        confirmButtonText: '确认停机',
        cancelButtonText: '取消',
        type: 'error'
      }
    )
    
    // 停止所有塔吊
    cranes.value.forEach(crane => {
      if (crane.status === 'online') {
        crane.rotationSpeed = 0
        crane.heightTrend = 'stable'
        engine?.emergencyStopCrane(crane.id)
      }
    })
    
    isControlling.value = false
    operationProgress.value = 0
    
    ElMessage.success('紧急停机执行成功')
    
  } catch {
    // 用户取消
  }
}

/// <summary>
/// 调整路径
/// </summary>
const adjustCranePath = () => {
  if (!collisionAlert.value) return
  
  // TODO: 实现智能路径调整算法
  ElMessage.info('路径调整算法启动中...')
  dismissCollisionAlert()
}

/// <summary>
/// 关闭碰撞警报
/// </summary>
const dismissCollisionAlert = () => {
  collisionAlert.value = null
  collisionStatus.value.risk = false
}

/// <summary>
/// 切换应力热力图
/// </summary>
const toggleStressHeatmap = () => {
  stressHeatmap.value = !stressHeatmap.value
  engine?.enableStressHeatmap(stressHeatmap.value)
  
  ElMessage.info(`应力热力图${stressHeatmap.value ? '已开启' : '已关闭'}`)
}

/// <summary>
/// 播放碰撞预演
/// </summary>
const playCollisionSimulation = () => {
  if (!engine) return
  
  // 在3D场景中播放碰撞预演动画
  engine.playCollisionSimulation(cranes.value.filter(c => c.status === 'online'))
  
  ElMessage.info('正在播放碰撞预演动画...')
}

/// <summary>
/// 切换手势控制
/// </summary>
const toggleGestureControl = () => {
  gestureControl.value = !gestureControl.value
  
  if (gestureControl.value) {
    engine?.enableGestureControl(true)
    ElMessage.success('手势控制已启用')
  } else {
    engine?.enableGestureControl(false)
    ElMessage.info('手势控制已关闭')
  }
}

/// <summary>
/// 聚焦节点
/// </summary>
const focusOnNode = (node: any) => {
  if (selectedCrane.value && engine) {
    engine.focusOnStressNode(selectedCrane.value.id, node.id)
    ElMessage.info(`已聚焦到: ${node.name}`)
  }
}

/// <summary>
/// 更新传感器图表
/// </summary>
const updateSensorChart = () => {
  const canvas = sensorChart.value
  if (!canvas || sensorDataHistory.value.length === 0) return
  
  const ctx = canvas.getContext('2d')!
  const width = canvas.width
  const height = canvas.height
  
  // 清除画布
  ctx.clearRect(0, 0, width, height)
  
  // 绘制网格
  ctx.strokeStyle = '#333'
  ctx.lineWidth = 1
  for (let i = 0; i <= 10; i++) {
    const y = (height / 10) * i
    ctx.beginPath()
    ctx.moveTo(0, y)
    ctx.lineTo(width, y)
    ctx.stroke()
  }
  
  // 绘制数据线
  const drawDataLine = (data: number[], color: string, scale: number) => {
    ctx.strokeStyle = color
    ctx.lineWidth = 2
    ctx.beginPath()
    
    data.forEach((value, index) => {
      const x = (width / (data.length - 1)) * index
      const y = height - (value / scale) * height
      
      if (index === 0) {
        ctx.moveTo(x, y)
      } else {
        ctx.lineTo(x, y)
      }
    })
    
    ctx.stroke()
  }
  
  const loadData = sensorDataHistory.value.map(d => d.load)
  const stressData = sensorDataHistory.value.map(d => d.stress)
  const vibrationData = sensorDataHistory.value.map(d => d.vibration)
  
  drawDataLine(loadData, '#409eff', 10) // 负载线 (蓝色)
  drawDataLine(stressData, '#f56c6c', 100) // 应力线 (红色) 
  drawDataLine(vibrationData, '#67c23a', 5) // 振动线 (绿色)
}

/// <summary>
/// 播放碰撞模拟动画到Canvas
/// </summary>
const playCollisionSimulationCanvas = () => {
  const canvas = collisionCanvas.value
  if (!canvas) return
  
  const ctx = canvas.getContext('2d')!
  const width = canvas.width
  const height = canvas.height
  
  let frame = 0
  const totalFrames = 60
  
  const animate = () => {
    ctx.clearRect(0, 0, width, height)
    
    // 绘制两个塔吊的运动轨迹
    const progress = frame / totalFrames
    
    // 塔吊1 (蓝色)
    const crane1X = 50 + progress * 100
    const crane1Y = height / 2
    ctx.fillStyle = '#409eff'
    ctx.fillRect(crane1X - 10, crane1Y - 10, 20, 20)
    
    // 塔吊2 (红色)
    const crane2X = 250 - progress * 100
    const crane2Y = height / 2
    ctx.fillStyle = '#f56c6c'
    ctx.fillRect(crane2X - 10, crane2Y - 10, 20, 20)
    
    // 绘制工作半径
    ctx.strokeStyle = 'rgba(64, 158, 255, 0.3)'
    ctx.beginPath()
    ctx.arc(crane1X, crane1Y, 30, 0, Math.PI * 2)
    ctx.stroke()
    
    ctx.strokeStyle = 'rgba(245, 108, 108, 0.3)'
    ctx.beginPath()
    ctx.arc(crane2X, crane2Y, 30, 0, Math.PI * 2)
    ctx.stroke()
    
    // 危险区域
    if (progress > 0.7) {
      ctx.fillStyle = 'rgba(255, 0, 0, 0.2)'
      const overlapX = (crane1X + crane2X) / 2
      ctx.fillRect(overlapX - 40, crane1Y - 30, 80, 60)
    }
    
    frame++
    if (frame <= totalFrames) {
      requestAnimationFrame(animate)
    }
  }
  
  animate()
}

/// <summary>
/// 检测异常值
/// </summary>
const detectAnomalies = (data: any) => {
  // 检测负载异常
  if (data.load > selectedCrane.value.maxLoad * 0.9) {
    ElMessage.warning('负载接近上限，请注意安全！')
  }
  
  // 检测应力异常
  if (data.stress > 90) {
    ElMessage.error('结构应力过高，建议立即检查！')
  }
  
  // 检测振动异常
  if (data.vibration > 4.0) {
    ElMessage.warning('振动异常，可能存在机械故障！')
  }
}

/// <summary>
/// 获取负载等级样式
/// </summary>
const getLoadClass = (load: number, maxLoad: number): string => {
  const ratio = load / maxLoad
  if (ratio >= 0.9) return 'critical'
  if (ratio >= 0.7) return 'warning'
  return 'normal'
}

/// <summary>
/// 获取负载进度条颜色
/// </summary>
const getLoadProgressColor = (load: number, maxLoad: number): string => {
  const ratio = load / maxLoad
  if (ratio >= 0.9) return '#f56c6c'
  if (ratio >= 0.7) return '#e6a23c'
  return '#67c23a'
}

/// <summary>
/// 获取应力颜色
/// </summary>
const getStressColor = (stress: number): string => {
  if (stress >= 80) return 'linear-gradient(90deg, #f56c6c, #ff8a80)'
  if (stress >= 60) return 'linear-gradient(90deg, #e6a23c, #ffb74d)'
  return 'linear-gradient(90deg, #67c23a, #81c784)'
}

/// <summary>
/// 获取采样频率等级样式
/// </summary>
const getSamplingClass = (frequency: number): string => {
  if (frequency >= 10) return 'excellent'
  if (frequency >= 8) return 'good'
  return 'poor'
}

/// <summary>
/// 获取同步精度等级样式
/// </summary>
const getSyncClass = (error: number): string => {
  if (error <= 0.5) return 'excellent'
  if (error <= 1.0) return 'good'
  return 'poor'
}

// 数据更新定时器
let dataUpdateInterval: NodeJS.Timeout
let stressUpdateInterval: NodeJS.Timeout

onMounted(async () => {
  await nextTick()
  
  // 初始化可视化引擎
  await initVisualization()
  
  // 初始化传感器数据采集
  initSensorDataCollection()
  
  // 定期更新塔吊状态 - 模拟实时数据
  dataUpdateInterval = setInterval(() => {
    cranes.value.forEach(crane => {
      if (crane.status === 'online') {
        // 模拟微小的数据变化
        crane.load += (Math.random() - 0.5) * 0.1
        crane.load = Math.max(0, Math.min(crane.load, crane.maxLoad))
        
        crane.stressLevel += (Math.random() - 0.5) * 2
        crane.stressLevel = Math.max(0, Math.min(crane.stressLevel, 100))
        
        crane.sensors.vibration += (Math.random() - 0.5) * 0.1
        crane.sensors.vibration = Math.max(0, Math.min(crane.sensors.vibration, 5))
        
        // 更新3D模型
        engine?.updateCraneRealtime(crane.id, crane)
      }
    })
  }, 1000) // 每秒更新一次显示数据
  
  // 定期更新应力分析
  stressUpdateInterval = setInterval(() => {
    // 模拟应力分析数据变化
    stressNodes.value.forEach(node => {
      node.stress += (Math.random() - 0.5) * 5
      node.stress = Math.max(50, Math.min(node.stress, 300))
      
      // 更新状态
      if (node.stress > 200) {
        node.status = '高应力'
        node.level = 'critical'
      } else if (node.stress > 150) {
        node.status = '预警'
        node.level = 'warning'
      } else {
        node.status = '正常'
        node.level = 'normal'
      }
    })
    
    // 更新统计
    stressAnalysis.value.critical = stressNodes.value.filter(n => n.level === 'critical').length
    stressAnalysis.value.warning = stressNodes.value.filter(n => n.level === 'warning').length
    stressAnalysis.value.normal = stressNodes.value.filter(n => n.level === 'normal').length
  }, 5000) // 每5秒更新应力分析
})

onUnmounted(() => {
  if (dataUpdateInterval) clearInterval(dataUpdateInterval)
  if (stressUpdateInterval) clearInterval(stressUpdateInterval)
  
  engine?.destroy()
})
</script>

<style lang="scss" scoped>
.crane-monitoring-dashboard {
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
      
      .sampling-status {
        display: flex;
        flex-direction: column;
        gap: 4px;
        
        .status-indicator {
          display: flex;
          align-items: center;
          gap: 6px;
          font-size: 14px;
          
          &.online {
            color: #67c23a;
          }
        }
        
        .sync-accuracy {
          font-size: 12px;
          color: rgba(255, 255, 255, 0.7);
        }
      }
    }
    
    .control-right {
      display: flex;
      align-items: center;
      gap: 20px;
      
      .collision-status {
        display: flex;
        flex-direction: column;
        gap: 4px;
        
        .collision-indicator {
          display: flex;
          align-items: center;
          gap: 6px;
          font-size: 14px;
          
          &.safe {
            color: #67c23a;
          }
          
          &.warning {
            color: #e6a23c;
          }
        }
        
        .safety-distance {
          font-size: 12px;
          color: rgba(255, 255, 255, 0.7);
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
      
      .crane-status-panel {
        position: absolute;
        top: 20px;
        left: 20px;
        width: 280px;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 8px;
        padding: 16px;
        backdrop-filter: blur(10px);
        
        h4 {
          margin: 0 0 12px 0;
          color: white;
          font-size: 14px;
        }
        
        .crane-grid {
          display: flex;
          flex-direction: column;
          gap: 12px;
          max-height: 400px;
          overflow-y: auto;
          
          .crane-item {
            padding: 12px;
            border-radius: 8px;
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
            
            &.warning {
              border-color: #e6a23c;
            }
            
            .crane-header {
              display: flex;
              justify-content: space-between;
              align-items: center;
              margin-bottom: 8px;
              
              .crane-name {
                font-weight: 600;
                color: white;
              }
              
              .crane-status {
                font-size: 12px;
                color: rgba(255, 255, 255, 0.7);
              }
            }
            
            .crane-metrics {
              display: grid;
              grid-template-columns: 1fr 1fr;
              gap: 6px;
              margin-bottom: 8px;
              
              .metric-row {
                display: flex;
                justify-content: space-between;
                font-size: 12px;
                
                .label {
                  color: rgba(255, 255, 255, 0.7);
                }
                
                .value {
                  color: white;
                  font-weight: 600;
                  
                  &.critical {
                    color: #f56c6c;
                  }
                  
                  &.warning {
                    color: #e6a23c;
                  }
                  
                  &.normal {
                    color: #67c23a;
                  }
                }
              }
            }
            
            .stress-indicator {
              .stress-bar {
                width: 100%;
                height: 4px;
                background: rgba(255, 255, 255, 0.2);
                border-radius: 2px;
                overflow: hidden;
                margin-bottom: 4px;
                
                .stress-fill {
                  height: 100%;
                  transition: width 0.3s ease;
                  border-radius: 2px;
                }
              }
              
              .stress-label {
                font-size: 10px;
                color: rgba(255, 255, 255, 0.6);
              }
            }
            
            .warning-badge {
              position: absolute;
              top: 8px;
              right: 8px;
              width: 16px;
              height: 16px;
              background: #e6a23c;
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
      
      .collision-alert-popup {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 450px;
        background: rgba(0, 0, 0, 0.95);
        border-radius: 12px;
        border: 2px solid #e6a23c;
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
            color: #e6a23c;
            font-size: 16px;
          }
        }
        
        .alert-content {
          padding: 20px;
          
          .collision-info {
            margin-bottom: 16px;
            
            .collision-cranes {
              display: flex;
              align-items: center;
              gap: 8px;
              font-size: 16px;
              font-weight: 600;
              margin-bottom: 8px;
            }
            
            .collision-distance {
              margin-bottom: 4px;
              
              .distance {
                color: #f56c6c;
                font-weight: 600;
              }
            }
            
            .collision-time {
              .time {
                color: #e6a23c;
                font-weight: 600;
              }
            }
          }
          
          .collision-simulation {
            border: 1px solid rgba(255, 255, 255, 0.2);
            border-radius: 8px;
            overflow: hidden;
          }
        }
        
        .alert-actions {
          display: flex;
          gap: 12px;
          padding: 16px 20px;
          border-top: 1px solid rgba(255, 255, 255, 0.1);
        }
      }
      
      .gesture-indicator {
        position: absolute;
        bottom: 20px;
        right: 20px;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 8px;
        padding: 16px;
        backdrop-filter: blur(10px);
        
        .gesture-status {
          display: flex;
          align-items: center;
          gap: 8px;
          margin-bottom: 12px;
          color: #409eff;
          font-weight: 600;
        }
        
        .gesture-hints {
          .hint-item {
            font-size: 12px;
            color: rgba(255, 255, 255, 0.7);
            margin-bottom: 4px;
            
            &:last-child {
              margin-bottom: 0;
            }
          }
        }
      }
    }
    
    .control-panel {
      width: 450px;
      display: flex;
      flex-direction: column;
      gap: 20px;
      
      .realtime-monitoring-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .monitoring-content {
          .metrics-cards {
            display: flex;
            gap: 12px;
            margin-bottom: 20px;
            
            .metric-card {
              flex: 1;
              display: flex;
              align-items: center;
              gap: 8px;
              padding: 12px;
              background: rgba(255, 255, 255, 0.05);
              border-radius: 8px;
              
              .metric-icon {
                width: 32px;
                height: 32px;
                border-radius: 6px;
                display: flex;
                align-items: center;
                justify-content: center;
                
                &.load {
                  background: linear-gradient(135deg, #409eff, #66b1ff);
                }
                
                &.height {
                  background: linear-gradient(135deg, #67c23a, #85ce61);
                }
                
                &.rotation {
                  background: linear-gradient(135deg, #e6a23c, #f0a020);
                }
              }
              
              .metric-content {
                flex: 1;
                
                .metric-value {
                  font-size: 16px;
                  font-weight: 700;
                  color: white;
                  line-height: 1;
                }
                
                .metric-label {
                  font-size: 11px;
                  color: rgba(255, 255, 255, 0.6);
                  margin-top: 2px;
                }
                
                .metric-progress {
                  margin-top: 4px;
                }
                
                .metric-trend {
                  margin-top: 2px;
                  
                  .up {
                    color: #67c23a;
                  }
                  
                  .down {
                    color: #f56c6c;
                  }
                  
                  .stable {
                    color: #909399;
                  }
                }
                
                .metric-speed {
                  font-size: 10px;
                  color: rgba(255, 255, 255, 0.5);
                  margin-top: 2px;
                }
              }
            }
          }
          
          .sensor-charts {
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .chart-container {
              border: 1px solid rgba(255, 255, 255, 0.2);
              border-radius: 8px;
              overflow: hidden;
              margin-bottom: 8px;
            }
            
            .chart-legend {
              display: flex;
              gap: 16px;
              
              .legend-item {
                display: flex;
                align-items: center;
                gap: 6px;
                font-size: 12px;
                color: rgba(255, 255, 255, 0.7);
                
                .color-indicator {
                  width: 12px;
                  height: 3px;
                  border-radius: 1px;
                  
                  &.load {
                    background: #409eff;
                  }
                  
                  &.stress {
                    background: #f56c6c;
                  }
                  
                  &.vibration {
                    background: #67c23a;
                  }
                }
              }
            }
          }
        }
        
        .no-selection {
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          gap: 8px;
          padding: 40px;
          color: #666;
        }
      }
      
      .stress-analysis-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .stress-content {
          .stress-overview {
            margin-bottom: 16px;
            
            .stress-summary {
              display: flex;
              gap: 16px;
              
              .summary-item {
                flex: 1;
                text-align: center;
                padding: 12px;
                border-radius: 8px;
                
                &.critical {
                  background: rgba(245, 108, 108, 0.1);
                  border: 1px solid #f56c6c;
                }
                
                &.warning {
                  background: rgba(230, 162, 60, 0.1);
                  border: 1px solid #e6a23c;
                }
                
                &.normal {
                  background: rgba(103, 194, 58, 0.1);
                  border: 1px solid #67c23a;
                }
                
                .count {
                  display: block;
                  font-size: 20px;
                  font-weight: 700;
                  color: white;
                }
                
                .label {
                  font-size: 12px;
                  color: rgba(255, 255, 255, 0.7);
                }
              }
            }
          }
          
          .key-nodes {
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .nodes-list {
              max-height: 180px;
              overflow-y: auto;
              
              .node-item {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 8px 12px;
                border-radius: 6px;
                margin-bottom: 6px;
                cursor: pointer;
                transition: background-color 0.3s;
                
                &:hover {
                  background: rgba(255, 255, 255, 0.05);
                }
                
                &.critical {
                  border-left: 3px solid #f56c6c;
                }
                
                &.warning {
                  border-left: 3px solid #e6a23c;
                }
                
                &.normal {
                  border-left: 3px solid #67c23a;
                }
                
                .node-name {
                  font-size: 12px;
                  color: white;
                  font-weight: 600;
                }
                
                .node-stress {
                  font-size: 12px;
                  color: rgba(255, 255, 255, 0.8);
                }
                
                .node-status {
                  font-size: 11px;
                  color: rgba(255, 255, 255, 0.6);
                }
              }
            }
          }
        }
      }
      
      .crane-control-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .control-content {
          .motion-controls {
            .control-group {
              margin-bottom: 16px;
              
              h4 {
                margin: 0 0 8px 0;
                color: white;
                font-size: 14px;
              }
              
              .control-buttons {
                display: flex;
                gap: 8px;
              }
            }
          }
          
          .safety-controls {
            margin: 20px 0;
            
            .emergency-stop {
              width: 100%;
              height: 48px;
              font-size: 16px;
              font-weight: 600;
            }
          }
          
          .control-status {
            background: rgba(64, 158, 255, 0.1);
            border: 1px solid #409eff;
            border-radius: 8px;
            padding: 12px;
            
            .status-text {
              display: flex;
              align-items: center;
              gap: 8px;
              margin-bottom: 8px;
              color: #409eff;
              font-weight: 600;
            }
          }
        }
        
        .control-disabled {
          display: flex;
          flex-direction: column;
          align-items: center;
          justify-content: center;
          gap: 8px;
          padding: 40px;
          color: #666;
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
              
              &.poor {
                color: #f56c6c;
              }
            }
          }
        }
      }
    }
  }
}
</style> 