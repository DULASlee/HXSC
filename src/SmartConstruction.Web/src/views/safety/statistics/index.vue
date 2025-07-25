<template>
  <div class="safety-statistics-container">
    <page-header 
      title="安全统计" 
      subtitle="查看和分析安全事件统计数据"
    />

    <!-- 统计卡片 -->
    <div class="statistics-cards">
      <stat-card
        title="本月事件总数"
        :value="overviewStats.monthlyTotal"
        suffix="起"
        color="#409eff"
        icon="Warning"
      />
      <stat-card
        title="待处理事件"
        :value="overviewStats.pendingCount"
        suffix="起"
        color="#e6a23c"
        icon="Clock"
      />
      <stat-card
        title="严重事件"
        :value="overviewStats.seriousCount"
        suffix="起"
        color="#f56c6c"
        icon="Warning"
      />
      <stat-card
        title="安全评分"
        :value="overviewStats.safetyScore"
        suffix="分"
        color="#67c23a"
        icon="Trophy"
      />
    </div>

    <!-- 图表区域 -->
    <div class="charts-container">
      <div class="chart-card">
        <div class="chart-header">
          <h3>事件趋势图</h3>
        </div>
        <div class="chart-content">
          <chart-card
            :option="trendChartOption"
            height="300px"
          />
        </div>
      </div>

      <div class="chart-card">
        <div class="chart-header">
          <h3>事件级别分布</h3>
        </div>
        <div class="chart-content">
          <chart-card
            :option="levelChartOption"
            height="300px"
          />
        </div>
      </div>
    </div>

    <!-- 详细统计表格 -->
    <div class="table-container">
      <h3>安全事件统计详情</h3>
      <data-table
        :data="statisticsData"
        :loading="loading"
        :show-pagination="false"
      >
        <el-table-column prop="date" label="日期" width="120" />
        <el-table-column prop="totalCount" label="事件总数" width="100" align="center" />
        <el-table-column prop="minorCount" label="轻微事件" width="100" align="center" />
        <el-table-column prop="generalCount" label="一般事件" width="100" align="center" />
        <el-table-column prop="seriousCount" label="严重事件" width="100" align="center" />
        <el-table-column prop="majorCount" label="重大事件" width="100" align="center" />
        <el-table-column prop="processedCount" label="已处理" width="100" align="center" />
        <el-table-column prop="pendingCount" label="待处理" width="100" align="center" />
      </data-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { safetyIncidentService } from '@/api/services'
import PageHeader from '@/components/PageHeader.vue'
import StatCard from '@/components/StatCard.vue'
import ChartCard from '@/components/ChartCard.vue'
import DataTable from '@/components/DataTable.vue'

const loading = ref(false)
const overviewStats = ref({
  monthlyTotal: 0,
  pendingCount: 0,
  seriousCount: 0,
  safetyScore: 0
})

const statisticsData = ref<any[]>([])
const trendChartOption = ref<any>({})
const levelChartOption = ref<any>({})

onMounted(() => {
  fetchOverviewStats()
  fetchStatisticsData()
  fetchChartData()
})

const fetchOverviewStats = async () => {
  try {
    const { data } = await safetyIncidentService.getOverviewStats()
    overviewStats.value = data
  } catch (error) {
    console.error('获取概览统计失败:', error)
  }
}

const fetchStatisticsData = async () => {
  loading.value = true
  try {
    const { data } = await safetyIncidentService.getStatistics()
    statisticsData.value = data
  } catch (error) {
    console.error('获取统计数据失败:', error)
  } finally {
    loading.value = false
  }
}

const fetchChartData = async () => {
  try {
    // 事件趋势图
    const { data: trendData } = await safetyIncidentService.getTrendData()
    trendChartOption.value = {
      title: { text: '安全事件趋势' },
      xAxis: {
        type: 'category',
        data: trendData.dates
      },
      yAxis: { type: 'value' },
      series: [{
        name: '事件数量',
        type: 'line',
        data: trendData.counts,
        smooth: true
      }]
    }

    // 级别分布图
    const { data: levelData } = await safetyIncidentService.getLevelDistribution()
    levelChartOption.value = {
      title: { text: '事件级别分布' },
      series: [{
        type: 'pie',
        radius: '60%',
        data: levelData
      }]
    }
  } catch (error) {
    console.error('获取图表数据失败:', error)
  }
}
</script>

<style lang="scss" scoped>
.safety-statistics-container {
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
  }
}
</style>