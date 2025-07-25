// =============================================
// 全局核心类型定义 - The Bible of Types
// =============================================

/**
 * API标准响应结构
 */
export interface ApiResponse<T = any> {
  success: boolean
  code: number
  message: string
  data: T | null
  timestamp: string
  traceId?: string
}

/**
 * 分页查询的标准请求参数
 */
export interface PageQueryRequest {
  page: number
  pageSize: number
  keyword?: string
  orderBy?: string
  descending?: boolean
}

/**
 * 分页查询的标准响应结构
 */
export interface PagedResult<T> {
  items: T[]
  total: number
}

// ----------------- Auth & User -----------------

/**
 * 登录请求
 */
export interface LoginRequest {
  tenantCode: string
  username: string
  password: string
  rememberMe?: boolean
  deviceId?: string
  deviceType?: string
}

/**
 * 登录响应
 */
export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

/**
 * 令牌信息
 */
export interface TokenInfo {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

/**
 * 完整的用户信息
 */
export interface User {
  id: string
  username: string
  displayName: string
  email: string
  mobile: string
  avatar: string
  status: number // 0: Disabled, 1: Enabled, 2: Locked
  tenantId: string
  organizationId: string
  organizationName: string
  organizationPath: string[]
  roles: Role[]
  createdAt: string
  lastLoginAt?: string
}

// ----------------- Tenant & Role -----------------

/**
 * 角色信息
 */
export interface Role {
  id: string
  code: string
  name: string
  description: string
  dataScope: 'All' | 'Department' | 'DepartmentOnly' | 'Self' | 'Custom'
  status: number // 0: Disabled, 1: Enabled
  isSystem: boolean
  createdAt: string
}

/**
 * 租户基础信息 (用于列表)
 */
export interface Tenant {
  id: string
  code: string
  name: string
  status: number // 0: Disabled, 1: Enabled, 2: Maintenance
  isolationMode: 'Shared' | 'Dedicated'
  logo: string
  createdAt: string
}

/**
 * 租户详细信息
 */
export interface TenantDetail extends Tenant {
  description: string
  connectionString?: string // 敏感信息，仅特定权限可查看
  features: string[]
  adminUser: {
    username: string
    email: string
  }
}

// ----------------- Menu & Permission -----------------

/**
 * 菜单信息
 */
export interface Menu {
  id: string
  parentId?: string
  code: string
  name: string
  type: 'Directory' | 'Menu' | 'Button'
  path: string
  component?: string
  icon?: string
  permission?: string
  sortOrder: number
  isEnabled: boolean
  isVisible: boolean
  isKeepAlive: boolean
  isExternal: boolean
  children?: Menu[]
  createdAt: string
}

// ----------------- Organization -----------------
/**
 * 组织机构信息
 */
export interface Organization {
  id: string
  parentId?: string
  code: string
  name: string
  type: 'Company' | 'Department' | 'Team' | 'Group'
  managerName?: string
  status: number // 0: Disabled, 1: Enabled
  sortOrder: number
  children?: Organization[]
}