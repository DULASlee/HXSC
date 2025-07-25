// =============================================
// API类型定义
// =============================================

// 分页请求参数
export interface PageRequest {
  pageIndex: number
  pageSize: number
}

// 查询参数基类
export interface QueryParams extends PageRequest {
  keyword?: string
  orderBy?: string
  isDescending?: boolean
}

// 租户相关
export interface TenantListRequest extends QueryParams {
  isActive?: boolean
}

export interface CreateTenantParams {
  code: string
  name: string
  description?: string
  contactPerson?: string
  contactPhone?: string
  address?: string
  expireDate?: string
}

export interface UpdateTenantParams extends CreateTenantParams {
  id: string
}

export interface TenantStatusParams {
  isActive: boolean
}

// 公司相关
export interface CompanyListRequest extends QueryParams {
  tenantId?: string
  type?: string
  isActive?: boolean
}

export interface CreateCompanyParams {
  tenantId: string
  code: string
  name: string
  unifiedSocialCreditCode?: string
  legalPerson?: string
  contactPhone?: string
  address?: string
  type: string
}

export interface UpdateCompanyParams extends CreateCompanyParams {
  id: string
}

// 项目相关
export interface ProjectListRequest extends QueryParams {
  companyId?: string
  status?: string
  startDateFrom?: string
  startDateTo?: string
}

export interface CreateProjectParams {
  companyId: string
  code: string
  name: string
  description?: string
  address?: string
  longitude?: number
  latitude?: number
  startDate: string
  endDate?: string
  totalInvestment?: number
  projectManager?: string
  managerPhone?: string
}

export interface UpdateProjectParams extends CreateProjectParams {
  id: string
}

// 班组相关
export interface TeamListRequest extends QueryParams {
  companyId?: string
  type?: string
  isActive?: boolean
}

export interface CreateTeamParams {
  companyId: string
  name: string
  leader?: string
  leaderPhone?: string
  type: string
}

export interface UpdateTeamParams extends CreateTeamParams {
  id: string
}

// 工人相关
export interface WorkerListRequest extends QueryParams {
  teamId?: string
  projectId?: string
  gender?: string
  type?: string
  isActive?: boolean
}

export interface CreateWorkerParams {
  teamId: string
  name: string
  idCardNumber: string
  phone?: string
  gender: string
  birthday?: string
  address?: string
  emergencyContact?: string
  emergencyPhone?: string
  type: string
  faceImage?: string
}

export interface UpdateWorkerParams extends CreateWorkerParams {
  id: string
}

// 考勤相关
export interface AttendanceListRequest extends QueryParams {
  workerId?: string
  projectId?: string
  dateFrom?: string
  dateTo?: string
  type?: string
}

export interface CreateAttendanceParams {
  workerId: string
  projectId: string
  checkInTime: string
  type: string
  deviceCode?: string
  faceImage?: string
  temperature?: number
}

export interface UpdateAttendanceParams extends CreateAttendanceParams {
  id: string
  checkOutTime?: string
}

// 设备相关
export interface DeviceListRequest extends QueryParams {
  projectId?: string
  type?: string
  status?: string
}

export interface CreateDeviceParams {
  projectId: string
  code: string
  name: string
  type: string
  model?: string
  manufacturer?: string
  installDate?: string
  ipAddress?: string
  port?: number
  location?: string
  parameters?: Record<string, any>
}

export interface UpdateDeviceParams extends CreateDeviceParams {
  id: string
}

export interface DeviceStatusParams {
  status: string
}

// 安全事件相关
export interface SafetyIncidentListRequest extends QueryParams {
  projectId?: string
  level?: string
  status?: string
  dateFrom?: string
  dateTo?: string
}

export interface CreateSafetyIncidentParams {
  projectId: string
  title: string
  description?: string
  level: string
  occurredTime: string
  location?: string
  reporter?: string
  attachments?: string[]
}

export interface UpdateSafetyIncidentParams extends CreateSafetyIncidentParams {
  id: string
}

export interface HandleSafetyIncidentParams {
  handledBy: string
  handlingResult: string
}

// IoT集成相关
export interface IoTDataParams {
  deviceCode: string
  deviceType: string
  timestamp: string
  data: Record<string, any>
}

export interface IoTAlertParams {
  deviceCode: string
  alertType: string
  alertLevel: string
  message: string
  timestamp: string
  data?: Record<string, any>
}

export interface DeviceStatusChangeParams {
  deviceId: string
  deviceName: string
  status: string
}

// 仪表盘相关
export interface DashboardParams {
  startDate?: string
  endDate?: string
}

export interface ProjectDashboardParams extends DashboardParams {
  projectId: string
}

export interface CompanyDashboardParams extends DashboardParams {
  companyId: string
}