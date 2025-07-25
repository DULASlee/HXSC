<template>
  <el-select
    v-model="selectedValue"
    :placeholder="placeholder"
    :multiple="multiple"
    :clearable="clearable"
    :filterable="filterable"
    :remote="remote"
    :remote-method="handleRemoteSearch"
    :loading="loading"
    :disabled="disabled"
    :reserve-keyword="reserveKeyword"
    @change="handleChange"
    @visible-change="handleVisibleChange"
  >
    <el-option
      v-for="user in options"
      :key="user.id"
      :label="user.displayName"
      :value="user.id"
    >
      <div class="user-option">
        <el-avatar :size="24" :src="user.avatar">
          <el-icon><User /></el-icon>
        </el-avatar>
        <div class="user-info">
          <div class="user-name">{{ user.displayName }}</div>
          <div class="user-detail">
            {{ user.organizationName }} - {{ user.username }}
          </div>
        </div>
        <el-tag v-if="user.status !== 1" type="danger" size="small">
          {{ getStatusLabel(user.status) }}
        </el-tag>
      </div>
    </el-option>
  </el-select>
</template>

<script setup lang="ts">
// 组合式API和工具函数已由unplugin-auto-import自动导入
interface Props {
  modelValue?: string | string[]
  placeholder?: string
  multiple?: boolean
  clearable?: boolean
  filterable?: boolean
  remote?: boolean
  disabled?: boolean
  reserveKeyword?: boolean
  organizationId?: string
  roleIds?: string[]
  status?: number
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请选择用户',
  multiple: false,
  clearable: true,
  filterable: true,
  remote: true,
  disabled: false,
  reserveKeyword: false,
  status: 1
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedValue = ref(props.modelValue)

const { data: options, loading, execute: executeSearch } = useApi(searchUsers, { 
  initialData: [],
  transform: (response) => response.items || [] 
})

// 获取状态标签
const getStatusLabel = (status: number) => ({ 0: '禁用', 1: '正常', 2: '锁定' }[status] || '未知')

// 处理值变化
const handleChange = (value: any) => {
  emit('update:modelValue', value)
  emit('change', value)
}

// 远程搜索
const handleRemoteSearch = (query: string) => {
  if (!query) {
    options.value = []
    return
  }
  executeSearch({
    keyword: query,
    organizationId: props.organizationId,
    roleIds: props.roleIds,
    status: props.status,
    pageIndex: 1,
    pageSize: 20
  })
}

// 处理下拉框显示状态变化
const handleVisibleChange = (visible: boolean) => {
  if (visible && !props.remote && options.value.length === 0) {
    executeSearch({ 
      keyword: '', 
      pageIndex: 1, 
      pageSize: 50,
      organizationId: props.organizationId,
      roleIds: props.roleIds,
      status: props.status
    })
  }
}

// 根据ID加载初始值
const loadInitialData = async (ids: string[]) => {
  if (!ids || ids.length === 0) return
  const { data: initialUsers } = await useApi(getUserList, {
    transform: (res) => res.items || []
  }).execute({ ids, pageIndex: 1, pageSize: ids.length })
  
  if (initialUsers.value) {
    const existingIds = new Set(options.value.map(u => u.id))
    const newUsers = initialUsers.value.filter(u => !existingIds.has(u.id))
    options.value.push(...newUsers)
  }
}

// 监听外部值变化
watch(() => props.modelValue, (newValue) => {
  selectedValue.value = newValue
  if (newValue) {
    const ids = Array.isArray(newValue) ? newValue : [newValue]
    const missingIds = ids.filter(id => !options.value.some(u => u.id === id))
    if (missingIds.length > 0) {
      loadInitialData(missingIds)
    }
  }
}, { immediate: true })

// 监听过滤条件变化
watch([
  () => props.organizationId,
  () => props.roleIds,
  () => props.status
], () => {
  options.value = []
  if (!props.remote) {
    handleVisibleChange(true)
  }
}, { deep: true })
</script>

<style lang="scss" scoped>
.user-option {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 4px 0;

  .user-info {
    flex: 1;
    min-width: 0;

    .user-name {
      font-size: 14px;
      font-weight: 500;
      color: var(--el-text-color-primary);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .user-detail {
      font-size: 12px;
      color: var(--el-text-color-secondary);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
      margin-top: 2px;
    }
  }

  .el-tag {
    flex-shrink: 0;
  }
}
</style>
</style>