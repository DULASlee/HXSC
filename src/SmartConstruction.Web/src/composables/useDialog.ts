// =============================================
// 对话框通用组合式函数
// =============================================
import { ref, computed } from 'vue'

export interface DialogConfig {
  title?: string
  width?: string | number
  fullscreen?: boolean
  modal?: boolean
  closeOnClickModal?: boolean
  closeOnPressEscape?: boolean
  showClose?: boolean
  destroyOnClose?: boolean
}

export function useDialog(config: DialogConfig = {}) {
  const visible = ref(false)
  const loading = ref(false)
  const title = ref(config.title || '')
  const width = ref(config.width || '50%')
  const fullscreen = ref(config.fullscreen || false)

  // 默认配置
  const defaultConfig = {
    modal: true,
    closeOnClickModal: false,
    closeOnPressEscape: true,
    showClose: true,
    destroyOnClose: false,
    ...config
  }

  // 计算属性
  const dialogConfig = computed(() => ({
    ...defaultConfig,
    width: width.value,
    fullscreen: fullscreen.value
  }))

  // 打开对话框
  const open = (dialogTitle?: string) => {
    if (dialogTitle) {
      title.value = dialogTitle
    }
    visible.value = true
  }

  // 关闭对话框
  const close = () => {
    visible.value = false
    loading.value = false
  }

  // 设置标题
  const setTitle = (newTitle: string) => {
    title.value = newTitle
  }

  // 设置宽度
  const setWidth = (newWidth: string | number) => {
    width.value = newWidth
  }

  // 切换全屏
  const toggleFullscreen = () => {
    fullscreen.value = !fullscreen.value
  }

  // 设置加载状态
  const setLoading = (state: boolean) => {
    loading.value = state
  }

  // 确认操作
  const confirm = async (callback?: () => Promise<boolean> | boolean) => {
    if (callback) {
      setLoading(true)
      try {
        const result = await callback()
        if (result !== false) {
          close()
        }
      } catch (error) {
        console.error('Dialog confirm error:', error)
      } finally {
        setLoading(false)
      }
    } else {
      close()
    }
  }

  // 取消操作
  const cancel = (callback?: () => void) => {
    if (callback) {
      callback()
    }
    close()
  }

  return {
    // 状态
    visible,
    loading,
    title,
    width,
    fullscreen,
    dialogConfig,

    // 方法
    open,
    close,
    setTitle,
    setWidth,
    toggleFullscreen,
    setLoading,
    confirm,
    cancel
  }
}