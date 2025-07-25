<template>
  <div class="image-upload">
    <el-upload
      ref="uploadRef"
      :action="action"
      :headers="headers"
      :data="data"
      :multiple="multiple"
      :limit="limit"
      :accept="accept"
      :disabled="disabled"
      :auto-upload="autoUpload"
      :show-file-list="showFileList"
      :drag="drag"
      :list-type="listType"
      :file-list="fileList"
      :on-preview="handlePreview"
      :on-remove="handleRemove"
      :on-success="handleSuccess"
      :on-error="handleError"
      :on-exceed="handleExceed"
      :on-change="handleChange"
      :before-upload="beforeUpload"
      :http-request="customHttpRequest"
    >
      <template v-if="drag">
        <el-icon class="el-icon--upload"><upload-filled /></el-icon>
        <div class="el-upload__text">
          将文件拖到此处，或<em>点击上传</em>
        </div>
      </template>
      <template v-else-if="listType === 'picture-card'">
        <el-icon><plus /></el-icon>
      </template>
      <template v-else>
        <el-button type="primary">点击上传</el-button>
      </template>
      
      <template #tip>
        <div class="el-upload__tip" v-if="tip">
          {{ tip }}
        </div>
      </template>
    </el-upload>
    
    <el-dialog v-model="previewVisible" :title="previewTitle">
      <img :src="previewUrl" alt="Preview Image" style="width: 100%;" />
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { Plus, UploadFilled } from '@element-plus/icons-vue'
import type { UploadInstance, UploadFile, UploadFiles, UploadRequestOptions } from 'element-plus'
import { getToken } from '@/utils/auth'

const props = defineProps({
  value: {
    type: [String, Array],
    default: ''
  },
  action: {
    type: String,
    default: ''
  },
  headers: {
    type: Object,
    default: () => ({})
  },
  data: {
    type: Object,
    default: () => ({})
  },
  multiple: {
    type: Boolean,
    default: false
  },
  limit: {
    type: Number,
    default: 1
  },
  accept: {
    type: String,
    default: 'image/*'
  },
  disabled: {
    type: Boolean,
    default: false
  },
  autoUpload: {
    type: Boolean,
    default: true
  },
  showFileList: {
    type: Boolean,
    default: true
  },
  drag: {
    type: Boolean,
    default: false
  },
  listType: {
    type: String,
    default: 'picture-card'
  },
  tip: {
    type: String,
    default: ''
  },
  customRequest: {
    type: Function,
    default: null
  }
})

const emit = defineEmits(['update:value', 'change', 'success', 'error', 'remove', 'exceed'])

const uploadRef = ref<UploadInstance>()
const fileList = ref<UploadFiles>([])
const previewVisible = ref(false)
const previewUrl = ref('')
const previewTitle = ref('')

// 初始化文件列表
const initFileList = () => {
  if (!props.value) {
    fileList.value = []
    return
  }
  
  if (typeof props.value === 'string') {
    // 单个文件
    fileList.value = [
      {
        name: props.value.split('/').pop() || 'image',
        url: props.value
      }
    ]
  } else if (Array.isArray(props.value)) {
    // 多个文件
    fileList.value = props.value.map((url, index) => ({
      name: url.split('/').pop() || `image-${index}`,
      url
    }))
  }
}

// 监听value变化
watch(
  () => props.value,
  () => {
    initFileList()
  },
  { immediate: true }
)

// 预览图片
const handlePreview = (file: UploadFile) => {
  previewUrl.value = file.url || ''
  previewTitle.value = file.name || ''
  previewVisible.value = true
}

// 移除图片
const handleRemove = (file: UploadFile, files: UploadFiles) => {
  emit('remove', file, files)
  
  // 更新value
  if (props.multiple) {
    const urls = files.map(f => f.url || f.response?.data)
    emit('update:value', urls)
    emit('change', urls)
  } else {
    emit('update:value', '')
    emit('change', '')
  }
}

// 上传成功
const handleSuccess = (response: any, file: UploadFile, files: UploadFiles) => {
  emit('success', response, file, files)
  
  // 更新value
  if (props.multiple) {
    const urls = files.map(f => f.url || f.response?.data)
    emit('update:value', urls)
    emit('change', urls)
  } else {
    const url = response.data
    emit('update:value', url)
    emit('change', url)
  }
}

// 上传失败
const handleError = (error: any, file: UploadFile, files: UploadFiles) => {
  emit('error', error, file, files)
}

// 超出限制
const handleExceed = (files: File[], uploadFiles: UploadFiles) => {
  emit('exceed', files, uploadFiles)
}

// 文件状态改变
const handleChange = (file: UploadFile, files: UploadFiles) => {
  // 如果不是自动上传，需要手动更新value
  if (!props.autoUpload && file.status === 'ready') {
    if (props.multiple) {
      const urls = files.map(f => f.url || '')
      emit('update:value', urls)
      emit('change', urls)
    } else {
      // 创建临时URL
      const url = URL.createObjectURL(file.raw!)
      emit('update:value', url)
      emit('change', url)
    }
  }
}

// 上传前校验
const beforeUpload = (file: File) => {
  // 可以在这里添加文件类型、大小等校验
  return true
}

// 自定义上传请求
const customHttpRequest = (options: UploadRequestOptions) => {
  if (props.customRequest) {
    return props.customRequest(options)
  }
  
  // 默认上传实现
  const { action, file, onProgress, onSuccess, onError } = options
  
  // 创建FormData
  const formData = new FormData()
  formData.append('file', file)
  
  // 添加额外数据
  if (props.data) {
    Object.keys(props.data).forEach(key => {
      formData.append(key, props.data[key])
    })
  }
  
  // 创建XHR
  const xhr = new XMLHttpRequest()
  xhr.open('POST', action, true)
  
  // 设置headers
  const headers = {
    Authorization: `Bearer ${getToken()}`,
    ...props.headers
  }
  
  Object.keys(headers).forEach(key => {
    xhr.setRequestHeader(key, headers[key])
  })
  
  // 上传进度
  xhr.upload.addEventListener('progress', e => {
    if (e.total > 0) {
      e.percent = (e.loaded / e.total) * 100
    }
    onProgress(e)
  })
  
  // 请求完成
  xhr.onload = () => {
    if (xhr.status >= 200 && xhr.status < 300) {
      const response = JSON.parse(xhr.responseText)
      onSuccess(response)
    } else {
      onError({ status: xhr.status, message: xhr.statusText })
    }
  }
  
  // 请求错误
  xhr.onerror = () => {
    onError({ status: xhr.status, message: '上传失败' })
  }
  
  // 发送请求
  xhr.send(formData)
  
  return xhr
}

// 手动上传
const submit = () => {
  uploadRef.value?.submit()
}

// 清空上传列表
const clearFiles = () => {
  uploadRef.value?.clearFiles()
}

// 中止上传
const abort = (file?: UploadFile) => {
  uploadRef.value?.abort(file)
}

// 暴露方法
defineExpose({
  uploadRef,
  fileList,
  submit,
  clearFiles,
  abort
})
</script>

<style lang="scss" scoped>
.image-upload {
  :deep(.el-upload--picture-card) {
    width: 100px;
    height: 100px;
    line-height: 100px;
  }
  
  :deep(.el-upload-list--picture-card .el-upload-list__item) {
    width: 100px;
    height: 100px;
  }
}
</style>