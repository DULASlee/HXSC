<template>
  <el-form
    ref="formRef"
    :model="formData"
    :inline="true"
    class="search-form"
    @submit.prevent="handleSearch"
  >
    <slot></slot>
    
    <el-form-item>
      <el-button type="primary" @click="handleSearch">
        <el-icon><Search /></el-icon>
        查询
      </el-button>
      <el-button @click="handleReset">
        <el-icon><Refresh /></el-icon>
        重置
      </el-button>
      <slot name="actions"></slot>
    </el-form-item>
  </el-form>
</template>

<script setup lang="ts">
import { ref, reactive, watch } from 'vue'
import { Search, Refresh } from '@element-plus/icons-vue'

const props = defineProps({
  initialValues: {
    type: Object,
    default: () => ({})
  }
})

const emit = defineEmits(['search', 'reset'])

const formRef = ref()
const formData = reactive({ ...props.initialValues })

// 监听初始值变化
watch(
  () => props.initialValues,
  (newValues) => {
    Object.assign(formData, newValues)
  },
  { deep: true }
)

// 查询
const handleSearch = () => {
  emit('search', { ...formData })
}

// 重置
const handleReset = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
  
  // 重置为初始值
  Object.keys(formData).forEach(key => {
    formData[key] = props.initialValues[key] || undefined
  })
  
  emit('reset', { ...formData })
}

// 暴露方法
defineExpose({
  formData,
  reset: handleReset,
  search: handleSearch
})
</script>

<style lang="scss" scoped>
.search-form {
  margin-bottom: 20px;
  padding: 20px;
  background-color: var(--el-bg-color);
  border-radius: 4px;
  box-shadow: var(--el-box-shadow-light);
  
  :deep(.el-form-item) {
    margin-bottom: 10px;
    margin-right: 20px;
  }
  
  :deep(.el-form-item__content) {
    display: flex;
    flex-wrap: wrap;
  }
}
</style>