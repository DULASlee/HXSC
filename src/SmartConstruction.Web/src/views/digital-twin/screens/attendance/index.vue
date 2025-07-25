<template>
  <div class="attendance-screen">
    <div class="screen-header">
      <h2 class="screen-title">项目考勤管理</h2>
      <div class="screen-controls">
        <el-button type="primary" size="small" @click="refreshData">
          <el-icon><Refresh /></el-icon>
          刷新数据
        </el-button>
      </div>
    </div>

    <div class="screen-content">
      <!-- 考勤统计卡片 -->
      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#409eff"><User /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ attendanceStats.totalWorkers }}</div>
            <div class="stat-label">总工人数</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#67c23a"><CircleCheckFilled /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ attendanceStats.presentToday }}</div>
            <div class="stat-label">今日出勤</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#e6a23c"><Clock /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ attendanceStats.attendanceRate }}%</div>
            <div class="stat-label">出勤率</div>
          </div>
        </div>
        
        <div class="stat-card">
          <div class="stat-icon">
            <el-icon color="#f56c6c"><Warning /></el-icon>
          </div>
          <div class="stat-content">
            <div class="stat-value">{{ attendanceStats.absentToday }}</div>
            <div class="stat-label">今日缺勤</div>
          </div>
        </div>
      </div>

      <!-- 考勤详情 -->
      <div class="attendance-details">
        <div class="section-header">
          <h3>今日考勤详情</h3>
          <div class="filter-controls">
            <el-select v-model="selectedTeam" placeholder="选择班组" size="small">
              <el-option label="全部班组" value="" />
              <el-option label="钢筋工班组" value="steel" />
              <el-option label="混凝土工班组" value="concrete" />
              <el-option label="钢结构工班组" value="steel-structure" />
            </el-select>
          </div>
        </div>
        
        <div class="attendance-table">
          <el-table :data="filteredAttendanceData" style="width: 100%" size="small">
            <el-table-column prop="workerName" label="工人姓名" width="120" />
            <el-table-column prop="teamName" label="班组" width="120" />
            <el-table-column prop="checkInTime" label="签到时间" width="120" />
            <el-table-column prop="checkOutTime" label="签退时间" width="120" />
            <el-table-column prop="workHours" label="工作时长" width="100" />
            <el-table-column prop="status" label="状态" width="80">
              <template #default="scope">
                <el-tag 
                  :type="scope.row.status === 'present' ? 'success' : 'danger'"
                  size="small"
                >
                  {{ scope.row.status === 'present' ? '出勤' : '缺勤' }}
                </el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="location" label="工作区域" />
          </el-table>
        </div>
      </div>

      <!-- 班组统计 -->
      <div class="team-stats">
        <div class="section-header">
          <h3>班组统计</h3>
        </div>
        
        <div class="team-grid">
          <div 
            v-for="team in teamStats" 
            :key="team.name"
            class="team-card"
          >
            <div class="team-header">
              <span class="team-name">{{ team.name }}</span>
              <span class="team-count">{{ team.totalWorkers }}人</span>
            </div>
            
            <div class="team-progress">
              <div class="progress-item">
                <span class="progress-label">出勤率</span>
                <el-progress 
                  :percentage="team.attendanceRate" 
                  :color="getProgressColor(team.attendanceRate)"
                  :stroke-width="8"
                />
              </div>
              
              <div class="progress-item">
                <span class="progress-label">工作时长</span>
                <span class="progress-value">{{ team.avgWorkHours }}小时</span>
              </div>
            </div>
            
            <div class="team-details">
              <div class="detail-item">
                <span class="detail-label">出勤:</span>
                <span class="detail-value">{{ team.presentCount }}人</span>
              </div>
              <div class="detail-item">
                <span class="detail-label">缺勤:</span>
                <span class="detail-value">{{ team.absentCount }}人</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { 
  Refresh, 
  User, 
  CircleCheckFilled, 
  Clock, 
  Warning 
} from '@element-plus/icons-vue'

interface AttendanceStats {
  totalWorkers: number
  presentToday: number
  attendanceRate: number
  absentToday: number
}

interface AttendanceRecord {
  workerName: string
  teamName: string
  checkInTime: string
  checkOutTime: string
  workHours: number
  status: 'present' | 'absent'
  location: string
}

interface TeamStats {
  name: string
  totalWorkers: number
  presentCount: number
  absentCount: number
  attendanceRate: number
  avgWorkHours: number
}

// 响应式数据
const attendanceStats = ref<AttendanceStats>({
  totalWorkers: 0,
  presentToday: 0,
  attendanceRate: 0,
  absentToday: 0
})

const attendanceData = ref<AttendanceRecord[]>([])
const teamStats = ref<TeamStats[]>([])
const selectedTeam = ref('')

// 模拟考勤数据
const mockAttendanceData: AttendanceRecord[] = [
  {
    workerName: '张三',
    teamName: '钢筋工班组',
    checkInTime: '08:00',
    checkOutTime: '17:30',
    workHours: 8.5,
    status: 'present',
    location: 'A区施工现场'
  },
  {
    workerName: '李四',
    teamName: '混凝土工班组',
    checkInTime: '08:15',
    checkOutTime: '17:45',
    workHours: 8.5,
    status: 'present',
    location: 'B区施工现场'
  },
  {
    workerName: '王五',
    teamName: '钢结构工班组',
    checkInTime: '08:00',
    checkOutTime: '17:00',
    workHours: 8,
    status: 'present',
    location: 'C区施工现场'
  },
  {
    workerName: '赵六',
    teamName: '钢筋工班组',
    checkInTime: '',
    checkOutTime: '',
    workHours: 0,
    status: 'absent',
    location: ''
  },
  {
    workerName: '钱七',
    teamName: '混凝土工班组',
    checkInTime: '08:30',
    checkOutTime: '17:30',
    workHours: 8,
    status: 'present',
    location: 'A区施工现场'
  }
]

// 过滤后的考勤数据
const filteredAttendanceData = computed(() => {
  if (!selectedTeam.value) {
    return attendanceData.value
  }
  return attendanceData.value.filter(item => {
    const teamMap: Record<string, string> = {
      'steel': '钢筋工班组',
      'concrete': '混凝土工班组',
      'steel-structure': '钢结构工班组'
    }
    return item.teamName === teamMap[selectedTeam.value]
  })
})

// 刷新数据
const refreshData = () => {
  loadAttendanceData()
  loadTeamStats()
}

// 加载考勤数据
const loadAttendanceData = () => {
  attendanceData.value = mockAttendanceData
  
  // 计算统计数据
  const totalWorkers = attendanceData.value.length
  const presentCount = attendanceData.value.filter(item => item.status === 'present').length
  const absentCount = totalWorkers - presentCount
  const attendanceRate = Math.round((presentCount / totalWorkers) * 100)
  
  attendanceStats.value = {
    totalWorkers,
    presentToday: presentCount,
    attendanceRate,
    absentToday: absentCount
  }
}

// 加载班组统计
const loadTeamStats = () => {
  const teams = ['钢筋工班组', '混凝土工班组', '钢结构工班组']
  
  teamStats.value = teams.map(teamName => {
    const teamData = attendanceData.value.filter(item => item.teamName === teamName)
    const totalWorkers = teamData.length
    const presentCount = teamData.filter(item => item.status === 'present').length
    const absentCount = totalWorkers - presentCount
    const attendanceRate = totalWorkers > 0 ? Math.round((presentCount / totalWorkers) * 100) : 0
    const avgWorkHours = teamData.length > 0 
      ? Math.round(teamData.reduce((sum, item) => sum + item.workHours, 0) / teamData.length * 10) / 10
      : 0
    
    return {
      name: teamName,
      totalWorkers,
      presentCount,
      absentCount,
      attendanceRate,
      avgWorkHours
    }
  })
}

// 获取进度条颜色
const getProgressColor = (percentage: number) => {
  if (percentage >= 90) return '#67c23a'
  if (percentage >= 70) return '#e6a23c'
  return '#f56c6c'
}

// 初始化
onMounted(() => {
  refreshData()
})
</script>

<style scoped>
.attendance-screen {
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
  overflow-y: auto;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 20px;
}

.stat-card {
  display: flex;
  align-items: center;
  padding: 20px;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.stat-icon {
  margin-right: 16px;
  font-size: 32px;
}

.stat-content {
  flex: 1;
}

.stat-value {
  font-size: 28px;
  font-weight: 600;
  color: #ffffff;
  line-height: 1;
}

.stat-label {
  font-size: 14px;
  color: #909399;
  margin-top: 4px;
}

.attendance-details,
.team-stats {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 20px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-header h3 {
  margin: 0;
  font-size: 18px;
  font-weight: 600;
  color: #ffffff;
}

.attendance-table {
  background: rgba(255, 255, 255, 0.02);
  border-radius: 8px;
  overflow: hidden;
}

.team-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 20px;
}

.team-card {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 16px;
  border: 1px solid rgba(255, 255, 255, 0.1);
}

.team-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 16px;
}

.team-name {
  font-weight: 600;
  color: #ffffff;
  font-size: 16px;
}

.team-count {
  color: #409eff;
  font-weight: 500;
}

.team-progress {
  margin-bottom: 16px;
}

.progress-item {
  margin-bottom: 12px;
}

.progress-item:last-child {
  margin-bottom: 0;
}

.progress-label {
  display: block;
  font-size: 14px;
  color: #909399;
  margin-bottom: 4px;
}

.progress-value {
  color: #ffffff;
  font-weight: 500;
}

.team-details {
  display: flex;
  justify-content: space-between;
}

.detail-item {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.detail-label {
  font-size: 12px;
  color: #909399;
  margin-bottom: 4px;
}

.detail-value {
  font-size: 16px;
  font-weight: 600;
  color: #ffffff;
}
</style> 