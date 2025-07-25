<template>
  <div class="iot-integration-container">
    <page-header 
      title="IoT集成" 
      subtitle="物联网设备数据集成和管理"
    />

    <!-- IoT设备状态 -->
    <div class="device-status-cards">
      <stat-card
        title="在线设备"
        :value="deviceStats.onlineCount"
        suffix="台"
        color="#67c23a"
        icon="Monitor"
      />
      <stat-card
        title="离线设备"
        :value="deviceStats.offlineCount"
        suffix="台"
        color="#f56c6c"
        icon="Monitor"
      />
      <stat-card
        title="数据接收量"
        :value="deviceStats.dataReceived"
        suffix="条"
        color="#409eff"
        icon="DataAnalysis"
      />
      <stat-card
        title="异常设备"
        :value="deviceStats.faultCount"
        suffix="台"
        color="#e6a23c"
        icon="Warning"
      />
    </div>

    <!-- 实时数据监控 -->
    <div class="realtime-monitor">
      <h3>实时数据监控</h3>
      <div class="monitor-grid">
        <!-- 环境数据 -->
        <div class="monitor-card">
          <h4>环境监测</h4>
          <div class="data-items">
            <div class="data-item">
              <span class="label">温度</span>
              <span class="value">{{ environmentData.temperature }}°C</span>
            </div>
            <div class="data-item">
              <span class="label">湿度</span>
              <span class="value">{{ environmentData.humidity }}%</span>
            </div>
            <div class="data-item">
              <span class="label">PM2.5</span>
              <span class="value" :class="getPm25Status(environmentData.pm25)">{{ environmentData.pm25 }}μg/m³</span>
            </div>
            <div class="data-item">
              <span class="label">噪音</span>
              <span class="value" :class="getNoiseStatus(environmentData.noise)">{{ environmentData.noise }}dB</span>
            </div>
          </div>
        </div>

        <!-- 设备运行状态 -->
        <div class="monitor-card">
          <h4>塔吊运行状态</h4>
          <div class="data-items">
            <div class="data-item">
              <span class="label">运行状态</span>
              <span class="value" :class="craneData.status === 'Running' ? 'status-running' : 'status-stopped'">
                {{ craneData.status === 'Running' ? '运行中' : '已停止' }}
              </span>
            </div>
            <div class="data-item">
              <span class="label">吊重</span>
              <span class="value">{{ craneData.load }}kg</span>
            </div>
            <div class="data-item">
              <span class="label">风速</span>
              <span class="value">{{ craneData.windSpeed }}m/s</span>
            </div>
            <div class="data-item">
              <span class="label">幅度</span>
              <span class="value">{{ craneData.amplitude }}m</span>
            </div>
          </div>
        </div>

        <!-- 人员检测 -->
        <div class="monitor-card">
          <h4>人员检测</h4>
          <div class="data-items">
            <div class="data-item">
              <span class="label">在场人员</span>
              <span class="value">{{ personnelData.onSiteCount }}人</span>
            </div>
            <div class="data-item">
              <span class="label">安全帽佩戴率</span>
              <span class="value" :class="getHelmetStatus(personnelData.helmetRate)">{{ personnelData.helmetRate }}%</span>
            </div>
            <div class="data-item">
              <span class="label">危险区域人员</span>
              <span class="value warning">{{ personnelData.dangerZoneCount }}人</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- MQTT连接状态 -->
    <div class="mqtt-status">
      <h3>MQTT连接状态</h3>
      <div class="connection-info">
        <div class="info-item">
          <span class="label">连接状态：</span>
          <status-tag
            :status="mqttStatus.connected"
            :status-map="{
              true: { type: 'success', label: '已连接' },
              false: { type: 'danger', label: '未连接' }
            }"
          />
        </div>
        <div class="info-item">
          <span class="label">服务器地址：</span>
          <span>{{ mqttStatus.brokerUrl }}</span>
        </div>
        <div class="info-item">
          <span class="label">订阅主题：</span>
          <span>{{ mqttStatus.subscribedTopics.join(', ') }}</span>
        </div>
        <div class="info-item">
          <span class="label">最后消息时间：</span>
          <span>{{ formatDateTime(mqttStatus.lastMessageTime) }}</span>
        </div>
      </div>
    </div>

    <!-- 数据接收日志 -->
    <div class="data-log">
      <h3>数据接收日志</h3>
      <data-table
        :data="dataLog"
        :loading="loading"
        :show-pagination="false"
        height="400px"
      >
        <el-table-column prop="timestamp" label="时间" width="150">
          <template #default="{ row }">
            {{ formatTime(row.timestamp) }}
          </template>
        </el-table-column>
        <el-table-column prop="deviceId" label="设备ID" width="120" />
        <el-table-column prop="dataType" label="数据类型" width="120" />
        <el-table-column prop="topic" label="MQTT主题" min-width="200" />
        <el-table-column prop="data" label="数据内容" min-width="300" show-overflow-tooltip />
        <el-table-column prop="status" label="处理状态" width="100">
          <template #default="{ row }">
            <status-tag
              :status="row.status"
              :status-map="{
                'Success': { type: 'success', label: '成功' },
                'Error': { type: 'danger', label: '错误' }
              }"
            />
          </template>
        </el-table-column>
      </data-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { integrationService } from '@/api/services'
import { formatDateTime, formatTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import StatCard from '@/components/StatCard.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'

// 设备统计
const deviceStats = ref({
  onlineCount: 0,
  offlineCount: 0,
  dataReceived: 0,
  faultCount: 0
})

// 实时环境数据
const environmentData = ref({
  temperature: 0,
  humidity: 0,
  pm25: 0,
  noise: 0
})

// 塔吊数据
const craneData = ref({
  status: 'Stopped',
  load: 0,
  windSpeed: 0,
  amplitude: 0
})

// 人员数据
const personnelData = ref({
  onSiteCount: 0,
  helmetRate: 0,
  dangerZoneCount: 0
})

// MQTT状态
const mqttStatus = ref({
  connected: false,
  brokerUrl: '',
  subscribedTopics: [],
  lastMessageTime: ''
})

// 数据日志
const dataLog = ref<any[]>([])
const loading = ref(false)

let refreshTimer: any = null

onMounted(() => {
  fetchDeviceStats()
  fetchRealtimeData()
  fetchMqttStatus()
  fetchDataLog()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})

// 获取设备统计
const fetchDeviceStats = async () => {
  try {
    const { data } = await integrationService.getIoTStats()
    deviceStats.value = data
  } catch (error) {
    console.error('获取设备统计失败:', error)
  }
}

// 获取实时数据
const fetchRealtimeData = async () => {
  try {
    const { data } = await integrationService.getRealtimeData()
    environmentData.value = data.environment
    craneData.value = data.crane
    personnelData.value = data.personnel
  } catch (error) {
    console.error('获取实时数据失败:', error)
  }
}

// 获取MQTT状态
const fetchMqttStatus = async () => {
  try {
    const { data } = await integrationService.getMqttStatus()
    mqttStatus.value = data
  } catch (error) {
    console.error('获取MQTT状态失败:', error)
  }
}

// 获取数据日志
const fetchDataLog = async () => {
  loading.value = true
  try {
    const { data } = await integrationService.getDataLog()
    dataLog.value = data
  } catch (error) {
    console.error('获取数据日志失败:', error)
  } finally {
    loading.value = false
  }
}

// 开始自动刷新
const startAutoRefresh = () => {
  refreshTimer = setInterval(() => {
    fetchRealtimeData()
    fetchDataLog()
  }, 5000) // 5秒刷新一次
}

// 停止自动刷新
const stopAutoRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
}

// 获取PM2.5状态样式
const getPm25Status = (value: number) => {
  if (value <= 35) return 'status-good'
  if (value <= 75) return 'status-moderate'
  return 'status-unhealthy'
}

// 获取噪音状态样式
const getNoiseStatus = (value: number) => {
  if (value <= 55) return 'status-good'
  if (value <= 70) return 'status-moderate'
  return 'status-unhealthy'
}

// 获取安全帽佩戴率状态样式
const getHelmetStatus = (rate: number) => {
  if (rate >= 95) return 'status-good'
  if (rate >= 80) return 'status-moderate'
  return 'status-unhealthy'
}
</script>

<style lang="scss" scoped>
.iot-integration-container {
  padding: 20px;
}

.device-status-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 30px;
}

.realtime-monitor {
  margin-bottom: 30px;

  h3 {
    margin-bottom: 16px;
    font-size: 16px;
    font-weight: 500;
  }
}

.monitor-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.monitor-card {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);

  h4 {
    margin: 0 0 16px 0;
    font-size: 14px;
    font-weight: 500;
    color: #303133;
  }
}

.data-items {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.data-item {
  display: flex;
  justify-content: space-between;
  align-items: center;

  .label {
    color: #606266;
    font-size: 13px;
  }

  .value {
    font-weight: 500;
    font-size: 14px;
    
    &.status-good {
      color: #67c23a;
    }
    
    &.status-moderate {
      color: #e6a23c;
    }
    
    &.status-unhealthy {
      color: #f56c6c;
    }
    
    &.status-running {
      color: #67c23a;
    }
    
    &.status-stopped {
      color: #909399;
    }
    
    &.warning {
      color: #f56c6c;
      font-weight: 600;
    }
  }
}

.mqtt-status {
  background: white;
  border-radius: 8px;
  padding: 20px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  margin-bottom: 30px;

  h3 {
    margin: 0 0 16px 0;
    font-size: 16px;
    font-weight: 500;
  }
}

.connection-info {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.info-item {
  display: flex;
  align-items: center;
  gap: 8px;

  .label {
    font-weight: 500;
    color: #606266;
    min-width: 80px;
  }
}

.data-log {
  h3 {
    margin-bottom: 16px;
    font-size: 16px;
    font-weight: 500;
  }
}
</style>