// =============================================
// 元数据管理组合式函数
// =============================================
import { ref, reactive, computed } from 'vue'
import { MetadataService } from '@/services/metadata'
import { ElMessage } from 'element-plus'

export function useMetadata() {
  const loading = ref(false)
  const metadataList = ref<MetadataDto[]>([])
  const metadataDefinitions = ref<MetadataDefinition[]>([])
  const entityMetadata = ref<Record<string, any>>({})

  // 查询参数
  const queryParams = reactive<MetadataQueryRequest>({
    entityType: '',
    entityId: '',
    metaKey: '',
    isActive: undefined,
    pageIndex: 1,
    pageSize: 20
  })

  // 获取元数据定义列表
  const fetchMetadataDefinitions = async (entityType?: string) => {
    try {
      loading.value = true
      metadataDefinitions.value = await MetadataService.getMetadataDefinitions(entityType)
    } catch (error) {
      ElMessage.error('获取元数据定义失败')
      console.error(error)
    } finally {
      loading.value = false
    }
  }

  // 获取实体的所有元数据
  const fetchEntityMetadata = async (entityType: string, entityId: string) => {
    try {
      loading.value = true
      entityMetadata.value = await MetadataService.getEntityMetadata(entityType, entityId)
      return entityMetadata.value
    } catch (error) {
      ElMessage.error('获取实体元数据失败')
      console.error(error)
      return {}
    } finally {
      loading.value = false
    }
  }

  // 获取单个元数据值
  const getMetadata = async <T = any>(entityType: string, entityId: string, key: string): Promise<T | null> => {
    try {
      return await MetadataService.getMetadata<T>(entityType, entityId, key)
    } catch (error) {
      console.error('获取元数据失败:', error)
      return null
    }
  }

  // 设置元数据值
  const setMetadata = async <T = any>(entityType: string, entityId: string, key: string, value: T): Promise<boolean> => {
    try {
      loading.value = true
      await MetadataService.setMetadata(entityType, entityId, key, value)
      ElMessage.success('元数据设置成功')
      // 刷新实体元数据
      await fetchEntityMetadata(entityType, entityId)
      return true
    } catch (error) {
      ElMessage.error('元数据设置失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 批量设置元数据
  const setEntityMetadata = async (entityType: string, entityId: string, metadata: Record<string, any>): Promise<boolean> => {
    try {
      loading.value = true
      await MetadataService.setEntityMetadata(entityType, entityId, metadata)
      ElMessage.success('元数据批量设置成功')
      // 刷新实体元数据
      await fetchEntityMetadata(entityType, entityId)
      return true
    } catch (error) {
      ElMessage.error('元数据批量设置失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 删除元数据
  const deleteMetadata = async (entityType: string, entityId: string, key: string): Promise<boolean> => {
    try {
      loading.value = true
      const success = await MetadataService.deleteMetadata(entityType, entityId, key)
      if (success) {
        ElMessage.success('元数据删除成功')
        // 刷新实体元数据
        await fetchEntityMetadata(entityType, entityId)
      }
      return success
    } catch (error) {
      ElMessage.error('元数据删除失败')
      console.error(error)
      return false
    } finally {
      loading.value = false
    }
  }

  // 搜索元数据
  const searchMetadata = async () => {
    try {
      loading.value = true
      metadataList.value = await MetadataService.searchMetadata(queryParams)
    } catch (error) {
      ElMessage.error('搜索元数据失败')
      console.error(error)
    } finally {
      loading.value = false
    }
  }

  // 验证元数据值
  const validateMetadataValue = (definition: MetadataDefinition, value: any) => {
    return MetadataService.validateMetadataValue(definition, value)
  }

  // 格式化元数据值用于显示
  const formatMetadataValue = (definition: MetadataDefinition, value: any): string => {
    return MetadataService.formatMetadataValue(definition, value)
  }

  // 获取元数据的默认值
  const getDefaultValue = (definition: MetadataDefinition): any => {
    return MetadataService.getDefaultValue(definition)
  }

  // 转换元数据值类型
  const convertMetadataValue = (definition: MetadataDefinition, value: any): any => {
    return MetadataService.convertMetadataValue(definition, value)
  }

  // 根据实体类型获取对应的元数据定义
  const getDefinitionsByEntityType = computed(() => {
    return (entityType: string) => {
      return metadataDefinitions.value.filter(def => def.entityType === entityType)
    }
  })

  // 根据key获取元数据定义
  const getDefinitionByKey = computed(() => {
    return (entityType: string, key: string) => {
      return metadataDefinitions.value.find(def => def.entityType === entityType && def.metaKey === key)
    }
  })

  // 构建表单字段
  const buildFormFields = (entityType: string) => {
    const definitions = getDefinitionsByEntityType.value(entityType)
    return definitions.map(def => ({
      key: def.metaKey,
      label: def.displayName,
      type: def.uiComponent || getDefaultUIComponent(def.valueType),
      required: def.required,
      defaultValue: getDefaultValue(def),
      options: def.options,
      placeholder: def.example || `请输入${def.displayName}`,
      rules: def.validationRule ? [{ pattern: new RegExp(def.validationRule), message: `${def.displayName}格式不正确` }] : []
    }))
  }

  // 获取默认UI组件
  const getDefaultUIComponent = (valueType: string): string => {
    switch (valueType) {
      case 'String':
        return 'input'
      case 'Number':
        return 'input-number'
      case 'Boolean':
        return 'switch'
      case 'Date':
        return 'date-picker'
      case 'JSON':
        return 'textarea'
      default:
        return 'input'
    }
  }

  return {
    // 状态
    loading,
    metadataList,
    metadataDefinitions,
    entityMetadata,
    queryParams,

    // 计算属性
    getDefinitionsByEntityType,
    getDefinitionByKey,

    // 方法
    fetchMetadataDefinitions,
    fetchEntityMetadata,
    getMetadata,
    setMetadata,
    setEntityMetadata,
    deleteMetadata,
    searchMetadata,

    // 工具方法
    validateMetadataValue,
    formatMetadataValue,
    getDefaultValue,
    convertMetadataValue,
    buildFormFields,
    getDefaultUIComponent
  }
}