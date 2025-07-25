<template>
  <div 
    ref="containerRef"
    class="virtual-list"
    :style="{ height: `${containerHeight}px`, overflow: 'auto' }"
    @scroll="handleScroll"
  >
    <!-- 占位容器，用于撑开滚动条 -->
    <div :style="{ height: `${totalHeight}px`, position: 'relative' }">
      <!-- 可见项目容器 -->
      <div 
        class="virtual-list__items"
        :style="{ transform: `translateY(${offsetY}px)` }"
      >
        <div
          v-for="{ item, index, style } in visibleItems"
          :key="getItemKey ? getItemKey(item, index) : index"
          class="virtual-list__item"
          :style="style"
          :data-index="index"
        >
          <slot :item="item" :index="index">
            {{ item }}
          </slot>
        </div>
      </div>
    </div>

    <!-- 加载更多指示器 -->
    <div 
      v-if="loading && hasMore"
      class="virtual-list__loading"
    >
      <slot name="loading">
        <div class="loading-spinner">加载中...</div>
      </slot>
    </div>

    <!-- 空状态 -->
    <div 
      v-if="items.length === 0 && !loading"
      class="virtual-list__empty"
    >
      <slot name="empty">
        <div class="empty-state">暂无数据</div>
      </slot>
    </div>
  </div>
</template>

<script setup lang="ts" generic="T">
import { computed, watch, nextTick } from 'vue'
import { useVirtualScroll } from '@/composables/useVirtualScroll'

interface Props {
  items: T[]
  itemHeight: number | ((index: number) => number)
  containerHeight: number
  buffer?: number
  threshold?: number
  overscan?: number
  getItemKey?: (item: T, index: number) => string | number
  loading?: boolean
  hasMore?: boolean
  loadMore?: () => void
}

const props = withDefaults(defineProps<Props>(), {
  buffer: 5,
  threshold: 0,
  overscan: 5,
  loading: false,
  hasMore: false
})

const emit = defineEmits<{
  scroll: [{ scrollTop: number, isScrolling: boolean }]
  visibleRangeChange: [{ startIndex: number, endIndex: number }]
  loadMore: []
}>()

// 使用虚拟滚动
const {
  containerRef,
  scrollTop,
  isScrolling,
  totalHeight,
  visibleItems,
  visibleRange,
  offsetY,
  scrollToIndex,
  scrollToOffset,
  getItemOffset,
  isItemVisible,
  getVisibleItemCount,
  handleScroll: originalHandleScroll
} = useVirtualScroll(props.items, {
  itemHeight: props.itemHeight,
  containerHeight: props.containerHeight,
  buffer: props.buffer,
  threshold: props.threshold,
  overscan: props.overscan
})

// 增强的滚动处理
const handleScroll = (event: Event) => {
  originalHandleScroll(event)
  
  // 发射滚动事件
  emit('scroll', {
    scrollTop: scrollTop.value,
    isScrolling: isScrolling.value
  })

  // 检查是否需要加载更多
  if (props.hasMore && props.loadMore && !props.loading) {
    const target = event.target as HTMLElement
    const { scrollTop, scrollHeight, clientHeight } = target
    
    // 当滚动到底部附近时触发加载更多
    if (scrollHeight - scrollTop - clientHeight < 100) {
      emit('loadMore')
      props.loadMore()
    }
  }
}

// 监听可见范围变化
watch(visibleRange, (newRange) => {
  emit('visibleRangeChange', newRange)
}, { deep: true })

// 暴露方法给父组件
defineExpose({
  scrollToIndex,
  scrollToOffset,
  getItemOffset,
  isItemVisible,
  getVisibleItemCount,
  scrollTop,
  isScrolling,
  visibleRange
})
</script>

<style lang="scss" scoped>
.virtual-list {
  position: relative;
  
  &__items {
    position: relative;
  }

  &__item {
    box-sizing: border-box;
  }

  &__loading {
    display: flex;
    justify-content: center;
    align-items: center;
    padding: var(--spacing-medium);
    
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
    display: flex;
    justify-content: center;
    align-items: center;
    height: 200px;
    
    .empty-state {
      color: var(--text-color-secondary);
      font-size: var(--font-size-medium);
    }
  }
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

// 滚动条样式
.virtual-list::-webkit-scrollbar {
  width: 6px;
}

.virtual-list::-webkit-scrollbar-track {
  background: var(--fill-color-light);
  border-radius: 3px;
}

.virtual-list::-webkit-scrollbar-thumb {
  background: var(--border-color-base);
  border-radius: 3px;
  
  &:hover {
    background: var(--border-color-darker);
  }
}
</style>