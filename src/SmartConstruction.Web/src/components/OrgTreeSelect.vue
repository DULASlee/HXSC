<template>
  <el-tree-select
    v-model="selectedValue"
    :data="treeData"
    :props="treeProps"
    :placeholder="placeholder"
    :multiple="multiple"
    :clearable="clearable"
    :filterable="filterable"
    :disabled="disabled"
    :loading="loading"
    :check-strictly="checkStrictly"
    :show-checkbox="multiple"
    node-key="id"
    @change="handleChange"
  >
    <template #default="{ node, data }">
      <div class="tree-node">
        <el-icon class="node-icon">
          <component :is="getNodeIcon(data.type)" />
        </el-icon>
        <span class="node-label">{{ data.name }}</span>
        <el-tag v-if="data.type" :type="getNodeTagType(data.type)" size="small">
          {{ getNodeTypeLabel(data.type) }}
        </el-tag>
      </div>
    </template>
  </el-tree-select>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue'
import { Office, User, UserFilled, Folder } from '@element-plus/icons-vue'

interface Props {
  modelValue?: string | string[]
  placeholder?: string
  multiple?: boolean
  clearable?: boolean
  filterable?: boolean
  disabled?: boolean
  checkStrictly?: boolean
  orgTypes?: string[]
  includeUsers?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  placeholder: '请选择组织',
  multiple: false,
  clearable: true,
  filterable: true,
  disabled: false,
  checkStrictly: false,
  orgTypes: () => ['Company', 'Department', 'Team', 'Group'],
  includeUsers: false
})

const emit = defineEmits(['update:modelValue', 'change'])

const selectedValue = ref(props.modelValue)

// 使用 useApi 获取组织树数据
const { data: treeData, loading, execute: loadTreeData } = useApi(getOrganizationTree, {
  immediate: true,
  params: () => ({
    includeUsers: props.includeUsers,
    types: props.orgTypes
  })
})

const treeProps = {
  children: 'children',
  label: 'name',
  value: 'id',
  disabled: 'disabled'
}

// 获取节点图标
const getNodeIcon = (type: string) => ({
  'Company': Office,
  'Department': Folder,
  'Team': UserFilled,
  'Group': User,
  'User': User
}[type] || Folder)

// 获取节点标签类型
const getNodeTagType = (type: string) => ({
  'Company': 'primary',
  'Department': 'success',
  'Team': 'warning',
  'Group': 'info',
  'User': 'default'
}[type] || 'default')

// 获取节点类型标签
const getNodeTypeLabel = (type: string) => ({
  'Company': '公司',
  'Department': '部门',
  'Team': '团队',
  'Group': '小组',
  'User': '用户'
}[type] || type)

// 处理值变化
const handleChange = (value: any) => {
  emit('update:modelValue', value)
  emit('change', value)
}

// 监听外部值变化
watch(() => props.modelValue, (newValue) => {
  selectedValue.value = newValue
})

// 监听选中值变化
watch(selectedValue, (newValue) => {
  if (newValue !== props.modelValue) {
    handleChange(newValue)
  }
})

// 监听过滤条件变化
watch(() => [props.orgTypes, props.includeUsers], () => {
  loadTreeData()
}, { deep: true })
</script>

<style lang="scss" scoped>
.tree-node {
  display: flex;
  align-items: center;
  gap: 8px;
  width: 100%;

  .node-icon {
    font-size: 14px;
    color: var(--el-text-color-secondary);
  }

  .node-label {
    flex: 1;
    font-size: 14px;
  }

  .el-tag {
    font-size: 10px;
    height: 18px;
    line-height: 16px;
  }
}
</style>