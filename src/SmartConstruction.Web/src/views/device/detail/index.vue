<template>
  <div class="device-detail-container" v-loading="loading">
    <page-header 
      :title="`设备详情 - ${deviceData.name}`" 
      :back-path="'/device/list'"
    >
      <template #actions>
        <el-button 
          type="primary" 
          @click="handleEdit" 
          v-permission="'device.edit'"
        >
          编辑
        </el-button>
        <el-button 
          type="success" 
          @click="handleMonitor" 
          v-permission="'device.monitor'"
        >
          实时监控
        </el-button>
      </template>
    </page-header>

    <div class="content-container">
      <!-- 设备基本信息 -->
      <detail-card title="基本信息" icon="Monitor">
        <div class="detail-grid">
          <div class="detail-item">
            <label>设备编号：</label>
            <span>{{ deviceData.code }}</span>
          </div>
          <div class="detail-item">
            <label>设备名称：</label>
            <span>{{ deviceData.name }}</span>
          </div>
          <div class="detail-item">
            <label>设备类型：</label>
            <span>{{ getDeviceTypeName(deviceData.type) }}</span>
          </div>
          <div class="detail-item">
            <label>设备状态：</label>
            <status-tag
              :status="deviceData.status"
              :status-map="{
                'Online': { type: 'success', label: '在线' },
                'Offline': { type: 'danger', label: '离线' },
                'Maintenance': { type: 'warning', label: '维护中' },
                'Fault': { type: 'danger', label: '故障' }
              }"
            />
          </div>
        </div>
      </detail-card>

      <!-- 设备参数 -->
      <detail-card title="设备参数" icon="Setting">
        <div class="detail-grid">
          <div class="detail-item">
            <label>制造商：</label>
            <span>{{ deviceData.manufacturer || '未设置' }}</span>
          </div>
          <div class="detail-item">
            <label>设备型号：</label>
            <span>{{ deviceData.model || '未设置' }}</span>
          </div>
          <div class="detail-item">
            <label>序列号：</label>
            <span>{{ deviceData.serialNumber || '未设置' }}</span>
          </div>
          <div class="detail-item">
            <label>IP地址：</label>
            <span>{{ deviceData.ipAddress || '未设置' }}</span>
          </div>
        </div>
      </detail-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { deviceService } from '@/api/services'
import PageHeader from '@/components/PageHeader.vue'
import DetailCard from '@/components/DetailCard.vue'
import StatusTag from '@/components/StatusTag.vue'

const route = useRoute()
const router = useRouter()
const deviceId = route.params.id as string

const loading = ref(false)
const deviceData = ref<any>({})

onMounted(() => {
  fetchDeviceDetail()
})

const fetchDeviceDetail = async () => {
  loading.value = true
  try {
    const { data } = await deviceService.getById(deviceId)
    deviceData.value = data
  } catch (error) {
    console.error('获取设备详情失败:', error)
    ElMessage.error('获取设备详情失败')
  } finally {
    loading.value = false
  }
}

const handleEdit = () => {
  router.push(`/device/list?edit=${deviceId}`)
}

const handleMonitor = () => {
  router.push(`/device/monitor?deviceId=${deviceId}`)
}

const getDeviceTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'TowerCrane': '塔吊',
    'Elevator': '升降机',
    'Camera': '监控摄像头',
    'Other': '其他'
  }
  return typeMap[type] || type
}
</script>

<style lang="scss" scoped>
.device-detail-container {
  padding: 20px;
}

.content-container {
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.detail-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 16px;
}

.detail-item {
  display: flex;
  align-items: center;
  
  label {
    font-weight: 500;
    color: #606266;
    min-width: 100px;
    margin-right: 12px;
  }
  
  span {
    color: #303133;
  }
}
</style>