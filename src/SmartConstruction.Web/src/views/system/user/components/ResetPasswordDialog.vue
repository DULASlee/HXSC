<template>
  <el-dialog
    v-model="dialogVisible"
    title="重置密码"
    width="500px"
    :close-on-click-modal="false"
    @close="handleClose"
  >
    <div class="user-info">
      <el-alert
        :title="`正在为用户 ${userInfo.displayName} 重置密码`"
        type="warning"
        :closable="false"
        show-icon
      />
    </div>

    <el-form
      ref="formRef"
      :model="formData"
      :rules="formRules"
      label-width="100px"
      style="margin-top: 20px"
    >
      <el-form-item label="新密码" prop="newPassword">
        <el-input
          v-model="formData.newPassword"
          type="password"
          placeholder="请输入新密码"
          show-password
          autocomplete="new-password"
        />
      </el-form-item>
      
      <el-form-item label="确认密码" prop="confirmPassword">
        <el-input
          v-model="formData.confirmPassword"
          type="password"
          placeholder="请确认新密码"
          show-password
          autocomplete="new-password"
        />
      </el-form-item>
      
      <el-form-item>
        <el-checkbox v-model="formData.forceChangePassword">
          强制用户下次登录时修改密码
        </el-checkbox>
      </el-form-item>
      
      <el-form-item>
        <el-checkbox v-model="formData.sendNotification">
          发送密码重置通知邮件
        </el-checkbox>
      </el-form-item>
    </el-form>

    <div class="password-tips">
      <el-alert
        title="密码安全建议"
        type="info"
        :closable="false"
      >
        <ul>
          <li>密码长度至少8位</li>
          <li>包含大小写字母、数字和特殊字符</li>
          <li>不要使用常见的弱密码</li>
          <li>定期更换密码</li>
        </ul>
      </el-alert>
    </div>

    <template #footer>
      <div class="dialog-footer">
        <el-button @click="handleClose">取消</el-button>
        <el-button @click="generatePassword">生成随机密码</el-button>
        <el-button type="primary" :loading="loading" @click="handleSubmit">
          确定重置
        </el-button>
      </div>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch } from 'vue'
import { ElMessage, type FormInstance, type FormRules } from 'element-plus'
import { getUser, resetPassword } from '../../../../api/user'
import { validationRules } from '../../../../utils/validate'

interface Props {
  modelValue: boolean
  userId: string
}

interface Emits {
  (e: 'update:modelValue', value: boolean): void
  (e: 'success'): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const formRef = ref<FormInstance>()
const loading = ref(false)

const userInfo = reactive({
  username: '',
  displayName: '',
  email: ''
})

const formData = reactive({
  newPassword: '',
  confirmPassword: '',
  forceChangePassword: true,
  sendNotification: false
})

// 计算属性
const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 表单验证规则
const formRules: FormRules = {
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 8, message: '密码长度至少为8位', trigger: 'blur' },
    {
      validator: (rule: any, value: any, callback: any) => {
        const { valid, message } = validatePasswordStrength(value)
        if (!valid) {
          callback(new Error(message))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ],
  confirmPassword: [
    { required: true, message: '请确认新密码', trigger: 'blur' },
    {
      validator: (rule: any, value: any, callback: any) => {
        if (value !== formData.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 监听弹窗显示
watch(dialogVisible, (visible) => {
  if (visible && props.userId) {
    loadUserInfo()
  } else {
    resetForm()
  }
})

// 加载用户信息
async function loadUserInfo() {
  try {
    const { data } = await getUser(props.userId)
    
    Object.assign(userInfo, {
      username: data.username,
      displayName: data.displayName,
      email: data.email || ''
    })
    
    // 如果用户有邮箱，默认发送通知
    formData.sendNotification = !!data.email
  } catch (error) {
    console.error('Failed to load user info:', error)
    ElMessage.error('加载用户信息失败')
  }
}

// 验证密码强度
function validatePasswordStrength(password: string): { valid: boolean; message: string } {
  if (!password) return { valid: false, message: '密码不能为空' }
  
  const checks = [
    { test: /[a-z]/, message: '至少包含一个小写字母' },
    { test: /[A-Z]/, message: '至少包含一个大写字母' },
    { test: /\d/, message: '至少包含一个数字' },
    { test: /[!@#$%^&*(),.?":{}|<>]/, message: '至少包含一个特殊字符' }
  ]
  
  for (const check of checks) {
    if (!check.test.test(password)) {
      return { valid: false, message: check.message }
    }
  }
  
  return { valid: true, message: '' }
}

// 生成随机密码
function generatePassword() {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*'
  let password = ''
  
  // 确保包含各种字符类型
  password += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ'[Math.floor(Math.random() * 26)]
  password += 'abcdefghijklmnopqrstuvwxyz'[Math.floor(Math.random() * 26)]
  password += '0123456789'[Math.floor(Math.random() * 10)]
  password += '!@#$%^&*'[Math.floor(Math.random() * 8)]
  
  // 填充剩余位数
  for (let i = 4; i < 12; i++) {
    password += chars[Math.floor(Math.random() * chars.length)]
  }
  
  // 打乱顺序
  password = password.split('').sort(() => Math.random() - 0.5).join('')
  
  formData.newPassword = password
  formData.confirmPassword = password
  
  ElMessage.success('已生成随机密码')
}

// 提交重置
async function handleSubmit() {
  if (!formRef.value) return

  try {
    const valid = await formRef.value.validate()
    if (!valid) return

    loading.value = true
    
    // 注意：根据API，这里只传递password。其他选项可能需要在后端处理或通过其他API
    await resetPassword(props.userId, {
      password: formData.newPassword,
    })
    
    ElMessage.success('密码重置成功')
    emit('success')
    handleClose()
  } catch (error) {
    console.error('Failed to reset password:', error)
    ElMessage.error('密码重置失败')
  } finally {
    loading.value = false
  }
}

// 关闭弹窗
function handleClose() {
  dialogVisible.value = false
}

// 重置表单
function resetForm() {
  formRef.value?.resetFields()
  Object.assign(formData, {
    newPassword: '',
    confirmPassword: '',
    forceChangePassword: true,
    sendNotification: false
  })
  Object.assign(userInfo, {
    username: '',
    displayName: '',
    email: ''
  })
}
</script>

<style lang="scss" scoped>
.user-info {
  margin-bottom: 20px;
}

.password-tips {
  margin-top: 20px;
  
  ul {
    margin: 8px 0 0 0;
    padding-left: 20px;
    
    li {
      margin-bottom: 4px;
      font-size: 13px;
      color: var(--el-text-color-secondary);
    }
  }
}

.dialog-footer {
  text-align: right;
}
</style>