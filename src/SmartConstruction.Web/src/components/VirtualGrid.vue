<template>
  <div 
    ref="containerRef"
    class="virtual-grid"
    :style="{ 
      width: `${containerWidth}px`,
      height: `${containerHeight}px`, 
      overflow: 'auto' 
    }"
    @scroll="handleScroll"
  >
    <!-- 占位容器 -->
    <div :style="{ height: `${totalHeight}px`, position: 'relative' }">
      <!-- 可见项目 -->
      <div
        v-for="{ item, index, style } in visibleItems"
        :key="getItemKey ? getItemKey(item, index) : index"
        class="virtual-grid__item"
        :style="style"
        :data-index="index"
      >
        <slot :item="item" :index="index">
          {{ item }}
        </slot>
      </div>
    </div>

    <!-- 加载更多 -->
    <div 
      v-if="loading && hasMore"
      class="virtual-grid__loading"
    >
      <slot name="loading">
        <div class="loading-spinner">加载中...</div>
      </slot>
    </div>

    <!-- 空状态 -->
    <div 
      v-if="items.length === 0 && !loading"
      class="virtual-grid__empty"
    >
      <slot name="empty">
        <div class="empty-state">暂无数据</div>
      </slot>
    </div>
  </div>
</template>

<script setup lang="ts" generic="T">
import { watch } from 'vue'
import { useVirtualGrid } from '@/composables/useVirtualScroll'

interface Props {
  items: T[]
  itemWidth: number
  itemHeight: number
  containerWidth: number
  containerHeight: number
  gap?: number
  buffer?: number
  getItemKey?: (item: T, index: number) => string | number
  loading?: boolean
  hasMore?: boolean
  loadMore?: () => void
}

const props = withDefaults(defineProps<Props>(), {
  gap: 0,
  buffer: 2,
  loading: false,
  hasMore: false
})

const emit = defineEmits<{
  scroll: [{ scrollTop: number, scrollLeft: number }]
  visibleRangeChange: [{ startRow: number, endRow: number }]
  loadMore: []
}>()

// 使用虚拟网格
const {
  containerRef,
  scrollTop,
  scrollLeft,
  totalHeight,
  visibleItems,
  visibleRowRange,
  columnCount,
  rowCount,
  scrollToItem,
  handleScroll: originalHandleScroll
} = useVirtualGrid(props.items, {
  itemWidth: props.itemWidth,
  itemHeight: props.itemHeight,
  containerWidth: props.containerWidth,
  containerHeight: props.containerHeight,
  gap: props.gap,
  buffer: props.buffer
})

// 增强的滚动处理
const handleScroll = (event: Event) => {
  originalHandleScroll(event)
  
  // 发射滚动事件
  emit('scroll', {
    scrollTop: scrollTop.value,
    scrollLeft: scrollLeft.value
  })

  // 检查是否需要加载更多
  if (props.hasMore && props.loadMore && !props.loading) {
    const target = event.target as HTMLElement
    const { scrollTop, scrollHeight, clientHeight } = target
    
    if (scrollHeight - scrollTop - clientHeight < 100) {
      emit('loadMore')
      props.loadMore()
    }
  }
}

// 监听可见范围变化
watch(visibleRowRange, (newRange) => {
  emit('visibleRangeChange', newRange)
}, { deep: true })

// 暴露方法
defineExpose({
  scrollToItem,
  scrollTop,
  scrollLeft,
  visibleRowRange,
  columnCount,
  rowCount
})
</script>

<style lang="scss" scoped>
.virtual-grid {
  position: relative;

  &__item {
    box-sizing: border-box;
    display: flex;
    align-items: center;
    justify-content: center;
  }

  &__loading {
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    padding: var(--spacing-medium);
    background: rgba(255, 255, 255, 0.9);
    
    .loading-spinner {
      display: flex;
      align-items: center;
      gap: var(--spacing-small);
      color: var(--text-color-secondary);
      font-size: var(--font-size-small);
      
      &::before {
        content: '';
        width: 16px;
        height: 16px;
        border: 2px solid var(--border-color-lighter);
        border-top-color: var(--primary-color);
        border-radius: 50%;
        animation: spin 1s linear infinite;
      }
    }
  }

  &__empty {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    
    .empty-state {
      color: var(--text-color-secondary);
      font-size: var(--font-size-medium);
      text-align: center;
    }
  }
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

// 滚动条样式
.virtual-grid::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

.virtual-grid::-webkit-scrollbar-track {
  background: var(--fill-color-light);
  border-radius: 3px;
}

.virtual-grid::-webkit-scrollbar-thumb {
  background: var(--border-color-base);
  border-radius: 3px;
  
  &:hover {
    background: var(--border-color-darker);
  }
}

.virtual-grid::-webkit-scrollbar-corner {
  background: var(--fill-color-light);
}
</style>