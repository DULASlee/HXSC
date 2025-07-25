<template>
  <div class="worker-detail-container" v-loading="loading">
    <page-header 
      :title="`工人详情 - ${workerData.name}`" 
      :back-path="'/worker/list'"
    >
      <template #actions>
        <el-button 
          type="primary" 
          @click="handleEdit" 
          v-permission="'worker.edit'"
        >
          编辑
        </el-button>
        <el-button 
          :type="workerData.isActive ? 'warning' : 'success'" 
          @click="handleToggleStatus"
          v-permission="'worker.edit'"
        >
          {{ workerData.isActive ? '离岗' : '入岗' }}
        </el-button>
      </template>
    </page-header>

    <div class="content-container">
      <!-- 基本信息 -->
      <detail-card title="基本信息" icon="User">
        <div class="detail-grid">
          <div class="detail-item">
            <label>工号：</label>
            <span>{{ workerData.employeeNo }}</span>
          </div>
          <div class="detail-item">
            <label>姓名：</label>
            <span>{{ workerData.name }}</span>
          </div>
          <div class="detail-item">
            <label>身份证号：</label>
            <span>{{ workerData.idCard }}</span>
          </div>
          <div class="detail-item">
            <label>联系电话：</label>
            <span>{{ workerData.phone }}</span>
          </div>
          <div class="detail-item">
            <label>性别：</label>
            <span>{{ workerData.gender === 'Male' ? '男' : '女' }}</span>
          </div>
          <div class="detail-item">
            <label>出生日期：</label>
            <span>{{ formatDate(workerData.dateOfBirth) }}</span>
          </div>
          <div class="detail-item">
            <label>年龄：</label>
            <span>{{ calculateAge(workerData.dateOfBirth) }}岁</span>
          </div>
          <div class="detail-item">
            <label>工人类型：</label>
            <span>{{ getWorkerTypeName(workerData.type) }}</span>
          </div>
          <div class="detail-item">
            <label>状态：</label>
            <status-tag
              :status="workerData.isActive"
              :status-map="{
                true: { type: 'success', label: '在岗' },
                false: { type: 'danger', label: '离岗' }
              }"
            />
          </div>
          <div class="detail-item">
            <label>入职时间：</label>
            <span>{{ formatDateTime(workerData.createdTime) }}</span>
          </div>
          <div class="detail-item full-width">
            <label>家庭住址：</label>
            <span>{{ workerData.address || '未填写' }}</span>
          </div>
        </div>
      </detail-card>

      <!-- 工作信息 -->
      <detail-card title="工作信息" icon="OfficeBuilding">
        <div class="detail-grid">
          <div class="detail-item">
            <label>所属公司：</label>
            <span>{{ workerData.companyName }}</span>
          </div>
          <div class="detail-item">
            <label>所属班组：</label>
            <span>{{ workerData.teamName }}</span>
          </div>
          <div class="detail-item">
            <label>班组长：</label>
            <span>{{ workerData.teamLeader || '未指定' }}</span>
          </div>
          <div class="detail-item">
            <label>工作项目：</label>
            <span>{{ workerData.projectNames?.join('、') || '暂无项目' }}</span>
          </div>
        </div>
      </detail-card>

      <!-- 考勤统计 -->
      <detail-card title="考勤统计" icon="Calendar">
        <div class="stats-container">
          <stat-card
            title="本月出勤天数"
            :value="attendanceStats.monthlyDays"
            suffix="天"
            color="#67c23a"
          />
          <stat-card
            title="本年出勤天数"
            :value="attendanceStats.yearlyDays"
            suffix="天"
            color="#409eff"
          />
          <stat-card
            title="迟到次数"
            :value="attendanceStats.lateCount"
            suffix="次"
            color="#e6a23c"
          />
          <stat-card
            title="早退次数"
            :value="attendanceStats.earlyLeaveCount"
            suffix="次"
            color="#f56c6c"
          />
        </div>
      </detail-card>

      <!-- 安全记录 -->
      <detail-card title="安全记录" icon="Warning">
        <div class="safety-records">
          <el-empty 
            v-if="safetyRecords.length === 0" 
            description="暂无安全记录"
          />
          <div v-else>
            <div 
              v-for="record in safetyRecords" 
              :key="record.id"
              class="safety-record-item"
            >
              <div class="record-header">
                <span class="record-type" :class="getRecordTypeClass(record.type)">
                  {{ getRecordTypeName(record.type) }}
                </span>
                <span class="record-time">{{ formatDateTime(record.createdTime) }}</span>
              </div>
              <div class="record-content">{{ record.description }}</div>
            </div>
          </div>
        </div>
      </detail-card>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { workerService, attendanceService } from '@/api/services'
import { formatDate, formatDateTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import DetailCard from '@/components/DetailCard.vue'
import StatusTag from '@/components/StatusTag.vue'
import StatCard from '@/components/StatCard.vue'

const route = useRoute()
const router = useRouter()
const workerId = route.params.id as string

// 数据
const loading = ref(false)
const workerData = ref<any>({})
const attendanceStats = ref({
  monthlyDays: 0,
  yearlyDays: 0,
  lateCount: 0,
  earlyLeaveCount: 0
})
const safetyRecords = ref<any[]>([])

// 初始化
onMounted(() => {
  fetchWorkerDetail()
  fetchAttendanceStats()
  fetchSafetyRecords()
})

// 获取工人详情
const fetchWorkerDetail = async () => {
  loading.value = true
  try {
    const { data } = await workerService.getById(workerId)
    workerData.value = data
  } catch (error) {
    console.error('获取工人详情失败:', error)
    ElMessage.error('获取工人详情失败')
  } finally {
    loading.value = false
  }
}

// 获取考勤统计
const fetchAttendanceStats = async () => {
  try {
    const { data } = await attendanceService.getWorkerStats(workerId)
    attendanceStats.value = data
  } catch (error) {
    console.error('获取考勤统计失败:', error)
  }
}

// 获取安全记录
const fetchSafetyRecords = async () => {
  try {
    const { data } = await workerService.getSafetyRecords(workerId)
    safetyRecords.value = data
  } catch (error) {
    console.error('获取安全记录失败:', error)
  }
}

// 编辑
const handleEdit = () => {
  router.push(`/worker/list?edit=${workerId}`)
}

// 切换状态
const handleToggleStatus = async () => {
  try {
    await ElMessageBox.confirm(
      `确定要${workerData.value.isActive ? '离岗' : '入岗'}该工人吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await workerService.updateStatus(workerId, !workerData.value.isActive)
    if (data) {
      ElMessage.success(`${workerData.value.isActive ? '离岗' : '入岗'}成功`)
      fetchWorkerDetail()
    }
  } catch (error) {
    console.error('操作失败:', error)
  }
}

// 计算年龄
const calculateAge = (dateOfBirth: string) => {
  if (!dateOfBirth) return 0
  const today = new Date()
  const birthDate = new Date(dateOfBirth)
  let age = today.getFullYear() - birthDate.getFullYear()
  const monthDiff = today.getMonth() - birthDate.getMonth()
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
    age--
  }
  
  return age
}

// 获取工人类型名称
const getWorkerTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'General': '普通工人',
    'Technical': '技术工人',
    'Management': '管理人员',
    'Safety': '安全员',
    'QualityControl': '质检员',
    'Other': '其他'
  }
  return typeMap[type] || type
}

// 获取记录类型名称
const getRecordTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'Training': '安全培训',
    'Incident': '安全事故',
    'Violation': '违规记录',
    'Achievement': '安全表彰'
  }
  return typeMap[type] || type
}

// 获取记录类型样式
const getRecordTypeClass = (type: string) => {
  const classMap: Record<string, string> = {
    'Training': 'success',
    'Incident': 'danger',
    'Violation': 'warning',
    'Achievement': 'primary'
  }
  return classMap[type] || 'info'
}
</script>

<style lang="scss" scoped>
.worker-detail-container {
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
  
  &.full-width {
    grid-column: 1 / -1;
    align-items: flex-start;
    flex-direction: column;
    gap: 8px;
  }
  
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

.stats-container {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 20px;
}

.safety-records {
  .safety-record-item {
    padding: 16px;
    border: 1px solid #ebeef5;
    border-radius: 8px;
    margin-bottom: 12px;
    
    &:last-child {
      margin-bottom: 0;
    }
  }
  
  .record-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 8px;
  }
  
  .record-type {
    padding: 4px 12px;
    border-radius: 12px;
    font-size: 12px;
    font-weight: 500;
    
    &.success {
      color: #67c23a;
      background-color: #f0f9ff;
    }
    
    &.danger {
      color: #f56c6c;
      background-color: #fef0f0;
    }
    
    &.warning {
      color: #e6a23c;
      background-color: #fdf6ec;
    }
    
    &.primary {
      color: #409eff;
      background-color: #ecf5ff;
    }
  }
  
  .record-time {
    color: #909399;
    font-size: 12px;
  }
  
  .record-content {
    color: #606266;
    line-height: 1.5;
  }
}
</style>