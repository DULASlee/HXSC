<template>
  <div class="team-detail-container">
    <page-header :title="team.name || '班组详情'" :subtitle="`所属公司: ${team.companyName || ''}`">
      <template #actions>
        <el-button @click="goBack">
          <el-icon><back /></el-icon>
          返回
        </el-button>
        <el-button type="primary" @click="handleEdit" v-permission="'team.edit'">
          <el-icon><edit /></el-icon>
          编辑
        </el-button>
      </template>
    </page-header>

    <el-row :gutter="20">
      <el-col :xs="24" :md="16">
        <!-- 基本信息 -->
        <detail-card title="基本信息" :loading="loading">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="班组名称">{{ team.name }}</el-descriptions-item>
            <el-descriptions-item label="所属公司">{{ team.companyName }}</el-descriptions-item>
            <el-descriptions-item label="班组长">{{ team.leader }}</el-descriptions-item>
            <el-descriptions-item label="联系电话">{{ team.leaderPhone }}</el-descriptions-item>
            <el-descriptions-item label="班组类型">{{ getTeamTypeName(team.type) }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <status-tag
                :status="team.isActive"
                :status-map="{
                  true: { type: 'success', label: '启用' },
                  false: { type: 'danger', label: '禁用' }
                }"
              />
            </el-descriptions-item>
            <el-descriptions-item label="创建时间">{{ formatDateTime(team.createdTime) }}</el-descriptions-item>
            <el-descriptions-item label="更新时间">{{ formatDateTime(team.updatedTime) }}</el-descriptions-item>
          </el-descriptions>
        </detail-card>

        <!-- 工人列表 -->
        <detail-card title="工人列表" :loading="workersLoading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleAddWorker" v-permission="'worker.create'">
              <el-icon><plus /></el-icon>
              添加工人
            </el-button>
          </template>

          <el-table :data="workers" border stripe>
            <el-table-column prop="name" label="姓名" min-width="100" />
            <el-table-column prop="idCardNumber" label="身份证号" min-width="180" show-overflow-tooltip />
            <el-table-column prop="phone" label="联系电话" min-width="120" />
            <el-table-column prop="gender" label="性别" width="80" align="center">
              <template #default="{ row }">
                {{ row.gender === 'Male' ? '男' : row.gender === 'Female' ? '女' : '其他' }}
              </template>
            </el-table-column>
            <el-table-column prop="type" label="工种" width="100">
              <template #default="{ row }">
                {{ getWorkerTypeName(row.type) }}
              </template>
            </el-table-column>
            <el-table-column prop="isActive" label="状态" width="80" align="center">
              <template #default="{ row }">
                <status-tag
                  :status="row.isActive"
                  :status-map="{
                    true: { type: 'success', label: '在职' },
                    false: { type: 'danger', label: '离职' }
                  }"
                />
              </template>
            </el-table-column>
            <el-table-column label="操作" width="150" align="center">
              <template #default="{ row }">
                <el-button type="primary" link @click="viewWorker(row)">
                  查看
                </el-button>
                <el-button type="danger" link @click="handleRemoveWorker(row)" v-permission="'worker.edit'">
                  移除
                </el-button>
              </template>
            </el-table-column>
          </el-table>

          <template #footer>
            <el-pagination
              v-model:current-page="workerParams.pageIndex"
              v-model:page-size="workerParams.pageSize"
              :page-sizes="[5, 10, 20, 50]"
              :total="workerTotal"
              layout="total, sizes, prev, pager, next"
              @size-change="handleWorkerSizeChange"
              @current-change="handleWorkerPageChange"
            />
          </template>
        </detail-card>

        <!-- 考勤统计 -->
        <detail-card title="考勤统计" :loading="statsLoading">
          <div class="attendance-stats">
            <el-row :gutter="20">
              <el-col :span="8">
                <stat-card
                  title="总工人数"
                  :value="stats.workerCount || 0"
                  icon="User"
                  icon-bg-color="#409EFF"
                />
              </el-col>
              <el-col :span="8">
                <stat-card
                  title="今日出勤"
                  :value="stats.todayAttendance || 0"
                  icon="Calendar"
                  icon-bg-color="#67C23A"
                  :footer="`出勤率: ${stats.attendanceRate || 0}%`"
                />
              </el-col>
              <el-col :span="8">
                <stat-card
                  title="本月工时"
                  :value="stats.monthlyWorkHours || 0"
                  icon="Timer"
                  icon-bg-color="#E6A23C"
                  :footer="`人均: ${stats.averageWorkHours || 0}小时`"
                />
              </el-col>
            </el-row>
          </div>

          <div class="attendance-chart mt-20">
            <chart-card
              title="近7天考勤统计"
              :options="attendanceChartOptions"
              height="300px"
              :loading="statsLoading"
            />
          </div>
        </detail-card>
      </el-col>

      <el-col :xs="24" :md="8">
        <!-- 班组统计 -->
        <detail-card title="班组统计" :loading="statsLoading">
          <div class="team-stats">
            <el-row :gutter="20">
              <el-col :span="12">
                <stat-card
                  title="工人数量"
                  :value="stats.workerCount || 0"
                  icon="User"
                  icon-bg-color="#409EFF"
                />
              </el-col>
              <el-col :span="12">
                <stat-card
                  title="项目数量"
                  :value="stats.projectCount || 0"
                  icon="Folder"
                  icon-bg-color="#67C23A"
                />
              </el-col>
            </el-row>
          </div>
        </detail-card>

        <!-- 工种分布 -->
        <detail-card title="工种分布" :loading="statsLoading">
          <div class="worker-type-chart">
            <chart-card
              title="工种分布"
              :options="workerTypeChartOptions"
              height="300px"
              :loading="statsLoading"
            />
          </div>
        </detail-card>

        <!-- 项目分布 -->
        <detail-card title="项目分布" :loading="statsLoading">
          <div class="project-list">
            <el-empty v-if="projects.length === 0" description="暂无项目" />
            <el-table v-else :data="projects" border stripe>
              <el-table-column prop="name" label="项目名称" min-width="150" />
              <el-table-column prop="workerCount" label="工人数" width="80" align="center" />
              <el-table-column label="操作" width="80" align="center">
                <template #default="{ row }">
                  <el-button type="primary" link @click="viewProject(row)">
                    查看
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </detail-card>
      </el-col>
    </el-row>

    <!-- 班组表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      title="编辑班组"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="500px"
      @submit="handleSubmit"
    >
      <el-form-item label="所属公司" prop="companyId">
        <el-select
          v-model="formData.companyId"
          placeholder="请选择公司"
          filterable
          disabled
        >
          <el-option
            v-for="item in companyOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="班组名称" prop="name">
        <el-input
          v-model="formData.name"
          placeholder="请输入班组名称"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="班组长" prop="leader">
            <el-input
              v-model="formData.leader"
              placeholder="请输入班组长姓名"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="联系电话" prop="leaderPhone">
            <el-input
              v-model="formData.leaderPhone"
              placeholder="请输入联系电话"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="班组类型" prop="type">
        <el-select
          v-model="formData.type"
          placeholder="请选择班组类型"
        >
          <el-option
            v-for="item in teamTypeOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="状态" prop="isActive">
        <el-switch
          v-model="formData.isActive"
          active-text="启用"
          inactive-text="禁用"
        />
      </el-form-item>
    </form-dialog>

    <!-- 添加工人对话框 -->
    <el-dialog
      v-model="addWorkerDialogVisible"
      title="添加工人"
      width="700px"
      destroy-on-close
    >
      <el-form :model="workerSearchParams" inline>
        <el-form-item label="姓名">
          <el-input
            v-model="workerSearchParams.keyword"
            placeholder="请输入工人姓名"
            clearable
            @keyup.enter="searchAvailableWorkers"
          />
        </el-form-item>
        <el-form-item label="身份证号">
          <el-input
            v-model="workerSearchParams.idCardNumber"
            placeholder="请输入身份证号"
            clearable
            @keyup.enter="searchAvailableWorkers"
          />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="searchAvailableWorkers" :loading="availableWorkersLoading">
            搜索
          </el-button>
          <el-button @click="resetWorkerSearch">
            重置
          </el-button>
        </el-form-item>
      </el-form>

      <el-table
        v-loading="availableWorkersLoading"
        :data="availableWorkers"
        border
        stripe
        @selection-change="handleWorkerSelectionChange"
      >
        <el-table-column type="selection" width="55" />
        <el-table-column prop="name" label="姓名" min-width="100" />
        <el-table-column prop="idCardNumber" label="身份证号" min-width="180" show-overflow-tooltip />
        <el-table-column prop="phone" label="联系电话" min-width="120" />
        <el-table-column prop="gender" label="性别" width="80" align="center">
          <template #default="{ row }">
            {{ row.gender === 'Male' ? '男' : row.gender === 'Female' ? '女' : '其他' }}
          </template>
        </el-table-column>
        <el-table-column prop="type" label="工种" width="100">
          <template #default="{ row }">
            {{ getWorkerTypeName(row.type) }}
          </template>
        </el-table-column>
      </el-table>

      <div class="dialog-footer">
        <el-pagination
          v-model:current-page="workerSearchParams.pageIndex"
          v-model:page-size="workerSearchParams.pageSize"
          :page-sizes="[5, 10, 20, 50]"
          :total="availableWorkerTotal"
          layout="total, sizes, prev, pager, next"
          @size-change="handleAvailableWorkerSizeChange"
          @current-change="handleAvailableWorkerPageChange"
        />
      </div>

      <template #footer>
        <span class="dialog-footer">
          <el-button @click="addWorkerDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="confirmAddWorkers" :loading="addingWorkers">
            确认添加
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
import { Back, Edit, Plus, Calendar, Timer } from '@element-plus/icons-vue'
import { teamService, workerService, companyService } from '@/api/services'
import { formatDate, formatDateTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import DetailCard from '@/components/DetailCard.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'
import StatCard from '@/components/StatCard.vue'
import ChartCard from '@/components/ChartCard.vue'

const route = useRoute()
const router = useRouter()
const teamId = route.params.id as string

// 班组信息
const team = ref<any>({})
const loading = ref(false)

// 统计信息
const stats = ref<any>({})
const statsLoading = ref(false)

// 工人列表
const workers = ref<any[]>([])
const workerTotal = ref(0)
const workersLoading = ref(false)
const workerParams = reactive({
  teamId,
  pageIndex: 1,
  pageSize: 10
})

// 项目列表
const projects = ref<any[]>([])
const projectsLoading = ref(false)

// 公司选项
const companyOptions = ref<{ label: string; value: string }[]>([])
const loadingCompanies = ref(false)

// 班组类型选项
const teamTypeOptions = [
  { label: '普通', value: 'General' },
  { label: '施工', value: 'Construction' },
  { label: '电气', value: 'Electrical' },
  { label: '管道', value: 'Plumbing' },
  { label: '暖通', value: 'HVAC' },
  { label: '砌筑', value: 'Masonry' },
  { label: '木工', value: 'Carpentry' },
  { label: '油漆', value: 'Painting' },
  { label: '管理', value: 'Management' },
  { label: '其他', value: 'Other' }
]

// 表单相关
const dialogVisible = ref(false)
const formLoading = ref(false)
const formData = reactive({
  id: '',
  companyId: '',
  name: '',
  leader: '',
  leaderPhone: '',
  type: 'General',
  isActive: true
})

// 表单校验规则
const formRules = {
  name: [
    { required: true, message: '请输入班组名称', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  leader: [
    { max: 50, message: '长度不能超过 50 个字符', trigger: 'blur' }
  ],
  leaderPhone: [
    { max: 20, message: '长度不能超过 20 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择班组类型', trigger: 'change' }
  ]
}

// 添加工人相关
const addWorkerDialogVisible = ref(false)
const availableWorkers = ref<any[]>([])
const availableWorkerTotal = ref(0)
const availableWorkersLoading = ref(false)
const selectedWorkers = ref<any[]>([])
const addingWorkers = ref(false)
const workerSearchParams = reactive({
  keyword: '',
  idCardNumber: '',
  excludeTeamId: teamId,
  pageIndex: 1,
  pageSize: 10
})

// 考勤图表配置
const attendanceChartOptions = computed(() => {
  const dates = stats.value?.attendanceStats?.map((item: any) => item.date) || []
  const attendanceData = stats.value?.attendanceStats?.map((item: any) => item.count) || []
  const rateData = stats.value?.attendanceStats?.map((item: any) => item.rate) || []

  return {
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    legend: {
      data: ['出勤人数', '出勤率']
    },
    grid: {
      left: '3%',
      right: '4%',
      bottom: '3%',
      containLabel: true
    },
    xAxis: [
      {
        type: 'category',
        data: dates
      }
    ],
    yAxis: [
      {
        type: 'value',
        name: '人数',
        min: 0,
        axisLabel: {
          formatter: '{value} 人'
        }
      },
      {
        type: 'value',
        name: '出勤率',
        min: 0,
        max: 100,
        axisLabel: {
          formatter: '{value} %'
        }
      }
    ],
    series: [
      {
        name: '出勤人数',
        type: 'bar',
        data: attendanceData
      },
      {
        name: '出勤率',
        type: 'line',
        yAxisIndex: 1,
        data: rateData
      }
    ]
  }
})

// 工种分布图表配置
const workerTypeChartOptions = computed(() => {
  const typeData = stats.value?.workerTypeStats || []

  return {
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'vertical',
      right: 10,
      top: 'center',
      data: typeData.map((item: any) => getWorkerTypeName(item.type))
    },
    series: [
      {
        name: '工种分布',
        type: 'pie',
        radius: ['50%', '70%'],
        avoidLabelOverlap: false,
        itemStyle: {
          borderRadius: 10,
          borderColor: '#fff',
          borderWidth: 2
        },
        label: {
          show: false,
          position: 'center'
        },
        emphasis: {
          label: {
            show: true,
            fontSize: '14',
            fontWeight: 'bold'
          }
        },
        labelLine: {
          show: false
        },
        data: typeData.map((item: any) => ({
          name: getWorkerTypeName(item.type),
          value: item.count
        }))
      }
    ]
  }
})

// 初始化
onMounted(() => {
  fetchCompanies()
  fetchTeamDetail()
  fetchTeamStats()
  fetchWorkers()
  fetchProjects()
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

// 获取班组详情
const fetchTeamDetail = async () => {
  loading.value = true
  try {
    const { data } = await teamService.getById(teamId)
    team.value = data
  } catch (error) {
    console.error('获取班组详情失败:', error)
    ElMessage.error('获取班组详情失败')
  } finally {
    loading.value = false
  }
}

// 获取班组统计信息
const fetchTeamStats = async () => {
  statsLoading.value = true
  try {
    const { data } = await teamService.getStats(teamId)
    stats.value = data
  } catch (error) {
    console.error('获取班组统计信息失败:', error)
  } finally {
    statsLoading.value = false
  }
}

// 获取工人列表
const fetchWorkers = async () => {
  workersLoading.value = true
  try {
    const { data } = await workerService.getByTeam(teamId, workerParams)
    workers.value = data.items
    workerTotal.value = data.total
  } catch (error) {
    console.error('获取工人列表失败:', error)
  } finally {
    workersLoading.value = false
  }
}

// 获取项目列表
const fetchProjects = async () => {
  projectsLoading.value = true
  try {
    const { data } = await teamService.getProjects(teamId)
    projects.value = data
  } catch (error) {
    console.error('获取项目列表失败:', error)
  } finally {
    projectsLoading.value = false
  }
}

// 返回上一页
const goBack = () => {
  router.back()
}

// 编辑班组
const handleEdit = () => {
  Object.assign(formData, team.value)
  dialogVisible.value = true
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    const { data } = await teamService.update(values.id, values)
    if (data) {
      ElMessage.success('更新成功')
      dialogVisible.value = false
      fetchTeamDetail()
    }
  } catch (error) {
    console.error('更新失败:', error)
    ElMessage.error('更新失败')
  } finally {
    formLoading.value = false
  }
}

// 工人页码变化
const handleWorkerPageChange = (page: number) => {
  workerParams.pageIndex = page
  fetchWorkers()
}

// 工人每页条数变化
const handleWorkerSizeChange = (size: number) => {
  workerParams.pageSize = size
  workerParams.pageIndex = 1
  fetchWorkers()
}

// 查看工人
const viewWorker = (row: any) => {
  router.push(`/worker/detail/${row.id}`)
}

// 查看项目
const viewProject = (row: any) => {
  router.push(`/project/detail/${row.id}`)
}

// 添加工人
const handleAddWorker = () => {
  resetWorkerSearch()
  addWorkerDialogVisible.value = true
  searchAvailableWorkers()
}

// 搜索可用工人
const searchAvailableWorkers = async () => {
  availableWorkersLoading.value = true
  try {
    const { data } = await workerService.getAvailable(workerSearchParams)
    availableWorkers.value = data.items
    availableWorkerTotal.value = data.total
  } catch (error) {
    console.error('获取可用工人失败:', error)
  } finally {
    availableWorkersLoading.value = false
  }
}

// 重置工人搜索
const resetWorkerSearch = () => {
  Object.assign(workerSearchParams, {
    keyword: '',
    idCardNumber: '',
    excludeTeamId: teamId,
    pageIndex: 1,
    pageSize: 10
  })
  selectedWorkers.value = []
}

// 可用工人页码变化
const handleAvailableWorkerPageChange = (page: number) => {
  workerSearchParams.pageIndex = page
  searchAvailableWorkers()
}

// 可用工人每页条数变化
const handleAvailableWorkerSizeChange = (size: number) => {
  workerSearchParams.pageSize = size
  workerSearchParams.pageIndex = 1
  searchAvailableWorkers()
}

// 工人选择变化
const handleWorkerSelectionChange = (selection: any[]) => {
  selectedWorkers.value = selection
}

// 确认添加工人
const confirmAddWorkers = async () => {
  if (selectedWorkers.value.length === 0) {
    ElMessage.warning('请选择要添加的工人')
    return
  }

  addingWorkers.value = true
  try {
    const workerIds = selectedWorkers.value.map(worker => worker.id)
    const { data } = await teamService.addWorkers(teamId, workerIds)
    if (data) {
      ElMessage.success(`成功添加 ${workerIds.length} 名工人`)
      addWorkerDialogVisible.value = false
      fetchWorkers()
      fetchTeamStats()
    }
  } catch (error) {
    console.error('添加工人失败:', error)
    ElMessage.error('添加工人失败')
  } finally {
    addingWorkers.value = false
  }
}

// 移除工人
const handleRemoveWorker = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要将工人"${row.name}"从班组中移除吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    const { data } = await teamService.removeWorker(teamId, row.id)
    if (data) {
      ElMessage.success('移除成功')
      fetchWorkers()
      fetchTeamStats()
    }
  } catch (error) {
    console.error('移除工人失败:', error)
  }
}

// 获取班组类型名称
const getTeamTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'General': '普通',
    'Construction': '施工',
    'Electrical': '电气',
    'Plumbing': '管道',
    'HVAC': '暖通',
    'Masonry': '砌筑',
    'Carpentry': '木工',
    'Painting': '油漆',
    'Management': '管理',
    'Other': '其他'
  }
  return typeMap[type] || type
}

// 获取工人类型名称
const getWorkerTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'General': '普通工',
    'Skilled': '技术工',
    'Technician': '技师',
    'Engineer': '工程师',
    'Supervisor': '监理',
    'Manager': '管理人员',
    'Other': '其他'
  }
  return typeMap[type] || type
}
</script>

<style lang="scss" scoped>
.team-detail-container {
  padding: 20px;

  .mt-20 {
    margin-top: 20px;
  }

  .dialog-footer {
    padding: 20px 0 0;
    text-align: right;
  }
}
</style>
