<template>
  <el-dropdown class="notification-dropdown" trigger="click">
    <el-button text class="notification-button">
      <el-badge :value="unreadCount" :hidden="unreadCount === 0">
        <el-icon :size="18">
          <Bell />
        </el-icon>
      </el-badge>
    </el-button>
    
    <template #dropdown>
      <el-dropdown-menu class="notification-menu">
        <div class="notification-header">
          <span>通知消息</span>
          <el-button text size="small" @click="markAllAsRead">全部已读</el-button>
        </div>
        
        <div class="notification-list">
          <div
            v-for="notification in notifications"
            :key="notification.id"
            class="notification-item"
            :class="{ 'is-unread': !notification.read }"
            @click="handleNotificationClick(notification)"
          >
            <div class="notification-content">
              <div class="notification-title">{{ notification.title }}</div>
              <div class="notification-message">{{ notification.message }}</div>
              <div class="notification-time">{{ formatTime(notification.time) }}</div>
            </div>
            <div class="notification-status" v-if="!notification.read"></div>
          </div>
        </div>
        
        <div class="notification-footer">
          <el-button text size="small" @click="viewAllNotifications">查看全部</el-button>
        </div>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { Bell } from '@element-plus/icons-vue'
import { useRouter } from 'vue-router'

const router = useRouter()

interface Notification {
  id: string
  title: string
  message: string
  time: number
  read: boolean
  type: 'info' | 'success' | 'warning' | 'error'
}

const notifications = ref<Notification[]>([
  {
    id: '1',
    title: '系统更新',
    message: '系统将在今晚22:00进行维护更新',
    time: Date.now() - 1000 * 60 * 30,
    read: false,
    type: 'info'
  },
  {
    id: '2',
    title: '新消息',
    message: '您有一条新的待办事项',
    time: Date.now() - 1000 * 60 * 60,
    read: false,
    type: 'success'
  }
])

const unreadCount = computed(() => {
  return notifications.value.filter(n => !n.read).length
})

const handleNotificationClick = (notification: Notification) => {
  notification.read = true
  // 处理通知点击
  console.log('Notification clicked:', notification)
}

const markAllAsRead = () => {
  notifications.value.forEach(n => n.read = true)
}

const viewAllNotifications = () => {
  router.push('/notifications')
}

const formatTime = (timestamp: number) => {
  const now = Date.now()
  const diff = now - timestamp
  const minutes = Math.floor(diff / (1000 * 60))
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(diff / (1000 * 60 * 60 * 24))
  
  if (minutes < 1) return '刚刚'
  if (minutes < 60) return `${minutes}分钟前`
  if (hours < 24) return `${hours}小时前`
  return `${days}天前`
}
</script>

<style lang="scss" scoped>
@import '@/styles/mixins.scss';

.notification-dropdown {
  .notification-button {
    display: flex;
    align-items: center;
    justify-content: center;
    width: 36px;
    height: 36px;
    border-radius: var(--border-radius-base);
    color: var(--text-color-regular);
    transition: var(--transition-base);
    
    &:hover {
      background-color: var(--fill-color-light);
      color: var(--primary-color);
    }
  }
}

:deep(.el-dropdown-menu) {
  width: 320px;
  max-height: 400px;
  padding: 0;
  
  .notification-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: var(--spacing-medium);
    border-bottom: 1px solid var(--border-color-lighter);
    background-color: var(--fill-color-light);
    
    span {
      font-weight: var(--font-weight-primary);
      color: var(--text-color-primary);
    }
  }
  
  .notification-list {
    max-height: 300px;
    overflow-y: auto;
    @include scrollbar();
    
    .notification-item {
      display: flex;
      align-items: flex-start;
      padding: var(--spacing-medium);
      border-bottom: 1px solid var(--border-color-extra-light);
      cursor: pointer;
      transition: var(--transition-base);
      
      &:hover {
        background-color: var(--fill-color-light);
      }
      
      &.is-unread {
        background-color: var(--primary-color-light-9, #ecf5ff);
      }
      
      .notification-content {
        flex: 1;
        
        .notification-title {
          font-size: var(--font-size-small);
          font-weight: var(--font-weight-primary);
          color: var(--text-color-primary);
          margin-bottom: var(--spacing-extra-small);
          @include text-ellipsis;
        }
        
        .notification-message {
          font-size: var(--font-size-extra-small);
          color: var(--text-color-secondary);
          margin-bottom: var(--spacing-extra-small);
          @include text-ellipsis(2);
        }
        
        .notification-time {
          font-size: var(--font-size-extra-small);
          color: var(--text-color-placeholder);
        }
      }
      
      .notification-status {
        width: 8px;
        height: 8px;
        background-color: var(--primary-color);
        border-radius: 50%;
        margin-left: var(--spacing-small);
        margin-top: var(--spacing-extra-small);
      }
    }
  }
  
  .notification-footer {
    padding: var(--spacing-medium);
    border-top: 1px solid var(--border-color-lighter);
    text-align: center;
    background-color: var(--fill-color-light);
  }
}
</style>