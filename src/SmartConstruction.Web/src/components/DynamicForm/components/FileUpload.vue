<template>
  <div class="file-upload">
    <el-upload
      ref="uploadRef"
      :action="uploadUrl"
      :headers="uploadHeaders"
      :data="uploadData"
      :multiple="multiple"
      :accept="accept"
      :limit="limit"
      :file-list="fileList"
      :before-upload="beforeUpload"
      :on-success="handleSuccess"
      :on-error="handleError"
      :on-remove="handleRemove"
      :on-exceed="handleExceed"
      :on-preview="handlePreview"
      :disabled="disabled"
      :drag="drag"
      :show-file-list="showFileList"
      class="upload-component"
    >
      <template v-if="drag">
        <div class="upload-dragger">
          <el-icon class="upload-icon">
            <UploadFilled />
          </el-icon>
          <div class="upload-text">
            <p>将文件拖到此处，或<em>点击上传</em></p>
            <p class="upload-tip">{{ uploadTip }}</p>
          </div>
        </div>
      </template>
      <template v-else>
        <el-button type="primary" :disabled="disabled">
          <el-icon><Upload /></el-icon>
          选择文件
        </el-button>
        <div v-if="uploadTip" class="upload-tip">{{ uploadTip }}</div>
      </template>
    </el-upload>

    <!-- 文件预览弹窗 -->
    <el-dialog
      v-model="previewDialog.visible"
      :title="previewDialog.title"
      width="80%"
      append-to-body
    >
      <div class="file-preview">
        <!-- 图片预览 -->
        <img
          v-if="isImage(previewDialog.url)"
          :src="previewDialog.url"
          class="preview-image"
          alt="预览图片"
        />
        <!-- 视频预览 -->
        <video
          v-else-if="isVideo(previewDialog.url)"
          :src="previewDialog.url"
          controls
          class="preview-video"
        />
        <!-- 其他文件类型 -->
        <div v-else class="preview-other">
          <el-icon class="file-icon">
            <Document />
          </el-icon>
          <p>{{ previewDialog.title }}</p>
          <el-button type="primary" @click="downloadFile(previewDialog.url, previewDialog.title)">
            <el-icon><Download /></el-icon>
            下载文件
          </el-button>
        </div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { ElMessage, ElMessageBox, UploadFile, UploadFiles, UploadRawFile } from 'element-plus'
import { Upload, UploadFilled, Document, Download } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'

interface FileItem {
  name: string
  url: string
  size?: number
  type?: string
  uid?: string | number
}

interface Props {
  modelValue?: FileItem[] | FileItem | string[] | string
  uploadUrl?: string
  multiple?: boolean
  accept?: string
  limit?: number
  maxSize?: number // MB
  disabled?: boolean
  drag?: boolean
  showFileList?: boolean
  uploadTip?: string
}

const props = withDefaults(defineProps<Props>(), {
  uploadUrl: '/api/files/upload',
  multiple: false,
  accept: '*',
  limit: 5,
  maxSize: 10,
  disabled: false,
  drag: false,
  showFileList: true,
  uploadTip: '支持常见文件格式，单个文件不超过10MB'
})

const emit = defineEmits(['update:modelValue', 'change', 'success', 'error'])

const userStore = useUserStore()
const uploadRef = ref()
const fileList = ref<UploadFile[]>([])

// 预览弹窗
const previewDialog = ref({
  visible: false,
  title: '',
  url: ''
})

// 上传请求头
const uploadHeaders = computed(() => ({
  'Authorization': `Bearer ${userStore.token?.accessToken}`,
  'X-Tenant-Id': userStore.currentUser?.tenantId
}))

// 上传额外数据
const uploadData = computed(() => ({
  type: 'dynamic-form',
  category: 'attachment'
}))

// 初始化文件列表
function initFileList() {
  if (!props.modelValue) {
    fileList.value = []
    return
  }

  let files: FileItem[] = []
  
  if (Array.isArray(props.modelValue)) {
    files = props.modelValue.map(item => {
      if (typeof item === 'string') {
        return {
          name: getFileNameFromUrl(item),
          url: item,
          uid: Date.now() + Math.random()
        }
      }
      return item
    })
  } else if (typeof props.modelValue === 'string') {
    files = [{
      name: getFileNameFromUrl(props.modelValue),
      url: props.modelValue,
      uid: Date.now()
    }]
  } else {
    files = [props.modelValue]
  }

  fileList.value = files.map(file => ({
    name: file.name,
    url: file.url,
    uid: file.uid || Date.now() + Math.random(),
    status: 'success' as const
  }))
}

// 从URL获取文件名
function getFileNameFromUrl(url: string): string {
  return url.split('/').pop() || 'unknown'
}

// 上传前检查
function beforeUpload(file: UploadRawFile) {
  // 检查文件大小
  if (file.size / 1024 / 1024 > props.maxSize) {
    ElMessage.error(`文件大小不能超过 ${props.maxSize}MB`)
    return false
  }

  // 检查文件类型
  if (props.accept !== '*') {
    const acceptTypes = props.accept.split(',').map(type => type.trim())
    const fileType = file.type || ''
    const fileName = file.name || ''
    
    const isAccepted = acceptTypes.some(type => {
      if (type.startsWith('.')) {
        return fileName.toLowerCase().endsWith(type.toLowerCase())
      }
      return fileType.includes(type.replace('*', ''))
    })

    if (!isAccepted) {
      ElMessage.error(`不支持的文件类型，请选择 ${props.accept} 格式的文件`)
      return false
    }
  }

  return true
}

// 上传成功
function handleSuccess(response: any, file: UploadFile) {
  if (response.code === 200) {
    const fileInfo: FileItem = {
      name: file.name,
      url: response.data.url,
      size: file.size,
      type: file.raw?.type,
      uid: file.uid
    }

    updateValue()
    emit('success', fileInfo)
    ElMessage.success('文件上传成功')
  } else {
    handleError(new Error(response.message), file)
  }
}

// 上传失败
function handleError(error: Error, file: UploadFile) {
  ElMessage.error(`文件上传失败: ${error.message}`)
  emit('error', error)
  
  // 移除失败的文件
  const index = fileList.value.findIndex(item => item.uid === file.uid)
  if (index > -1) {
    fileList.value.splice(index, 1)
    updateValue()
  }
}

// 移除文件
function handleRemove(file: UploadFile) {
  updateValue()
}

// 超出限制
function handleExceed(files: File[]) {
  ElMessage.warning(`最多只能上传 ${props.limit} 个文件`)
}

// 预览文件
function handlePreview(file: UploadFile) {
  previewDialog.value = {
    visible: true,
    title: file.name,
    url: file.url || ''
  }
}

// 判断是否为图片
function isImage(url: string): boolean {
  return /\.(jpg|jpeg|png|gif|bmp|webp)$/i.test(url)
}

// 判断是否为视频
function isVideo(url: string): boolean {
  return /\.(mp4|avi|mov|wmv|flv|webm)$/i.test(url)
}

// 下载文件
function downloadFile(url: string, filename: string) {
  const link = document.createElement('a')
  link.href = url
  link.download = filename
  link.target = '_blank'
  document.body.appendChild(link)
  link.click()
  document.body.removeChild(link)
}

// 更新值
function updateValue() {
  const files = fileList.value
    .filter(file => file.status === 'success')
    .map(file => ({
      name: file.name,
      url: file.url || '',
      size: file.size,
      uid: file.uid
    }))

  let value: any
  if (props.multiple) {
    value = files
  } else {
    value = files.length > 0 ? files[0] : null
  }

  emit('update:modelValue', value)
  emit('change', value)
}

// 监听外部值变化
watch(() => props.modelValue, () => {
  initFileList()
}, { immediate: true, deep: true })

// 清空文件列表
function clearFiles() {
  uploadRef.value?.clearFiles()
  fileList.value = []
  updateValue()
}

// 手动上传
function submit() {
  uploadRef.value?.submit()
}

// 暴露方法
defineExpose({
  clearFiles,
  submit
})
</script>

<style lang="scss" scoped>
.file-upload {
  .upload-component {
    width: 100%;
  }

  .upload-dragger {
    padding: 40px;
    text-align: center;
    border: 2px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    transition: border-color 0.3s;

    &:hover {
      border-color: var(--el-color-primary);
    }

    .upload-icon {
      font-size: 48px;
      color: var(--el-text-color-secondary);
      margin-bottom: 16px;
    }

    .upload-text {
      p {
        margin: 0;
        font-size: 14px;
        color: var(--el-text-color-regular);

        em {
          color: var(--el-color-primary);
          font-style: normal;
        }
      }

      .upload-tip {
        font-size: 12px;
        color: var(--el-text-color-secondary);
        margin-top: 8px;
      }
    }
  }

  .upload-tip {
    font-size: 12px;
    color: var(--el-text-color-secondary);
    margin-top: 8px;
  }

  .file-preview {
    text-align: center;

    .preview-image {
      max-width: 100%;
      max-height: 500px;
      object-fit: contain;
    }

    .preview-video {
      max-width: 100%;
      max-height: 500px;
    }

    .preview-other {
      padding: 40px;

      .file-icon {
        font-size: 64px;
        color: var(--el-text-color-secondary);
        margin-bottom: 16px;
      }

      p {
        font-size: 16px;
        margin: 16px 0;
        color: var(--el-text-color-regular);
      }
    }
  }
}
</style>