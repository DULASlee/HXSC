<template>
  <el-breadcrumb class="breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item
        v-for="(item, index) in breadcrumbs"
        :key="item.path"
        :to="index === breadcrumbs.length - 1 ? undefined : item.path"
        :class="{ 'is-active': index === breadcrumbs.length - 1 }"
      >
        <el-icon v-if="item.meta?.icon && index === 0">
          <component :is="item.meta.icon" />
        </el-icon>
        <span>{{ item.meta?.title }}</span>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMenuStore } from '@/stores/menu'

const route = useRoute()
const router = useRouter()
const menuStore = useMenuStore()

// 生成面包屑导航
const breadcrumbs = computed(() => {
  const matched = route.matched.filter(item => item.meta && item.meta.title)
  
  // 如果第一个不是首页，添加首页
  const first = matched[0]
  if (first && first.name !== 'Dashboard') {
    matched.unshift({
      path: '/dashboard',
      name: 'Dashboard',
      meta: { title: '首页', icon: 'House' }
    } as any)
  }
  
  return matched.map(item => ({
    path: item.path,
    name: item.name,
    meta: item.meta
  }))
})
</script>

<style lang="scss" scoped>
.breadcrumb {
  display: inline-block;
  font-size: 14px;
  line-height: 50px;
  margin-left: 8px;

  :deep(.el-breadcrumb__item) {
    .el-breadcrumb__inner {
      display: flex;
      align-items: center;
      color: var(--el-text-color-secondary);
      font-weight: normal;
      
      .el-icon {
        margin-right: 4px;
        font-size: 16px;
      }
    }

    &:last-child .el-breadcrumb__inner {
      color: var(--el-text-color-primary);
      font-weight: 500;
    }

    .el-breadcrumb__inner a {
      color: var(--el-text-color-secondary);
      text-decoration: none;
      transition: color 0.3s;

      &:hover {
        color: var(--el-color-primary);
      }
    }
  }
}

// 面包屑动画
.breadcrumb-enter-active,
.breadcrumb-leave-active {
  transition: all 0.3s;
}

.breadcrumb-enter-from,
.breadcrumb-leave-to {
  opacity: 0;
  transform: translateX(20px);
}

.breadcrumb-move {
  transition: all 0.3s;
}
</style>