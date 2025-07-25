<template>
  <div class="command-center-screen">
    <!-- 顶部概览卡片区域 -->
    <div class="overview-section">
      <div class="overview-grid">
        <StatisticCard
          :value="overviewData.projectSummary?.totalProjects || 0"
          label="项目总数"
          description="全部项目统计"
          icon="OfficeBuilding"
          type="primary"
          :trend="2.5"
          status="normal"
        />
        <StatisticCard
          :value="overviewData.projectSummary?.activeProjects || 0"
          label="在建项目"
          description="当前活跃项目"
          icon="Ship"
          type="success"
          :trend="5.2"
          status="online"
        />
        <StatisticCard
          :value="overviewData.personnelSummary?.onSitePersonnel || 0"
          label="在场人员"
          description="实时在场统计"
          icon="User"
          type="info"
          :trend="-1.8"
          status="warning"
        />
        <StatisticCard
          :value="overviewData.equipmentSummary?.onlineEquipment || 0"
          label="在线设备"
          description="设备在线统计"
          icon="Monitor"
          type="warning"
          :trend="3.2"
          status="online"
        />
        <StatisticCard
          :value="overviewData.safetySummary?.safetyScore || 0"
          label="安全评分"
          description="综合安全指数"
          icon="Shield"
          type="danger"
          suffix="分"
          :trend="0.8"
          :show-progress="true"
          :percentage="overviewData.safetySummary?.safetyScore || 0"
          status="normal"
        />
        <StatisticCard
          :value="formatMoney(overviewData.projectSummary?.totalInvestment || 0)"
          label="总投资额"
          description="项目总投资"
          icon="Money"
          type="success"
          suffix="元"
          status="normal"
        />
      </div>
    </div>

    <!-- 实时数据和趋势图表区域 -->
    <div class="charts-section">
      <div class="charts-grid">
        <!-- 实时数据监控 -->
        <DataCard title="实时数据监控" class="realtime-card">
          <div class="realtime-stats">
            <div class="stat-item">
              <div class="stat-icon">
                <el-icon><User /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-label">今日考勤</div>
                <div class="stat-value">{{ realtimeData.attendanceToday || 0 }}</div>
              </div>
            </div>
            <div class="stat-item">
              <div class="stat-icon">
                <el-icon><Monitor /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-label">设备在线</div>
                <div class="stat-value">{{ realtimeData.equipmentOnline || 0 }}</div>
              </div>
            </div>
            <div class="stat-item">
              <div class="stat-icon">
                <el-icon><Warning /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-label">安全告警</div>
                <div class="stat-value">{{ realtimeData.safetyAlerts || 0 }}</div>
              </div>
            </div>
            <div class="stat-item">
              <div class="stat-icon">
                <el-icon><Location /></el-icon>
              </div>
              <div class="stat-content">
                <div class="stat-label">环境告警</div>
                <div class="stat-value">{{ realtimeData.environmentAlerts || 0 }}</div>
              </div>
            </div>
          </div>
          <div class="last-update">
            最后更新：{{ formatDateTime(realtimeData.lastUpdateTime) }}
          </div>
        </DataCard>

        <!-- 人员考勤趋势 -->
        <DataCard title="人员考勤趋势" class="trend-card">
          <DigitalChart
            :option="attendanceTrendOption"
            :loading="chartLoading"
            height="280px"
          />
        </DataCard>

        <!-- 设备在线状态 -->
        <DataCard title="设备在线状态" class="equipment-card">
          <DigitalChart
            :option="equipmentStatusOption"
            :loading="chartLoading"
            height="280px"
          />
        </DataCard>
      </div>
    </div>

    <!-- 项目列表区域 -->
    <div class="projects-section">
      <DataCard title="项目状态总览" class="projects-card">
        <div class="projects-list">
          <div 
            class="project-item"
            v-for="project in projectList"
            :key="project.id"
            @click="handleProjectClick(project)"
          >
            <div class="project-header">
              <h4 class="project-name">{{ project.name }}</h4>
              <div class="project-status" :class="`status--${project.status.toLowerCase()}`">
                {{ getStatusText(project.status) }}
              </div>
            </div>
            <div class="project-content">
              <div class="project-info">
                <div class="info-item">
                  <span class="label">进度：</span>
                  <span class="value">{{ project.progress }}%</span>
                </div>
                <div class="info-item">
                  <span class="label">地址：</span>
                  <span class="value">{{ project.location?.address }}</span>
                </div>
                <div class="info-item">
                  <span class="label">人员：</span>
                  <span class="value">{{ project.personnel?.onSite }}/{{ project.personnel?.total }}</span>
                </div>
                <div class="info-item">
                  <span class="label">设备：</span>
                  <span class="value">{{ project.equipment?.online }}/{{ project.equipment?.total }}</span>
                </div>
              </div>
              <div class="project-progress">
                <el-progress
                  :percentage="project.progress"
                  :stroke-width="8"
                  :show-text="false"
                  :color="getProgressColor(project.progress)"
                />
              </div>
            </div>
          </div>
        </div>
      </DataCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted, computed } from 'vue'
import { ElMessage } from 'element-plus'
import {
  DataAnalysis,
  FullScreen,
  OfficeBuilding, // Building -> OfficeBuilding
  VideoCamera,
  User,
  Van,
  Bell,
  Warning,
  Ship, // Construction -> Ship (as a placeholder)
  Cpu,
  DataLine,
  Compass,
  CircleCheck,
  CircleClose,
  QuestionFilled,
  InfoFilled,
  WarnTriangleFilled
} from '@element-plus/icons-vue'

import ScreenHeader from '../../../components/PageHeader.vue';
import StatisticCard from '../components/StatisticCard.vue';
import DataCard from '../components/DataCard.vue';
import DigitalChart from '../components/DigitalChart.vue';
import AlertNotificationPanel from '../components/AlertNotificationPanel.vue';

import { DigitalTwinService } from '../../../api/services/digital-twin.service';
import { formatDateTime } from '../../../utils/format';

// 数据状态
const overviewData = ref<any>({})
const realtimeData = ref<any>({})
const projectList = ref<any[]>([])
const trendsData = ref<any>({})
const chartLoading = ref(false)

// 定时器
let realtimeTimer: NodeJS.Timeout | null = null

const digitalTwinService = new DigitalTwinService();

// 加载总览数据
const loadOverviewData = async () => {
  try {
    const response = await digitalTwinService.getCommandCenterOverview()
    if (response.success) {
      overviewData.value = response.data
    }
  } catch (error) {
    console.error('Failed to load overview data:', error)
  }
}

// 加载项目列表
const loadProjectList = async () => {
  try {
    const projectResponse = await digitalTwinService.getProjectList()
    if (projectResponse.success) {
      projectList.value = projectResponse.data
    }
  } catch (error) {
    console.error('Failed to load project list:', error)
  }
}

// 加载实时数据
const loadRealtimeData = async () => {
  try {
    const realtimeResponse = await digitalTwinService.getRealtimeStats()
    if (realtimeResponse.success) {
      realtimeData.value = realtimeResponse.data
    }
  } catch (error) {
    console.error('Failed to load realtime data:', error)
  }
}

// 加载趋势数据
const loadTrendsData = async () => {
  try {
    chartLoading.value = true
    const response = await digitalTwinService.getTrends('attendance', 'week')
    if (response.success) {
      trendsData.value = response.data
    }
  } catch (error) {
    console.error('Failed to load trends data:', error)
  } finally {
    chartLoading.value = false
  }
}

// 考勤趋势图表配置
const attendanceTrendOption = computed(() => {
  const data = trendsData.value.attendance
  if (!data) return {}

  return {
    tooltip: {
      trigger: 'axis',
      backgroundColor: 'rgba(30, 42, 58, 0.9)',
      borderColor: '#3498db',
      textStyle: { color: '#ffffff' }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: data.dates || [],
      axisLabel: {
        color: '#7f8c8d'
      },
      axisLine: {
        lineStyle: {
          color: '#34495e'
        }
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: '#7f8c8d',
        formatter: '{value}'
      },
      axisLine: {
        lineStyle: {
          color: '#34495e'
        }
      },
      splitLine: {
        lineStyle: {
          color: '#34495e',
          type: 'dashed'
        }
      }
    },
    series: [
      {
        name: '考勤人数',
        type: 'line',
        data: data.values || [],
        smooth: true,
        lineStyle: {
          color: '#3498db',
          width: 3
        },
        itemStyle: {
          color: '#3498db'
        },
        areaStyle: {
          color: {
            type: 'linear',
            x: 0,
            y: 0,
            x2: 0,
            y2: 1,
            colorStops: [
              { offset: 0, color: 'rgba(52, 152, 219, 0.3)' },
              { offset: 1, color: 'rgba(52, 152, 219, 0.1)' }
            ]
          }
        }
      }
    ]
  }
})

// 设备状态图表配置
const equipmentStatusOption = computed(() => {
  const data = trendsData.value.equipment
  if (!data) return {}

  return {
    tooltip: {
      trigger: 'axis',
      backgroundColor: 'rgba(30, 42, 58, 0.9)',
      borderColor: '#3498db',
      textStyle: { color: '#ffffff' }
    },
    legend: {
      data: ['在线', '离线'],
      textStyle: {
        color: '#7f8c8d'
      }
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: {
      type: 'category',
      data: data.dates || [],
      axisLabel: {
        color: '#7f8c8d'
      },
      axisLine: {
        lineStyle: {
          color: '#34495e'
        }
      }
    },
    yAxis: {
      type: 'value',
      axisLabel: {
        color: '#7f8c8d'
      },
      axisLine: {
        lineStyle: {
          color: '#34495e'
        }
      },
      splitLine: {
        lineStyle: {
          color: '#34495e',
          type: 'dashed'
        }
      }
    },
    series: [
      {
        name: '在线',
        type: 'bar',
        data: data.online || [],
        itemStyle: {
          color: '#2ecc71'
        }
      },
      {
        name: '离线',
        type: 'bar',
        data: data.offline || [],
        itemStyle: {
          color: '#e74c3c'
        }
      }
    ]
  }
})

// 工具函数
const formatMoney = (value: number) => {
  if (value >= 100000000) {
    return (value / 100000000).toFixed(1) + '亿'
  } else if (value >= 10000) {
    return (value / 10000).toFixed(1) + '万'
  }
  return value.toLocaleString()
}

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Active': '进行中',
    'Completed': '已完成',
    'Pending': '待开始',
    'Suspended': '暂停'
  }
  return statusMap[status] || status
}

const getProgressColor = (progress: number) => {
  if (progress >= 80) return '#2ecc71'
  if (progress >= 60) return '#3498db'
  if (progress >= 40) return '#f39c12'
  return '#e74c3c'
}

const handleProjectClick = (project: any) => {
  ElMessage.info(`查看项目：${project.name}`)
  // 这里可以跳转到项目详情页面
}

// 初始化数据
const initData = async () => {
  await Promise.all([
    loadOverviewData(),
    loadProjectList(),
    loadRealtimeData(),
    loadTrendsData()
  ])
}

// 启动实时数据更新
const startRealtimeUpdate = () => {
  realtimeTimer = setInterval(() => {
    loadRealtimeData()
  }, 30000) // 30秒更新一次
}

// 停止实时数据更新
const stopRealtimeUpdate = () => {
  if (realtimeTimer) {
    clearInterval(realtimeTimer)
    realtimeTimer = null
  }
}

onMounted(() => {
  initData()
  startRealtimeUpdate()
})

onUnmounted(() => {
  stopRealtimeUpdate()
})
</script>

<style lang="scss" scoped>
.command-center-screen {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
  min-height: 100vh;
}

// 概览区域
.overview-section {
  .overview-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
    gap: 20px;
  }
}

// 图表区域
.charts-section {
  .charts-grid {
    display: grid;
    grid-template-columns: 1fr 1fr 1fr;
    gap: 20px;
  }
  
  .realtime-card {
    .realtime-stats {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 16px;
      margin-bottom: 16px;
      
      .stat-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        
        .stat-icon {
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
        
        .stat-content {
          flex: 1;
          
          .stat-label {
            font-size: 12px;
            color: #7f8c8d;
            margin-bottom: 4px;
          }
          
          .stat-value {
            font-size: 20px;
            font-weight: 600;
            color: #ffffff;
          }
        }
      }
    }
    
    .last-update {
      text-align: center;
      font-size: 12px;
      color: #7f8c8d;
    }
  }
}

// 项目区域
.projects-section {
  .projects-list {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
    gap: 16px;
  }
  
  .project-item {
    padding: 16px;
    border-radius: 8px;
    background: rgba(255, 255, 255, 0.05);
    border: 1px solid rgba(255, 255, 255, 0.1);
    cursor: pointer;
    transition: all 0.3s ease;
    
    &:hover {
      background: rgba(255, 255, 255, 0.08);
      border-color: #3498db;
      transform: translateY(-2px);
    }
    
    .project-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 12px;
      
      .project-name {
        font-size: 16px;
        font-weight: 600;
        color: #ffffff;
        margin: 0;
      }
      
      .project-status {
        padding: 4px 12px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 500;
        
        &.status--active {
          background: rgba(46, 204, 113, 0.2);
          color: #2ecc71;
        }
        
        &.status--completed {
          background: rgba(52, 152, 219, 0.2);
          color: #3498db;
        }
        
        &.status--pending {
          background: rgba(243, 156, 18, 0.2);
          color: #f39c12;
        }
      }
    }
    
    .project-content {
      .project-info {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 8px;
        margin-bottom: 12px;
        
        .info-item {
          font-size: 14px;
          
          .label {
            color: #7f8c8d;
          }
          
          .value {
            color: #ffffff;
            font-weight: 500;
          }
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 1200px) {
  .charts-section .charts-grid {
    grid-template-columns: 1fr 1fr;
  }
}

@media (max-width: 768px) {
  .command-center-screen {
    padding: 12px;
  }
  
  .overview-section .overview-grid {
    grid-template-columns: 1fr;
  }
  
  .charts-section .charts-grid {
    grid-template-columns: 1fr;
  }
  
  .projects-section .projects-list {
    grid-template-columns: 1fr;
  }
  
  .realtime-card .realtime-stats {
    grid-template-columns: 1fr;
  }
  
  .project-item .project-info {
    grid-template-columns: 1fr;
  }
}
</style>