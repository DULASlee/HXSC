<template>
  <div class="profile-container">
    <el-card class="profile-card">
      <template #header>
        <div class="card-header">
          <span>个人中心</span>
        </div>
      </template>
      
      <div class="profile-content">
        <div class="profile-avatar">
          <el-avatar :size="120" :src="userStore.avatar">
            <el-icon size="60"><User /></el-icon>
          </el-avatar>
          <el-button type="primary" size="small" class="upload-btn">
            更换头像
          </el-button>
        </div>
        
        <div class="profile-info">
          <el-form :model="profileForm" label-width="100px">
            <el-form-item label="用户名">
              <el-input v-model="profileForm.username" disabled />
            </el-form-item>
            
            <el-form-item label="显示名称">
              <el-input v-model="profileForm.displayName" />
            </el-form-item>
            
            <el-form-item label="邮箱">
              <el-input v-model="profileForm.email" />
            </el-form-item>
            
            <el-form-item label="手机号">
              <el-input v-model="profileForm.mobile" />
            </el-form-item>
            
            <el-form-item label="当前租户">
              <el-input v-model="profileForm.tenantName" disabled />
            </el-form-item>
            
            <el-form-item label="用户角色">
              <el-tag
                v-for="role in userStore.roles"
                :key="role"
                type="success"
                class="role-tag"
              >
                {{ role }}
              </el-tag>
            </el-form-item>
            
            <el-form-item>
              <el-button type="primary" @click="updateProfile">
                保存修改
              </el-button>
              <el-button @click="resetForm">
                重置
              </el-button>
            </el-form-item>
          </el-form>
        </div>
      </div>
    </el-card>
    
    <el-card class="password-card">
      <template #header>
        <div class="card-header">
          <span>修改密码</span>
        </div>
      </template>
      
      <el-form :model="passwordForm" :rules="passwordRules" ref="passwordFormRef" label-width="100px">
        <el-form-item label="当前密码" prop="oldPassword">
          <el-input
            v-model="passwordForm.oldPassword"
            type="password"
            show-password
            placeholder="请输入当前密码"
          />
        </el-form-item>
        
        <el-form-item label="新密码" prop="newPassword">
          <el-input
            v-model="passwordForm.newPassword"
            type="password"
            show-password
            placeholder="请输入新密码"
          />
        </el-form-item>
        
        <el-form-item label="确认密码" prop="confirmPassword">
          <el-input
            v-model="passwordForm.confirmPassword"
            type="password"
            show-password
            placeholder="请再次输入新密码"
          />
        </el-form-item>
        
        <el-form-item>
          <el-button type="primary" @click="changePassword">
            修改密码
          </el-button>
          <el-button @click="resetPasswordForm">
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { ElMessage, ElForm } from 'element-plus'
import { User } from '@element-plus/icons-vue'

const userStore = useUserStore()
const passwordFormRef = ref<InstanceType<typeof ElForm>>()

// 个人信息表单
const profileForm = reactive({
  username: '',
  displayName: '',
  email: '',
  mobile: '',
  tenantName: ''
})

// 密码修改表单
const passwordForm = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// 密码验证规则
const passwordRules = {
  oldPassword: [
    { required: true, message: '请输入当前密码', trigger: 'blur' }
  ],
  newPassword: [
    { required: true, message: '请输入新密码', trigger: 'blur' },
    { min: 6, message: '密码长度至少6位', trigger: 'blur' }
  ],
  confirmPassword: [
    { required: true, message: '请确认新密码', trigger: 'blur' },
    {
      validator: (rule: any, value: string, callback: Function) => {
        if (value !== passwordForm.newPassword) {
          callback(new Error('两次输入的密码不一致'))
        } else {
          callback()
        }
      },
      trigger: 'blur'
    }
  ]
}

// 初始化表单数据
const initForm = () => {
  const userInfo = userStore.userInfo
  if (userInfo) {
    profileForm.username = userInfo.username
    profileForm.displayName = userInfo.displayName
    profileForm.email = userInfo.email || ''
    profileForm.mobile = userInfo.mobile || ''
  }
  profileForm.tenantName = userStore.tenantName || 'SYSTEM'
}

// 更新个人信息
const updateProfile = async () => {
  try {
    // 这里应该调用API更新用户信息
    ElMessage.success('个人信息更新成功')
  } catch (error) {
    ElMessage.error('更新失败')
  }
}

// 重置个人信息表单
const resetForm = () => {
  initForm()
}

// 修改密码
const changePassword = async () => {
  const valid = await passwordFormRef.value?.validate()
  if (!valid) return

  try {
    // 这里应该调用API修改密码
    ElMessage.success('密码修改成功')
    resetPasswordForm()
  } catch (error) {
    ElMessage.error('密码修改失败')
  }
}

// 重置密码表单
const resetPasswordForm = () => {
  passwordForm.oldPassword = ''
  passwordForm.newPassword = ''
  passwordForm.confirmPassword = ''
  passwordFormRef.value?.clearValidate()
}

onMounted(() => {
  initForm()
})
</script>

<style lang="scss" scoped>
.profile-container {
  max-width: 800px;
  margin: 0 auto;
  
  .profile-card {
    margin-bottom: 20px;
    
    .profile-content {
      display: flex;
      gap: 40px;
      
      .profile-avatar {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 20px;
        
        .upload-btn {
          margin-top: 10px;
        }
      }
      
      .profile-info {
        flex: 1;
        
        .role-tag {
          margin-right: 8px;
          margin-bottom: 4px;
        }
      }
    }
  }
  
  .password-card {
    .el-form {
      max-width: 400px;
    }
  }
}

@media (max-width: 768px) {
  .profile-container {
    .profile-card {
      .profile-content {
        flex-direction: column;
        align-items: center;
        text-align: center;
      }
    }
  }
}
</style>