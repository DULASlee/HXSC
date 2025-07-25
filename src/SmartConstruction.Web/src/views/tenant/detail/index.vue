<template>
  <div class="tenant-detail-container">
    <page-header :title="tenant.name || '租户详情'" :subtitle="tenant.code">
      <template #actions>
        <el-button @click="goBack">
          <el-icon><back /></el-icon>
          返回
        </el-button>
        <el-button type="primary" @click="handleEdit" v-permission="'tenant.edit'">
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
            <el-descriptions-item label="租户编码">{{ tenant.code }}</el-descriptions-item>
            <el-descriptions-item label="租户名称">{{ tenant.name }}</el-descriptions-item>
            <el-descriptions-item label="联系人">{{ tenant.contactPerson }}</el-descriptions-item>
            <el-descriptions-item label="联系电话">{{ tenant.contactPhone }}</el-descriptions-item>
            <el-descriptions-item label="地址" :span="2">{{ tenant.address }}</el-descriptions-item>
            <el-descriptions-item label="描述" :span="2">{{ tenant.description }}</el-descriptions-item>
            <el-descriptions-item label="过期时间">{{ formatDateTime(tenant.expireDate) }}</el-descriptions-item>
            <el-descriptions-item label="状态">
              <status-tag
                :status="tenant.isActive"
                :status-map="{
                  true: { type: 'success', label: '启用' },
                  false: { type: 'danger', label: '禁用' }
                }"
              />
            </el-descriptions-item>
            <el-descriptions-item label="创建时间">{{ formatDateTime(tenant.createdTime) }}</el-descriptions-item>
            <el-descriptions-item label="更新时间">{{ formatDateTime(tenant.updatedTime) }}</el-descriptions-item>
          </el-descriptions>
        </detail-card>
        
        <!-- 公司列表 -->
        <detail-card title="公司列表" :loading="companiesLoading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleAddCompany" v-permission="'company.create'">
              <el-icon><plus /></el-icon>
              添加公司
            </el-button>
          </template>
          
          <el-table :data="companies" border stripe>
            <el-table-column prop="code" label="公司编码" min-width="120" />
            <el-table-column prop="name" label="公司名称" min-width="150" />
            <el-table-column prop="unifiedSocialCreditCode" label="统一社会信用代码" min-width="200" show-overflow-tooltip />
            <el-table-column prop="legalPerson" label="法人代表" min-width="120" />
            <el-table-column prop="contactPhone" label="联系电话" min-width="120" />
            <el-table-column prop="type" label="类型" width="120">
              <template #default="{ row }">
                {{ getCompanyTypeName(row.type) }}
              </template>
            </el-table-column>
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
                <el-button type="primary" link @click="viewCompany(row)">
                  查看
                </el-button>
                <el-button type="primary" link @click="editCompany(row)" v-permission="'company.edit'">
                  编辑
                </el-button>
              </template>
            </el-table-column>
          </el-table>
          
          <template #footer>
            <el-pagination
              v-model:current-page="companyParams.pageIndex"
              v-model:page-size="companyParams.pageSize"
              :page-sizes="[5, 10, 20, 50]"
              :total="companyTotal"
              layout="total, sizes, prev, pager, next"
              @size-change="handleCompanySizeChange"
              @current-change="handleCompanyPageChange"
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
                title="公司数量"
                :value="stats.companyCount || 0"
                icon="OfficeBuilding"
                icon-bg-color="#409EFF"
              />
            </el-col>
            <el-col :span="12">
              <stat-card
                title="项目数量"
                :value="stats.projectCount || 0"
                icon="Document"
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
        
        <!-- 租户配置 -->
        <detail-card title="租户配置" :loading="loading">
          <template #header-actions>
            <el-button type="primary" size="small" @click="handleEditConfig" v-permission="'tenant.config'">
              <el-icon><setting /></el-icon>
              配置
            </el-button>
          </template>
          
          <el-descriptions :column="1" border>
            <el-descriptions-item label="隔离模式">{{ getTenantIsolationMode(tenant.isolationMode) }}</el-descriptions-item>
            <el-descriptions-item label="数据库连接">{{ tenant.dbConnection || '默认' }}</el-descriptions-item>
                            <el-descriptions-item label="存储空间">{{ formatFileSize(tenant.storageQuota) }}</el-descriptions-item>
                <el-descriptions-item label="已使用">{{ formatFileSize(tenant.storageUsed) }}</el-descriptions-item>
            <el-descriptions-item label="用户数上限">{{ tenant.userLimit || '无限制' }}</el-descriptions-item>
            <el-descriptions-item label="当前用户数">{{ stats.userCount || 0 }}</el-descriptions-item>
          </el-descriptions>
        </detail-card>
      </el-col>
    </el-row>
    
    <!-- 租户表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      title="编辑租户"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="600px"
      @submit="handleSubmit"
    >
      <el-form-item label="租户编码" prop="code">
        <el-input
          v-model="formData.code"
          placeholder="请输入租户编码"
          disabled
        />
      </el-form-item>
      
      <el-form-item label="租户名称" prop="name">
        <el-input
          v-model="formData.name"
          placeholder="请输入租户名称"
        />
      </el-form-item>
      
      <el-form-item label="联系人" prop="contactPerson">
        <el-input
          v-model="formData.contactPerson"
          placeholder="请输入联系人"
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
      
      <el-form-item label="描述" prop="description">
        <el-input
          v-model="formData.description"
          placeholder="请输入描述"
          type="textarea"
          :rows="3"
        />
      </el-form-item>
      
      <el-form-item label="过期时间" prop="expireDate">
        <el-date-picker
          v-model="formData.expireDate"
          type="datetime"
          placeholder="请选择过期时间"
          format="YYYY-MM-DD HH:mm:ss"
          value-format="YYYY-MM-DD HH:mm:ss"
        />
      </el-form-item>
      
      <el-form-item label="状态" prop="isActive">
        <el-switch
          v-model="formData.isActive"
          active-text="启用"
          inactive-text="禁用"
        />
      </el-form-item>
    </form-dialog>
    
    <!-- 租户配置对话框 -->
    <form-dialog
      v-model:visible="configDialogVisible"
      title="租户配置"
      :model="configData"
      :rules="configRules"
      :loading="configLoading"
      width="600px"
      @submit="handleConfigSubmit"
    >
      <el-form-item label="隔离模式" prop="isolationMode">
        <el-select
          v-model="configData.isolationMode"
          placeholder="请选择隔离模式"
        >
          <el-option label="共享数据库" value="SharedDatabase" />
          <el-option label="独立架构" value="SeparateSchema" />
          <el-option label="独立数据库" value="SeparateDatabase" />
        </el-select>
      </el-form-item>
      
      <el-form-item label="数据库连接" prop="dbConnection" v-if="configData.isolationMode === 'SeparateDatabase'">
        <el-input
          v-model="configData.dbConnection"
          placeholder="请输入数据库连接字符串"
          type="textarea"
          :rows="2"
        />
      </el-form-item>
      
      <el-form-item label="存储空间(MB)" prop="storageQuota">
        <el-input-number
          v-model="configData.storageQuota"
          :min="0"
          :step="1024"
        />
      </el-form-item>
      
      <el-form-item label="用户数上限" prop="userLimit">
        <el-input-number
          v-model="configData.userLimit"
          :min="0"
          :step="10"
        />
        <div class="form-tip">0表示无限制</div>
      </el-form-item>
    </form-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { Back, Edit, Plus, Setting } from '@element-plus/icons-vue'
import { tenantService } from '@/api/services'
import { formatDate, formatDateTime, formatFileSize } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import DetailCard from '@/components/DetailCard.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'
import StatCard from '@/components/StatCard.vue'

const route = useRoute()
const router = useRouter()
const tenantId = route.params.id as string

// 租户信息
const tenant = ref<any>({})
const loading = ref(false)

// 统计信息
const stats = ref<any>({})
const statsLoading = ref(false)

// 公司列表
const companies = ref<any[]>([])
const companyTotal = ref(0)
const companiesLoading = ref(false)
const companyParams = reactive({
  tenantId,
  pageIndex: 1,
  pageSize: 5
})

// 表单相关
const dialogVisible = ref(false)
const formLoading = ref(false)
const formData = reactive({
  id: '',
  code: '',
  name: '',
  description: '',
  contactPerson: '',
  contactPhone: '',
  address: '',
  expireDate: '',
  isActive: true
})

// 表单校验规则
const formRules = {
  name: [
    { required: true, message: '请输入租户名称', trigger: 'blur' },
    { min: 2, max: 100, message: '长度在 2 到 100 个字符', trigger: 'blur' }
  ],
  contactPerson: [
    { max: 50, message: '长度不能超过 50 个字符', trigger: 'blur' }
  ],
  contactPhone: [
    { max: 20, message: '长度不能超过 20 个字符', trigger: 'blur' }
  ],
  address: [
    { max: 200, message: '长度不能超过 200 个字符', trigger: 'blur' }
  ],
  description: [
    { max: 500, message: '长度不能超过 500 个字符', trigger: 'blur' }
  ]
}

// 配置相关
const configDialogVisible = ref(false)
const configLoading = ref(false)
const configData = reactive({
  id: '',
  isolationMode: 'SharedDatabase',
  dbConnection: '',
  storageQuota: 0,
  userLimit: 0
})

// 配置校验规则
const configRules = {
  isolationMode: [
    { required: true, message: '请选择隔离模式', trigger: 'change' }
  ],
  dbConnection: [
    { required: true, message: '请输入数据库连接字符串', trigger: 'blur', validator: (rule: any, value: string, callback: any) => {
      if (configData.isolationMode === 'SeparateDatabase' && !value) {
        callback(new Error('请输入数据库连接字符串'))
      } else {
        callback()
      }
    }}
  ]
}

// 初始化
onMounted(() => {
  fetchTenantDetail()
  fetchTenantStats()
  fetchCompanies()
})

// 获取租户详情
const fetchTenantDetail = async () => {
  loading.value = true
  try {
    const { data } = await tenantService.getById(tenantId)
    tenant.value = data
  } catch (error) {
    console.error('获取租户详情失败:', error)
    ElMessage.error('获取租户详情失败')
  } finally {
    loading.value = false
  }
}

// 获取租户统计信息
const fetchTenantStats = async () => {
  statsLoading.value = true
  try {
    const { data } = await tenantService.getStats(tenantId)
    stats.value = data
  } catch (error) {
    console.error('获取租户统计信息失败:', error)
  } finally {
    statsLoading.value = false
  }
}

// 获取公司列表
const fetchCompanies = async () => {
  companiesLoading.value = true
  try {
    const { data } = await tenantService.getCompanies(tenantId, companyParams)
    companies.value = data.items
    companyTotal.value = data.total
  } catch (error) {
    console.error('获取公司列表失败:', error)
  } finally {
    companiesLoading.value = false
  }
}

// 返回上一页
const goBack = () => {
  router.back()
}

// 编辑租户
const handleEdit = () => {
  Object.assign(formData, tenant.value)
  dialogVisible.value = true
}

// 提交表单
const handleSubmit = async (values: any) => {
  formLoading.value = true
  try {
    const { data } = await tenantService.update(values.id, values)
    if (data) {
      ElMessage.success('更新成功')
      dialogVisible.value = false
      fetchTenantDetail()
    }
  } catch (error) {
    console.error('更新失败:', error)
    ElMessage.error('更新失败')
  } finally {
    formLoading.value = false
  }
}

// 编辑配置
const handleEditConfig = () => {
  Object.assign(configData, {
    id: tenant.value.id,
    isolationMode: tenant.value.isolationMode || 'SharedDatabase',
    dbConnection: tenant.value.dbConnection || '',
    storageQuota: tenant.value.storageQuota || 0,
    userLimit: tenant.value.userLimit || 0
  })
  configDialogVisible.value = true
}

// 提交配置
const handleConfigSubmit = async (values: any) => {
  configLoading.value = true
  try {
    const { data } = await tenantService.updateConfig(values.id, values)
    if (data) {
      ElMessage.success('配置更新成功')
      configDialogVisible.value = false
      fetchTenantDetail()
    }
  } catch (error) {
    console.error('配置更新失败:', error)
    ElMessage.error('配置更新失败')
  } finally {
    configLoading.value = false
  }
}

// 添加公司
const handleAddCompany = () => {
  router.push({
    path: '/company/list',
    query: { tenantId }
  })
}

// 查看公司
const viewCompany = (row: any) => {
  router.push(`/company/detail/${row.id}`)
}

// 编辑公司
const editCompany = (row: any) => {
  router.push({
    path: `/company/detail/${row.id}`,
    query: { edit: 'true' }
  })
}

// 公司页码变化
const handleCompanyPageChange = (page: number) => {
  companyParams.pageIndex = page
  fetchCompanies()
}

// 公司每页条数变化
const handleCompanySizeChange = (size: number) => {
  companyParams.pageSize = size
  companyParams.pageIndex = 1
  fetchCompanies()
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

// 获取租户隔离模式名称
const getTenantIsolationMode = (mode: string) => {
  const modeMap: Record<string, string> = {
    'SharedDatabase': '共享数据库',
    'SeparateSchema': '独立架构',
    'SeparateDatabase': '独立数据库'
  }
  return modeMap[mode] || mode
}
</script>

<style lang="scss" scoped>
.tenant-detail-container {
  padding: 20px;
  
  .mt-20 {
    margin-top: 20px;
  }
  
  .form-tip {
    font-size: 12px;
    color: var(--el-text-color-secondary);
    margin-top: 5px;
  }
}
</style>