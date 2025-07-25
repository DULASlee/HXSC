<template>
  <div class="project-detail-container">
    <page-header :title="project.name || '项目详情'" :subtitle="project.code">
      <template #actions>
        <el-button @click="goBack">
          <el-icon><back /></el-icon>
          返回
        </el-button>
        <el-button type="primary" @click="handleEdit" v-permission="'project.edit'">
          <el-icon><edit /></el-icon>
          编辑
        </el-button>
        <el-button type="success" @click="viewDashboard">
          <el-icon><data-analysis /></el-icon>
          仪表盘
        </el-button>
      </template>
    </page-header>
    
    <el-row :gutter="20">
      <el-col :xs="24" :md="16">
        <!-- 基本信息 -->
        <detail-card title="基本信息" :loading="loading">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="项目编码">{{ project.code }}</el-descriptions-item>
            <el-descriptions-item label="项目名称">{{ project.name }}</el-descriptions-item>
            <el-descriptions-item label="所属公司">{{ project.companyName }}</el-descriptions-item>
            <el-descriptions-item label="项目状态">
              <status-tag
                :status="project.status"
                :status-map="{
                  'Planning': { type: 'info', label: '规划中' },
                  'InProgress': { type: 'primary', label: '进行中' },
                  'Completed': { type: 'success', label: '已完成' },
                  'OnHold': { type: 'warning', label: '已暂停' },
                  'Cancelled': { type: 'danger', label: '已取消' }
                }"
              />
            </el-descriptions-item>
            <el-descriptions-item label="项目经理">{{ project.projectManager }}</el-descriptions-item>
            <el-descriptions-item label="联系电话">{{ project.managerPhone }}</el-descriptions-item>
            <el-descriptions-item label="项目地址" :span="2">{{ project.address }}</el-descriptions-item>
            <el-descriptions-item label="经纬度">{{ formatLocation(project.longitude, project.latitude) }}</el-descriptions-item>
                          <el-descriptions-item label="总投资">{{ formatCurrency(project.totalInvestment) }}万元</el-descriptions-item>
            <el-descriptions-item label="开始日期">{{ formatDate(project.startDate) }}</el-descriptions-item>
            <el-descriptions-item label="结束日期">{{ formatDate(project.endDate) }}</el-descriptions-item>
            <el-descriptions-item label="项目描述" :span="2">{{ project.description }}</el-descriptions-item>
            <el-descriptions-item label="创建时间">{{ formatDateTime(project.createdTime) }}</el-descriptions-item>
            <el-descriptions-item label="更新时间">{{ formatDateTime(project.updatedTime) }}</el-descriptions-item>
          </el-descriptions>
        </detail-card>
        
        <!-- 项目进度 -->
        <detail-card title="项目进度" :loading="loading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleUpdateProgress" v-permission="'project.edit'">
              <el-icon><edit /></el-icon>
              更新进度
            </el-button>
          </template>
          
          <div class="progress-container">
            <div class="progress-header">
              <span class="progress-title">总体进度</span>
              <span class="progress-value">{{ project.progress }}%</span>
            </div>
            <el-progress
              :percentage="project.progress"
              :color="getProgressColor(project.progress)"
              :stroke-width="15"
            />
          </div>
          
          <div class="milestone-list" v-if="milestones.length > 0">
            <h4>项目里程碑</h4>
            <el-timeline>
              <el-timeline-item
                v-for="(item, index) in milestones"
                :key="index"
                :timestamp="formatDate(item.date)"
                :type="getMilestoneType(item.status)"
                :color="getMilestoneColor(item.status)"
              >
                <h5>{{ item.name }}</h5>
                <p>{{ item.description }}</p>
                <p v-if="item.status === 'Completed'" class="milestone-completed">完成于 {{ formatDate(item.completedDate) }}</p>
              </el-timeline-item>
            </el-timeline>
          </div>
        </detail-card>
        
        <!-- 工人列表 -->
        <detail-card title="工人列表" :loading="workersLoading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleManageWorkers" v-permission="'worker.view'">
              <el-icon><plus /></el-icon>
              管理工人
            </el-button>
          </template>
          
          <el-table :data="workers" border stripe>
            <el-table-column prop="name" label="姓名" min-width="100" />
            <el-table-column prop="idCardNumber" label="身份证号" min-width="180" show-overflow-tooltip />
            <el-table-column prop="teamName" label="所属班组" min-width="120" />
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
                <el-button type="primary" link @click="viewAttendance(row)">
                  考勤
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
      </el-col>
      
      <el-col :xs="24" :md="8">
        <!-- 项目地图 -->
        <detail-card title="项目位置" :loading="loading">
          <div class="map-container" id="projectMap" v-if="hasLocation"></div>
          <el-empty v-else description="暂无位置信息" />
        </detail-card>
        
        <!-- 统计信息 -->
        <detail-card title="统计信息" :loading="statsLoading">
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
                title="设备数量"
                :value="stats.deviceCount || 0"
                icon="Monitor"
                icon-bg-color="#67C23A"
              />
            </el-col>
          </el-row>
          
          <el-row :gutter="20" class="mt-20">
            <el-col :span="12">
              <stat-card
                title="今日出勤"
                :value="stats.todayAttendance || 0"
                icon="Calendar"
                icon-bg-color="#E6A23C"
                :footer="`出勤率: ${stats.attendanceRate || 0}%`"
              />
            </el-col>
            <el-col :span="12">
              <stat-card
                title="安全事件"
                :value="stats.safetyIncidentCount || 0"
                icon="Warning"
                icon-bg-color="#F56C6C"
                :footer="`未处理: ${stats.unresolvedIncidentCount || 0}`"
              />
            </el-col>
          </el-row>
        </detail-card>
        
        <!-- 设备状态 -->
        <detail-card title="设备状态" :loading="statsLoading">
          <div class="device-status-chart">
            <chart-card
              title="设备状态分布"
              :options="deviceStatusChartOptions"
              height="200px"
              :loading="statsLoading"
            />
          </div>
          
          <div class="device-actions mt-20">
            <el-button type="primary" @click="handleManageDevices" v-permission="'device.view'">
              <el-icon><monitor /></el-icon>
              管理设备
            </el-button>
            <el-button type="success" @click="handleMonitorDevices" v-permission="'device.monitor'">
              <el-icon><view /></el-icon>
              设备监控
            </el-button>
          </div>
        </detail-card>
        
        <!-- 安全管理 -->
        <detail-card title="安全管理" :loading="statsLoading">
          <div class="safety-stats">
            <div class="safety-stat-item">
              <div class="safety-stat-label">安全事件</div>
              <div class="safety-stat-value">{{ stats.safetyIncidentCount || 0 }}</div>
            </div>
            <div class="safety-stat-item">
              <div class="safety-stat-label">未处理</div>
              <div class="safety-stat-value">{{ stats.unresolvedIncidentCount || 0 }}</div>
            </div>
            <div class="safety-stat-item">
              <div class="safety-stat-label">高风险</div>
              <div class="safety-stat-value">{{ stats.highRiskIncidentCount || 0 }}</div>
            </div>
            <div class="safety-stat-item">
              <div class="safety-stat-label">已解决</div>
              <div class="safety-stat-value">{{ stats.resolvedIncidentCount || 0 }}</div>
            </div>
          </div>
          
          <div class="safety-actions mt-20">
            <el-button type="primary" @click="handleManageSafety" v-permission="'safety.incident.view'">
              <el-icon><warning /></el-icon>
              安全事件
            </el-button>
            <el-button type="success" @click="handleSafetyStats" v-permission="'safety.statistics'">
              <el-icon><pie-chart /></el-icon>
              安全统计
            </el-button>
          </div>
        </detail-card>
      </el-col>
    </el-row>
    
    <!-- 项目表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      title="编辑项目"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="700px"
      @submit="handleSubmit"
    >
      <el-form-item label="项目编码" prop="code">
        <el-input
          v-model="formData.code"
          placeholder="请输入项目编码"
          disabled
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
      
      <el-form-item label="项目状态" prop="status">
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
    
    <!-- 进度更新对话框 -->
    <form-dialog
      v-model:visible="progressDialogVisible"
      title="更新项目进度"
      :model="progressData"
      :rules="progressRules"
      :loading="progressLoading"
      width="500px"
      @submit="handleProgressSubmit"
    >
      <el-form-item label="当前进度" prop="progress">
        <el-slider
          v-model="progressData.progress"
          :min="0"
          :max="100"
          :step="1"
          :format-tooltip="value => `${value}%`"
          :marks="{
            0: '0%',
            25: '25%',
            50: '50%',
            75: '75%',
            100: '100%'
          }"
        />
      </el-form-item>
      
      <el-form-item label="进度说明" prop="progressNote">
        <el-input
          v-model="progressData.progressNote"
          type="textarea"
          :rows="3"
          placeholder="请输入进度说明"
        />
      </el-form-item>
    </form-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Back, Edit, Plus, DataAnalysis, Monitor, View, Warning, PieChart, Calendar } from '@element-plus/icons-vue'
import { projectService, workerService } from '@/api/services'
import { formatDate, formatDateTime, formatCurrency } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import DetailCard from '@/components/DetailCard.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'
import StatCard from '@/components/StatCard.vue'
import ChartCard from '@/components/ChartCard.vue'

const route = useRoute()
const router = useRouter()
const projectId = route.params.id as string

// 项目信息
const project = ref<any>({})
const loading = ref(false)

// 统计信息
const stats = ref<any>({})
const statsLoading = ref(false)

// 里程碑
const milestones = ref<any[]>([])

// 工人列表
const workers = ref<any[]>([])
const workerTotal = ref(0)
const workersLoading = ref(false)
const workerParams = reactive({
  projectId,
  pageIndex: 1,
  pageSize: 5
})

// 项目状态选项
const projectStatusOptions = [
  { label: '规划中', value: 'Planning' },
  { label: '进行中', value: 'InProgress' },
  { label: '已完成', value: 'Completed' },
  { label: '已暂停', value: 'OnHold' },
  { label: '已取消', value: 'Cancelled' }
]

// 设备状态图表配置
const deviceStatusChartOptions = computed(() => {
  return {
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'horizontal',
      bottom: 0,
      data: ['在线', '离线', '维护中', '故障', '禁用']
    },
    series: [
      {
        name: '设备状态',
        type: 'pie',
        radius: ['40%', '70%'],
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
        data: [
          { value: stats.value?.onlineDeviceCount || 0, name: '在线', itemStyle: { color: '#67C23A' } },
          { value: stats.value?.offlineDeviceCount || 0, name: '离线', itemStyle: { color: '#909399' } },
          { value: stats.value?.maintenanceDeviceCount || 0, name: '维护中', itemStyle: { color: '#E6A23C' } },
          { value: stats.value?.faultDeviceCount || 0, name: '故障', itemStyle: { color: '#F56C6C' } },
          { value: stats.value?.disabledDeviceCount || 0, name: '禁用', itemStyle: { color: '#C0C4CC' } }
        ]
      }
    ]
  }
})

// 表单相关
const dialogVisible = ref(false)
const formLoading = ref(false)
const formData = reactive({
  id: '',
  companyId: '',
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
  status: ''
})

// 表单校验规则
const formRules = {
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
  ],
  status: [
    { required: true, message: '请选择项目状态', trigger: 'change' }
  ]
}

// 进度更新相关
const progressDialogVisible = ref(false)
const progressLoading = ref(false)
const progressData = reactive({
  progress: 0,
  progressNote: ''
})

// 进度更新校验规则
const progressRules = {
  progress: [
    { required: true, message: '请设置项目进度', trigger: 'change' }
  ],
  progressNote: [
    { max: 500, message: '长度不能超过 500 个字符', trigger: 'blur' }
  ]
}

// 地图相关
let map: any = null
const hasLocation = computed(() => {
  return project.value?.longitude && project.value?.latitude
})

// 初始化
onMounted(() => {
  fetchProjectDetail()
  fetchProjectStats()
  fetchMilestones()
  fetchWorkers()
  
  // 如果URL中有edit参数，则自动打开编辑对话框
  if (route.query.edit === 'true') {
    handleEdit()
  }
})

// 清理
onUnmounted(() => {
  if (map) {
    map.destroy()
    map = null
  }
})

// 获取项目详情
const fetchProjectDetail = async () => {
  loading.value = true
  try {
    const { data } = await projectService.getById(projectId)
    project.value = data
    
    // 初始化地图
    if (hasLocation.value) {
      initMap()
    }
  } catch (error) {
    console.error('获取项目详情失败:', error)
    ElMessage.error('获取项目详情失败')
  } finally {
    loading.value = false
  }
}

// 获取项目统计信息
const fetchProjectStats = async () => {
  statsLoading.value = true
  try {
    const { data } = await projectService.getStats(projectId)
    stats.value = data
  } catch (error) {
    console.error('获取项目统计信息失败:', error)
  } finally {
    statsLoading.value = false
  }
}

// 获取项目里程碑
const fetchMilestones = async () => {
  try {
    const { data } = await projectService.getMilestones(projectId)
    milestones.value = data
  } catch (error) {
    console.error('获取项目里程碑失败:', error)
  }
}

// 获取工人列表
const fetchWorkers = async () => {
  workersLoading.value = true
  try {
    const { data } = await workerService.getByProject(projectId, workerParams)
    workers.value = data.items
    workerTotal.value = data.total
  } catch (error) {
    console.error('获取工人列表失败:', error)
  } finally {
    workersLoading.value = false
  }
}

// 初始化地图
const initMap = () => {
  // 这里使用高德地图API，实际项目中可以替换为其他地图API
  if (window.AMap && project.value?.longitude && project.value?.latitude) {
    setTimeout(() => {
      map = new window.AMap.Map('projectMap', {
        zoom: 15,
        center: [project.value.longitude, project.value.latitude]
      })
      
      // 添加标记
      const marker = new window.AMap.Marker({
        position: [project.value.longitude, project.value.latitude],
        title: project.value.name
      })
      
      map.add(marker)
      
      // 添加信息窗体
      const infoWindow = new window.AMap.InfoWindow({
        content: `<div style="padding:10px;">
          <h4>${project.value.name}</h4>
          <p>${project.value.address}</p>
        </div>`,
        offset: new window.AMap.Pixel(0, -30)
      })
      
      marker.on('click', () => {
        infoWindow.open(map, marker.getPosition())
      })
    }, 500)
  }
}

// 返回上一页
const goBack = () => {
  router.back()
}

// 编辑项目
const handleEdit = () => {
  Object.assign(formData, project.value)
  dialogVisible.value = true
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    const { data } = await projectService.update(values.id, values)
    if (data) {
      ElMessage.success('更新成功')
      dialogVisible.value = false
      fetchProjectDetail()
    }
  } catch (error) {
    console.error('更新失败:', error)
    ElMessage.error('更新失败')
  } finally {
    formLoading.value = false
  }
}

// 更新进度
const handleUpdateProgress = () => {
  progressData.progress = project.value.progress || 0
  progressData.progressNote = ''
  progressDialogVisible.value = true
}

// 提交进度更新
const handleProgressSubmit = async (values: any) => {
  progressLoading.value = true
  try {
    const { data } = await projectService.updateProgress(projectId, values.progress, values.progressNote)
    if (data) {
      ElMessage.success('进度更新成功')
      progressDialogVisible.value = false
      fetchProjectDetail()
    }
  } catch (error) {
    console.error('进度更新失败:', error)
    ElMessage.error('进度更新失败')
  } finally {
    progressLoading.value = false
  }
}

// 查看仪表盘
const viewDashboard = () => {
  router.push(`/dashboard/project/${projectId}`)
}

// 管理工人
const handleManageWorkers = () => {
  router.push({
    path: '/worker/list',
    query: { projectId }
  })
}

// 查看工人
const viewWorker = (row: any) => {
  router.push(`/worker/detail/${row.id}`)
}

// 查看考勤
const viewAttendance = (row: any) => {
  router.push({
    path: '/attendance/list',
    query: { workerId: row.id, projectId }
  })
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

// 管理设备
const handleManageDevices = () => {
  router.push({
    path: '/device/list',
    query: { projectId }
  })
}

// 设备监控
const handleMonitorDevices = () => {
  router.push({
    path: '/device/monitor',
    query: { projectId }
  })
}

// 安全事件
const handleManageSafety = () => {
  router.push({
    path: '/safety/incident',
    query: { projectId }
  })
}

// 安全统计
const handleSafetyStats = () => {
  router.push({
    path: '/safety/statistics',
    query: { projectId }
  })
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

// 获取里程碑类型
const getMilestoneType = (status: string) => {
  switch (status) {
    case 'Completed': return 'success'
    case 'InProgress': return 'primary'
    case 'Pending': return 'info'
    case 'Delayed': return 'warning'
    case 'Cancelled': return 'danger'
    default: return 'info'
  }
}

// 获取里程碑颜色
const getMilestoneColor = (status: string) => {
  switch (status) {
    case 'Completed': return '#67C23A'
    case 'InProgress': return '#409EFF'
    case 'Pending': return '#909399'
    case 'Delayed': return '#E6A23C'
    case 'Cancelled': return '#F56C6C'
    default: return '#909399'
  }
}

// 获取进度颜色
const getProgressColor = (progress: number) => {
  if (progress >= 80) return '#67C23A'
  if (progress >= 50) return '#409EFF'
  if (progress >= 30) return '#E6A23C'
  return '#F56C6C'
}

// 格式化位置信息
const formatLocation = (longitude?: number, latitude?: number) => {
  if (!longitude || !latitude) return '暂无'
  return `${longitude.toFixed(6)}, ${latitude.toFixed(6)}`
}
</script>

<style lang="scss" scoped>
.project-detail-container {
  padding: 20px;
  
  .mt-20 {
    margin-top: 20px;
  }
  
  .map-container {
    height: 300px;
    width: 100%;
  }
  
  .progress-container {
    margin-bottom: 20px;
    
    .progress-header {
      display: flex;
      justify-content: space-between;
      margin-bottom: 10px;
      
      .progress-title {
        font-size: 16px;
        font-weight: 500;
        color: var(--el-text-color-primary);
      }
      
      .progress-value {
        font-size: 16px;
        font-weight: bold;
        color: var(--el-text-color-primary);
      }
    }
  }
  
  .milestone-list {
    margin-top: 20px;
    
    h4 {
      margin-top: 0;
      margin-bottom: 15px;
      font-size: 16px;
      font-weight: 500;
      color: var(--el-text-color-primary);
    }
    
    h5 {
      margin: 0;
      font-size: 14px;
      font-weight: 500;
      color: var(--el-text-color-primary);
    }
    
    p {
      margin: 5px 0 0;
      font-size: 13px;
      color: var(--el-text-color-regular);
      
      &.milestone-completed {
        color: #67C23A;
        font-weight: 500;
      }
    }
  }
  
  .device-actions,
  .safety-actions {
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
  
  .safety-stats {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 15px;
    
    .safety-stat-item {
      background-color: var(--el-bg-color-page);
      border-radius: 4px;
      padding: 15px;
      text-align: center;
      
      .safety-stat-label {
        font-size: 14px;
        color: var(--el-text-color-secondary);
        margin-bottom: 5px;
      }
      
      .safety-stat-value {
        font-size: 24px;
        font-weight: bold;
        color: var(--el-text-color-primary);
      }
      
      &:nth-child(1) .safety-stat-value {
        color: var(--el-color-primary);
      }
      
      &:nth-child(2) .safety-stat-value {
        color: var(--el-color-danger);
      }
      
      &:nth-child(3) .safety-stat-value {
        color: var(--el-color-warning);
      }
      
      &:nth-child(4) .safety-stat-value {
        color: var(--el-color-success);
      }
    }
  }
}
</style>