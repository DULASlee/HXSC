<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item
        v-for="(item, index) in breadcrumbList"
        :key="item.path"
        :to="index === breadcrumbList.length - 1 ? undefined : { path: item.path }"
        :class="{ 'is-active': index === breadcrumbList.length - 1 }"
      >
        <el-icon v-if="item.icon" class="breadcrumb-icon">
          <component :is="item.icon" />
        </el-icon>
        <span>{{ item.title }}</span>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMenuStore } from '@/stores/menu'

const route = useRoute()
const router = useRouter()
const menuStore = useMenuStore()

// 面包屑项目接口
interface BreadcrumbItem {
  title: string
  path: string
  icon?: string
}

// 计算面包屑列表
const breadcrumbList = computed(() => {
  const breadcrumbs: BreadcrumbItem[] = []
  
  // 获取匹配的路由
  const matched = route.matched.filter(item => item.meta && item.meta.title)
  
  // 添加首页
  if (route.path !== '/dashboard') {
    breadcrumbs.push({
      title: '首页',
      path: '/dashboard',
      icon: 'House'
    })
  }
  
  // 处理匹配的路由
  matched.forEach((routeItem, index) => {
    // 跳过根路由
    if (routeItem.path === '/') return
    
    const meta = routeItem.meta
    const isLast = index === matched.length - 1
    
    breadcrumbs.push({
      title: meta.title as string,
      path: isLast ? route.path : routeItem.path,
      icon: meta.icon as string
    })
  })
  
  return breadcrumbs
})

// 监听路由变化
watch(
  () => route.path,
  () => {
    // 路由变化时可以执行一些操作
  },
  { immediate: true }
)
</script>

<style lang="scss" scoped>
.app-breadcrumb {
  display: flex;
  align-items: center;
  font-size: var(--font-size-small);
  
  :deep(.el-breadcrumb__item) {
    .el-breadcrumb__inner {
      display: flex;
      align-items: center;
      color: var(--text-color-secondary);
      font-weight: normal;
      transition: var(--transition-color);
      
      &:hover {
        color: var(--primary-color);
      }
      
      .breadcrumb-icon {
        margin-right: var(--spacing-extra-small);
        font-size: var(--font-size-small);
      }
    }
    
    &:last-child {
      .el-breadcrumb__inner {
        color: var(--text-color-primary);
        font-weight: var(--font-weight-primary);
        cursor: default;
        
        &:hover {
          color: var(--text-color-primary);
        }
      }
    }
    
    .el-breadcrumb__separator {
      color: var(--text-color-placeholder);
      margin: 0 var(--spacing-small);
    }
  }
}

// 面包屑动画
.breadcrumb-enter-active {
  transition: all 0.3s ease;
}

.breadcrumb-leave-active {
  transition: all 0.3s ease;
}

.breadcrumb-enter-from {
  opacity: 0;
  transform: translateX(20px);
}

.breadcrumb-leave-to {
  opacity: 0;
  transform: translateX(-20px);
}

.breadcrumb-move {
  transition: transform 0.3s ease;
}

// 响应式适配
@include respond-to(xs) {
  .app-breadcrumb {
    font-size: var(--font-size-extra-small);
    
    :deep(.el-breadcrumb__item) {
      .el-breadcrumb__inner {
        .breadcrumb-icon {
          display: none;
        }
      }
      
      .el-breadcrumb__separator {
        margin: 0 var(--spacing-extra-small);
      }
    }
  }
}
</style>