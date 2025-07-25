<template>
  <div class="header-search">
    <el-input
      v-model="searchQuery"
      placeholder="搜索菜单、功能..."
      :prefix-icon="Search"
      clearable
      @input="handleSearch"
      @keyup.enter="handleEnter"
      class="search-input"
    />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { Search } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const searchQuery = ref('')

const handleSearch = (value: string) => {
  // 实现搜索逻辑
  console.log('Search:', value)
}

const handleEnter = () => {
  if (searchQuery.value.trim()) {
    // 执行搜索
    console.log('Execute search:', searchQuery.value)
  }
}
</script>

<style lang="scss" scoped>
.header-search {
  .search-input {
    width: 200px;
    
    :deep(.el-input__inner) {
      background-color: var(--fill-color-light);
      border-color: transparent;
      
      &:focus {
        background-color: var(--fill-color-blank);
        border-color: var(--primary-color);
      }
    }
  }
}
</style>