<template>
  <div class="sidebar-menu-item">
    <!-- 单级菜单 -->
    <el-menu-item 
      v-if="!menu.children || menu.children.length === 0"
      :index="menu.fullPath || ''"
      @click="handleMenuClick(menu)"
    >
      <el-icon v-if="menu.icon">
        <component :is="getIconComponent(menu.icon)" />
      </el-icon>
      <template #title>
        <span>{{ menu.name }}</span>
      </template>
    </el-menu-item>
    
    <!-- 多级菜单 -->
    <el-sub-menu 
      v-else
      :index="menu.fullPath || ''"
    >
      <template #title>
        <el-icon v-if="menu.icon">
          <component :is="getIconComponent(menu.icon)" />
        </el-icon>
        <span>{{ menu.name }}</span>
      </template>
      
      <sidebar-menu-item
        v-for="child in menu.children"
        :key="child.id"
        :menu="child"
        :is-collapsed="isCollapsed"
      />
    </el-sub-menu>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'

// 组件名称定义
defineOptions({
  name: 'SidebarMenuItem'
})

// 菜单类型定义
interface Menu {
  id: string
  code: string
  name: string
  type: string
  path?: string
  icon?: string
  isVisible: boolean
  isEnabled: boolean
  isExternal?: boolean
  children?: Menu[]
  fullPath?: string // 确保接口定义一致
}

interface Props {
  menu: Menu
  isCollapsed: boolean
}

const props = withDefaults(defineProps<Props>(), {})

const router = useRouter()

// 图标映射
import { 
  House,
  User,
  Setting,
  Document,
  Folder,
  Files,
  DataAnalysis,
  Monitor,
  Lock,
  Avatar,
  Menu as MenuIcon,
  OfficeBuilding,
  Connection,
  Warning,
  Bell,
  TrendCharts,
  UserFilled,
  Share,
  Postcard
} from '@element-plus/icons-vue'

const iconMap: Record<string, any> = {
  House,
  User,
  Setting,
  Document,
  Folder,
  Files,
  DataAnalysis,
  Monitor,
  Lock,
  Avatar,
  Menu: MenuIcon,
  OfficeBuilding,
  Connection,
  Warning,
  Bell,
  TrendCharts,
  UserFilled,
  Share,
  Postcard,
  HomeFilled: House,
}

/// <summary>
/// 获取图标组件
/// </summary>
const getIconComponent = (iconName: string) => {
  return iconMap[iconName] || Document
}

// 不再需要 visibleChildren, hasVisibleChildren, isActive, hasActiveChild 这些复杂的计算属性

/// <summary>
/// 处理菜单点击事件
/// </summary>
const handleMenuClick = (menu: Menu) => {
  // 使用 fullPath 进行导航
  const path = menu.fullPath
  if (!path) return
  
  console.log(`[菜单点击] ${menu.name}: ${path}`)
  
  if (menu.isExternal) {
    // 外部链接在新窗口打开
    window.open(path, '_blank')
  } else {
    // 内部路由跳转
    router.push(path)
  }
}
</script>

<style lang="scss" scoped>
.sidebar-menu-item {
  // 样式由父组件定义
}
</style>