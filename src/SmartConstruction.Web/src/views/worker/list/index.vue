<template>
  <div class="worker-list-container">
    <page-header title="工人管理" subtitle="管理系统中的工人信息">
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'worker.create'">
          <el-icon><plus /></el-icon>
          新增工人
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
            v-for="item in teamOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="工人类型" prop="type">
        <el-select
          v-model="searchParams.type"
          placeholder="请选择工人类型"
          clearable
        >
          <el-option
            v-for="item in workerTypeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="状态" prop="isActive">
        <el-select
          v-model="searchParams.isActive"
          placeholder="请选择状态"
          clearable
        >
          <el-option label="在岗" :value="true" />
          <el-option label="离岗" :value="false" />
        </el-select>
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
      <el-table-column prop="employeeNo" label="工号" min-width="120" />
      <el-table-column prop="name" label="姓名" min-width="100" />
      <el-table-column prop="idCard" label="身份证号" min-width="180" show-overflow-tooltip />
      <el-table-column prop="phone" label="联系电话" min-width="120" />
      <el-table-column prop="companyName" label="所属公司" min-width="150" show-overflow-tooltip />
      <el-table-column prop="teamName" label="所属班组" min-width="120" />
      <el-table-column prop="type" label="工人类型" width="100">
        <template #default="{ row }">
          {{ getWorkerTypeName(row.type) }}
        </template>
      </el-table-column>
      <el-table-column prop="gender" label="性别" width="80">
        <template #default="{ row }">
          {{ row.gender === 'Male' ? '男' : '女' }}
        </template>
      </el-table-column>
      <el-table-column prop="isActive" label="状态" width="80" align="center">
        <template #default="{ row }">
          <status-tag
            :status="row.isActive"
            :status-map="{
              true: { type: 'success', label: '在岗' },
              false: { type: 'danger', label: '离岗' }
            }"
          />
        </template>
      </el-table-column>
      <el-table-column prop="createdTime" label="入职时间" min-width="150">
        <template #default="{ row }">
          {{ formatDateTime(row.createdTime) }}
        </template>
      </el-table-column>
      
      <template #actions="{ row }">
        <el-button
          type="primary"
          link
          @click="handleView(row)"
          v-permission="'worker.view'"
        >
          查看
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleEdit(row)"
          v-permission="'worker.edit'"
        >
          编辑
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleToggleStatus(row)"
          v-permission="'worker.edit'"
        >
          {{ row.isActive ? '离岗' : '入岗' }}
        </el-button>
        <el-button
          type="danger"
          link
          @click="handleDelete(row)"
          v-permission="'worker.delete'"
        >
          删除
        </el-button>
      </template>
    </data-table>
    
    <!-- 工人表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      :title="dialogTitle"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="700px"
      @submit="handleSubmit"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="工号" prop="employeeNo">
            <el-input
              v-model="formData.employeeNo"
              placeholder="请输入工号"
              :disabled="formType === 'edit'"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="姓名" prop="name">
            <el-input
              v-model="formData.name"
              placeholder="请输入姓名"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="身份证号" prop="idCard">
            <el-input
              v-model="formData.idCard"
              placeholder="请输入身份证号"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="联系电话" prop="phone">
            <el-input
              v-model="formData.phone"
              placeholder="请输入联系电话"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="性别" prop="gender">
            <el-select
              v-model="formData.gender"
              placeholder="请选择性别"
            >
              <el-option label="男" value="Male" />
              <el-option label="女" value="Female" />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="出生日期" prop="dateOfBirth">
            <el-date-picker
              v-model="formData.dateOfBirth"
              type="date"
              placeholder="请选择出生日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="所属公司" prop="companyId">
            <el-select
              v-model="formData.companyId"
              placeholder="请选择公司"
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
        </el-col>
        <el-col :span="12">
          <el-form-item label="所属班组" prop="teamId">
            <el-select
              v-model="formData.teamId"
              placeholder="请选择班组"
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
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="工人类型" prop="type">
            <el-select
              v-model="formData.type"
              placeholder="请选择工人类型"
            >
              <el-option
                v-for="item in workerTypeOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="状态" prop="isActive" v-if="formType === 'edit'">
            <el-switch
              v-model="formData.isActive"
              active-text="在岗"
              inactive-text="离岗"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="家庭住址" prop="address">
        <el-input
          v-model="formData.address"
          placeholder="请输入家庭住址"
          type="textarea"
          :rows="2"
        />
      </el-form-item>
    </form-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { workerService, companyService, teamService } from '@/api/services'
import { formatDate, formatDateTime } from '@/utils/format'
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
  isActive: undefined,
  pageIndex: 1,
  pageSize: 10
})

// 表格数据
const tableData = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 选项数据
const companyOptions = ref<{ label: string; value: string }[]>([])
const teamOptions = ref<{ label: string; value: string; companyId: string }[]>([])

// 工人类型选项
const workerTypeOptions = [
  { label: '普通工人', value: 'General' },
  { label: '技术工人', value: 'Technical' },
  { label: '管理人员', value: 'Management' },
  { label: '安全员', value: 'Safety' },
  { label: '质检员', value: 'QualityControl' },
  { label: '其他', value: 'Other' }
]

// 表单相关
const dialogVisible = ref(false)
const formType = ref<'create' | 'edit'>('create')
const formLoading = ref(false)
const formData = reactive({
  id: '',
  employeeNo: '',
  name: '',
  idCard: '',
  phone: '',
  gender: 'Male',
  dateOfBirth: '',
  companyId: '',
  teamId: '',
  type: 'General',
  address: '',
  isActive: true
})

// 表单校验规则
const formRules = {
  employeeNo: [
    { required: true, message: '请输入工号', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入姓名', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  idCard: [
    { required: true, message: '请输入身份证号', trigger: 'blur' },
    { pattern: /^\d{17}[\dXx]$/, message: '请输入正确的身份证号', trigger: 'blur' }
  ],
  phone: [
    { required: true, message: '请输入联系电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  companyId: [
    { required: true, message: '请选择公司', trigger: 'change' }
  ],
  teamId: [
    { required: true, message: '请选择班组', trigger: 'change' }
  ],
  type: [
    { required: true, message: '请选择工人类型', trigger: 'change' }
  ]
}

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增工人' : '编辑工人'
})

// 过滤后的班组选项
const filteredTeamOptions = computed(() => {
  if (!formData.companyId) return []
  return teamOptions.value.filter(team => team.companyId === formData.companyId)
})

// 组件引用
const searchFormRef = ref()
const tableRef = ref()

// 初始化
onMounted(() => {
  fetchCompanies()
  fetchTeams()
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

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await workerService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取工人列表失败:', error)
    ElMessage.error('获取工人列表失败')
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
  formData.teamId = ''
}

// 新增
const handleCreate = () => {
  formType.value = 'create'
  resetForm()
  
  // 如果URL中有参数，则设置为默认值
  if (route.query.companyId) {
    formData.companyId = route.query.companyId as string
  }
  if (route.query.teamId) {
    formData.teamId = route.query.teamId as string
  }
  
  dialogVisible.value = true
}

// 查看
const handleView = (row: any) => {
  router.push(`/worker/detail/${row.id}`)
}

// 编辑
const handleEdit = (row: any) => {
  formType.value = 'edit'
  resetForm()
  Object.assign(formData, row)
  dialogVisible.value = true
}

// 切换状态
const handleToggleStatus = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要${row.isActive ? '离岗' : '入岗'}该工人吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await workerService.updateStatus(row.id, !row.isActive)
    if (data) {
      ElMessage.success(`${row.isActive ? '离岗' : '入岗'}成功`)
      fetchData()
    }
  } catch (error) {
    console.error('操作失败:', error)
  }
}

// 删除
const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除工人"${row.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await workerService.delete(row.id)
    if (data) {
      ElMessage.success('删除成功')
      fetchData()
    }
  } catch (error) {
    console.error('删除失败:', error)
  }
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    if (formType.value === 'create') {
      // 检查工号是否存在
      const { data: employeeNoExists } = await workerService.checkEmployeeNo(values.employeeNo)
      if (employeeNoExists) {
        ElMessage.error('工号已存在')
        formLoading.value = false
        return
      }
      
      // 检查身份证号是否存在
      const { data: idCardExists } = await workerService.checkIdCard(values.idCard)
      if (idCardExists) {
        ElMessage.error('身份证号已存在')
        formLoading.value = false
        return
      }
      
      // 创建工人
      const { data } = await workerService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      // 更新工人
      const { data } = await workerService.update(values.id, values)
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
    employeeNo: '',
    name: '',
    idCard: '',
    phone: '',
    gender: 'Male',
    dateOfBirth: '',
    companyId: '',
    teamId: '',
    type: 'General',
    address: '',
    isActive: true
  })
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
</script>

<style lang="scss" scoped>
.worker-list-container {
  padding: 20px;
}
</style>