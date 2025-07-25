<template>
  <el-dialog
    v-model="dialogVisible"
    title="分配角色"
    width="600px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <div class="user-info">
      <el-descriptions :column="2" border>
        <el-descriptions-item label="用户名">{{ userInfo.username }}</el-descriptions-item>
        <el-descriptions-item label="姓名">{{ userInfo.displayName }}</el-descriptions-item>
        <el-descriptions-item label="邮箱">{{ userInfo.email }}</el-descriptions-item>
        <el-descriptions-item label="手机号">{{ userInfo.mobile }}</el-descriptions-item>
      </el-descriptions>
    </div>

    <el-divider content-position="left">角色分配</el-divider>

    <div class="role-assignment">
      <div class="role-list">
        <div class="list-header">
          <span>可选角色</span>
          <el-input
            v-model="searchKeyword"
            placeholder="搜索角色"
            size="small"
            style="width: 200px"
            clearable
          >
            <template #prefix>
              <el-icon><Search /></el-icon>
            </template>
          </el-input>
        </div>
        <div class="list-content">
          <el-checkbox-group v-model="selectedRoleIds">
            <el-checkbox
              v-for="role in filteredRoles"
              :key="role.id"
              :label="role.id"
              class="role-item"
            >
              <div class="role-info">
                <div class="role-name">{{ role.name }}</div>
                <div class="role-desc">{{ role.code }}</div>
                <el-tag v-if="role.isSystem" type="warning" size="small">
                  系统角色
                </el-tag>
              </div>
            </el-checkbox>
          </el-checkbox-group>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="loading" @click="handleSubmit">
          确定
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Search } from '@element-plus/icons-vue'
import { getUser, assignRoles } from '../../../../api/user'
import { getRoles } from '../../../../api/role'
import type { User, Role } from '../../../../types/global'

interface Props {
  modelValue: boolean
  userId: string
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const loading = ref(false)
const searchKeyword = ref('')
const selectedRoleIds = ref<string[]>([])
const roleList = ref<Role[]>([])

const userInfo = reactive({
  username: '',
  displayName: '',
  email: '',
  mobile: ''
})

// 计算属性
const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

const filteredRoles = computed(() => {
  if (!searchKeyword.value) return roleList.value
  
  const keyword = searchKeyword.value.toLowerCase()
  return roleList.value.filter(role =>
    role.name.toLowerCase().includes(keyword) ||
    role.code.toLowerCase().includes(keyword)
  )
})

// 监听弹窗显示
watch(dialogVisible, (visible) => {
  if (visible && props.userId) {
    initDialog()
  } else {
    resetData()
  }
})

// 初始化弹窗
async function initDialog() {
  await Promise.all([
    loadUserInfo(),
    loadRoleList()
  ])
}

// 加载用户信息
async function loadUserInfo() {
  try {
    const { data } = await getUser(props.userId)
    
    Object.assign(userInfo, {
      username: data.username,
      displayName: data.displayName,
      email: data.email || '',
      mobile: data.mobile || ''
    })
    
    selectedRoleIds.value = data.roles?.map(r => r.id) || []
  } catch (error) {
    console.error('Failed to load user info:', error)
    ElMessage.error('加载用户信息失败')
  }
}

// 加载角色列表
async function loadRoleList() {
  try {
    const { data } = await getRoles({ page: 1, size: 1000 })
    roleList.value = data.items
  } catch (error) {
    console.error('Failed to load role list:', error)
    ElMessage.error('加载角色列表失败')
  }
}

// 提交分配
async function handleSubmit() {
  try {
    loading.value = true
    
    await assignRoles(props.userId, selectedRoleIds.value)
    
    ElMessage.success('角色分配成功')
    emit('success')
    handleClose()
  } catch (error) {
    console.error('Failed to assign roles:', error)
    ElMessage.error('角色分配失败')
  } finally {
    loading.value = false
  }
}

// 关闭弹窗
function handleClose() {
  dialogVisible.value = false
}

// 重置数据
function resetData() {
  searchKeyword.value = ''
  selectedRoleIds.value = []
  Object.assign(userInfo, {
    username: '',
    displayName: '',
    email: '',
    mobile: ''
  })
}
</script>

<style lang="scss" scoped>
.user-info {
  margin-bottom: 20px;
}

.role-assignment {
  .role-list {
    .list-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
      
      span {
        font-weight: 500;
        font-size: 14px;
      }
    }
    
    .list-content {
      max-height: 300px;
      overflow-y: auto;
      border: 1px solid var(--el-border-color);
      border-radius: 4px;
      padding: 12px;
      
      .role-item {
        margin-bottom: 12px;
        
        &:last-child {
          margin-bottom: 0;
        }
        
        .role-info {
          display: flex;
          align-items: center;
          gap: 8px;
          margin-left: 8px;
          
          .role-name {
            font-weight: 500;
          }
          
          .role-desc {
            color: var(--el-text-color-secondary);
            font-size: 12px;
          }
        }
      }
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>