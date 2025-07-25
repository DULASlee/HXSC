<template>
  <el-menu
    :default-active="activeMenu"
    :collapse="collapse"
    :unique-opened="true"
    :collapse-transition="false"
    mode="vertical"
    class="sidebar-menu"
    @select="handleMenuSelect"
  >
    <template v-for="menu in visibleMenus">
      <!-- 只有一个子菜单或没有子菜单的情况 -->
      <el-menu-item 
        v-if="!menu.children || menu.children.length === 0"
        :key="menu.path"
        :index="menu.path"
      >
        <el-icon v-if="menu.meta?.icon">
          <component :is="getIconComponent(menu.meta.icon)" />
        </el-icon>
        <template #title>
          <span>{{ getMenuTitle(menu) }}</span>
        </template>
      </el-menu-item>
      
      <!-- 有多个子菜单的情况 -->
      <el-sub-menu 
        v-else
        :key="menu.path"
        :index="menu.path"
      >
        <template #title>
          <el-icon v-if="menu.meta?.icon">
            <component :is="getIconComponent(menu.meta.icon)" />
          </el-icon>
          <span>{{ getMenuTitle(menu) }}</span>
        </template>
        
        <!-- 递归渲染子菜单 -->
        <template v-for="childMenu in menu.children">
          <el-menu-item 
            v-if="!childMenu.meta?.hidden"
            :key="childMenu.path"
            :index="childMenu.path"
          >
            <el-icon v-if="childMenu.meta?.icon">
              <component :is="getIconComponent(childMenu.meta.icon)" />
            </el-icon>
            <template #title>
              <span>{{ getMenuTitle(childMenu) }}</span>
            </template>
          </el-menu-item>
        </template>
      </el-sub-menu>
    </template>
  </el-menu>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useI18n } from 'vue-i18n';
import type { RouteRecordRaw } from 'vue-router';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';

// 菜单路由类型定义
interface MenuRoute {
  path: string;
  name?: string;
  component?: any;
  children?: MenuRoute[];
  meta?: {
    title?: string;
    icon?: string;
    hidden?: boolean;
    activeMenu?: string;
    keepAlive?: boolean;
    permissions?: string[];
    sortOrder?: number;
    menuData?: any;
  };
}

const props = defineProps<{
  menus: MenuRoute[];
  collapse: boolean;
}>();

const route = useRoute();
const router = useRouter();
const { t } = useI18n();

const activeMenu = computed(() => {
  const { meta, path } = route;
  return meta.activeMenu || path;
});

// 图标组件映射
const getIconComponent = (iconName: string) => {
  if (!iconName) return (ElementPlusIconsVue as any)['Document'];
  
  // 处理不同的图标命名格式
  const iconMap: Record<string, any> = {
    'House': ElementPlusIconsVue.House,
    'HomeFilled': ElementPlusIconsVue.HomeFilled,
    'User': ElementPlusIconsVue.User,
    'UserFilled': ElementPlusIconsVue.UserFilled,
    'Setting': ElementPlusIconsVue.Setting,
    'Document': ElementPlusIconsVue.Document,
    'OfficeBuilding': ElementPlusIconsVue.OfficeBuilding,
    'Monitor': ElementPlusIconsVue.Monitor,
    'Calendar': ElementPlusIconsVue.Calendar,
    'Warning': ElementPlusIconsVue.Warning,
    'Connection': ElementPlusIconsVue.Connection,
    'DataAnalysis': ElementPlusIconsVue.DataAnalysis,
    'List': ElementPlusIconsVue.List,
    'PieChart': ElementPlusIconsVue.PieChart,
    'Upload': ElementPlusIconsVue.Upload,
  };
  
  return iconMap[iconName] || (ElementPlusIconsVue as any)[iconName] || ElementPlusIconsVue.Document;
};

// 获取国际化菜单标题
const getMenuTitle = (menu: MenuRoute) => {
  // 尝试从国际化中获取翻译
  const menuKey = getMenuI18nKey(menu);
  if (menuKey) {
    const translatedTitle = t(menuKey, '');
    if (translatedTitle) {
      return translatedTitle;
    }
  }
  
  // 智能匹配：尝试根据标题关键词进行匹配
  const originalTitle = menu.meta?.title || menu.name || '';
  const smartTranslation = getSmartTranslation(originalTitle);
  if (smartTranslation) {
    return smartTranslation;
  }
  
  // 最后的回退
  return originalTitle || '未命名菜单';
};

// 智能翻译匹配
const getSmartTranslation = (title: string): string | null => {
  if (!title) return null;
  
  const lowerTitle = title.toLowerCase().trim();
  
  // 直接匹配常见关键词
  const keywordMap: Record<string, string> = {
    'dashboard': '工作台',
    'home': '首页',
    'main': '主页',
    'index': '首页',
    'overview': '概览',
    'workspace': '工作区',
    'digital twin': '数字孪生',
    'digitaltwin': '数字孪生',
    'device': '设备',
    'devices': '设备',
    'monitoring': '监控',
    'monitor': '监控',
    'visualization': '可视化',
    'project': '项目',
    'projects': '项目',
    'task': '任务',
    'tasks': '任务',
    'user': '用户',
    'users': '用户',
    'role': '角色',
    'roles': '角色',
    'permission': '权限',
    'permissions': '权限',
    'tenant': '租户',
    'tenants': '租户',
    'organization': '组织',
    'organizations': '组织',
    'system': '系统',
    'admin': '管理',
    'administration': '管理',
    'management': '管理',
    'config': '配置',
    'configuration': '配置',
    'setting': '设置',
    'settings': '设置',
    'setup': '设置',
    'log': '日志',
    'logs': '日志',
    'audit': '审计',
    'security': '安全',
    'safety': '安全',
    'attendance': '考勤',
    'report': '报表',
    'reports': '报表',
    'analytics': '分析',
    'analysis': '分析',
    'statistics': '统计',
    'stats': '统计',
    'chart': '图表',
    'charts': '图表',
    'data': '数据',
    'integration': '集成',
    'api': 'API'
  };
  
  // 精确匹配
  if (keywordMap[lowerTitle]) {
    return keywordMap[lowerTitle];
  }
  
  // 包含匹配 (处理组合词)
  for (const [keyword, translation] of Object.entries(keywordMap)) {
    if (lowerTitle.includes(keyword)) {
      // 处理常见的组合模式
      if (lowerTitle.includes('management') || lowerTitle.includes('manage')) {
        return `${translation}管理`;
      } else if (lowerTitle.includes('list')) {
        return `${translation}列表`;
      } else if (lowerTitle.includes('setting') || lowerTitle.includes('config')) {
        return `${translation}设置`;
      } else if (lowerTitle.includes('monitoring') || lowerTitle.includes('monitor')) {
        return `${translation}监控`;
      } else if (lowerTitle.includes('analysis') || lowerTitle.includes('analytics')) {
        return `${translation}分析`;
      } else {
        return translation;
      }
    }
  }
  
  return null;
};

// 获取菜单的国际化键
const getMenuI18nKey = (menu: MenuRoute): string | null => {
  // 根据菜单路径或名称生成国际化键
  const path = menu.path;
  const name = menu.name?.toLowerCase();
  const title = menu.meta?.title?.toLowerCase();
  
  // 扩展的菜单路径映射到国际化键
  const pathMap: Record<string, string> = {
    '/dashboard': 'menu.dashboard',
    '/digital-twin': 'menu.digitalTwin',
    '/digital-twin/dashboard': 'menu.digitalTwinDashboard',
    '/digital-twin/device': 'menu.deviceManagement',
    '/digital-twin/monitoring': 'menu.realTimeMonitoring',
    '/digital-twin/visualization': 'menu.dataVisualization',
    '/project': 'menu.projectManagement',
    '/project/list': 'menu.projectList',
    '/project/progress': 'menu.projectProgress',
    '/project/task': 'menu.taskManagement',
    '/project/milestone': 'menu.milestoneTracking',
    '/user': 'menu.userManagement',
    '/user/list': 'menu.userList',
    '/users': 'menu.userManagement',
    '/role': 'menu.roleManagement',
    '/roles': 'menu.roleManagement',
    '/permission': 'menu.permissionManagement',
    '/permissions': 'menu.permissionManagement',
    '/organization': 'menu.organizationManagement',
    '/organizations': 'menu.organizationManagement',
    '/tenant': 'menu.tenantManagement',
    '/tenants': 'menu.tenantManagement',
    '/tenant/list': 'menu.tenantList',
    '/tenant/settings': 'menu.tenantSettings',
    '/system': 'menu.systemManagement',
    '/system/menu': 'menu.menuManagement',
    '/system/menus': 'menu.menuManagement',
    '/system/config': 'menu.configurationManagement',
    '/system/configuration': 'menu.configurationManagement',
    '/system/audit': 'menu.auditLog',
    '/system/monitoring': 'menu.systemMonitoring',
    '/attendance': 'menu.attendanceManagement',
    '/attendance/record': 'menu.attendanceRecord',
    '/attendance/statistics': 'menu.attendanceStatistics',
    '/safety': 'menu.safetyManagement',
    '/safety/incident': 'menu.incidentReporting',
    '/safety/inspection': 'menu.safetyInspection',
    '/safety/compliance': 'menu.complianceManagement',
    '/reports': 'menu.reports',
    '/reports/analysis': 'menu.dataAnalysis',
    '/reports/bi': 'menu.businessIntelligence',
    '/reports/custom': 'menu.customReports',
    '/settings': 'menu.settings',
    '/settings/profile': 'menu.profile',
    '/settings/preferences': 'menu.preferences',
    '/settings/security': 'menu.security',
    '/integration': 'menu.integration',
    '/integration/api': 'menu.apiManagement',
    '/integration/webhooks': 'menu.webhooks',
    '/integration/third-party': 'menu.thirdPartyServices'
  };
  
  // 精确匹配路径
  if (pathMap[path]) {
    return pathMap[path];
  }
  
  // 扩展的名称映射 (支持中英文菜单名称)
  if (name) {
    const nameMap: Record<string, string> = {
      'dashboard': 'menu.dashboard',
      '工作台': 'menu.dashboard',
      'digitaltwin': 'menu.digitalTwin',
      'digital-twin': 'menu.digitalTwin',
      '数字孪生': 'menu.digitalTwin',
      'project': 'menu.projectManagement',
      'projects': 'menu.projectManagement',
      '项目管理': 'menu.projectManagement',
      'user': 'menu.userManagement',
      'users': 'menu.userManagement',
      '用户管理': 'menu.userManagement',
      'role': 'menu.roleManagement',
      'roles': 'menu.roleManagement',
      '角色管理': 'menu.roleManagement',
      'permission': 'menu.permissionManagement',
      'permissions': 'menu.permissionManagement',
      '权限管理': 'menu.permissionManagement',
      'tenant': 'menu.tenantManagement',
      'tenants': 'menu.tenantManagement',
      '租户管理': 'menu.tenantManagement',
      'system': 'menu.systemManagement',
      '系统管理': 'menu.systemManagement',
      'attendance': 'menu.attendanceManagement',
      '考勤管理': 'menu.attendanceManagement',
      'safety': 'menu.safetyManagement',
      '安全管理': 'menu.safetyManagement',
      'reports': 'menu.reports',
      '报表中心': 'menu.reports',
      'settings': 'menu.settings',
      '设置': 'menu.settings',
      'organization': 'menu.organizationManagement',
      'organizations': 'menu.organizationManagement',
      '组织管理': 'menu.organizationManagement'
    };
    
    if (nameMap[name]) {
      return nameMap[name];
    }
  }
  
  // 扩展的标题映射 (支持常见英文标题)
  if (title) {
    const titleMap: Record<string, string> = {
      'dashboard': 'menu.dashboard',
      'digital twin': 'menu.digitalTwin',
      'project management': 'menu.projectManagement',
      'user management': 'menu.userManagement',
      'role management': 'menu.roleManagement',
      'permission management': 'menu.permissionManagement',
      'tenant management': 'menu.tenantManagement',
      'system management': 'menu.systemManagement',
      'menu management': 'menu.menuManagement',
      'attendance management': 'menu.attendanceManagement',
      'safety management': 'menu.safetyManagement',
      'device management': 'menu.deviceManagement',
      'data visualization': 'menu.dataVisualization',
      'real-time monitoring': 'menu.realTimeMonitoring',
      'project list': 'menu.projectList',
      'user list': 'menu.userList',
      'tenant list': 'menu.tenantList',
      'audit log': 'menu.auditLog',
      'system monitoring': 'menu.systemMonitoring',
      'reports': 'menu.reports',
      'data analysis': 'menu.dataAnalysis',
      'business intelligence': 'menu.businessIntelligence',
      'settings': 'menu.settings',
      'profile': 'menu.profile',
      'security': 'menu.security',
      'organization management': 'menu.organizationManagement',
      'configuration management': 'menu.configurationManagement',
      'api management': 'menu.apiManagement',
      'integration': 'menu.integration'
    };
    
    if (titleMap[title]) {
      return titleMap[title];
    }
  }
  
  return null;
};

// 菜单选择处理
const handleMenuSelect = (index: string) => {
  if (index && index !== route.path) {
    router.push(index);
  }
};

// 过滤可见菜单
const visibleMenus = computed(() => {
  console.log('SidebarMenu - 原始菜单数据:', props.menus);
  
  const filtered = props.menus.filter(menu => {
    if (!menu || menu.meta?.hidden) return false;
    
    // 如果有子菜单，确保至少有一个可见的子菜单
    if (menu.children && menu.children.length > 0) {
      return menu.children.some(child => !child.meta?.hidden);
    }
    
    return true;
  });
  
  console.log('SidebarMenu - 过滤后的菜单:', filtered);
  return filtered;
});
</script>

<style lang="scss">
.sidebar-menu {
  width: 100%;
  border-right: none !important;
  background-color: #fafbfc !important; // 更淡的背景色

  .el-menu-item, .el-sub-menu__title {
    color: #2c3e50 !important; // 更深的前景色，深蓝灰
    background-color: transparent !important;
    height: 48px;
    line-height: 48px;
    margin: 2px 12px;
    border-radius: 8px;
    transition: all 0.3s ease;
    position: relative;
    overflow: hidden;
    font-weight: 500; // 字体加粗，更显高端

    .el-icon {
      color: #34495e; // 深灰蓝色图标
      margin-right: 12px;
      font-size: 18px;
      transition: color 0.3s ease;
    }
    
    &:hover {
      background-color: #e8f4fd !important; // 淡蓝色悬停背景
      color: #1a365d !important; // 更深的悬停文字
      transform: translateX(2px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08); // 轻微阴影增加层次
      
      .el-icon {
        color: #2b77e6; // 悬停时图标变为蓝色
      }
    }
  }

  .el-menu-item.is-active {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important; // 高端渐变色
    color: #ffffff !important;
    box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
    font-weight: 600;
    
    .el-icon {
      color: #ffffff;
    }
    
    &::before {
      content: '';
      position: absolute;
      left: 0;
      top: 0;
      width: 4px;
      height: 100%;
      background: linear-gradient(180deg, #ffffff 0%, rgba(255, 255, 255, 0.7) 100%);
      border-radius: 0 4px 4px 0;
    }
  }

  // 子菜单样式优化
  .el-sub-menu {
    .el-sub-menu__title {
      font-weight: 600; // 主菜单标题更粗
      
      &:hover {
        .el-sub-menu__icon-arrow {
          color: #2b77e6;
        }
      }
    }
  }

  .el-menu--inline {
    background-color: #f8fafc !important; // 子菜单更淡的背景
    margin-top: 4px;
    border-radius: 8px;
    border: 1px solid #e2e8f0; // 添加淡边框增加层次
    
    .el-menu-item {
      margin: 2px 16px 2px 24px;
      height: 42px;
      line-height: 42px;
      padding-left: 48px !important;
      font-size: 14px;
      position: relative;
      color: #475569 !important; // 子菜单文字颜色
      font-weight: 400;
      
      &:hover {
        background-color: #e2e8f0 !important; // 子菜单悬停色
        color: #1e293b !important;
        transform: translateX(4px);
      }
      
      &.is-active {
        background: linear-gradient(90deg, rgba(102, 126, 234, 0.1) 0%, rgba(118, 75, 162, 0.1) 100%) !important;
        color: #667eea !important;
        font-weight: 600;
        
        &::before {
          background-color: #667eea;
          width: 6px;
          height: 6px;
        }
      }

      &::before {
        content: '';
        position: absolute;
        left: 32px;
        top: 50%;
        transform: translateY(-50%);
        width: 4px;
        height: 4px;
        border-radius: 50%;
        background-color: #cbd5e1; // 更淡的小圆点
        transition: all 0.3s ease;
      }
    }
  }

  // 收缩状态下的样式
  &.el-menu--collapse {
    .el-menu-item, .el-sub-menu__title {
      margin: 2px 8px;
      padding: 0 !important;
      text-align: center;
      
      .el-icon {
        margin-right: 0;
        font-size: 20px;
      }
    }
  }
}

// 深色主题样式 - 高端奢华版
[data-theme="dark"] {
  .sidebar-menu {
    background-color: #1a202c !important; // 深色背景
    
    .el-menu-item, .el-sub-menu__title {
      color: #e2e8f0 !important; // 浅色文字
      
      .el-icon {
        color: #cbd5e1;
      }
      
      &:hover {
        background-color: rgba(255, 255, 255, 0.05) !important;
        color: #f7fafc !important;
        
        .el-icon {
          color: #90cdf4;
        }
      }
    }
    
    .el-menu--inline {
      background-color: rgba(0, 0, 0, 0.2) !important;
      border-color: rgba(255, 255, 255, 0.1);
      
      .el-menu-item {
        color: #a0aec0 !important;
        
        &:hover {
          background-color: rgba(255, 255, 255, 0.08) !important;
          color: #e2e8f0 !important;
        }
        
        &::before {
          background-color: #4a5568;
        }
      }
    }
  }
}
</style>