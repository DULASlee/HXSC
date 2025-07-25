<template>
  <div class="app-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>租户管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            添加
          </el-button>
        </div>
      </template>

      <!-- 搜索表单 -->
      <el-form :model="searchForm" inline>
        <el-form-item label="租户编码">
          <el-input v-model="searchForm.code" placeholder="请输入租户编码" />
        </el-form-item>
        <el-form-item label="租户名称">
          <el-input v-model="searchForm.name" placeholder="请输入租户名称" />
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="searchForm.status" placeholder="请选择状态">
            <el-option label="启用" :value="1" />
            <el-option label="禁用" :value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>

      <!-- 数据表格 -->
      <el-table
        v-loading="loading"
        :data="tenantList"
        border
        style="width: 100%"
      >
        <el-table-column prop="code" label="租户编码" width="120" />
        <el-table-column prop="name" label="租户名称" width="150" />
        <el-table-column prop="contactName" label="联系人" width="120" />
        <el-table-column prop="contactPhone" label="联系电话" width="120" />
        <el-table-column prop="contactEmail" label="联系邮箱" width="180" />
        <el-table-column prop="expireDate" label="过期时间" width="120">
          <template #default="{ row }">
            {{ formatDate(row.expireDate) }}
          </template>
        </el-table-column>
        <el-table-column prop="maxUsers" label="最大用户数" width="100" />
        <el-table-column prop="status" label="状态" width="80">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'">
              {{ row.status === 1 ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" width="160">
          <template #default="{ row }">
            {{ formatDate(row.createdAt) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">
              编辑
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">
              删除
            </el-button>
            <el-button type="info" size="small" @click="handleIsolation(row)">
              隔离模式
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        :current-page="pagination.page"
        :page-size="pagination.size"
        :total="pagination.total"
        :page-sizes="[10, 20, 50, 100]"
        layout="total, sizes, prev, pager, next, jumper"
        @update:current-page="handleCurrentChange"
        @update:page-size="handleSizeChange"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
      />

      <!-- 租户表单对话框 -->
      <el-dialog
        v-model="dialogVisible"
        :title="dialogTitle"
        width="600px"
        @close="handleDialogClose"
      >
        <el-form
          ref="tenantFormRef"
          :model="tenantForm"
          :rules="tenantRules"
          label-width="120px"
        >
          <el-form-item label="租户编码" prop="code">
            <el-input v-model="tenantForm.code" :disabled="isEdit" />
          </el-form-item>
          <el-form-item label="租户名称" prop="name">
            <el-input v-model="tenantForm.name" />
          </el-form-item>
          <el-form-item label="联系人">
            <el-input v-model="tenantForm.contactName" />
          </el-form-item>
          <el-form-item label="联系电话">
            <el-input v-model="tenantForm.contactPhone" />
          </el-form-item>
          <el-form-item label="联系邮箱">
            <el-input v-model="tenantForm.contactEmail" />
          </el-form-item>
          <el-form-item label="联系地址">
            <el-input v-model="tenantForm.address" type="textarea" />
          </el-form-item>
          <el-form-item label="描述">
            <el-input v-model="tenantForm.description" type="textarea" />
          </el-form-item>
          <el-form-item label="过期时间">
            <el-date-picker
              v-model="tenantForm.expireDate"
              type="date"
              placeholder="选择过期时间"
            />
          </el-form-item>
          <el-form-item label="最大用户数">
            <el-input-number v-model="tenantForm.maxUsers" :min="1" />
          </el-form-item>
          <el-form-item label="状态">
            <el-switch
              v-model="tenantForm.status"
              :active-value="1"
              :inactive-value="0"
            />
          </el-form-item>
        </el-form>
        <template #footer>
          <span class="dialog-footer">
            <el-button @click="dialogVisible = false">取消</el-button>
            <el-button type="primary" @click="handleSubmit">确定</el-button>
          </span>
        </template>
      </el-dialog>

      <!-- 隔离模式对话框 -->
      <el-dialog
        v-model="isolationDialogVisible"
        title="租户隔离模式配置"
        width="500px"
      >
        <el-form :model="isolationForm" label-width="120px">
          <el-form-item label="隔离模式">
            <el-select v-model="isolationForm.currentLevel">
              <el-option label="共享数据库" value="Shared" />
              <el-option label="独立Schema" value="Schema" />
              <el-option label="独立数据库" value="Database" />
            </el-select>
          </el-form-item>
          <el-form-item label="连接字符串" v-if="isolationForm.currentLevel === 'Database'">
            <el-input v-model="isolationForm.connectionString" type="textarea" />
          </el-form-item>
          <el-form-item label="Schema名称" v-if="isolationForm.currentLevel === 'Schema'">
            <el-input v-model="isolationForm.schemaName" />
          </el-form-item>
          <el-form-item label="自动扩展">
            <el-switch v-model="isolationForm.autoScaleEnabled" />
          </el-form-item>
        </el-form>
        <template #footer>
          <span class="dialog-footer">
            <el-button @click="isolationDialogVisible = false">取消</el-button>
            <el-button type="primary" @click="handleIsolationSubmit">确定</el-button>
          </span>
        </template>
      </el-dialog>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { Tenant, TenantIsolation } from '@/types/global'
import { getTenants, createTenant, updateTenant, deleteTenant, getTenantIsolation, updateTenantIsolation } from '@/api/tenant'

// 搜索表单
const searchForm = reactive({
  code: '',
  name: '',
  status: undefined as number | undefined
})

// 分页
const pagination = reactive({
  page: 1,
  size: 10,
  total: 0
})

// 数据
const loading = ref(false)
const tenantList = ref<Tenant[]>([])
const currentTenant = ref<Tenant | null>(null)

// 对话框
const dialogVisible = ref(false)
const isolationDialogVisible = ref(false)
const isEdit = ref(false)
const dialogTitle = ref('')

// 表单
const tenantForm = reactive({
  id: '',
  code: '',
  name: '',
  contactName: '',
  contactPhone: '',
  contactEmail: '',
  address: '',
  description: '',
  expireDate: '',
  maxUsers: 100,
  status: 1
})

const isolationForm = reactive({
  tenantId: '',
  currentLevel: 'Shared' as 'Shared' | 'Schema' | 'Database',
  connectionString: '',
  schemaName: '',
  autoScaleEnabled: true
})

// 表单验证规则
const tenantRules = {
  code: [
    { required: true, message: '请输入租户编码', trigger: 'blur' },
    { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
  ],
  name: [
    { required: true, message: '请输入租户名称', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ]
}

// 格式化日期
const formatDate = (date: string) => {
  if (!date) return ''
  return new Date(date).toLocaleDateString()
}

// 获取租户列表
const getTenantList = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      page: pagination.page,
      size: pagination.size
    }
    const { data } = await getTenants(params)
    tenantList.value = data.items
    pagination.total = data.total
  } catch (error) {
    console.error('获取租户列表失败:', error)
  } finally {
    loading.value = false
  }
}

// 搜索
const handleSearch = () => {
  pagination.page = 1
  getTenantList()
}

// 重置
const handleReset = () => {
  searchForm.code = ''
  searchForm.name = ''
  searchForm.status = undefined
  handleSearch()
}

// 添加
const handleAdd = () => {
  isEdit.value = false
  dialogTitle.value = '添加租户'
  resetForm()
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: Tenant) => {
  isEdit.value = true
  dialogTitle.value = '编辑租户'
  Object.assign(tenantForm, row)
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: Tenant) => {
  try {
    await ElMessageBox.confirm('确定要删除该租户吗？', '提示', {
      type: 'warning'
    })
    await deleteTenant(row.id)
    ElMessage.success('删除成功')
    getTenantList()
  } catch (error) {
    console.error('删除租户失败:', error)
  }
}

// 隔离模式
const handleIsolation = async (row: Tenant) => {
  currentTenant.value = row
  try {
    const { data } = await getTenantIsolation(row.id)
    Object.assign(isolationForm, data)
    isolationForm.tenantId = row.id
    isolationDialogVisible.value = true
  } catch (error) {
    console.error('获取隔离配置失败:', error)
  }
}

// 提交表单
const handleSubmit = async () => {
  try {
    if (isEdit.value) {
      await updateTenant(tenantForm.id, tenantForm)
      ElMessage.success('更新成功')
    } else {
      await createTenant(tenantForm)
      ElMessage.success('创建成功')
    }
    dialogVisible.value = false
    getTenantList()
  } catch (error) {
    console.error('提交表单失败:', error)
  }
}

// 提交隔离配置
const handleIsolationSubmit = async () => {
  try {
    await updateTenantIsolation(isolationForm.tenantId, isolationForm)
    ElMessage.success('配置成功')
    isolationDialogVisible.value = false
  } catch (error) {
    console.error('配置隔离模式失败:', error)
  }
}

// 重置表单
const resetForm = () => {
  Object.assign(tenantForm, {
    id: '',
    code: '',
    name: '',
    contactName: '',
    contactPhone: '',
    contactEmail: '',
    address: '',
    description: '',
    expireDate: '',
    maxUsers: 100,
    status: 1
  })
}

// 分页变化
const handleSizeChange = (val: number) => {
  pagination.size = val
  getTenantList()
}

const handleCurrentChange = (val: number) => {
  pagination.page = val
  getTenantList()
}

// 关闭对话框
const handleDialogClose = () => {
  resetForm()
}

// 初始化
onMounted(() => {
  getTenantList()
})
</script>

<style scoped>
.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>
