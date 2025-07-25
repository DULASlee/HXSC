<template>
  <div class="quick-actions">
    <el-card>
      <template #header>
        <div class="card-header">
          <el-icon><Lightning /></el-icon>
          <span>快速操作</span>
        </div>
      </template>
      
      <div class="actions-grid">
        <div
          v-for="action in quickActions"
          :key="action.key"
          :class="[
            'action-item',
            { 'action-item--disabled': action.disabled }
          ]"
          @click="handleAction(action)"
        >
          <div class="action-icon" :style="{ backgroundColor: action.color }">
            <el-icon :size="24">
              <component :is="action.icon" />
            </el-icon>
          </div>
          <div class="action-content">
            <div class="action-title">{{ action.title }}</div>
            <div class="action-desc">{{ action.description }}</div>
          </div>
          <el-badge 
            v-if="action.badge" 
            :value="action.badge" 
            class="action-badge"
          />
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import {
  Lightning, User, Setting, Monitor, Document,
  DataAnalysis, Warning, Upload, Download, Refresh
} from '@element-plus/icons-vue'

const router = useRouter()

// 快速操作配置
const quickActions = ref([
  {
    key: 'user-management',
    title: '用户管理',
    description: '管理系统用户',
    icon: User,
    color: '#409EFF',
    route: '/system/user',
    badge: null,
    disabled: false
  },
  {
    key: 'system-monitor',
    title: '系统监控',
    description: '查看系统状态',
    icon: Monitor,
    color: '#67C23A',
    route: '/monitor/server',
    badge: null,
    disabled: false
  },
  {
    key: 'system-logs',
    title: '系统日志',
    description: '查看系统日志',
    icon: Document,
    color: '#E6A23C',
    route: '/monitor/logs',
    badge: '5',
    disabled: false
  },
  {
    key: 'data-analysis',
    title: '数据分析',
    description: '查看数据报表',
    icon: DataAnalysis,
    color: '#9C27B0',
    route: '/dashboard',
    badge: null,
    disabled: false
  },
  {
    key: 'system-settings',
    title: '系统设置',
    description: '配置系统参数',
    icon: Setting,
    color: '#607D8B',
    route: '/system/settings',
    badge: null,
    disabled: false
  },
  {
    key: 'security-check',
    title: '安全检查',
    description: '系统安全扫描',
    icon: Warning,
    color: '#FF5722',
    action: 'security-scan',
    badge: null,
    disabled: false
  },
  {
    key: 'backup-data',
    title: '数据备份',
    description: '备份系统数据',
    icon: Upload,
    color: '#795548',
    action: 'backup',
    badge: null,
    disabled: false
  },
  {
    key: 'system-update',
    title: '系统更新',
    description: '检查系统更新',
    icon: Download,
    color: '#FF9800',
    action: 'update-check',
    badge: '1',
    disabled: false
  },
  {
    key: 'cache-clear',
    title: '清理缓存',
    description: '清理系统缓存',
    icon: Refresh,
    color: '#00BCD4',
    action: 'clear-cache',
    badge: null,
    disabled: false
  }
])

// 处理操作点击
const handleAction = (action: any) => {
  if (action.disabled) {
    ElMessage.warning('该功能暂时不可用')
    return
  }
  
  if (action.route) {
    // 路由跳转
    router.push(action.route)
  } else if (action.action) {
    // 执行特定操作
    executeAction(action.action, action.title)
  }
}

// 执行特定操作
const executeAction = (actionType: string, actionTitle: string) => {
  switch (actionType) {
    case 'security-scan':
      ElMessage.info('正在进行安全扫描...')
      setTimeout(() => {
        ElMessage.success('安全扫描完成，系统安全')
      }, 2000)
      break
      
    case 'backup':
      ElMessage.info('正在备份数据...')
      setTimeout(() => {
        ElMessage.success('数据备份完成')
      }, 3000)
      break
      
    case 'update-check':
      ElMessage.info('正在检查更新...')
      setTimeout(() => {
        ElMessage.success('系统已是最新版本')
      }, 1500)
      break
      
    case 'clear-cache':
      ElMessage.info('正在清理缓存...')
      setTimeout(() => {
        ElMessage.success('缓存清理完成')
      }, 1000)
      break
      
    default:
      ElMessage.info(`执行操作：${actionTitle}`)
  }
}
</script>

<style lang="scss" scoped>
.quick-actions {
  .card-header {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    font-size: 16px;
    font-weight: 600;
    color: var(--text-primary);
  }
  
  .actions-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: var(--spacing-md);
    
    .action-item {
      display: flex;
      align-items: center;
      gap: var(--spacing-md);
      padding: var(--spacing-md);
      border: 1px solid var(--border-color-light);
      border-radius: var(--radius-lg);
      cursor: pointer;
      transition: all 0.3s ease;
      position: relative;
      
      &:hover {
        border-color: var(--primary-color);
        box-shadow: var(--shadow-sm);
        transform: translateY(-2px);
      }
      
      &--disabled {
        opacity: 0.5;
        cursor: not-allowed;
        
        &:hover {
          transform: none;
          border-color: var(--border-color-light);
          box-shadow: none;
        }
      }
      
      .action-icon {
        width: 48px;
        height: 48px;
        border-radius: var(--radius-md);
        display: flex;
        align-items: center;
        justify-content: center;
        color: white;
        flex-shrink: 0;
      }
      
      .action-content {
        flex: 1;
        
        .action-title {
          font-weight: 600;
          color: var(--text-primary);
          margin-bottom: 4px;
          font-size: 14px;
        }
        
        .action-desc {
          font-size: 12px;
          color: var(--text-secondary);
          line-height: 1.4;
        }
      }
      
      .action-badge {
        position: absolute;
        top: -8px;
        right: -8px;
      }
    }
  }
  
  // 响应式适配
  @media (max-width: 768px) {
    .actions-grid {
      grid-template-columns: 1fr;
      gap: var(--spacing-sm);
      
      .action-item {
        padding: var(--spacing-sm);
        
        .action-icon {
          width: 40px;
          height: 40px;
        }
        
        .action-content {
          .action-title {
            font-size: 13px;
          }
          
          .action-desc {
            font-size: 11px;
          }
        }
      }
    }
  }
}
</style>