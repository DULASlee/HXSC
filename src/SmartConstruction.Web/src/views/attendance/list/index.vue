<template>
  <div class="attendance-list-container">
    <page-header title="考勤记录" subtitle="查看和管理工人考勤记录">
      <template #actions>
        <el-button type="primary" @click="handleExport" v-permission="'attendance.export'">
          <el-icon><download /></el-icon>
          导出记录
        </el-button>
        <el-button type="success" @click="handleImport" v-permission="'attendance.import'">
          <el-icon><upload /></el-icon>
          批量导入
        </el-button>
      </template>
    </page-header>
    
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="工人姓名" prop="keyword">
        <el-input
          v-model="searchParams.keyword"
          placeholder="请输入工人姓名或工号"
          clearable
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
      
      <el-form-item label="考勤类型" prop="type">
        <el-select
          v-model="searchParams.type"
          placeholder="请选择考勤类型"
          clearable
        >
          <el-option
            v-for="item in attendanceTypeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="考勤日期" prop="dateRange">
        <el-date-picker
          v-model="dateRange"
          type="daterange"
          range-separator="至"
          start-placeholder="开始日期"
          end-placeholder="结束日期"
          format="YYYY-MM-DD"
          value-format="YYYY-MM-DD"
          @change="handleDateRangeChange"
        />
      </el-form-item>
    </search-form>
    
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
      <el-table-column prop="attendanceDate" label="考勤日期" width="120">
        <template #default="{ row }">
          {{ formatDate(row.attendanceDate) }}
        </template>
      </el-table-column>
      <el-table-column prop="workerName" label="工人姓名" min-width="100" />
      <el-table-column prop="workerEmployeeNo" label="工号" min-width="120" />
      <el-table-column prop="companyName" label="所属公司" min-width="150" show-overflow-tooltip />
      <el-table-column prop="teamName" label="所属班组" min-width="120" />
      <el-table-column prop="type" label="考勤类型" width="100">
        <template #default="{ row }">
          <el-tag :type="getAttendanceTypeColor(row.type)">
            {{ getAttendanceTypeName(row.type) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="checkInTime" label="签到时间" min-width="120">
        <template #default="{ row }">
          {{ row.checkInTime ? formatTime(row.checkInTime) : '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="checkOutTime" label="签退时间" min-width="120">
        <template #default="{ row }">
          {{ row.checkOutTime ? formatTime(row.checkOutTime) : '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="workHours" label="工作时长" width="100">
        <template #default="{ row }">
          {{ row.workHours ? `${row.workHours.toFixed(1)}h` : '-' }}
        </template>
      </el-table-column>
      <el-table-column prop="overtimeHours" label="加班时长" width="100">
        <template #default="{ row }">
          {{ row.overtimeHours ? `${row.overtimeHours.toFixed(1)}h` : '0h' }}
        </template>
      </el-table-column>
      <el-table-column prop="status" label="考勤状态" width="100">
        <template #default="{ row }">
          <status-tag
            :status="row.status"
            :status-map="{
              'Normal': { type: 'success', label: '正常' },
              'Late': { type: 'warning', label: '迟到' },
              'EarlyLeave': { type: 'warning', label: '早退' },
              'Absent': { type: 'danger', label: '缺勤' },
              'Leave': { type: 'info', label: '请假' }
            }"
          />
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="备注" min-width="150" show-overflow-tooltip />
      <el-table-column prop="createdTime" label="记录时间" min-width="150">
        <template #default="{ row }">
          {{ formatDateTime(row.createdTime) }}
        </template>
      </el-table-column>
      
      <template #actions="{ row }">
        <el-button
          type="primary"
          link
          @click="handleView(row)"
          v-permission="'attendance.view'"
        >
          查看
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleEdit(row)"
          v-permission="'attendance.edit'"
        >
          编辑
        </el-button>
      </template>
    </data-table>

    <!-- 考勤记录表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      :title="dialogTitle"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="600px"
      @submit="handleSubmit"
    >
      <el-form-item label="工人" prop="workerId">
        <el-select
          v-model="formData.workerId"
          placeholder="请选择工人"
          filterable
          :disabled="formType === 'edit'"
        >
          <el-option
            v-for="item in workerOptions"
            :key="item.value"
            :label="`${item.label} (${item.employeeNo})`"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="考勤日期" prop="attendanceDate">
        <el-date-picker
          v-model="formData.attendanceDate"
          type="date"
          placeholder="请选择考勤日期"
          format="YYYY-MM-DD"
          value-format="YYYY-MM-DD"
          style="width: 100%"
        />
      </el-form-item>
      
      <el-form-item label="考勤类型" prop="type">
        <el-select
          v-model="formData.type"
          placeholder="请选择考勤类型"
        >
          <el-option
            v-for="item in attendanceTypeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="签到时间" prop="checkInTime">
            <el-time-picker
              v-model="formData.checkInTime"
              placeholder="请选择签到时间"
              format="HH:mm:ss"
              value-format="HH:mm:ss"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="签退时间" prop="checkOutTime">
            <el-time-picker
              v-model="formData.checkOutTime"
              placeholder="请选择签退时间"
              format="HH:mm:ss"
              value-format="HH:mm:ss"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="考勤状态" prop="status">
        <el-select
          v-model="formData.status"
          placeholder="请选择考勤状态"
        >
          <el-option label="正常" value="Normal" />
          <el-option label="迟到" value="Late" />
          <el-option label="早退" value="EarlyLeave" />
          <el-option label="缺勤" value="Absent" />
          <el-option label="请假" value="Leave" />
        </el-select>
      </el-form-item>
      
      <el-form-item label="备注" prop="notes">
        <el-input
          v-model="formData.notes"
          type="textarea"
          :rows="3"
          placeholder="请输入备注信息"
        />
      </el-form-item>
    </form-dialog>

    <!-- 批量导入对话框 -->
    <el-dialog
      v-model="importDialogVisible"
      title="批量导入考勤记录"
      width="500px"
    >
      <el-upload
        class="upload-area"
        drag
        action="#"
        :before-upload="handleBeforeUpload"
        :show-file-list="false"
        accept=".xlsx,.xls"
      >
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">
          将文件拖到此处，或<em>点击上传</em>
        </div>
        <template #tip>
          <div class="el-upload__tip">
            只能上传xlsx/xls文件，且不超过10MB
            <el-link type="primary" @click="downloadTemplate">下载模板</el-link>
          </div>
        </template>
      </el-upload>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Download, Upload, UploadFilled } from '@element-plus/icons-vue'
import { attendanceService, companyService, teamService, workerService } from '@/api/services'
import { formatDate, formatDateTime, formatTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import SearchForm from '@/components/SearchForm.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'

const route = useRoute()
const router = useRouter()

// 搜索参数
const searchParams = reactive({
  keyword: '',
  companyId: route.query.companyId as string || undefined,
  teamId: route.query.teamId as string || undefined,
  type: undefined,
  startDate: '',
  endDate: '',
  pageIndex: 1,
  pageSize: 10
})

// 日期范围
const dateRange = ref<[string, string] | null>(null)

// 表格数据
const tableData = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 选项数据
const companyOptions = ref<{ label: string; value: string }[]>([])
const teamOptions = ref<{ label: string; value: string; companyId: string }[]>([])
const workerOptions = ref<{ label: string; value: string; employeeNo: string }[]>([])

// 考勤类型选项
const attendanceTypeOptions = [
  { label: '正常考勤', value: 'Normal' },
  { label: '加班', value: 'Overtime' },
  { label: '请假', value: 'Leave' },
  { label: '出差', value: 'BusinessTrip' }
]

// 表单相关
const dialogVisible = ref(false)
const formType = ref<'create' | 'edit'>('create')
const formLoading = ref(false)
const formData = reactive({
  id: '',
  workerId: '',
  attendanceDate: '',
  type: 'Normal',
  checkInTime: '',
  checkOutTime: '',
  status: 'Normal',
  notes: ''
})

// 导入对话框
const importDialogVisible = ref(false)

// 表单校验规则
const formRules = {
  workerId: [
    { required: true, message: '请选择工人', trigger: 'change' }
  ],
  attendanceDate: [
    { required: true, message: '请选择考勤日期', trigger: 'change' }
  ],
  type: [
    { required: true, message: '请选择考勤类型', trigger: 'change' }
  ]
}

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增考勤记录' : '编辑考勤记录'
})

// 过滤后的班组选项
const filteredTeamOptions = computed(() => {
  if (!searchParams.companyId) return teamOptions.value
  return teamOptions.value.filter(team => team.companyId === searchParams.companyId)
})

// 组件引用
const searchFormRef = ref()
const tableRef = ref()

// 初始化
onMounted(() => {
  fetchCompanies()
  fetchTeams()
  fetchWorkers()
  fetchData()
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

// 获取工人列表
const fetchWorkers = async () => {
  try {
    const { data } = await workerService.getAll()
    workerOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id,
      employeeNo: item.employeeNo
    }))
  } catch (error) {
    console.error('获取工人列表失败:', error)
  }
}

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await attendanceService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取考勤记录失败:', error)
    ElMessage.error('获取考勤记录失败')
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = (values: any) => {
  searchParams.pageIndex = 1
  Object.assign(searchParams, values)
  fetchData()
}

// 重置
const handleReset = () => {
  searchParams.pageIndex = 1
  dateRange.value = null
  searchParams.startDate = ''
  searchParams.endDate = ''
  fetchData()
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
  searchParams.teamId = undefined
}

// 日期范围变化
const handleDateRangeChange = (dates: [string, string] | null) => {
  if (dates) {
    searchParams.startDate = dates[0]
    searchParams.endDate = dates[1]
  } else {
    searchParams.startDate = ''
    searchParams.endDate = ''
  }
}

// 查看
const handleView = (row: any) => {
  // 显示考勤记录详情
  ElMessage.info('功能开发中...')
}

// 编辑
const handleEdit = (row: any) => {
  formType.value = 'edit'
  resetForm()
  Object.assign(formData, row)
  dialogVisible.value = true
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    if (formType.value === 'create') {
      const { data } = await attendanceService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      const { data } = await attendanceService.update(values.id, values)
      if (data) {
        ElMessage.success('更新成功')
        dialogVisible.value = false
        fetchData()
      }
    }
  } catch (error) {
    console.error('操作失败:', error)
    ElMessage.error('操作失败')
  } finally {
    formLoading.value = false
  }
}

// 导出
const handleExport = async () => {
  try {
    const { data } = await attendanceService.export(searchParams)
    // 处理文件下载
    const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `考勤记录_${formatDate(new Date())}.xlsx`
    link.click()
    window.URL.revokeObjectURL(url)
    ElMessage.success('导出成功')
  } catch (error) {
    console.error('导出失败:', error)
    ElMessage.error('导出失败')
  }
}

// 导入
const handleImport = () => {
  importDialogVisible.value = true
}

// 上传前处理
const handleBeforeUpload = (file: File) => {
  // 处理文件上传
  ElMessage.info('批量导入功能开发中...')
  return false
}

// 下载模板
const downloadTemplate = async () => {
  try {
    const { data } = await attendanceService.downloadTemplate()
    const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = '考勤记录导入模板.xlsx'
    link.click()
    window.URL.revokeObjectURL(url)
  } catch (error) {
    console.error('下载模板失败:', error)
    ElMessage.error('下载模板失败')
  }
}

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    id: '',
    workerId: '',
    attendanceDate: '',
    type: 'Normal',
    checkInTime: '',
    checkOutTime: '',
    status: 'Normal',
    notes: ''
  })
}

// 获取考勤类型名称
const getAttendanceTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'Normal': '正常考勤',
    'Overtime': '加班',
    'Leave': '请假',
    'BusinessTrip': '出差'
  }
  return typeMap[type] || type
}

// 获取考勤类型颜色
const getAttendanceTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    'Normal': '',
    'Overtime': 'warning',
    'Leave': 'info',
    'BusinessTrip': 'success'
  }
  return colorMap[type] || ''
}
</script>

<style lang="scss" scoped>
.attendance-list-container {
  padding: 20px;
}

.upload-area {
  margin: 20px 0;
}
</style>