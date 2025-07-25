// =============================================
// 元数据服务 - 对应后端 IMetadataService
// =============================================
import { request } from '@/api/request'

export class MetadataService {
  // 获取单个元数据值
  static async getMetadata<T = any>(entityType: string, entityId: string, key: string): Promise<T | null> {
    try {
      const { data } = await request.get<T>(`/metadata/${entityType}/${entityId}/${key}`)
      return data
    } catch (error) {
      console.error('获取元数据失败:', error)
      return null
    }
  }

  // 设置元数据值
  static async setMetadata<T = any>(entityType: string, entityId: string, key: string, value: T): Promise<void> {
    await request.put(`/metadata/${entityType}/${entityId}/${key}`, { value })
  }

  // 获取实体的所有元数据
  static async getEntityMetadata(entityType: string, entityId: string): Promise<Record<string, any>> {
    const { data } = await request.get<Record<string, any>>(`/metadata/${entityType}/${entityId}`)
    return data
  }

  // 批量设置元数据
  static async setEntityMetadata(entityType: string, entityId: string, metadata: Record<string, any>): Promise<void> {
    await request.post(`/metadata/${entityType}/${entityId}/batch`, { metadata })
  }

  // 删除元数据
  static async deleteMetadata(entityType: string, entityId: string, key: string): Promise<boolean> {
    const { data } = await request.delete<boolean>(`/metadata/${entityType}/${entityId}/${key}`)
    return data
  }

  // 搜索元数据
  static async searchMetadata(params: MetadataQueryRequest): Promise<MetadataDto[]> {
    const { data } = await request.get<MetadataDto[]>('/metadata/search', params)
    return data
  }

  // 获取元数据定义
  static async getMetadataDefinitions(entityType?: string): Promise<MetadataDefinition[]> {
    const { data } = await request.get<MetadataDefinition[]>('/metadata/definitions', {
      entityType
    })
    return data
  }

  // 验证元数据值
  static validateMetadataValue(definition: MetadataDefinition, value: any): { valid: boolean; error?: string } {
    // 必填验证
    if (definition.required && (value === null || value === undefined || value === '')) {
      return { valid: false, error: `${definition.displayName}是必填项` }
    }

    // 类型验证
    if (value !== null && value !== undefined && value !== '') {
      switch (definition.valueType) {
        case 'String':
          if (typeof value !== 'string') {
            return { valid: false, error: `${definition.displayName}必须是字符串类型` }
          }
          break
        case 'Number':
          if (typeof value !== 'number' && isNaN(Number(value))) {
            return { valid: false, error: `${definition.displayName}必须是数字类型` }
          }
          break
        case 'Boolean':
          if (typeof value !== 'boolean') {
            return { valid: false, error: `${definition.displayName}必须是布尔类型` }
          }
          break
        case 'Date':
          if (!(value instanceof Date) && isNaN(Date.parse(value))) {
            return { valid: false, error: `${definition.displayName}必须是有效的日期格式` }
          }
          break
      }
    }

    // 选项验证
    if (definition.options && definition.options.length > 0) {
      const validValues = definition.options.map(opt => opt.value)
      if (!validValues.includes(String(value))) {
        return { valid: false, error: `${definition.displayName}的值不在允许的选项中` }
      }
    }

    // 自定义验证规则
    if (definition.validationRule) {
      try {
        const regex = new RegExp(definition.validationRule)
        if (!regex.test(String(value))) {
          return { valid: false, error: `${definition.displayName}格式不正确` }
        }
      } catch (error) {
        console.error('验证规则错误:', error)
      }
    }

    return { valid: true }
  }

  // 格式化元数据值用于显示
  static formatMetadataValue(definition: MetadataDefinition, value: any): string {
    if (value === null || value === undefined) {
      return '-'
    }

    switch (definition.valueType) {
      case 'Boolean':
        return value ? '是' : '否'
      case 'Date':
        return new Date(value).toLocaleDateString()
      case 'JSON':
        return typeof value === 'object' ? JSON.stringify(value, null, 2) : String(value)
      default:
        // 如果有选项，显示对应的标签
        if (definition.options && definition.options.length > 0) {
          const option = definition.options.find(opt => opt.value === String(value))
          return option ? option.label : String(value)
        }
        return String(value)
    }
  }

  // 获取元数据的默认值
  static getDefaultValue(definition: MetadataDefinition): any {
    if (definition.defaultValue !== undefined) {
      return definition.defaultValue
    }

    switch (definition.valueType) {
      case 'String':
        return ''
      case 'Number':
        return 0
      case 'Boolean':
        return false
      case 'Date':
        return new Date()
      case 'JSON':
        return {}
      default:
        return null
    }
  }

  // 转换元数据值类型
  static convertMetadataValue(definition: MetadataDefinition, value: any): any {
    if (value === null || value === undefined || value === '') {
      return null
    }

    switch (definition.valueType) {
      case 'String':
        return String(value)
      case 'Number':
        return Number(value)
      case 'Boolean':
        return Boolean(value)
      case 'Date':
        return value instanceof Date ? value : new Date(value)
      case 'JSON':
        return typeof value === 'string' ? JSON.parse(value) : value
      default:
        return value
    }
  }
}