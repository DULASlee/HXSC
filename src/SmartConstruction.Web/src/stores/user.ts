import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import router, { resetRouter } from '@/router';
import { ElMessage } from 'element-plus';
import { PermissionService } from '@/services/permission';
import { useMenuStore } from './menu';
import {
  login as loginApi,
  logout as logoutApi,
  refreshTokenApi,
} from '@/api/modules/auth';
import {
  getCurrentUser,
  changePassword as changePasswordApi,
  resetPassword as resetPasswordApi
} from '@/api/modules/user';
import type { LoginRequest, User, Tenant } from '@/types/global';

// =============================================
// 手动状态持久化：应用启动时从 localStorage 恢复状态
// =============================================
const getInitialState = (): UserState => {
  // 尝试从 localStorage 恢复状态
  try {
    const storedState = localStorage.getItem('user');
    if (storedState) {
      const parsedState = JSON.parse(storedState);
      return {
        token: parsedState.token || '',
        refreshTokenValue: parsedState.refreshTokenValue || parsedState.refreshToken || '', // 兼容旧键名
        userInfo: parsedState.userInfo || null,
        currentTenant: parsedState.currentTenant || null,
        permissions: parsedState.permissions || [],
        roles: parsedState.roles || [],
        loginTime: parsedState.loginTime || null
      };
    }
  } catch (error) {
    console.error('从 localStorage 恢复用户状态失败:', error);
  }

  // 默认初始状态
  return {
    token: '',
    refreshTokenValue: '', // 更新字段名
    userInfo: null,
    currentTenant: null,
    permissions: [],
    roles: [],
    loginTime: null
  };
};


interface UserState {
  token: string;
  refreshTokenValue: string; // 重命名避免与action冲突
  userInfo: User | null;
  currentTenant: Tenant | null;
  permissions: string[];
  roles: string[];
  loginTime: number | null;
}

export const useUserStore = defineStore('user', {
  state: (): UserState => getInitialState(),

  getters: {
    isLoggedIn: (state) => !!state.token,
    userName: (state) => state.userInfo?.displayName || state.userInfo?.username || '',
    avatar: (state) => state.userInfo?.avatar || '',
    tenantName: (state) => state.currentTenant?.name || '',
    hasPermission: (state) => (permission: string) => {
      if (!permission) return true;
      return state.permissions.includes(permission);
    },
    hasRole: (state) => (role: string) => {
      return state.roles.includes(role);
    },
    dataScope: (state) => {
      return state.userInfo?.roles?.[0]?.dataScope || 'Self';
    }
  },

  actions: {
    /**
     * 设置用户和令牌信息（由 SystemInit 调用）
     */
    setTokenAndUser(responseData: any) {
      if (!responseData || !responseData.token) {
        console.error('❌ [Auth] setTokenAndUser 失败: 无效的响应数据');
        return;
      }

      // 1. 更新 Pinia State
      this.token = responseData.token.accessToken;
      this.refreshTokenValue = responseData.token.refreshToken;
      this.loginTime = Date.now();
      this.userInfo = responseData.user;
      this.roles = responseData.user?.roles?.map((r:any) => r.code) || [];
      this.permissions = responseData.permissions || [];
      this.currentTenant = {
        id: responseData.user?.tenantId,
        code: 'DUMMY_CODE', // 登录接口似乎没返回，需要确认
        name: responseData.user?.organizationName || '默认租户',
        status: 1,
        createdAt: new Date().toISOString(),
        isolationMode: 'Shared',
        logo: ''
      };

      // 2. 将核心状态持久化到 localStorage
      try {
        const persistentState = {
          token: this.token,
          refreshTokenValue: this.refreshTokenValue, // 保持键名统一
          loginTime: this.loginTime,
          currentTenant: this.currentTenant,
          userInfo: this.userInfo,
          permissions: this.permissions,
          roles: this.roles
        };
        localStorage.setItem('user', JSON.stringify(persistentState));
        console.log('💾 [Auth] 已将用户状态固化到 localStorage');
      } catch (e) {
        console.error('[持久化失败] 无法写入 user state 到 localStorage:', e);
      }
    },

    /**
     * @description 用户登录 (现在委托给 SystemInit)
     * @param {LoginRequest} loginForm - 登录表单
     */
    async login(loginForm: LoginRequest) {
      try {
        console.log('🚀 [Auth] 开始登录请求:', loginForm);

        // http.ts 拦截器会处理外层包装，这里拿到的是完整的 ApiResult 对象
        const response = await loginApi(loginForm) as any;
        console.log('✅ [Auth] 登录接口返回的完整响应:', response);

        // [最终修正] 从完整的 ApiResult 中解构出真正的核心数据 data
        const responseData = response.data;
        if (!responseData) {
          throw new Error('登录响应中缺少核心 data 对象');
        }

        // 从核心数据中解析 token
        const tokenStr = responseData.token;
        const refreshTokenStr = responseData.refreshToken;

        if (!tokenStr || !refreshTokenStr) {
          console.error('❌ [Auth] 登录响应无效，缺少 token 或 refreshToken', responseData);
          throw new Error('登录响应中缺少有效的 token 或 refreshToken');
        }

        // 1. 立即更新 Pinia State
        this.token = tokenStr;
        this.refreshTokenValue = refreshTokenStr;
        this.loginTime = Date.now();
        this.userInfo = responseData.user;
        this.roles = responseData.user?.roles?.map((r:any) => r.code) || [];
        this.permissions = responseData.permissions || [];
        this.currentTenant = {
          id: responseData.tenantId,
          code: responseData.tenantCode || 'DEFAULT',
          name: responseData.tenantName || '默认租户',
          status: 1,
          createdAt: new Date().toISOString(),
          isolationMode: 'Shared',
          logo: ''
        };

        // 2. 将核心状态持久化到 localStorage
        try {
          const persistentState = {
            token: this.token,
            refreshTokenValue: this.refreshTokenValue, // 保持键名统一
            loginTime: this.loginTime,
            currentTenant: this.currentTenant,
            userInfo: this.userInfo,
            permissions: this.permissions,
            roles: this.roles
          };
          localStorage.setItem('user', JSON.stringify(persistentState));
          console.log('💾 [Auth] 已将用户状态固化到 localStorage');
        } catch (e) {
          console.error('[持久化失败] 无法写入 user state 到 localStorage:', e);
        }

        return { success: true };
      } catch (error: any) {
        console.error('❌ [Auth] 登录失败:', error);
        return { success: false, message: error.message || '登录失败' };
      }
    },

    /**
     * @description 刷新令牌
     * @returns {Promise<string>} 新的 accessToken
     */
    async refreshToken(): Promise<string> {
      console.log('🔄 [Auth] 尝试刷新令牌...');
      try {
        if (!this.refreshTokenValue) {
          throw new Error('无可用刷新令牌');
        }

        // http.ts 拦截器会处理外层包装，这里拿到的是核心 data
        const response = await refreshTokenApi(this.refreshTokenValue);

        if (!response || !response.accessToken) {
          throw new Error('刷新令牌响应无效');
        }

        const { accessToken, refreshToken: newRefreshToken } = response;

        // 1. 更新 Pinia State
        this.token = accessToken;
        if (newRefreshToken) {
          this.refreshTokenValue = newRefreshToken;
        }
        this.loginTime = Date.now();

        // 2. 更新 localStorage
        try {
          const storedState = JSON.parse(localStorage.getItem('user') || '{}');
          storedState.token = this.token;
          storedState.refreshTokenValue = this.refreshTokenValue; // 保持键名统一
          delete storedState.refreshToken; // 删除旧键名
          storedState.loginTime = this.loginTime;
          localStorage.setItem('user', JSON.stringify(storedState));
          console.log('💾 [Auth] 刷新令牌后，已更新 localStorage');
        } catch (e) {
          console.error('[持久化失败] 刷新令牌后无法更新 localStorage:', e);
        }

        console.log('✅ [Auth] 令牌刷新成功');
        return accessToken;
      } catch (error) {
        console.error('❌ [Auth] 刷新令牌失败:', error);
        // 刷新失败，直接重置用户状态并跳转登录，而不是再次调用logout()，避免死循环
        this.resetUser();
        router.push('/login');
        // 必须抛出错误，以便 http.ts 中的调用者能捕获到失败
        throw error;
      }
    },

    /**
     * @description 获取当前用户信息
     */
      async getUserInfo() {
        try {
          const { data } = (await getCurrentUser()) as any;
          if (data && data.user) {
            this.userInfo = data.user;
            this.permissions = data.permissions || [];
            this.roles = data.user.roles?.map((r: any) => r.code) || [];
          } else {
            throw new Error('API返回的用户信息格式不正确');
          }
        } catch (error) {
          console.error('获取用户信息失败:', error);
          await this.logout();
          throw error;
        }
      },

      /**
       * @description 用户登出
       */
      async logout() {
        try {
          await logoutApi();
        } catch (error) {
          console.error('登出接口调用失败:', error);
        } finally {
          this.resetUser();
          resetRouter();
          router.push('/login');
        }
      },

      /**
       * @description 重置用户所有状态
       */
      resetUser() {
        // 1. 清理 localStorage
        try {
          localStorage.removeItem('user');
        } catch (e) {
          console.error('[持久化失败] 无法重置 user state 到 localStorage:', e);
        }

        // 2. 重置 Pinia state
        this.token = '';
        this.refreshTokenValue = '';
        this.userInfo = null;
        this.currentTenant = null;
        this.permissions = [];
        this.roles = [];
        this.loginTime = null;

        // 3. 清理菜单状态
        const menuStore = useMenuStore();
        menuStore.clearMenus();

        console.log('🧹 [Auth] 用户状态已完全重置');
      },

      /**
       * @description 切换租户
       * @param {Tenant} tenant - 目标租户
       */
      async switchTenant(tenant: Tenant) {
        this.currentTenant = tenant;
        await this.getUserInfo();
        ElMessage.success(`已切换到 ${tenant.name}`);
        router.push('/');
      },

      /**
       * @description 检查权限（从服务器验证）
       * @param {string} resourceCode - 资源编码
       * @param {Record<string, any>} [context] - 上下文
       */
      async checkPermission(resourceCode: string, context?: Record<string, any>): Promise<boolean> {
        return await PermissionService.checkPermission(resourceCode, context);
      },

      /**
       * @description 检查是否拥有任意一个权限
       * @param {string[]} resourceCodes - 资源编码列表
       */
      async hasAnyPermission(resourceCodes: string[]): Promise<boolean> {
        return await PermissionService.hasAnyPermission(resourceCodes);
      },

      /**
       * @description 检查是否拥有所有权限
       * @param {string[]} resourceCodes - 资源编码列表
       */
      async hasAllPermissions(resourceCodes: string[]): Promise<boolean> {
        return await PermissionService.hasAllPermissions(resourceCodes);
      },

      /**
       * @description 检查数据范围权限
       * @param {string} [targetUserId] - 目标用户ID
       * @param {string} [targetOrgId] - 目标组织ID
       */
      checkDataScope(targetUserId?: string, targetOrgId?: string): boolean {
        if (!this.userInfo) return false;

        return PermissionService.checkDataScope(
          this.dataScope,
          this.userInfo.id,
          targetUserId,
          targetOrgId
        );
      },

      /**
       * @description 刷新用户权限
       */
      async refreshPermissions() {
        try {
          const permissions = await PermissionService.getUserPermissions();
          this.permissions = permissions;
          return permissions;
        } catch (error) {
          console.error('刷新权限失败:', error);
          return [];
        }
      },

      /**
       * @description 刷新用户菜单
       */
      async refreshMenus() {
        try {
          const menus = await PermissionService.getUserMenus();
          const menuStore = useMenuStore();
          await menuStore.generateRoutes(menus);
          return menus;
        } catch (error) {
          console.error('刷新菜单失败:', error);
          return [];
        }
      },

      /**
       * @description 检查是否为系统管理员
       */
      isSystemAdmin(): boolean {
        return this.roles.includes('SYSTEM_ADMIN') || this.roles.includes('SUPER_ADMIN');
      },

      /**
       * @description 检查是否为租户管理员
       */
      isTenantAdmin(): boolean {
        return this.roles.includes('TENANT_ADMIN') || this.roles.includes('ADMIN');
      },

      /**
       * @description 获取用户组织路径
       */
      getOrganizationPath(): string[] {
        if (!this.userInfo?.organizationPath) return [];
        return (this.userInfo.organizationPath as unknown as string).split('/');
      },

      /**
       * @description 检查是否在指定组织下
       * @param {string} organizationId - 组织ID
       */
      isInOrganization(organizationId: string): boolean {
        const orgPath = this.getOrganizationPath();
        return orgPath.includes(organizationId);
      }
    }
  // 彻底移除 pinia-plugin-persistedstate 的配置
  // persist: { ... }
});
