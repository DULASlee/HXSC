import { defineStore } from 'pinia';
import { computed } from 'vue';
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
} from '@/api/modules/user';
import type { LoginRequest, User, Tenant } from '@/types/global';
import { getAuthState, setAuthState, clearAuthState, type AuthState } from '@/utils/authStorage';
import { resetAllStores } from '@/stores';

// =============================================
// 重构后的初始化流程
// =============================================
const getInitialState = (): AuthState => {
  const storedState = getAuthState();
  if (storedState) {
    return storedState;
  }
  
  // 默认初始状态
  return {
    token: '',
    refreshTokenValue: '',
    userInfo: null,
    currentTenant: null,
    permissions: [],
    roles: [],
    loginTime: null
  };
};

// [重要] UserState 接口现在直接复用 AuthState 接口
type UserState = AuthState;

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
     * [内部] 统一更新内存和持久化状态
     */
    _updateAuthState(partialState: Partial<UserState>) {
      // 1. 更新 Pinia State
      Object.assign(this, partialState);
      
      // 2. 更新 localStorage
      const currentState = {
        token: this.token,
        refreshTokenValue: this.refreshTokenValue,
        userInfo: this.userInfo,
        currentTenant: this.currentTenant,
        permissions: this.permissions,
        roles: this.roles,
        loginTime: this.loginTime,
      };
      setAuthState(currentState);
    },

    /**
     * @description 用户登录
     */
    async login(loginForm: LoginRequest) {
      try {
        const responseData = await loginApi(loginForm) as any;
        
        if (!responseData || !responseData.token || !responseData.refreshToken) {
          throw new Error('登录响应中缺少有效的 token 或 refreshToken');
        }

        const newUserState: UserState = {
          token: responseData.token,
          refreshTokenValue: responseData.refreshToken,
          userInfo: responseData.user,
          roles: responseData.user?.roles?.map((r:any) => r.code) || [],
          permissions: responseData.permissions || [],
          currentTenant: {
            id: responseData.tenantId,
            code: responseData.tenantCode || 'DEFAULT',
            name: responseData.tenantName || '默认租户',
            status: 1,
            createdAt: new Date().toISOString(),
            isolationMode: 'Shared',
            logo: ''
          },
          loginTime: Date.now()
        };
        
        this._updateAuthState(newUserState);

        return { success: true };
      } catch (error: any) {
        await this.resetUser();
        return { success: false, message: error.message || '登录失败' };
      }
    },

    /**
     * @description 刷新令牌
     */
    async refreshToken(): Promise<string> {
      try {
        if (!this.refreshTokenValue) {
          throw new Error('无可用刷新令牌');
        }

        const response = await refreshTokenApi(this.refreshTokenValue);

        if (!response || !response.accessToken) {
          throw new Error('刷新令牌响应无效');
        }
        
        this._updateAuthState({
          token: response.accessToken,
          refreshTokenValue: response.refreshToken || this.refreshTokenValue,
          loginTime: Date.now()
        });
        
        console.log('✅ [Auth] 令牌刷新成功');
        return response.accessToken;
      } catch (error) {
        console.error('❌ [Auth] 刷新令牌失败，将重置用户状态');
        await this.resetUser();
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
          resetAllStores();
          router.push('/login');
        }
      },

      /**
       * @description 重置用户所有状态
       */
      async resetUser() {
        // 1. 清理 localStorage
        clearAuthState();

        // 2. 重置 Pinia state
        this.$reset();

        // [最终解耦] userStore 不再负责清理 menuStore，各自的状态由自己管理
        // const menuStore = useMenuStore();
        // menuStore.clearMenus();
        
        // 3. 重置路由是必要的，因为它依赖于权限
        resetRouter();

        console.log('🧹 [Auth] 用户状态已完全重置');
      },

      /**
       * @description 获取当前用户信息
       */
      async getUserInfo() {
        try {
          const { data } = (await getCurrentUser()) as any;
          if (data && data.user) {
            this._updateAuthState({
              userInfo: data.user,
              permissions: data.permissions || [],
              roles: data.user.roles?.map((r: any) => r.code) || [],
            });
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
