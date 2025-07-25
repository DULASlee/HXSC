<template>
  <div class="user-management">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>用户管理</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            新增用户
          </el-button>
        </div>
      </template>
      
      <!-- 搜索区域 -->
      <div class="search-area">
        <el-form :model="searchForm" inline>
          <el-form-item label="关键词">
            <el-input
              v-model="searchForm.keyword"
              placeholder="用户名/显示名称/邮箱"
              clearable
            />
          </el-form-item>
          <el-form-item label="状态">
            <el-select v-model="searchForm.status" placeholder="请选择状态" clearable>
              <el-option label="启用" :value="1" />
              <el-option label="禁用" :value="0" />
            </el-select>
          </el-form-item>
          <el-form-item label="组织">
            <el-select v-model="searchForm.organizationId" placeholder="请选择组织" clearable>
              <el-option label="全部" :value="undefined" />
              <el-option label="系统管理部" value="1" />
              <el-option label="业务部门" value="2" />
              <el-option label="测试部门" value="3" />
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
        <el-table-column prop="username" label="用户名" />
        <el-table-column prop="displayName" label="显示名称" />
        <el-table-column prop="email" label="邮箱" />
        <el-table-column prop="mobile" label="手机号" />
        <el-table-column prop="organizationName" label="所属组织" />
        <el-table-column prop="status" label="状态">
          <template #default="{ row }">
            <el-tag :type="row.status === 1 ? 'success' : 'danger'">
              {{ row.status === 1 ? '启用' : '禁用' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="createdAt" label="创建时间" />
        <el-table-column label="操作" width="280">
          <template #default="{ row }">
            <el-button type="primary" size="small" @click="handleEdit(row)">
              编辑
            </el-button>
            <el-button 
              :type="row.status === 1 ? 'warning' : 'success'" 
              size="small" 
              @click="handleToggleStatus(row)"
            >
              {{ row.status === 1 ? '禁用' : '启用' }}
            </el-button>
            <el-button type="danger" size="small" @click="handleDelete(row)">
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
    </el-card>
      
    <!-- 用户表单对话框 -->
    <user-form-dialog
      v-model:visible="dialogVisible"
      :user-data="currentUser"
      :is-edit="isEdit"
      @success="handleFormSuccess"
    />
  </div>
</template>

<script setup lang="ts">
// 组合式API和工具函数已由unplugin-auto-import自动导入
const handleGlobalError = inject('handleGlobalError')

// 搜索表单
const searchForm = reactive({
  keyword: '',
  status: undefined,
  organizationId: undefined
})

// 使用 useApiList 获取用户列表
const { list, loading, pagination, refresh, search } = useApiList(getUserList, { 
  params: searchForm,
  immediate: true 
})

// 对话框相关
const dialogVisible = ref(false)
const isEdit = ref(false)
const currentUser = ref<any>({})

// 搜索
const handleSearch = () => {
  search()
}

// 重置
const handleReset = () => {
  searchForm.keyword = ''
  searchForm.status = undefined
  searchForm.organizationId = undefined
  search()
}

// 新增
const handleAdd = () => {
  isEdit.value = false
  currentUser.value = {
    status: 1,
    roleIds: []
  }
  dialogVisible.value = true
}

// 编辑
const handleEdit = (row: any) => {
  isEdit.value = true
  currentUser.value = { ...row }
  dialogVisible.value = true
}

// 表单提交成功
const handleFormSuccess = () => {
  refresh() // 重新加载数据
}

// 删除
const { execute: executeDelete, loading: deleteLoading } = useApi(deleteUser)
const handleDelete = async (row: any) => {
  await ElMessageBox.confirm(
    `确定要删除用户 "${row.displayName}" 吗？`,
    '确认删除',
    { type: 'warning' }
  )
  await executeDelete(row.id)
  ElMessage.success('删除成功')
  refresh()
}

// 启用/禁用用户
const { execute: executeToggleStatus, loading: toggleLoading } = useApi(toggleUserStatus)
const handleToggleStatus = async (row: any) => {
  const newStatus = row.status === 1 ? 0 : 1
  const statusText = newStatus === 1 ? '启用' : '禁用'
  
  await ElMessageBox.confirm(
    `确定要${statusText}用户 "${row.displayName}" 吗？`,
    `确认${statusText}`,
    { type: 'warning' }
  )
  await executeToggleStatus(row.id, newStatus)
  ElMessage.success(`${statusText}成功`)
  refresh()
}
</script>

<style lang="scss" scoped>
.user-management {
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