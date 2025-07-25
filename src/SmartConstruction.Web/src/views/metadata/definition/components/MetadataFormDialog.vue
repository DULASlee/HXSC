<template>
  <el-dialog
    v-model="dialogVisible"
    :title="dialogTitle"
    width="800px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="120px"
      class="metadata-form"
      v-loading="loading"
    >
      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="实体类型" prop="entityType">
            <el-input v-model="formData.entityType" disabled />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="键名" prop="metaKey">
            <el-input
              v-model="formData.metaKey"
              placeholder="请输入键名（英文）"
              :disabled="mode === 'edit'"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="显示名称" prop="displayName">
            <el-input v-model="formData.displayName" placeholder="请输入显示名称" />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="值类型" prop="valueType">
            <el-select v-model="formData.valueType" placeholder="请选择值类型" @change="handleValueTypeChange">
              <el-option label="字符串" value="String" />
              <el-option label="数字" value="Number" />
              <el-option label="布尔值" value="Boolean" />
              <el-option label="日期" value="Date" />
              <el-option label="JSON" value="JSON" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="描述" prop="description">
        <el-input
          v-model="formData.description"
          type="textarea"
          :rows="3"
          placeholder="请输入描述信息"
        />
      </el-form-item>

      <el-row :gutter="20">
        <el-col :span="12">
          <el-form-item label="默认值" prop="defaultValue">
            <component
              :is="getDefaultValueComponent()"
              v-model="formData.defaultValue"
              :placeholder="getDefaultValuePlaceholder()"
              v-bind="getDefaultValueProps()"
            />
          </el-form-item>
        </el-col>
        <el-col :span="12">
          <el-form-item label="排序" prop="sortOrder">
            <el-input-number
              v-model="formData.sortOrder"
              :min="0"
              :max="9999"
              controls-position="right"
              style="width: 100%"
            />
          </el-form-item>
        </el-col>
      </el-row>

      <el-row :gutter="20">
        <el-col :span="8">
          <el-form-item label="必填" prop="required">
            <el-switch v-model="formData.required" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="启用" prop="isActive">
            <el-switch v-model="formData.isActive" />
          </el-form-item>
        </el-col>
        <el-col :span="8">
          <el-form-item label="显示选项" prop="showOptions">
            <el-switch v-model="showOptions" @change="handleShowOptionsChange" />
          </el-form-item>
        </el-col>
      </el-row>

      <el-form-item label="UI组件" prop="uiComponent">
        <el-select v-model="formData.uiComponent" placeholder="请选择UI组件" clearable>
          <el-option
            v-for="component in getUIComponents()"
            :key="component.value"
            :label="component.label"
            :value="component.value"
          />
        </el-select>
      </el-form-item>

      <el-form-item label="验证规则" prop="validationRule">
        <el-input
          v-model="formData.validationRule"
          placeholder="请输入正则表达式"
          clearable
        />
      </el-form-item>

      <el-form-item label="示例值" prop="example">
        <el-input v-model="formData.example" placeholder="请输入示例值" />
      </el-form-item>

      <!-- 选项配置 -->
      <el-form-item v-if="showOptions" label="选项配置">
        <div class="options-container">
          <div class="options-header">
            <span>选项列表</span>
            <el-button type="primary" size="small" @click="handleAddOption">
              <el-icon><Plus /></el-icon>
              添加选项
            </el-button>
          </div>
          
          <div class="options-list">
            <div
              v-for="(option, index) in formData.options"
              :key="index"
              class="option-item"
            >
              <el-input v-model="option.value" placeholder="选项值" style="width: 120px" />
              <el-input v-model="option.label" placeholder="显示标签" style="width: 150px" />
              <el-color-picker v-model="option.color" />
              <el-input v-model="option.icon" placeholder="图标" style="width: 100px" />
              <el-input-number v-model="option.sortOrder" :min="0" controls-position="right" style="width: 100px" />
              <el-button type="danger" size="small" @click="handleRemoveOption(index)">
                <el-icon><Delete /></el-icon>
              </el-button>
            </div>
          </div>
        </div>
      </el-form-item>
    </el-form>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button type="primary" :loading="loading" @click="handleSubmit">确定</el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
// 组合式API和工具函数已由unplugin-auto-import自动导入
interface Props {
  modelValue: boolean
  mode: 'create' | 'edit'
  entityType: string
  metadataId?: string
}
interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = withDefaults(defineProps<Props>(), { metadataId: '' })
const emit = defineEmits<Emits>()

const formRef = ref<FormInstance>()
const showOptions = ref(false)

const { execute: executeGet, loading: getLoading } = useApi(getMetadataDefinition, { immediate: false })
const { execute: executeCreate, loading: createLoading } = useApi(createMetadataDefinition)
const { execute: executeUpdate, loading: updateLoading } = useApi(updateMetadataDefinition)
const loading = computed(() => getLoading.value || createLoading.value || updateLoading.value)

const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})
const dialogTitle = computed(() => (props.mode === 'create' ? '新增元数据定义' : '编辑元数据定义'))

const formData = reactive<Partial<MetadataDefinition>>({
  entityType: '', metaKey: '', displayName: '', description: '',
  valueType: 'String', defaultValue: '', required: false, uiComponent: '',
  validationRule: '', options: [], sortOrder: 0, isActive: true, example: ''
})

const formRules: FormRules = {
  metaKey: [
    { required: true, message: '请输入键名', trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9_]*$/, message: '键名必须以字母开头，只能包含字母、数字和下划线', trigger: 'blur' }
  ],
  displayName: [{ required: true, message: '请输入显示名称', trigger: 'blur' }],
  valueType: [{ required: true, message: '请选择值类型', trigger: 'change' }]
}

watch(dialogVisible, (visible) => {
  if (visible) {
    if (props.mode === 'edit' && props.metadataId) {
      loadMetadataDefinition()
    } else {
      resetForm()
      formData.entityType = props.entityType
    }
  }
})

const loadMetadataDefinition = async () => {
  const data = await executeGet(props.metadataId!)
  if (data) {
    Object.assign(formData, data)
    showOptions.value = !!(data.options && data.options.length > 0)
  }
}

const resetForm = () => {
  Object.assign(formData, {
    entityType: props.entityType, metaKey: '', displayName: '', description: '',
    valueType: 'String', defaultValue: '', required: false, uiComponent: '',
    validationRule: '', options: [], sortOrder: 0, isActive: true, example: ''
  })
  showOptions.value = false
}

const handleValueTypeChange = (valueType: string) => {
  formData.defaultValue = ''
  const componentMap: Record<string, string> = { 'String': 'input', 'Number': 'input-number', 'Boolean': 'switch', 'Date': 'date-picker', 'JSON': 'textarea' }
  formData.uiComponent = componentMap[valueType] || 'input'
}

const getDefaultValueComponent = () => ({ 'Number': 'el-input-number', 'Boolean': 'el-switch', 'Date': 'el-date-picker', 'JSON': 'el-input' }[formData.valueType] || 'el-input')
const getDefaultValuePlaceholder = () => ({ 'String': '请输入默认字符串值', 'Number': '请输入默认数字值', 'JSON': '请输入默认JSON值' }[formData.valueType] || '请输入默认值')
const getDefaultValueProps = () => ({ 'Number': { controlsPosition: 'right', style: 'width: 100%' }, 'Date': { type: 'date', placeholder: '请选择默认日期', style: 'width: 100%' }, 'JSON': { type: 'textarea', rows: 3 } }[formData.valueType] || {})
const getUIComponents = () => [
    { label: '输入框', value: 'input' }, { label: '文本域', value: 'textarea' },
    { label: '数字输入框', value: 'input-number' }, { label: '开关', value: 'switch' },
    { label: '日期选择器', value: 'date-picker' }, { label: '时间选择器', value: 'time-picker' },
    { label: '日期时间选择器', value: 'datetime-picker' }, { label: '选择器', value: 'select' },
    { label: '单选框组', value: 'radio-group' }, { label: '复选框组', value: 'checkbox-group' },
    { label: '颜色选择器', value: 'color-picker' }, { label: '文件上传', value: 'upload' }
]

const handleShowOptionsChange = (show: boolean) => {
  if (!show) formData.options = []
  else if (!formData.options || formData.options.length === 0) {
    formData.options = []; handleAddOption()
  }
}
const handleAddOption = () => {
  if (!formData.options) formData.options = []
  formData.options.push({ value: '', label: '', color: '', icon: '', sortOrder: formData.options.length })
}
const handleRemoveOption = (index: number) => formData.options?.splice(index, 1)

const handleSubmit = async () => {
  if (!formRef.value) return
  const valid = await formRef.value.validate();
  if (!valid) return

  if (!showOptions.value) formData.options = []

  if (props.mode === 'create') {
    await executeCreate(formData)
    ElMessage.success('创建成功')
  } else {
    await executeUpdate(props.metadataId!, formData)
    ElMessage.success('更新成功')
  }
  emit('success')
  handleClose()
}

const handleClose = () => {
  dialogVisible.value = false
  formRef.value?.resetFields()
}
</script>

<style lang="scss" scoped>
.metadata-form {
  .options-container {
    width: 100%;
    border: 1px solid var(--el-border-color);
    border-radius: 4px;
    padding: 16px;
    
    .options-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
      
      span {
        font-weight: 500;
      }
    }
    
    .options-list {
      .option-item {
        display: flex;
        align-items: center;
        gap: 12px;
        margin-bottom: 12px;
        
        &:last-child {
          margin-bottom: 0;
        }
      }
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>