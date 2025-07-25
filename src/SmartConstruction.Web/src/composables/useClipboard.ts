// =============================================
// 剪贴板组合式函数
// =============================================
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

export function useClipboard() {
  const isSupported = ref(!!navigator.clipboard)

  // 复制文本到剪贴板
  const copy = async (text: string): Promise<boolean> => {
    try {
      if (navigator.clipboard) {
        await navigator.clipboard.writeText(text)
      } else {
        // 降级方案
        const textArea = document.createElement('textarea')
        textArea.value = text
        textArea.style.position = 'fixed'
        textArea.style.left = '-999999px'
        textArea.style.top = '-999999px'
        document.body.appendChild(textArea)
        textArea.focus()
        textArea.select()
        document.execCommand('copy')
        textArea.remove()
      }
      
      ElMessage.success('复制成功')
      return true
    } catch (error) {
      console.error('复制失败:', error)
      ElMessage.error('复制失败')
      return false
    }
  }

  // 读取剪贴板内容
  const read = async (): Promise<string> => {
    try {
      if (navigator.clipboard) {
        return await navigator.clipboard.readText()
      } else {
        throw new Error('不支持读取剪贴板')
      }
    } catch (error) {
      console.error('读取剪贴板失败:', error)
      return ''
    }
  }

  // 复制对象为JSON字符串
  const copyObject = async (obj: any): Promise<boolean> => {
    try {
      const jsonString = JSON.stringify(obj, null, 2)
      return await copy(jsonString)
    } catch (error) {
      console.error('复制对象失败:', error)
      ElMessage.error('复制失败')
      return false
    }
  }

  return {
    isSupported,
    copy,
    read,
    copyObject
  }
}