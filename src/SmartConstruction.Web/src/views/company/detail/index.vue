<template>
  <div class="company-detail-container">
    <page-header :title="company.name || '公司详情'" :subtitle="company.code">
      <template #actions>
        <el-button @click="goBack">
          <el-icon><back /></el-icon>
          返回
        </el-button>
        <el-button type="primary" @click="handleEdit" v-permission="'company.edit'">
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
            <el-descriptions-item label="公司编码">{{ company.code }}</el-descriptions-item>
            <el-descriptions-item label="公司名称">{{ company.name }}</el-descriptions-item>
            <el-descriptions-item label="所属租户" v-if="showTenantInfo">{{ company.tenantName }}</el-descriptions-item>
            <el-descriptions-item label="统一社会信用代码" :span="showTenantInfo ? 1 : 2">{{ company.unifiedSocialCreditCode }}</el-descriptions-item>
            <el-descriptions-item label="法人代表">{{ company.legalPerson }}</el-descriptions-item>
            <el-descriptions-item label="联系电话">{{ company.contactPhone }}</el-descriptions-item>
            <el-descriptions-item label="地址" :span="2">{{ company.address }}</el-descriptions-item>
            <el-descriptions-item label="公司类型">{{ getCompanyTypeName(company.type) }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <status-tag
                :status="company.isActive"
                :status-map="{
                  true: { type: 'success', label: '启用' },
                  false: { type: 'danger', label: '禁用' }
                }"
              />
            </el-descriptions-item>
            <el-descriptions-item label="创建时间">{{ formatDateTime(company.createdTime) }}</el-descriptions-item>
            <el-descriptions-item label="更新时间">{{ formatDateTime(company.updatedTime) }}</el-descriptions-item>
          </el-descriptions>
        </detail-card>
        
        <!-- 项目列表 -->
        <detail-card title="项目列表" :loading="projectsLoading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleAddProject" v-permission="'project.create'">
              <el-icon><plus /></el-icon>
              添加项目
            </el-button>
          </template>
          
          <el-table :data="projects" border stripe>
            <el-table-column prop="code" label="项目编码" min-width="120" />
            <el-table-column prop="name" label="项目名称" min-width="150" />
            <el-table-column prop="address" label="项目地址" min-width="200" show-overflow-tooltip />
            <el-table-column prop="projectManager" label="项目经理" min-width="120" />
            <el-table-column prop="managerPhone" label="联系电话" min-width="120" />
            <el-table-column prop="startDate" label="开始日期" min-width="120">
              <template #default="{ row }">
                {{ formatDate(row.startDate) }}
              </template>
            </el-table-column>
            <el-table-column prop="status" label="状态" width="100" align="center">
              <template #default="{ row }">
                <status-tag
                  :status="row.status"
                  :status-map="{
                    'Planning': { type: 'info', label: '规划中' },
                    'InProgress': { type: 'primary', label: '进行中' },
                    'Completed': { type: 'success', label: '已完成' },
                    'OnHold': { type: 'warning', label: '已暂停' },
                    'Cancelled': { type: 'danger', label: '已取消' }
                  }"
                />
              </template>
            </el-table-column>
            <el-table-column label="操作" width="150" align="center">
              <template #default="{ row }">
                <el-button type="primary" link @click="viewProject(row)">
                  查看
                </el-button>
                <el-button type="primary" link @click="editProject(row)" v-permission="'project.edit'">
                  编辑
                </el-button>
              </template>
            </el-table-column>
          </el-table>
          
          <template #footer>
            <el-pagination
              v-model:current-page="projectParams.pageIndex"
              v-model:page-size="projectParams.pageSize"
              :page-sizes="[5, 10, 20, 50]"
              :total="projectTotal"
              layout="total, sizes, prev, pager, next"
              @size-change="handleProjectSizeChange"
              @current-change="handleProjectPageChange"
            />
          </template>
        </detail-card>
        
        <!-- 班组列表 -->
        <detail-card title="班组列表" :loading="teamsLoading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleAddTeam" v-permission="'team.create'">
              <el-icon><plus /></el-icon>
              添加班组
            </el-button>
          </template>
          
          <el-table :data="teams" border stripe>
            <el-table-column prop="name" label="班组名称" min-width="150" />
            <el-table-column prop="leader" label="班组长" min-width="120" />
            <el-table-column prop="leaderPhone" label="联系电话" min-width="120" />
            <el-table-column prop="type" label="类型" width="100">
              <template #default="{ row }">
                {{ getTeamTypeName(row.type) }}
              </template>
            </el-table-column>
            <el-table-column prop="workerCount" label="工人数量" width="100" align="center" />
            <el-table-column prop="isActive" label="状态" width="80" align="center">
              <template #default="{ row }">
                <status-tag
                  :status="row.isActive"
                  :status-map="{
                    true: { type: 'success', label: '启用' },
                    false: { type: 'danger', label: '禁用' }
                  }"
                />
              </template>
            </el-table-column>
            <el-table-column label="操作" width="150" align="center">
              <template #default="{ row }">
                <el-button type="primary" link @click="viewTeam(row)">
                  查看
                </el-button>
                <el-button type="primary" link @click="editTeam(row)" v-permission="'team.edit'">
                  编辑
                </el-button>
              </template>
            </el-table-column>
          </el-table>
          
          <template #footer>
            <el-pagination
              v-model:current-page="teamParams.pageIndex"
              v-model:page-size="teamParams.pageSize"
              :page-sizes="[5, 10, 20, 50]"
              :total="teamTotal"
              layout="total, sizes, prev, pager, next"
              @size-change="handleTeamSizeChange"
              @current-change="handleTeamPageChange"
            />
          </template>
        </detail-card>
      </el-col>
      
      <el-col :xs="24" :md="8">
        <!-- 统计信息 -->
        <detail-card title="统计信息" :loading="statsLoading">
          <el-row :gutter="20">
            <el-col :span="12">
              <stat-card
                title="项目数量"
                :value="stats.projectCount || 0"
                icon="Document"
                icon-bg-color="#409EFF"
              />
            </el-col>
            <el-col :span="12">
              <stat-card
                title="班组数量"
                :value="stats.teamCount || 0"
                icon="User"
                icon-bg-color="#67C23A"
              />
            </el-col>
          </el-row>
          
          <el-row :gutter="20" class="mt-20">
            <el-col :span="12">
              <stat-card
                title="工人数量"
                :value="stats.workerCount || 0"
                icon="User"
                icon-bg-color="#E6A23C"
              />
            </el-col>
            <el-col :span="12">
              <stat-card
                title="设备数量"
                :value="stats.deviceCount || 0"
                icon="Monitor"
                icon-bg-color="#F56C6C"
              />
            </el-col>
          </el-row>
        </detail-card>
        
        <!-- 项目进度 -->
        <detail-card title="项目进度" :loading="statsLoading">
          <div v-if="projectProgress.length > 0">
            <div v-for="(item, index) in projectProgress" :key="index" class="progress-item">
              <div class="progress-header">
                <span class="progress-name">{{ item.name }}</span>
                <span class="progress-value">{{ item.progress }}%</span>
              </div>
              <el-progress
                :percentage="item.progress"
                :color="getProgressColor(item.progress)"
                :stroke-width="10"
              />
            </div>
          </div>
          <el-empty v-else description="暂无项目进度数据" />
        </detail-card>
        
        <!-- 安全统计 -->
        <detail-card title="安全统计" :loading="statsLoading">
          <el-row :gutter="20">
            <el-col :span="12">
              <stat-card
                title="安全事件"
                :value="stats.safetyIncidentCount || 0"
                icon="Warning"
                icon-bg-color="#E6A23C"
              />
            </el-col>
            <el-col :span="12">
              <stat-card
                title="未处理事件"
                :value="stats.unresolvedIncidentCount || 0"
                icon="Warning"
                icon-bg-color="#F56C6C"
              />
            </el-col>
          </el-row>
          
          <div class="mt-20">
            <chart-card
              title="事件级别分布"
              :options="safetyChartOptions"
              height="200px"
              :loading="statsLoading"
            />
          </div>
        </detail-card>
      </el-col>
    </el-row>
    
    <!-- 公司表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      title="编辑公司"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="600px"
      @submit="handleSubmit"
    >
      <el-form-item label="公司编码" prop="code">
        <el-input
          v-model="formData.code"
          placeholder="请输入公司编码"
          disabled
        />
      </el-form-item>
      
      <el-form-item label="公司名称" prop="name">
        <el-input
          v-model="formData.name"
          placeholder="请输入公司名称"
        />
      </el-form-item>
      
      <el-form-item label="统一社会信用代码" prop="unifiedSocialCreditCode">
        <el-input
          v-model="formData.unifiedSocialCreditCode"
          placeholder="请输入统一社会信用代码"
        />
      </el-form-item>
      
      <el-form-item label="法人代表" prop="legalPerson">
        <el-input
          v-model="formData.legalPerson"
          placeholder="请输入法人代表"
        />
      </el-form-item>
      
      <el-form-item label="联系电话" prop="contactPhone">
        <el-input
          v-model="formData.contactPhone"
          placeholder="请输入联系电话"
        />
      </el-form-item>
      
      <el-form-item label="地址" prop="address">
        <el-input
          v-model="formData.address"
          placeholder="请输入地址"
          type="textarea"
          :rows="2"
        />
      </el-form-item>
      
      <el-form-item label="公司类型" prop="type">
        <el-select
          v-model="formData.type"
          placeholder="请选择公司类型"
        >
          <el-option
            v-for="item in companyTypeOptions"
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
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Back, Edit, Plus } from '@element-plus/icons-vue'
import { companyService, projectService, teamService } from '@/api/services'
import { formatDate, formatDateTime } from '@/utils/format'
import { useUserStore } from '@/stores/user'
import PageHeader from '@/components/PageHeader.vue'
import DetailCard from '@/components/DetailCard.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'
import StatCard from '@/components/StatCard.vue'
import ChartCard from '@/components/ChartCard.vue'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const companyId = route.params.id as string

// 是否显示租户信息
const showTenantInfo = computed(() => {
  return userStore.isSuperAdmin || userStore.isSystemAdmin
})

// 公司信息
const company = ref<any>({})
const loading = ref(false)

// 统计信息
const stats = ref<any>({})
const statsLoading = ref(false)

// 项目列表
const projects = ref<any[]>([])
const projectTotal = ref(0)
const projectsLoading = ref(false)
const projectParams = reactive({
  companyId,
  pageIndex: 1,
  pageSize: 5
})

// 班组列表
const teams = ref<any[]>([])
const teamTotal = ref(0)
const teamsLoading = ref(false)
const teamParams = reactive({
  companyId,
  pageIndex: 1,
  pageSize: 5
})

// 项目进度
const projectProgress = ref<{ name: string; progress: number }[]>([])

// 安全统计图表配置
const safetyChartOptions = computed(() => {
  return {
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'horizontal',
      bottom: 0,
      data: ['低风险', '中风险', '高风险', '严重']
    },
    series: [
      {
        name: '安全事件',
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
          { value: stats.value?.lowRiskIncidentCount || 0, name: '低风险', itemStyle: { color: '#67C23A' } },
          { value: stats.value?.mediumRiskIncidentCount || 0, name: '中风险', itemStyle: { color: '#E6A23C' } },
          { value: stats.value?.highRiskIncidentCount || 0, name: '高风险', itemStyle: { color: '#F56C6C' } },
          { value: stats.value?.criticalIncidentCount || 0, name: '严重', itemStyle: { color: '#FF0000' } }
        ]
      }
    ]
  }
})

// 公司类型选项
const companyTypeOptions = [
  { label: '普通', value: 'General' },
  { label: '建筑', value: 'Construction' },
  { label: '供应商', value: 'Supplier' },
  { label: '分包商', value: 'Subcontractor' },
  { label: '咨询', value: 'Consultant' },
  { label: '其他', value: 'Other' }
]

// 表单相关
const dialogVisible = ref(false)
const formLoading = ref(false)
const formData = reactive({
  id: '',
  tenantId: '',
  code: '',
  name: '',
  unifiedSocialCreditCode: '',
  legalPerson: '',
  contactPhone: '',
  address: '',
  type: 'General',
  isActive: true
})

// 表单校验规则
const formRules = {
  name: [
    { required: true, message: '请输入公司名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  unifiedSocialCreditCode: [
    { max: 50, message: '长度不能超过 50 个字符', trigger: 'blur' }
  ],
  legalPerson: [
    { max: 50, message: '长度不能超过 50 个字符', trigger: 'blur' }
  ],
  contactPhone: [
    { max: 20, message: '长度不能超过 20 个字符', trigger: 'blur' }
  ],
  address: [
    { max: 200, message: '长度不能超过 200 个字符', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择公司类型', trigger: 'change' }
  ]
}

// 初始化
onMounted(() => {
  fetchCompanyDetail()
  fetchCompanyStats()
  fetchProjects()
  fetchTeams()
  
  // 如果URL中有edit参数，则自动打开编辑对话框
  if (route.query.edit === 'true') {
    handleEdit()
  }
})

// 获取公司详情
const fetchCompanyDetail = async () => {
  loading.value = true
  try {
    const { data } = await companyService.getById(companyId)
    company.value = data
  } catch (error) {
    console.error('获取公司详情失败:', error)
    ElMessage.error('获取公司详情失败')
  } finally {
    loading.value = false
  }
}

// 获取公司统计信息
const fetchCompanyStats = async () => {
  statsLoading.value = true
  try {
    const { data } = await companyService.getStats(companyId)
    stats.value = data
    
    // 获取项目进度
    if (data.projectProgress && data.projectProgress.length > 0) {
      projectProgress.value = data.projectProgress.map((item: any) => ({
        name: item.name,
        progress: item.progress
      }))
    }
  } catch (error) {
    console.error('获取公司统计信息失败:', error)
  } finally {
    statsLoading.value = false
  }
}

// 获取项目列表
const fetchProjects = async () => {
  projectsLoading.value = true
  try {
    const { data } = await projectService.getByCompany(companyId, projectParams)
    projects.value = data.items
    projectTotal.value = data.total
  } catch (error) {
    console.error('获取项目列表失败:', error)
  } finally {
    projectsLoading.value = false
  }
}

// 获取班组列表
const fetchTeams = async () => {
  teamsLoading.value = true
  try {
    const { data } = await teamService.getByCompany(companyId, teamParams)
    teams.value = data.items
    teamTotal.value = data.total
  } catch (error) {
    console.error('获取班组列表失败:', error)
  } finally {
    teamsLoading.value = false
  }
}

// 返回上一页
const goBack = () => {
  router.back()
}

// 编辑公司
const handleEdit = () => {
  Object.assign(formData, company.value)
  dialogVisible.value = true
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    const { data } = await companyService.update(values.id, values)
    if (data) {
      ElMessage.success('更新成功')
      dialogVisible.value = false
      fetchCompanyDetail()
    }
  } catch (error) {
    console.error('更新失败:', error)
    ElMessage.error('更新失败')
  } finally {
    formLoading.value = false
  }
}

// 添加项目
const handleAddProject = () => {
  router.push({
    path: '/project/list',
    query: { companyId }
  })
}

// 查看项目
const viewProject = (row: any) => {
  router.push(`/project/detail/${row.id}`)
}

// 编辑项目
const editProject = (row: any) => {
  router.push({
    path: `/project/detail/${row.id}`,
    query: { edit: 'true' }
  })
}

// 项目页码变化
const handleProjectPageChange = (page: number) => {
  projectParams.pageIndex = page
  fetchProjects()
}

// 项目每页条数变化
const handleProjectSizeChange = (size: number) => {
  projectParams.pageSize = size
  projectParams.pageIndex = 1
  fetchProjects()
}

// 添加班组
const handleAddTeam = () => {
  router.push({
    path: '/team/list',
    query: { companyId }
  })
}

// 查看班组
const viewTeam = (row: any) => {
  router.push(`/team/detail/${row.id}`)
}

// 编辑班组
const editTeam = (row: any) => {
  router.push({
    path: `/team/detail/${row.id}`,
    query: { edit: 'true' }
  })
}

// 班组页码变化
const handleTeamPageChange = (page: number) => {
  teamParams.pageIndex = page
  fetchTeams()
}

// 班组每页条数变化
const handleTeamSizeChange = (size: number) => {
  teamParams.pageSize = size
  teamParams.pageIndex = 1
  fetchTeams()
}

// 获取公司类型名称
const getCompanyTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    'General': '普通',
    'Construction': '建筑',
    'Supplier': '供应商',
    'Subcontractor': '分包商',
    'Consultant': '咨询',
    'Other': '其他'
  }
  return typeMap[type] || type
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

// 获取进度颜色
const getProgressColor = (progress: number) => {
  if (progress >= 80) return '#67C23A'
  if (progress >= 50) return '#409EFF'
  if (progress >= 30) return '#E6A23C'
  return '#F56C6C'
}
</script>

<style lang="scss" scoped>
.company-detail-container {
  padding: 20px;
  
  .mt-20 {
    margin-top: 20px;
  }
  
  .progress-item {
    margin-bottom: 15px;
    
    .progress-header {
      display: flex;
      justify-content: space-between;
      margin-bottom: 5px;
      
      .progress-name {
        font-size: 14px;
        color: var(--el-text-color-primary);
      }
      
      .progress-value {
        font-size: 14px;
        font-weight: bold;
        color: var(--el-text-color-primary);
      }
    }
  }
}
</style>