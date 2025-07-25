// =============================================
// 下载工具组合式函数
// =============================================
import { ref } from 'vue'
import { ElMessage } from 'element-plus'

export function useDownload() {
  const downloading = ref(false)

  // 下载文件
  const downloadFile = (url: string, filename?: string) => {
    const link = document.createElement('a')
    link.href = url
    if (filename) {
      link.download = filename
    }
    link.style.display = 'none'
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }

  // 下载Blob数据
  const downloadBlob = (blob: Blob, filename: string) => {
    const url = URL.createObjectURL(blob)
    downloadFile(url, filename)
    URL.revokeObjectURL(url)
  }

  // 下载文本内容
  const downloadText = (content: string, filename: string, type = 'text/plain') => {
    const blob = new Blob([content], { type })
    downloadBlob(blob, filename)
  }

  // 下载JSON数据
  const downloadJson = (data: any, filename: string) => {
    const jsonString = JSON.stringify(data, null, 2)
    downloadText(jsonString, filename, 'application/json')
  }

  // 下载CSV数据
  const downloadCsv = (data: any[], filename: string, headers?: string[]) => {
    let csvContent = ''
    
    // 添加表头
    if (headers && headers.length > 0) {
      csvContent += headers.join(',') + '\n'
    }
    
    // 添加数据行
    data.forEach(row => {
      if (Array.isArray(row)) {
        csvContent += row.join(',') + '\n'
      } else if (typeof row === 'object') {
        const values = headers ? headers.map(header => row[header] || '') : Object.values(row)
        csvContent += values.join(',') + '\n'
      }
    })
    
    // 添加BOM以支持中文
    const bom = '\uFEFF'
    downloadText(bom + csvContent, filename, 'text/csv')
  }

  // 从URL下载文件
  const downloadFromUrl = async (url: string, filename?: string) => {
    try {
      downloading.value = true
      
      const response = await fetch(url)
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`)
      }
      
      const blob = await response.blob()
      const finalFilename = filename || getFilenameFromUrl(url) || 'download'
      
      downloadBlob(blob, finalFilename)
      ElMessage.success('下载成功')
    } catch (error) {
      console.error('下载失败:', error)
      ElMessage.error('下载失败')
    } finally {
      downloading.value = false
    }
  }

  // 从URL获取文件名
  const getFilenameFromUrl = (url: string): string => {
    try {
      const urlObj = new URL(url)
      const pathname = urlObj.pathname
      return pathname.substring(pathname.lastIndexOf('/') + 1)
    } catch {
      return ''
    }
  }

  // 导出Excel（需要配合后端API）
  const exportExcel = async (apiUrl: string, params: any = {}, filename = 'export.xlsx') => {
    try {
      downloading.value = true
      
      const queryString = new URLSearchParams(params).toString()
      const url = queryString ? `${apiUrl}?${queryString}` : apiUrl
      
      await downloadFromUrl(url, filename)
    } catch (error) {
      console.error('导出Excel失败:', error)
      ElMessage.error('导出失败')
    } finally {
      downloading.value = false
    }
  }

  return {
    downloading,
    downloadFile,
    downloadBlob,
    downloadText,
    downloadJson,
    downloadCsv,
    downloadFromUrl,
    exportExcel,
    getFilenameFromUrl
  }
}