// =============================================
// 表格通用组合式函数
// =============================================
import { ref, reactive, computed } from 'vue'

export interface TableColumn {
  prop: string
  label: string
  width?: string | number
  minWidth?: string | number
  fixed?: boolean | 'left' | 'right'
  sortable?: boolean
  formatter?: (row: any, column: any, cellValue: any) => string
  render?: (row: any) => any
}

export interface TablePagination {
  current: number
  pageSize: number
  total: number
  showSizeChanger?: boolean
  showQuickJumper?: boolean
  pageSizes?: number[]
}

export function useTable<T = any>() {
  const loading = ref(false)
  const data = ref<T[]>([])
  const selectedRows = ref<T[]>([])
  const selectedRowKeys = ref<string[]>([])

  // 分页配置
  const pagination = reactive<TablePagination>({
    current: 1,
    pageSize: 20,
    total: 0,
    showSizeChanger: true,
    showQuickJumper: true,
    pageSizes: [10, 20, 50, 100]
  })

  // 排序配置
  const sortConfig = reactive({
    prop: '',
    order: '' // ascending, descending
  })

  // 筛选配置
  const filterConfig = reactive<Record<string, any>>({})

  // 计算属性
  const hasSelection = computed(() => selectedRows.value.length > 0)
  const isAllSelected = computed(() => 
    data.value.length > 0 && selectedRows.value.length === data.value.length
  )
  const isIndeterminate = computed(() => 
    selectedRows.value.length > 0 && selectedRows.value.length < data.value.length
  )

  // 设置表格数据
  const setData = (newData: T[]) => {
    data.value = newData
  }

  // 设置加载状态
  const setLoading = (state: boolean) => {
    loading.value = state
  }

  // 设置分页信息
  const setPagination = (paginationData: Partial<TablePagination>) => {
    Object.assign(pagination, paginationData)
  }

  // 处理选择变化
  const handleSelectionChange = (selection: T[]) => {
    selectedRows.value = selection
    selectedRowKeys.value = selection.map((row: any) => row.id || row.key)
  }

  // 处理单行选择
  const handleRowSelect = (selection: T[], row: T) => {
    selectedRows.value = selection
    selectedRowKeys.value = selection.map((item: any) => item.id || item.key)
  }

  // 处理全选
  const handleSelectAll = (selection: T[]) => {
    selectedRows.value = selection
    selectedRowKeys.value = selection.map((item: any) => item.id || item.key)
  }

  // 清空选择
  const clearSelection = () => {
    selectedRows.value = []
    selectedRowKeys.value = []
  }

  // 处理排序变化
  const handleSortChange = ({ prop, order }: { prop: string; order: string }) => {
    sortConfig.prop = prop
    sortConfig.order = order
  }

  // 处理筛选变化
  const handleFilterChange = (filters: Record<string, any>) => {
    Object.assign(filterConfig, filters)
  }

  // 分页变化
  const handlePageChange = (page: number) => {
    pagination.current = page
  }

  // 页大小变化
  const handlePageSizeChange = (pageSize: number) => {
    pagination.pageSize = pageSize
    pagination.current = 1
  }

  // 刷新表格
  const refresh = () => {
    // 这个方法需要在使用时重写
    console.log('请在使用时重写 refresh 方法')
  }

  // 重置表格状态
  const reset = () => {
    pagination.current = 1
    sortConfig.prop = ''
    sortConfig.order = ''
    Object.keys(filterConfig).forEach(key => {
      delete filterConfig[key]
    })
    clearSelection()
  }

  // 获取查询参数
  const getQueryParams = () => {
    return {
      page: pagination.current,
      pageSize: pagination.pageSize,
      sortBy: sortConfig.prop,
      sortOrder: sortConfig.order,
      ...filterConfig
    }
  }

  return {
    // 状态
    loading,
    data,
    selectedRows,
    selectedRowKeys,
    pagination,
    sortConfig,
    filterConfig,

    // 计算属性
    hasSelection,
    isAllSelected,
    isIndeterminate,

    // 方法
    setData,
    setLoading,
    setPagination,
    handleSelectionChange,
    handleRowSelect,
    handleSelectAll,
    clearSelection,
    handleSortChange,
    handleFilterChange,
    handlePageChange,
    handlePageSizeChange,
    refresh,
    reset,
    getQueryParams
  }
}