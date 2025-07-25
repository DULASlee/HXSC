// =============================================
// 角色指令
// =============================================
import type { Directive, DirectiveBinding } from 'vue'
import { usePermissionStore } from '@/stores/permission'

// 角色指令
export const vRole: Directive = {
  mounted(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    checkRole(el, binding)
  },
  updated(el: HTMLElement, binding: DirectiveBinding<string | string[]>) {
    checkRole(el, binding)
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
    checkRole(el, binding)
  },
  updated(el: HTMLElement, binding: DirectiveBinding) {
    checkRole(el, binding)
  }
}

// 角色指令的使用示例：
/*
<!-- 单个角色 -->
<el-button v-role="'admin'">管理员功能</el-button>

<!-- 多个角色（OR逻辑） -->
<el-button v-role="['admin', 'manager']">管理功能</el-button>
*/