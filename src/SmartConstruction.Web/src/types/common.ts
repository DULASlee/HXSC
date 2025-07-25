// 通用类型定义

// 分页请求参数
export interface PagedRequest {
  pageIndex: number
  pageSize: number
}

// 分页结果
export interface PagedResult<T> {
  items: T[]
  total: number
  pageIndex: number
  pageSize: number
  totalPages: number
}

// 基础查询参数
export interface BaseQueryParams extends PagedRequest {
  keyword?: string
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
}

// API响应基础结构
export interface ApiResponse<T = any> {
  success: boolean
  message: string
  data: T
  code?: number
  timestamp?: string
}

// 选项接口
export interface SelectOption {
  label: string
  value: string | number
  disabled?: boolean
}

// 文件上传相关
export interface UploadFile {
  id: string
  name: string
  url: string
  size: number
  type: string
  uploadTime: string
}

// 操作日志
export interface OperationLog {
  id: string
  operation: string
  operator: string
  operatorId: string
  target: string
  targetId: string
  details: string
  timestamp: string
  ip: string
  userAgent: string
} 