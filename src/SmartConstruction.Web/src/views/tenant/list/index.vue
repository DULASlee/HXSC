<template>
  <div class="tenant-list-container">
    <page-header title="租户管理" subtitle="管理系统中的租户信息">
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'tenant.create'">
          <el-icon><plus /></el-icon>
          新增租户
        </el-button>
      </template>
    </page-header>
    
    <search-form
      ref="searchFormRef"
      :initial-values="searchParams"
      @search="handleSearch"
      @reset="handleReset"
    >
      <el-form-item label="租户名称" prop="keyword">
        <el-input
          v-model="searchParams.keyword"
          placeholder="请输入租户名称或编码"
          clearable
        />
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
      <el-table-column prop="code" label="租户编码" min-width="120" />
      <el-table-column prop="name" label="租户名称" min-width="150" />
      <el-table-column prop="contactPerson" label="联系人" min-width="120" />
      <el-table-column prop="contactPhone" label="联系电话" min-width="120" />
      <el-table-column prop="address" label="地址" min-width="200" show-overflow-tooltip />
      <el-table-column prop="expireDate" label="过期时间" min-width="150">
        <template #default="{ row }">
          {{ formatDate(row.expireDate) }}
        </template>
      </el-table-column>
      <el-table-column prop="isActive" label="状态" width="100" align="center">
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
          v-permission="'tenant.view'"
        >
          查看
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleEdit(row)"
          v-permission="'tenant.edit'"
        >
          编辑
        </el-button>
        <el-button
          type="primary"
          link
          @click="handleToggleStatus(row)"
          v-permission="'tenant.edit'"
        >
          {{ row.isActive ? '禁用' : '启用' }}
        </el-button>
        <el-button
          type="danger"
          link
          @click="handleDelete(row)"
          v-permission="'tenant.delete'"
        >
          删除
        </el-button>
      </template>
    </data-table>
    
    <!-- 租户表单对话框 -->
    <form-dialog
      v-model:visible="dialogVisible"
      :title="dialogTitle"
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
          :disabled="formType === 'edit'"
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
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { tenantService } from '@/api/services'
import { formatDate, formatDateTime } from '@/utils/format'
import PageHeader from '@/components/PageHeader.vue'
import SearchForm from '@/components/SearchForm.vue'
import DataTable from '@/components/DataTable.vue'
import StatusTag from '@/components/StatusTag.vue'
import FormDialog from '@/components/FormDialog.vue'

// 搜索参数
const searchParams = reactive({
  keyword: '',
  isActive: undefined,
  pageIndex: 1,
  pageSize: 10
})

// 表格数据
const tableData = ref<any[]>([])
const total = ref(0)
const loading = ref(false)

// 表单相关
const dialogVisible = ref(false)
const formType = ref<'create' | 'edit'>('create')
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
  code: [
    { required: true, message: '请输入租户编码', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
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

// 对话框标题
const dialogTitle = computed(() => {
  return formType.value === 'create' ? '新增租户' : '编辑租户'
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
    const { data } = await tenantService.getList(searchParams)
    tableData.value = data.items
    total.value = data.total
  } catch (error) {
    console.error('获取租户列表失败:', error)
    ElMessage.error('获取租户列表失败')
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
  dialogVisible.value = true
}

// 查看
const handleView = (row: any) => {
  // 跳转到详情页
  window.open(`#/tenant/detail/${row.id}`, '_blank')
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
      `确定要${row.isActive ? '禁用' : '启用'}该租户吗？`,
      '提示',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await tenantService.updateStatus(row.id, !row.isActive)
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
      `确定要删除租户"${row.name}"吗？此操作不可恢复！`,
      '警告',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    const { data } = await tenantService.delete(row.id)
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
      // 检查租户编码是否存在
      const { data: exists } = await tenantService.checkCode(values.code)
      if (exists) {
        ElMessage.error('租户编码已存在')
        return
      }
      
      // 创建租户
      const { data } = await tenantService.create(values)
      if (data) {
        ElMessage.success('创建成功')
        dialogVisible.value = false
        fetchData()
      }
    } else {
      // 更新租户
      const { data } = await tenantService.update(values.id, values)
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
    code: '',
    name: '',
    description: '',
    contactPerson: '',
    contactPhone: '',
    address: '',
    expireDate: '',
    isActive: true
  })
}
</script>

<style lang="scss" scoped>
.tenant-list-container {
  padding: 20px;
}
</style>