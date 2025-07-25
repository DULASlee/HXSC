// =============================================
// 资源服务 - 对应后端 IResourceService
// =============================================
import { request } from '@/api/request'

export class ResourceService {
  // 获取资源树
  static async getResourceTree(): Promise<ResourceTreeDto[]> {
    const { data } = await request.get<ResourceTreeDto[]>('/resources/tree')
    return data
  }

  // 获取资源列表（分页）
  static async getResources(params: ResourceQueryRequest): Promise<PagedResult<Resource>> {
    const { data } = await request.get<PagedResult<Resource>>('/resources', params)
    return data
  }

  // 获取资源详情
  static async getResource(id: string): Promise<ResourceDetailDto> {
    const { data } = await request.get<ResourceDetailDto>(`/resources/${id}`)
    return data
  }

  // 创建资源
  static async createResource(request: CreateResourceRequest): Promise<string> {
    const { data } = await request.post<string>('/resources', request)
    return data
  }

  // 更新资源
  static async updateResource(id: string, request: UpdateResourceRequest): Promise<void> {
    await request.put(`/resources/${id}`, request)
  }

  // 删除资源
  static async deleteResource(id: string): Promise<void> {
    await request.delete(`/resources/${id}`)
  }

  // 同步API资源
  static async syncApiResources(): Promise<boolean> {
    const { data } = await request.post<boolean>('/resources/sync-api')
    return data
  }

  // 构建资源树（前端工具方法）
  static buildResourceTree(resources: Resource[]): Resource[] {
    const resourceMap = new Map<string, Resource>()
    const rootResources: Resource[] = []

    // 创建映射
    resources.forEach(resource => {
      resourceMap.set(resource.id, { ...resource, children: [] })
    })

    // 构建树结构
    resources.forEach(resource => {
      const resourceNode = resourceMap.get(resource.id)!
      
      if (resource.parentId) {
        const parent = resourceMap.get(resource.parentId)
        if (parent) {
          if (!parent.children) parent.children = []
          parent.children.push(resourceNode)
        }
      } else {
        rootResources.push(resourceNode)
      }
    })

    return rootResources
  }

  // 获取资源路径
  static getResourcePath(resources: Resource[], targetId: string): string[] {
    const path: string[] = []
    
    function findPath(nodes: Resource[], target: string): boolean {
      for (const node of nodes) {
        path.push(node.name)
        
        if (node.id === target) {
          return true
        }
        
        if (node.children && findPath(node.children, target)) {
          return true
        }
        
        path.pop()
      }
      return false
    }
    
    findPath(resources, targetId)
    return path
  }
}