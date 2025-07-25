<template>
  <el-dialog
    v-model="dialogVisible"
    title="切换租户"
    width="500px"
    :close-on-click-modal="false"
    class="tenant-switch-dialog"
  >
    <div class="tenant-container">
      <div class="current-tenant">
        <h4>当前租户</h4>
        <div class="tenant-card current">
          <div class="tenant-avatar">
            <img v-if="currentTenant?.logo" :src="currentTenant.logo" :alt="currentTenant.name">
            <el-icon v-else><OfficeBuilding /></el-icon>
          </div>
          <div class="tenant-info">
            <div class="tenant-name">{{ currentTenant?.name }}</div>
            <div class="tenant-code">{{ currentTenant?.code }}</div>
          </div>
          <div class="tenant-status">
            <el-tag type="success" size="small">当前</el-tag>
          </div>
        </div>
      </div>

      <div class="available-tenants">
        <div class="section-header">
          <h4>可切换租户</h4>
          <el-button type="primary" size="small" @click="refresh()">
            <el-icon><Refresh /></el-icon>
            刷新
          </el-button>
        </div>

        <div v-if="loading" class="loading-container">
          <el-skeleton :rows="3" animated />
        </div>

        <div v-else-if="availableTenants.length === 0" class="empty-container">
          <el-empty description="暂无可切换的租户" :image-size="80" />
        </div>

        <div v-else class="tenant-list">
          <div
            v-for="tenant in availableTenants"
            :key="tenant.id"
            :class="['tenant-card', { selected: selectedTenant?.id === tenant.id }]"
            @click="selectTenant(tenant)"
          >
            <div class="tenant-avatar">
              <img v-if="tenant.logo" :src="tenant.logo" :alt="tenant.name">
              <el-icon v-else><OfficeBuilding /></el-icon>
            </div>
            <div class="tenant-info">
              <div class="tenant-name">{{ tenant.name }}</div>
              <div class="tenant-code">{{ tenant.code }}</div>
              <div class="tenant-desc">{{ tenant.description || '暂无描述' }}</div>
            </div>
            <div class="tenant-status">
              <el-tag :type="getStatusType(tenant.status)" size="small">
                {{ getStatusText(tenant.status) }}
              </el-tag>
            </div>
          </div>
        </div>
      </div>

      <div class="recent-tenants" v-if="recentTenants.length > 0">
        <h4>最近访问</h4>
        <div class="recent-list">
          <div
            v-for="tenant in recentTenants"
            :key="tenant.id"
            class="recent-item"
            @click="selectTenant(tenant)"
          >
            <div class="recent-avatar">
              <img v-if="tenant.logo" :src="tenant.logo" :alt="tenant.name">
              <el-icon v-else><OfficeBuilding /></el-icon>
            </div>
            <div class="recent-info">
              <div class="recent-name">{{ tenant.name }}</div>
              <div class="recent-time">{{ formatTime(tenant.lastAccessTime) }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button
          type="primary"
          :loading="switchLoading"
          :disabled="!selectedTenant"
          @click="handleSwitch"
        >
          切换租户
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
// 组合式API和工具函数已由unplugin-auto-import自动导入
interface TenantWithAccess extends Tenant {
  lastAccessTime?: string
  description?: string
}

interface Props {
  modelValue: boolean
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue'])

const userStore = useUserStore()

const selectedTenant = ref<TenantWithAccess>()

// 对话框显示状态
const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 当前租户
const currentTenant = computed(() => userStore.currentTenant)

// 获取租户列表
const { data: tenantData, loading, refresh } = useApi(getUserTenants, { immediate: false })
const availableTenants = computed(() => tenantData.value?.tenants.filter(t => t.id !== currentTenant.value?.id) || [])
const recentTenants = computed(() => tenantData.value?.recentTenants || [])

// 切换租户
const { execute: executeSwitch, loading: switchLoading } = useApi(switchTenant)
async function handleSwitch() {
  if (!selectedTenant.value) return

  await ElMessageBox.confirm(`确定要切换到租户 "${selectedTenant.value.name}" 吗？`, '确认切换', { type: 'warning' })

  await executeSwitch(selectedTenant.value.id)

  await userStore.switchTenant(selectedTenant.value)
  ElMessage.success('租户切换成功，页面即将刷新...')
  dialogVisible.value = false
  setTimeout(() => window.location.reload(), 1000)
}

// 获取状态类型
function getStatusType(status: number) {
  const statusMap: Record<number, string> = { 0: 'danger', 1: 'success', 2: 'warning' }
  return statusMap[status] || 'info'
}

// 获取状态文本
function getStatusText(status: number): string {
  const statusMap: Record<number, string> = { 0: '禁用', 1: '正常', 2: '维护' }
  return statusMap[status] || '未知'
}

// 格式化时间
function formatTime(time?: string): string {
  if (!time) return ''
  const date = new Date(time)
  const now = new Date()
  const diff = now.getTime() - date.getTime()
  const minutes = Math.floor(diff / (1000 * 60))
  if (minutes < 60) return `${minutes}分钟前`
  const hours = Math.floor(diff / (1000 * 60 * 60))
  if (hours < 24) return `${hours}小时前`
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  if (days < 7) return `${days}天前`
  return date.toLocaleDateString()
}

// 选择租户
function selectTenant(tenant: TenantWithAccess) {
  selectedTenant.value = tenant
}

// 监听对话框显示状态
watch(dialogVisible, (visible) => {
  if (visible) {
    refresh()
    selectedTenant.value = undefined
  }
})
</script>

<style lang="scss" scoped>
.tenant-switch-dialog {
  :deep(.el-dialog__body) {
    padding: 20px;
  }
}

.tenant-container {
  .current-tenant,
  .available-tenants,
  .recent-tenants {
    margin-bottom: 24px;
    
    &:last-child {
      margin-bottom: 0;
    }
    
    h4 {
      margin: 0 0 12px 0;
      font-size: 14px;
      font-weight: 500;
      color: var(--el-text-color-primary);
    }
  }
  
  .section-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 12px;
    
    h4 {
      margin: 0;
    }
  }
  
  .tenant-card {
    display: flex;
    align-items: center;
    padding: 16px;
    border: 1px solid var(--el-border-color);
    border-radius: 8px;
    cursor: pointer;
    transition: all 0.3s;
    margin-bottom: 8px;
    
    &:last-child {
      margin-bottom: 0;
    }
    
    &:hover {
      border-color: var(--el-color-primary);
      background-color: var(--el-color-primary-light-9);
    }
    
    &.current {
      border-color: var(--el-color-success);
      background-color: var(--el-color-success-light-9);
      cursor: default;
    }
    
    &.selected {
      border-color: var(--el-color-primary);
      background-color: var(--el-color-primary-light-9);
    }
    
    .tenant-avatar {
      width: 48px;
      height: 48px;
      border-radius: 8px;
      overflow: hidden;
      background-color: var(--el-bg-color-page);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-right: 16px;
      
      img {
        width: 100%;
        height: 100%;
        object-fit: cover;
      }
      
      .el-icon {
        font-size: 24px;
        color: var(--el-text-color-secondary);
      }
    }
    
    .tenant-info {
      flex: 1;
      min-width: 0;
      
      .tenant-name {
        font-size: 16px;
        font-weight: 500;
        color: var(--el-text-color-primary);
        margin-bottom: 4px;
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
      
      .tenant-code {
        font-size: 12px;
        color: var(--el-text-color-secondary);
        margin-bottom: 4px;
      }
      
      .tenant-desc {
        font-size: 12px;
        color: var(--el-text-color-secondary);
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }
    
    .tenant-status {
      margin-left: 16px;
    }
  }
  
  .loading-container,
  .empty-container {
    padding: 40px 0;
    text-align: center;
  }
  
  .recent-list {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
    gap: 12px;
    
    .recent-item {
      display: flex;
      align-items: center;
      padding: 12px;
      border: 1px solid var(--el-border-color-lighter);
      border-radius: 6px;
      cursor: pointer;
      transition: all 0.3s;
      
      &:hover {
        border-color: var(--el-color-primary);
        background-color: var(--el-color-primary-light-9);
      }
      
      .recent-avatar {
        width: 32px;
        height: 32px;
        border-radius: 4px;
        overflow: hidden;
        background-color: var(--el-bg-color-page);
        display: flex;
        align-items: center;
        justify-content: center;
        margin-right: 12px;
        
        img {
          width: 100%;
          height: 100%;
          object-fit: cover;
        }
        
        .el-icon {
          font-size: 16px;
          color: var(--el-text-color-secondary);
        }
      }
      
      .recent-info {
        flex: 1;
        min-width: 0;
        
        .recent-name {
          font-size: 14px;
          font-weight: 500;
          color: var(--el-text-color-primary);
          margin-bottom: 2px;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }
        
        .recent-time {
          font-size: 11px;
          color: var(--el-text-color-secondary);
        }
      }
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>