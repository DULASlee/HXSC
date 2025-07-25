/**
 * 布局调试工具
 * 用于快速识别和解决布局问题
 */

// 显示元素轮廓，帮助识别布局问题
export function showElementOutlines(selector = '*', color = 'red') {
  const style = document.createElement('style')
  style.id = 'layout-debugger-outlines'
  style.textContent = `
    ${selector} {
      outline: 1px solid ${color} !important;
    }
  `
  document.head.appendChild(style)
  
  console.log('[布局调试] 已启用元素轮廓显示')
  return () => {
    const styleElement = document.getElementById('layout-debugger-outlines')
    if (styleElement) {
      styleElement.remove()
      console.log('[布局调试] 已禁用元素轮廓显示')
    }
  }
}

// 强制重绘元素
export function forceRepaint(selector: string) {
  const elements = document.querySelectorAll(selector)
  
  elements.forEach(el => {
    // 保存当前显示状态
    const display = window.getComputedStyle(el).display
    
    // 触发重绘
    ;(el as HTMLElement).style.display = 'none'
    
    // 强制浏览器重新计算布局
    void el.offsetHeight
    
    // 恢复显示状态
    ;(el as HTMLElement).style.display = display
  })
  
  console.log(`[布局调试] 已强制重绘 ${elements.length} 个元素`)
}

// 检测布局溢出
export function detectOverflow() {
  const overflowElements: HTMLElement[] = []
  
  // 检查所有元素是否有水平溢出
  document.querySelectorAll('*').forEach(el => {
    const element = el as HTMLElement
    if (element.offsetWidth > element.parentElement?.offsetWidth) {
      overflowElements.push(element)
    }
  })
  
  if (overflowElements.length > 0) {
    console.warn('[布局调试] 检测到以下元素存在溢出问题:')
    overflowElements.forEach(el => {
      console.warn(el)
      // 高亮溢出元素
      el.style.outline = '2px dashed red'
    })
  } else {
    console.log('[布局调试] 未检测到溢出问题')
  }
  
  return overflowElements
}

// 检测z-index堆叠问题
export function analyzeZIndex() {
  const zIndexMap = new Map<number, HTMLElement[]>()
  
  document.querySelectorAll('*').forEach(el => {
    const element = el as HTMLElement
    const style = window.getComputedStyle(element)
    const zIndex = parseInt(style.zIndex)
    
    if (!isNaN(zIndex) && zIndex !== 0) {
      if (!zIndexMap.has(zIndex)) {
        zIndexMap.set(zIndex, [])
      }
      zIndexMap.get(zIndex)?.push(element)
    }
  })
  
  // 按z-index值排序
  const sortedEntries = [...zIndexMap.entries()].sort((a, b) => b[0] - a[0])
  
  console.log('[布局调试] z-index堆叠顺序分析:')
  sortedEntries.forEach(([zIndex, elements]) => {
    console.log(`z-index: ${zIndex}, 元素数量: ${elements.length}`)
    console.log(elements)
  })
  
  return sortedEntries
}

// 检测菜单层级问题
export function validateMenuStructure() {
  try {
    // 检查菜单DOM结构
    const menuElements = document.querySelectorAll('.el-menu')
    if (menuElements.length === 0) {
      console.warn('[布局调试] 未找到菜单元素')
      return false
    }
    
    console.log('[布局调试] 菜单结构验证:')
    menuElements.forEach((menu, index) => {
      const menuItems = menu.querySelectorAll('.el-menu-item')
      const subMenus = menu.querySelectorAll('.el-sub-menu')
      
      console.log(`菜单 #${index + 1}:`)
      console.log(`- 菜单项数量: ${menuItems.length}`)
      console.log(`- 子菜单数量: ${subMenus.length}`)
      
      // 检查是否有激活的菜单项
      const activeItems = menu.querySelectorAll('.el-menu-item.is-active')
      console.log(`- 激活的菜单项: ${activeItems.length}`)
      
      if (activeItems.length === 0) {
        console.warn('  警告: 没有激活的菜单项')
      } else if (activeItems.length > 1) {
        console.warn('  警告: 多个菜单项被同时激活')
      }
    })
    
    return true
  } catch (error) {
    console.error('[布局调试] 菜单结构验证失败:', error)
    return false
  }
}

// 导出调试工具集
export const layoutDebugger = {
  showElementOutlines,
  forceRepaint,
  detectOverflow,
  analyzeZIndex,
  validateMenuStructure
}

export default layoutDebugger