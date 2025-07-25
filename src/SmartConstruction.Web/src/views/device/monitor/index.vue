<template>
  <div class="device-monitor-container">
    <page-header 
      title="设备监控" 
      subtitle="实时监控设备运行状态和数据"
    />

    <div class="monitor-content">
      <!-- 设备状态卡片 -->
      <div class="status-cards">
        <stat-card
          title="在线设备"
          :value="monitorStats.onlineCount"
          suffix="台"
          color="#67c23a"
          icon="Monitor"
        />
        <stat-card
          title="离线设备"
          :value="monitorStats.offlineCount"
          suffix="台"
          color="#f56c6c"
          icon="Monitor"
        />
        <stat-card
          title="故障设备"
          :value="monitorStats.faultCount"
          suffix="台"
          color="#e6a23c"
          icon="Warning"
        />
        <stat-card
          title="维护中设备"
          :value="monitorStats.maintenanceCount"
          suffix="台"
          color="#409eff"
          icon="Setting"
        />
      </div>

      <!-- 实时数据表格 -->
      <div class="monitor-table">
        <h3>设备实时状态</h3>
        <data-table
          :data="monitorData"
          :loading="loading"
          :show-pagination="false"
          height="400px"
        >
          <el-table-column prop="name" label="设备名称" min-width="150" />
          <el-table-column prop="type" label="设备类型" width="120" />
          <el-table-column prop="status" label="状态" width="100">
            <template #default="{ row }">
              <status-tag
                :status="row.status"
                :status-map="{
                  'Online': { type: 'success', label: '在线' },
                  'Offline': { type: 'danger', label: '离线' },
                  'Fault': { type: 'danger', label: '故障' }
                }"
              />
            </template>
          </el-table-column>
          <el-table-column prop="lastData" label="最新数据" min-width="200" />
          <el-table-column prop="lastUpdateTime" label="更新时间" min-width="150">
            <template #default="{ row }">
              {{ formatDateTime(row.lastUpdateTime) }}
            </template>
          </el-table-column>
        </data-table>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { deviceService } from '@/api/services'
import { formatDateTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import StatCard from '@/components/StatCard.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'

const loading = ref(false)
const monitorStats = ref({
  onlineCount: 0,
  offlineCount: 0,
  faultCount: 0,
  maintenanceCount: 0
})
const monitorData = ref<any[]>([])
let refreshTimer: any = null

onMounted(() => {
  fetchMonitorData()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})

const fetchMonitorData = async () => {
  loading.value = true
  try {
    const { data } = await deviceService.getMonitorData()
    monitorStats.value = data.stats
    monitorData.value = data.devices
  } catch (error) {
    console.error('获取监控数据失败:', error)
  } finally {
    loading.value = false
  }
}

const startAutoRefresh = () => {
  refreshTimer = setInterval(fetchMonitorData, 30000) // 30秒刷新一次
}

const stopAutoRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
}
</script>

<style lang="scss" scoped>
.device-monitor-container {
  padding: 20px;
}

.monitor-content {
  margin-top: 20px;
}

.status-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.monitor-table {
  h3 {
    margin-bottom: 16px;
    font-size: 16px;
    font-weight: 500;
    color: #303133;
  }
}
</style>