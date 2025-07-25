<template>
  <div class="realtime-alert-notifier" :class="{ 'has-alerts': alerts.length > 0 }">
    <div class="alert-header" @click="toggleExpanded">
      <div class="header-left">
        <div class="alert-icon" :class="highestAlertLevel">
          <el-icon><Warning /></el-icon>
        </div>
        <div class="alert-title">
          <span>实时告警</span>
          <el-badge :value="alerts.length" :max="99" type="danger" v-if="alerts.length > 0" />
        </div>
      </div>
      <div class="header-right">
        <el-button type="text" size="small" @click.stop="handleClearAll" v-if="alerts.length > 0">
          清除全部
        </el-button>
        <el-icon :class="{ 'is-expanded': expanded }"><ArrowDown /></el-icon>
      </div>
    </div>
    
    <div class="alert-content" v-show="expanded">
      <div class="alert-list" v-if="alerts.length > 0">
        <TransitionGroup name="alert">
          <div 
            class="alert-item" 
            v-for="alert in alerts" 
            :key="alert.id"
            :class="alert.level"
          >
            <div class="alert-item-header">
              <div class="alert-level-tag" :class="alert.level">
                {{ getAlertLevelText(alert.level) }}
              </div>
              <div class="alert-time">{{ formatTime(alert.time) }}</div>
              <el-button 
                type="text" 
                size="small" 
                class="close-btn"
                @click="handleDismissAlert(alert.id)"
              >
                <el-icon><Close /></el-icon>
              </el-button>
            </div>
            <div class="alert-item-body">
              <div class="alert-source">
                <span class="label">来源:</span>
                <span class="value">{{ alert.source }}</span>
              </div>
              <div class="alert-message">{{ alert.message }}</div>
            </div>
            <div class="alert-item-footer">
              <div class="alert-location">
                <el-icon><Location /></el-icon>
                <span>{{ alert.location }}</span>
              </div>
              <div class="alert-actions">
                <el-button type="primary" size="small" @click="handleViewDetail(alert)">查看详情</el-button>
                <el-button type="success" size="small" @click="handleProcessAlert(alert)">处理</el-button>
              </div>
            </div>
          </div>
        </TransitionGroup>
      </div>
      <div class="empty-alert" v-else>
        <el-empty description="暂无告警信息" :image-size="60" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Warning, ArrowDown, Close, Location } from '@element-plus/icons-vue'

const props = defineProps({
  autoRefresh: {
    type: Boolean,
    default: true
  },
  refreshInterval: {
    type: Number,
    default: 30000 // 30秒
  }
})

const emit = defineEmits(['view-detail', 'process-alert', 'clear-all'])

// 状态
const expanded = ref(false)
const alerts = ref<any[]>([])
let refreshTimer: NodeJS.Timeout | null = null

// 计算最高告警级别
const highestAlertLevel = computed(() => {
  if (alerts.value.length === 0) return 'normal'
  
  if (alerts.value.some(alert => alert.level === 'critical')) {
    return 'critical'
  }
  
  if (alerts.value.some(alert => alert.level === 'danger')) {
    return 'danger'
  }
  
  if (alerts.value.some(alert => alert.level === 'warning')) {
    return 'warning'
  }
  
  return 'normal'
})

// 切换展开状态
const toggleExpanded = () => {
  expanded.value = !expanded.value
}

// 处理查看详情
const handleViewDetail = (alert: any) => {
  emit('view-detail', alert)
}

// 处理告警
const handleProcessAlert = (alert: any) => {
  ElMessageBox.confirm(
    `确定要处理告警 "${alert.message}" 吗？`,
    '处理告警',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }
  ).then(() => {
    // 移除告警
    alerts.value = alerts.value.filter(a => a.id !== alert.id)
    
    // 发送事件
    emit('process-alert', alert)
    
    ElMessage({
      type: 'success',
      message: '告警已处理'
    })
  }).catch(() => {})
}

// 处理忽略告警
const handleDismissAlert = (alertId: string) => {
  alerts.value = alerts.value.filter(alert => alert.id !== alertId)
}

// 处理清除全部
const handleClearAll = () => {
  ElMessageBox.confirm(
    '确定要清除所有告警吗？',
    '清除告警',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }
  ).then(() => {
    alerts.value = []
    
    emit('clear-all')
    
    ElMessage({
      type: 'success',
      message: '所有告警已清除'
    })
  }).catch(() => {})
}

// 加载告警数据
const loadAlerts = () => {
  // 模拟从API获取告警数据
  // 实际项目中应该调用真实的API
  const mockAlerts = [
    {
      id: 'alert-001',
      level: 'warning',
      time: new Date().toISOString(),
      source: '扬尘监测系统',
      message: 'PM10浓度超标，当前值: 158μg/m³，阈值: 150μg/m³',
      location: 'A区-东北角监测点',
      data: {
        currentValue: 158,
        threshold: 150,
        unit: 'μg/m³'
      }
    },
    {
      id: 'alert-002',
      level: 'danger',
      time: new Date(Date.now() - 5 * 60000).toISOString(),
      source: '噪音监测系统',
      message: '噪音超标，当前值: 92dB，阈值: 85dB',
      location: 'B区-施工现场',
      data: {
        currentValue: 92,
        threshold: 85,
        unit: 'dB'
      }
    }
  ]
  
  // 随机添加或不添加告警
  if (Math.random() > 0.7) {
    const newAlert = {
      id: `alert-${Date.now()}`,
      level: Math.random() > 0.7 ? 'danger' : 'warning',
      time: new Date().toISOString(),
      source: Math.random() > 0.5 ? '扬尘监测系统' : '噪音监测系统',
      message: Math.random() > 0.5 
        ? `PM2.5浓度超标，当前值: ${Math.floor(80 + Math.random() * 50)}μg/m³，阈值: 75μg/m³`
        : `噪音超标，当前值: ${Math.floor(85 + Math.random() * 15)}dB，阈值: 85dB`,
      location: Math.random() > 0.5 ? 'A区-东北角监测点' : 'B区-施工现场',
      data: {
        currentValue: Math.floor(80 + Math.random() * 50),
        threshold: 75,
        unit: Math.random() > 0.5 ? 'μg/m³' : 'dB'
      }
    }
    
    // 添加新告警到列表开头
    alerts.value = [newAlert, ...alerts.value]
    
    // 如果告警数量超过10个，移除最旧的
    if (alerts.value.length > 10) {
      alerts.value = alerts.value.slice(0, 10)
    }
    
    // 如果有高级别告警，自动展开
    if (newAlert.level === 'danger' || newAlert.level === 'critical') {
      expanded.value = true
    }
    
    // 播放告警声音
    playAlertSound(newAlert.level)
  }
}

// 播放告警声音
const playAlertSound = (level: string) => {
  // 实际项目中可以根据不同级别播放不同的声音
  // 这里只是一个示例
  try {
    const audio = new Audio()
    audio.src = level === 'danger' || level === 'critical'
      ? '/sounds/critical-alert.mp3'
      : '/sounds/warning-alert.mp3'
    audio.play()
  } catch (error) {
    console.error('Failed to play alert sound:', error)
  }
}

// 启动自动刷新
const startAutoRefresh = () => {
  if (props.autoRefresh) {
    refreshTimer = setInterval(() => {
      loadAlerts()
    }, props.refreshInterval)
  }
}

// 停止自动刷新
const stopAutoRefresh = () => {
  if (refreshTimer) {
    clearInterval(refreshTimer)
    refreshTimer = null
  }
}

// 工具函数
const getAlertLevelText = (level: string) => {
  const levelMap: Record<string, string> = {
    'critical': '紧急',
    'danger': '严重',
    'warning': '警告',
    'info': '提示'
  }
  return levelMap[level] || level
}

const formatTime = (time: string) => {
  if (!time) return ''
  
  const date = new Date(time)
  const now = new Date()
  
  // 如果是今天，只显示时间
  if (date.toDateString() === now.toDateString()) {
    return date.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })
  }
  
  // 否则显示日期和时间
  return date.toLocaleString('zh-CN', {
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit'
  })
}

// 生命周期钩子
onMounted(() => {
  // 初始化加载
  loadAlerts()
  
  // 启动自动刷新
  startAutoRefresh()
})

onUnmounted(() => {
  // 停止自动刷新
  stopAutoRefresh()
})

// 公开方法
defineExpose({
  addAlert: (alert: any) => {
    alerts.value = [alert, ...alerts.value]
    
    // 如果告警数量超过10个，移除最旧的
    if (alerts.value.length > 10) {
      alerts.value = alerts.value.slice(0, 10)
    }
    
    // 如果有高级别告警，自动展开
    if (alert.level === 'danger' || alert.level === 'critical') {
      expanded.value = true
    }
    
    // 播放告警声音
    playAlertSound(alert.level)
  },
  clearAlerts: () => {
    alerts.value = []
  }
})
</script>

<style lang="scss" scoped>
.realtime-alert-notifier {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 400px;
  background: rgba(30, 42, 58, 0.9);
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
  z-index: 1000;
  overflow: hidden;
  transition: all 0.3s ease;
  
  &.has-alerts {
    border-left: 4px solid;
    
    &:has(.alert-icon.critical) {
      border-color: #e74c3c;
    }
    
    &:has(.alert-icon.danger) {
      border-color: #e74c3c;
    }
    
    &:has(.alert-icon.warning) {
      border-color: #f39c12;
    }
    
    &:has(.alert-icon.normal) {
      border-color: #3498db;
    }
  }
  
  .alert-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 12px 16px;
    cursor: pointer;
    
    .header-left {
      display: flex;
      align-items: center;
      gap: 12px;
      
      .alert-icon {
        width: 32px;
        height: 32px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        
        &.normal {
          background: rgba(52, 152, 219, 0.2);
          color: #3498db;
        }
        
        &.warning {
          background: rgba(243, 156, 18, 0.2);
          color: #f39c12;
        }
        
        &.danger, &.critical {
          background: rgba(231, 76, 60, 0.2);
          color: #e74c3c;
        }
        
        .el-icon {
          font-size: 18px;
        }
      }
      
      .alert-title {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 16px;
        font-weight: 600;
        color: #ffffff;
      }
    }
    
    .header-right {
      display: flex;
      align-items: center;
      gap: 8px;
      
      .el-icon {
        transition: transform 0.3s ease;
        
        &.is-expanded {
          transform: rotate(180deg);
        }
      }
    }
  }
  
  .alert-content {
    max-height: 400px;
    overflow-y: auto;
    
    .alert-list {
      padding: 0 16px 16px;
      
      .alert-item {
        margin-bottom: 12px;
        border-radius: 8px;
        overflow: hidden;
        
        &:last-child {
          margin-bottom: 0;
        }
        
        &.warning {
          background: rgba(243, 156, 18, 0.1);
          border: 1px solid rgba(243, 156, 18, 0.3);
        }
        
        &.danger, &.critical {
          background: rgba(231, 76, 60, 0.1);
          border: 1px solid rgba(231, 76, 60, 0.3);
        }
        
        .alert-item-header {
          display: flex;
          align-items: center;
          padding: 8px 12px;
          background: rgba(0, 0, 0, 0.2);
          
          .alert-level-tag {
            padding: 2px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            
            &.warning {
              background: rgba(243, 156, 18, 0.2);
              color: #f39c12;
            }
            
            &.danger, &.critical {
              background: rgba(231, 76, 60, 0.2);
              color: #e74c3c;
            }
          }
          
          .alert-time {
            margin-left: 12px;
            font-size: 12px;
            color: #7f8c8d;
          }
          
          .close-btn {
            margin-left: auto;
          }
        }
        
        .alert-item-body {
          padding: 12px;
          
          .alert-source {
            margin-bottom: 8px;
            font-size: 12px;
            
            .label {
              color: #7f8c8d;
            }
            
            .value {
              color: #bdc3c7;
              font-weight: 500;
            }
          }
          
          .alert-message {
            font-size: 14px;
            color: #ffffff;
          }
        }
        
        .alert-item-footer {
          display: flex;
          align-items: center;
          justify-content: space-between;
          padding: 8px 12px;
          background: rgba(0, 0, 0, 0.1);
          
          .alert-location {
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
    
    .empty-alert {
      padding: 20px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }
}

// 告警动画
.alert-enter-active,
.alert-leave-active {
  transition: all 0.5s ease;
}

.alert-enter-from {
  opacity: 0;
  transform: translateX(30px);
}

.alert-leave-to {
  opacity: 0;
  transform: translateX(-30px);
}

// 响应式设计
@media (max-width: 768px) {
  .realtime-alert-notifier {
    width: calc(100% - 40px);
    max-width: 400px;
  }
}
</style>