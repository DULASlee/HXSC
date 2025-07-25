<template>
  <div class="project-dashboard-container" v-loading="loading">
    <page-header 
      :title="`项目仪表盘 - ${projectData.name}`" 
      :back-path="'/dashboard'"
    >
      <template #actions>
        <el-button type="primary" @click="refreshData">
          <el-icon><refresh /></el-icon>
          刷新数据
        </el-button>
      </template>
    </page-header>

    <div class="dashboard-content">
      <!-- 项目概览 -->
      <div class="overview-cards">
        <stat-card
          title="总工人数"
          :value="projectStats.totalWorkers"
          suffix="人"
          color="#409eff"
          icon="User"
        />
        <stat-card
          title="在岗人数"
          :value="projectStats.onSiteWorkers"
          suffix="人"
          color="#67c23a"
          icon="UserFilled"
        />
        <stat-card
          title="设备总数"
          :value="projectStats.totalDevices"
          suffix="台"
          color="#e6a23c"
          icon="Monitor"
        />
        <stat-card
          title="在线设备"
          :value="projectStats.onlineDevices"
          suffix="台"
          color="#67c23a"
          icon="Monitor"
        />
        <stat-card
          title="安全事件"
          :value="projectStats.safetyIncidents"
          suffix="起"
          color="#f56c6c"
          icon="Warning"
        />
        <stat-card
          title="项目进度"
          :value="projectStats.progress"
          suffix="%"
          color="#909399"
          icon="TrendCharts"
        />
      </div>

      <!-- 图表区域 -->
      <div class="charts-container">
        <!-- 考勤趋势图 -->
        <div class="chart-card">
          <div class="chart-header">
            <h3>考勤趋势</h3>
            <el-radio-group v-model="attendancePeriod" size="small" @change="fetchAttendanceChart">
              <el-radio-button value="week">近7天</el-radio-button>
              <el-radio-button value="month">近30天</el-radio-button>
            </el-radio-group>
          </div>
          <div class="chart-content">
            <chart-card
              :option="attendanceChartOption"
              height="300px"
            />
          </div>
        </div>

        <!-- 设备状态分布 -->
        <div class="chart-card">
          <div class="chart-header">
            <h3>设备状态分布</h3>
          </div>
          <div class="chart-content">
            <chart-card
              :option="deviceStatusChartOption"
              height="300px"
            />
          </div>
        </div>

        <!-- 安全事件级别分布 -->
        <div class="chart-card">
          <div class="chart-header">
            <h3>安全事件级别分布</h3>
          </div>
          <div class="chart-content">
            <chart-card
              :option="safetyChartOption"
              height="300px"
            />
          </div>
        </div>

        <!-- 班组出勤率排行 -->
        <div class="chart-card">
          <div class="chart-header">
            <h3>班组出勤率排行</h3>
          </div>
          <div class="chart-content">
            <chart-card
              :option="teamAttendanceChartOption"
              height="300px"
            />
          </div>
        </div>
      </div>

      <!-- 实时数据表格 -->
      <div class="realtime-tables">
        <!-- 最新考勤记录 -->
        <div class="table-card">
          <div class="table-header">
            <h3>最新考勤记录</h3>
            <el-link type="primary" @click="goToAttendance">查看全部</el-link>
          </div>
          <data-table
            :data="recentAttendance"
            :show-pagination="false"
            height="200px"
          >
            <el-table-column prop="workerName" label="工人姓名" width="100" />
            <el-table-column prop="teamName" label="班组" width="100" />
            <el-table-column prop="checkInTime" label="签到时间" width="120">
              <template #default="{ row }">
                {{ formatTime(row.checkInTime) }}
              </template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="80">
              <template #default="{ row }">
                <status-tag
                  :status="row.status"
                  :status-map="{
                    'Normal': { type: 'success', label: '正常' },
                    'Late': { type: 'warning', label: '迟到' }
                  }"
                />
              </template>
            </el-table-column>
          </data-table>
        </div>

        <!-- 设备告警 -->
        <div class="table-card">
          <div class="table-header">
            <h3>设备告警</h3>
            <el-link type="primary" @click="goToDeviceMonitor">查看全部</el-link>
          </div>
          <data-table
            :data="deviceAlerts"
            :show-pagination="false"
            height="200px"
          >
            <el-table-column prop="deviceName" label="设备名称" width="120" />
            <el-table-column prop="alertType" label="告警类型" width="100" />
            <el-table-column prop="alertMessage" label="告警信息" min-width="200" show-overflow-tooltip />
            <el-table-column prop="alertTime" label="告警时间" width="120">
              <template #default="{ row }">
                {{ formatTime(row.alertTime) }}
              </template>
            </el-table-column>
          </data-table>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Refresh } from '@element-plus/icons-vue'
import { dashboardService } from '@/api/services'
import { formatTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import StatCard from '@/components/StatCard.vue'
import ChartCard from '@/components/ChartCard.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'

const route = useRoute()
const router = useRouter()
const projectId = route.params.id as string

// 数据
const loading = ref(false)
const projectData = ref<any>({})
const projectStats = ref({
  totalWorkers: 0,
  onSiteWorkers: 0,
  totalDevices: 0,
  onlineDevices: 0,
  safetyIncidents: 0,
  progress: 0
})

// 考勤周期
const attendancePeriod = ref('week')

// 图表数据
const attendanceChartOption = ref<any>({})
const deviceStatusChartOption = ref<any>({})
const safetyChartOption = ref<any>({})
const teamAttendanceChartOption = ref<any>({})

// 实时数据
const recentAttendance = ref<any[]>([])
const deviceAlerts = ref<any[]>([])

onMounted(() => {
  fetchProjectData()
  fetchProjectStats()
  fetchChartData()
  fetchRealtimeData()
})

// 获取项目信息
const fetchProjectData = async () => {
  try {
    const { data } = await dashboardService.getProjectInfo(projectId)
    projectData.value = data
  } catch (error) {
    console.error('获取项目信息失败:', error)
  }
}

// 获取项目统计
const fetchProjectStats = async () => {
  try {
    const { data } = await dashboardService.getProjectStats(projectId)
    projectStats.value = data
  } catch (error) {
    console.error('获取项目统计失败:', error)
  }
}

// 获取图表数据
const fetchChartData = async () => {
  try {
    // 考勤趋势图
    await fetchAttendanceChart()

    // 设备状态分布
    const { data: deviceData } = await dashboardService.getProjectDeviceStatus(projectId)
    deviceStatusChartOption.value = {
      title: { text: '设备状态分布' },
      series: [{
        type: 'pie',
        radius: '60%',
        data: deviceData
      }]
    }

    // 安全事件分布
    const { data: safetyData } = await dashboardService.getProjectSafetyStats(projectId)
    safetyChartOption.value = {
      title: { text: '安全事件级别分布' },
      series: [{
        type: 'pie',
        radius: '60%',
        data: safetyData
      }]
    }

    // 班组出勤率
    const { data: teamData } = await dashboardService.getProjectTeamAttendance(projectId)
    teamAttendanceChartOption.value = {
      title: { text: '班组出勤率排行' },
      xAxis: {
        type: 'category',
        data: teamData.teams
      },
      yAxis: {
        type: 'value',
        max: 100,
        axisLabel: { formatter: '{value}%' }
      },
      series: [{
        type: 'bar',
        data: teamData.rates
      }]
    }
  } catch (error) {
    console.error('获取图表数据失败:', error)
  }
}

// 获取考勤趋势图
const fetchAttendanceChart = async () => {
  try {
    const { data } = await dashboardService.getProjectAttendanceTrend(projectId, attendancePeriod.value)
    attendanceChartOption.value = {
      title: { text: '考勤趋势' },
      xAxis: {
        type: 'category',
        data: data.dates
      },
      yAxis: { type: 'value' },
      series: [
        {
          name: '出勤人数',
          type: 'line',
          data: data.attendance,
          smooth: true
        },
        {
          name: '应出勤人数',
          type: 'line',
          data: data.expected,
          smooth: true
        }
      ]
    }
  } catch (error) {
    console.error('获取考勤趋势失败:', error)
  }
}

// 获取实时数据
const fetchRealtimeData = async () => {
  try {
    const { data } = await dashboardService.getProjectRealtimeData(projectId)
    recentAttendance.value = data.recentAttendance
    deviceAlerts.value = data.deviceAlerts
  } catch (error) {
    console.error('获取实时数据失败:', error)
  }
}

// 刷新数据
const refreshData = () => {
  fetchProjectStats()
  fetchChartData()
  fetchRealtimeData()
  ElMessage.success('数据已刷新')
}

// 跳转到考勤管理
const goToAttendance = () => {
  router.push(`/attendance/list?projectId=${projectId}`)
}

// 跳转到设备监控
const goToDeviceMonitor = () => {
  router.push(`/device/monitor?projectId=${projectId}`)
}
</script>

<style lang="scss" scoped>
.project-dashboard-container {
  padding: 20px;
}

.dashboard-content {
  margin-top: 20px;
}

.overview-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.charts-container {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 20px;
  margin-bottom: 30px;

  @media (max-width: 1200px) {
    grid-template-columns: 1fr;
  }
}

.chart-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  overflow: hidden;

  .chart-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 16px 20px;
    border-bottom: 1px solid #ebeef5;

    h3 {
      margin: 0;
      font-size: 16px;
      font-weight: 500;
    }
  }

  .chart-content {
    padding: 20px;
  }
}

.realtime-tables {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;

  @media (max-width: 768px) {
    grid-template-columns: 1fr;
  }
}

.table-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  overflow: hidden;

  .table-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 16px 20px;
    border-bottom: 1px solid #ebeef5;

    h3 {
      margin: 0;
      font-size: 16px;
      font-weight: 500;
    }
  }
}
</style>