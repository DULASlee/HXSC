<template>
  <!-- 单个菜单项 -->
  <el-menu-item
    v-if="!hasChildren"
    :index="resolvePath"
    :class="{ 'is-active': isActive }"
  >
    <Icon v-if="menu.meta?.icon" :name="menu.meta.icon" />
    <template #title>
      <span>{{ $t(menu.meta?.title || '') }}</span>
    </template>
  </el-menu-item>

  <!-- 子菜单 -->
  <el-sub-menu
    v-else
    :index="resolvePath"
    :class="{ 'is-active': hasActiveChild }"
  >
    <template #title>
      <Icon v-if="menu.meta?.icon" :name="menu.meta.icon" />
      <span>{{ $t(menu.meta?.title || '') }}</span>
    </template>

    <sidebar-menu-item
      v-for="child in menu.children"
      :key="child.path || child.name"
      :menu="child"
      :collapse="collapse"
      :base-path="resolvePath"
    />
  </el-sub-menu>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'
import type { MenuRoute } from '@/stores/menu'
import { useI18n } from 'vue-i18n';
import Icon from '@/components/common/Icon.vue'

interface Props {
  menu: MenuRoute
  collapse: boolean
  basePath?: string
}

const props = defineProps<Props>()
const route = useRoute()
const { t } = useI18n(); // 引入国际化

// 解析完整路径
const resolvePath = computed(() => {
  if (props.menu.path?.startsWith('/')) {
    return props.menu.path
  }
  
  const basePath = props.basePath || ''
  return basePath + '/' + (props.menu.path || props.menu.name)
})

// 是否有子菜单
const hasChildren = computed(() => {
  return props.menu.children && props.menu.children.length > 0
})

// 是否为当前激活菜单
const isActive = computed(() => {
  return route.path === resolvePath.value || route.meta.activeMenu === resolvePath.value
})

// 是否有激活的子菜单
const hasActiveChild = computed(() => {
  if (!hasChildren.value) return false
  
  function checkActiveChild(children: MenuRoute[]): boolean {
    return children.some(child => {
      const childPath = child.path?.startsWith('/') 
        ? child.path 
        : resolvePath.value + '/' + (child.path || child.name)
      
      if (route.path === childPath || route.meta.activeMenu === childPath) {
        return true
      }
      
      if (child.children && child.children.length > 0) {
        return checkActiveChild(child.children)
      }
      
      return false
    })
  }
  
  return checkActiveChild(props.menu.children!)
})
</script>

<style lang="scss" scoped>
// 样式继承自父组件 SidebarMenu
</style>