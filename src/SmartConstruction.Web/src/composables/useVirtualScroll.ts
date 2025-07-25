// 虚拟滚动组合式函数
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'

interface VirtualScrollOptions {
  itemHeight: number | ((index: number) => number)
  containerHeight: number
  buffer?: number
  threshold?: number
  overscan?: number
}

interface VirtualScrollItem {
  index: number
  top: number
  height: number
  bottom: number
}

export function useVirtualScroll<T = any>(
  items: T[],
  options: VirtualScrollOptions
) {
  const containerRef = ref<HTMLElement>()
  const scrollTop = ref(0)
  const isScrolling = ref(false)
  const buffer = options.buffer || 5
  const threshold = options.threshold || 0
  const overscan = options.overscan || 5

  let scrollTimer: number | null = null

  // 计算项目高度
  const getItemHeight = (index: number): number => {
    if (typeof options.itemHeight === 'function') {
      return options.itemHeight(index)
    }
    return options.itemHeight
  }

  // 计算所有项目的位置信息
  const itemPositions = computed(() => {
    const positions: VirtualScrollItem[] = []
    let top = 0

    for (let i = 0; i < items.length; i++) {
      const height = getItemHeight(i)
      positions.push({
        index: i,
        top,
        height,
        bottom: top + height
      })
      top += height
    }

    return positions
  })

  // 总高度
  const totalHeight = computed(() => {
    const lastItem = itemPositions.value[itemPositions.value.length - 1]
    return lastItem ? lastItem.bottom : 0
  })

  // 可见区域的开始和结束索引
  const visibleRange = computed(() => {
    const containerHeight = options.containerHeight
    const start = scrollTop.value
    const end = start + containerHeight

    let startIndex = 0
    let endIndex = items.length - 1

    // 二分查找开始索引
    let left = 0
    let right = itemPositions.value.length - 1

    while (left <= right) {
      const mid = Math.floor((left + right) / 2)
      const item = itemPositions.value[mid]

      if (item.bottom <= start) {
        left = mid + 1
      } else if (item.top >= start) {
        right = mid - 1
      } else {
        startIndex = mid
        break
      }
    }

    // 二分查找结束索引
    left = startIndex
    right = itemPositions.value.length - 1

    while (left <= right) {
      const mid = Math.floor((left + right) / 2)
      const item = itemPositions.value[mid]

      if (item.top <= end) {
        left = mid + 1
      } else {
        right = mid - 1
        endIndex = mid - 1
        break
      }
    }

    // 添加缓冲区
    startIndex = Math.max(0, startIndex - buffer)
    endIndex = Math.min(items.length - 1, endIndex + buffer)

    return { startIndex, endIndex }
  })

  // 可见项目
  const visibleItems = computed(() => {
    const { startIndex, endIndex } = visibleRange.value
    const result = []

    for (let i = startIndex; i <= endIndex; i++) {
      const position = itemPositions.value[i]
      result.push({
        item: items[i],
        index: i,
        style: {
          position: 'absolute' as const,
          top: `${position.top}px`,
          height: `${position.height}px`,
          width: '100%'
        }
      })
    }

    return result
  })

  // 滚动偏移量
  const offsetY = computed(() => {
    const { startIndex } = visibleRange.value
    return startIndex > 0 ? itemPositions.value[startIndex].top : 0
  })

  // 处理滚动事件
  const handleScroll = (event: Event) => {
    const target = event.target as HTMLElement
    scrollTop.value = target.scrollTop

    // 设置滚动状态
    isScrolling.value = true
    
    // 清除之前的定时器
    if (scrollTimer) {
      clearTimeout(scrollTimer)
    }

    // 设置定时器，滚动结束后重置状态
    scrollTimer = window.setTimeout(() => {
      isScrolling.value = false
    }, 150)
  }

  // 滚动到指定索引
  const scrollToIndex = (index: number, behavior: ScrollBehavior = 'smooth') => {
    if (!containerRef.value || index < 0 || index >= items.length) {
      return
    }

    const position = itemPositions.value[index]
    if (position) {
      containerRef.value.scrollTo({
        top: position.top,
        behavior
      })
    }
  }

  // 滚动到指定位置
  const scrollToOffset = (offset: number, behavior: ScrollBehavior = 'smooth') => {
    if (!containerRef.value) {
      return
    }

    containerRef.value.scrollTo({
      top: offset,
      behavior
    })
  }

  // 获取项目在容器中的位置
  const getItemOffset = (index: number): number => {
    const position = itemPositions.value[index]
    return position ? position.top : 0
  }

  // 检查项目是否可见
  const isItemVisible = (index: number): boolean => {
    const { startIndex, endIndex } = visibleRange.value
    return index >= startIndex && index <= endIndex
  }

  // 获取可见项目数量
  const getVisibleItemCount = (): number => {
    const { startIndex, endIndex } = visibleRange.value
    return endIndex - startIndex + 1
  }

  // 动态调整项目高度
  const updateItemHeight = (index: number, height: number) => {
    if (typeof options.itemHeight === 'function') {
      console.warn('Cannot update item height when using dynamic height function')
      return
    }

    // 这里可以实现动态高度更新逻辑
    // 需要重新计算所有项目位置
  }

  // 预加载项目
  const preloadItems = (count: number = overscan) => {
    const { endIndex } = visibleRange.value
    const preloadEnd = Math.min(items.length - 1, endIndex + count)
    
    // 这里可以触发预加载逻辑
    console.log(`Preloading items ${endIndex + 1} to ${preloadEnd}`)
  }

  // 组件挂载时绑定滚动事件
  onMounted(() => {
    if (containerRef.value) {
      containerRef.value.addEventListener('scroll', handleScroll, { passive: true })
    }
  })

  // 组件卸载时清理
  onUnmounted(() => {
    if (containerRef.value) {
      containerRef.value.removeEventListener('scroll', handleScroll)
    }
    if (scrollTimer) {
      clearTimeout(scrollTimer)
    }
  })

  return {
    // 引用
    containerRef,
    
    // 状态
    scrollTop,
    isScrolling,
    totalHeight,
    visibleItems,
    visibleRange,
    offsetY,
    
    // 方法
    scrollToIndex,
    scrollToOffset,
    getItemOffset,
    isItemVisible,
    getVisibleItemCount,
    updateItemHeight,
    preloadItems,
    
    // 事件处理
    handleScroll
  }
}

// 虚拟网格滚动
export function useVirtualGrid<T = any>(
  items: T[],
  options: {
    itemWidth: number
    itemHeight: number
    containerWidth: number
    containerHeight: number
    gap?: number
    buffer?: number
  }
) {
  const containerRef = ref<HTMLElement>()
  const scrollTop = ref(0)
  const scrollLeft = ref(0)
  const gap = options.gap || 0
  const buffer = options.buffer || 2

  // 计算列数
  const columnCount = computed(() => {
    return Math.floor((options.containerWidth + gap) / (options.itemWidth + gap))
  })

  // 计算行数
  const rowCount = computed(() => {
    return Math.ceil(items.length / columnCount.value)
  })

  // 总高度
  const totalHeight = computed(() => {
    return rowCount.value * (options.itemHeight + gap) - gap
  })

  // 可见行范围
  const visibleRowRange = computed(() => {
    const startRow = Math.floor(scrollTop.value / (options.itemHeight + gap))
    const endRow = Math.min(
      rowCount.value - 1,
      Math.ceil((scrollTop.value + options.containerHeight) / (options.itemHeight + gap))
    )

    return {
      startRow: Math.max(0, startRow - buffer),
      endRow: Math.min(rowCount.value - 1, endRow + buffer)
    }
  })

  // 可见项目
  const visibleItems = computed(() => {
    const { startRow, endRow } = visibleRowRange.value
    const result = []

    for (let row = startRow; row <= endRow; row++) {
      for (let col = 0; col < columnCount.value; col++) {
        const index = row * columnCount.value + col
        if (index >= items.length) break

        const x = col * (options.itemWidth + gap)
        const y = row * (options.itemHeight + gap)

        result.push({
          item: items[index],
          index,
          row,
          col,
          style: {
            position: 'absolute' as const,
            left: `${x}px`,
            top: `${y}px`,
            width: `${options.itemWidth}px`,
            height: `${options.itemHeight}px`
          }
        })
      }
    }

    return result
  })

  // 处理滚动事件
  const handleScroll = (event: Event) => {
    const target = event.target as HTMLElement
    scrollTop.value = target.scrollTop
    scrollLeft.value = target.scrollLeft
  }

  // 滚动到指定项目
  const scrollToItem = (index: number, behavior: ScrollBehavior = 'smooth') => {
    if (!containerRef.value || index < 0 || index >= items.length) {
      return
    }

    const row = Math.floor(index / columnCount.value)
    const y = row * (options.itemHeight + gap)

    containerRef.value.scrollTo({
      top: y,
      behavior
    })
  }

  onMounted(() => {
    if (containerRef.value) {
      containerRef.value.addEventListener('scroll', handleScroll, { passive: true })
    }
  })

  onUnmounted(() => {
    if (containerRef.value) {
      containerRef.value.removeEventListener('scroll', handleScroll)
    }
  })

  return {
    containerRef,
    scrollTop,
    scrollLeft,
    totalHeight,
    visibleItems,
    visibleRowRange,
    columnCount,
    rowCount,
    scrollToItem,
    handleScroll
  }
}