<template>
  <div class="dashboard-container">
    <!-- 欢迎区域 -->
    <div class="welcome-section">
      <h1 class="welcome-title">{{ t('dashboard.welcome') }}</h1>
      <p class="system-description">{{ t('dashboard.description') }}</p>
    </div>

    <!-- 统计卡片区域 -->
    <el-row :gutter="20" class="stats-row">
      <el-col 
        v-for="(stat, index) in stats" 
        :key="index" 
        :xs="24" :sm="12" :md="8" :lg="6">
        <div class="stat-card">
          <div class="stat-value">{{ stat.value }}</div>
          <div class="stat-title">{{ t(stat.title) }}</div>
        </div>
      </el-col>
    </el-row>

    <!-- 内容区域 -->
    <el-row :gutter="20" class="content-row">
      <!-- 系统信息面板 -->
      <el-col :xs="24" :sm="16">
        <div class="info-card">
          <h2 class="card-header">{{ t('dashboard.systemInfo') }}</h2>
          <div class="info-grid">
            <div 
              v-for="(info, index) in systemInfo" 
              :key="index" 
              class="info-item">
              <span class="info-label">{{ t(info.label) }}:</span>
              <span class="info-value">{{ info.value }}</span>
            </div>
          </div>
        </div>
      </el-col>
      
      <!-- 快速操作区域 -->
      <el-col :xs="24" :sm="8">
        <div class="actions-card">
          <h2 class="card-header">{{ t('dashboard.quickActions') }}</h2>
          <div class="actions-grid">
            <el-button
              v-for="(action, index) in quickActions"
              :key="index"
              type="primary"
              class="action-button"
              @click="handleAction(action)">
              {{ t(action.title) }}
            </el-button>
          </div>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useI18n } from 'vue-i18n';
import { useRouter } from 'vue-router';
import { useUserStore } from '@/stores/user';
import { storeToRefs } from 'pinia';
import { useTheme } from '@/composables/useTheme';

const { t } = useI18n();
const router = useRouter();
const userStore = useUserStore();
const { currentTheme } = useTheme();

// 使用 storeToRefs 保持响应性
const { userInfo, currentTenant, roles } = storeToRefs(userStore);

// 登录时间
const loginTime = computed(() => {
  if (userStore.loginTime) {
    return new Date(userStore.loginTime).toLocaleString();
  }
  return 'N/A';
});

// 统计卡片数据
const stats = ref([
  { value: userStore.userInfo ? 3 : 0, title: 'dashboard.users' },
  { value: currentTenant.value ? 1 : 0, title: 'dashboard.tenants' },
  { value: roles.value.length || 0, title: 'dashboard.roles' },
  { value: 0, title: 'dashboard.menus' }
]);

// 系统信息数据
const systemInfo = ref([
  { label: 'dashboard.currentUser', value: userInfo.value?.displayName || userInfo.value?.username || '系统管理员' },
  { label: 'dashboard.currentTenant', value: currentTenant.value?.name || '系统管理租户' },
  { label: 'dashboard.userRole', value: roles.value.join(', ') || 'ADMIN' },
  { label: 'dashboard.loginTime', value: loginTime.value },
  { label: 'dashboard.systemVersion', value: 'v1.0.0' },
  { label: 'dashboard.runtimeEnv', value: 'Production' },
  { label: 'dashboard.serverTime', value: new Date().toLocaleString() },
  { label: 'dashboard.runDuration', value: '3小时15分钟' }
]);

// 快速操作按钮
const quickActions = ref([
  { title: 'dashboard.userMgt', action: 'userManagement' },
  { title: 'dashboard.roleMgt', action: 'roleManagement' },
  { title: 'dashboard.tenantMgt', action: 'tenantManagement' },
  { title: 'dashboard.menuMgt', action: 'menuManagement' }
]);

// 处理操作按钮点击
const handleAction = (action) => {
  console.log(`执行操作: ${action.title}`);
  
  switch(action.action) {
    case 'userManagement':
      router.push('/system/user');
      break;
    case 'roleManagement':
      router.push('/system/role');
      break;
    case 'tenantManagement':
      router.push('/tenant/list');
      break;
    case 'menuManagement':
      router.push('/system/menu');
      break;
    default:
      console.log('未知操作');
  }
};
</script>

<style lang="scss" scoped>
.dashboard-container {
  padding: var(--spacing-lg);
  max-width: 1400px;
  margin: 0 auto;
  background-color: var(--bg-body);
  min-height: calc(100vh - 50px);
  transition: all 0.3s ease;
  
  // 响应式适配
  @media (max-width: 768px) {
    padding: var(--spacing-md);
  }
}

.welcome-section {
  text-align: center;
  margin-bottom: var(--spacing-xl);
  padding: var(--spacing-lg);
  background: var(--bg-container);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-sm);
  transition: all 0.3s ease;
  
  .welcome-title {
    font-size: 2.5rem;
    margin-bottom: var(--spacing-sm);
    color: var(--primary-color);
    transition: color 0.3s ease;
    
    @media (max-width: 768px) {
      font-size: 2rem;
    }
    
    @media (max-width: 480px) {
      font-size: 1.8rem;
    }
  }
  
  .system-description {
    font-size: 1.2rem;
    color: var(--text-secondary);
    transition: color 0.3s ease;
    
    @media (max-width: 768px) {
      font-size: 1.1rem;
    }
    
    @media (max-width: 480px) {
      font-size: 1rem;
    }
  }
  
  // 响应式适配
  @media (max-width: 768px) {
    margin-bottom: var(--spacing-lg);
    padding: var(--spacing-md);
  }
}

.stats-row {
  margin-bottom: 30px;
  
  .stat-card {
    background: var(--card-bg);
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
    text-align: center;
    transition: transform 0.3s ease;
    
    &:hover {
      transform: translateY(-5px);
      box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
    }
    
    .stat-value {
      font-size: 3rem;
      font-weight: bold;
      color: var(--primary-color);
      margin-bottom: 10px;
    }
    
    .stat-title {
      font-size: 1.1rem;
      color: var(--text-regular);
    }
  }
}

.content-row {
  .info-card,
  .actions-card {
    background: var(--card-bg);
    border-radius: 8px;
    padding: 25px;
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
    height: 100%;
  }
  
  .card-header {
    margin-bottom: 20px;
    font-size: 1.5rem;
    color: var(--primary-color);
    border-bottom: 2px solid var(--border-light);
    padding-bottom: 10px;
  }
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 15px;
  
  .info-item {
    display: flex;
    flex-direction: column;
    padding: 10px;
    border-bottom: 1px solid var(--border-lighter);
    
    .info-label {
      font-weight: bold;
      color: var(--text-secondary);
      font-size: 0.9rem;
      margin-bottom: 5px;
    }
    
    .info-value {
      color: var(--text-regular);
      font-size: 1.1rem;
    }
  }
}

.actions-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 15px;
  
  .action-button {
    width: 100%;
    padding: 12px 5px;
    margin: 5px 0;
  }
}

/* 响应式调整 */
@media (max-width: 768px) {
  .welcome-title {
    font-size: 2rem !important;
  }
  
  .stat-card {
    margin-bottom: 15px;
    
    .stat-value {
      font-size: 2.5rem !important;
    }
  }
  
  .actions-grid {
    grid-template-columns: 1fr;
  }
}
</style>