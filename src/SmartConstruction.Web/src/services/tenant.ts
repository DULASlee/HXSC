// =============================================
// 租户服务 - 对应后端 ITenantService
// =============================================
import { request } from '@/api/request'
import { ElMessage } from 'element-plus'

export class TenantService {
  // 获取租户列表（分页）
  static async getTenants(params: TenantQueryRequest): Promise<PagedResult<Tenant>> {
    const { data } = await request.get<PagedResult<Tenant>>('/api/tenant', params)
    return data
  }

  // 获取租户详情
  static async getTenant(id: string): Promise<TenantDetailDto> {
    const { data } = await request.get<TenantDetailDto>(`/api/tenant/${id}`)
    return data
  }

  // 创建租户
  static async createTenant(request: CreateTenantRequest): Promise<string> {
    const { data } = await request.post<string>('/api/tenant', request)
    ElMessage.success('租户创建成功')
    return data
  }

  // 更新租户
  static async updateTenant(id: string, request: UpdateTenantRequest): Promise<void> {
    await request.put(`/api/tenant/${id}`, request)
    ElMessage.success('租户更新成功')
  }

  // 切换租户状态
  static async toggleTenantStatus(id: string): Promise<boolean> {
    const { data } = await request.post<boolean>(`/api/tenant/${id}/toggle-status`)
    ElMessage.success('租户状态切换成功')
    return data
  }

  // 获取租户隔离配置
  static async getTenantIsolation(id: string): Promise<TenantIsolationDto> {
    const { data } = await request.get<TenantIsolationDto>(`/api/tenant/${id}/isolation`)
    return data
  }

  // 升级租户隔离级别
  static async upgradeTenantIsolation(id: string, targetLevel: 'Shared' | 'Schema' | 'Database'): Promise<boolean> {
    const { data } = await request.post<boolean>(`/api/tenant/${id}/upgrade-isolation`, {
      targetLevel
    })
    ElMessage.success(`租户隔离级别升级到 ${targetLevel} 成功`)
    return data
  }

  // 获取租户统计信息
  static async getTenantStats(id: string): Promise<any> {
    const { data } = await request.get(`/api/tenant/${id}/stats`)
    return data
  }

  // 检查租户代码是否可用
  static async checkTenantCodeAvailable(code: string, excludeId?: string): Promise<boolean> {
    const { data } = await request.get<boolean>('/api/tenant/check-code', {
      code,
      excludeId
    })
    return data
  }

  // 获取租户隔离级别说明
  static getIsolationLevelDescription(level: string): string {
    switch (level) {
      case 'Shared':
        return '共享模式：多个租户共享同一数据库和表，通过TenantId字段隔离数据'
      case 'Schema':
        return 'Schema模式：每个租户使用独立的数据库Schema，提供更好的数据隔离'
      case 'Database':
        return '数据库模式：每个租户使用完全独立的数据库，提供最高级别的数据隔离'
      default:
        return '未知隔离模式'
    }
  }

  // 获取隔离级别升级建议
  static getUpgradeRecommendation(isolation: TenantIsolationDto): string | null {
    if (isolation.currentLevel === 'Shared' && isolation.recordCount > (isolation.upgradeThreshold || 10000)) {
      return '建议升级到Schema模式以获得更好的性能和数据隔离'
    }
    
    if (isolation.currentLevel === 'Schema' && isolation.recordCount > 100000) {
      return '建议升级到Database模式以获得最佳性能和完全的数据隔离'
    }
    
    return null
  }

  // 计算存储使用率
  static calculateStorageUsage(usedMB: number, limitMB: number = 1024): number {
    return Math.round((usedMB / limitMB) * 100)
  }

  // 格式化存储大小
  static formatStorageSize(sizeInMB: number): string {
    if (sizeInMB < 1024) {
      return `${sizeInMB.toFixed(1)} MB`
    } else {
      return `${(sizeInMB / 1024).toFixed(1)} GB`
    }
  }
}