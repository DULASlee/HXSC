// =============================================
// 组织服务
// =============================================
import { BaseApiService } from './base.service';
import { request } from '../request';
import type { ApiResponse, Organization } from '@/types/global';
import type { CreateOrganizationParams, UpdateOrganizationParams } from '@/types/api';

/**
 * 组织服务类
 */
export class OrganizationService extends BaseApiService<Organization, CreateOrganizationParams, UpdateOrganizationParams> {
  constructor() {
    super('/api/organizations');
  }

  /**
   * 获取组织树结构
   * @returns 组织树
   */
  async getTree(): Promise<ApiResponse<Organization[]>> {
    return request.get<Organization[]>(`${this.baseUrl}/tree`);
  }

  /**
   * 获取指定组织的用户列表
   * @param orgId 组织ID
   * @returns 用户列表
   */
  async getUsers(orgId: string): Promise<ApiResponse<any[]>> {
    return request.get<any[]>(`${this.baseUrl}/${orgId}/users`);
  }
}