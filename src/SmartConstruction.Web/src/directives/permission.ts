// =============================================
// 权限指令
// =============================================
import type { Directive, DirectiveBinding } from 'vue'
import { usePermissionStore } from '@/stores/permission'

export interface PermissionBinding {
  permission?: string | string[]
  role?: string | string[]
  mode?: 'and' | 'or' // 多个权限/角色的逻辑关系
}

// 权限指令
export const vPermission: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[] | PermissionBinding>) {
    checkPermission(el, binding)
  },
  updated(el: HTMLElement, binding: DirectiveBinding<string | string[] | PermissionBinding>) {
    checkPermission(el, binding)
  }
}

// 角色指令
export const vRole: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    checkRole(el, binding)
  },
  updated(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    checkRole(el, binding)
  }
}

// 权限检查函数
function checkPermission(el: HTMLElement, binding: DirectiveBinding<string | string[] | PermissionBinding>) {
  const permissionStore = usePermissionStore()
  let hasPermission = false

  if (typeof binding.value === 'string') {
    // 单个权限字符串
    hasPermission = permissionStore.hasPermission(binding.value)
  } else if (Array.isArray(binding.value)) {
    // 权限数组，默认为 OR 逻辑
    hasPermission = permissionStore.hasAnyPermission(binding.value)
  } else if (typeof binding.value === 'object') {
    // 复杂权限对象
    const { permission, role, mode = 'or' } = binding.value

    let permissionResult = true
    let roleResult = true

    // 检查权限
    if (permission) {
      if (typeof permission === 'string') {
        permissionResult = permissionStore.hasPermission(permission)
      } else if (Array.isArray(permission)) {
        permissionResult = mode === 'and' 
          ? permissionStore.hasAllPermissions(permission)
          : permissionStore.hasAnyPermission(permission)
      }
    }

    // 检查角色
    if (role) {
      if (typeof role === 'string') {
        roleResult = permissionStore.hasRole(role)
      } else if (Array.isArray(role)) {
        roleResult = mode === 'and'
          ? permissionStore.hasAllRoles(role)
          : permissionStore.hasAnyRole(role)
      }
    }

    // 根据模式组合结果
    hasPermission = mode === 'and' ? (permissionResult && roleResult) : (permissionResult || roleResult)
  }

  // 根据权限结果显示/隐藏元素
  if (!hasPermission) {
    el.style.display = 'none'
    el.setAttribute('disabled', 'true')
  } else {
    el.style.display = ''
    el.removeAttribute('disabled')
  }
}

// 角色检查函数
function checkRole(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
  const permissionStore = usePermissionStore()
  let hasRole = false

  if (typeof binding.value === 'string') {
    hasRole = permissionStore.hasRole(binding.value)
  } else if (Array.isArray(binding.value)) {
    hasRole = permissionStore.hasAnyRole(binding.value)
  }

  if (!hasRole) {
    el.style.display = 'none'
    el.setAttribute('disabled', 'true')
  } else {
    el.style.display = ''
    el.removeAttribute('disabled')
  }
}

// 兼容旧版本的默认导出
export default {
  mounted(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    const permissionStore = usePermissionStore()

    if (value && Array.isArray(value)) {
      if (value.length > 0) {
        const hasPermission = permissionStore.hasAnyPermission(value)
        if (!hasPermission) {
          el.style.display = 'none'
        }
      }
    } else if (value && typeof value === 'string') {
      if (!permissionStore.hasPermission(value)) {
        el.style.display = 'none'
      }
    } else {
      throw new Error(`权限指令使用错误，示例：v-permission="'user.create'" 或 v-permission="['user.create', 'user.update']"`)
    }
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    const { value } = binding
    const permissionStore = usePermissionStore()

    if (value && Array.isArray(value)) {
      if (value.length > 0) {
        const hasPermission = permissionStore.hasAnyPermission(value)
        if (!hasPermission) {
          el.style.display = 'none'
        } else {
          el.style.display = ''
        }
      }
    } else if (value && typeof value === 'string') {
      if (!permissionStore.hasPermission(value)) {
        el.style.display = 'none'
      } else {
        el.style.display = ''
      }
    }
  }
}

// 权限指令的使用示例：
/*
<!-- 单个权限 -->
<el-button v-permission="'user:create'">创建用户</el-button>

<!-- 多个权限（OR逻辑） -->
<el-button v-permission="['user:create', 'user:edit']">操作用户</el-button>

<!-- 复杂权限控制 -->
<el-button v-permission="{ permission: ['user:create', 'user:edit'], mode: 'and' }">
  需要创建和编辑权限
</el-button>

<!-- 权限和角色组合 -->
<el-button v-permission="{ permission: 'user:delete', role: 'admin', mode: 'and' }">
  删除用户（需要权限和角色）
</el-button>

<!-- 角色指令 -->
<el-button v-role="'admin'">管理员功能</el-button>
<el-button v-role="['admin', 'manager']">管理功能</el-button>
*/