<template>
  <div class="app-wrapper">
    <div class="sidebar-container" :class="{ 'collapsed': !appStore.sidebar.opened }">
      <el-scrollbar wrap-class="scrollbar-wrapper">
        <sidebar-menu 
          :menus="displayMenus"
          :collapse="!appStore.sidebar.opened"
        />
      </el-scrollbar>
    </div>
    <div class="main-container" :class="{ 'collapsed-sidebar': !appStore.sidebar.opened }">
      <div class="navbar">
        <div class="left-area">
          <hamburger :is-active="appStore.sidebar.opened" @toggleClick="appStore.toggleSidebar" />
          <breadcrumb />
        </div>
        <div class="right-menu">
          <!-- 主题切换器 -->
          <theme-switcher />
          
          <!-- 语言切换器 -->
          <language-switcher />
          
          <el-dropdown trigger="click">
            <div class="avatar-wrapper">
              <img src="@/assets/vue.svg" class="user-avatar" alt="avatar">
              <el-icon><arrow-down /></el-icon>
            </div>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item>个人中心</el-dropdown-item>
                <el-dropdown-item divided @click="logout">退出登录</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
      </div>
      <app-main />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useAppStore } from '../stores/app';
import { useMenuStore } from '../stores/menu';
import { useUserStore } from '../stores/user';
import { useRoute, useRouter } from 'vue-router';
import AppMain from './components/AppMain.vue';
import Hamburger from '../components/common/Hamburger.vue';
import Breadcrumb from '../components/common/Breadcrumb.vue';
import SidebarMenu from './components/SidebarMenu.vue';
import ThemeSwitcher from '../components/common/ThemeSwitcher.vue';
import LanguageSwitcher from '../components/layout/LanguageSwitcher.vue';
import { 
  House, 
  User, 
  Setting, 
  Document,
  HomeFilled,
  UserFilled,
  ArrowDown,
  OfficeBuilding,
  Monitor,
  Calendar,
  Warning,
  Connection,
  DataAnalysis
} from '@element-plus/icons-vue';

const appStore = useAppStore();
const menuStore = useMenuStore();
const userStore = useUserStore();
const route = useRoute();
const router = useRouter();

const activeMenu = computed(() => route.path);

// 临时硬编码菜单数据，确保菜单能够显示
const displayMenus = computed(() => {
  // 如果store中有菜单数据，优先使用
  if (menuStore.routes && menuStore.routes.length > 0) {
    console.log('使用store中的菜单数据:', menuStore.routes);
    return menuStore.routes;
  }
  
  // 否则使用硬编码的菜单数据
  console.log('使用硬编码的菜单数据');
  return [
    {
      path: '/dashboard',
      name: 'Dashboard',
      meta: {
        title: '工作台',
        icon: 'House'
      }
    },
    {
      path: '/system',
      name: 'System',
      meta: {
        title: '系统管理',
        icon: 'Setting'
      },
      children: [
        {
          path: '/system/user',
          name: 'SystemUser',
          meta: {
            title: '用户管理',
            icon: 'User'
          }
        },
        {
          path: '/system/role',
          name: 'SystemRole',
          meta: {
            title: '角色管理',
            icon: 'UserFilled'
          }
        },
        {
          path: '/system/menu',
          name: 'SystemMenu',
          meta: {
            title: '菜单管理',
            icon: 'Document'
          }
        }
      ]
    },
    {
      path: '/digital-twin',
      name: 'DigitalTwin',
      meta: {
        title: '数字孪生',
        icon: 'Connection'
      },
      children: [
        {
          path: '/digital-twin/dashboard',
          name: 'DigitalTwinDashboard',
          meta: {
            title: '孪生大屏',
            icon: 'DataAnalysis'
          }
        },
        {
          path: '/digital-twin/device',
          name: 'DigitalTwinDevice',
          meta: {
            title: '设备管理',
            icon: 'Monitor'
          }
        }
      ]
    }
  ];
});

// 图标组件映射
const iconMap: Record<string, any> = {
  House,
  HomeFilled: House,
  User,
  UserFilled,
  Setting,
  Document,
  OfficeBuilding,
  Monitor,
  Calendar,
  Warning,
  Connection,
  DataAnalysis,
  // 添加更多图标映射
};

const getIconComponent = (iconName: string) => {
  return iconMap[iconName] || Document;
};

// 移除handleMenuSelect，由SidebarMenu组件处理

const logout = async () => {
  try {
    // 直接调用store的logout action
    await (userStore as any).logout();
    router.push(`/login?redirect=${route.fullPath}`);
  } catch (error) {
    console.error('Logout failed:', error);
    // 即使logout失败，也强制跳转到登录页
    router.push('/login');
  }
};
</script>

<style lang="scss" scoped>
@import "../styles/variables.module.scss";

.app-wrapper {
  position: relative;
  height: 100vh;
  width: 100%;
  display: flex;
  background-color: var(--bg-body);
  color: var(--text-primary);
  transition: background-color 0.3s ease, color 0.3s ease;
  
  // 响应式布局修复
  @media (max-width: 768px) {
    flex-direction: column;
  }
}

.sidebar-container {
  transition: width 0.28s ease, background-color 0.3s ease;
  width: $sideBarWidth;
  background-color: var(--sidebar-bg);
  height: 100vh;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1001;
  overflow: hidden;
  box-shadow: var(--shadow-md);
  
  // 折叠状态
  &.collapsed {
    width: $sideBarCollapsedWidth;
  }
  
  // 移动端适配
  @media (max-width: 768px) {
    transform: translateX(-100%);
    transition: transform 0.3s ease, background-color 0.3s ease;
    
    &.mobile-open {
      transform: translateX(0);
    }
  }
}

.main-container {
  min-height: 100vh;
  width: calc(100% - #{$sideBarWidth});
  margin-left: $sideBarWidth;
  transition: margin-left 0.28s ease, width 0.28s ease;
  display: flex;
  flex-direction: column;
  
  // 侧边栏折叠时调整
  &.collapsed-sidebar {
    margin-left: $sideBarCollapsedWidth;
    width: calc(100% - #{$sideBarCollapsedWidth});
  }
  
  // 移动端无边距
  @media (max-width: 768px) {
    margin-left: 0;
    width: 100%;
  }
}

.navbar {
  height: 50px;
  overflow: hidden;
  position: relative;
  background: var(--navbar-bg);
  border-bottom: 1px solid var(--navbar-border);
  box-shadow: var(--shadow-sm);
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 var(--spacing-md);
  flex-shrink: 0; // 防止导航栏被压缩
  z-index: 1000;
  transition: all 0.3s ease;
  
  .left-area {
    display: flex;
    align-items: center;
    gap: var(--spacing-md);
  }
  
  // 响应式适配
  @media (max-width: 768px) {
    height: 48px;
    padding: 0 var(--spacing-sm);
    
    .left-area {
      gap: var(--spacing-sm);
    }
  }
}

.right-menu {
  display: flex;
  align-items: center;
  gap: var(--spacing-sm);
  
  // 响应式适配
  @media (max-width: 768px) {
    gap: var(--spacing-xs);
  }
}

.avatar-wrapper {
  cursor: pointer;
  display: flex;
  align-items: center;
  padding: var(--spacing-xs);
  border-radius: var(--radius-md);
  transition: background-color 0.3s ease;
  color: var(--navbar-text);
  
  &:hover {
    background-color: var(--sidebar-hover-bg);
  }
}

.user-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  margin-right: var(--spacing-xs);
  border: 2px solid var(--border-color-light);
  transition: border-color 0.3s ease;
  
  // 响应式适配
  @media (max-width: 768px) {
    width: 28px;
    height: 28px;
  }
}
</style>