// =============================================
// 认证服务
// =============================================
import { request } from '../request';
import type { ApiResponse, User } from '@/types/global';
import type { 
  LoginParams, 
  LoginResult, 
  RefreshTokenParams, 
  ChangePasswordParams 
} from '@/types/api';

/**
 * 认证服务类
 */
export class AuthService {
  private baseUrl = '/api/auth';

  /**
   * 用户登录
   * @param params 登录参数
   * @returns 登录结果
   */
  async login(params: LoginParams): Promise<ApiResponse<LoginResult>> {
    return request.post<LoginResult>(`${this.baseUrl}/login`, params);
  }

  /**
   * 用户登出
   * @returns 登出结果
   */
  async logout(): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/logout`);
  }

  /**
   * 获取当前用户信息
   * @returns 用户信息
   */
  async getCurrentUser(): Promise<ApiResponse<User>> {
    return request.get<User>(`${this.baseUrl}/me`);
  }

  /**
   * 刷新令牌
   * @param params 刷新令牌参数
   * @returns 新令牌
   */
  async refreshToken(params: RefreshTokenParams): Promise<ApiResponse<LoginResult>> {
    return request.post<LoginResult>(`${this.baseUrl}/refresh-token`, params);
  }

  /**
   * 修改密码
   * @param params 修改密码参数
   * @returns 修改结果
   */
  async changePassword(params: ChangePasswordParams): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/change-password`, params);
  }

  /**
   * 验证令牌
   * @returns 验证结果
   */
  async validateToken(): Promise<ApiResponse<boolean>> {
    return request.get<boolean>(`${this.baseUrl}/validate`);
  }
}