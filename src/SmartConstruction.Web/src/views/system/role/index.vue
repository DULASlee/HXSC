<template>
  <div class="role-management">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>角色管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            新增角色
          </el-button>
        </div>
      </template>
      
      <!-- 搜索区域 -->
      <div class="search-area">
        <el-form :model="searchForm" inline>
          <el-form-item label="角色名称">
            <el-input
              v-model="searchForm.keyword"
              placeholder="请输入角色名称或编码"
              clearable
            />
          </el-form-item>
          <el-form-item label="状态">
            <el-select v-model="searchForm.status" placeholder="请选择状态" clearable>
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
      </div>
      
      <!-- 表格区域 -->
      <el-table :data="list" v-loading="loading">
        <el-table-column prop="name" label="角色名称" />
        <el-table-column prop="code" label="角色编码" />
        <el-table-column prop="description" label="描述" show-overflow-tooltip />
        <el-table-column prop="dataScope" label="数据权限">
          <template #default="{ row }">
            <el-tag>{{ getDataScopeLabel(row.dataScope) }}</el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="status" label="状态">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'">
              {{ row.status === 1 ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="isSystem" label="系统角色">
          <template #default="{ row }">
            <el-tag v-if="row.isSystem" type="warning">是</el-tag>
            <span v-else>否</span>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" />
        <el-table-column label="操作" width="280">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)" :disabled="row.isSystem">
              编辑
            </el-button>
            <el-button type="success" size="small" @click="handlePermission(row)">
              权限
            </el-button>
            <el-button 
              :type="row.status === 1 ? 'warning' : 'success'" 
              size="small" 
              @click="handleToggleStatus(row)"
              :disabled="row.isSystem"
            >
              {{ row.status === 1 ? '禁用' : '启用' }}
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)" :disabled="row.isSystem">
              删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      
      <!-- 分页 -->
      <div class="pagination">
        <el-pagination
          v-model:current-page="pagination.page"
          v-model:page-size="pagination.pageSize"
          :total="pagination.total"
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="search"
          @current-change="refresh"
        />
      </div>
      
      <!-- 角色表单对话框 -->
      <el-dialog
        :title="isEdit ? '编辑角色' : '新增角色'"
        v-model="dialogVisible"
        width="500px"
        :close-on-click-modal="false"
      >
        <el-form
          ref="formRef"
          :model="form"
          :rules="rules"
          label-width="100px"
          label-position="right"
          v-loading="formLoading"
        >
          <el-form-item label="角色名称" prop="name">
            <el-input v-model="form.name" placeholder="请输入角色名称" />
          </el-form-item>
          
          <el-form-item label="角色编码" prop="code">
            <el-input v-model="form.code" placeholder="请输入角色编码" :disabled="isEdit" />
          </el-form-item>
          
          <el-form-item label="描述" prop="description">
            <el-input v-model="form.description" type="textarea" placeholder="请输入描述" />
          </el-form-item>
          
          <el-form-item label="数据权限" prop="dataScope">
            <el-select v-model="form.dataScope" placeholder="请选择数据权限" style="width: 100%">
              <el-option label="全部数据" value="All" />
              <el-option label="本部门及以下数据" value="Department" />
              <el-option label="本部门数据" value="DepartmentOnly" />
              <el-option label="仅本人数据" value="Self" />
              <el-option label="自定义数据" value="Custom" />
            </el-select>
          </el-form-item>
          
          <el-form-item label="状态" prop="status">
            <el-switch
              v-model="form.status"
              :active-value="1"
              :inactive-value="0"
              active-text="启用"
              inactive-text="禁用"
            />
          </el-form-item>
        </el-form>
        
        <template #footer>
          <el-button @click="dialogVisible = false">取消</el-button>
          <el-button type="primary" @click="submitForm" :loading="formLoading">确定</el-button>
        </template>
      </el-dialog>
      
      <!-- 权限分配对话框 -->
      <el-dialog
        title="分配权限"
        v-model="permissionDialogVisible"
        width="600px"
        :close-on-click-modal="false"
      >
        <div v-loading="permissionLoading">
          <el-tree
            ref="permissionTreeRef"
            :data="permissionTree"
            show-checkbox
            node-key="id"
            :props="{ label: 'name', children: 'children' }"
            :default-checked-keys="checkedPermissions"
          />
        </div>
        
        <template #footer>
          <el-button @click="permissionDialogVisible = false">取消</el-button>
          <el-button type="primary" @click="savePermissions" :loading="permissionLoading">保存</el-button>
        </template>
      </el-dialog>
    </el-card>
  </div>
</template>

<script setup lang="ts">
// 组合式API和工具函数已由unplugin-auto-import自动导入

// 搜索表单
const searchForm = reactive({
  keyword: '',
  status: undefined
})

// 使用 useApiList 获取角色列表
const { list, loading, pagination, refresh, search } = useApiList(getRoleList, {
  params: searchForm,
  immediate: true
})

// 角色表单对话框
const dialogVisible = ref(false)
const isEdit = ref(false)
const formRef = ref<InstanceType<typeof ElForm>>()
const { execute: executeCreate, loading: createLoading } = useApi(createRole)
const { execute: executeUpdate, loading: updateLoading } = useApi(updateRole)
const formLoading = computed(() => createLoading.value || updateLoading.value)

// 角色表单
const form = reactive({
  id: '',
  name: '',
  code: '',
  description: '',
  dataScope: 'Self',
  status: 1
})

// 表单验证规则
const rules = {
  name: [{ required: true, message: '请输入角色名称', trigger: 'blur' }],
  code: [
    { required: true, message: '请输入角色编码', trigger: 'blur' },
    { pattern: /^[A-Z_]+$/, message: '角色编码只能包含大写字母和下划线', trigger: 'blur' }
  ],
  dataScope: [{ required: true, message: '请选择数据权限', trigger: 'change' }]
}

// 权限分配对话框
const permissionDialogVisible = ref(false)
const permissionTreeRef = ref<InstanceType<typeof ElTree>>()
const currentRoleId = ref('')
const { data: permissionTree, loading: permissionTreeLoading } = useApi(getPermissionTree, { immediate: false })
const { data: checkedPermissions, loading: checkedPermissionsLoading, execute: loadCheckedPermissions } = useApi(getRolePermissions, { immediate: false })
const { execute: executeAssignPermissions, loading: assignPermissionsLoading } = useApi(assignRolePermissions)
const permissionLoading = computed(() => permissionTreeLoading.value || checkedPermissionsLoading.value || assignPermissionsLoading.value)

// 获取数据权限标签
const getDataScopeLabel = (dataScope: string) => {
  const map: Record<string, string> = {
    'All': '全部数据',
    'Department': '本部门及以下数据',
    'DepartmentOnly': '本部门数据',
    'Self': '仅本人数据',
    'Custom': '自定义数据'
  }
  return map[dataScope] || dataScope
}

// 搜索
const handleSearch = () => search()

// 重置
const handleReset = () => {
  searchForm.keyword = ''
  searchForm.status = undefined
  search()
}

// 新增
const handleAdd = () => {
  isEdit.value = false
  resetForm()
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: any) => {
  isEdit.value = true
  resetForm()
  Object.assign(form, row)
  dialogVisible.value = true
}

// 删除
const { execute: executeDelete } = useApi(deleteRole)
const handleDelete = async (row: any) => {
  await ElMessageBox.confirm(`确定要删除角色 "${row.name}" 吗？`, '确认删除', { type: 'warning' })
  await executeDelete(row.id)
  ElMessage.success('删除成功')
  refresh()
}

// 启用/禁用角色
const { execute: executeToggleStatus } = useApi(toggleRoleStatus)
const handleToggleStatus = async (row: any) => {
  const newStatus = row.status === 1 ? 0 : 1
  const statusText = newStatus === 1 ? '启用' : '禁用'
  await ElMessageBox.confirm(`确定要${statusText}角色 "${row.name}" 吗？`, `确认${statusText}`, { type: 'warning' })
  await executeToggleStatus(row.id, newStatus)
  ElMessage.success(`${statusText}成功`)
  refresh()
}

// 分配权限
const handlePermission = async (row: any) => {
  currentRoleId.value = row.id
  permissionDialogVisible.value = true
  // 并行加载权限树和角色已有权限
  await Promise.all([
    useApi(getPermissionTree).execute(),
    loadCheckedPermissions(row.id)
  ])
}

// 保存权限
const savePermissions = async () => {
  if (!permissionTreeRef.value) return
  const checkedKeys = permissionTreeRef.value.getCheckedKeys(false) as string[]
  const halfCheckedKeys = permissionTreeRef.value.getHalfCheckedKeys() as string[]
  await executeAssignPermissions(currentRoleId.value, [...checkedKeys, ...halfCheckedKeys])
  ElMessage.success('权限分配成功')
  permissionDialogVisible.value = false
}

// 重置表单
const resetForm = () => {
  Object.assign(form, { id: '', name: '', code: '', description: '', dataScope: 'Self', status: 1 })
  formRef.value?.resetFields()
}

// 提交表单
const submitForm = async () => {
  if (!formRef.value) return
  await formRef.value.validate()
  
  if (isEdit.value) {
    await executeUpdate(form.id, form)
    ElMessage.success('更新角色成功')
  } else {
    await executeCreate(form)
    ElMessage.success('创建角色成功')
  }
  
  dialogVisible.value = false
  refresh()
}
</script>

<style lang="scss" scoped>
.role-management {
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .search-area {
    margin-bottom: 20px;
    padding: 20px;
    background: var(--el-bg-color-page);
    border-radius: 4px;
  }
  
  .pagination {
    margin-top: 20px;
    text-align: right;
  }
}
</style>