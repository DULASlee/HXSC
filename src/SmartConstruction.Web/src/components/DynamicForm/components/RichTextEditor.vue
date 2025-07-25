<template>
  <div class="rich-text-editor">
    <div class="editor-toolbar">
      <el-button-group size="small">
        <el-button @click="execCommand('bold')" :class="{ active: isActive('bold') }">
          <el-icon><Bold /></el-icon>
        </el-button>
        <el-button @click="execCommand('italic')" :class="{ active: isActive('italic') }">
          <el-icon><Italic /></el-icon>
        </el-button>
        <el-button @click="execCommand('underline')" :class="{ active: isActive('underline') }">
          <el-icon><Underline /></el-icon>
        </el-button>
      </el-button-group>

      <el-button-group size="small">
        <el-button @click="execCommand('justifyLeft')" :class="{ active: isActive('justifyLeft') }">
          <el-icon><AlignLeft /></el-icon>
        </el-button>
        <el-button @click="execCommand('justifyCenter')" :class="{ active: isActive('justifyCenter') }">
          <el-icon><AlignCenter /></el-icon>
        </el-button>
        <el-button @click="execCommand('justifyRight')" :class="{ active: isActive('justifyRight') }">
          <el-icon><AlignRight /></el-icon>
        </el-button>
      </el-button-group>

      <el-button-group size="small">
        <el-button @click="execCommand('insertUnorderedList')" :class="{ active: isActive('insertUnorderedList') }">
          <el-icon><List /></el-icon>
        </el-button>
        <el-button @click="execCommand('insertOrderedList')" :class="{ active: isActive('insertOrderedList') }">
          <el-icon><Numbered /></el-icon>
        </el-button>
      </el-button-group>

      <el-select
        v-model="currentFontSize"
        size="small"
        style="width: 80px"
        @change="changeFontSize"
      >
        <el-option label="12px" value="1" />
        <el-option label="14px" value="2" />
        <el-option label="16px" value="3" />
        <el-option label="18px" value="4" />
        <el-option label="24px" value="5" />
        <el-option label="32px" value="6" />
        <el-option label="48px" value="7" />
      </el-select>

      <el-color-picker
        v-model="currentColor"
        size="small"
        @change="changeColor"
      />

      <el-button-group size="small">
        <el-button @click="insertLink">
          <el-icon><Link /></el-icon>
        </el-button>
        <el-button @click="insertImage">
          <el-icon><Picture /></el-icon>
        </el-button>
      </el-button-group>

      <el-button-group size="small">
        <el-button @click="undo">
          <el-icon><RefreshLeft /></el-icon>
        </el-button>
        <el-button @click="redo">
          <el-icon><RefreshRight /></el-icon>
        </el-button>
      </el-button-group>

      <el-button size="small" @click="toggleSource">
        {{ showSource ? '预览' : '源码' }}
      </el-button>
    </div>

    <div class="editor-content">
      <div
        v-if="!showSource"
        ref="editorRef"
        class="editor-area"
        contenteditable
        :style="{ height: height + 'px' }"
        @input="handleInput"
        @keydown="handleKeydown"
        @paste="handlePaste"
        @focus="handleFocus"
        @blur="handleBlur"
      />
      <el-input
        v-else
        v-model="htmlContent"
        type="textarea"
        :rows="Math.floor(height / 20)"
        @input="handleSourceInput"
      />
    </div>

    <!-- 插入链接弹窗 -->
    <el-dialog v-model="linkDialog.visible" title="插入链接" width="400px">
      <el-form :model="linkDialog.form" label-width="80px">
        <el-form-item label="链接文本">
          <el-input v-model="linkDialog.form.text" placeholder="请输入链接文本" />
        </el-form-item>
        <el-form-item label="链接地址">
          <el-input v-model="linkDialog.form.url" placeholder="请输入链接地址" />
        </el-form-item>
        <el-form-item label="打开方式">
          <el-radio-group v-model="linkDialog.form.target">
            <el-radio label="_self">当前窗口</el-radio>
            <el-radio label="_blank">新窗口</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="linkDialog.visible = false">取消</el-button>
          <el-button type="primary" @click="confirmInsertLink">确定</el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 插入图片弹窗 -->
    <el-dialog v-model="imageDialog.visible" title="插入图片" width="400px">
      <el-tabs v-model="imageDialog.activeTab">
        <el-tab-pane label="网络图片" name="url">
          <el-form :model="imageDialog.form" label-width="80px">
            <el-form-item label="图片地址">
              <el-input v-model="imageDialog.form.url" placeholder="请输入图片地址" />
            </el-form-item>
            <el-form-item label="替代文本">
              <el-input v-model="imageDialog.form.alt" placeholder="请输入替代文本" />
            </el-form-item>
            <el-form-item label="宽度">
              <el-input v-model="imageDialog.form.width" placeholder="例如：300px 或 50%" />
            </el-form-item>
          </el-form>
        </el-tab-pane>
        <el-tab-pane label="上传图片" name="upload">
          <file-upload
            v-model="imageDialog.uploadFile"
            accept="image/*"
            :drag="true"
            :show-file-list="false"
            upload-tip="支持 jpg、png、gif 格式，大小不超过 2MB"
            @success="handleImageUpload"
          />
        </el-tab-pane>
      </el-tabs>
      <template #footer>
        <div class="dialog-footer">
          <el-button @click="imageDialog.visible = false">取消</el-button>
          <el-button type="primary" @click="confirmInsertImage">确定</el-button>
        </div>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, watch, nextTick, onMounted, onBeforeUnmount } from 'vue'
import { ElMessage } from 'element-plus'
import {
  Bold, Italic, Underline, AlignLeft, AlignCenter, AlignRight,
  List, Link, Picture, RefreshLeft, RefreshRight
} from '@element-plus/icons-vue'
import FileUpload from './FileUpload.vue'

// 自定义图标组件
const Numbered = {
  template: '<svg viewBox="0 0 1024 1024"><path d="M384 832h640v128H384v-128zm0-384h640v128H384V448zm0-384h640v128H384V64zM192 128c0-35.3-28.7-64-64-64s-64 28.7-64 64 28.7 64 64 64 64-28.7 64-64zm0 384c0-35.3-28.7-64-64-64s-64 28.7-64 64 28.7 64 64 64 64-28.7 64-64zm0 384c0-35.3-28.7-64-64-64s-64 28.7-64 64 28.7 64 64 64 64-28.7 64-64z"/></svg>'
}

interface Props {
  modelValue?: string
  height?: number
  placeholder?: string
  disabled?: boolean
  readonly?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  modelValue: '',
  height: 300,
  placeholder: '请输入内容...',
  disabled: false,
  readonly: false
})

const emit = defineEmits(['update:modelValue', 'change', 'focus', 'blur'])

const editorRef = ref<HTMLElement>()
const htmlContent = ref('')
const showSource = ref(false)
const currentFontSize = ref('3')
const currentColor = ref('#000000')

// 链接弹窗
const linkDialog = reactive({
  visible: false,
  form: {
    text: '',
    url: '',
    target: '_self'
  }
})

// 图片弹窗
const imageDialog = reactive({
  visible: false,
  activeTab: 'url',
  form: {
    url: '',
    alt: '',
    width: ''
  },
  uploadFile: null as any
})

// 初始化内容
function initContent() {
  htmlContent.value = props.modelValue || ''
  if (editorRef.value && !showSource.value) {
    editorRef.value.innerHTML = htmlContent.value
  }
}

// 执行命令
function execCommand(command: string, value?: string) {
  if (props.disabled || props.readonly) return
  
  document.execCommand(command, false, value)
  updateContent()
}

// 检查命令状态
function isActive(command: string): boolean {
  return document.queryCommandState(command)
}

// 改变字体大小
function changeFontSize(size: string) {
  execCommand('fontSize', size)
}

// 改变颜色
function changeColor(color: string) {
  execCommand('foreColor', color)
}

// 撤销
function undo() {
  execCommand('undo')
}

// 重做
function redo() {
  execCommand('redo')
}

// 插入链接
function insertLink() {
  const selection = window.getSelection()
  if (selection && selection.toString()) {
    linkDialog.form.text = selection.toString()
  }
  linkDialog.visible = true
}

// 确认插入链接
function confirmInsertLink() {
  const { text, url, target } = linkDialog.form
  if (!url) {
    ElMessage.warning('请输入链接地址')
    return
  }

  const linkHtml = `<a href="${url}" target="${target}">${text || url}</a>`
  execCommand('insertHTML', linkHtml)
  
  linkDialog.visible = false
  linkDialog.form = { text: '', url: '', target: '_self' }
}

// 插入图片
function insertImage() {
  imageDialog.visible = true
}

// 确认插入图片
function confirmInsertImage() {
  const { url, alt, width } = imageDialog.form
  if (!url) {
    ElMessage.warning('请输入图片地址')
    return
  }

  let imgHtml = `<img src="${url}" alt="${alt || ''}">`
  if (width) {
    imgHtml = `<img src="${url}" alt="${alt || ''}" style="width: ${width};">`
  }
  
  execCommand('insertHTML', imgHtml)
  
  imageDialog.visible = false
  imageDialog.form = { url: '', alt: '', width: '' }
}

// 处理图片上传成功
function handleImageUpload(file: any) {
  imageDialog.form.url = file.url
  imageDialog.form.alt = file.name
}

// 切换源码模式
function toggleSource() {
  showSource.value = !showSource.value
  
  if (showSource.value) {
    htmlContent.value = editorRef.value?.innerHTML || ''
  } else {
    nextTick(() => {
      if (editorRef.value) {
        editorRef.value.innerHTML = htmlContent.value
      }
    })
  }
}

// 处理输入
function handleInput() {
  updateContent()
}

// 处理源码输入
function handleSourceInput() {
  updateContent()
}

// 处理键盘事件
function handleKeydown(event: KeyboardEvent) {
  // Ctrl+Z 撤销
  if (event.ctrlKey && event.key === 'z') {
    event.preventDefault()
    undo()
  }
  // Ctrl+Y 重做
  if (event.ctrlKey && event.key === 'y') {
    event.preventDefault()
    redo()
  }
}

// 处理粘贴
function handlePaste(event: ClipboardEvent) {
  event.preventDefault()
  
  const clipboardData = event.clipboardData
  if (!clipboardData) return

  // 获取纯文本内容
  const text = clipboardData.getData('text/plain')
  if (text) {
    execCommand('insertText', text)
  }
}

// 处理焦点
function handleFocus() {
  emit('focus')
}

// 处理失焦
function handleBlur() {
  updateContent()
  emit('blur')
}

// 更新内容
function updateContent() {
  let content = ''
  
  if (showSource.value) {
    content = htmlContent.value
  } else {
    content = editorRef.value?.innerHTML || ''
    htmlContent.value = content
  }

  emit('update:modelValue', content)
  emit('change', content)
}

// 监听外部值变化
watch(() => props.modelValue, () => {
  if (props.modelValue !== htmlContent.value) {
    initContent()
  }
})

// 监听禁用状态
watch(() => props.disabled, (disabled) => {
  if (editorRef.value) {
    editorRef.value.contentEditable = (!disabled && !props.readonly).toString()
  }
})

// 监听只读状态
watch(() => props.readonly, (readonly) => {
  if (editorRef.value) {
    editorRef.value.contentEditable = (!props.disabled && !readonly).toString()
  }
})

onMounted(() => {
  initContent()
  
  if (editorRef.value) {
    editorRef.value.contentEditable = (!props.disabled && !props.readonly).toString()
    
    // 设置占位符
    if (!htmlContent.value && props.placeholder) {
      editorRef.value.setAttribute('data-placeholder', props.placeholder)
    }
  }
})

onBeforeUnmount(() => {
  // 清理事件监听器
})
</script>

<style lang="scss" scoped>
.rich-text-editor {
  border: 1px solid var(--el-border-color);
  border-radius: 4px;

  .editor-toolbar {
    display: flex;
    align-items: center;
    gap: 8px;
    padding: 8px 12px;
    border-bottom: 1px solid var(--el-border-color);
    background-color: var(--el-bg-color-page);

    .el-button.active {
      background-color: var(--el-color-primary);
      color: white;
    }
  }

  .editor-content {
    .editor-area {
      padding: 12px;
      outline: none;
      overflow-y: auto;
      line-height: 1.6;
      font-size: 14px;
      color: var(--el-text-color-regular);

      &:empty::before {
        content: attr(data-placeholder);
        color: var(--el-text-color-placeholder);
        pointer-events: none;
      }

      // 基础样式
      p {
        margin: 0 0 8px 0;
        
        &:last-child {
          margin-bottom: 0;
        }
      }

      ul, ol {
        margin: 8px 0;
        padding-left: 24px;
      }

      li {
        margin: 4px 0;
      }

      a {
        color: var(--el-color-primary);
        text-decoration: none;

        &:hover {
          text-decoration: underline;
        }
      }

      img {
        max-width: 100%;
        height: auto;
        vertical-align: middle;
      }

      blockquote {
        margin: 8px 0;
        padding: 8px 16px;
        border-left: 4px solid var(--el-color-primary);
        background-color: var(--el-bg-color-page);
        color: var(--el-text-color-secondary);
      }

      code {
        padding: 2px 4px;
        background-color: var(--el-bg-color-page);
        border-radius: 2px;
        font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
        font-size: 12px;
      }

      pre {
        margin: 8px 0;
        padding: 12px;
        background-color: var(--el-bg-color-page);
        border-radius: 4px;
        overflow-x: auto;

        code {
          padding: 0;
          background: none;
        }
      }
    }
  }

  .dialog-footer {
    text-align: right;
  }
}
</style>