// =============================================
// 系统服务
// =============================================
import { request } from '../request';
import type { ApiResponse } from '@/types/global';
import type { SystemInfo, CacheStatus } from '@/types/api';

/**
 * 系统服务类
 */
export class SystemService {
  private baseUrl = '/api/system';
  private cacheUrl = '/api/cache';

  /**
   * 获取系统信息
   * @returns 系统信息
   */
  async getInfo(): Promise<ApiResponse<SystemInfo>> {
    return request.get<SystemInfo>(`${this.baseUrl}/info`);
  }

  /**
   * 获取缓存状态
   * @returns 缓存状态
   */
  async getCacheStatus(): Promise<ApiResponse<CacheStatus>> {
    return request.get<CacheStatus>(`${this.cacheUrl}/status`);
  }

  /**
   * 清除缓存
   * @returns 操作结果
   */
  async clearCache(): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.cacheUrl}/clear`);
  }

  /**
   * 手动初始化数据
   * @returns 操作结果
   */
  async seedData(): Promise<ApiResponse<boolean>> {
    return request.post<boolean>(`${this.baseUrl}/seed-data`);
  }
}