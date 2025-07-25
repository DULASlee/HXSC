// =============================================
// 资源服务
// =============================================
import { BaseApiService } from './base.service';
import { request } from '../request';
import type { ApiResponse, Resource } from '@/types/global';
import type { CreateResourceParams, UpdateResourceParams } from '@/types/api';

/**
 * 资源服务类
 */
export class ResourceService extends BaseApiService<Resource, CreateResourceParams, UpdateResourceParams> {
  constructor() {
    super('/api/resources');
  }

  /**
   * 获取资源树结构
   * @returns 资源树
   */
  async getTree(): Promise<ApiResponse<Resource[]>> {
    return request.get<Resource[]>(`${this.baseUrl}/tree`);
  }

  /**
   * 根据类型获取资源列表
   * @param type 资源类型
   * @returns 资源列表
   */
  async getByType(type: string): Promise<ApiResponse<Resource[]>> {
    return request.get<Resource[]>(`${this.baseUrl}/type/${type}`);
  }
}