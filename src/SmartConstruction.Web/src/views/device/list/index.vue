<template>
  <div class="device-list-container">
    <page-header title="设备管理" subtitle="管理工地设备信息和监控状态">
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'device.create'">
          <el-icon><plus /></el-icon>
          新增设备
        </el-button>
      </template>
    </page-header>
    
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="设备名称" prop="keyword">
        <el-input
          v-model="searchParams.keyword"
          placeholder="请输入设备名称或编号"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="所属项目" prop="projectId">
        <el-select
          v-model="searchParams.projectId"
          placeholder="请选择项目"
          clearable
          filterable
        >
          <el-option
            v-for="item in projectOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="设备类型" prop="type">
        <el-select
          v-model="searchParams.type"
          placeholder="请选择设备类型"
          clearable
        >
          <el-option
            v-for="item in deviceTypeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="设备状态" prop="status">
        <el-select
          v-model="searchParams.status"
          placeholder="请选择设备状态"
          clearable
        >
          <el-option
            v-for="item in deviceStatusOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
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
      <el-table-column prop="code" label="设备编号" min-width="120" />
      <el-table-column prop="name" label="设备名称" min-width="150" />
      <el-table-column prop="type" label="设备类型" width="120">
        <template #default="{ row }">
          {{ getDeviceTypeName(row.type) }}
        </template>
      </el-table-column>
      <el-table-column prop="projectName" label="所属项目" min-width="150" show-overflow-tooltip />
      <el-table-column prop="manufacturer" label="制造商" min-width="120" />
      <el-table-column prop="model" label="型号" min-width="120" />
      <el-table-column prop="status" label="设备状态" width="100" align="center">
        <template #default="{ row }">
          <status-tag
            :status="row.status"
            :status-map="{
              'Online': { type: 'success', label: '在线' },
              'Offline': { type: 'danger', label: '离线' },
              'Maintenance': { type: 'warning', label: '维护中' },
              'Fault': { type: 'danger', label: '故障' },
              'Idle': { type: 'info', label: '空闲' }
            }"
          />
        </template>
      </el-table-column>
      <el-table-column prop="lastOnlineTime" label="最后在线时间" min-width="150">
        <template #default="{ row }">
          {{ row.lastOnlineTime ? formatDateTime(row.lastOnlineTime) : '从未在线' }}
        </template>
      </el-table-column>
      <el-table-column prop="installationDate" label="安装日期" min-width="120">
        <template #default="{ row }">
          {{ formatDate(row.installationDate) }}
        </template>
      </el-table-column>
      <el-table-column prop="location" label="安装位置" min-width="150" show-overflow-tooltip />
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
          v-permission="'device.view'"
        >
          查看
        </el-button>
        <el-button
          type="success"
          link
          @click="handleMonitor(row)"
          v-permission="'device.monitor'"
        >
          监控
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleEdit(row)"
          v-permission="'device.edit'"
        >
          编辑
        </el-button>
        <el-dropdown
          trigger="click"
          @command="(command) => handleCommand(command, row)"
        >
          <el-button type="primary" link>
            更多
            <el-icon><arrow-down /></el-icon>
          </el-button>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item command="maintenance" v-permission="'device.maintenance'">
                设置维护
              </el-dropdown-item>
              <el-dropdown-item command="reset" v-permission="'device.reset'">
                重置设备
              </el-dropdown-item>
              <el-dropdown-item command="delete" divided v-permission="'device.delete'">
                删除设备
              </el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </template>
    </data-table>
    
    <!-- 设备表单对话框 -->
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
          <el-form-item label="设备编号" prop="code">
            <el-input
              v-model="formData.code"
              placeholder="请输入设备编号"
              :disabled="formType === 'edit'"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="设备名称" prop="name">
            <el-input
              v-model="formData.name"
              placeholder="请输入设备名称"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="设备类型" prop="type">
            <el-select
              v-model="formData.type"
              placeholder="请选择设备类型"
            >
              <el-option
                v-for="item in deviceTypeOptions"
                :key="item.value"
                :label="item.label"
                :value="item.value"
              />
            </el-select>
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="所属项目" prop="projectId">
            <el-select
              v-model="formData.projectId"
              placeholder="请选择项目"
              filterable
            >
              <el-option
                v-for="item in projectOptions"
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
          <el-form-item label="制造商" prop="manufacturer">
            <el-input
              v-model="formData.manufacturer"
              placeholder="请输入制造商"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="设备型号" prop="model">
            <el-input
              v-model="formData.model"
              placeholder="请输入设备型号"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="序列号" prop="serialNumber">
            <el-input
              v-model="formData.serialNumber"
              placeholder="请输入序列号"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="安装日期" prop="installationDate">
            <el-date-picker
              v-model="formData.installationDate"
              type="date"
              placeholder="请选择安装日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>
      
      <el-form-item label="安装位置" prop="location">
        <el-input
          v-model="formData.location"
          placeholder="请输入安装位置"
        />
      </el-form-item>
      
      <el-form-item label="IP地址" prop="ipAddress">
        <el-input
          v-model="formData.ipAddress"
          placeholder="请输入IP地址"
        />
      </el-form-item>
      
      <el-form-item label="设备描述" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          placeholder="请输入设备描述"
        />
      </el-form-item>
    </form-dialog>

    <!-- 维护设置对话框 -->
    <el-dialog
      v-model="maintenanceDialogVisible"
      title="设置设备维护"
      width="500px"
    >
      <el-form
        ref="maintenanceFormRef"
        :model="maintenanceForm"
        :rules="maintenanceRules"
        label-width="100px"
      >
        <el-form-item label="维护类型" prop="type">
          <el-select
            v-model="maintenanceForm.type"
            placeholder="请选择维护类型"
          >
            <el-option label="定期维护" value="regular" />
            <el-option label="故障维护" value="repair" />
            <el-option label="升级维护" value="upgrade" />
          </el-select>
        </el-form-item>
        <el-form-item label="维护开始时间" prop="startTime">
          <el-date-picker
            v-model="maintenanceForm.startTime"
            type="datetime"
            placeholder="请选择开始时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="预计结束时间" prop="estimatedEndTime">
          <el-date-picker
            v-model="maintenanceForm.estimatedEndTime"
            type="datetime"
            placeholder="请选择预计结束时间"
            format="YYYY-MM-DD HH:mm:ss"
            value-format="YYYY-MM-DD HH:mm:ss"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="维护说明" prop="description">
          <el-input
            v-model="maintenanceForm.description"
            type="textarea"
            :rows="3"
            placeholder="请输入维护说明"
          />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="maintenanceDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="handleMaintenanceSubmit" :loading="maintenanceLoading">
            确定
          </el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, ArrowDown } from '@element-plus/icons-vue'
import { deviceService, projectService } from '@/api/services'
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
  projectId: route.query.projectId as string || undefined,
  type: undefined,
  status: undefined,
  pageIndex: 1,
  pageSize: 10
})

// 表格数据
const tableData = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 选项数据
const projectOptions = ref<{ label: string; value: string }[]>([])

// 设备类型选项
const deviceTypeOptions = [
  { label: '塔吊', value: 'TowerCrane' },
  { label: '升降机', value: 'Elevator' },
  { label: '监控摄像头', value: 'Camera' },
  { label: '环境监测仪', value: 'EnvironmentSensor' },
  { label: '安全帽识别', value: 'HelmetDetector' },
  { label: '扬尘监测', value: 'DustMonitor' },
  { label: '噪音监测', value: 'NoiseMonitor' },
  { label: '其他', value: 'Other' }
]

// 设备状态选项
const deviceStatusOptions = [
  { label: '在线', value: 'Online' },
  { label: '离线', value: 'Offline' },
  { label: '维护中', value: 'Maintenance' },
  { label: '故障', value: 'Fault' },
  { label: '空闲', value: 'Idle' }
]

// 表单相关
const dialogVisible = ref(false)
const formType = ref<'create' | 'edit'>('create')
const formLoading = ref(false)
const formData = reactive({
  id: '',
  code: '',
  name: '',
  type: '',
  projectId: '',
  manufacturer: '',
  model: '',
  serialNumber: '',
  installationDate: '',
  location: '',
  ipAddress: '',
  description: ''
})

// 维护表单
const maintenanceDialogVisible = ref(false)
const maintenanceLoading = ref(false)
const maintenanceFormRef = ref()
const maintenanceForm = reactive({
  deviceId: '',
  type: '',
  startTime: '',
  estimatedEndTime: '',
  description: ''
})

// 表单校验规则
const formRules = {
  code: [
    { required: true, message: '请输入设备编号', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入设备名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择设备类型', trigger: 'change' }
  ],
  projectId: [
    { required: true, message: '请选择项目', trigger: 'change' }
  ],
  installationDate: [
    { required: true, message: '请选择安装日期', trigger: 'change' }
  ]
}

// 维护表单校验规则
const maintenanceRules = {
  type: [
    { required: true, message: '请选择维护类型', trigger: 'change' }
  ],
  startTime: [
    { required: true, message: '请选择维护开始时间', trigger: 'change' }
  ],
  description: [
    { required: true, message: '请输入维护说明', trigger: 'blur' }
  ]
}

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增设备' : '编辑设备'
})

// 组件引用
const searchFormRef = ref()
const tableRef = ref()

// 初始化
onMounted(() => {
  fetchProjects()
  fetchData()
})

// 获取项目列表
const fetchProjects = async () => {
  try {
    const { data } = await projectService.getAll()
    projectOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id
    }))
  } catch (error) {
    console.error('获取项目列表失败:', error)
  }
}

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await deviceService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取设备列表失败:', error)
    ElMessage.error('获取设备列表失败')
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

// 新增
const handleCreate = () => {
  formType.value = 'create'
  resetForm()
  
  // 如果URL中有项目ID参数，则设置为默认值
  if (route.query.projectId) {
    formData.projectId = route.query.projectId as string
  }
  
  dialogVisible.value = true
}

// 查看
const handleView = (row: any) => {
  router.push(`/device/detail/${row.id}`)
}

// 监控
const handleMonitor = (row: any) => {
  router.push(`/device/monitor?deviceId=${row.id}`)
}

// 编辑
const handleEdit = (row: any) => {
  formType.value = 'edit'
  resetForm()
  Object.assign(formData, row)
  dialogVisible.value = true
}

// 命令处理
const handleCommand = (command: string, row: any) => {
  switch (command) {
    case 'maintenance':
      handleSetMaintenance(row)
      break
    case 'reset':
      handleResetDevice(row)
      break
    case 'delete':
      handleDelete(row)
      break
  }
}

// 设置维护
const handleSetMaintenance = (row: any) => {
  maintenanceForm.deviceId = row.id
  maintenanceForm.type = ''
  maintenanceForm.startTime = ''
  maintenanceForm.estimatedEndTime = ''
  maintenanceForm.description = ''
  maintenanceDialogVisible.value = true
}

// 重置设备
const handleResetDevice = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要重置设备"${row.name}"吗？重置后设备需要重新连接。`,
      '重置设备',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await deviceService.reset(row.id)
    if (data) {
      ElMessage.success('设备重置成功')
      fetchData()
    }
  } catch (error) {
    console.error('重置设备失败:', error)
  }
}

// 删除
const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除设备"${row.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await deviceService.delete(row.id)
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
      // 检查设备编号是否存在
      const { data: codeExists } = await deviceService.checkCode(values.code)
      if (codeExists) {
        ElMessage.error('设备编号已存在')
        formLoading.value = false
        return
      }
      
      // 创建设备
      const { data } = await deviceService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      // 更新设备
      const { data } = await deviceService.update(values.id, values)
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

// 提交维护设置
const handleMaintenanceSubmit = async () => {
  if (!maintenanceFormRef.value) return
  
  try {
    await maintenanceFormRef.value.validate()
    maintenanceLoading.value = true
    
    const { data } = await deviceService.setMaintenance(maintenanceForm.deviceId, maintenanceForm)
    if (data) {
      ElMessage.success('维护设置成功')
      maintenanceDialogVisible.value = false
      fetchData()
    }
  } catch (error) {
    console.error('维护设置失败:', error)
    ElMessage.error('维护设置失败')
  } finally {
    maintenanceLoading.value = false
  }
}

// 重置表单
const resetForm = () => {
  Object.assign(formData, {
    id: '',
    code: '',
    name: '',
    type: '',
    projectId: '',
    manufacturer: '',
    model: '',
    serialNumber: '',
    installationDate: '',
    location: '',
    ipAddress: '',
    description: ''
  })
}

// 获取设备类型名称
const getDeviceTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'TowerCrane': '塔吊',
    'Elevator': '升降机',
    'Camera': '监控摄像头',
    'EnvironmentSensor': '环境监测仪',
    'HelmetDetector': '安全帽识别',
    'DustMonitor': '扬尘监测',
    'NoiseMonitor': '噪音监测',
    'Other': '其他'
  }
  return typeMap[type] || type
}
</script>

<style lang="scss" scoped>
.device-list-container {
  padding: 20px;
}
</style>