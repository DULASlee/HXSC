<template>
  <div class="government-integration-container">
    <page-header 
      title="政府推送" 
      subtitle="向政府监管平台推送数据和报告"
    >
      <template #actions>
        <el-button type="primary" @click="handleManualPush">
          <el-icon><upload /></el-icon>
          手动推送
        </el-button>
      </template>
    </page-header>

    <!-- 推送配置 -->
    <div class="config-section">
      <h3>推送配置</h3>
      <el-form :model="configForm" label-width="120px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="推送地址">
              <el-input v-model="configForm.pushUrl" placeholder="请输入政府平台推送地址" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="推送频率">
              <el-select v-model="configForm.frequency" placeholder="请选择推送频率">
                <el-option label="实时推送" value="realtime" />
                <el-option label="每小时" value="hourly" />
                <el-option label="每日" value="daily" />
                <el-option label="每周" value="weekly" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="推送内容">
          <el-checkbox-group v-model="configForm.pushContent">
            <el-checkbox label="personnel" value="personnel">人员信息</el-checkbox>
            <el-checkbox label="attendance" value="attendance">考勤数据</el-checkbox>
            <el-checkbox label="safety" value="safety">安全事件</el-checkbox>
            <el-checkbox label="device" value="device">设备状态</el-checkbox>
            <el-checkbox label="environment" value="environment">环境监测</el-checkbox>
          </el-checkbox-group>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="saveConfig">保存配置</el-button>
          <el-button type="success" @click="testConnection">测试连接</el-button>
        </el-form-item>
      </el-form>
    </div>

    <!-- 推送状态统计 -->
    <div class="statistics-cards">
      <stat-card
        title="今日推送次数"
        :value="pushStats.todayCount"
        suffix="次"
        color="#409eff"
        icon="Upload"
      />
      <stat-card
        title="推送成功率"
        :value="pushStats.successRate"
        suffix="%"
        color="#67c23a"
        icon="CircleCheck"
      />
      <stat-card
        title="失败推送"
        :value="pushStats.failedCount"
        suffix="次"
        color="#f56c6c"
        icon="CircleClose"
      />
      <stat-card
        title="待推送数据"
        :value="pushStats.pendingCount"
        suffix="条"
        color="#e6a23c"
        icon="Clock"
      />
    </div>

    <!-- 推送记录 -->
    <div class="push-records">
      <h3>推送记录</h3>
      <data-table
        :data="pushRecords"
        :loading="loading"
        :total="total"
        :page="pageInfo.pageIndex"
        :size="pageInfo.pageSize"
        @page-change="handlePageChange"
        @size-change="handleSizeChange"
      >
        <el-table-column prop="pushTime" label="推送时间" min-width="150">
          <template #default="{ row }">
            {{ formatDateTime(row.pushTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="dataType" label="数据类型" width="120">
          <template #default="{ row }">
            {{ getDataTypeName(row.dataType) }}
          </template>
        </el-table-column>
        <el-table-column prop="recordCount" label="记录数量" width="100" align="center" />
        <el-table-column prop="status" label="推送状态" width="100" align="center">
          <template #default="{ row }">
            <status-tag
              :status="row.status"
              :status-map="{
                'Success': { type: 'success', label: '成功' },
                'Failed': { type: 'danger', label: '失败' },
                'Pending': { type: 'warning', label: '推送中' }
              }"
            />
          </template>
        </el-table-column>
        <el-table-column prop="responseMessage" label="响应信息" min-width="200" show-overflow-tooltip />
        <el-table-column prop="duration" label="耗时" width="80" align="center">
          <template #default="{ row }">
            {{ row.duration }}ms
          </template>
        </el-table-column>
        
        <template #actions="{ row }">
          <el-button
            type="primary"
            link
            @click="handleRetry(row)"
            v-if="row.status === 'Failed'"
          >
            重试
          </el-button>
          <el-button
            type="primary"
            link
            @click="handleViewDetail(row)"
          >
            详情
          </el-button>
        </template>
      </data-table>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Upload } from '@element-plus/icons-vue'
import { integrationService } from '@/api/services'
import { formatDateTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import StatCard from '@/components/StatCard.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'

// 配置表单
const configForm = reactive({
  pushUrl: '',
  frequency: 'daily',
  pushContent: ['personnel', 'attendance', 'safety']
})

// 统计数据
const pushStats = ref({
  todayCount: 0,
  successRate: 0,
  failedCount: 0,
  pendingCount: 0
})

// 推送记录
const pushRecords = ref<any[]>([])
const loading = ref(false)
const total = ref(0)
const pageInfo = reactive({
  pageIndex: 1,
  pageSize: 10
})

onMounted(() => {
  fetchConfig()
  fetchPushStats()
  fetchPushRecords()
})

// 获取配置
const fetchConfig = async () => {
  try {
    const { data } = await integrationService.getGovernmentConfig()
    Object.assign(configForm, data)
  } catch (error) {
    console.error('获取配置失败:', error)
  }
}

// 获取统计数据
const fetchPushStats = async () => {
  try {
    const { data } = await integrationService.getPushStats()
    pushStats.value = data
  } catch (error) {
    console.error('获取统计数据失败:', error)
  }
}

// 获取推送记录
const fetchPushRecords = async () => {
  loading.value = true
  try {
    const params = {
      pageIndex: pageInfo.pageIndex,
      pageSize: pageInfo.pageSize
    }
    const { data } = await integrationService.getPushRecords(params)
    pushRecords.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取推送记录失败:', error)
  } finally {
    loading.value = false
  }
}

// 保存配置
const saveConfig = async () => {
  try {
    await integrationService.saveGovernmentConfig(configForm)
    ElMessage.success('配置保存成功')
  } catch (error) {
    console.error('保存配置失败:', error)
    ElMessage.error('保存配置失败')
  }
}

// 测试连接
const testConnection = async () => {
  try {
    const { data } = await integrationService.testGovernmentConnection(configForm.pushUrl)
    if (data.success) {
      ElMessage.success('连接测试成功')
    } else {
      ElMessage.error(`连接测试失败: ${data.message}`)
    }
  } catch (error) {
    console.error('连接测试失败:', error)
    ElMessage.error('连接测试失败')
  }
}

// 手动推送
const handleManualPush = async () => {
  try {
    const { data } = await integrationService.manualPush(configForm.pushContent)
    ElMessage.success(`推送完成，共推送 ${data.count} 条数据`)
    fetchPushStats()
    fetchPushRecords()
  } catch (error) {
    console.error('手动推送失败:', error)
    ElMessage.error('手动推送失败')
  }
}

// 重试推送
const handleRetry = async (row: any) => {
  try {
    await integrationService.retryPush(row.id)
    ElMessage.success('重新推送成功')
    fetchPushRecords()
  } catch (error) {
    console.error('重试推送失败:', error)
    ElMessage.error('重试推送失败')
  }
}

// 查看详情
const handleViewDetail = (row: any) => {
  ElMessage.info('查看详情功能开发中...')
}

// 分页
const handlePageChange = (page: number) => {
  pageInfo.pageIndex = page
  fetchPushRecords()
}

const handleSizeChange = (size: number) => {
  pageInfo.pageSize = size
  pageInfo.pageIndex = 1
  fetchPushRecords()
}

// 获取数据类型名称
const getDataTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'personnel': '人员信息',
    'attendance': '考勤数据',
    'safety': '安全事件',
    'device': '设备状态',
    'environment': '环境监测'
  }
  return typeMap[type] || type
}
</script>

<style lang="scss" scoped>
.government-integration-container {
  padding: 20px;
}

.config-section {
  background: white;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  margin-bottom: 20px;

  h3 {
    margin-bottom: 20px;
    font-size: 16px;
    font-weight: 500;
  }
}

.statistics-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
  margin-bottom: 20px;
}

.push-records {
  h3 {
    margin-bottom: 16px;
    font-size: 16px;
    font-weight: 500;
  }
}
</style>