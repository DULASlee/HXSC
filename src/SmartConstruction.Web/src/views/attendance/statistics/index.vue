<template>
  <div class="attendance-statistics-container">
    <page-header 
      title="考勤统计" 
      subtitle="查看和分析工人考勤统计数据"
    >
      <template #actions>
        <el-button type="primary" @click="handleExport">
          <el-icon><download /></el-icon>
          导出报表
        </el-button>
      </template>
    </page-header>

    <!-- 筛选条件 -->
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="统计维度" prop="dimension">
        <el-select
          v-model="searchParams.dimension"
          placeholder="请选择统计维度"
        >
          <el-option label="按工人" value="worker" />
          <el-option label="按班组" value="team" />
          <el-option label="按公司" value="company" />
        </el-select>
      </el-form-item>
      
      <el-form-item label="统计时间" prop="timeRange">
        <el-date-picker
          v-model="timeRange"
          type="monthrange"
          range-separator="至"
          start-placeholder="开始月份"
          end-placeholder="结束月份"
          format="YYYY-MM"
          value-format="YYYY-MM"
          @change="handleTimeRangeChange"
        />
      </el-form-item>
      
      <el-form-item label="所属公司" prop="companyId">
        <el-select
          v-model="searchParams.companyId"
          placeholder="请选择公司"
          clearable
          filterable
          @change="handleCompanyChange"
        >
          <el-option
            v-for="item in companyOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="所属班组" prop="teamId">
        <el-select
          v-model="searchParams.teamId"
          placeholder="请选择班组"
          clearable
          filterable
        >
          <el-option
            v-for="item in filteredTeamOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
    </search-form>

    <!-- 统计卡片 -->
    <div class="statistics-cards">
      <stat-card
        title="总出勤人数"
        :value="overviewStats.totalWorkers"
        suffix="人"
        color="#67c23a"
        icon="User"
      />
      <stat-card
        title="总出勤天数"
        :value="overviewStats.totalAttendanceDays"
        suffix="天"
        color="#409eff"
        icon="Calendar"
      />
      <stat-card
        title="平均出勤率"
        :value="overviewStats.averageAttendanceRate"
        suffix="%"
        color="#e6a23c"
        icon="TrendCharts"
      />
      <stat-card
        title="迟到次数"
        :value="overviewStats.totalLateCount"
        suffix="次"
        color="#f56c6c"
        icon="Warning"
      />
    </div>

    <!-- 图表区域 -->
    <div class="charts-container">
      <div class="chart-card">
        <div class="chart-header">
          <h3>出勤率趋势图</h3>
        </div>
        <div class="chart-content">
          <chart-card
            :option="attendanceRateChartOption"
            height="300px"
          />
        </div>
      </div>

      <div class="chart-card">
        <div class="chart-header">
          <h3>考勤状态分布</h3>
        </div>
        <div class="chart-content">
          <chart-card
            :option="attendanceStatusChartOption"
            height="300px"
          />
        </div>
      </div>
    </div>

    <!-- 详细统计表格 -->
    <div class="table-container">
      <h3>详细统计数据</h3>
      <data-table
        ref="tableRef"
        v-loading="loading"
        :data="tableData"
        :total="total"
        :page="searchParams.pageIndex"
        :size="searchParams.pageSize"
        @page-change="handlePageChange"
        @size-change="handleSizeChange"
      >
        <!-- 按工人统计 -->
        <template v-if="searchParams.dimension === 'worker'">
          <el-table-column prop="workerName" label="工人姓名" min-width="100" />
          <el-table-column prop="workerEmployeeNo" label="工号" min-width="120" />
          <el-table-column prop="companyName" label="所属公司" min-width="150" />
          <el-table-column prop="teamName" label="所属班组" min-width="120" />
        </template>
        
        <!-- 按班组统计 -->
        <template v-else-if="searchParams.dimension === 'team'">
          <el-table-column prop="teamName" label="班组名称" min-width="150" />
          <el-table-column prop="companyName" label="所属公司" min-width="150" />
          <el-table-column prop="workerCount" label="工人数量" width="100" align="center" />
        </template>
        
        <!-- 按公司统计 -->
        <template v-else>
          <el-table-column prop="companyName" label="公司名称" min-width="200" />
          <el-table-column prop="teamCount" label="班组数量" width="100" align="center" />
          <el-table-column prop="workerCount" label="工人数量" width="100" align="center" />
        </template>

        <!-- 通用统计列 -->
        <el-table-column prop="totalDays" label="应出勤天数" width="120" align="center" />
        <el-table-column prop="actualDays" label="实际出勤天数" width="120" align="center" />
        <el-table-column prop="attendanceRate" label="出勤率" width="100" align="center">
          <template #default="{ row }">
            <span :class="getAttendanceRateClass(row.attendanceRate)">
              {{ row.attendanceRate }}%
            </span>
          </template>
        </el-table-column>
        <el-table-column prop="normalDays" label="正常出勤" width="100" align="center" />
        <el-table-column prop="lateDays" label="迟到次数" width="100" align="center" />
        <el-table-column prop="earlyLeaveDays" label="早退次数" width="100" align="center" />
        <el-table-column prop="absentDays" label="缺勤次数" width="100" align="center" />
        <el-table-column prop="leaveDays" label="请假天数" width="100" align="center" />
        <el-table-column prop="overtimeHours" label="加班时长" width="100" align="center">
          <template #default="{ row }">
            {{ row.overtimeHours }}h
          </template>
        </el-table-column>
        
        <template #actions="{ row }">
          <el-button
            type="primary"
            link
            @click="handleViewDetail(row)"
          >
            查看详情
          </el-button>
        </template>
      </data-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Download } from '@element-plus/icons-vue'
import { attendanceService, companyService, teamService } from '@/api/services'
import { formatDate } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import SearchForm from '@/components/SearchForm.vue'
import DataTable from '@/components/DataTable.vue'
import StatCard from '@/components/StatCard.vue'
import ChartCard from '@/components/ChartCard.vue'

// 搜索参数
const searchParams = reactive({
  dimension: 'worker',
  startMonth: '',
  endMonth: '',
  companyId: '',
  teamId: '',
  pageIndex: 1,
  pageSize: 10
})

// 时间范围
const timeRange = ref<[string, string] | null>(null)

// 数据
const loading = ref(false)
const tableData = ref<any[]>([])
const total = ref(0)
const overviewStats = ref({
  totalWorkers: 0,
  totalAttendanceDays: 0,
  averageAttendanceRate: 0,
  totalLateCount: 0
})

// 选项数据
const companyOptions = ref<{ label: string; value: string }[]>([])
const teamOptions = ref<{ label: string; value: string; companyId: string }[]>([])

// 图表数据
const attendanceRateChartOption = ref<any>({})
const attendanceStatusChartOption = ref<any>({})

// 过滤后的班组选项
const filteredTeamOptions = computed(() => {
  if (!searchParams.companyId) return teamOptions.value
  return teamOptions.value.filter(team => team.companyId === searchParams.companyId)
})

// 组件引用
const searchFormRef = ref()
const tableRef = ref()

// 监听统计维度变化
watch(() => searchParams.dimension, () => {
  searchParams.pageIndex = 1
  fetchData()
})

// 初始化
onMounted(() => {
  // 设置默认时间范围为当月
  const now = new Date()
  const currentMonth = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}`
  timeRange.value = [currentMonth, currentMonth]
  searchParams.startMonth = currentMonth
  searchParams.endMonth = currentMonth
  
  fetchCompanies()
  fetchTeams()
  fetchData()
  fetchOverviewStats()
  fetchChartData()
})

// 获取公司列表
const fetchCompanies = async () => {
  try {
    const { data } = await companyService.getAll()
    companyOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id
    }))
  } catch (error) {
    console.error('获取公司列表失败:', error)
  }
}

// 获取班组列表
const fetchTeams = async () => {
  try {
    const { data } = await teamService.getAll()
    teamOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id,
      companyId: item.companyId
    }))
  } catch (error) {
    console.error('获取班组列表失败:', error)
  }
}

// 获取统计数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await attendanceService.getStatistics(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取考勤统计失败:', error)
    ElMessage.error('获取考勤统计失败')
  } finally {
    loading.value = false
  }
}

// 获取概览统计
const fetchOverviewStats = async () => {
  try {
    const { data } = await attendanceService.getOverviewStats(searchParams)
    overviewStats.value = data
  } catch (error) {
    console.error('获取概览统计失败:', error)
  }
}

// 获取图表数据
const fetchChartData = async () => {
  try {
    // 获取出勤率趋势数据
    const { data: trendData } = await attendanceService.getAttendanceRateTrend(searchParams)
    attendanceRateChartOption.value = {
      title: {
        text: '出勤率趋势'
      },
      tooltip: {
        trigger: 'axis'
      },
      xAxis: {
        type: 'category',
        data: trendData.dates
      },
      yAxis: {
        type: 'value',
        max: 100,
        axisLabel: {
          formatter: '{value}%'
        }
      },
      series: [{
        name: '出勤率',
        type: 'line',
        data: trendData.rates,
        smooth: true,
        itemStyle: {
          color: '#409eff'
        }
      }]
    }

    // 获取考勤状态分布数据
    const { data: statusData } = await attendanceService.getAttendanceStatusDistribution(searchParams)
    attendanceStatusChartOption.value = {
      title: {
        text: '考勤状态分布'
      },
      tooltip: {
        trigger: 'item'
      },
      series: [{
        name: '考勤状态',
        type: 'pie',
        radius: '60%',
        data: statusData,
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }]
    }
  } catch (error) {
    console.error('获取图表数据失败:', error)
  }
}

// 搜索
const handleSearch = (values: any) => {
  searchParams.pageIndex = 1
  Object.assign(searchParams, values)
  fetchData()
  fetchOverviewStats()
  fetchChartData()
}

// 重置
const handleReset = () => {
  searchParams.pageIndex = 1
  timeRange.value = null
  searchParams.startMonth = ''
  searchParams.endMonth = ''
  searchParams.companyId = ''
  searchParams.teamId = ''
  fetchData()
  fetchOverviewStats()
  fetchChartData()
}

// 页码变化
const handlePageChange = (page: number) => {
  searchParams.pageIndex = page
  fetchData()
}

// 每页条数变化
const handleSizeChange = (size: number) => {
  searchParams.pageSize = size
  searchParams.pageIndex = 1
  fetchData()
}

// 公司变化
const handleCompanyChange = () => {
  searchParams.teamId = ''
}

// 时间范围变化
const handleTimeRangeChange = (months: [string, string] | null) => {
  if (months) {
    searchParams.startMonth = months[0]
    searchParams.endMonth = months[1]
  } else {
    searchParams.startMonth = ''
    searchParams.endMonth = ''
  }
}

// 查看详情
const handleViewDetail = (row: any) => {
  // 根据统计维度跳转到不同的详情页
  if (searchParams.dimension === 'worker') {
    // 跳转到工人考勤详情
    ElMessage.info('工人考勤详情功能开发中...')
  } else if (searchParams.dimension === 'team') {
    // 跳转到班组考勤详情
    ElMessage.info('班组考勤详情功能开发中...')
  } else {
    // 跳转到公司考勤详情
    ElMessage.info('公司考勤详情功能开发中...')
  }
}

// 导出报表
const handleExport = async () => {
  try {
    const { data } = await attendanceService.exportStatistics(searchParams)
    const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `考勤统计报表_${formatDate(new Date())}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch (error) {
    console.error('导出失败:', error)
    ElMessage.error('导出失败')
  }
}

// 获取出勤率样式类
const getAttendanceRateClass = (rate: number) => {
  if (rate >= 95) return 'rate-excellent'
  if (rate >= 85) return 'rate-good'
  if (rate >= 75) return 'rate-normal'
  return 'rate-poor'
}
</script>

<style lang="scss" scoped>
.attendance-statistics-container {
  padding: 20px;
}

.statistics-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin: 20px 0;
}

.charts-container {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin: 20px 0;

  @media (max-width: 768px) {
    grid-template-columns: 1fr;
  }
}

.chart-card {
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  overflow: hidden;

  .chart-header {
    padding: 16px 20px;
    border-bottom: 1px solid #ebeef5;

    h3 {
      margin: 0;
      font-size: 16px;
      font-weight: 500;
      color: #303133;
    }
  }

  .chart-content {
    padding: 20px;
  }
}

.table-container {
  margin-top: 20px;

  h3 {
    margin-bottom: 16px;
    font-size: 16px;
    font-weight: 500;
    color: #303133;
  }
}

// 出勤率颜色样式
.rate-excellent {
  color: #67c23a;
  font-weight: 500;
}

.rate-good {
  color: #409eff;
  font-weight: 500;
}

.rate-normal {
  color: #e6a23c;
  font-weight: 500;
}

.rate-poor {
  color: #f56c6c;
  font-weight: 500;
}
</style>