// =============================================
// 审计日志服务
// =============================================
import { request } from '../request';
import type { ApiResponse, PagedResult } from '@/types/global';
import type { AuditLogQueryParams, AuditLogExportParams } from '@/types/api';

/**
 * 审计日志服务类
 */
export class AuditService {
  private baseUrl = '/api/audit';

  /**
   * 获取审计日志列表
   * @param params 查询参数
   * @returns 分页结果
   */
  async getList(params?: AuditLogQueryParams): Promise<ApiResponse<PagedResult<any>>> {
    return request.get<PagedResult<any>>(this.baseUrl, params);
  }

  /**
   * 导出审计日志
   * @param params 导出参数
   * @returns 文件下载URL
   */
  async export(params: AuditLogExportParams): Promise<ApiResponse<string>> {
    return request.get<string>(`${this.baseUrl}/export`, params);
  }

  /**
   * 获取审计日志详情
   * @param id 日志ID
   * @returns 日志详情
   */
  async getById(id: string): Promise<ApiResponse<any>> {
    return request.get<any>(`${this.baseUrl}/${id}`);
  }
}