<template>
  <el-dialog
    v-model="dialogVisible"
    :title="title"
    :width="width"
    :fullscreen="fullscreen"
    :top="top"
    :modal="modal"
    :append-to-body="appendToBody"
    :destroy-on-close="destroyOnClose"
    :close-on-click-modal="closeOnClickModal"
    :close-on-press-escape="closeOnPressEscape"
    :show-close="showClose"
    :before-close="handleBeforeClose"
    @open="handleOpen"
    @opened="handleOpened"
    @close="handleClose"
    @closed="handleClosed"
  >
    <el-form
      ref="formRef"
      :model="formData"
      :rules="rules"
      :label-width="labelWidth"
      :label-position="labelPosition"
      :validate-on-rule-change="validateOnRuleChange"
      :size="size"
      :disabled="disabled || loading"
      :status-icon="statusIcon"
      :hide-required-asterisk="hideRequiredAsterisk"
      :scroll-to-error="scrollToError"
    >
      <slot :form="formData"></slot>
    </el-form>
    
    <template #footer>
      <slot name="footer" :loading="loading" :submit="handleSubmit" :cancel="handleCancel">
        <div class="dialog-footer">
          <el-button @click="handleCancel">取消</el-button>
          <el-button type="primary" @click="handleSubmit" :loading="loading">确定</el-button>
        </div>
      </slot>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, watch, nextTick } from 'vue'
import type { FormInstance, FormRules } from 'element-plus'

const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  title: {
    type: String,
    default: '表单'
  },
  width: {
    type: String,
    default: '50%'
  },
  fullscreen: {
    type: Boolean,
    default: false
  },
  top: {
    type: String,
    default: '15vh'
  },
  modal: {
    type: Boolean,
    default: true
  },
  appendToBody: {
    type: Boolean,
    default: false
  },
  destroyOnClose: {
    type: Boolean,
    default: false
  },
  closeOnClickModal: {
    type: Boolean,
    default: false
  },
  closeOnPressEscape: {
    type: Boolean,
    default: true
  },
  showClose: {
    type: Boolean,
    default: true
  },
  labelWidth: {
    type: String,
    default: '100px'
  },
  labelPosition: {
    type: String,
    default: 'right'
  },
  validateOnRuleChange: {
    type: Boolean,
    default: true
  },
  size: {
    type: String,
    default: 'default'
  },
  disabled: {
    type: Boolean,
    default: false
  },
  loading: {
    type: Boolean,
    default: false
  },
  statusIcon: {
    type: Boolean,
    default: false
  },
  hideRequiredAsterisk: {
    type: Boolean,
    default: false
  },
  scrollToError: {
    type: Boolean,
    default: true
  },
  model: {
    type: Object,
    default: () => ({})
  },
  rules: {
    type: Object as () => FormRules,
    default: () => ({})
  }
})

const emit = defineEmits([
  'update:visible',
  'open',
  'opened',
  'close',
  'closed',
  'submit',
  'cancel'
])

const dialogVisible = ref(props.visible)
const formRef = ref<FormInstance>()
const formData = reactive({ ...props.model })

// 监听visible变化
watch(
  () => props.visible,
  (newVisible) => {
    dialogVisible.value = newVisible
  }
)

// 监听dialogVisible变化
watch(
  () => dialogVisible.value,
  (newVisible) => {
    emit('update:visible', newVisible)
  }
)

// 监听model变化
watch(
  () => props.model,
  (newModel) => {
    Object.keys(formData).forEach(key => {
      delete formData[key]
    })
    Object.assign(formData, newModel)
  },
  { deep: true }
)

// 对话框打开前
const handleOpen = () => {
  emit('open')
}

// 对话框打开后
const handleOpened = () => {
  emit('opened')
}

// 对话框关闭前
const handleBeforeClose = (done: () => void) => {
  done()
}

// 对话框关闭时
const handleClose = () => {
  emit('close')
}

// 对话框关闭后
const handleClosed = () => {
  emit('closed')
}

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return
  
  try {
    await formRef.value.validate()
    emit('submit', { ...formData })
  } catch (error) {
    console.error('表单验证失败:', error)
  }
}

// 取消
const handleCancel = () => {
  dialogVisible.value = false
  emit('cancel')
}

// 重置表单
const resetForm = () => {
  if (formRef.value) {
    formRef.value.resetFields()
  }
}

// 暴露方法
defineExpose({
  formRef,
  formData,
  resetForm,
  submit: handleSubmit,
  cancel: handleCancel
})
</script>

<style lang="scss" scoped>
.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
}
</style>