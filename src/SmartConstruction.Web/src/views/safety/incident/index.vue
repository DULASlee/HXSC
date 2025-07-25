<template>
  <div class="safety-incident-container">
    <page-header title="安全事件" subtitle="管理和记录安全事件信息">
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'safety.incident.create'">
          <el-icon><plus /></el-icon>
          新增事件
        </el-button>
      </template>
    </page-header>
    
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="事件标题" prop="keyword">
        <el-input
          v-model="searchParams.keyword"
          placeholder="请输入事件标题"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="事件级别" prop="level">
        <el-select
          v-model="searchParams.level"
          placeholder="请选择事件级别"
          clearable
        >
          <el-option
            v-for="item in incidentLevelOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="事件状态" prop="status">
        <el-select
          v-model="searchParams.status"
          placeholder="请选择事件状态"
          clearable
        >
          <el-option
            v-for="item in incidentStatusOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="发生时间" prop="dateRange">
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
      <el-table-column prop="title" label="事件标题" min-width="200" show-overflow-tooltip />
      <el-table-column prop="level" label="事件级别" width="100" align="center">
        <template #default="{ row }">
          <el-tag :type="getIncidentLevelColor(row.level)">
            {{ getIncidentLevelName(row.level) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="事件状态" width="100" align="center">
        <template #default="{ row }">
          <status-tag
            :status="row.status"
            :status-map="{
              'Pending': { type: 'warning', label: '待处理' },
              'Processing': { type: 'primary', label: '处理中' },
              'Completed': { type: 'success', label: '已完成' },
              'Closed': { type: 'info', label: '已关闭' }
            }"
          />
        </template>
      </el-table-column>
      <el-table-column prop="occurredTime" label="发生时间" min-width="150">
        <template #default="{ row }">
          {{ formatDateTime(row.occurredTime) }}
        </template>
      </el-table-column>
      <el-table-column prop="location" label="发生地点" min-width="150" show-overflow-tooltip />
      <el-table-column prop="reporterName" label="上报人" width="100" />
      <el-table-column prop="handlerName" label="处理人" width="100" />
      <el-table-column prop="createdTime" label="创建时间" min-width="150">
        <template #default="{ row }">
          {{ formatDateTime(row.createdTime) }}
        </template>
      </el-table-column>
      
      <template #actions="{ row }">
        <el-button
          type="primary"
          link
          @click="handleView(row)"
          v-permission="'safety.incident.view'"
        >
          查看
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleEdit(row)"
          v-permission="'safety.incident.edit'"
        >
          编辑
        </el-button>
        <el-button
          type="success"
          link
          @click="handleProcess(row)"
          v-permission="'safety.incident.process'"
          v-if="row.status === 'Pending'"
        >
          处理
        </el-button>
      </template>
    </data-table>
    
    <!-- 安全事件表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      :title="dialogTitle"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="800px"
      @submit="handleSubmit"
    >
      <el-form-item label="事件标题" prop="title">
        <el-input
          v-model="formData.title"
          placeholder="请输入事件标题"
        />
      </el-form-item>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="事件级别" prop="level">
            <el-select
              v-model="formData.level"
              placeholder="请选择事件级别"
            >
              <el-option
                v-for="item in incidentLevelOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="发生时间" prop="occurredTime">
            <el-date-picker
              v-model="formData.occurredTime"
              type="datetime"
              placeholder="请选择发生时间"
              format="YYYY-MM-DD HH:mm:ss"
              value-format="YYYY-MM-DD HH:mm:ss"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="发生地点" prop="location">
        <el-input
          v-model="formData.location"
          placeholder="请输入发生地点"
        />
      </el-form-item>
      
      <el-form-item label="事件描述" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="4"
          placeholder="请详细描述事件经过"
        />
      </el-form-item>
      
      <el-form-item label="涉及人员" prop="involvedPersonnel">
        <el-input
          v-model="formData.involvedPersonnel"
          placeholder="请输入涉及人员信息"
        />
      </el-form-item>
      
      <el-form-item label="损失情况" prop="damage">
        <el-input
          v-model="formData.damage"
          type="textarea"
          :rows="2"
          placeholder="请描述损失情况"
        />
      </el-form-item>
      
      <el-form-item label="初步原因" prop="preliminaryCause">
        <el-input
          v-model="formData.preliminaryCause"
          type="textarea"
          :rows="2"
          placeholder="请分析初步原因"
        />
      </el-form-item>
    </form-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { safetyIncidentService } from '@/api/services'
import { formatDateTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import SearchForm from '@/components/SearchForm.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'

// 搜索参数
const searchParams = reactive({
  keyword: '',
  level: undefined,
  status: undefined,
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

// 事件级别选项
const incidentLevelOptions = [
  { label: '轻微', value: 'Minor' },
  { label: '一般', value: 'General' },
  { label: '严重', value: 'Serious' },
  { label: '重大', value: 'Major' }
]

// 事件状态选项
const incidentStatusOptions = [
  { label: '待处理', value: 'Pending' },
  { label: '处理中', value: 'Processing' },
  { label: '已完成', value: 'Completed' },
  { label: '已关闭', value: 'Closed' }
]

// 表单相关
const dialogVisible = ref(false)
const formType = ref<'create' | 'edit'>('create')
const formLoading = ref(false)
const formData = reactive({
  id: '',
  title: '',
  level: '',
  occurredTime: '',
  location: '',
  description: '',
  involvedPersonnel: '',
  damage: '',
  preliminaryCause: ''
})

// 表单校验规则
const formRules = {
  title: [
    { required: true, message: '请输入事件标题', trigger: 'blur' }
  ],
  level: [
    { required: true, message: '请选择事件级别', trigger: 'change' }
  ],
  occurredTime: [
    { required: true, message: '请选择发生时间', trigger: 'change' }
  ],
  location: [
    { required: true, message: '请输入发生地点', trigger: 'blur' }
  ],
  description: [
    { required: true, message: '请输入事件描述', trigger: 'blur' }
  ]
}

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增安全事件' : '编辑安全事件'
})

// 组件引用
const searchFormRef = ref()
const tableRef = ref()

// 初始化
onMounted(() => {
  fetchData()
})

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await safetyIncidentService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取安全事件列表失败:', error)
    ElMessage.error('获取安全事件列表失败')
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

// 新增
const handleCreate = () => {
  formType.value = 'create'
  resetForm()
  dialogVisible.value = true
}

// 查看
const handleView = (row: any) => {
  ElMessage.info('查看详情功能开发中...')
}

// 编辑
const handleEdit = (row: any) => {
  formType.value = 'edit'
  resetForm()
  Object.assign(formData, row)
  dialogVisible.value = true
}

// 处理
const handleProcess = (row: any) => {
  ElMessage.info('事件处理功能开发中...')
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    if (formType.value === 'create') {
      const { data } = await safetyIncidentService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      const { data } = await safetyIncidentService.update(values.id, values)
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

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    id: '',
    title: '',
    level: '',
    occurredTime: '',
    location: '',
    description: '',
    involvedPersonnel: '',
    damage: '',
    preliminaryCause: ''
  })
}

// 获取事件级别名称
const getIncidentLevelName = (level: string) => {
  const levelMap: Record<string, string> = {
    'Minor': '轻微',
    'General': '一般',
    'Serious': '严重',
    'Major': '重大'
  }
  return levelMap[level] || level
}

// 获取事件级别颜色
const getIncidentLevelColor = (level: string) => {
  const colorMap: Record<string, string> = {
    'Minor': 'success',
    'General': '',
    'Serious': 'warning',
    'Major': 'danger'
  }
  return colorMap[level] || ''
}
</script>

<style lang="scss" scoped>
.safety-incident-container {
  padding: 20px;
}
</style>