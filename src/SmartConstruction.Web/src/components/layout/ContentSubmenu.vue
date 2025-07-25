<template>
  <div class="content-submenu">
    <h3 class="submenu-title" v-if="title">{{ title }}</h3>
    
    <el-menu
      :default-active="activeMenu"
      class="submenu-menu"
      @select="handleSelect"
    >
      <template v-for="item in menuItems" :key="item.path">
        <el-menu-item :index="item.path" v-if="!item.children || item.children.length === 0">
          <el-icon v-if="item.icon"><component :is="getIconComponent(item.icon)" /></el-icon>
          <span>{{ item.title }}</span>
        </el-menu-item>
        
        <el-sub-menu :index="item.path" v-else>
          <template #title>
            <el-icon v-if="item.icon"><component :is="getIconComponent(item.icon)" /></el-icon>
            <span>{{ item.title }}</span>
          </template>
          
          <el-menu-item 
            v-for="child in item.children" 
            :key="child.path"
            :index="child.path"
          >
            <el-icon v-if="child.icon"><component :is="getIconComponent(child.icon)" /></el-icon>
            <span>{{ child.title }}</span>
          </el-menu-item>
        </el-sub-menu>
      </template>
    </el-menu>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { 
  House, User, Setting, Document, Folder, Files, DataAnalysis, 
  Monitor, Lock, Avatar, Menu as MenuIcon, OfficeBuilding, 
  Connection, Warning, Bell, TrendCharts, UserFilled, Share, 
  Postcard, Edit, Grid, Location
} from '@element-plus/icons-vue'

const props = defineProps({
  title: {
    type: String,
    default: ''
  },
  menuItems: {
    type: Array,
    required: true
  }
})

const route = useRoute()
const router = useRouter()

// 图标映射
const iconMap: Record<string, any> = {
  House, User, Setting, Document, Folder, Files, DataAnalysis,
  Monitor, Lock, Avatar, Menu: MenuIcon, OfficeBuilding,
  Connection, Warning, Bell, TrendCharts, UserFilled, Share,
  Postcard, Edit, Grid, Location, HomeFilled: House,
  // 添加更多图标映射
}

// 获取图标组件
const getIconComponent = (iconName: string) => {
  return iconMap[iconName] || Document
}

// 计算当前激活的菜单项
const activeMenu = computed(() => {
  const { meta, path } = route
  // 优先使用meta中的activeMenu，否则使用当前路径
  return meta.activeMenu || path
})

// 处理菜单选择
const handleSelect = (index: string) => {
  router.push(index)
}
</script>

<style lang="scss" scoped>
.content-submenu {
  width: var(--content-submenu-width, 180px);
  flex-shrink: 0;
  
  .submenu-title {
    font-size: 16px;
    font-weight: 500;
    margin: 0 0 16px 0;
    padding-bottom: 8px;
    border-bottom: 1px solid var(--border-color-lighter);
  }
  
  .submenu-menu {
    border-right: none;
    
    :deep(.el-menu-item) {
      height: 40px;
      line-height: 40px;
      font-size: 14px;
      padding: 0 12px;
      
      &.is-active {
        background-color: var(--primary-color-light-9);
        color: var(--primary-color);
        font-weight: 500;
      }
      
      .el-icon {
        margin-right: 8px;
        font-size: 16px;
      }
    }
    
    :deep(.el-sub-menu__title) {
      height: 40px;
      line-height: 40px;
      font-size: 14px;
      padding: 0 12px;
      
      .el-icon {
        margin-right: 8px;
        font-size: 16px;
      }
    }
  }
}

// 响应式调整
@media (max-width: 768px) {
  .content-submenu {
    width: 100%;
    margin-bottom: 16px;
    
    .submenu-menu {
      display: flex;
      flex-wrap: wrap;
      border-bottom: 1px solid var(--border-color-lighter);
      padding-bottom: 8px;
      
      :deep(.el-menu-item),
      :deep(.el-sub-menu) {
        margin-right: 16px;
        margin-bottom: 8px;
        border-radius: 4px;
      }
    }
  }
}
</style>