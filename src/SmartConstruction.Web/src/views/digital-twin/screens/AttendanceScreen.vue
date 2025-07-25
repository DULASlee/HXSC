<template>
  <div class="attendance-screen">
    <!-- 顶部统计卡片 -->
    <div class="stats-section">
      <div class="stats-grid">
        <StatisticCard
          :value="attendanceOverview.todayAttendance?.totalWorkers || 0"
          label="应到人数"
          description="今日应到岗人员"
          icon="User"
          type="primary"
          status="normal"
        />
        <StatisticCard
          :value="attendanceOverview.todayAttendance?.checkedIn || 0"
          label="已签到"
          description="今日已签到人员"
          icon="Check"
          type="success"
          :trend="2.1"
          status="online"
        />
        <StatisticCard
          :value="attendanceOverview.todayAttendance?.absent || 0"
          label="缺勤人数"
          description="今日缺勤人员"
          icon="Close"
          type="danger"
          :trend="-0.8"
          status="offline"
        />
        <StatisticCard
          :value="attendanceOverview.todayAttendance?.late || 0"
          label="迟到人数"
          description="今日迟到人员"
          icon="Clock"
          type="warning"
          :trend="1.5"
          status="warning"
        />
        <StatisticCard
          :value="attendanceOverview.todayAttendance?.attendanceRate || 0"
          label="出勤率"
          description="今日出勤率统计"
          icon="PieChart"
          type="info"
          suffix="%"
          :show-progress="true"
          :percentage="attendanceOverview.todayAttendance?.attendanceRate || 0"
          :trend="0.5"
          status="normal"
        />
      </div>
    </div>

    <!-- 中间数据展示区域 -->
    <div class="data-section">
      <div class="data-grid">
        <!-- 实时考勤动态 -->
        <DataCard title="实时考勤动态" class="realtime-attendance-card">
          <div class="realtime-list">
            <div 
              class="attendance-item"
              v-for="record in realtimeAttendance.recentCheckins"
              :key="record.workerId"
            >
              <div class="worker-avatar">
                <el-icon><User /></el-icon>
              </div>
              <div class="worker-info">
                <div class="worker-name">{{ record.workerName }}</div>
                <div class="worker-team">{{ record.teamName }}</div>
              </div>
              <div class="check-info">
                <div class="check-time">{{ formatTime(record.checkTime) }}</div>
                <div class="check-location">{{ record.location }}</div>
              </div>
              <div class="status-badge" :class="`status--${record.status.toLowerCase()}`">
                {{ getStatusText(record.status) }}
              </div>
            </div>
          </div>
          <div class="realtime-summary">
            <span class="summary-text">
              当前在场：<strong>{{ realtimeAttendance.currentStats?.checkedInNow || 0 }}</strong> 人
              / 预计：<strong>{{ realtimeAttendance.currentStats?.expectedTotal || 0 }}</strong> 人
              / 迟到：<strong>{{ realtimeAttendance.currentStats?.lateCount || 0 }}</strong> 人
            </span>
          </div>
        </DataCard>

        <!-- 班组考勤排行 -->
        <DataCard title="班组考勤排行" class="team-ranking-card">
          <div class="ranking-list">
            <div 
              class="ranking-item"
              v-for="team in teamRanking"
              :key="team.teamId"
              :class="{ 'top-rank': team.rank <= 3 }"
            >
              <div class="rank-number" :class="`rank--${team.rank}`">
                {{ team.rank }}
              </div>
              <div class="team-info">
                <div class="team-name">{{ team.teamName }}</div>
                <div class="team-stats">
                  {{ team.presentWorkers }}/{{ team.totalWorkers }}人
                </div>
              </div>
              <div class="attendance-rate">
                <div class="rate-value">{{ team.attendanceRate }}%</div>
                <el-progress
                  :percentage="team.attendanceRate"
                  :stroke-width="4"
                  :show-text="false"
                  :color="getRankColor(team.rank)"
                />
              </div>
            </div>
          </div>
        </DataCard>

        <!-- 考勤趋势图表 -->
        <DataCard title="考勤趋势分析" class="trend-chart-card">
          <div class="chart-tabs">
            <div class="tab-buttons">
              <button
                v-for="tab in chartTabs"
                :key="tab.key"
                :class="['tab-btn', { active: activeChartTab === tab.key }]"
                @click="activeChartTab = tab.key"
              >
                {{ tab.label }}
              </button>
            </div>
          </div>
          <DigitalChart
            :option="currentChartOption"
            :loading="chartLoading"
            height="300px"
          />
        </DataCard>
      </div>
    </div>

    <!-- 底部月度统计 -->
    <div class="monthly-section">
      <DataCard title="月度考勤统计" class="monthly-stats-card">
        <div class="monthly-grid">
          <div class="stat-block">
            <div class="stat-icon">
              <el-icon><User /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ attendanceOverview.monthlyStats?.averageAttendance || 0 }}</div>
              <div class="stat-label">平均出勤人数</div>
            </div>
          </div>
          <div class="stat-block">
            <div class="stat-icon">
              <el-icon><PieChart /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ attendanceOverview.monthlyStats?.averageAttendanceRate || 0 }}%</div>
              <div class="stat-label">平均出勤率</div>
            </div>
          </div>
          <div class="stat-block">
            <div class="stat-icon">
              <el-icon><Calendar /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ attendanceOverview.monthlyStats?.totalWorkDays || 0 }}</div>
              <div class="stat-label">工作日总数</div>
            </div>
          </div>
          <div class="stat-block">
            <div class="stat-icon">
              <el-icon><Warning /></el-icon>
            </div>
            <div class="stat-content">
              <div class="stat-value">{{ attendanceOverview.monthlyStats?.totalAbsent || 0 }}</div>
              <div class="stat-label">缺勤总次数</div>
            </div>
          </div>
        </div>
      </DataCard>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { ElMessage } from 'element-plus'
import {
  User,
  Check,
  Close,
  Clock,
  PieChart,
  Calendar,
  Warning
} from '@element-plus/icons-vue'

import StatisticCard from '../components/StatisticCard.vue'
import DataCard from '../components/DataCard.vue'
import DigitalChart from '../components/DigitalChart.vue'

import { digitalTwinService } from '@/api/services/digital-twin.service'
import { formatTime } from '@/utils/format'

// 数据状态
const attendanceOverview = ref<any>({})
const realtimeAttendance = ref<any>({})
const teamRanking = ref<any[]>([])
const attendanceTrends = ref<any>({})
const chartLoading = ref(false)

// 图表标签页
const chartTabs = [
  { key: 'daily', label: '日趋势' },
  { key: 'hourly', label: '时段分布' }
]
const activeChartTab = ref('daily')

// 定时器
let realtimeTimer: NodeJS.Timeout | null = null

// 加载考勤总览
const loadAttendanceOverview = async () => {
  try {
    const response = await digitalTwinService.getAttendanceOverview()
    if (response.success) {
      attendanceOverview.value = response.data
    }
  } catch (error) {
    console.error('Failed to load attendance overview:', error)
    ElMessage.error('加载考勤总览失败')
  }
}

// 加载实时考勤
const loadRealtimeAttendance = async () => {
  try {
    const response = await digitalTwinService.getAttendanceRealtime()
    if (response.success) {
      realtimeAttendance.value = response.data
    }
  } catch (error) {
    console.error('Failed to load realtime attendance:', error)
  }
}

// 加载班组排行
const loadTeamRanking = async () => {
  try {
    const response = await digitalTwinService.getTeamRanking()
    if (response.success) {
      teamRanking.value = response.data
    }
  } catch (error) {
    console.error('Failed to load team ranking:', error)
    ElMessage.error('加载班组排行失败')
  }
}

// 加载考勤趋势
const loadAttendanceTrends = async () => {
  try {
    chartLoading.value = true
    const response = await digitalTwinService.getAttendanceTrends('week')
    if (response.success) {
      attendanceTrends.value = response.data
    }
  } catch (error) {
    console.error('Failed to load attendance trends:', error)
    ElMessage.error('加载考勤趋势失败')
  } finally {
    chartLoading.value = false
  }
}

// 当前图表配置
const currentChartOption = computed(() => {
  const trends = attendanceTrends.value
  if (!trends) return {}

  if (activeChartTab.value === 'daily') {
    const dailyTrend = trends.dailyTrend
    if (!dailyTrend) return {}

    return {
      tooltip: {
        trigger: 'axis',
        backgroundColor: 'rgba(30, 42, 58, 0.9)',
        borderColor: '#3498db',
        textStyle: { color: '#ffffff' }
      },
      legend: {
        data: ['出勤人数', '出勤率'],
        textStyle: { color: '#7f8c8d' }
      },
      grid: {
        left: '3%',
        right: '4%',
        bottom: '3%',
        containLabel: true
      },
      xAxis: {
        type: 'category',
        data: dailyTrend.dates || [],
        axisLabel: { color: '#7f8c8d' },
        axisLine: { lineStyle: { color: '#34495e' } }
      },
      yAxis: [
        {
          type: 'value',
          name: '人数',
          position: 'left',
          axisLabel: { color: '#7f8c8d' },
          axisLine: { lineStyle: { color: '#34495e' } },
          splitLine: { lineStyle: { color: '#34495e', type: 'dashed' } }
        },
        {
          type: 'value',
          name: '出勤率(%)',
          position: 'right',
          axisLabel: { color: '#7f8c8d', formatter: '{value}%' },
          axisLine: { lineStyle: { color: '#34495e' } },
          splitLine: { show: false }
        }
      ],
      series: [
        {
          name: '出勤人数',
          type: 'bar',
          yAxisIndex: 0,
          data: dailyTrend.attendance || [],
          itemStyle: { color: '#3498db' }
        },
        {
          name: '出勤率',
          type: 'line',
          yAxisIndex: 1,
          data: dailyTrend.attendanceRate || [],
          lineStyle: { color: '#2ecc71', width: 3 },
          itemStyle: { color: '#2ecc71' }
        }
      ]
    }
  } else {
    const hourlyDist = trends.hourlyDistribution
    if (!hourlyDist) return {}

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
        data: hourlyDist.hours || [],
        axisLabel: { color: '#7f8c8d' },
        axisLine: { lineStyle: { color: '#34495e' } }
      },
      yAxis: {
        type: 'value',
        name: '签到人数',
        axisLabel: { color: '#7f8c8d' },
        axisLine: { lineStyle: { color: '#34495e' } },
        splitLine: { lineStyle: { color: '#34495e', type: 'dashed' } }
      },
      series: [
        {
          name: '签到人数',
          type: 'bar',
          data: hourlyDist.checkins || [],
          itemStyle: {
            color: {
              type: 'linear',
              x: 0,
              y: 0,
              x2: 0,
              y2: 1,
              colorStops: [
                { offset: 0, color: '#f39c12' },
                { offset: 1, color: '#e67e22' }
              ]
            }
          }
        }
      ]
    }
  }
})

// 工具函数
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Normal': '正常',
    'Late': '迟到',
    'Early': '早退',
    'Absent': '缺勤'
  }
  return statusMap[status] || status
}

const getRankColor = (rank: number) => {
  if (rank === 1) return '#f1c40f'
  if (rank === 2) return '#95a5a6'
  if (rank === 3) return '#e67e22'
  return '#3498db'
}

// 初始化数据
const initData = async () => {
  await Promise.all([
    loadAttendanceOverview(),
    loadRealtimeAttendance(),
    loadTeamRanking(),
    loadAttendanceTrends()
  ])
}

// 启动实时数据更新
const startRealtimeUpdate = () => {
  realtimeTimer = setInterval(() => {
    loadRealtimeAttendance()
  }, 10000) // 10秒更新一次
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
.attendance-screen {
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
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: 16px;
  }
}

// 数据展示区域
.data-section {
  .data-grid {
    display: grid;
    grid-template-columns: 1fr 350px 1fr;
    gap: 20px;
  }
  
  .realtime-attendance-card {
    .realtime-list {
      max-height: 320px;
      overflow-y: auto;
      margin-bottom: 16px;
      
      .attendance-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px;
        margin-bottom: 8px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.03);
        border: 1px solid rgba(255, 255, 255, 0.05);
        
        &:hover {
          background: rgba(255, 255, 255, 0.05);
          border-color: #3498db;
        }
        
        .worker-avatar {
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
        
        .worker-info {
          flex: 1;
          
          .worker-name {
            font-size: 14px;
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 2px;
          }
          
          .worker-team {
            font-size: 12px;
            color: #7f8c8d;
          }
        }
        
        .check-info {
          text-align: right;
          
          .check-time {
            font-size: 13px;
            font-weight: 500;
            color: #ffffff;
            margin-bottom: 2px;
          }
          
          .check-location {
            font-size: 11px;
            color: #7f8c8d;
          }
        }
        
        .status-badge {
          padding: 4px 8px;
          border-radius: 12px;
          font-size: 11px;
          font-weight: 500;
          
          &.status--normal {
            background: rgba(46, 204, 113, 0.2);
            color: #2ecc71;
          }
          
          &.status--late {
            background: rgba(243, 156, 18, 0.2);
            color: #f39c12;
          }
          
          &.status--early {
            background: rgba(155, 89, 182, 0.2);
            color: #9b59b6;
          }
        }
      }
    }
    
    .realtime-summary {
      padding: 12px;
      border-radius: 6px;
      background: rgba(52, 152, 219, 0.1);
      border: 1px solid rgba(52, 152, 219, 0.2);
      text-align: center;
      
      .summary-text {
        font-size: 13px;
        color: #ecf0f1;
        
        strong {
          color: #3498db;
          font-weight: 600;
        }
      }
    }
  }
  
  .team-ranking-card {
    .ranking-list {
      .ranking-item {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 12px;
        margin-bottom: 8px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.03);
        border: 1px solid rgba(255, 255, 255, 0.05);
        
        &.top-rank {
          background: rgba(241, 196, 15, 0.1);
          border-color: rgba(241, 196, 15, 0.3);
        }
        
        .rank-number {
          width: 32px;
          height: 32px;
          border-radius: 50%;
          display: flex;
          align-items: center;
          justify-content: center;
          font-weight: 700;
          font-size: 14px;
          
          &.rank--1 {
            background: linear-gradient(135deg, #f1c40f, #f39c12);
            color: #ffffff;
          }
          
          &.rank--2 {
            background: linear-gradient(135deg, #95a5a6, #7f8c8d);
            color: #ffffff;
          }
          
          &.rank--3 {
            background: linear-gradient(135deg, #e67e22, #d35400);
            color: #ffffff;
          }
          
          &:not(.rank--1):not(.rank--2):not(.rank--3) {
            background: rgba(52, 152, 219, 0.2);
            color: #3498db;
          }
        }
        
        .team-info {
          flex: 1;
          
          .team-name {
            font-size: 14px;
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 2px;
          }
          
          .team-stats {
            font-size: 12px;
            color: #7f8c8d;
          }
        }
        
        .attendance-rate {
          width: 80px;
          text-align: right;
          
          .rate-value {
            font-size: 16px;
            font-weight: 700;
            color: #ffffff;
            margin-bottom: 4px;
          }
        }
      }
    }
  }
  
  .trend-chart-card {
    .chart-tabs {
      margin-bottom: 16px;
      
      .tab-buttons {
        display: flex;
        gap: 8px;
        
        .tab-btn {
          padding: 8px 16px;
          border: 1px solid rgba(255, 255, 255, 0.2);
          background: transparent;
          color: #7f8c8d;
          border-radius: 20px;
          font-size: 12px;
          cursor: pointer;
          transition: all 0.3s ease;
          
          &:hover {
            border-color: #3498db;
            color: #3498db;
          }
          
          &.active {
            background: #3498db;
            border-color: #3498db;
            color: #ffffff;
          }
        }
      }
    }
  }
}

// 月度统计区域
.monthly-section {
  .monthly-stats-card {
    .monthly-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 20px;
      
      .stat-block {
        display: flex;
        align-items: center;
        gap: 16px;
        padding: 20px;
        border-radius: 8px;
        background: rgba(255, 255, 255, 0.05);
        border: 1px solid rgba(255, 255, 255, 0.1);
        
        .stat-icon {
          width: 50px;
          height: 50px;
          border-radius: 50%;
          background: rgba(52, 152, 219, 0.2);
          display: flex;
          align-items: center;
          justify-content: center;
          
          .el-icon {
            font-size: 22px;
            color: #3498db;
          }
        }
        
        .stat-content {
          .stat-value {
            font-size: 24px;
            font-weight: 700;
            color: #ffffff;
            margin-bottom: 4px;
          }
          
          .stat-label {
            font-size: 13px;
            color: #7f8c8d;
          }
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 1200px) {
  .data-section .data-grid {
    grid-template-columns: 1fr;
    gap: 16px;
  }
}

@media (max-width: 768px) {
  .attendance-screen {
    padding: 12px;
  }
  
  .stats-section .stats-grid {
    grid-template-columns: 1fr;
    gap: 12px;
  }
  
  .monthly-section .monthly-grid {
    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
    gap: 12px;
  }
}
</style>