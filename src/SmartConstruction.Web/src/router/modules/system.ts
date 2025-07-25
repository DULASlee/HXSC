// =============================================
// 系统管理路由配置
// =============================================
import type { RouteRecordRaw } from 'vue-router'

// 动态导入布局组件
const Layout = () => import('@/layouts/DefaultLayout.vue');

// 系统管理路由
const systemRoutes: RouteRecordRaw[] = [
  {
    path: '/system',
    component: Layout,
    redirect: '/system/user',
    name: 'SystemManagement',
    meta: { title: '系统管理', icon: 'Setting', permission: 'system.view' },
    children: [
      {
        path: 'user',
        name: 'UserManagement',
        component: () => import('@/views/system/user/index.vue'),
        meta: { title: '用户管理', icon: 'User', permission: 'user.view' }
      },
      {
        path: 'role',
        name: 'RoleManagement',
        component: () => import('@/views/system/role/index.vue'),
        meta: { title: '角色管理', icon: 'Avatar', permission: 'role.view' }
      },
      {
        path: 'menu',
        name: 'MenuManagement',
        component: () => import('@/views/system/menu/index.vue'),
        meta: { title: '菜单管理', icon: 'Menu', permission: 'menu.view' }
      },
      {
        path: 'permission',
        name: 'PermissionManagement',
        component: () => import('@/views/system/permission/index.vue'),
        meta: { title: '权限管理', icon: 'Lock', permission: 'permission.view' }
      },
      {
        path: 'resource',
        name: 'ResourceManagement',
        component: () => import('@/views/system/resource/index.vue'),
        meta: { title: '资源管理', icon: 'Files', permission: 'resource.view' }
      },
      {
        path: 'settings',
        name: 'SystemSettings',
        component: () => import('@/views/system/settings/index.vue'),
        meta: { title: '系统设置', icon: 'Setting', permission: 'system.settings' }
      }
    ]
  },
  {
    path: '/metadata',
    component: Layout,
    redirect: '/metadata/definition',
    name: 'MetadataManagement',
    meta: { title: '元数据管理', icon: 'DataAnalysis', permission: 'metadata.view' },
    children: [
      {
        path: 'definition',
        name: 'MetadataDefinition',
        component: () => import('@/views/metadata/definition/index.vue'),
        meta: { title: '元数据定义', icon: 'Document', permission: 'metadata.definition.view' }
      },
      {
        path: 'config',
        name: 'MetadataConfig',
        component: () => import('@/views/metadata/config/index.vue'),
        meta: { title: '元数据配置', icon: 'Setting', permission: 'metadata.config.view' }
      },
      {
        path: 'preview',
        name: 'MetadataPreview',
        component: () => import('@/views/metadata/preview/index.vue'),
        meta: { title: '动态表单预览', icon: 'View', permission: 'metadata.preview' }
      }
    ]
  },
  {
    path: '/monitor',
    component: Layout,
    redirect: '/monitor/online',
    name: 'MonitorManagement',
    meta: { title: '系统监控', icon: 'Monitor', permission: 'monitor.view' },
    children: [
      {
        path: 'online',
        name: 'OnlineUser',
        component: () => import('@/views/monitor/online/index.vue'),
        meta: { title: '在线用户', icon: 'UserFilled', permission: 'monitor.online' }
      },
      {
        path: 'server',
        name: 'ServerInfo',
        component: () => import('@/views/monitor/server/index.vue'),
        meta: { title: '服务监控', icon: 'Monitor', permission: 'monitor.server' }
      },
      {
        path: 'logs',
        name: 'SystemLogs',
        component: () => import('@/views/monitor/logs/index.vue'),
        meta: { title: '系统日志', icon: 'Document', permission: 'monitor.logs' }
      },
      {
        path: 'performance',
        name: 'Performance',
        component: () => import('@/views/monitor/performance/index.vue'),
        meta: { title: '性能监控', icon: 'TrendCharts', permission: 'monitor.performance' }
      }
    ]
  }
]

export default systemRoutes 