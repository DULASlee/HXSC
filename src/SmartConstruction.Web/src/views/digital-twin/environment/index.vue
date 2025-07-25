<template>
  <div class="environment-monitoring-dashboard">
    <!-- 顶部控制栏 -->
    <div class="control-bar">
      <div class="control-left">
        <h2 class="page-title">🌬️ 扬尘噪音管理大屏</h2>
        <div class="monitoring-status">
          <div class="status-indicator" :class="monitoringStatus.active ? 'online' : 'offline'">
            <el-icon><Sunny /></el-icon>
            监测站点: {{ monitoringStatus.activeStations }}/{{ monitoringStatus.totalStations }}
          </div>
          <div class="air-quality">
            空气质量: {{ airQualityIndex }} ({{ getAirQualityLevel(airQualityIndex) }})
          </div>
        </div>
      </div>
      
      <div class="control-right">
        <div class="alert-status">
          <div class="alert-indicator" :class="alertStatus.level">
            <el-icon><Warning /></el-icon>
            {{ alertStatus.level === 'critical' ? '严重污染' : alertStatus.level === 'warning' ? '轻度污染' : '环境良好' }}
          </div>
          <div class="exceeding-stations">
            超标站点: {{ alertStatus.exceedingStations }}
          </div>
        </div>
        
        <div class="action-buttons">
          <el-button size="small" @click="toggleDiffusionSimulation">
            <el-icon><TrendCharts /></el-icon>
            {{ diffusionSimulation ? '关闭扩散' : '扩散模拟' }}
          </el-button>
          <el-button size="small" @click="toggleHistoryComparison">
            <el-icon><Clock /></el-icon>
            历史对比
          </el-button>
          <el-button size="small" @click="activateEmergencyTreatment">
            <el-icon><Tools /></el-icon>
            应急治理
          </el-button>
        </div>
      </div>
    </div>

    <!-- 主要内容区域 -->
    <div class="main-content">
      <!-- 3D场景区域 -->
      <div class="scene-section">
        <div ref="threejsContainer" class="threejs-container"></div>
        
        <!-- 监测站点面板 -->
        <div class="stations-panel">
          <h4>环境监测站点</h4>
          <div class="stations-grid">
            <div 
              v-for="station in monitoringStations" 
              :key="station.id"
              class="station-item"
              :class="{ 
                active: selectedStation?.id === station.id,
                online: station.status === 'online',
                offline: station.status === 'offline',
                exceeding: station.isExceeding
              }"
              @click="selectStation(station)"
            >
              <div class="station-header">
                <div class="station-name">{{ station.name }}</div>
                <div class="station-status">{{ station.status === 'online' ? '在线' : '离线' }}</div>
              </div>
              
              <div class="station-metrics">
                <div class="metric-row dust">
                  <span class="label">PM2.5:</span>
                  <span class="value" :class="getDustClass(station.pm25)">{{ station.pm25 }}μg/m³</span>
                </div>
                <div class="metric-row dust">
                  <span class="label">PM10:</span>
                  <span class="value" :class="getDustClass(station.pm10)">{{ station.pm10 }}μg/m³</span>
                </div>
                <div class="metric-row noise">
                  <span class="label">噪音:</span>
                  <span class="value" :class="getNoiseClass(station.noise)">{{ station.noise }}dB</span>
                </div>
                <div class="metric-row wind">
                  <span class="label">风速:</span>
                  <span class="value">{{ station.windSpeed }}m/s</span>
                </div>
              </div>
              
              <div v-if="station.isExceeding" class="exceeding-badge">
                <el-icon><Warning /></el-icon>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 智能治理建议弹窗 -->
        <div v-if="treatmentSuggestion" class="treatment-popup">
          <div class="popup-header">
            <h3>🤖 智能治理建议</h3>
            <el-button text @click="dismissTreatment">
              <el-icon><Close /></el-icon>
            </el-button>
          </div>
          <div class="popup-content">
            <div class="pollution-analysis">
              <div class="analysis-item">
                <span class="label">污染类型:</span>
                <span class="value">{{ treatmentSuggestion.pollutionType }}</span>
              </div>
              <div class="analysis-item">
                <span class="label">污染程度:</span>
                <span class="value" :class="treatmentSuggestion.severity">{{ treatmentSuggestion.severityText }}</span>
              </div>
              <div class="analysis-item">
                <span class="label">影响范围:</span>
                <span class="value">{{ treatmentSuggestion.affectedRadius }}m</span>
              </div>
            </div>
            
            <div class="treatment-plan">
              <h4>推荐治理方案</h4>
              <div class="plan-steps">
                <div v-for="(step, index) in treatmentSuggestion.steps" :key="index" class="step-item">
                  <div class="step-number">{{ index + 1 }}</div>
                  <div class="step-content">
                    <div class="step-action">{{ step.action }}</div>
                    <div class="step-description">{{ step.description }}</div>
                    <div class="step-duration">预计时长: {{ step.duration }}</div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="popup-actions">
            <el-button type="primary" @click="executeTreatment">
              执行治理方案
            </el-button>
            <el-button @click="customizeTreatment">
              自定义方案
            </el-button>
            <el-button @click="dismissTreatment">
              稍后处理
            </el-button>
          </div>
        </div>
        
        <!-- 历史对比滑块 -->
        <div v-if="historyComparison" class="history-slider">
          <div class="slider-header">
            <h4>历史数据对比</h4>
            <div class="time-display">{{ formatHistoryTime(historyTimeIndex) }}</div>
          </div>
          <el-slider
            v-model="historyTimeIndex"
            :min="0"
            :max="historyData.length - 1"
            :marks="historyMarks"
            @change="updateHistoryComparison"
          />
          <div class="comparison-stats">
            <div class="stat-item">
              <span class="label">与当前对比:</span>
              <span class="value" :class="historyComparison.trend">
                {{ historyComparison.changeText }}
              </span>
            </div>
          </div>
        </div>
      </div>
      
      <!-- 控制面板 -->
      <div class="control-panel">
        <!-- 实时监测数据 -->
        <div class="realtime-section">
          <h3>实时监测数据</h3>
          <div v-if="selectedStation" class="realtime-content">
            <!-- 关键指标卡片 -->
            <div class="metrics-cards">
              <div class="metric-card dust">
                <div class="metric-icon">
                  <el-icon><Cloudy /></el-icon>
                </div>
                <div class="metric-content">
                  <div class="metric-value">{{ selectedStation.pm25 }}</div>
                  <div class="metric-unit">μg/m³</div>
                  <div class="metric-label">PM2.5</div>
                  <div class="metric-status" :class="getDustClass(selectedStation.pm25)">
                    {{ getDustStatusText(selectedStation.pm25) }}
                  </div>
                </div>
              </div>
              
              <div class="metric-card noise">
                <div class="metric-icon">
                  <el-icon><Bell /></el-icon>
                </div>
                <div class="metric-content">
                  <div class="metric-value">{{ selectedStation.noise }}</div>
                  <div class="metric-unit">dB</div>
                  <div class="metric-label">噪音</div>
                  <div class="metric-status" :class="getNoiseClass(selectedStation.noise)">
                    {{ getNoiseStatusText(selectedStation.noise) }}
                  </div>
                </div>
              </div>
              
              <div class="metric-card weather">
                <div class="metric-icon">
                  <el-icon><Sunny /></el-icon>
                </div>
                <div class="metric-content">
                  <div class="metric-value">{{ selectedStation.windSpeed }}</div>
                  <div class="metric-unit">m/s</div>
                  <div class="metric-label">风速</div>
                  <div class="metric-extra">{{ selectedStation.windDirection }}</div>
                </div>
              </div>
            </div>
            
            <!-- 实时趋势图 -->
            <div class="trend-charts">
              <h4>24小时趋势</h4>
              <div class="chart-tabs">
                <el-tabs v-model="activeChartTab" @tab-change="updateChart">
                  <el-tab-pane label="扬尘" name="dust">
                    <canvas ref="dustChart" width="360" height="200"></canvas>
                  </el-tab-pane>
                  <el-tab-pane label="噪音" name="noise">
                    <canvas ref="noiseChart" width="360" height="200"></canvas>
                  </el-tab-pane>
                  <el-tab-pane label="气象" name="weather">
                    <canvas ref="weatherChart" width="360" height="200"></canvas>
                  </el-tab-pane>
                </el-tabs>
              </div>
            </div>
          </div>
          <div v-else class="no-selection">
            <el-icon><Monitor /></el-icon>
            <span>请选择监测站点查看详细数据</span>
          </div>
        </div>
        
        <!-- 污染扩散分析 -->
        <div class="diffusion-section">
          <h3>污染扩散分析</h3>
          <div class="diffusion-content">
            <div class="diffusion-controls">
              <div class="control-row">
                <label>扩散模型:</label>
                <el-select v-model="diffusionModel" size="small" @change="updateDiffusionModel">
                  <el-option label="高斯扩散" value="gaussian"></el-option>
                  <el-option label="拉格朗日" value="lagrangian"></el-option>
                  <el-option label="欧拉网格" value="eulerian"></el-option>
                </el-select>
              </div>
              <div class="control-row">
                <label>风速影响:</label>
                <el-slider v-model="windInfluence" :min="0" :max="100" @change="updateDiffusion" />
              </div>
              <div class="control-row">
                <label>稳定度:</label>
                <el-select v-model="atmosphericStability" size="small" @change="updateDiffusion">
                  <el-option label="极不稳定" value="A"></el-option>
                  <el-option label="不稳定" value="B"></el-option>
                  <el-option label="轻微不稳定" value="C"></el-option>
                  <el-option label="中性" value="D"></el-option>
                  <el-option label="轻微稳定" value="E"></el-option>
                  <el-option label="稳定" value="F"></el-option>
                </el-select>
              </div>
            </div>
            
            <div class="diffusion-results">
              <h4>扩散预测</h4>
              <div class="prediction-list">
                <div v-for="prediction in diffusionPredictions" :key="prediction.time" class="prediction-item">
                  <div class="prediction-time">{{ prediction.time }}小时后</div>
                  <div class="prediction-range">扩散半径: {{ prediction.radius }}m</div>
                  <div class="prediction-concentration">浓度: {{ prediction.concentration }}μg/m³</div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 智能联动控制 -->
        <div class="linkage-section">
          <h3>智能联动控制</h3>
          <div class="linkage-content">
            <div class="linkage-rules">
              <h4>联动规则</h4>
              <div class="rule-list">
                <div v-for="rule in linkageRules" :key="rule.id" class="rule-item" :class="{ active: rule.active }">
                  <div class="rule-header">
                    <div class="rule-name">{{ rule.name }}</div>
                    <el-switch v-model="rule.active" @change="toggleRule(rule)" />
                  </div>
                  <div class="rule-condition">{{ rule.condition }}</div>
                  <div class="rule-action">{{ rule.action }}</div>
                </div>
              </div>
            </div>
            
            <div class="active-treatments">
              <h4>活跃治理措施</h4>
              <div class="treatment-list">
                <div v-for="treatment in activeTreatments" :key="treatment.id" class="treatment-item">
                  <div class="treatment-device">{{ treatment.device }}</div>
                  <div class="treatment-status">{{ treatment.status }}</div>
                  <div class="treatment-progress">
                    <el-progress :percentage="treatment.progress" :status="treatment.progressStatus" />
                  </div>
                  <div class="treatment-actions">
                    <el-button size="small" @click="stopTreatment(treatment)">停止</el-button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 系统状态 -->
        <div class="system-section">
          <h3>系统状态</h3>
          <div class="system-content">
            <div class="system-metrics">
              <div class="metric-item">
                <span class="metric-label">数据完整性:</span>
                <span class="metric-value">{{ systemMetrics.dataIntegrity }}%</span>
              </div>
              <div class="metric-item">
                <span class="metric-label">响应时间:</span>
                <span class="metric-value">{{ systemMetrics.responseTime }}ms</span>
              </div>
              <div class="metric-item">
                <span class="metric-label">治理效率:</span>
                <span class="metric-value">{{ systemMetrics.treatmentEfficiency }}%</span>
              </div>
              <div class="metric-item">
                <span class="metric-label">预警准确率:</span>
                <span class="metric-value">{{ systemMetrics.alertAccuracy }}%</span>
              </div>
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
  Sunny, Warning, TrendCharts, Clock, Tools, Close, Cloudy, 
  Bell, Monitor
} from '@element-plus/icons-vue'
import { digitalTwinApi } from '@/api/modules/digitalTwin'
import { EnvironmentVisualizationEngine } from './engine/EnvironmentVisualizationEngine'

// 组件状态
const threejsContainer = ref<HTMLElement>()
const dustChart = ref<HTMLCanvasElement>()
const noiseChart = ref<HTMLCanvasElement>()
const weatherChart = ref<HTMLCanvasElement>()
const selectedStation = ref<any>(null)
const activeChartTab = ref('dust')
const diffusionSimulation = ref(true)
const historyComparison = ref(false)
const historyTimeIndex = ref(23)

// 3D引擎实例
let engine: EnvironmentVisualizationEngine | null = null

// 监测状态
const monitoringStatus = ref({
  active: true,
  activeStations: 8,
  totalStations: 10
})

// 空气质量指数
const airQualityIndex = ref(87)

// 警报状态
const alertStatus = ref({
  level: 'warning', // 'normal', 'warning', 'critical'
  exceedingStations: 2
})

// 监测站点数据
const monitoringStations = ref([
  {
    id: 'env001',
    name: '1号监测站',
    position: { x: 0, y: 0, z: 50 },
    status: 'online',
    pm25: 45,
    pm10: 82,
    noise: 68,
    windSpeed: 3.2,
    windDirection: '东南风',
    temperature: 25,
    humidity: 65,
    isExceeding: true
  },
  {
    id: 'env002',
    name: '2号监测站',
    position: { x: 80, y: 0, z: -20 },
    status: 'online',
    pm25: 35,
    pm10: 65,
    noise: 55,
    windSpeed: 2.8,
    windDirection: '西北风',
    temperature: 26,
    humidity: 62,
    isExceeding: false
  },
  {
    id: 'env003',
    name: '3号监测站',
    position: { x: -60, y: 0, z: 30 },
    status: 'online',
    pm25: 95,
    pm10: 145,
    noise: 75,
    windSpeed: 1.5,
    windDirection: '北风',
    temperature: 24,
    humidity: 70,
    isExceeding: true
  },
  {
    id: 'env004',
    name: '4号监测站',
    position: { x: 40, y: 0, z: -80 },
    status: 'offline',
    pm25: 0,
    pm10: 0,
    noise: 0,
    windSpeed: 0,
    windDirection: '',
    temperature: 0,
    humidity: 0,
    isExceeding: false
  }
])

// 扩散模型参数
const diffusionModel = ref('gaussian')
const windInfluence = ref(60)
const atmosphericStability = ref('D')

// 扩散预测
const diffusionPredictions = ref([
  { time: 1, radius: 120, concentration: 65 },
  { time: 2, radius: 180, concentration: 48 },
  { time: 4, radius: 280, concentration: 32 },
  { time: 8, radius: 450, concentration: 18 }
])

// 智能联动规则
const linkageRules = ref([
  {
    id: 'rule1',
    name: 'PM2.5超标自动喷淋',
    condition: 'PM2.5 > 75μg/m³ 且 风速 < 2m/s',
    action: '启动半径200m内所有喷淋设备',
    active: true
  },
  {
    id: 'rule2',
    name: '噪音超标施工限制',
    condition: '噪音 > 70dB 且 时间在22:00-06:00',
    action: '暂停高噪音设备作业，发送通知',
    active: true
  },
  {
    id: 'rule3',
    name: '重污染应急响应',
    condition: 'PM10 > 150μg/m³ 持续30分钟',
    action: '启动全场雾炮车，通知环保部门',
    active: false
  }
])

// 活跃治理措施
const activeTreatments = ref([
  {
    id: 'treat1',
    device: '1号雾炮车',
    status: '运行中',
    progress: 75,
    progressStatus: 'success'
  },
  {
    id: 'treat2',
    device: '喷淋系统A区',
    status: '运行中',
    progress: 60,
    progressStatus: 'success'
  }
])

// 治理建议
const treatmentSuggestion = ref<any>(null)

// 历史数据
const historyData = ref(generateHistoryData())
const historyMarks = ref({
  0: '00:00',
  6: '06:00',
  12: '12:00',
  18: '18:00',
  23: '23:00'
})

// 系统指标
const systemMetrics = ref({
  dataIntegrity: 98.5,
  responseTime: 156,
  treatmentEfficiency: 89,
  alertAccuracy: 94
})

/// <summary>
/// 初始化3D可视化引擎
/// </summary>
const initVisualization = async () => {
  if (!threejsContainer.value) return
  
  try {
    engine = new EnvironmentVisualizationEngine(threejsContainer.value)
    await engine.init()
    
    // 设置事件监听
    engine.on('stationClick', handleStationClick)
    engine.on('pollutionAlert', handlePollutionAlert)
    
    // 初始化监测站点
    monitoringStations.value.forEach(station => {
      engine?.addMonitoringStation(station)
    })
    
    // 启用扩散模拟
    if (diffusionSimulation.value) {
      engine?.enableDiffusionSimulation(true)
    }
    
    // 开始渲染
    engine.startRender()
    
    ElMessage.success('环境监测系统初始化成功')
    
  } catch (error) {
    console.error('初始化失败:', error)
    ElMessage.error('系统初始化失败')
  }
}

/// <summary>
/// 选择监测站点
/// </summary>
const selectStation = (station: any) => {
  selectedStation.value = station
  
  // 在3D场景中高亮站点
  engine?.highlightStation(station.id)
  
  // 更新图表
  updateChart(activeChartTab.value)
  
  ElMessage.info(`已选择: ${station.name}`)
}

/// <summary>
/// 站点点击事件
/// </summary>
const handleStationClick = (stationData: any) => {
  const station = monitoringStations.value.find(s => s.id === stationData.id)
  if (station) {
    selectStation(station)
  }
}

/// <summary>
/// 污染警报处理
/// </summary>
const handlePollutionAlert = (alertData: any) => {
  // 触发智能治理建议
  generateTreatmentSuggestion(alertData)
  
  ElMessage.warning(`污染预警: ${alertData.location} ${alertData.pollutant}超标`)
}

/// <summary>
/// 生成治理建议
/// </summary>
const generateTreatmentSuggestion = (alertData: any) => {
  treatmentSuggestion.value = {
    pollutionType: alertData.pollutant === 'PM25' ? 'PM2.5扬尘污染' : '噪音污染',
    severity: alertData.level,
    severityText: alertData.level === 'critical' ? '严重超标' : '轻度超标',
    affectedRadius: 200,
    steps: [
      {
        action: '启动喷淋系统',
        description: '激活污染源周围200米范围内的高压喷淋设备',
        duration: '10分钟'
      },
      {
        action: '部署雾炮车',
        description: '调度最近的雾炮车到污染中心进行定点治理',
        duration: '30分钟'
      },
      {
        action: '暂停作业',
        description: '通知相关施工班组暂停易产尘作业',
        duration: '待环境改善'
      }
    ]
  }
}

/// <summary>
/// 切换扩散模拟
/// </summary>
const toggleDiffusionSimulation = () => {
  diffusionSimulation.value = !diffusionSimulation.value
  engine?.enableDiffusionSimulation(diffusionSimulation.value)
  
  if (diffusionSimulation.value) {
    updateDiffusion()
  }
  
  ElMessage.info(`污染扩散模拟${diffusionSimulation.value ? '已开启' : '已关闭'}`)
}

/// <summary>
/// 切换历史对比
/// </summary>
const toggleHistoryComparison = () => {
  historyComparison.value = !historyComparison.value
  
  if (historyComparison.value) {
    updateHistoryComparison(historyTimeIndex.value)
  } else {
    // 恢复当前数据显示
    engine?.showCurrentData()
  }
}

/// <summary>
/// 更新扩散模拟
/// </summary>
const updateDiffusion = () => {
  if (!engine || !diffusionSimulation.value) return
  
  const params = {
    model: diffusionModel.value,
    windInfluence: windInfluence.value / 100,
    stability: atmosphericStability.value,
    sources: monitoringStations.value.filter(s => s.isExceeding)
  }
  
  engine.updateDiffusionParameters(params)
  
  // 更新扩散预测
  updateDiffusionPredictions(params)
}

/// <summary>
/// 更新扩散预测
/// </summary>
const updateDiffusionPredictions = (params: any) => {
  // 基于高斯扩散模型的简化计算
  const baseRadius = 100
  const windFactor = 1 + params.windInfluence
  const stabilityFactor = getStabilityFactor(params.stability)
  
  diffusionPredictions.value = [1, 2, 4, 8].map(time => ({
    time,
    radius: Math.round(baseRadius * Math.sqrt(time) * windFactor * stabilityFactor),
    concentration: Math.round(85 / (1 + time * 0.3))
  }))
}

/// <summary>
/// 获取稳定度系数
/// </summary>
const getStabilityFactor = (stability: string): number => {
  const factors = { A: 1.5, B: 1.3, C: 1.1, D: 1.0, E: 0.8, F: 0.6 }
  return factors[stability as keyof typeof factors] || 1.0
}

/// <summary>
/// 更新历史对比
/// </summary>
const updateHistoryComparison = (index: number) => {
  const historyPoint = historyData.value[index]
  if (!historyPoint || !engine) return
  
  // 在3D场景中显示历史数据
  engine.showHistoryData(historyPoint.data)
  
  // 计算变化趋势
  const currentAvg = monitoringStations.value.reduce((sum, s) => sum + s.pm25, 0) / monitoringStations.value.length
  const historyAvg = historyPoint.data.reduce((sum: number, s: any) => sum + s.pm25, 0) / historyPoint.data.length
  const change = ((currentAvg - historyAvg) / historyAvg * 100).toFixed(1)
  
  historyComparison.value = {
    trend: change > 0 ? 'worse' : 'better',
    changeText: `${change > 0 ? '上升' : '下降'}${Math.abs(parseFloat(change))}%`
  } as any
}

/// <summary>
/// 激活应急治理
/// </summary>
const activateEmergencyTreatment = async () => {
  try {
    await ElMessageBox.confirm(
      '确认激活应急治理模式？这将启动所有可用的治理设备。',
      '应急治理确认',
      {
        confirmButtonText: '立即激活',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 模拟启动治理设备
    const emergencyTreatments = [
      { id: 'emergency1', device: '全场雾炮车', status: '启动中', progress: 0 },
      { id: 'emergency2', device: '全区域喷淋', status: '启动中', progress: 0 },
      { id: 'emergency3', device: '移动净化车', status: '调度中', progress: 0 }
    ]
    
    activeTreatments.value.push(...emergencyTreatments)
    
    // 在3D场景中显示治理效果
    engine?.activateEmergencyTreatment()
    
    ElMessage.success('应急治理模式已激活')
    
  } catch {
    // 用户取消
  }
}

/// <summary>
/// 执行治理方案
/// </summary>
const executeTreatment = () => {
  if (!treatmentSuggestion.value) return
  
  // 执行治理步骤
  treatmentSuggestion.value.steps.forEach((step: any, index: number) => {
    setTimeout(() => {
      const treatment = {
        id: `auto_${Date.now()}_${index}`,
        device: step.action,
        status: '执行中',
        progress: 0,
        progressStatus: 'success'
      }
      
      activeTreatments.value.push(treatment)
      
      // 模拟进度更新
      const progressInterval = setInterval(() => {
        treatment.progress += 10
        if (treatment.progress >= 100) {
          clearInterval(progressInterval)
          treatment.status = '已完成'
          treatment.progressStatus = 'success'
        }
      }, 1000)
      
    }, index * 2000)
  })
  
  dismissTreatment()
  ElMessage.success('治理方案执行中...')
}

/// <summary>
/// 自定义治理
/// </summary>
const customizeTreatment = () => {
  ElMessage.info('自定义治理功能开发中...')
  dismissTreatment()
}

/// <summary>
/// 关闭治理建议
/// </summary>
const dismissTreatment = () => {
  treatmentSuggestion.value = null
}

/// <summary>
/// 停止治理
/// </summary>
const stopTreatment = (treatment: any) => {
  const index = activeTreatments.value.findIndex(t => t.id === treatment.id)
  if (index > -1) {
    activeTreatments.value.splice(index, 1)
    ElMessage.info(`已停止: ${treatment.device}`)
  }
}

/// <summary>
/// 切换联动规则
/// </summary>
const toggleRule = (rule: any) => {
  ElMessage.info(`联动规则"${rule.name}"${rule.active ? '已启用' : '已禁用'}`)
}

/// <summary>
/// 更新扩散模型
/// </summary>
const updateDiffusionModel = () => {
  updateDiffusion()
}

/// <summary>
/// 更新图表
/// </summary>
const updateChart = (type: string) => {
  if (!selectedStation.value) return
  
  switch (type) {
    case 'dust':
      updateDustChart()
      break
    case 'noise':
      updateNoiseChart()
      break
    case 'weather':
      updateWeatherChart()
      break
  }
}

/// <summary>
/// 更新扬尘图表
/// </summary>
const updateDustChart = () => {
  const canvas = dustChart.value
  if (!canvas) return
  
  const ctx = canvas.getContext('2d')!
  const width = canvas.width
  const height = canvas.height
  
  // 清除画布
  ctx.clearRect(0, 0, width, height)
  
  // 生成24小时模拟数据
  const hours = Array.from({ length: 24 }, (_, i) => i)
  const pm25Data = hours.map(() => 30 + Math.random() * 40)
  const pm10Data = hours.map(() => 50 + Math.random() * 60)
  
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
  
  // 绘制PM2.5线
  ctx.strokeStyle = '#f56c6c'
  ctx.lineWidth = 2
  ctx.beginPath()
  pm25Data.forEach((value, index) => {
    const x = (width / (pm25Data.length - 1)) * index
    const y = height - (value / 100) * height
    
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
  
  // 绘制PM10线
  ctx.strokeStyle = '#e6a23c'
  ctx.lineWidth = 2
  ctx.beginPath()
  pm10Data.forEach((value, index) => {
    const x = (width / (pm10Data.length - 1)) * index
    const y = height - (value / 150) * height
    
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
}

/// <summary>
/// 更新噪音图表
/// </summary>
const updateNoiseChart = () => {
  const canvas = noiseChart.value
  if (!canvas) return
  
  const ctx = canvas.getContext('2d')!
  const width = canvas.width
  const height = canvas.height
  
  // 清除画布
  ctx.clearRect(0, 0, width, height)
  
  // 生成噪音数据
  const hours = Array.from({ length: 24 }, (_, i) => i)
  const noiseData = hours.map(hour => {
    // 白天噪音较高，夜间较低
    const baseNoise = hour >= 6 && hour <= 22 ? 60 : 45
    return baseNoise + Math.random() * 15
  })
  
  // 绘制柱状图
  const barWidth = width / noiseData.length
  noiseData.forEach((value, index) => {
    const x = index * barWidth
    const barHeight = (value / 80) * height
    const y = height - barHeight
    
    // 根据噪音等级设置颜色
    if (value > 70) {
      ctx.fillStyle = '#f56c6c'
    } else if (value > 55) {
      ctx.fillStyle = '#e6a23c'
    } else {
      ctx.fillStyle = '#67c23a'
    }
    
    ctx.fillRect(x, y, barWidth - 1, barHeight)
  })
}

/// <summary>
/// 更新气象图表
/// </summary>
const updateWeatherChart = () => {
  const canvas = weatherChart.value
  if (!canvas) return
  
  const ctx = canvas.getContext('2d')!
  const width = canvas.width
  const height = canvas.height
  
  // 清除画布
  ctx.clearRect(0, 0, width, height)
  
  // 生成气象数据
  const hours = Array.from({ length: 24 }, (_, i) => i)
  const tempData = hours.map(() => 20 + Math.random() * 10)
  const humidityData = hours.map(() => 50 + Math.random() * 30)
  
  // 绘制温度线
  ctx.strokeStyle = '#409eff'
  ctx.lineWidth = 2
  ctx.beginPath()
  tempData.forEach((value, index) => {
    const x = (width / (tempData.length - 1)) * index
    const y = height - ((value - 15) / 20) * height
    
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
  
  // 绘制湿度线
  ctx.strokeStyle = '#67c23a'
  ctx.lineWidth = 2
  ctx.beginPath()
  humidityData.forEach((value, index) => {
    const x = (width / (humidityData.length - 1)) * index
    const y = height - (value / 100) * height
    
    if (index === 0) {
      ctx.moveTo(x, y)
    } else {
      ctx.lineTo(x, y)
    }
  })
  ctx.stroke()
}

/// <summary>
/// 生成历史数据
/// </summary>
function generateHistoryData() {
  return Array.from({ length: 24 }, (_, hour) => ({
    time: hour,
    timestamp: Date.now() - (23 - hour) * 3600000,
    data: monitoringStations.value.map(station => ({
      ...station,
      pm25: station.pm25 + (Math.random() - 0.5) * 20,
      pm10: station.pm10 + (Math.random() - 0.5) * 30,
      noise: station.noise + (Math.random() - 0.5) * 10
    }))
  }))
}

/// <summary>
/// 格式化历史时间
/// </summary>
const formatHistoryTime = (index: number): string => {
  const hour = index.toString().padStart(2, '0')
  return `${hour}:00`
}

/// <summary>
/// 获取扬尘等级样式
/// </summary>
const getDustClass = (value: number): string => {
  if (value > 75) return 'critical'
  if (value > 35) return 'warning'
  return 'normal'
}

/// <summary>
/// 获取噪音等级样式
/// </summary>
const getNoiseClass = (value: number): string => {
  if (value > 70) return 'critical'
  if (value > 55) return 'warning'
  return 'normal'
}

/// <summary>
/// 获取空气质量等级
/// </summary>
const getAirQualityLevel = (index: number): string => {
  if (index > 200) return '重度污染'
  if (index > 150) return '中度污染'
  if (index > 100) return '轻度污染'
  if (index > 50) return '良'
  return '优'
}

/// <summary>
/// 获取扬尘状态文本
/// </summary>
const getDustStatusText = (value: number): string => {
  if (value > 75) return '严重超标'
  if (value > 35) return '轻度超标'
  return '正常'
}

/// <summary>
/// 获取噪音状态文本
/// </summary>
const getNoiseStatusText = (value: number): string => {
  if (value > 70) return '严重超标'
  if (value > 55) return '轻度超标'
  return '正常'
}

// 数据更新定时器
let dataUpdateInterval: NodeJS.Timeout
let linkageCheckInterval: NodeJS.Timeout

onMounted(async () => {
  await nextTick()
  
  // 初始化可视化引擎
  await initVisualization()
  
  // 定期更新监测数据
  dataUpdateInterval = setInterval(() => {
    monitoringStations.value.forEach(station => {
      if (station.status === 'online') {
        // 模拟数据波动
        station.pm25 += (Math.random() - 0.5) * 5
        station.pm25 = Math.max(0, Math.min(station.pm25, 200))
        
        station.pm10 += (Math.random() - 0.5) * 8
        station.pm10 = Math.max(0, Math.min(station.pm10, 300))
        
        station.noise += (Math.random() - 0.5) * 3
        station.noise = Math.max(30, Math.min(station.noise, 90))
        
        // 检查超标状态
        station.isExceeding = station.pm25 > 75 || station.pm10 > 150 || station.noise > 70
        
        // 更新3D显示
        engine?.updateStationData(station.id, station)
      }
    })
    
    // 更新空气质量指数
    const avgPM25 = monitoringStations.value
      .filter(s => s.status === 'online')
      .reduce((sum, s) => sum + s.pm25, 0) / monitoringStations.value.filter(s => s.status === 'online').length
    airQualityIndex.value = Math.round(avgPM25 * 2.5)
    
    // 更新警报状态
    const exceedingCount = monitoringStations.value.filter(s => s.isExceeding).length
    alertStatus.value.exceedingStations = exceedingCount
    if (exceedingCount >= 3) {
      alertStatus.value.level = 'critical'
    } else if (exceedingCount > 0) {
      alertStatus.value.level = 'warning'
    } else {
      alertStatus.value.level = 'normal'
    }
    
    // 更新当前选中站点的图表
    if (selectedStation.value) {
      updateChart(activeChartTab.value)
    }
  }, 3000) // 每3秒更新一次
  
  // 智能联动检查
  linkageCheckInterval = setInterval(() => {
    linkageRules.value.forEach(rule => {
      if (rule.active) {
        checkAndExecuteRule(rule)
      }
    })
  }, 10000) // 每10秒检查一次联动规则
  
  // 初始选择第一个在线站点
  const firstOnlineStation = monitoringStations.value.find(s => s.status === 'online')
  if (firstOnlineStation) {
    selectStation(firstOnlineStation)
  }
})

/// <summary>
/// 检查并执行联动规则
/// </summary>
const checkAndExecuteRule = (rule: any) => {
  // 简化的规则检查逻辑
  if (rule.id === 'rule1') {
    // PM2.5超标自动喷淋
    const exceedingStations = monitoringStations.value.filter(s => s.pm25 > 75 && s.windSpeed < 2)
    if (exceedingStations.length > 0 && !activeTreatments.value.some(t => t.device.includes('喷淋'))) {
      activeTreatments.value.push({
        id: `auto_spray_${Date.now()}`,
        device: '自动喷淋系统',
        status: '运行中',
        progress: 0,
        progressStatus: 'success'
      })
      ElMessage.success('智能联动: 喷淋系统已自动启动')
    }
  }
}

onUnmounted(() => {
  if (dataUpdateInterval) clearInterval(dataUpdateInterval)
  if (linkageCheckInterval) clearInterval(linkageCheckInterval)
  
  engine?.destroy()
})
</script>

<style lang="scss" scoped>
.environment-monitoring-dashboard {
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
      
      .monitoring-status {
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
          
          &.offline {
            color: #f56c6c;
          }
        }
        
        .air-quality {
          font-size: 12px;
          color: rgba(255, 255, 255, 0.7);
        }
      }
    }
    
    .control-right {
      display: flex;
      align-items: center;
      gap: 20px;
      
      .alert-status {
        display: flex;
        flex-direction: column;
        gap: 4px;
        
        .alert-indicator {
          display: flex;
          align-items: center;
          gap: 6px;
          font-size: 14px;
          
          &.normal {
            color: #67c23a;
          }
          
          &.warning {
            color: #e6a23c;
          }
          
          &.critical {
            color: #f56c6c;
          }
        }
        
        .exceeding-stations {
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
      
      .stations-panel {
        position: absolute;
        top: 20px;
        left: 20px;
        width: 280px;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 8px;
        padding: 16px;
        backdrop-filter: blur(10px);
        max-height: 60%;
        overflow-y: auto;
        
        h4 {
          margin: 0 0 12px 0;
          color: white;
          font-size: 14px;
        }
        
        .stations-grid {
          display: flex;
          flex-direction: column;
          gap: 8px;
          
          .station-item {
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
            
            &.exceeding {
              border-color: #f56c6c;
              animation: pulse 2s infinite;
            }
            
            .station-header {
              display: flex;
              justify-content: space-between;
              align-items: center;
              margin-bottom: 8px;
              
              .station-name {
                font-weight: 600;
                color: white;
                font-size: 12px;
              }
              
              .station-status {
                font-size: 11px;
                color: rgba(255, 255, 255, 0.7);
              }
            }
            
            .station-metrics {
              display: grid;
              grid-template-columns: 1fr 1fr;
              gap: 4px;
              
              .metric-row {
                display: flex;
                justify-content: space-between;
                font-size: 11px;
                
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
            
            .exceeding-badge {
              position: absolute;
              top: 8px;
              right: 8px;
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
      
      .treatment-popup {
        position: absolute;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        width: 480px;
        max-height: 80%;
        background: rgba(0, 0, 0, 0.95);
        border-radius: 12px;
        border: 2px solid #67c23a;
        backdrop-filter: blur(20px);
        z-index: 1000;
        overflow-y: auto;
        
        .popup-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          padding: 16px 20px;
          border-bottom: 1px solid rgba(255, 255, 255, 0.1);
          
          h3 {
            margin: 0;
            color: #67c23a;
            font-size: 16px;
          }
        }
        
        .popup-content {
          padding: 20px;
          
          .pollution-analysis {
            margin-bottom: 20px;
            
            .analysis-item {
              display: flex;
              justify-content: space-between;
              margin-bottom: 8px;
              
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
              }
            }
          }
          
          .treatment-plan {
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .plan-steps {
              .step-item {
                display: flex;
                gap: 12px;
                margin-bottom: 12px;
                
                .step-number {
                  width: 24px;
                  height: 24px;
                  background: #67c23a;
                  border-radius: 50%;
                  display: flex;
                  align-items: center;
                  justify-content: center;
                  color: white;
                  font-size: 12px;
                  font-weight: 600;
                  flex-shrink: 0;
                }
                
                .step-content {
                  flex: 1;
                  
                  .step-action {
                    font-weight: 600;
                    color: white;
                    margin-bottom: 4px;
                  }
                  
                  .step-description {
                    font-size: 12px;
                    color: rgba(255, 255, 255, 0.7);
                    margin-bottom: 4px;
                  }
                  
                  .step-duration {
                    font-size: 11px;
                    color: #67c23a;
                  }
                }
              }
            }
          }
        }
        
        .popup-actions {
          display: flex;
          gap: 12px;
          padding: 16px 20px;
          border-top: 1px solid rgba(255, 255, 255, 0.1);
        }
      }
      
      .history-slider {
        position: absolute;
        bottom: 20px;
        left: 20px;
        right: 20px;
        background: rgba(0, 0, 0, 0.8);
        border-radius: 8px;
        padding: 16px;
        backdrop-filter: blur(10px);
        
        .slider-header {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 12px;
          
          h4 {
            margin: 0;
            color: white;
            font-size: 14px;
          }
          
          .time-display {
            color: #409eff;
            font-weight: 600;
          }
        }
        
        .comparison-stats {
          margin-top: 8px;
          
          .stat-item {
            display: flex;
            justify-content: space-between;
            
            .label {
              color: rgba(255, 255, 255, 0.7);
              font-size: 12px;
            }
            
            .value {
              font-size: 12px;
              font-weight: 600;
              
              &.better {
                color: #67c23a;
              }
              
              &.worse {
                color: #f56c6c;
              }
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
      
      .realtime-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .realtime-content {
          .metrics-cards {
            display: flex;
            gap: 12px;
            margin-bottom: 20px;
            
            .metric-card {
              flex: 1;
              display: flex;
              flex-direction: column;
              align-items: center;
              padding: 16px 8px;
              background: rgba(255, 255, 255, 0.05);
              border-radius: 8px;
              text-align: center;
              
              .metric-icon {
                font-size: 24px;
                margin-bottom: 8px;
                
                &.dust {
                  color: #e6a23c;
                }
                
                &.noise {
                  color: #f56c6c;
                }
                
                &.weather {
                  color: #409eff;
                }
              }
              
              .metric-content {
                .metric-value {
                  font-size: 20px;
                  font-weight: 700;
                  color: white;
                  line-height: 1;
                }
                
                .metric-unit {
                  font-size: 10px;
                  color: rgba(255, 255, 255, 0.6);
                  margin: 2px 0;
                }
                
                .metric-label {
                  font-size: 12px;
                  color: rgba(255, 255, 255, 0.7);
                  margin-bottom: 4px;
                }
                
                .metric-status {
                  font-size: 10px;
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
                
                .metric-extra {
                  font-size: 10px;
                  color: rgba(255, 255, 255, 0.5);
                }
              }
            }
          }
          
          .trend-charts {
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .chart-tabs {
              .el-tabs {
                --el-tabs-header-color: rgba(255, 255, 255, 0.7);
              }
              
              canvas {
                width: 100%;
                border: 1px solid rgba(255, 255, 255, 0.2);
                border-radius: 8px;
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
      
      .diffusion-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .diffusion-content {
          .diffusion-controls {
            margin-bottom: 16px;
            
            .control-row {
              display: flex;
              align-items: center;
              justify-content: space-between;
              margin-bottom: 12px;
              
              label {
                color: rgba(255, 255, 255, 0.7);
                font-size: 12px;
                min-width: 60px;
              }
              
              .el-select {
                width: 120px;
              }
              
              .el-slider {
                flex: 1;
                margin-left: 12px;
              }
            }
          }
          
          .diffusion-results {
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .prediction-list {
              .prediction-item {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 8px;
                background: rgba(255, 255, 255, 0.05);
                border-radius: 6px;
                margin-bottom: 6px;
                
                .prediction-time {
                  font-size: 12px;
                  color: white;
                  font-weight: 600;
                }
                
                .prediction-range {
                  font-size: 11px;
                  color: rgba(255, 255, 255, 0.7);
                }
                
                .prediction-concentration {
                  font-size: 11px;
                  color: #e6a23c;
                }
              }
            }
          }
        }
      }
      
      .linkage-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .linkage-content {
          .linkage-rules {
            margin-bottom: 16px;
            
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .rule-list {
              .rule-item {
                padding: 12px;
                background: rgba(255, 255, 255, 0.05);
                border-radius: 8px;
                margin-bottom: 8px;
                opacity: 0.6;
                transition: opacity 0.3s;
                
                &.active {
                  opacity: 1;
                  border: 1px solid #67c23a;
                }
                
                .rule-header {
                  display: flex;
                  justify-content: space-between;
                  align-items: center;
                  margin-bottom: 6px;
                  
                  .rule-name {
                    font-weight: 600;
                    color: white;
                    font-size: 12px;
                  }
                }
                
                .rule-condition {
                  font-size: 11px;
                  color: #409eff;
                  margin-bottom: 4px;
                }
                
                .rule-action {
                  font-size: 11px;
                  color: rgba(255, 255, 255, 0.7);
                }
              }
            }
          }
          
          .active-treatments {
            h4 {
              margin: 0 0 12px 0;
              color: white;
              font-size: 14px;
            }
            
            .treatment-list {
              .treatment-item {
                display: flex;
                align-items: center;
                gap: 8px;
                padding: 8px;
                background: rgba(255, 255, 255, 0.05);
                border-radius: 6px;
                margin-bottom: 6px;
                
                .treatment-device {
                  flex: 1;
                  font-size: 12px;
                  color: white;
                  font-weight: 600;
                }
                
                .treatment-status {
                  font-size: 11px;
                  color: #67c23a;
                }
                
                .treatment-progress {
                  width: 80px;
                }
                
                .treatment-actions {
                  .el-button {
                    font-size: 11px;
                    padding: 4px 8px;
                  }
                }
              }
            }
          }
        }
      }
      
      .system-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
        }
        
        .system-content {
          .system-metrics {
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
                color: #67c23a;
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
    opacity: 0.6;
  }
}
</style> 