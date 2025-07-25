<template>
  <div class="avatar-upload">
    <el-upload
      class="avatar-uploader"
      :action="uploadUrl"
      :headers="uploadHeaders"
      :show-file-list="false"
      :before-upload="beforeUpload"
      :on-success="handleSuccess"
      :on-error="handleError"
      :disabled="disabled"
    >
      <img v-if="avatarUrl" :src="avatarUrl" class="avatar" />
      <div v-else class="avatar-placeholder">
        <el-icon class="avatar-uploader-icon"><Plus /></el-icon>
        <div class="upload-text">上传头像</div>
      </div>
    </el-upload>
    
    <div class="upload-tips">
      <p>支持 JPG、PNG 格式</p>
      <p>文件大小不超过 2MB</p>
      <p>建议尺寸 200x200 像素</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { Plus } from '@element-plus/icons-vue'
import { getToken } from '@/utils/auth'

interface Props {
  modelValue?: string
  disabled?: boolean
}

interface Emits {
  (e: 'update:modelValue', value: string): void
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  disabled: false
})

const emit = defineEmits<Emits>()

// 计算属性
const avatarUrl = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 上传配置
const uploadUrl = `${import.meta.env.VITE_API_BASE_URL}/upload/avatar`
const uploadHeaders = {
  Authorization: `Bearer ${getToken()}`
}

// 上传前检查
function beforeUpload(file: File) {
  const isImage = file.type.startsWith('image/')
  const isLt2M = file.size / 1024 / 1024 < 2

  if (!isImage) {
    ElMessage.error('只能上传图片文件!')
    return false
  }
  
  if (!isLt2M) {
    ElMessage.error('图片大小不能超过 2MB!')
    return false
  }
  
  return true
}

// 上传成功
function handleSuccess(response: any) {
  if (response.code === 200) {
    avatarUrl.value = response.data.url
    ElMessage.success('头像上传成功')
  } else {
    ElMessage.error(response.message || '上传失败')
  }
}

// 上传失败
function handleError(error: any) {
  console.error('Upload error:', error)
  ElMessage.error('头像上传失败')
}
</script>

<style lang="scss" scoped>
.avatar-upload {
  display: flex;
  align-items: flex-start;
  gap: 20px;
}

.avatar-uploader {
  :deep(.el-upload) {
    border: 1px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: var(--el-transition-duration-fast);
    
    &:hover {
      border-color: var(--el-color-primary);
    }
  }
}

.avatar {
  width: 120px;
  height: 120px;
  display: block;
  object-fit: cover;
}

.avatar-placeholder {
  width: 120px;
  height: 120px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--el-text-color-secondary);
  
  .avatar-uploader-icon {
    font-size: 28px;
    margin-bottom: 8px;
  }
  
  .upload-text {
    font-size: 14px;
  }
}

.upload-tips {
  color: var(--el-text-color-secondary);
  font-size: 12px;
  line-height: 1.5;
  
  p {
    margin: 0 0 4px 0;
    
    &:last-child {
      margin-bottom: 0;
    }
  }
}
</style>