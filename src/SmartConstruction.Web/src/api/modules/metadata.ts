// =============================================
// 元数据相关API
// =============================================
import { request } from '@/api/request'

// 获取元数据定义列表
export function getMetadataDefinitions(params?: {
  entityType?: string
  page?: number
  pageSize?: number
}) {
  return request.get<PagedResult<MetadataDefinition>>('/metadata/definitions', params)
}

// 获取单个元数据定义
export function getMetadataDefinition(id: string) {
  return request.get<MetadataDefinition>(`/metadata/definitions/${id}`)
}

// 创建元数据定义
export function createMetadataDefinition(data: Partial<MetadataDefinition>) {
  return request.post<MetadataDefinition>('/metadata/definitions', data)
}

// 更新元数据定义
export function updateMetadataDefinition(id: string, data: Partial<MetadataDefinition>) {
  return request.put<MetadataDefinition>(`/metadata/definitions/${id}`, data)
}

// 删除元数据定义
export function deleteMetadataDefinition(id: string) {
  return request.delete(`/metadata/definitions/${id}`)
}

// 获取实体的元数据值
export function getEntityMetadata(entityType: string, entityId: string) {
  return request.get<MetadataValue[]>(`/metadata/${entityType}/${entityId}`)
}

// 设置元数据值
export function setMetadataValue(entityType: string, entityId: string, metaKey: string, value: any) {
  return request.put(`/metadata/${entityType}/${entityId}/${metaKey}`, { value })
}

// 批量设置元数据值
export function setMetadataValues(entityType: string, entityId: string, metadata: Record<string, any>) {
  return request.post(`/metadata/bulk/set`, {
    entityType,
    entityId,
    metadata
  })
}