<template>
  <div class="attendance-dashboard">
    <!-- 顶部控制栏 -->
    <div class="control-bar">
      <div class="control-left">
        <h2 class="page-title">🏗️ 实名制考勤大屏</h2>
        <div class="filter-controls">
          <el-select v-model="filterOptions.workType" placeholder="工种筛选" size="small" @change="updateHeatmap">
            <el-option label="全部工种" value="all"></el-option>
            <el-option label="钢筋工" value="steelworker"></el-option>
            <el-option label="混凝土工" value="concreter"></el-option>
            <el-option label="架子工" value="scaffolder"></el-option>
            <el-option label="电工" value="electrician"></el-option>
            <el-option label="安全员" value="safety"></el-option>
          </el-select>
          
          <el-select v-model="filterOptions.team" placeholder="班组筛选" size="small" @change="updateHeatmap">
            <el-option label="全部班组" value="all"></el-option>
            <el-option v-for="team in teams" :key="team.id" :label="team.name" :value="team.id"></el-option>
          </el-select>
          
          <el-date-picker
            v-model="filterOptions.date"
            type="date"
            placeholder="选择日期"
            size="small"
            @change="loadAttendanceData"
          />
        </div>
      </div>
      
      <div class="control-right">
        <div class="realtime-status">
          <div class="status-item" :class="systemStatus.connection ? 'online' : 'offline'">
            <el-icon><Connection /></el-icon>
            {{ systemStatus.connection ? '实时同步' : '连接中断' }}
          </div>
          <div class="update-time">
            最后更新: {{ formatTime(lastUpdateTime) }}
          </div>
        </div>
        
        <div class="action-buttons">
          <el-button size="small" @click="toggleHeatmapMode">
            <el-icon><Grid /></el-icon>
            {{ heatmapMode === 'density' ? '切换工种' : '切换密度' }}
          </el-button>
          <el-button size="small" @click="exportReport">
            <el-icon><Download /></el-icon>
            导出报告
          </el-button>
        </div>
      </div>
    </div>

    <!-- 核心可视化区域 -->
    <div class="visualization-container">
      <!-- 3D场景 -->
      <div class="scene-section">
        <div ref="threejsContainer" class="threejs-container"></div>
        
        <!-- 热力图图例 -->
        <div class="heatmap-legend">
          <h4>人员密度分布</h4>
          <div class="legend-scale">
            <div class="legend-item" v-for="item in heatmapLegend" :key="item.range">
              <span class="color-block" :style="{ background: item.color }"></span>
              <span class="range-text">{{ item.range }}</span>
            </div>
          </div>
        </div>
        
        <!-- 异常预警面板 -->
        <div class="alert-panel" v-if="abnormalAlerts.length > 0">
          <div class="alert-header">
            <el-icon><Warning /></el-icon>
            <span>异常考勤预警 ({{ abnormalAlerts.length }})</span>
          </div>
          <div class="alert-list">
            <div 
              v-for="alert in abnormalAlerts" 
              :key="alert.id"
              class="alert-item"
              :class="alert.level"
              @click="locatePersonnel(alert.personnelId)"
            >
              <div class="alert-icon">
                <el-icon><User /></el-icon>
              </div>
              <div class="alert-content">
                <div class="alert-title">{{ alert.title }}</div>
                <div class="alert-desc">{{ alert.description }}</div>
                <div class="alert-time">{{ formatTime(alert.timestamp) }}</div>
              </div>
              <div class="alert-actions">
                <el-button size="small" type="primary" @click.stop="handleAlert(alert)">
                  处理
                </el-button>
              </div>
            </div>
          </div>
        </div>
      </div>
      
      <!-- 数据面板 -->
      <div class="data-panel">
        <!-- 考勤统计看板 -->
        <div class="stats-section">
          <h3>考勤统计看板</h3>
          <div class="stats-grid">
            <div class="stat-card">
              <div class="stat-icon attendance">
                <el-icon><User /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ attendanceStats.attendanceRate }}%</div>
                <div class="stat-label">在岗率</div>
              </div>
            </div>
            
            <div class="stat-card">
              <div class="stat-icon late">
                <el-icon><Clock /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ attendanceStats.lateRate }}%</div>
                <div class="stat-label">迟到率</div>
              </div>
            </div>
            
            <div class="stat-card">
              <div class="stat-icon work-type">
                <el-icon><Tools /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ attendanceStats.workTypeCount }}</div>
                <div class="stat-label">工种数量</div>
              </div>
            </div>
            
            <div class="stat-card">
              <div class="stat-icon safety">
                <el-icon><ShieldCheck /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ attendanceStats.safetyScore }}</div>
                <div class="stat-label">安全评分</div>
              </div>
            </div>
            
            <div class="stat-card">
              <div class="stat-icon team">
                <el-icon><UserFilled /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ attendanceStats.teamCount }}</div>
                <div class="stat-label">班组数量</div>
              </div>
            </div>
            
            <div class="stat-card">
              <div class="stat-icon efficiency">
                <el-icon><TrendCharts /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-value">{{ attendanceStats.efficiency }}%</div>
                <div class="stat-label">工作效率</div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 人员轨迹查询 -->
        <div class="trajectory-section">
          <h3>人员轨迹回溯</h3>
          <div class="trajectory-controls">
            <el-select 
              v-model="selectedPersonnel" 
              placeholder="选择人员" 
              filterable
              @change="loadPersonnelTrajectory"
            >
              <el-option 
                v-for="person in personnelList" 
                :key="person.id"
                :label="`${person.workId} - ${person.profession}`"
                :value="person.id"
              >
                <span>{{ person.workId }}</span>
                <span style="color: #8492a6; float: right;">{{ person.profession }}</span>
              </el-option>
            </el-select>
            
            <el-date-picker
              v-model="trajectoryTimeRange"
              type="datetimerange"
              range-separator="至"
              start-placeholder="开始时间"
              end-placeholder="结束时间"
              :default-time="[new Date(2000, 1, 1, 8, 0, 0), new Date(2000, 1, 1, 18, 0, 0)]"
              size="small"
              @change="updateTrajectory"
            />
            
            <el-button size="small" type="primary" @click="playTrajectory" :disabled="!trajectoryData">
              <el-icon><VideoPlay /></el-icon>
              回放轨迹
            </el-button>
          </div>
          
          <!-- 轨迹统计信息 -->
          <div v-if="trajectoryData" class="trajectory-stats">
            <div class="trajectory-info">
              <div class="info-item">
                <span class="label">总距离:</span>
                <span class="value">{{ trajectoryData.totalDistance }}km</span>
              </div>
              <div class="info-item">
                <span class="label">工作时长:</span>
                <span class="value">{{ trajectoryData.workDuration }}h</span>
              </div>
              <div class="info-item">
                <span class="label">活动区域:</span>
                <span class="value">{{ trajectoryData.activityAreas }}</span>
              </div>
            </div>
            
            <!-- 轨迹回放控制 -->
            <div class="playback-controls" v-if="isPlayingTrajectory">
              <el-slider
                v-model="playbackProgress"
                :max="100"
                :format-tooltip="formatPlaybackTooltip"
                @change="seekTrajectory"
              />
              <div class="playback-buttons">
                <el-button size="small" @click="pauseTrajectory">
                  <el-icon><VideoPause /></el-icon>
                </el-button>
                <el-button size="small" @click="stopTrajectory">
                  <el-icon><VideoStop /></el-icon>
                </el-button>
                <span class="playback-speed">
                  播放速度: {{ playbackSpeed }}x
                  <el-button size="small" @click="changePlaybackSpeed">切换</el-button>
                </span>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 人证核验状态 -->
        <div class="verification-section">
          <h3>三维人证核验</h3>
          <div class="verification-stats">
            <div class="verification-item success">
              <span class="count">{{ verificationStats.passed }}</span>
              <span class="label">通过验证</span>
            </div>
            <div class="verification-item warning">
              <span class="count">{{ verificationStats.pending }}</span>
              <span class="label">待验证</span>
            </div>
            <div class="verification-item danger">
              <span class="count">{{ verificationStats.failed }}</span>
              <span class="label">验证失败</span>
            </div>
          </div>
          
          <!-- 未通过验证人员列表 -->
          <div v-if="failedVerifications.length > 0" class="failed-list">
            <div 
              v-for="person in failedVerifications" 
              :key="person.id"
              class="failed-item"
              @click="locatePersonnel(person.id)"
            >
              <div class="person-avatar">
                <el-icon><User /></el-icon>
              </div>
              <div class="person-info">
                <div class="person-name">工号: {{ person.workId }}</div>
                <div class="person-status">{{ person.failureReason }}</div>
              </div>
              <div class="person-location">
                <el-icon><Location /></el-icon>
                {{ person.lastLocation }}
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
  Connection, Grid, Download, Warning, User, Clock, Tools, 
  ShieldCheck, UserFilled, TrendCharts, VideoPlay, VideoPause, 
  VideoStop, Location 
} from '@element-plus/icons-vue'
import { digitalTwinApi } from '@/api/modules/digitalTwin'
import { AttendanceVisualizationEngine } from './engine/AttendanceVisualizationEngine'

// 组件状态
const threejsContainer = ref<HTMLElement>()
const lastUpdateTime = ref(new Date())
const selectedPersonnel = ref('')
const trajectoryTimeRange = ref<[Date, Date]>()
const isPlayingTrajectory = ref(false)
const playbackProgress = ref(0)
const playbackSpeed = ref(1)

// 筛选选项
const filterOptions = ref({
  workType: 'all',
  team: 'all',
  date: new Date()
})

// 热力图模式
const heatmapMode = ref<'density' | 'worktype'>('density')

// 3D引擎实例
let engine: AttendanceVisualizationEngine | null = null

// 数据状态
const systemStatus = ref({
  connection: true,
  dataQuality: 95
})

const teams = ref([
  { id: 'team1', name: '第一班组' },
  { id: 'team2', name: '第二班组' },
  { id: 'team3', name: '第三班组' },
  { id: 'team4', name: '第四班组' },
  { id: 'team5', name: '第五班组' }
])

const attendanceStats = ref({
  attendanceRate: 92.3,
  lateRate: 5.2,
  workTypeCount: 8,
  safetyScore: 87,
  teamCount: 5,
  efficiency: 89
})

const verificationStats = ref({
  passed: 156,
  pending: 3,
  failed: 2
})

const personnelList = ref<any[]>([])
const trajectoryData = ref<any>(null)
const abnormalAlerts = ref<any[]>([])
const failedVerifications = ref<any[]>([])

// 热力图图例
const heatmapLegend = ref([
  { range: '0-5人', color: '#4CAF50' },
  { range: '6-15人', color: '#FFC107' },
  { range: '16-30人', color: '#FF9800' },
  { range: '31-50人', color: '#F44336' },
  { range: '50+人', color: '#9C27B0' }
])

/// <summary>
/// 初始化3D可视化引擎
/// </summary>
const initVisualization = async () => {
  if (!threejsContainer.value) return
  
  try {
    const startTime = performance.now()
    
    engine = new AttendanceVisualizationEngine(threejsContainer.value)
    await engine.init()
    
    // 设置事件监听
    engine.on('personnelClick', handlePersonnelClick)
    engine.on('areaClick', handleAreaClick)
    engine.on('performanceUpdate', updatePerformance)
    
    const endTime = performance.now()
    const loadTime = (endTime - startTime) / 1000
    
    console.log(`3D场景加载完成，耗时: ${loadTime.toFixed(2)}s`)
    
    if (loadTime > 1.5) {
      ElMessage.warning(`场景加载时间超标: ${loadTime.toFixed(2)}s，建议优化`)
    }
    
    // 开始渲染
    engine.startRender()
    
  } catch (error) {
    console.error('可视化引擎初始化失败:', error)
    ElMessage.error('3D场景初始化失败')
  }
}

/// <summary>
/// 加载考勤数据 - 性能要求≤1.5s
/// </summary>
const loadAttendanceData = async () => {
  try {
    const startTime = performance.now()
    
    // 并行加载多个数据源
    const [attendanceData, personnelData, alertData] = await Promise.all([
      digitalTwinApi.getPersonnelPositions(filterOptions.value.date.toISOString().split('T')[0]),
      digitalTwinApi.getPersonnelPositions(),
      loadAbnormalAlerts()
    ])
    
    const endTime = performance.now()
    const loadTime = (endTime - startTime) / 1000
    
    console.log(`考勤数据加载完成，耗时: ${loadTime.toFixed(2)}s`)
    
    if (loadTime <= 1.5) {
      ElMessage.success(`数据加载完成 (${loadTime.toFixed(2)}s)`)
    } else {
      ElMessage.warning(`数据加载超时: ${loadTime.toFixed(2)}s`)
    }
    
    // 更新数据
    if (attendanceData.data) {
      personnelList.value = attendanceData.data.personnel || []
      updateAttendanceStats(attendanceData.data)
      engine?.updatePersonnelData(attendanceData.data.personnel)
    }
    
    abnormalAlerts.value = alertData
    lastUpdateTime.value = new Date()
    
  } catch (error) {
    console.error('加载考勤数据失败:', error)
    ElMessage.error('数据加载失败')
  }
}

/// <summary>
/// 更新热力图
/// </summary>
const updateHeatmap = () => {
  if (!engine) return
  
  const filteredData = personnelList.value.filter(person => {
    const workTypeMatch = filterOptions.value.workType === 'all' || person.profession === filterOptions.value.workType
    const teamMatch = filterOptions.value.team === 'all' || person.team === filterOptions.value.team
    return workTypeMatch && teamMatch
  })
  
  engine.updateHeatmap(filteredData, heatmapMode.value)
}

/// <summary>
/// 切换热力图模式
/// </summary>
const toggleHeatmapMode = () => {
  heatmapMode.value = heatmapMode.value === 'density' ? 'worktype' : 'density'
  updateHeatmap()
}

/// <summary>
/// 加载人员轨迹
/// </summary>
const loadPersonnelTrajectory = async () => {
  if (!selectedPersonnel.value) return
  
  try {
    const dateStr = filterOptions.value.date.toISOString().split('T')[0]
    const response = await digitalTwinApi.getPersonnelTrajectory(selectedPersonnel.value, dateStr)
    
    if (response.data) {
      trajectoryData.value = response.data
      engine?.displayTrajectory(response.data.trajectoryPoints)
    }
    
  } catch (error) {
    console.error('加载轨迹失败:', error)
    ElMessage.error('轨迹数据加载失败')
  }
}

/// <summary>
/// 更新轨迹显示
/// </summary>
const updateTrajectory = () => {
  if (trajectoryTimeRange.value && selectedPersonnel.value) {
    loadPersonnelTrajectory()
  }
}

/// <summary>
/// 播放轨迹回放
/// </summary>
const playTrajectory = () => {
  if (!trajectoryData.value || !engine) return
  
  isPlayingTrajectory.value = true
  playbackProgress.value = 0
  
  engine.playTrajectoryAnimation(
    trajectoryData.value.trajectoryPoints,
    playbackSpeed.value,
    (progress: number) => {
      playbackProgress.value = progress
    },
    () => {
      isPlayingTrajectory.value = false
      playbackProgress.value = 100
    }
  )
}

/// <summary>
/// 暂停轨迹回放
/// </summary>
const pauseTrajectory = () => {
  engine?.pauseTrajectoryAnimation()
}

/// <summary>
/// 停止轨迹回放
/// </summary>
const stopTrajectory = () => {
  isPlayingTrajectory.value = false
  playbackProgress.value = 0
  engine?.stopTrajectoryAnimation()
}

/// <summary>
/// 跳转轨迹进度
/// </summary>
const seekTrajectory = (progress: number) => {
  engine?.seekTrajectoryAnimation(progress / 100)
}

/// <summary>
/// 切换播放速度
/// </summary>
const changePlaybackSpeed = () => {
  const speeds = [0.5, 1, 2, 4]
  const currentIndex = speeds.indexOf(playbackSpeed.value)
  playbackSpeed.value = speeds[(currentIndex + 1) % speeds.length]
  engine?.setPlaybackSpeed(playbackSpeed.value)
}

/// <summary>
/// 定位人员
/// </summary>
const locatePersonnel = (personnelId: string) => {
  const person = personnelList.value.find(p => p.userId === personnelId)
  if (person && engine) {
    engine.focusOnPersonnel(person)
    ElMessage.info(`已定位到人员: ${person.userId}`)
  }
}

/// <summary>
/// 处理异常预警
/// </summary>
const handleAlert = async (alert: any) => {
  try {
    await ElMessageBox.confirm(
      `确认处理预警: ${alert.description}?`,
      '预警处理',
      {
        confirmButtonText: '确认处理',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 模拟处理预警
    abnormalAlerts.value = abnormalAlerts.value.filter(a => a.id !== alert.id)
    ElMessage.success('预警已处理')
    
    // TODO: 调用后端API处理预警
    
  } catch {
    // 用户取消
  }
}

/// <summary>
/// 人员点击事件
/// </summary>
const handlePersonnelClick = (person: any) => {
  console.log('点击人员:', person)
  // 显示人员详情弹窗
}

/// <summary>
/// 区域点击事件
/// </summary>
const handleAreaClick = (area: any) => {
  console.log('点击区域:', area)
  // 显示区域统计信息
}

/// <summary>
/// 性能更新
/// </summary>
const updatePerformance = (perfData: any) => {
  console.log('性能数据:', perfData)
}

/// <summary>
/// 更新考勤统计
/// </summary>
const updateAttendanceStats = (data: any) => {
  attendanceStats.value = {
    attendanceRate: Math.round((data.onSiteCount / data.totalCount) * 100 * 10) / 10,
    lateRate: 5.2, // 模拟数据
    workTypeCount: 8,
    safetyScore: 87,
    teamCount: 5,
    efficiency: 89
  }
}

/// <summary>
/// 加载异常预警
/// </summary>
const loadAbnormalAlerts = async () => {
  // 模拟异常预警数据
  return [
    {
      id: 'alert001',
      personnelId: 'worker001',
      title: '未打卡人员在施工区',
      description: '工号001在施工区域内活动，但未进行打卡验证',
      level: 'critical',
      timestamp: Date.now() - 300000,
      location: '1号楼3层'
    },
    {
      id: 'alert002',
      personnelId: 'worker045',
      title: '连续3天未打卡',
      description: '工号045已连续3个工作日未进行考勤打卡',
      level: 'warning',
      timestamp: Date.now() - 600000,
      location: '2号楼地下室'
    }
  ]
}

/// <summary>
/// 导出报告
/// </summary>
const exportReport = () => {
  // TODO: 实现报告导出功能
  ElMessage.info('报告导出功能开发中...')
}

/// <summary>
/// 格式化时间
/// </summary>
const formatTime = (timestamp: number | Date) => {
  const date = new Date(timestamp)
  return date.toLocaleTimeString()
}

/// <summary>
/// 格式化回放进度提示
/// </summary>
const formatPlaybackTooltip = (value: number) => {
  if (!trajectoryData.value) return `${value}%`
  
  const totalPoints = trajectoryData.value.trajectoryPoints.length
  const currentPoint = Math.floor((value / 100) * (totalPoints - 1))
  const point = trajectoryData.value.trajectoryPoints[currentPoint]
  
  if (point) {
    return new Date(point.timestamp).toLocaleTimeString()
  }
  
  return `${value}%`
}

// 实时数据更新
let updateInterval: NodeJS.Timeout

onMounted(async () => {
  await nextTick()
  
  // 初始化可视化
  await initVisualization()
  
  // 加载初始数据
  await loadAttendanceData()
  
  // 启动实时更新 - 每30秒更新一次
  updateInterval = setInterval(() => {
    loadAttendanceData()
  }, 30000)
})

onUnmounted(() => {
  if (updateInterval) {
    clearInterval(updateInterval)
  }
  
  engine?.destroy()
})
</script>

<style lang="scss" scoped>
.attendance-dashboard {
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
        color: white;
      }
      
      .filter-controls {
        display: flex;
        gap: 12px;
      }
    }
    
    .control-right {
      display: flex;
      align-items: center;
      gap: 20px;
      
      .realtime-status {
        display: flex;
        flex-direction: column;
        align-items: flex-end;
        
        .status-item {
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
        
        .update-time {
          font-size: 12px;
          color: rgba(255, 255, 255, 0.6);
          margin-top: 4px;
        }
      }
      
      .action-buttons {
        display: flex;
        gap: 8px;
      }
    }
  }
  
  .visualization-container {
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
      
      .heatmap-legend {
        position: absolute;
        top: 20px;
        left: 20px;
        background: rgba(0, 0, 0, 0.8);
        padding: 16px;
        border-radius: 8px;
        backdrop-filter: blur(10px);
        
        h4 {
          margin: 0 0 12px 0;
          color: white;
          font-size: 14px;
        }
        
        .legend-scale {
          display: flex;
          flex-direction: column;
          gap: 6px;
          
          .legend-item {
            display: flex;
            align-items: center;
            gap: 8px;
            
            .color-block {
              width: 16px;
              height: 12px;
              border-radius: 2px;
            }
            
            .range-text {
              font-size: 12px;
              color: rgba(255, 255, 255, 0.8);
            }
          }
        }
      }
      
      .alert-panel {
        position: absolute;
        top: 20px;
        right: 20px;
        width: 320px;
        max-height: 400px;
        background: rgba(0, 0, 0, 0.9);
        border-radius: 8px;
        overflow: hidden;
        backdrop-filter: blur(10px);
        
        .alert-header {
          display: flex;
          align-items: center;
          gap: 8px;
          padding: 12px 16px;
          background: rgba(245, 108, 108, 0.2);
          border-bottom: 1px solid rgba(255, 255, 255, 0.1);
          color: #f56c6c;
          font-weight: 600;
        }
        
        .alert-list {
          max-height: 320px;
          overflow-y: auto;
          
          .alert-item {
            display: flex;
            gap: 12px;
            padding: 12px 16px;
            border-bottom: 1px solid rgba(255, 255, 255, 0.1);
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
            
            .alert-icon {
              color: #f56c6c;
              font-size: 18px;
            }
            
            .alert-content {
              flex: 1;
              
              .alert-title {
                font-weight: 600;
                font-size: 14px;
                margin-bottom: 4px;
              }
              
              .alert-desc {
                font-size: 12px;
                color: rgba(255, 255, 255, 0.7);
                margin-bottom: 4px;
              }
              
              .alert-time {
                font-size: 11px;
                color: rgba(255, 255, 255, 0.5);
              }
            }
            
            .alert-actions {
              display: flex;
              align-items: center;
            }
          }
        }
      }
    }
    
    .data-panel {
      width: 400px;
      display: flex;
      flex-direction: column;
      gap: 20px;
      
      .stats-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
          font-weight: 600;
        }
        
        .stats-grid {
          display: grid;
          grid-template-columns: repeat(2, 1fr);
          gap: 12px;
          
          .stat-card {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 16px;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 8px;
            border: 1px solid rgba(255, 255, 255, 0.1);
            
            .stat-icon {
              width: 40px;
              height: 40px;
              border-radius: 8px;
              display: flex;
              align-items: center;
              justify-content: center;
              font-size: 18px;
              
              &.attendance {
                background: linear-gradient(135deg, #67c23a, #85ce61);
                color: white;
              }
              
              &.late {
                background: linear-gradient(135deg, #e6a23c, #f0a020);
                color: white;
              }
              
              &.work-type {
                background: linear-gradient(135deg, #409eff, #66b1ff);
                color: white;
              }
              
              &.safety {
                background: linear-gradient(135deg, #f56c6c, #f78989);
                color: white;
              }
              
              &.team {
                background: linear-gradient(135deg, #9c27b0, #ba68c8);
                color: white;
              }
              
              &.efficiency {
                background: linear-gradient(135deg, #00bcd4, #4dd0e1);
                color: white;
              }
            }
            
            .stat-content {
              .stat-value {
                font-size: 20px;
                font-weight: 700;
                color: white;
                line-height: 1;
              }
              
              .stat-label {
                font-size: 12px;
                color: rgba(255, 255, 255, 0.7);
                margin-top: 4px;
              }
            }
          }
        }
      }
      
      .trajectory-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
          font-weight: 600;
        }
        
        .trajectory-controls {
          display: flex;
          flex-direction: column;
          gap: 12px;
          margin-bottom: 16px;
        }
        
        .trajectory-stats {
          .trajectory-info {
            display: flex;
            flex-direction: column;
            gap: 8px;
            margin-bottom: 16px;
            
            .info-item {
              display: flex;
              justify-content: space-between;
              
              .label {
                color: rgba(255, 255, 255, 0.7);
              }
              
              .value {
                color: white;
                font-weight: 600;
              }
            }
          }
          
          .playback-controls {
            .playback-buttons {
              display: flex;
              align-items: center;
              gap: 8px;
              margin-top: 8px;
              
              .playback-speed {
                margin-left: auto;
                font-size: 12px;
                color: rgba(255, 255, 255, 0.7);
              }
            }
          }
        }
      }
      
      .verification-section {
        background: rgba(255, 255, 255, 0.05);
        border-radius: 12px;
        padding: 20px;
        backdrop-filter: blur(10px);
        
        h3 {
          margin: 0 0 16px 0;
          color: white;
          font-size: 16px;
          font-weight: 600;
        }
        
        .verification-stats {
          display: flex;
          gap: 16px;
          margin-bottom: 16px;
          
          .verification-item {
            flex: 1;
            text-align: center;
            padding: 12px;
            border-radius: 8px;
            
            &.success {
              background: rgba(103, 194, 58, 0.2);
              border: 1px solid #67c23a;
            }
            
            &.warning {
              background: rgba(230, 162, 60, 0.2);
              border: 1px solid #e6a23c;
            }
            
            &.danger {
              background: rgba(245, 108, 108, 0.2);
              border: 1px solid #f56c6c;
            }
            
            .count {
              display: block;
              font-size: 24px;
              font-weight: 700;
              color: white;
            }
            
            .label {
              font-size: 12px;
              color: rgba(255, 255, 255, 0.7);
            }
          }
        }
        
        .failed-list {
          .failed-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px;
            border-radius: 8px;
            background: rgba(255, 255, 255, 0.05);
            margin-bottom: 8px;
            cursor: pointer;
            transition: background-color 0.3s;
            
            &:hover {
              background: rgba(255, 255, 255, 0.1);
            }
            
            .person-avatar {
              width: 32px;
              height: 32px;
              border-radius: 50%;
              background: #f56c6c;
              display: flex;
              align-items: center;
              justify-content: center;
              color: white;
            }
            
            .person-info {
              flex: 1;
              
              .person-name {
                font-weight: 600;
                font-size: 14px;
              }
              
              .person-status {
                font-size: 12px;
                color: #f56c6c;
              }
            }
            
            .person-location {
              display: flex;
              align-items: center;
              gap: 4px;
              font-size: 12px;
              color: rgba(255, 255, 255, 0.7);
            }
          }
        }
      }
    }
  }
}
</style> 