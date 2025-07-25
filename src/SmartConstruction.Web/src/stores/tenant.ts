// =============================================
// 租户状态管理
// =============================================
import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Tenant } from '@/types/global'

export const useTenantStore = defineStore('tenant', () => {
  // 状态
  const tenants = ref<Tenant[]>([])
  const currentTenant = ref<Tenant | null>(null)
  const loading = ref(false)

  // 计算属性
  const tenantCount = computed(() => tenants.value.length)
  const activeTenants = computed(() => tenants.value.filter(t => t.status === 1))
  const inactiveTenants = computed(() => tenants.value.filter(t => t.status === 0))

  // 设置当前租户
  function setCurrentTenant(tenant: Tenant) {
    currentTenant.value = tenant
  }

  // 设置租户列表
  function setTenants(newTenants: Tenant[]) {
    tenants.value = newTenants
  }

  // 添加租户
  function addTenant(tenant: Tenant) {
    tenants.value.push(tenant)
  }

  // 更新租户
  function updateTenant(id: string, data: Partial<Tenant>) {
    const index = tenants.value.findIndex(t => t.id === id)
    if (index > -1) {
      tenants.value[index] = { ...tenants.value[index], ...data }
    }
  }

  // 删除租户
  function deleteTenant(id: string) {
    tenants.value = tenants.value.filter(t => t.id !== id)
  }

  // 清空租户数据
  function clearTenants() {
    tenants.value = []
    currentTenant.value = null
  }

  return {
    // 状态
    tenants,
    currentTenant,
    loading,
    
    // 计算属性
    tenantCount,
    activeTenants,
    inactiveTenants,
    
    // 操作
    setCurrentTenant,
    setTenants,
    addTenant,
    updateTenant,
    deleteTenant,
    clearTenants
  }
})