// =============================================
// 基础API服务
// =============================================
import { request } from '../request';
import type { ApiResponse, PagedResult } from '@/types/global';
import type { PageRequest, QueryParams } from '@/types/api';

/**
 * 基础API服务类
 * 提供通用的CRUD操作
 */
export class BaseApiService<T = any, CreateDto = any, UpdateDto = any> {
  /**
   * 构造函数
   * @param baseUrl API基础路径
   */
  constructor(protected baseUrl: string) {}

  /**
   * 获取列表（分页）
   * @param params 查询参数
   * @returns 分页结果
   */
  async getList(params?: QueryParams): Promise<ApiResponse<PagedResult<T>>> {
    return request.get<PagedResult<T>>(this.baseUrl, params);
  }

  /**
   * 获取所有记录（不分页）
   * @returns 记录列表
   */
  async getAll(): Promise<ApiResponse<T[]>> {
    return request.get<T[]>(`${this.baseUrl}/all`);
  }

  /**
   * 根据ID获取详情
   * @param id 记录ID
   * @returns 记录详情
   */
  async getById(id: string): Promise<ApiResponse<T>> {
    return request.get<T>(`${this.baseUrl}/${id}`);
  }

  /**
   * 创建记录
   * @param data 创建数据
   * @returns 创建结果
   */
  async create(data: CreateDto): Promise<ApiResponse<T>> {
    return request.post<T>(this.baseUrl, data);
  }

  /**
   * 更新记录
   * @param id 记录ID
   * @param data 更新数据
   * @returns 更新结果
   */
  async update(id: string, data: UpdateDto): Promise<ApiResponse<T>> {
    return request.put<T>(`${this.baseUrl}/${id}`, data);
  }

  /**
   * 删除记录
   * @param id 记录ID
   * @returns 删除结果
   */
  async delete(id: string): Promise<ApiResponse<boolean>> {
    return request.delete<boolean>(`${this.baseUrl}/${id}`);
  }

  /**
   * 批量删除记录
   * @param ids 记录ID数组
   * @returns 删除结果
   */
  async batchDelete(ids: string[]): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/batch-delete`, { ids });
  }
}