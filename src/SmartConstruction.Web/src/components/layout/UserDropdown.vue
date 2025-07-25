<template>
  <el-dropdown 
    class="user-dropdown"
    trigger="click"
    @command="handleCommand"
  >
    <div class="user-info">
      <el-avatar 
        :size="32" 
        :src="userInfo?.avatar" 
        class="user-avatar"
      >
        <el-icon><User /></el-icon>
      </el-avatar>
      <div class="user-details" v-if="!isMobile">
        <div class="user-name">{{ displayName }}</div>
        <div class="user-role">{{ currentRole }}</div>
      </div>
      <el-icon class="dropdown-icon" v-if="!isMobile">
        <ArrowDown />
      </el-icon>
    </div>
    
    <template #dropdown>
      <el-dropdown-menu>
        <!-- 用户信息 -->
        <div class="user-info-header">
          <el-avatar :size="40" :src="userInfo?.avatar">
            <el-icon><User /></el-icon>
          </el-avatar>
          <div class="user-info-details">
            <div class="user-name">{{ displayName }}</div>
            <div class="user-email">{{ userInfo?.email || '未设置邮箱' }}</div>
            <div class="user-tenant">{{ tenantName }}</div>
          </div>
        </div>
        
        <el-dropdown-item divided command="profile">
          <el-icon><User /></el-icon>
          <span>个人中心</span>
        </el-dropdown-item>
        
        <el-dropdown-item command="settings">
          <el-icon><Setting /></el-icon>
          <span>个人设置</span>
        </el-dropdown-item>
        
        <el-dropdown-item command="password">
          <el-icon><Lock /></el-icon>
          <span>修改密码</span>
        </el-dropdown-item>
        
        <el-dropdown-item divided command="about">
          <el-icon><InfoFilled /></el-icon>
          <span>关于系统</span>
        </el-dropdown-item>
        
        <el-dropdown-item command="help">
          <el-icon><QuestionFilled /></el-icon>
          <span>帮助文档</span>
        </el-dropdown-item>
        
        <el-dropdown-item divided command="logout" class="logout-item">
          <el-icon><SwitchButton /></el-icon>
          <span>退出登录</span>
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
import { computed, inject } from 'vue'
import { useRouter } from 'vue-router'
import { 
  User, 
  ArrowDown, 
  Setting, 
  Lock, 
  InfoFilled, 
  QuestionFilled, 
  SwitchButton 
} from '@element-plus/icons-vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { useUserStore } from '@/stores/user'
import { useAppStore } from '@/stores/app'

const router = useRouter()
const userStore = useUserStore()
const appStore = useAppStore()

// 获取移动端状态
const isMobile = inject('isMobile', false)

// 计算属性
const userInfo = computed(() => userStore.userInfo)
const displayName = computed(() => userInfo.value?.displayName || userInfo.value?.username || '用户')
const currentRole = computed(() => {
  const roles = userStore.roles
  return roles.length > 0 ? roles[0] : '普通用户'
})
const tenantName = computed(() => userStore.currentTenant?.name || '默认租户')

// 处理下拉菜单命令
const handleCommand = async (command: string) => {
  switch (command) {
    case 'profile':
      router.push('/profile')
      break
      
    case 'settings':
      router.push('/profile/settings')
      break
      
    case 'password':
      router.push('/profile/password')
      break
      
    case 'about':
      showAboutDialog()
      break
      
    case 'help':
      window.open('/docs', '_blank')
      break
      
    case 'logout':
      handleLogout()
      break
      
    default:
      console.log('Unknown command:', command)
  }
}

// 显示关于对话框
const showAboutDialog = () => {
  ElMessageBox.alert(
    `
    <div style="text-align: center;">
      <h3>智慧建设巴伯框架</h3>
      <p>版本：v1.0.0</p>
      <p>多租户企业级管理系统</p>
      <p>© 2024 All Rights Reserved</p>
    </div>
    `,
    '关于系统',
    {
      dangerouslyUseHTMLString: true,
      confirmButtonText: '确定'
    }
  )
}

// 处理退出登录
const handleLogout = () => {
  ElMessageBox.confirm(
    '确定要退出登录吗？',
    '退出确认',
    {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    }
  ).then(async () => {
    try {
      await userStore.logout()
      ElMessage.success('已退出登录')
      router.push('/login')
    } catch (error) {
      console.error('Logout error:', error)
      ElMessage.error('退出登录失败')
    }
  }).catch(() => {
    // 用户取消退出
  })
}
</script>

<style lang="scss" scoped>
@import '@/styles/mixins.scss';

.user-dropdown {
  .user-info {
    display: flex;
    align-items: center;
    padding: var(--spacing-extra-small) var(--spacing-small);
    border-radius: var(--border-radius-base);
    cursor: pointer;
    transition: var(--transition-base);
    
    &:hover {
      background-color: var(--fill-color-light);
    }
    
    .user-avatar {
      margin-right: var(--spacing-small);
      border: 2px solid var(--border-color-lighter);
      transition: var(--transition-base);
      
      &:hover {
        border-color: var(--primary-color);
      }
    }
    
    .user-details {
      flex: 1;
      min-width: 0;
      margin-right: var(--spacing-small);
      
      .user-name {
        font-size: var(--font-size-small);
        font-weight: var(--font-weight-primary);
        color: var(--text-color-primary);
        line-height: 1.2;
        @include text-ellipsis;
      }
      
      .user-role {
        font-size: var(--font-size-extra-small);
        color: var(--text-color-secondary);
        line-height: 1.2;
        margin-top: 2px;
        @include text-ellipsis;
      }
    }
    
    .dropdown-icon {
      font-size: var(--font-size-small);
      color: var(--text-color-placeholder);
      transition: var(--transition-base);
    }
    
    &:hover .dropdown-icon {
      color: var(--text-color-secondary);
    }
  }
}

:deep(.el-dropdown-menu) {
  min-width: 200px;
  
  .user-info-header {
    display: flex;
    align-items: center;
    padding: var(--spacing-medium);
    border-bottom: 1px solid var(--border-color-lighter);
    background-color: var(--fill-color-light);
    
    .user-info-details {
      margin-left: var(--spacing-medium);
      flex: 1;
      
      .user-name {
        font-size: var(--font-size-base);
        font-weight: var(--font-weight-primary);
        color: var(--text-color-primary);
        margin-bottom: var(--spacing-extra-small);
      }
      
      .user-email {
        font-size: var(--font-size-small);
        color: var(--text-color-secondary);
        margin-bottom: var(--spacing-extra-small);
        @include text-ellipsis;
      }
      
      .user-tenant {
        font-size: var(--font-size-extra-small);
        color: var(--text-color-placeholder);
        @include text-ellipsis;
      }
    }
  }
  
  .el-dropdown-menu__item {
    display: flex;
    align-items: center;
    padding: var(--spacing-small) var(--spacing-medium);
    
    .el-icon {
      margin-right: var(--spacing-small);
      font-size: var(--font-size-base);
      color: var(--text-color-secondary);
    }
    
    span {
      font-size: var(--font-size-small);
    }
    
    &:hover {
      background-color: var(--fill-color-light);
      
      .el-icon {
        color: var(--primary-color);
      }
    }
    
    &.logout-item {
      color: var(--danger-color);
      
      .el-icon {
        color: var(--danger-color);
      }
      
      &:hover {
        background-color: var(--danger-color-light-9, #fef0f0);
      }
    }
  }
}

// 移动端适配
@include respond-to(xs) {
  .user-dropdown {
    .user-info {
      padding: var(--spacing-extra-small);
      
      .user-avatar {
        margin-right: 0;
      }
    }
  }
}

// 用户状态指示器
.user-dropdown {
  position: relative;
  
  &::before {
    content: '';
    position: absolute;
    top: 0;
    right: 0;
    width: 8px;
    height: 8px;
    background-color: var(--success-color);
    border-radius: 50%;
    border: 2px solid var(--bg-color-overlay);
    z-index: 1;
  }
}

// 暗色主题适配
[data-theme="dark"] {
  .user-dropdown {
    .user-info {
      &:hover {
        background-color: var(--fill-color-dark);
      }
    }
  }
  
  :deep(.el-dropdown-menu) {
    .user-info-header {
      background-color: var(--fill-color-dark);
    }
    
    .el-dropdown-menu__item {
      &:hover {
        background-color: var(--fill-color-dark);
      }
    }
  }
}
</style>