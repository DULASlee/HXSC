<template>
  <div class="company-list-container">
    <page-header title="公司管理" subtitle="管理系统中的公司信息">
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'company.create'">
          <el-icon><plus /></el-icon>
          新增公司
        </el-button>
      </template>
    </page-header>
    
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="公司名称" prop="keyword">
        <el-input
          v-model="searchParams.keyword"
          placeholder="请输入公司名称或编码"
          clearable
        />
      </el-form-item>
      
      <el-form-item label="租户" prop="tenantId" v-if="showTenantFilter">
        <el-select
          v-model="searchParams.tenantId"
          placeholder="请选择租户"
          clearable
          filterable
        >
          <el-option
            v-for="item in tenantOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="公司类型" prop="type">
        <el-select
          v-model="searchParams.type"
          placeholder="请选择公司类型"
          clearable
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
        <el-select
          v-model="searchParams.isActive"
          placeholder="请选择状态"
          clearable
        >
          <el-option label="启用" :value="true" />
          <el-option label="禁用" :value="false" />
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
      <el-table-column prop="code" label="公司编码" min-width="120" />
      <el-table-column prop="name" label="公司名称" min-width="150" />
      <el-table-column prop="tenantName" label="所属租户" min-width="150" v-if="showTenantColumn" />
      <el-table-column prop="unifiedSocialCreditCode" label="统一社会信用代码" min-width="200" show-overflow-tooltip />
      <el-table-column prop="legalPerson" label="法人代表" min-width="120" />
      <el-table-column prop="contactPhone" label="联系电话" min-width="120" />
      <el-table-column prop="type" label="类型" width="100">
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
          v-permission="'company.view'"
        >
          查看
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleEdit(row)"
          v-permission="'company.edit'"
        >
          编辑
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleToggleStatus(row)"
          v-permission="'company.edit'"
        >
          {{ row.isActive ? '禁用' : '启用' }}
        </el-button>
        <el-button
          type="danger"
          link
          @click="handleDelete(row)"
          v-permission="'company.delete'"
        >
          删除
        </el-button>
      </template>
    </data-table>
    
    <!-- 公司表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      :title="dialogTitle"
      :model="formData"
      :rules="formRules"
      :loading="formLoading"
      width="600px"
      @submit="handleSubmit"
    >
      <el-form-item label="所属租户" prop="tenantId" v-if="showTenantSelect">
        <el-select
          v-model="formData.tenantId"
          placeholder="请选择租户"
          filterable
          :disabled="formType === 'edit'"
        >
          <el-option
            v-for="item in tenantOptions"
            :key="item.value"
            :label="item.label"
            :value="item.value"
          />
        </el-select>
      </el-form-item>
      
      <el-form-item label="公司编码" prop="code">
        <el-input
          v-model="formData.code"
          placeholder="请输入公司编码"
          :disabled="formType === 'edit'"
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
      
      <el-form-item label="状态" prop="isActive" v-if="formType === 'edit'">
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
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { companyService, tenantService } from '@/api/services'
import { formatDate, formatDateTime } from '@/utils/format'
import { useUserStore } from '@/stores/user'
import PageHeader from '@/components/PageHeader.vue'
import SearchForm from '@/components/SearchForm.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()

// 是否显示租户相关字段
const showTenantFilter = computed(() => {
  return userStore.isSuperAdmin || userStore.isSystemAdmin
})

const showTenantColumn = computed(() => {
  return userStore.isSuperAdmin || userStore.isSystemAdmin
})

const showTenantSelect = computed(() => {
  return userStore.isSuperAdmin || userStore.isSystemAdmin
})

// 搜索参数
const searchParams = reactive({
  keyword: '',
  tenantId: route.query.tenantId as string || undefined,
  type: undefined,
  isActive: undefined,
  pageIndex: 1,
  pageSize: 10
})

// 表格数据
const tableData = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 租户选项
const tenantOptions = ref<{ label: string; value: string }[]>([])
const loadingTenants = ref(false)

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
const formType = ref<'create' | 'edit'>('create')
const formLoading = ref(false)
const formData = reactive({
  id: '',
  tenantId: userStore.currentTenant?.id || '',
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
  tenantId: [
    { required: true, message: '请选择租户', trigger: 'change' }
  ],
  code: [
    { required: true, message: '请输入公司编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
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

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增公司' : '编辑公司'
})

// 组件引用
const searchFormRef = ref()
const tableRef = ref()

// 初始化
onMounted(() => {
  if (showTenantFilter.value) {
    fetchTenants()
  }
  fetchData()
})

// 获取租户列表
const fetchTenants = async () => {
  loadingTenants.value = true
  try {
    const { data } = await tenantService.getAll()
    tenantOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id
    }))
  } catch (error) {
    console.error('获取租户列表失败:', error)
  } finally {
    loadingTenants.value = false
  }
}

// 获取数据
const fetchData = async () => {
  loading.value = true
  try {
    const { data } = await companyService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取公司列表失败:', error)
    ElMessage.error('获取公司列表失败')
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
  
  // 如果URL中有tenantId参数，则设置为默认值
  if (route.query.tenantId) {
    formData.tenantId = route.query.tenantId as string
  } else if (userStore.currentTenant?.id) {
    formData.tenantId = userStore.currentTenant.id
  }
  
  dialogVisible.value = true
}

// 查看
const handleView = (row: any) => {
  // 跳转到详情页
  router.push(`/company/detail/${row.id}`)
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
      `确定要${row.isActive ? '禁用' : '启用'}该公司吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await companyService.updateStatus(row.id, !row.isActive)
    if (data) {
      ElMessage.success(`${row.isActive ? '禁用' : '启用'}成功`)
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
      `确定要删除公司"${row.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await companyService.delete(row.id)
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
      // 检查公司编码是否存在
      const { data: codeExists } = await companyService.checkCode(values.code)
      if (codeExists) {
        ElMessage.error('公司编码已存在')
        formLoading.value = false
        return
      }
      
      // 检查公司名称是否存在
      const { data: nameExists } = await companyService.checkName(values.name)
      if (nameExists) {
        ElMessage.error('公司名称已存在')
        formLoading.value = false
        return
      }
      
      // 创建公司
      const { data } = await companyService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      // 更新公司
      const { data } = await companyService.update(values.id, values)
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
    tenantId: userStore.currentTenant?.id || '',
    code: '',
    name: '',
    unifiedSocialCreditCode: '',
    legalPerson: '',
    contactPhone: '',
    address: '',
    type: 'General',
    isActive: true
  })
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
</script>

<style lang="scss" scoped>
.company-list-container {
  padding: 20px;
}
</style>