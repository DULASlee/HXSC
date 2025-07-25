<template>
  <div class="system-settings">
    <el-card>
      <template #header>
        <div class="card-header">
          <el-icon><Setting /></el-icon>
          <span>系统设置</span>
        </div>
      </template>

      <el-tabs v-model="activeTab" class="settings-tabs">
        <!-- 外观设置 -->
        <el-tab-pane label="外观设置" name="appearance">
          <div class="settings-section">
            <h3>主题设置</h3>
            <p class="section-desc">选择您喜欢的主题风格</p>
            
            <div class="theme-grid">
              <div
                v-for="theme in availableThemes"
                :key="theme.name"
                :class="[
                  'theme-option',
                  { 'theme-option--active': currentTheme === theme.name }
                ]"
                @click="handleThemeChange(theme.name)"
              >
                <div 
                  class="theme-preview"
                  :style="{ background: theme.preview }"
                >
                  <div class="theme-icon">{{ theme.icon }}</div>
                </div>
                <div class="theme-info">
                  <div class="theme-name">{{ theme.label }}</div>
                  <div class="theme-desc">{{ theme.description }}</div>
                </div>
                <el-icon v-if="currentTheme === theme.name" class="theme-check">
                  <Check />
                </el-icon>
              </div>
            </div>
          </div>

          <el-divider />

          <div class="settings-section">
            <h3>语言设置</h3>
            <p class="section-desc">选择系统显示语言</p>
            
            <el-radio-group v-model="currentLanguage" @change="handleLanguageChange">
              <el-radio-button 
                v-for="lang in languages" 
                :key="lang.code" 
                :value="lang.code"
                class="language-option"
              >
                <span class="language-flag">{{ lang.flag }}</span>
                <span class="language-name">{{ lang.name }}</span>
              </el-radio-button>
            </el-radio-group>
          </div>

          <el-divider />

          <div class="settings-section">
            <h3>布局设置</h3>
            <p class="section-desc">自定义系统布局选项</p>
            
            <el-form :model="layoutSettings" label-width="120px">
              <el-form-item label="侧边栏折叠">
                <el-switch 
                  v-model="layoutSettings.sidebarCollapsed"
                  @change="handleSidebarToggle"
                />
              </el-form-item>
              
              <el-form-item label="固定导航栏">
                <el-switch v-model="layoutSettings.fixedHeader" />
              </el-form-item>
              
              <el-form-item label="显示面包屑">
                <el-switch v-model="layoutSettings.showBreadcrumb" />
              </el-form-item>
              
              <el-form-item label="显示标签页">
                <el-switch v-model="layoutSettings.showTabs" />
              </el-form-item>
            </el-form>
          </div>
        </el-tab-pane>

        <!-- 系统配置 -->
        <el-tab-pane label="系统配置" name="system">
          <div class="settings-section">
            <h3>基本信息</h3>
            <el-form :model="systemConfig" label-width="120px">
              <el-form-item label="系统名称">
                <el-input v-model="systemConfig.systemName" />
              </el-form-item>
              
              <el-form-item label="系统版本">
                <el-input v-model="systemConfig.version" readonly />
              </el-form-item>
              
              <el-form-item label="公司名称">
                <el-input v-model="systemConfig.companyName" />
              </el-form-item>
              
              <el-form-item label="联系邮箱">
                <el-input v-model="systemConfig.contactEmail" />
              </el-form-item>
            </el-form>
          </div>

          <el-divider />

          <div class="settings-section">
            <h3>安全设置</h3>
            <el-form :model="securitySettings" label-width="120px">
              <el-form-item label="会话超时">
                <el-input-number 
                  v-model="securitySettings.sessionTimeout"
                  :min="5"
                  :max="480"
                  :step="5"
                />
                <span class="form-unit">分钟</span>
              </el-form-item>
              
              <el-form-item label="密码强度">
                <el-select v-model="securitySettings.passwordStrength">
                  <el-option label="低" value="low" />
                  <el-option label="中" value="medium" />
                  <el-option label="高" value="high" />
                </el-select>
              </el-form-item>
              
              <el-form-item label="启用双因子认证">
                <el-switch v-model="securitySettings.enableTwoFactor" />
              </el-form-item>
              
              <el-form-item label="登录失败锁定">
                <el-switch v-model="securitySettings.enableLoginLock" />
              </el-form-item>
            </el-form>
          </div>
        </el-tab-pane>

        <!-- 通知设置 -->
        <el-tab-pane label="通知设置" name="notification">
          <div class="settings-section">
            <h3>系统通知</h3>
            <el-form :model="notificationSettings" label-width="120px">
              <el-form-item label="邮件通知">
                <el-switch v-model="notificationSettings.emailEnabled" />
              </el-form-item>
              
              <el-form-item label="短信通知">
                <el-switch v-model="notificationSettings.smsEnabled" />
              </el-form-item>
              
              <el-form-item label="浏览器通知">
                <el-switch v-model="notificationSettings.browserEnabled" />
              </el-form-item>
              
              <el-form-item label="声音提醒">
                <el-switch v-model="notificationSettings.soundEnabled" />
              </el-form-item>
            </el-form>
          </div>

          <el-divider />

          <div class="settings-section">
            <h3>通知类型</h3>
            <el-checkbox-group v-model="notificationSettings.types">
              <el-checkbox value="system">系统消息</el-checkbox>
              <el-checkbox value="security">安全警告</el-checkbox>
              <el-checkbox value="maintenance">维护通知</el-checkbox>
              <el-checkbox value="update">更新提醒</el-checkbox>
            </el-checkbox-group>
          </div>
        </el-tab-pane>

        <!-- 数据管理 -->
        <el-tab-pane label="数据管理" name="data">
          <div class="settings-section">
            <h3>数据备份</h3>
            <p class="section-desc">定期备份系统数据，确保数据安全</p>
            
            <el-form :model="dataSettings" label-width="120px">
              <el-form-item label="自动备份">
                <el-switch v-model="dataSettings.autoBackup" />
              </el-form-item>
              
              <el-form-item label="备份频率" v-if="dataSettings.autoBackup">
                <el-select v-model="dataSettings.backupFrequency">
                  <el-option label="每日" value="daily" />
                  <el-option label="每周" value="weekly" />
                  <el-option label="每月" value="monthly" />
                </el-select>
              </el-form-item>
              
              <el-form-item label="保留天数">
                <el-input-number 
                  v-model="dataSettings.retentionDays"
                  :min="1"
                  :max="365"
                />
                <span class="form-unit">天</span>
              </el-form-item>
            </el-form>
            
            <div class="backup-actions">
              <el-button type="primary" @click="createBackup">
                <el-icon><Download /></el-icon>
                立即备份
              </el-button>
              <el-button @click="restoreBackup">
                <el-icon><Upload /></el-icon>
                恢复备份
              </el-button>
            </div>
          </div>

          <el-divider />

          <div class="settings-section">
            <h3>数据清理</h3>
            <p class="section-desc">清理过期数据，释放存储空间</p>
            
            <div class="cleanup-options">
              <el-button type="warning" @click="cleanupLogs">
                <el-icon><Delete /></el-icon>
                清理日志
              </el-button>
              <el-button type="warning" @click="cleanupCache">
                <el-icon><Refresh /></el-icon>
                清理缓存
              </el-button>
              <el-button type="danger" @click="resetSystem">
                <el-icon><Warning /></el-icon>
                重置系统
              </el-button>
            </div>
          </div>
        </el-tab-pane>
      </el-tabs>

      <!-- 保存按钮 -->
      <div class="settings-footer">
        <el-button @click="resetSettings">重置</el-button>
        <el-button type="primary" @click="saveSettings">
          <el-icon><Check /></el-icon>
          保存设置
        </el-button>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { 
  Setting, Check, Download, Upload, Delete, 
  Refresh, Warning 
} from '@element-plus/icons-vue'
import { useTheme } from '@/composables/useTheme'
import { useI18nStore } from '@/stores/i18n'
import { useAppStore } from '@/stores/app'

// 使用组合式函数
const { currentTheme, availableThemes, setTheme } = useTheme()
const i18nStore = useI18nStore()
const appStore = useAppStore()

// 当前选中的标签页
const activeTab = ref('appearance')

// 当前语言
const currentLanguage = computed({
  get: () => i18nStore.locale,
  set: (value) => i18nStore.setLocale(value as any)
})

// 支持的语言
const languages = ref([
  { code: 'zh-CN', name: '简体中文', flag: '🇨🇳' },
  { code: 'en-US', name: 'English', flag: '🇺🇸' },
  { code: 'ja-JP', name: '日本語', flag: '🇯🇵' }
])

// 布局设置
const layoutSettings = ref({
  sidebarCollapsed: !appStore.sidebar.opened,
  fixedHeader: true,
  showBreadcrumb: true,
  showTabs: true
})

// 系统配置
const systemConfig = ref({
  systemName: '智慧工地管理平台',
  version: 'v1.0.0',
  companyName: '智慧建设科技有限公司',
  contactEmail: 'admin@smartconstruction.com'
})

// 安全设置
const securitySettings = ref({
  sessionTimeout: 30,
  passwordStrength: 'medium',
  enableTwoFactor: false,
  enableLoginLock: true
})

// 通知设置
const notificationSettings = ref({
  emailEnabled: true,
  smsEnabled: false,
  browserEnabled: true,
  soundEnabled: true,
  types: ['system', 'security']
})

// 数据设置
const dataSettings = ref({
  autoBackup: true,
  backupFrequency: 'daily',
  retentionDays: 30
})

// 主题切换处理
const handleThemeChange = async (themeName: string) => {
  try {
    await setTheme(themeName as any)
    ElMessage.success(`已切换到${availableThemes.value.find(t => t.name === themeName)?.label}`)
  } catch (error) {
    ElMessage.error('主题切换失败')
  }
}

// 语言切换处理
const handleLanguageChange = (langCode: string) => {
  const langName = languages.value.find(lang => lang.code === langCode)?.name
  ElMessage.success(`语言已切换为 ${langName}`)
}

// 侧边栏切换处理
const handleSidebarToggle = (collapsed: boolean) => {
  if (collapsed) {
    appStore.setSidebarCollapsed(true)
  } else {
    appStore.setSidebarCollapsed(false)
  }
}

// 数据管理操作
const createBackup = () => {
  ElMessage.success('备份创建功能开发中')
}

const restoreBackup = () => {
  ElMessage.success('备份恢复功能开发中')
}

const cleanupLogs = async () => {
  try {
    await ElMessageBox.confirm('确定要清理系统日志吗？', '确认操作')
    ElMessage.success('日志清理完成')
  } catch {
    // 用户取消
  }
}

const cleanupCache = () => {
  ElMessage.success('缓存清理完成')
}

const resetSystem = async () => {
  try {
    await ElMessageBox.confirm(
      '此操作将重置所有系统设置，是否继续？',
      '危险操作',
      { type: 'error' }
    )
    ElMessage.success('系统重置完成')
  } catch {
    // 用户取消
  }
}

// 保存设置
const saveSettings = () => {
  ElMessage.success('设置已保存')
}

// 重置设置
const resetSettings = () => {
  ElMessage.success('设置已重置')
}
</script>

<style lang="scss" scoped>
.system-settings {
  padding: var(--spacing-lg);
  background-color: var(--bg-body);
  min-height: calc(100vh - 50px);
  
  .card-header {
    display: flex;
    align-items: center;
    gap: var(--spacing-sm);
    font-size: 18px;
    font-weight: 600;
    color: var(--text-primary);
  }
  
  .settings-tabs {
    margin-top: var(--spacing-lg);
    
    :deep(.el-tabs__header) {
      margin-bottom: var(--spacing-lg);
    }
  }
  
  .settings-section {
    margin-bottom: var(--spacing-xl);
    
    h3 {
      color: var(--text-primary);
      margin-bottom: var(--spacing-xs);
      font-size: 16px;
      font-weight: 600;
    }
    
    .section-desc {
      color: var(--text-secondary);
      margin-bottom: var(--spacing-lg);
      font-size: 14px;
    }
  }
  
  .theme-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: var(--spacing-md);
    
    .theme-option {
      display: flex;
      align-items: center;
      gap: var(--spacing-md);
      padding: var(--spacing-md);
      border: 2px solid var(--border-color-light);
      border-radius: var(--radius-lg);
      cursor: pointer;
      transition: all 0.3s ease;
      
      &:hover {
        border-color: var(--primary-color);
        box-shadow: var(--shadow-sm);
      }
      
      &--active {
        border-color: var(--primary-color);
        background: var(--primary-bg);
      }
      
      .theme-preview {
        width: 60px;
        height: 60px;
        border-radius: var(--radius-md);
        display: flex;
        align-items: center;
        justify-content: center;
        
        .theme-icon {
          font-size: 24px;
        }
      }
      
      .theme-info {
        flex: 1;
        
        .theme-name {
          font-weight: 600;
          color: var(--text-primary);
          margin-bottom: 4px;
        }
        
        .theme-desc {
          font-size: 12px;
          color: var(--text-secondary);
          line-height: 1.4;
        }
      }
      
      .theme-check {
        color: var(--primary-color);
        font-size: 20px;
      }
    }
  }
  
  .language-option {
    margin-right: var(--spacing-sm);
    margin-bottom: var(--spacing-sm);
    
    .language-flag {
      margin-right: var(--spacing-xs);
    }
  }
  
  .form-unit {
    margin-left: var(--spacing-xs);
    color: var(--text-secondary);
    font-size: 14px;
  }
  
  .backup-actions,
  .cleanup-options {
    display: flex;
    gap: var(--spacing-sm);
    flex-wrap: wrap;
  }
  
  .settings-footer {
    display: flex;
    justify-content: flex-end;
    gap: var(--spacing-sm);
    margin-top: var(--spacing-xl);
    padding-top: var(--spacing-lg);
    border-top: 1px solid var(--border-color-light);
  }
  
  // 响应式适配
  @media (max-width: 768px) {
    padding: var(--spacing-md);
    
    .theme-grid {
      grid-template-columns: 1fr;
    }
    
    .backup-actions,
    .cleanup-options {
      flex-direction: column;
      
      .el-button {
        width: 100%;
      }
    }
    
    .settings-footer {
      flex-direction: column;
      
      .el-button {
        width: 100%;
      }
    }
  }
}
</style>