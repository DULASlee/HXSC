// =============================================
// 资源管理组合式函数
// =============================================
import { ref, reactive } from 'vue'
import { ResourceService } from '@/services/resource'
import { ElMessage } from 'element-plus'

export function useResource() {
  const loading = ref(false)
  const resources = ref<Resource[]>([])
  const resourceTree = ref<ResourceTreeDto[]>([])
  const currentResource = ref<ResourceDetailDto | null>(null)

  // 查询参数
  const queryParams = reactive<ResourceQueryRequest>({
    type: '',
    keyword: '',
    pageIndex: 1,
    pageSize: 20
  })

  // 分页信息
  const pagination = reactive({
    total: 0,
    current: 1,
    pageSize: 20
  })

  // 获取资源树
  const fetchResourceTree = async () => {
    try {
      loading.value = true
      resourceTree.value = await ResourceService.getResourceTree()
    } catch (error) {
      ElMessage.error('获取资源树失败')
      console.error(error)
    } finally {
      loading.value = false
    }
  }

  // 获取资源列表
  const fetchResources = async () => {
    try {
      loading.value = true
      const result = await ResourceService.getResources(queryParams)
      resources.value = result.items
      pagination.total = result.total
      pagination.current = result.page
    } catch (error) {
      ElMessage.error('获取资源列表失败')
      console.error(error)
    } finally {
      loading.value = false
    }
  }

  // 获取资源详情
  const fetchResourceDetail = async (id: string) => {
    try {
      loading.value = true
      currentResource.value = await ResourceService.getResource(id)
      return currentResource.value
    } catch (error) {
      ElMessage.error('获取资源详情失败')
      console.error(error)
      return null
    } finally {
      loading.value = false
    }
  }

  // 创建资源
  const createResource = async (data: CreateResourceRequest) => {
    try {
      loading.value = true
      const id = await ResourceService.createResource(data)
      ElMessage.success('资源创建成功')
      await fetchResources()
      await fetchResourceTree()
      return id
    } catch (error) {
      ElMessage.error('资源创建失败')
      console.error(error)
      return null
    } finally {
      loading.value = false
    }
  }

  // 更新资源
  const updateResource = async (id: string, data: UpdateResourceRequest) => {
    try {
      loading.value = true
      await ResourceService.updateResource(id, data)
      ElMessage.success('资源更新成功')
      await fetchResources()
      await fetchResourceTree()
      return true
    } catch (error) {
      ElMessage.error('资源更新失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 删除资源
  const deleteResource = async (id: string) => {
    try {
      loading.value = true
      await ResourceService.deleteResource(id)
      ElMessage.success('资源删除成功')
      await fetchResources()
      await fetchResourceTree()
      return true
    } catch (error) {
      ElMessage.error('资源删除失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 同步API资源
  const syncApiResources = async () => {
    try {
      loading.value = true
      const success = await ResourceService.syncApiResources()
      if (success) {
        ElMessage.success('API资源同步成功')
        await fetchResources()
        await fetchResourceTree()
      }
      return success
    } catch (error) {
      ElMessage.error('API资源同步失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 搜索资源
  const searchResources = async (keyword: string) => {
    queryParams.keyword = keyword
    queryParams.pageIndex = 1
    await fetchResources()
  }

  // 筛选资源类型
  const filterByType = async (type: string) => {
    queryParams.type = type
    queryParams.pageIndex = 1
    await fetchResources()
  }

  // 重置查询条件
  const resetQuery = () => {
    queryParams.type = ''
    queryParams.keyword = ''
    queryParams.pageIndex = 1
  }

  // 分页变化
  const handlePageChange = async (page: number) => {
    queryParams.pageIndex = page
    await fetchResources()
  }

  // 页大小变化
  const handlePageSizeChange = async (pageSize: number) => {
    queryParams.pageSize = pageSize
    queryParams.pageIndex = 1
    await fetchResources()
  }

  return {
    // 状态
    loading,
    resources,
    resourceTree,
    currentResource,
    queryParams,
    pagination,

    // 方法
    fetchResourceTree,
    fetchResources,
    fetchResourceDetail,
    createResource,
    updateResource,
    deleteResource,
    syncApiResources,
    searchResources,
    filterByType,
    resetQuery,
    handlePageChange,
    handlePageSizeChange
  }
}