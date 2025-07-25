// =============================================
// 角色服务
// =============================================
import { BaseApiService } from './base.service';
import { request } from '../request';
import type { ApiResponse, Role } from '@/types/global';
import type { CreateRoleParams, UpdateRoleParams, AssignPermissionsParams } from '@/types/api';

/**
 * 角色服务类
 */
export class RoleService extends BaseApiService<Role, CreateRoleParams, UpdateRoleParams> {
  constructor() {
    super('/api/roles');
  }

  /**
   * 为角色分配权限
   * @param roleId 角色ID
   * @param params 权限分配参数
   * @returns 分配结果
   */
  async assignPermissions(roleId: string, params: AssignPermissionsParams): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/${roleId}/permissions`, params);
  }

  /**
   * 获取角色的权限列表
   * @param roleId 角色ID
   * @returns 权限列表
   */
  async getPermissions(roleId: string): Promise<ApiResponse<string[]>> {
    return request.get<string[]>(`${this.baseUrl}/${roleId}/permissions`);
  }
}