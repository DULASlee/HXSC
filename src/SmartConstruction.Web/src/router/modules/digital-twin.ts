// =============================================
// 路由配置 - 数字孪生模块
// =============================================
import type { RouteRecordRaw } from 'vue-router';

const Layout = () => import('@/layouts/DefaultLayout.vue');

const digitalTwinRoutes: RouteRecordRaw[] = [
  {
    path: '/digital-twin',
    component: Layout,
    redirect: '/digital-twin/index',
    meta: {
      title: '数字孪生',
      icon: 'DataBoard',
      permissions: ['digital-twin:view']
    },
    children: [
      {
        path: 'index',
        name: 'DigitalTwinIndex',
        component: () => import('@/views/digital-twin/index.vue'),
        meta: {
          title: '孪生大屏',
          icon: 'DataBoard',
          affix: true,
          permissions: ['digital-twin:view']
        }
      },
      {
        path: 'command-center',
        name: 'CommandCenter',
        component: () => import('@/views/digital-twin/command-center/index.vue'),
        meta: {
          title: '指挥中心',
          permissions: ['digital-twin:command-center:view']
        }
      },
      {
        path: 'attendance',
        name: 'DigitalTwinAttendance',
        component: () => import('@/views/digital-twin/attendance/index.vue'),
        meta: {
          title: '实名制考勤',
          permissions: ['digital-twin:attendance:view']
        }
      },
      {
        path: 'crane-elevator',
        name: 'CraneElevator',
        component: () => import('@/views/digital-twin/crane-elevator/index.vue'),
        meta: {
          title: '塔吊监控',
          permissions: ['digital-twin:crane:view']
        }
      },
      {
        path: 'environment',
        name: 'DigitalTwinEnvironment',
        component: () => import('@/views/digital-twin/environment/index.vue'),
        meta: {
          title: '扬尘噪音',
          permissions: ['digital-twin:environment:view']
        }
      },
      {
        path: 'video-security',
        name: 'VideoSecurity',
        component: () => import('@/views/digital-twin/video-security/index.vue'),
        meta: {
          title: '视频安防',
          permissions: ['digital-twin:video:view']
        }
      }
    ]
  }
];

export default digitalTwinRoutes; 