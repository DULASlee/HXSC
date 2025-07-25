// =============================================
// 租户管理组合式函数
// =============================================
import { ref, reactive, computed } from 'vue'
import { TenantService } from '@/services/tenant'
import { ElMessage, ElMessageBox } from 'element-plus'

export function useTenant() {
  const loading = ref(false)
  const tenants = ref<Tenant[]>([])
  const currentTenant = ref<TenantDetailDto | null>(null)
  const tenantIsolation = ref<TenantIsolationDto | null>(null)

  // 查询参数
  const queryParams = reactive<TenantQueryRequest>({
    keyword: '',
    status: undefined,
    pageIndex: 1,
    pageSize: 20
  })

  // 分页信息
  const pagination = reactive({
    total: 0,
    current: 1,
    pageSize: 20
  })

  // 计算属性
  const activeTenants = computed(() => tenants.value.filter(t => t.status === 1))
  const inactiveTenants = computed(() => tenants.value.filter(t => t.status === 0))

  // 获取租户列表
  const fetchTenants = async () => {
    try {
      loading.value = true
      const result = await TenantService.getTenants(queryParams)
      tenants.value = result.items
      pagination.total = result.total
      pagination.current = result.page
    } catch (error) {
      ElMessage.error('获取租户列表失败')
      console.error(error)
    } finally {
      loading.value = false
    }
  }

  // 获取租户详情
  const fetchTenantDetail = async (id: string) => {
    try {
      loading.value = true
      currentTenant.value = await TenantService.getTenant(id)
      return currentTenant.value
    } catch (error) {
      ElMessage.error('获取租户详情失败')
      console.error(error)
      return null
    } finally {
      loading.value = false
    }
  }

  // 获取租户隔离配置
  const fetchTenantIsolation = async (id: string) => {
    try {
      loading.value = true
      tenantIsolation.value = await TenantService.getTenantIsolation(id)
      return tenantIsolation.value
    } catch (error) {
      ElMessage.error('获取租户隔离配置失败')
      console.error(error)
      return null
    } finally {
      loading.value = false
    }
  }

  // 创建租户
  const createTenant = async (data: CreateTenantRequest) => {
    try {
      loading.value = true
      const id = await TenantService.createTenant(data)
      await fetchTenants()
      return id
    } catch (error) {
      ElMessage.error('租户创建失败')
      console.error(error)
      return null
    } finally {
      loading.value = false
    }
  }

  // 更新租户
  const updateTenant = async (id: string, data: UpdateTenantRequest) => {
    try {
      loading.value = true
      await TenantService.updateTenant(id, data)
      await fetchTenants()
      if (currentTenant.value?.id === id) {
        await fetchTenantDetail(id)
      }
      return true
    } catch (error) {
      ElMessage.error('租户更新失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 切换租户状态
  const toggleTenantStatus = async (id: string, currentStatus: number) => {
    try {
      const action = currentStatus === 1 ? '禁用' : '启用'
      await ElMessageBox.confirm(
        `确定要${action}该租户吗？`,
        '确认操作',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )

      loading.value = true
      await TenantService.toggleTenantStatus(id)
      await fetchTenants()
      return true
    } catch (error) {
      if (error !== 'cancel') {
        ElMessage.error('操作失败')
        console.error(error)
      }
      return false
    } finally {
      loading.value = false
    }
  }

  // 升级租户隔离级别
  const upgradeTenantIsolation = async (id: string, targetLevel: 'Shared' | 'Schema' | 'Database') => {
    try {
      await ElMessageBox.confirm(
        `确定要将租户隔离级别升级到 ${targetLevel} 吗？此操作可能需要一些时间。`,
        '确认升级',
        {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }
      )

      loading.value = true
      const success = await TenantService.upgradeTenantIsolation(id, targetLevel)
      if (success && tenantIsolation.value) {
        await fetchTenantIsolation(id)
      }
      return success
    } catch (error) {
      if (error !== 'cancel') {
        ElMessage.error('升级失败')
        console.error(error)
      }
      return false
    } finally {
      loading.value = false
    }
  }

  // 检查租户代码是否可用
  const checkTenantCodeAvailable = async (code: string, excludeId?: string): Promise<boolean> => {
    try {
      return await TenantService.checkTenantCodeAvailable(code, excludeId)
    } catch (error) {
      console.error(error)
      return false
    }
  }

  // 搜索租户
  const searchTenants = async (keyword: string) => {
    queryParams.keyword = keyword
    queryParams.pageIndex = 1
    await fetchTenants()
  }

  // 筛选租户状态
  const filterByStatus = async (status?: number) => {
    queryParams.status = status
    queryParams.pageIndex = 1
    await fetchTenants()
  }

  // 重置查询条件
  const resetQuery = () => {
    queryParams.keyword = ''
    queryParams.status = undefined
    queryParams.pageIndex = 1
  }

  // 分页变化
  const handlePageChange = async (page: number) => {
    queryParams.pageIndex = page
    await fetchTenants()
  }

  // 页大小变化
  const handlePageSizeChange = async (pageSize: number) => {
    queryParams.pageSize = pageSize
    queryParams.pageIndex = 1
    await fetchTenants()
  }

  // 获取隔离级别描述
  const getIsolationLevelDescription = (level: string): string => {
    return TenantService.getIsolationLevelDescription(level)
  }

  // 获取升级建议
  const getUpgradeRecommendation = (isolation: TenantIsolationDto): string | null => {
    return TenantService.getUpgradeRecommendation(isolation)
  }

  // 格式化存储大小
  const formatStorageSize = (sizeInMB: number): string => {
    return TenantService.formatStorageSize(sizeInMB)
  }

  return {
    // 状态
    loading,
    tenants,
    currentTenant,
    tenantIsolation,
    queryParams,
    pagination,

    // 计算属性
    activeTenants,
    inactiveTenants,

    // 方法
    fetchTenants,
    fetchTenantDetail,
    fetchTenantIsolation,
    createTenant,
    updateTenant,
    toggleTenantStatus,
    upgradeTenantIsolation,
    checkTenantCodeAvailable,
    searchTenants,
    filterByStatus,
    resetQuery,
    handlePageChange,
    handlePageSizeChange,

    // 工具方法
    getIsolationLevelDescription,
    getUpgradeRecommendation,
    formatStorageSize
  }
}