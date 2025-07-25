// =============================================
// 元数据服务
// =============================================
import { BaseApiService } from './base.service';
import { request } from '../request';
import type { ApiResponse, MetadataDefinition } from '@/types/global';
import type { CreateMetadataParams, UpdateMetadataParams } from '@/types/api';

/**
 * 元数据服务类
 */
export class MetadataService extends BaseApiService<MetadataDefinition, CreateMetadataParams, UpdateMetadataParams> {
  constructor() {
    super('/api/metadata');
  }

  /**
   * 根据实体类型获取元数据定义
   * @param entityType 实体类型
   * @returns 元数据定义列表
   */
  async getByEntityType(entityType: string): Promise<ApiResponse<MetadataDefinition[]>> {
    return request.get<MetadataDefinition[]>(`${this.baseUrl}/entity-type/${entityType}`);
  }

  /**
   * 获取实体的元数据值
   * @param entityType 实体类型
   * @param entityId 实体ID
   * @returns 元数据值列表
   */
  async getEntityMetadata(entityType: string, entityId: string): Promise<ApiResponse<any>> {
    return request.get<any>(`${this.baseUrl}/values/${entityType}/${entityId}`);
  }

  /**
   * 保存实体的元数据值
   * @param entityType 实体类型
   * @param entityId 实体ID
   * @param values 元数据值
   * @returns 保存结果
   */
  async saveEntityMetadata(entityType: string, entityId: string, values: Record<string, any>): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/values/${entityType}/${entityId}`, values);
  }
}