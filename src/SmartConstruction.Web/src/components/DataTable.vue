<template>
  <div class="data-table-wrapper">
    <div class="table-header" v-if="$slots.header">
      <slot name="header"></slot>
    </div>
    
    <el-table
      ref="tableRef"
      v-loading="loading"
      :data="data"
      :border="border"
      :stripe="stripe"
      :height="height"
      :max-height="maxHeight"
      :row-key="rowKey"
      :highlight-current-row="highlightCurrentRow"
      @selection-change="handleSelectionChange"
      @sort-change="handleSortChange"
      @row-click="handleRowClick"
    >
      <el-table-column
        v-if="showSelection"
        type="selection"
        width="55"
        align="center"
      />
      
      <el-table-column
        v-if="showIndex"
        type="index"
        width="60"
        label="序号"
        align="center"
      />
      
      <slot></slot>
      
      <el-table-column
        v-if="showActions"
        label="操作"
        :width="actionsWidth"
        :fixed="actionsFixed"
        align="center"
      >
        <template #default="scope">
          <slot name="actions" :row="scope.row" :$index="scope.$index"></slot>
        </template>
      </el-table-column>
      
      <template #empty>
        <el-empty description="暂无数据" />
      </template>
    </el-table>
    
    <div class="table-footer" v-if="showPagination">
      <el-pagination
        v-model:current-page="currentPage"
        v-model:page-size="pageSize"
        :page-sizes="pageSizes"
        :total="total"
        :background="true"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="handleSizeChange"
        @current-change="handleCurrentChange"
      />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'

const props = defineProps({
  data: {
    type: Array,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  },
  border: {
    type: Boolean,
    default: true
  },
  stripe: {
    type: Boolean,
    default: true
  },
  height: {
    type: [String, Number],
    default: ''
  },
  maxHeight: {
    type: [String, Number],
    default: ''
  },
  rowKey: {
    type: String,
    default: 'id'
  },
  highlightCurrentRow: {
    type: Boolean,
    default: false
  },
  showSelection: {
    type: Boolean,
    default: false
  },
  showIndex: {
    type: Boolean,
    default: false
  },
  showActions: {
    type: Boolean,
    default: true
  },
  actionsWidth: {
    type: [String, Number],
    default: 150
  },
  actionsFixed: {
    type: String,
    default: 'right'
  },
  showPagination: {
    type: Boolean,
    default: true
  },
  page: {
    type: Number,
    default: 1
  },
  size: {
    type: Number,
    default: 10
  },
  total: {
    type: Number,
    default: 0
  },
  pageSizes: {
    type: Array,
    default: () => [10, 20, 50, 100]
  }
})

const emit = defineEmits([
  'selection-change',
  'sort-change',
  'row-click',
  'page-change',
  'size-change',
  'update:page',
  'update:size'
])

const tableRef = ref()
const currentPage = ref(props.page)
const pageSize = ref(props.size)

// 监听页码变化
watch(
  () => props.page,
  (newPage) => {
    currentPage.value = newPage
  }
)

// 监听每页条数变化
watch(
  () => props.size,
  (newSize) => {
    pageSize.value = newSize
  }
)

// 选择变化
const handleSelectionChange = (selection: any[]) => {
  emit('selection-change', selection)
}

// 排序变化
const handleSortChange = (sort: any) => {
  emit('sort-change', sort)
}

// 行点击
const handleRowClick = (row: any, column: any, event: any) => {
  emit('row-click', row, column, event)
}

// 页码变化
const handleCurrentChange = (page: number) => {
  currentPage.value = page
  emit('update:page', page)
  emit('page-change', page)
}

// 每页条数变化
const handleSizeChange = (size: number) => {
  pageSize.value = size
  emit('update:size', size)
  emit('size-change', size)
}

// 暴露方法
defineExpose({
  tableRef,
  clearSelection: () => tableRef.value?.clearSelection(),
  toggleRowSelection: (row: any, selected?: boolean) => tableRef.value?.toggleRowSelection(row, selected),
  toggleAllSelection: () => tableRef.value?.toggleAllSelection(),
  toggleRowExpansion: (row: any, expanded?: boolean) => tableRef.value?.toggleRowExpansion(row, expanded),
  setCurrentRow: (row: any) => tableRef.value?.setCurrentRow(row),
  clearSort: () => tableRef.value?.clearSort(),
  clearFilter: (columnKeys?: string[]) => tableRef.value?.clearFilter(columnKeys),
  doLayout: () => tableRef.value?.doLayout(),
  sort: (prop: string, order: string) => tableRef.value?.sort(prop, order)
})
</script>

<style lang="scss" scoped>
.data-table-wrapper {
  .table-header {
    margin-bottom: 15px;
  }
  
  .table-footer {
    margin-top: 15px;
    display: flex;
    justify-content: flex-end;
  }
}
</style>