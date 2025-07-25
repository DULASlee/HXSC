<template>
  <div class="sidebar-menu">
    <el-menu
      :default-active="activeMenu"
      :collapse="isCollapsed"
      :unique-opened="true"
      :router="false"
      class="sidebar-menu-el"
      @select="handleMenuSelect"
    >
      <sidebar-menu-item
        v-for="menu in menuList"
        :key="menu.id"
        :menu="menu"
        :is-collapsed="isCollapsed"
        :active-path="activeMenu"
      />
    </el-menu>
  </div>
</template>

<script setup lang="ts">
import { computed, inject, watch, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useMenuStore } from '@/stores/menu'
import SidebarMenuItem from './SidebarMenuItem.vue'

const route = useRoute()
const router = useRouter()
const menuStore = useMenuStore()

// 获取折叠状态
const isCollapsed = inject('isCollapsed', false)

// 当前激活的菜单路径
const activeMenu = ref('')

// 计算属性
const menuList = computed(() => menuStore.menuTree)

/// <summary>
/// 计算当前应该激活的菜单路径
/// </summary>
const calculateActiveMenu = () => {
  const { meta, path, matched } = route
  
  // 1. 优先使用路由元数据中的 activeMenuPath（在路由守卫中设置）
  if (meta?.activeMenuPath) {
    return meta.activeMenuPath
  }
  
  // 2. 使用路由元数据中的 activeMenu
  if (meta?.activeMenu) {
    return meta.activeMenu
  }
  
  // 3. 处理多级路由 - 查找有效的父级菜单
  if (matched.length >= 3) {
    // 从菜单数据中查找匹配的路径
    const flatMenus = menuStore.flatMenus
    
    // 首先尝试精确匹配
    let matchedMenu = flatMenus.find(menu => menu.path === path)
    
    // 如果没有精确匹配，则查找父级菜单
    if (!matchedMenu) {
      for (let i = matched.length - 2; i > 0; i--) {
        const parentPath = matched[i].path
        matchedMenu = flatMenus.find(menu => menu.path === parentPath)
        if (matchedMenu) break
      }
    }
    
    if (matchedMenu) {
      return matchedMenu.path
    }
  }
  
  // 4. 默认使用当前路径
  return path
}

/// <summary>
/// 处理菜单选择事件
/// </summary>
const handleMenuSelect = (menuPath: string) => {
  console.log(`[菜单选择] 选中菜单: ${menuPath}`)
  
  // 查找对应的菜单项
  const menu = menuStore.flatMenus.find(m => m.path === menuPath)
  
  if (menu) {
    if (menu.isExternal) {
      // 外部链接在新窗口打开
      window.open(menu.path, '_blank')
    } else {
      // 内部路由跳转
      router.push(menuPath)
    }
  } else {
    // 直接路由跳转
    router.push(menuPath)
  }
}

/// <summary>
/// 初始化和更新激活菜单
/// </summary>
const updateActiveMenu = () => {
  const newActiveMenu = calculateActiveMenu()
  if (newActiveMenu !== activeMenu.value) {
    activeMenu.value = newActiveMenu
    console.log(`[菜单激活] 更新激活菜单: ${newActiveMenu}`)
  }
}

// 监听路由变化
watch(
  () => route.fullPath,
  () => {
    updateActiveMenu()
  },
  { immediate: true }
)

// 监听菜单数据变化
watch(
  () => menuStore.menuTree,
  () => {
    updateActiveMenu()
  },
  { immediate: true }
)
</script>

<style lang="scss" scoped>
.sidebar-menu {
  height: 100%;
  
  .sidebar-menu-el {
    border-right: none;
    height: 100%;
    background-color: var(--bg-color-overlay);
    
    :deep(.el-menu-item) {
      height: 44px;
      line-height: 44px;
      color: var(--text-color-regular);
      border-radius: 0 var(--border-radius-base) var(--border-radius-base) 0;
      margin: 1px var(--spacing-small) 1px 0;
      padding: 0 16px;
      font-size: 14px;
      transition: all 0.3s ease;
      
      &:hover {
        background-color: var(--fill-color-light);
        color: var(--primary-color);
      }
      
      &.is-active {
        background-color: var(--primary-color-light-9, #ecf5ff);
        color: var(--primary-color);
        position: relative;
        font-weight: 500;
        
        &::before {
          content: '';
          position: absolute;
          left: 0;
          top: 0;
          bottom: 0;
          width: 3px;
          background-color: var(--primary-color);
          border-radius: 0 2px 2px 0;
        }
      }
      
      .el-icon {
        margin-right: var(--spacing-small);
        font-size: 16px;
        width: 16px;
        text-align: center;
        transition: transform 0.3s ease;
      }
    }
    
    :deep(.el-submenu) {
      .el-submenu__title {
        height: 44px;
        line-height: 44px;
        color: var(--text-color-regular);
        border-radius: 0 var(--border-radius-base) var(--border-radius-base) 0;
        margin: 1px var(--spacing-small) 1px 0;
        padding: 0 16px;
        font-size: 14px;
        transition: all 0.3s ease;
        
        &:hover {
          background-color: var(--fill-color-light);
          color: var(--primary-color);
        }
        
        .el-icon {
          margin-right: var(--spacing-small);
          font-size: 16px;
          width: 16px;
          text-align: center;
          transition: transform 0.3s ease;
        }
        
        .el-submenu__icon-arrow {
          right: var(--spacing-medium);
          transition: transform 0.3s ease;
        }
      }
      
      &.is-opened {
        .el-submenu__title {
          color: var(--primary-color);
          font-weight: 500;
          background-color: var(--fill-color-light);
          
          .el-submenu__icon-arrow {
            transform: rotateZ(180deg);
          }
        }
      }
      
      &.is-active {
        .el-submenu__title {
          color: var(--primary-color);
          font-weight: 500;
          background-color: var(--primary-color-light-9, #ecf5ff);
          position: relative;
          
          &::before {
            content: '';
            position: absolute;
            left: 0;
            top: 0;
            bottom: 0;
            width: 3px;
            background-color: var(--primary-color);
            border-radius: 0 2px 2px 0;
          }
        }
      }
      
      .el-menu {
        background-color: var(--fill-color-light);
        
        .el-menu-item {
          height: 40px;
          line-height: 40px;
          padding: 0 16px 0 40px !important;
          background-color: transparent;
          font-size: 13px;
          margin: 1px var(--spacing-small) 1px 0;
          border-radius: 0 var(--border-radius-base) var(--border-radius-base) 0;
          transition: all 0.3s ease;
          
          &:hover {
            background-color: var(--fill-color);
          }
          
          &.is-active {
            background-color: var(--primary-color-light-8, #d9ecff);
            color: var(--primary-color);
            font-weight: 500;
            position: relative;
            
            &::before {
              content: '';
              position: absolute;
              left: 20px;
              top: 0;
              bottom: 0;
              width: 2px;
              background-color: var(--primary-color);
              border-radius: 1px;
            }
          }
        }
      }
    }
    
    // 折叠状态样式
    &.el-menu--collapse {
      width: var(--sidebar-collapsed-width);
      
      :deep(.el-menu-item),
      :deep(.el-submenu__title) {
        padding: 0 calc((var(--sidebar-collapsed-width) - 24px) / 2);
        margin-right: 0;
        border-radius: 0;
        text-align: center;
        
        .el-icon {
          margin-right: 0;
          font-size: 18px;
        }
        
        span {
          display: none;
        }
      }
      
      :deep(.el-submenu) {
        .el-submenu__icon-arrow {
          display: none;
        }
        
        .el-menu {
          display: none;
        }
      }
      
      // 折叠状态的悬浮提示
      :deep(.el-tooltip__trigger) {
        width: 100%;
      }
    }
  }
}

// 暗色主题适配
[data-theme="dark"] {
  .sidebar-menu {
    .sidebar-menu-el {
      :deep(.el-menu-item) {
        &:hover {
          background-color: var(--fill-color-dark);
        }
        
        &.is-active {
          background-color: var(--primary-color-dark-2);
        }
      }
      
      :deep(.el-submenu) {
        .el-submenu__title {
          &:hover {
            background-color: var(--fill-color-dark);
          }
        }
        
        &.is-opened,
        &.is-active {
          .el-submenu__title {
            background-color: var(--fill-color-dark);
          }
        }
        
        .el-menu {
          background-color: var(--fill-color-darker);
          
          .el-menu-item {
            &:hover {
              background-color: var(--fill-color-dark);
            }
            
            &.is-active {
              background-color: var(--primary-color-dark-2);
            }
          }
        }
      }
    }
  }
}
</style>