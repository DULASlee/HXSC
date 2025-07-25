<template>
  <div class="alert-notification-panel" :class="{ 'is-expanded': isExpanded }">
    <div class="panel-header" @click="toggleExpand">
      <div class="header-left">
        <div class="alert-icon" :class="{ 'has-alerts': hasAlerts }">
          <el-icon><Bell /></el-icon>
          <div class="alert-badge" v-if="totalAlerts > 0">{{ totalAlerts > 99 ? '99+' : totalAlerts }}</div>
        </div>
        <h3 class="panel-title">实时告警</h3>
      </div>
      <div class="header-right">
        <span class="alert-count" v-if="totalAlerts > 0">{{ totalAlerts }}条告警</span>
        <el-button type="text" @click.stop="clearAllAlerts" v-if="totalAlerts > 0">
          清空
        </el-button>
        <el-icon class="expand-icon" :class="{ 'is-expanded': isExpanded }">
          <ArrowRight />
        </el-icon>
      </div>
    </div>
    
    <div class="panel-content" v-if="isExpanded">
      <div class="alert-filters">
        <el-radio-group v-model="currentFilter" size="small">
          <el-radio-button label="all">全部</el-radio-button>
          <el-radio-button label="critical">严重</el-radio-button>
          <el-radio-button label="warning">警告</el-radio-button>
          <el-radio-button label="info">提示</el-radio-button>
        </el-radio-group>
        
        <el-select v-model="currentCategory" placeholder="类别" size="small">
          <el-option label="全部类别" value="all" />
          <el-option v-for="category in categories" :key="category.value" :label="category.label" :value="category.value" />
        </el-select>
      </div>
      
      <div class="alerts-container" v-if="filteredAlerts.length > 0">
        <div 
          class="alert-item"
          v-for="alert in filteredAlerts"
          :key="alert.id"
          :class="[
            `alert-level-${alert.level.toLowerCase()}`,
            { 'is-unread': !alert.read }
          ]"
        >
          <div class="alert-header">
            <div class="alert-level">{{ getLevelText(alert.level) }}</div>
            <div class="alert-time">{{ formatTime(alert.time) }}</div>
          </div>
          <div class="alert-body">
            <div class="alert-title">{{ alert.title }}</div>
            <div class="alert-message">{{ alert.message }}</div>
          </div>
          <div class="alert-footer">
            <div class="alert-source">
              <el-icon><Location /></el-icon>
              <span>{{ alert.source }}</span>
            </div>
            <div class="alert-actions">
              <el-button 
                type="primary" 
                size="small" 
                plain 
                @click="handleAlertAction(alert, 'view')"
              >
                查看
              </el-button>
              <el-button 
                type="success" 
                size="small" 
                plain 
                @click="handleAlertAction(alert, 'process')"
              >
                处理
              </el-button>
            </div>
          </div>
        </div>
      </div>
      
      <div class="empty-alerts" v-else>
        <el-empty description="暂无告警信息" :image-size="80">
          <template #image>
            <el-icon class="empty-icon"><Bell /></el-icon>
          </template>
        </el-empty>
      </div>
      
      <div class="panel-footer" v-if="filteredAlerts.length > 0">
        <el-pagination
          :current-page="currentPage"
          :page-size="pageSize"
          layout="prev, pager, next"
          :total="totalFilteredAlerts"
          @update:current-page="handlePageChange"
          small
        />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Bell, ArrowRight, Location } from '@element-plus/icons-vue'

interface Alert {
  id: string | number;
  level: 'Critical' | 'Warning' | 'Info';
  category: 'environment' | 'equipment' | 'personnel' | 'system';
  title: string;
  message: string;
  source: string;
  time: string;
  read: boolean;
}

const props = defineProps({
  alerts: {
    type: Array as () => Alert[],
    default: () => []
  },
  autoExpand: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['alert-action', 'clear-all'])

// 状态变量
const isExpanded = ref(props.autoExpand)
const currentFilter = ref('all')
const currentCategory = ref('all')
const currentPage = ref(1)
const pageSize = ref(5)

// 类别选项
const categories = [
  { label: '环境监测', value: 'environment' },
  { label: '设备安全', value: 'equipment' },
  { label: '人员安全', value: 'personnel' },
  { label: '系统告警', value: 'system' }
]

// 计算属性
const totalAlerts = computed(() => props.alerts.length)

const hasAlerts = computed(() => totalAlerts.value > 0)

const filteredAlerts = computed(() => {
  let filtered = [...props.alerts]
  
  // 按级别过滤
  if (currentFilter.value !== 'all') {
    filtered = filtered.filter(alert => alert.level.toLowerCase() === currentFilter.value)
  }
  
  // 按类别过滤
  if (currentCategory.value !== 'all') {
    filtered = filtered.filter(alert => alert.category === currentCategory.value)
  }
  
  // 分页
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  
  return filtered.slice(start, end)
})

const totalFilteredAlerts = computed(() => {
  let filtered = [...props.alerts]
  
  // 按级别过滤
  if (currentFilter.value !== 'all') {
    filtered = filtered.filter(alert => alert.level.toLowerCase() === currentFilter.value)
  }
  
  // 按类别过滤
  if (currentCategory.value !== 'all') {
    filtered = filtered.filter(alert => alert.category === currentCategory.value)
  }
  
  return filtered.length
})

// 方法
const toggleExpand = () => {
  isExpanded.value = !isExpanded.value
}

const clearAllAlerts = () => {
  emit('clear-all')
}

const handleAlertAction = (alert: Alert, action: string) => {
  emit('alert-action', { alert, action })
}

const handlePageChange = (page: number) => {
  currentPage.value = page
}

// 工具函数
const getLevelText = (level: string) => {
  const levelMap: Record<string, string> = {
    'Critical': '严重',
    'Warning': '警告',
    'Info': '提示'
  }
  return levelMap[level] || level
}

const formatTime = (time: string) => {
  if (!time) return ''
  
  const date = new Date(time)
  const now = new Date()
  
  // 如果是今天
  if (date.toDateString() === now.toDateString()) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  }
  
  // 如果是昨天
  const yesterday = new Date(now)
  yesterday.setDate(now.getDate() - 1)
  if (date.toDateString() === yesterday.toDateString()) {
    return '昨天 ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  }
  
  // 其他日期
  return date.toLocaleDateString() + ' ' + date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
}

// 监听过滤器变化
watch([currentFilter, currentCategory], () => {
  currentPage.value = 1
})

// 监听告警变化
watch(() => props.alerts, (newAlerts) => {
  // 如果有新告警，自动展开面板
  if (newAlerts.length > 0 && !isExpanded.value) {
    isExpanded.value = true
  }
}, { deep: true })
</script>

<style lang="scss" scoped>
.alert-notification-panel {
  position: fixed;
  top: 80px;
  right: 0;
  width: 360px;
  background-color: rgba(26, 26, 46, 0.9);
  border-radius: 8px 0 0 8px;
  box-shadow: -2px 0 10px rgba(0, 0, 0, 0.3);
  z-index: 1000;
  transition: transform 0.3s ease;
  transform: translateX(calc(100% - 60px));
  
  &.is-expanded {
    transform: translateX(0);
  }
  
  .panel-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 16px;
    cursor: pointer;
    border-bottom: 1px solid rgba(255, 255, 255, 0.1);
    
    .header-left {
      display: flex;
      align-items: center;
      gap: 12px;
      
      .alert-icon {
        position: relative;
        width: 32px;
        height: 32px;
        border-radius: 50%;
        background-color: rgba(52, 152, 219, 0.2);
        display: flex;
        align-items: center;
        justify-content: center;
        color: #3498db;
        
        &.has-alerts {
          animation: pulse 2s infinite;
          background-color: rgba(231, 76, 60, 0.2);
          color: #e74c3c;
        }
        
        .alert-badge {
          position: absolute;
          top: -5px;
          right: -5px;
          min-width: 18px;
          height: 18px;
          border-radius: 9px;
          background-color: #e74c3c;
          color: #ffffff;
          font-size: 10px;
          font-weight: 600;
          display: flex;
          align-items: center;
          justify-content: center;
          padding: 0 4px;
        }
      }
      
      .panel-title {
        margin: 0;
        color: #ffffff;
        font-size: 16px;
      }
    }
    
    .header-right {
      display: flex;
      align-items: center;
      gap: 8px;
      
      .alert-count {
        font-size: 12px;
        color: #7f8c8d;
      }
      
      .expand-icon {
        transition: transform 0.3s ease;
        
        &.is-expanded {
          transform: rotate(180deg);
        }
      }
    }
  }
  
  .panel-content {
    padding: 16px;
    max-height: 500px;
    overflow-y: auto;
    
    .alert-filters {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 16px;
    }
    
    .alerts-container {
      display: flex;
      flex-direction: column;
      gap: 12px;
      
      .alert-item {
        padding: 12px;
        border-radius: 8px;
        background-color: rgba(255, 255, 255, 0.05);
        border-left: 3px solid transparent;
        
        &.is-unread {
          background-color: rgba(255, 255, 255, 0.1);
        }
        
        &.alert-level-critical {
          border-left-color: #e74c3c;
        }
        
        &.alert-level-warning {
          border-left-color: #f39c12;
        }
        
        &.alert-level-info {
          border-left-color: #3498db;
        }
        
        .alert-header {
          display: flex;
          align-items: center;
          justify-content: space-between;
          margin-bottom: 8px;
          
          .alert-level {
            padding: 2px 8px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            
            .alert-level-critical & {
              background-color: rgba(231, 76, 60, 0.2);
              color: #e74c3c;
            }
            
            .alert-level-warning & {
              background-color: rgba(243, 156, 18, 0.2);
              color: #f39c12;
            }
            
            .alert-level-info & {
              background-color: rgba(52, 152, 219, 0.2);
              color: #3498db;
            }
          }
          
          .alert-time {
            font-size: 12px;
            color: #7f8c8d;
          }
        }
        
        .alert-body {
          margin-bottom: 12px;
          
          .alert-title {
            font-size: 14px;
            font-weight: 600;
            color: #ffffff;
            margin-bottom: 4px;
          }
          
          .alert-message {
            font-size: 12px;
            color: #bdc3c7;
          }
        }
        
        .alert-footer {
          display: flex;
          align-items: center;
          justify-content: space-between;
          
          .alert-source {
            display: flex;
            align-items: center;
            gap: 4px;
            font-size: 12px;
            color: #7f8c8d;
          }
          
          .alert-actions {
            display: flex;
            gap: 8px;
          }
        }
      }
    }
    
    .empty-alerts {
      padding: 24px 0;
      
      .empty-icon {
        font-size: 32px;
        color: #7f8c8d;
      }
    }
    
    .panel-footer {
      margin-top: 16px;
      display: flex;
      justify-content: center;
    }
  }
}

@keyframes pulse {
  0% {
    box-shadow: 0 0 0 0 rgba(231, 76, 60, 0.4);
  }
  70% {
    box-shadow: 0 0 0 10px rgba(231, 76, 60, 0);
  }
  100% {
    box-shadow: 0 0 0 0 rgba(231, 76, 60, 0);
  }
}

// 响应式设计
@media (max-width: 768px) {
  .alert-notification-panel {
    width: 300px;
    top: 60px;
  }
}

@media (max-width: 480px) {
  .alert-notification-panel {
    width: 280px;
    
    .panel-content {
      max-height: 400px;
    }
    
    .alert-filters {
      flex-direction: column;
      align-items: flex-start;
      gap: 8px;
      
      .el-radio-group,
      .el-select {
        width: 100%;
      }
    }
    
    .alert-item {
      .alert-footer {
        flex-direction: column;
        align-items: flex-start;
        gap: 8px;
        
        .alert-actions {
          width: 100%;
          justify-content: flex-end;
        }
      }
    }
  }
}
</style>