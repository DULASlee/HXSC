<template>
  <div class="login-container">
    <div class="login-bg">
      <div class="bg-item" v-for="i in 5" :key="i"></div>
    </div>
    
    <el-card class="login-card">
      <div class="login-header">
        <div class="logo-container">
          <el-icon :size="60" color="#409EFF" class="logo">
            <HomeFilled />
          </el-icon>
        </div>
        <h1 class="title">{{ tenantName }}</h1>
        <p class="subtitle">{{ $t('login.subtitle') || '请输入您的登录信息' }}</p>
      </div>
      
      <el-form
        ref="loginFormRef"
        :model="loginForm"
        :rules="loginRules"
        class="login-form"
        label-position="top"
      >
        <el-form-item prop="tenantCode" :label="$t('login.tenant') || '租户'">
          <el-select
            v-model="loginForm.tenantCode"
            :placeholder="$t('login.selectTenant') || '请选择租户'"
            :loading="tenantsLoading"
            @change="handleTenantChange"
            size="large"
            style="width: 100%"
          >
            <el-option
              v-for="tenant in tenants"
              :key="tenant.code"
              :label="tenant.name"
              :value="tenant.code"
            >
              <div class="tenant-option">
                <div class="tenant-logo-container">
                  <el-icon :size="20" color="#409EFF">
                    <HomeFilled v-if="tenant.code === 'SYSTEM'" />
                    <OfficeBuilding v-else />
                  </el-icon>
                </div>
                <span>{{ tenant.name }}</span>
                <el-tag v-if="tenant.isolationMode" size="small" class="tenant-tag">
                  {{ getTenantIsolationLabel(tenant.isolationMode) }}
                </el-tag>
              </div>
            </el-option>
          </el-select>
        </el-form-item>
        
        <el-form-item prop="username" :label="$t('login.username') || '用户名'">
          <el-input
            v-model="loginForm.username"
            :placeholder="$t('login.enterUsername') || '请输入用户名'"
            prefix-icon="User"
            size="large"
            clearable
          />
        </el-form-item>
        
        <el-form-item prop="password" :label="$t('login.password') || '密码'">
          <el-input
            v-model="loginForm.password"
            type="password"
            :placeholder="$t('login.enterPassword') || '请输入密码'"
            prefix-icon="Lock"
            size="large"
            show-password
            @keyup.enter="handleLogin"
          />
        </el-form-item>
        
        <el-form-item>
          <div class="login-options">
            <el-checkbox v-model="loginForm.rememberMe">
              {{ $t('login.rememberMe') || '记住我' }}
            </el-checkbox>
            <el-link type="primary" underline="never">
              {{ $t('login.forgotPassword') || '忘记密码？' }}
            </el-link>
          </div>
        </el-form-item>
        
        <el-form-item>
          <el-button
            type="primary"
            size="large"
            :loading="loading"
            @click="handleLogin"
            style="width: 100%"
          >
            {{ $t('login.loginButton') || '登录' }}
          </el-button>
        </el-form-item>
      </el-form>
      
      <div class="login-footer">
        <div class="demo-accounts">
          <p>{{ $t('login.demoAccount') || '演示账号' }}：</p>
          <el-space>
            <el-link @click="fillDemo('admin')" underline="never">管理员</el-link>
            <el-divider direction="vertical" />
            <el-link @click="fillDemo('user')" underline="never">普通用户</el-link>
            <el-divider direction="vertical" />
            <el-link @click="fillDemo('auditor')" underline="never">审计员</el-link>
          </el-space>
        </div>
        
        <div class="language-switch">
          <el-dropdown @command="handleLanguageChange">
            <el-button text>
              <el-icon><Setting /></el-icon>
              {{ currentLanguageLabel }}
            </el-button>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item command="zh-CN">简体中文</el-dropdown-item>
                <el-dropdown-item command="en">English</el-dropdown-item>
                <el-dropdown-item command="ja">日本語</el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </div>
        
        <div class="theme-switch">
          <el-switch
            v-model="isDark"
            :active-icon="Moon"
            :inactive-icon="Sunny"
            @change="handleThemeChange"
          />
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, nextTick } from 'vue'
import { ElForm, ElMessage } from 'element-plus'
import { useUserStore } from '../../stores/user'
import { useAppStore } from '../../stores/app'
import { getTenantList } from '../../api/modules/tenant'
import { useApi } from '../../composables/useApi'
import { Moon, Sunny, HomeFilled, OfficeBuilding, Setting } from '@element-plus/icons-vue'
import type { LoginRequest, Tenant } from '../../types/global'
import type { FormInstance } from 'element-plus'
import { useRouter } from 'vue-router'

const userStore = useUserStore()
const appStore = useAppStore()
const router = useRouter()

const loginFormRef = ref<FormInstance>()
const loading = ref(false)
const isDark = ref(appStore.theme === 'dark')

// 使用 useApi 加载租户列表
const { data: tenants, loading: tenantsLoading } = useApi(getTenantList, {
  immediate: true,
  transform: (response: any) => {
    // 确保 SYSTEM 租户始终存在
    const tenantList = response?.items || []
    if (!tenantList.some((t: Tenant) => t.code === 'SYSTEM')) {
      tenantList.unshift({
        id: '1',
        code: 'SYSTEM',
        name: '系统管理租户',
        status: 1,
        isolationMode: 'Shared',
        createdAt: new Date().toISOString()
      })
    }
    return tenantList
  },
  onError: () => {
    // API 失败时，提供一个默认的 SYSTEM 租户用于登录
    return [{
      id: '1',
      code: 'SYSTEM',
      name: '系统管理租户 (备用)',
      status: 1,
      isolationMode: 'Shared',
      createdAt: new Date().toISOString()
    }]
  }
})

// 登录表单
const loginForm = reactive<LoginRequest>({
  tenantCode: 'SYSTEM',
  username: '',
  password: '',
  rememberMe: false
})

// 表单验证规则
const loginRules = computed(() => ({
  tenantCode: [{ required: true, message: '请选择租户', trigger: 'change' }],
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [
    { required: true, message: '请输入密码', trigger: 'blur' }
  ]
}))

// 当前租户信息
const currentTenant = computed(() => tenants.value?.find((t: Tenant) => t.code === loginForm.tenantCode))
const tenantName = computed(() => currentTenant.value?.name || '多租户权限管理系统')
const tenantLogo = computed(() => currentTenant.value?.logo || '/logo.png')

// 当前语言标签
const currentLanguageLabel = computed(() => {
  const labels: Record<string, string> = {
    'zh-CN': '简体中文',
    'en': 'English',
    'ja': '日本語'
  }
  return labels[appStore.language] || '简体中文'
})

// 获取租户隔离模式标签
function getTenantIsolationLabel(mode: string) {
  const labels: Record<string, string> = {
    'Shared': '共享数据库',
    'Schema': '独立Schema',
    'Database': '独立数据库'
  }
  return labels[mode] || mode
}

// 切换租户
function handleTenantChange(tenantCode: string) {
  const tenant = tenants.value?.find((t: Tenant) => t.code === tenantCode)
  if (tenant?.theme) {
    document.documentElement.setAttribute('data-tenant-theme', tenant.theme)
  }
}

// 登录
async function handleLogin() {
  const valid = await loginFormRef.value?.validate()
  if (!valid) return

  loading.value = true
  try {
    const loginResult = await userStore.login({
      ...loginForm,
      deviceId: getDeviceId(),
      deviceType: 'Web'
    });

    if (loginResult && loginResult.success) {
      // 登录成功后，不再负责加载菜单，只负责跳转
      // 菜单和用户信息的加载，完全交给路由守卫的初始化流程
      ElMessage.success('登录成功');
      router.push('/');
    } else {
      ElMessage.error(loginResult.message || '登录失败，请检查您的凭证或联系管理员');
    }
  } catch (error: any) {
    console.error('登录流程发生意外错误:', error)
    ElMessage.error(error.message || '发生未知错误，请稍后再试')
  } finally {
    loading.value = false
  }
}

// 填充演示账号
function fillDemo(type: 'admin' | 'user' | 'auditor') {
  const accounts: Record<string, { username: string; password: string; tenantCode?: string }> = {
    admin: { username: 'admin', password: 'Admin@123', tenantCode: 'SYSTEM' },
    user: { username: 'zhangsan', password: 'Admin@123', tenantCode: 'SYSTEM' },
    auditor: { username: 'auditor001', password: 'Admin@123', tenantCode: 'SYSTEM' }
  }

  const account = accounts[type]
  if (account) {
    loginForm.username = account.username
    loginForm.password = account.password
    if (account.tenantCode) {
      loginForm.tenantCode = account.tenantCode
      handleTenantChange(account.tenantCode)
    }
    ElMessage.success('已填充演示账号，请点击登录')
  }
}

// 切换语言
function handleLanguageChange(language: string) {
  appStore.setLanguage(language as any)
  location.reload()
}

// 切换主题
function handleThemeChange(dark: any) {
  appStore.setTheme(dark ? 'dark' : 'light')
}

// 获取设备ID
function getDeviceId(): string {
  let deviceId = localStorage.getItem('deviceId')
  if (!deviceId) {
    deviceId = `web-${Math.random().toString(36).substr(2, 9)}`
    localStorage.setItem('deviceId', deviceId)
  }
  return deviceId
}
</script>

<style lang="scss" scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  overflow: hidden;

  .login-bg {
    position: absolute;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;

    .bg-item {
      position: absolute;
      width: 400px;
      height: 400px;
      border-radius: 50%;
      filter: blur(100px);
      opacity: 0.3;
      animation: float 20s infinite ease-in-out;

      &:nth-child(1) {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        top: -200px;
        left: -200px;
        animation-delay: 0s;
      }

      &:nth-child(2) {
        background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
        bottom: -200px;
        right: -200px;
        animation-delay: 5s;
      }

      &:nth-child(3) {
        background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        animation-delay: 10s;
      }

      &:nth-child(4) {
        background: linear-gradient(135deg, #fa709a 0%, #fee140 100%);
        top: -100px;
        right: -100px;
        animation-delay: 15s;
      }

      &:nth-child(5) {
        background: linear-gradient(135deg, #30cfd0 0%, #330867 100%);
        bottom: -100px;
        left: -100px;
        animation-delay: 20s;
      }
    }
  }

  @keyframes float {
    0%, 100% {
      transform: translateY(0) scale(1);
    }
    50% {
      transform: translateY(-50px) scale(1.1);
    }
  }

  .login-card {
    width: 500px;
    position: relative;
    z-index: 10;
    backdrop-filter: blur(10px);
    background: rgba(255, 255, 255, 0.9);

    :deep(.el-card__body) {
      padding: 40px;
    }

    .login-header {
      text-align: center;
      margin-bottom: 40px;

      .logo-container {
        display: flex;
        justify-content: center;
        margin-bottom: 20px;
      }
      
      .logo {
        padding: 10px;
        border-radius: 20px;
        box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
        background-color: #f5f7fa;
      }

      .title {
        font-size: 28px;
        font-weight: 600;
        color: var(--el-text-color-primary);
        margin: 0 0 10px;
      }

      .subtitle {
        font-size: 16px;
        color: var(--el-text-color-regular);
        margin: 0;
      }
    }

    .login-form {
      .login-options {
        display: flex;
        justify-content: space-between;
        align-items: center;
        width: 100%;
      }
    }

    .login-footer {
      margin-top: 30px;
      padding-top: 20px;
      border-top: 1px solid var(--el-border-color-lighter);

      .demo-accounts {
        text-align: center;
        margin-bottom: 20px;

        p {
          margin: 0 0 10px;
          color: var(--el-text-color-regular);
          font-size: 14px;
        }
      }

      .language-switch,
      .theme-switch {
        display: flex;
        justify-content: center;
        margin-top: 10px;
      }
    }
  }

  .tenant-option {
    display: flex;
    align-items: center;

    .tenant-logo-container {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 24px;
      height: 24px;
      margin-right: 8px;
      border-radius: 4px;
      background-color: #f5f7fa;
    }

    .tenant-tag {
      margin-left: auto;
    }
  }
}

// 暗黑模式适配
html.dark {
  .login-card {
    background: rgba(30, 30, 30, 0.9);
  }
}

// 响应式设计
@media (max-width: 768px) {
  .login-container {
    padding: 20px;
    
    .login-card {
      width: 100%;
      max-width: 400px;
      
      :deep(.el-card__body) {
        padding: 30px 20px;
      }
    }
  }
}
</style>