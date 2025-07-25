// =============================================
// Vue指令统一导出
// =============================================
import type { App } from 'vue'
import { vPermission } from './permission'
import { vRole } from './role'
import permissionDirective from './permission'
import roleDirective from './role'

// 所有指令
const directives = {
  permission: vPermission,
  role: vRole
}

// 兼容旧版本的指令
const legacyDirectives = {
  permission: permissionDirective,
  role: roleDirective
}

// 安装所有指令
export function setupDirectives(app: App) {
  Object.keys(directives).forEach(key => {
    app.directive(key, directives[key as keyof typeof directives])
  })
}

// 安装兼容版本的指令
export function setupLegacyDirectives(app: App) {
  Object.keys(legacyDirectives).forEach(key => {
    app.directive(key, legacyDirectives[key as keyof typeof legacyDirectives])
  })
}

// 单独导出指令
export { vPermission, vRole }
export type { PermissionBinding } from './permission'

// 兼容旧版本的默认导出
export default {
  permission: permissionDirective,
  role: roleDirective
}