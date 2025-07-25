<template>
  <div class="worker-management">
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>工人管理</span>
          <el-button type="primary" @click="showCreateDialog = true">创建工人</el-button>
        </div>
      </template>

      <!-- 搜索栏 -->
      <el-form :inline="true" :model="searchForm" class="search-form">
        <el-form-item label="项目名称">
          <el-input v-model="searchForm.projectName" placeholder="请输入项目名称" />
        </el-form-item>
        <el-form-item label="工人类型">
          <el-select v-model="searchForm.workerType" placeholder="请选择工人类型">
            <el-option label="全部" value="" />
            <el-option label="木工" value="木工" />
            <el-option label="电工" value="电工" />
            <el-option label="钢筋工" value="钢筋工" />
            <el-option label="混凝土工" value="混凝土工" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="searchWorkers">搜索</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>

      <!-- 工人列表 -->
      <el-table :data="workers" style="width: 100%" v-loading="loading">
        <el-table-column prop="workerNumber" label="工号" width="120" />
        <el-table-column prop="name" label="姓名" width="120" />
        <el-table-column prop="phoneNumber" label="手机号" width="150" />
        <el-table-column prop="workerType" label="工种" width="100" />
        <el-table-column prop="status" label="状态" width="100">
          <template #default="scope">
            <el-tag :type="scope.row.status === 'Active' ? 'success' : 'danger'">
              {{ scope.row.status === 'Active' ? '在职' : '离职' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="joinDate" label="入职日期" width="120">
          <template #default="scope">
            {{ formatDate(scope.row.joinDate) }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button size="small" type="primary" @click="editWorker(scope.row)">编辑</el-button>
            <el-button size="small" type="danger" @click="deleteWorker(scope.row.id)">删除</el-button>
            <el-button size="small" @click="showAttendance(scope.row.id)">考勤</el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <el-pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :page-sizes="[10, 20, 50, 100]"
        :total="total"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
      />
    </el-card>

    <!-- 创建/编辑工人对话框 -->
    <el-dialog
      v-model="showCreateDialog"
      :title="isEdit ? '编辑工人' : '创建工人'"
      width="500px"
    >
      <el-form :model="workerForm" :rules="rules" ref="workerFormRef" label-width="100px">
        <el-form-item label="姓名" prop="name">
          <el-input v-model="workerForm.name" />
        </el-form-item>
        <el-form-item label="工号" prop="workerNumber">
          <el-input v-model="workerForm.workerNumber" />
        </el-form-item>
        <el-form-item label="手机号" prop="phoneNumber">
          <el-input v-model="workerForm.phoneNumber" />
        </el-form-item>
        <el-form-item label="工种" prop="workerType">
          <el-select v-model="workerForm.workerType" placeholder="请选择工种">
            <el-option label="木工" value="木工" />
            <el-option label="电工" value="电工" />
            <el-option label="钢筋工" value="钢筋工" />
            <el-option label="混凝土工" value="混凝土工" />
          </el-select>
        </el-form-item>
        <el-form-item label="项目ID" prop="projectId">
          <el-input v-model="workerForm.projectId" />
        </el-form-item>
        <el-form-item label="紧急联系人" prop="emergencyContact">
          <el-input v-model="workerForm.emergencyContact" />
        </el-form-item>
        <el-form-item label="紧急电话" prop="emergencyPhone">
          <el-input v-model="workerForm.emergencyPhone" />
        </el-form-item>
      </el-form>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="showCreateDialog = false">取消</el-button>
          <el-button type="primary" @click="submitWorker">确定</el-button>
        </span>
      </template>
    </el-dialog>

    <!-- 考勤记录对话框 -->
    <el-dialog
      v-model="showAttendanceDialog"
      title="考勤记录"
      width="800px"
    >
      <el-table :data="attendanceRecords" style="width: 100%">
        <el-table-column prop="clockInTime" label="打卡时间" width="180">
          <template #default="scope">
            {{ formatDateTime(scope.row.clockInTime) }}
          </template>
        </el-table-column>
        <el-table-column prop="clockOutTime" label="下班时间" width="180">
          <template #default="scope">
            {{ scope.row.clockOutTime ? formatDateTime(scope.row.clockOutTime) : '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="workHours" label="工作时长(小时)" width="120">
          <template #default="scope">
            {{ scope.row.workHours?.toFixed(2) || '-' }}
          </template>
        </el-table-column>
        <el-table-column prop="clockInLocation" label="打卡地点" />
      </el-table>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { FormInstance, FormRules } from 'element-plus'

// 数据
const workers = ref([])
const attendanceRecords = ref([])
const loading = ref(false)
const showCreateDialog = ref(false)
const showAttendanceDialog = ref(false)
const isEdit = ref(false)
const currentPage = ref(1)
const pageSize = ref(10)
const total = ref(0)

// 搜索表单
const searchForm = reactive({
  projectName: '',
  workerType: ''
})

// 工人表单
const workerForm = reactive({
  id: '',
  name: '',
  workerNumber: '',
  phoneNumber: '',
  workerType: '',
  projectId: '',
  emergencyContact: '',
  emergencyPhone: ''
})

// 表单验证规则
const rules: FormRules = {
  name: [{ required: true, message: '请输入姓名', trigger: 'blur' }],
  workerNumber: [{ required: true, message: '请输入工号', trigger: 'blur' }],
  phoneNumber: [
    { required: true, message: '请输入手机号', trigger: 'blur' },
    { pattern: /^1[3-9]\d{9}$/, message: '请输入正确的手机号', trigger: 'blur' }
  ],
  workerType: [{ required: true, message: '请选择工种', trigger: 'change' }],
  projectId: [{ required: true, message: '请输入项目ID', trigger: 'blur' }]
}

const workerFormRef = ref<FormInstance>()

// 方法
const loadWorkers = async () => {
  loading.value = true
  try {
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 500))
    workers.value = [
      {
        id: '11111111-1111-1111-1111-111111111111',
        name: '张三',
        workerNumber: 'W001',
        phoneNumber: '13800138001',
        workerType: '木工',
        projectId: '00000000-0000-0000-0000-000000000001',
        status: 'Active',
        joinDate: new Date('2024-01-15'),
        emergencyContact: '张父',
        emergencyPhone: '13800138002'
      },
      {
        id: '22222222-2222-2222-2222-222222222222',
        name: '李四',
        workerNumber: 'W002',
        phoneNumber: '13800138003',
        workerType: '电工',
        projectId: '00000000-0000-0000-0000-000000000001',
        status: 'Active',
        joinDate: new Date('2024-03-20'),
        emergencyContact: '李母',
        emergencyPhone: '13800138004'
      }
    ]
    total.value = workers.value.length
  } catch (error) {
    ElMessage.error('加载工人列表失败')
  } finally {
    loading.value = false
  }
}

const searchWorkers = () => {
  loadWorkers()
}

const resetSearch = () => {
  searchForm.projectName = ''
  searchForm.workerType = ''
  loadWorkers()
}

const editWorker = (worker: any) => {
  isEdit.value = true
  Object.assign(workerForm, worker)
  showCreateDialog.value = true
}

const deleteWorker = async (id: string) => {
  try {
    await ElMessageBox.confirm('确定要删除这个工人吗？', '提示', {
      type: 'warning'
    })
    // 模拟删除
    workers.value = workers.value.filter(w => w.id !== id)
    ElMessage.success('删除成功')
  } catch {
    // 取消删除
  }
}

const showAttendance = async (workerId: string) => {
  try {
    // 模拟获取考勤记录
    attendanceRecords.value = [
      {
        id: '1',
        workerId: workerId,
        projectId: '00000000-0000-0000-0000-000000000001',
        clockInTime: new Date('2024-07-17 08:00:00'),
        clockOutTime: new Date('2024-07-17 17:30:00'),
        workHours: 8.5,
        clockInLocation: '项目现场东门'
      },
      {
        id: '2',
        workerId: workerId,
        projectId: '00000000-0000-0000-0000-000000000001',
        clockInTime: new Date('2024-07-16 08:15:00'),
        clockOutTime: new Date('2024-07-16 17:45:00'),
        workHours: 8.5,
        clockInLocation: '项目现场东门'
      }
    ]
    showAttendanceDialog.value = true
  } catch (error) {
    ElMessage.error('加载考勤记录失败')
  }
}

const submitWorker = async () => {
  if (!workerFormRef.value) return
  
  try {
    await workerFormRef.value.validate()
    
    if (isEdit.value) {
      // 模拟更新
      const index = workers.value.findIndex(w => w.id === workerForm.id)
      if (index !== -1) {
        Object.assign(workers.value[index], workerForm)
        ElMessage.success('更新成功')
      }
    } else {
      // 模拟创建
      const newWorker = {
        ...workerForm,
        id: crypto.randomUUID(),
        status: 'Active',
        joinDate: new Date()
      }
      workers.value.push(newWorker)
      ElMessage.success('创建成功')
    }
    
    showCreateDialog.value = false
    resetForm()
  } catch (error) {
    console.error('表单验证失败:', error)
  }
}

const resetForm = () => {
  Object.assign(workerForm, {
    id: '',
    name: '',
    workerNumber: '',
    phoneNumber: '',
    workerType: '',
    projectId: '',
    emergencyContact: '',
    emergencyPhone: ''
  })
  isEdit.value = false
}

const handleSizeChange = (val: number) => {
  pageSize.value = val
  loadWorkers()
}

const handleCurrentChange = (val: number) => {
  currentPage.value = val
  loadWorkers()
}

const formatDate = (date: Date) => {
  return new Date(date).toLocaleDateString('zh-CN')
}

const formatDateTime = (date: Date) => {
  return new Date(date).toLocaleString('zh-CN')
}

// 生命周期
onMounted(() => {
  loadWorkers()
})
</script>

<style scoped>
.worker-management {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-form {
  margin-bottom: 20px;
}

.dialog-footer {
  text-align: right;
}
</style>
