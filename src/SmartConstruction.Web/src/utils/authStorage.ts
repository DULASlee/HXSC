// =============================================
// 认证状态持久化服务 (localStorage)
// 遵循单一职责原则，将 localStorage 操作与 Pinia Store 分离
// =============================================
import type { User, Tenant } from '@/types/global';

const AUTH_STORAGE_KEY = 'user';

/**
 * 定义存储在 localStorage 中的认证状态的结构
 */
export interface AuthState {
  token: string;
  refreshTokenValue: string;
  userInfo: User | null;
  currentTenant: Tenant | null;
  permissions: string[];
  roles: string[];
  loginTime: number | null;
}

/**
 * 从 localStorage 获取完整的认证状态
 * @returns {AuthState | null}
 */
export function getAuthState(): AuthState | null {
  try {
    const storedState = localStorage.getItem(AUTH_STORAGE_KEY);
    if (storedState) {
      const parsedState = JSON.parse(storedState) as AuthState;
      // 可选：在这里可以添加对恢复数据的校验逻辑
      return parsedState;
    }
  } catch (error) {
    console.error('从 localStorage 恢复认证状态失败:', error);
    // 如果解析失败，清除可能已损坏的数据
    clearAuthState();
  }
  return null;
}

/**
 * 将完整的认证状态写入 localStorage
 * @param {AuthState} state - 要存储的认证状态
 */
export function setAuthState(state: AuthState): void {
  try {
    const stateString = JSON.stringify(state);
    localStorage.setItem(AUTH_STORAGE_KEY, stateString);
  } catch (error) {
    console.error('写入认证状态到 localStorage 失败:', error);
  }
}

/**
 * 从 localStorage 清除认证状态
 */
export function clearAuthState(): void {
  try {
    localStorage.removeItem(AUTH_STORAGE_KEY);
  } catch (error) {
    console.error('从 localStorage 清除认证状态失败:', error);
  }
} 