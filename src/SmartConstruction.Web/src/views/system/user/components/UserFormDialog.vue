<template>
  <el-dialog
    :title="isEdit ? '编辑用户' : '新增用户'"
    v-model="dialogVisible"
    width="600px"
    :close-on-click-modal="false"
    @closed="resetForm"
  >
    <el-form
      ref="formRef"
      :model="form"
      :rules="rules"
      label-width="100px"
      label-position="right"
      v-loading="formLoading"
    >
      <el-form-item label="用户名" prop="username">
        <el-input v-model="form.username" placeholder="请输入用户名" :disabled="isEdit" />
      </el-form-item>
      
      <el-form-item label="显示名称" prop="displayName">
        <el-input v-model="form.displayName" placeholder="请输入显示名称" />
      </el-form-item>
      
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="form.email" placeholder="请输入邮箱" />
      </el-form-item>
      
      <el-form-item label="手机号" prop="mobile">
        <el-input v-model="form.mobile" placeholder="请输入手机号" />
      </el-form-item>
      
      <el-form-item label="所属组织" prop="organizationId">
        <el-select v-model="form.organizationId" placeholder="请选择所属组织" style="width: 100%">
          <el-option label="系统管理部" value="1" />
          <el-option label="业务部门" value="2" />
          <el-option label="测试部门" value="3" />
        </el-select>
      </el-form-item>
      
      <el-form-item label="角色" prop="roleIds">
        <el-select
          v-model="form.roleIds"
          multiple
          placeholder="请选择角色"
          style="width: 100%"
        >
          <el-option label="管理员" value="1" />
          <el-option label="普通用户" value="2" />
          <el-option label="审计员" value="3" />
        </el-select>
      </el-form-item>
      
      <el-form-item label="状态" prop="status">
        <el-switch
          v-model="form.status"
          :active-value="1"
          :inactive-value="0"
          active-text="启用"
          inactive-text="禁用"
        />
      </el-form-item>
      
      <el-form-item v-if="!isEdit" label="密码" prop="password">
        <el-input
          v-model="form.password"
          type="password"
          placeholder="请输入密码"
          show-password
        />
      </el-form-item>
      
      <el-form-item v-if="!isEdit" label="确认密码" prop="confirmPassword">
        <el-input
          v-model="form.confirmPassword"
          type="password"
          placeholder="请再次输入密码"
          show-password
        />
      </el-form-item>
    </el-form>
    
    <template #footer>
      <el-button @click="dialogVisible = false">取消</el-button>
      <el-button type="primary" @click="submitForm" :loading="formLoading">确定</el-button>
    </template>
  </el-dialog>
</template>

<script setup lang="ts">
// 组合式API和工具函数已由unplugin-auto-import自动导入

// 定义组件属性
const props = defineProps({
  visible: {
    type: Boolean,
    default: false
  },
  userData: {
    type: Object,
    default: () => ({})
  },
  isEdit: {
    type: Boolean,
    default: false
  }
})

// 定义组件事件
const emit = defineEmits(['update:visible', 'success'])

// 表单引用
const formRef = ref<InstanceType<typeof ElForm>>()

// 对话框可见性
const dialogVisible = computed({
  get: () => props.visible,
  set: (val) => emit('update:visible', val)
})

// 使用 useApi 封装创建和更新操作
const { execute: executeCreate, loading: createLoading } = useApi(createUser)
const { execute: executeUpdate, loading: updateLoading } = useApi(updateUser)
const formLoading = computed(() => createLoading.value || updateLoading.value)

// 表单数据
const form = reactive({
  id: '',
  username: '',
  displayName: '',
  email: '',
  mobile: '',
  organizationId: '',
  roleIds: [] as string[],
  status: 1,
  password: '',
  confirmPassword: ''
})

// 表单验证规则
const rules = {
  username: [
    { required: true, message: '请输入用户名', trigger: 'blur' },
    { min: 3, max: 20, message: '用户名长度应为3-20个字符', trigger: 'blur' }
  ],
  displayName: [
    { required: true, message: '请输入显示名称', trigger: 'blur' }
  ],
  email: [
    { required: true, message: '请输入邮箱', trigger: 'blur' },
    { type: 'email', message: '请输入有效的邮箱地址', trigger: 'blur' }
  ],
  mobile: [
    { pattern: /^1[3-9]\d{9}$/, message: '请输入有效的手机号', trigger: 'blur' }
  ],
  organizationId: [
    { required: true, message: '请选择所属组织', trigger: 'change' }
  ],
  roleIds: [
    { required: true, message: '请选择角色', trigger: 'change' }
  ],
  password: [
    { required: !props.isEdit, message: '请输入密码', trigger: 'blur' },
    { min: 6, message: '密码长度不能少于6个字符', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: !props.isEdit, message: '请再次输入密码', trigger: 'blur' },
    {
      validator: (rule, value, callback) => {
        if (value !== form.password) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 监听用户数据变化
watch(
  () => props.userData,
  (userData) => {
    if (userData && props.isEdit) {
      Object.assign(form, userData)
      if (userData.roles && Array.isArray(userData.roles)) {
        form.roleIds = userData.roles.map((role: any) => role.id)
      }
    }
  },
  { immediate: true, deep: true }
)

// 重置表单
const resetForm = () => {
  formRef.value?.resetFields()
  Object.assign(form, {
    id: '',
    username: '',
    displayName: '',
    email: '',
    mobile: '',
    organizationId: '',
    roleIds: [],
    status: 1,
    password: '',
    confirmPassword: ''
  })
}

// 提交表单
const submitForm = async () => {
  if (!formRef.value) return
  await formRef.value.validate()

  const submitData = { ...form }
  delete submitData.confirmPassword
  if (props.isEdit) {
    delete submitData.password
  }

  if (props.isEdit) {
    await executeUpdate(form.id, submitData)
    ElMessage.success('更新用户成功')
  } else {
    await executeCreate(submitData)
    ElMessage.success('创建用户成功')
  }

  dialogVisible.value = false
  emit('success')
}

// 暴露方法给父组件
defineExpose({
  resetForm
})
</script>

<style lang="scss" scoped>
.el-select {
  width: 100%;
}
</style>