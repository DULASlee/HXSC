// =============================================
// API服务导出
// =============================================
import { TenantService } from './tenant.service'
import { CompanyService } from './company.service'
import { ProjectService } from './project.service'
import { TeamService } from './team.service'
import { WorkerService } from './worker.service'
import { AttendanceService } from './attendance.service'
import { DeviceService } from './device.service'
import { SafetyService } from './safety.service'
import { IntegrationService } from './integration.service'
import { DashboardService } from './dashboard.service'
import { DigitalTwinService } from './digital-twin.service'

// 创建服务实例
export const tenantService = new TenantService()
export const companyService = new CompanyService()
export const projectService = new ProjectService()
export const teamService = new TeamService()
export const workerService = new WorkerService()
export const attendanceService = new AttendanceService()
export const deviceService = new DeviceService()
export const safetyService = new SafetyService()
export const safetyIncidentService = safetyService // Alias for backward compatibility
export const integrationService = new IntegrationService()
export const dashboardService = new DashboardService()
export const digitalTwinService = new DigitalTwinService()

// 导出服务集合
export default {
  tenantService,
  companyService,
  projectService,
  teamService,
  workerService,
  attendanceService,
  deviceService,
  safetyService,
  integrationService,
  dashboardService,
  digitalTwinService
}