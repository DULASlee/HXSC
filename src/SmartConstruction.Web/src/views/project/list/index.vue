<template>
  <div class="project-list-container">
    <page-header title="项目管理" subtitle="管理系统中的项目信息">
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'project.create'">
          <el-icon><plus /></el-icon>
          新增项目
        </el-button>
      </template>
    </page-header>
    
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="项目名称" prop="keyword">
        <el-input
          v-model="searchParams.keyword"
          placeholder="请输入项目名称或编码"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="所属公司" prop="companyId">
        <el-select
          v-model="searchParams.companyId"
          placeholder="请选择公司"
          clearable
          filterable
        >
          <el-option
            v-for="item in companyOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="项目状态" prop="status">
        <el-select
          v-model="searchParams.status"
          placeholder="请选择状态"
          clearable
        >
          <el-option
            v-for="item in projectStatusOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="开始日期" prop="startDateRange">
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
    
    <el-row :gutter="20">
      <el-col :xs="24" :sm="12" :md="8" :lg="6" v-for="item in tableData" :key="item.id">
        <el-card class="project-card" shadow="hover">
          <div class="project-header">
            <div class="project-title">
              <h3>{{ item.name }}</h3>
              <p>{{ item.code }}</p>
            </div>
            <status-tag
              :status="item.status"
              :status-map="{
                'Planning': { type: 'info', label: '规划中' },
                'InProgress': { type: 'primary', label: '进行中' },
                'Completed': { type: 'success', label: '已完成' },
                'OnHold': { type: 'warning', label: '已暂停' },
                'Cancelled': { type: 'danger', label: '已取消' }
              }"
            />
          </div>
          
          <div class="project-info">
            <p><el-icon><office-building /></el-icon> {{ item.companyName }}</p>
            <p><el-icon><location /></el-icon> {{ item.address }}</p>
            <p><el-icon><user /></el-icon> {{ item.projectManager }}</p>
            <p><el-icon><phone /></el-icon> {{ item.managerPhone }}</p>
            <p><el-icon><calendar /></el-icon> {{ formatDate(item.startDate) }} ~ {{ formatDate(item.endDate) }}</p>
          </div>
          
          <div class="project-progress">
            <div class="progress-header">
              <span>项目进度</span>
              <span>{{ item.progress }}%</span>
            </div>
            <el-progress
              :percentage="item.progress"
              :color="getProgressColor(item.progress)"
              :stroke-width="10"
            />
          </div>
          
          <div class="project-stats">
            <div class="stat-item">
              <el-icon><user /></el-icon>
              <span>{{ item.workerCount }}</span>
              <span>工人</span>
            </div>
            <div class="stat-item">
              <el-icon><monitor /></el-icon>
              <span>{{ item.deviceCount }}</span>
              <span>设备</span>
            </div>
            <div class="stat-item">
              <el-icon><money /></el-icon>
                              <span>{{ formatCurrency(item.totalInvestment) }}</span>
              <span>投资</span>
            </div>
          </div>
          
          <div class="project-actions">
            <el-button type="primary" @click="handleView(item)" v-permission="'project.view'">
              查看
            </el-button>
            <el-button type="primary" @click="handleEdit(item)" v-permission="'project.edit'">
              编辑
            </el-button>
            <el-dropdown @command="(command) => handleCommand(command, item)">
              <el-button type="primary">
                更多<el-icon class="el-icon--right"><arrow-down /></el-icon>
              </el-button>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item command="dashboard">仪表盘</el-dropdown-item>
                  <el-dropdown-item command="workers">工人管理</el-dropdown-item>
                  <el-dropdown-item command="devices">设备管理</el-dropdown-item>
                  <el-dropdown-item command="safety">安全管理</el-dropdown-item>
                  <el-dropdown-item command="delete" divided>删除</el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </el-card>
      </el-col>
    </el-row>
    
    <div class="pagination-container">
      <el-pagination
        v-model:current-page="searchParams.pageIndex"
        v-model:page-size="searchParams.pageSize"
        :page-sizes="[8, 16, 24, 32]"
        :total="total"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handlePageChange"
      />
    </div>
    
    <!-- 项目表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      :title="dialogTitle"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="700px"
      @submit="handleSubmit"
    >
      <el-form-item label="所属公司" prop="companyId">
        <el-select
          v-model="formData.companyId"
          placeholder="请选择公司"
          filterable
          :disabled="formType === 'edit'"
        >
          <el-option
            v-for="item in companyOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="项目编码" prop="code">
        <el-input
          v-model="formData.code"
          placeholder="请输入项目编码"
          :disabled="formType === 'edit'"
        />
      </el-form-item>
      
      <el-form-item label="项目名称" prop="name">
        <el-input
          v-model="formData.name"
          placeholder="请输入项目名称"
        />
      </el-form-item>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="项目经理" prop="projectManager">
            <el-input
              v-model="formData.projectManager"
              placeholder="请输入项目经理"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="联系电话" prop="managerPhone">
            <el-input
              v-model="formData.managerPhone"
              placeholder="请输入联系电话"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="项目地址" prop="address">
        <el-input
          v-model="formData.address"
          placeholder="请输入项目地址"
        />
      </el-form-item>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="经度" prop="longitude">
            <el-input-number
              v-model="formData.longitude"
              :precision="6"
              :step="0.000001"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="纬度" prop="latitude">
            <el-input-number
              v-model="formData.latitude"
              :precision="6"
              :step="0.000001"
              :controls="false"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="开始日期" prop="startDate">
            <el-date-picker
              v-model="formData.startDate"
              type="date"
              placeholder="请选择开始日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="结束日期" prop="endDate">
            <el-date-picker
              v-model="formData.endDate"
              type="date"
              placeholder="请选择结束日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="总投资(万元)" prop="totalInvestment">
        <el-input-number
          v-model="formData.totalInvestment"
          :precision="2"
          :step="1"
          :min="0"
          style="width: 100%"
        />
      </el-form-item>
      
      <el-form-item label="项目描述" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          placeholder="请输入项目描述"
        />
      </el-form-item>
      
      <el-form-item label="项目状态" prop="status" v-if="formType === 'edit'">
        <el-select
          v-model="formData.status"
          placeholder="请选择项目状态"
        >
          <el-option
            v-for="item in projectStatusOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
    </form-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, OfficeBuilding, Location, User, Phone, Calendar, Monitor, Money, ArrowDown } from '@element-plus/icons-vue'
import { projectService, companyService } from '@/api/services'
import { formatDate, formatDateTime, formatCurrency } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import SearchForm from '@/components/SearchForm.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'

const route = useRoute()
const router = useRouter()

// 搜索参数
const searchParams = reactive({
  keyword: '',
  companyId: route.query.companyId as string || undefined,
  status: undefined,
  startDateFrom: undefined,
  startDateTo: undefined,
  pageIndex: 1,
  pageSize: 8
})

// 日期范围
const dateRange = ref<[string, string] | null>(null)

// 表格数据
const tableData = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 公司选项
const companyOptions = ref<{ label: string; value: string }[]>([])
const loadingCompanies = ref(false)

// 项目状态选项
const projectStatusOptions = [
  { label: '规划中', value: 'Planning' },
  { label: '进行中', value: 'InProgress' },
  { label: '已完成', value: 'Completed' },
  { label: '已暂停', value: 'OnHold' },
  { label: '已取消', value: 'Cancelled' }
]

// 表单相关
const dialogVisible = ref(false)
const formType = ref<'create' | 'edit'>('create')
const formLoading = ref(false)
const formData = reactive({
  id: '',
  companyId: route.query.companyId as string || '',
  code: '',
  name: '',
  description: '',
  address: '',
  longitude: undefined as number | undefined,
  latitude: undefined as number | undefined,
  startDate: '',
  endDate: '',
  totalInvestment: undefined as number | undefined,
  projectManager: '',
  managerPhone: '',
  status: 'Planning'
})

// 表单校验规则
const formRules = {
  companyId: [
    { required: true, message: '请选择所属公司', trigger: 'change' }
  ],
  code: [
    { required: true, message: '请输入项目编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入项目名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  startDate: [
    { required: true, message: '请选择开始日期', trigger: 'change' }
  ],
  projectManager: [
    { max: 50, message: '长度不能超过 50 个字符', trigger: 'blur' }
  ],
  managerPhone: [
    { max: 20, message: '长度不能超过 20 个字符', trigger: 'blur' }
  ],
  address: [
    { max: 200, message: '长度不能超过 200 个字符', trigger: 'blur' }
  ],
  description: [
    { max: 500, message: '长度不能超过 500 个字符', trigger: 'blur' }
  ]
}

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增项目' : '编辑项目'
})

// 组件引用
const searchFormRef = ref()

// 初始化
onMounted(() => {
  fetchCompanies()
  fetchData()
})

// 获取公司列表
const fetchCompanies = async () => {
  loadingCompanies.value = true
  try {
    const { data } = await companyService.getAll()
    companyOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id
    }))
  } catch (error) {
    console.error('获取公司列表失败:', error)
  } finally {
    loadingCompanies.value = false
  }
}

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await projectService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取项目列表失败:', error)
    ElMessage.error('获取项目列表失败')
  } finally {
    loading.value = false
  }
}

// 日期范围变化
const handleDateRangeChange = (val: [string, string] | null) => {
  if (val) {
    searchParams.startDateFrom = val[0]
    searchParams.startDateTo = val[1]
  } else {
    searchParams.startDateFrom = undefined
    searchParams.startDateTo = undefined
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

// 新增
const handleCreate = () => {
  formType.value = 'create'
  resetForm()
  
  // 如果URL中有companyId参数，则设置为默认值
  if (route.query.companyId) {
    formData.companyId = route.query.companyId as string
  }
  
  dialogVisible.value = true
}

// 查看
const handleView = (row: any) => {
  // 跳转到详情页
  router.push(`/project/detail/${row.id}`)
}

// 编辑
const handleEdit = (row: any) => {
  formType.value = 'edit'
  resetForm()
  Object.assign(formData, row)
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除项目"${row.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await projectService.delete(row.id)
    if (data) {
      ElMessage.success('删除成功')
      fetchData()
    }
  } catch (error) {
    console.error('删除失败:', error)
  }
}

// 处理下拉菜单命令
const handleCommand = (command: string, row: any) => {
  switch (command) {
    case 'dashboard':
      router.push(`/dashboard/project/${row.id}`)
      break
    case 'workers':
      router.push({
        path: '/worker/list',
        query: { projectId: row.id }
      })
      break
    case 'devices':
      router.push({
        path: '/device/list',
        query: { projectId: row.id }
      })
      break
    case 'safety':
      router.push({
        path: '/safety/incident',
        query: { projectId: row.id }
      })
      break
    case 'delete':
      handleDelete(row)
      break
  }
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    if (formType.value === 'create') {
      // 检查项目编码是否存在
      const { data: codeExists } = await projectService.checkCode(values.code)
      if (codeExists) {
        ElMessage.error('项目编码已存在')
        formLoading.value = false
        return
      }
      
      // 创建项目
      const { data } = await projectService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      // 更新项目
      const { data } = await projectService.update(values.id, values)
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
    companyId: route.query.companyId as string || '',
    code: '',
    name: '',
    description: '',
    address: '',
    longitude: undefined,
    latitude: undefined,
    startDate: '',
    endDate: '',
    totalInvestment: undefined,
    projectManager: '',
    managerPhone: '',
    status: 'Planning'
  })
}

// 获取进度颜色
const getProgressColor = (progress: number) => {
  if (progress >= 80) return '#67C23A'
  if (progress >= 50) return '#409EFF'
  if (progress >= 30) return '#E6A23C'
  return '#F56C6C'
}
</script>

<style lang="scss" scoped>
.project-list-container {
  padding: 20px;
  
  .project-card {
    margin-bottom: 20px;
    
    .project-header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 15px;
      
      .project-title {
        h3 {
          margin: 0 0 5px;
          font-size: 18px;
          font-weight: 600;
          color: var(--el-text-color-primary);
        }
        
        p {
          margin: 0;
          font-size: 14px;
          color: var(--el-text-color-secondary);
        }
      }
    }
    
    .project-info {
      margin-bottom: 15px;
      
      p {
        display: flex;
        align-items: center;
        margin: 5px 0;
        font-size: 14px;
        color: var(--el-text-color-regular);
        
        .el-icon {
          margin-right: 8px;
          font-size: 16px;
        }
      }
    }
    
    .project-progress {
      margin-bottom: 15px;
      
      .progress-header {
        display: flex;
        justify-content: space-between;
        margin-bottom: 5px;
        font-size: 14px;
        color: var(--el-text-color-primary);
      }
    }
    
    .project-stats {
      display: flex;
      justify-content: space-between;
      margin-bottom: 15px;
      
      .stat-item {
        display: flex;
        flex-direction: column;
        align-items: center;
        
        .el-icon {
          font-size: 24px;
          color: var(--el-color-primary);
          margin-bottom: 5px;
        }
        
        span:nth-child(2) {
          font-size: 16px;
          font-weight: bold;
          color: var(--el-text-color-primary);
        }
        
        span:nth-child(3) {
          font-size: 12px;
          color: var(--el-text-color-secondary);
        }
      }
    }
    
    .project-actions {
      display: flex;
      justify-content: space-between;
      
      .el-button {
        flex: 1;
        margin-right: 10px;
        
        &:last-child {
          margin-right: 0;
        }
      }
    }
  }
  
  .pagination-container {
    margin-top: 20px;
    display: flex;
    justify-content: center;
  }
}
</style>