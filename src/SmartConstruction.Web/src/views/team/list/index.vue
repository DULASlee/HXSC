<template>
  <div class="team-list-container">
    <page-header
      title="班组管理"
      subtitle="管理和查看所有班组信息"
    >
      <template #actions>
        <el-button type="primary" @click="handleCreate" v-permission="'team.create'">
          <el-icon><plus /></el-icon>
          新建班组
        </el-button>
      </template>
    </page-header>

    <div class="content-container">
      <!-- 搜索表单 -->
      <search-form :model="searchForm" @search="handleSearch" @reset="handleReset">
        <el-row :gutter="20">
          <el-col :span="6">
            <el-form-item label="班组名称">
              <el-input
                v-model="searchForm.keyword"
                placeholder="请输入班组名称"
                clearable
                @keyup.enter="handleSearch"
              />
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="所属公司">
              <el-select
                v-model="searchForm.companyId"
                placeholder="请选择公司"
                clearable
                filterable
              >
                <el-option
                  v-for="item in companyOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="班组类型">
              <el-select
                v-model="searchForm.type"
                placeholder="请选择类型"
                clearable
              >
                <el-option
                  v-for="item in teamTypeOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="6">
            <el-form-item label="状态">
              <el-select
                v-model="searchForm.isActive"
                placeholder="请选择状态"
                clearable
              >
                <el-option label="启用" :value="true" />
                <el-option label="禁用" :value="false" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
      </search-form>

      <!-- 数据表格 -->
      <data-table
        :data="tableData"
        :loading="loading"
        :pagination="pagination"
        @page-change="handlePageChange"
        @size-change="handleSizeChange"
      >
        <el-table-column prop="name" label="班组名称" min-width="150" show-overflow-tooltip />
        <el-table-column prop="companyName" label="所属公司" min-width="150" show-overflow-tooltip />
        <el-table-column prop="leader" label="班组长" min-width="120" />
        <el-table-column prop="leaderPhone" label="联系电话" min-width="120" />
        <el-table-column prop="type" label="班组类型" min-width="100">
          <template #default="{ row }">
            {{ getTeamTypeName(row.type) }}
          </template>
        </el-table-column>
        <el-table-column prop="workerCount" label="工人数量" min-width="100" align="center" />
        <el-table-column prop="isActive" label="状态" min-width="100" align="center">
          <template #default="{ row }">
            <status-tag
              :status="row.isActive"
              :status-map="{
                true: { text: '启用', type: 'success' },
                false: { text: '禁用', type: 'danger' }
              }"
            />
          </template>
        </el-table-column>
        <el-table-column prop="createdTime" label="创建时间" min-width="180">
          <template #default="{ row }">
            {{ formatDateTime(row.createdTime) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" min-width="200" fixed="right">
          <template #default="{ row }">
            <el-button type="primary" link @click="handleView(row)" v-permission="'team.view'">
              查看
            </el-button>
            <el-button type="primary" link @click="handleEdit(row)" v-permission="'team.edit'">
              编辑
            </el-button>
            <el-button
              type="danger"
              link
              @click="handleDelete(row)"
              v-permission="'team.delete'"
            >
              删除
            </el-button>
          </template>
        </el-table-column>
      </data-table>
    </div>

    <!-- 新建/编辑对话框 -->
    <form-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      :loading="saving"
      @confirm="handleSave"
      @cancel="handleCancel"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
        v-loading="formLoading"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="班组名称" prop="name">
              <el-input v-model="formData.name" placeholder="请输入班组名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="所属公司" prop="companyId">
              <el-select
                v-model="formData.companyId"
                placeholder="请选择公司"
                filterable
                style="width: 100%"
              >
                <el-option
                  v-for="item in companyOptions"
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
            <el-form-item label="班组长" prop="leader">
              <el-input v-model="formData.leader" placeholder="请输入班组长姓名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="leaderPhone">
              <el-input v-model="formData.leaderPhone" placeholder="请输入联系电话" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="班组类型" prop="type">
              <el-select
                v-model="formData.type"
                placeholder="请选择班组类型"
                style="width: 100%"
              >
                <el-option
                  v-for="item in teamTypeOptions"
                  :key="item.value"
                  :label="item.label"
                  :value="item.value"
                />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="isActive">
              <el-switch
                v-model="formData.isActive"
                active-text="启用"
                inactive-text="禁用"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="备注" prop="remark">
          <el-input
            v-model="formData.remark"
            type="textarea"
            :rows="3"
            placeholder="请输入备注信息"
          />
        </el-form-item>
      </el-form>
    </form-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { teamService, companyService } from '@/api/services'
import { formatDateTime } from '@/utils/format'
import type { FormInstance } from 'element-plus'

const router = useRouter()

// 搜索表单
const searchForm = reactive({
  keyword: '',
  companyId: '',
  type: '',
  isActive: undefined as boolean | undefined
})

// 表格数据
const tableData = ref<any[]>([])
const loading = ref(false)
const pagination = reactive({
  pageIndex: 1,
  pageSize: 10,
  total: 0
})

// 公司选项
const companyOptions = ref<any[]>([])

// 班组类型选项
const teamTypeOptions = [
  { label: '普通班组', value: 'General' },
  { label: '专业班组', value: 'Professional' },
  { label: '技术班组', value: 'Technical' },
  { label: '管理班组', value: 'Management' }
]

// 对话框
const dialogVisible = ref(false)
const dialogTitle = computed(() => (formData.value.id ? '编辑班组' : '新建班组'))
const saving = ref(false)
const formLoading = ref(false)
const formRef = ref<FormInstance>()
const formData = ref({
  id: '',
  name: '',
  companyId: '',
  leader: '',
  leaderPhone: '',
  type: 'General',
  isActive: true,
  remark: ''
})

// 表单验证规则
const formRules = {
  name: [
    { required: true, message: '请输入班组名称', trigger: 'blur' },
    { min: 2, max: 50, message: '长度在 2 到 50 个字符', trigger: 'blur' }
  ],
  companyId: [
    { required: true, message: '请选择所属公司', trigger: 'change' }
  ],
  leader: [
    { required: true, message: '请输入班组长姓名', trigger: 'blur' },
    { min: 2, max: 20, message: '长度在 2 到 20 个字符', trigger: 'blur' }
  ],
  leaderPhone: [
    { required: true, message: '请输入联系电话', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号码', trigger: 'blur' }
  ],
  type: [
    { required: true, message: '请选择班组类型', trigger: 'change' }
  ]
}

// 获取班组类型名称
const getTeamTypeName = (type: string) => {
  const option = teamTypeOptions.find(item => item.value === type)
  return option?.label || type
}

// 加载数据
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      pageIndex: pagination.pageIndex,
      pageSize: pagination.pageSize
    }
    const { data } = await teamService.getList(params)
    tableData.value = data.items
    pagination.total = data.total
  } catch (error) {
    console.error('加载班组列表失败:', error)
    ElMessage.error('加载班组列表失败')
  } finally {
    loading.value = false
  }
}

// 加载公司选项
const loadCompanyOptions = async () => {
  try {
    const { data } = await companyService.getAll()
    companyOptions.value = data.map((item: any) => ({
      label: item.name,
      value: item.id
    }))
  } catch (error) {
    console.error('加载公司选项失败:', error)
  }
}

// 搜索
const handleSearch = () => {
  pagination.pageIndex = 1
  loadData()
}

// 重置
const handleReset = () => {
  searchForm.keyword = ''
  searchForm.companyId = ''
  searchForm.type = ''
  searchForm.isActive = undefined
  handleSearch()
}

// 分页
const handlePageChange = (page: number) => {
  pagination.pageIndex = page
  loadData()
}

const handleSizeChange = (size: number) => {
  pagination.pageSize = size
  pagination.pageIndex = 1
  loadData()
}

// 查看
const handleView = (row: any) => {
  router.push(`/team/detail/${row.id}`)
}

// 编辑
const handleEdit = (row: any) => {
  formData.value = { ...row }
  dialogVisible.value = true
}

// 新建
const handleCreate = () => {
  formData.value = {
    id: '',
    name: '',
    companyId: '',
    leader: '',
    leaderPhone: '',
    type: 'General',
    isActive: true,
    remark: ''
  }
  dialogVisible.value = true
}

// 删除
const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要删除班组"${row.name}"吗？此操作将同时删除相关的所有数据，且不可恢复。`,
      '确认删除',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )

    await teamService.delete(row.id)
    ElMessage.success('删除成功')
    loadData()
  } catch (error) {
    if (error !== 'cancel') {
      console.error('删除班组失败:', error)
      ElMessage.error('删除失败')
    }
  }
}

// 保存
const handleSave = async () => {
  if (!formRef.value) return

  try {
    await formRef.value.validate()
    saving.value = true

    if (formData.value.id) {
      await teamService.update(formData.value.id, formData.value)
      ElMessage.success('更新成功')
    } else {
      await teamService.create(formData.value)
      ElMessage.success('创建成功')
    }

    dialogVisible.value = false
    loadData()
  } catch (error) {
    console.error('保存班组失败:', error)
    ElMessage.error('保存失败')
  } finally {
    saving.value = false
  }
}

// 取消
const handleCancel = () => {
  dialogVisible.value = false
  formRef.value?.resetFields()
}

// 初始化
onMounted(() => {
  loadData()
  loadCompanyOptions()
})
</script>

<style lang="scss" scoped>
.team-list-container {
  padding: 20px;
}

.content-container {
  margin-top: 20px;
}
</style>
