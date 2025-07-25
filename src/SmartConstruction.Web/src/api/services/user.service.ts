// =============================================
// 用户服务
// =============================================
import { BaseApiService } from './base.service';
import { request } from '../request';
import type { ApiResponse, User } from '@/types/global';
import type { CreateUserParams, UpdateUserParams, AssignRolesParams } from '@/types/api';

/**
 * 用户服务类
 */
export class UserService extends BaseApiService<User, CreateUserParams, UpdateUserParams> {
  constructor() {
    super('/api/users');
  }

  /**
   * 为用户分配角色
   * @param userId 用户ID
   * @param params 角色分配参数
   * @returns 分配结果
   */
  async assignRoles(userId: string, params: AssignRolesParams): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/${userId}/roles`, params);
  }

  /**
   * 重置用户密码
   * @param userId 用户ID
   * @returns 重置结果
   */
  async resetPassword(userId: string): Promise<ApiResponse<string>> {
    return request.post<string>(`${this.baseUrl}/${userId}/reset-password`);
  }

  /**
   * 启用用户
   * @param userId 用户ID
   * @returns 操作结果
   */
  async enable(userId: string): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/${userId}/enable`);
  }

  /**
   * 禁用用户
   * @param userId 用户ID
   * @returns 操作结果
   */
  async disable(userId: string): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/${userId}/disable`);
  }
}