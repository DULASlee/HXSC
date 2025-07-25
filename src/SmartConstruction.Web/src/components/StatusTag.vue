<template>
  <el-tag
    :type="tagType"
    :effect="effect"
    :size="size"
    :round="round"
    :closable="closable"
    :disable-transitions="disableTransitions"
    :hit="hit"
    :color="color"
  >
    <slot>{{ label }}</slot>
  </el-tag>
</template>

<script setup lang="ts">
import { computed } from 'vue'

const props = defineProps({
  status: {
    type: [String, Number],
    required: true
  },
  statusMap: {
    type: Object,
    default: () => ({})
  },
  effect: {
    type: String,
    default: 'light'
  },
  size: {
    type: String,
    default: 'default'
  },
  round: {
    type: Boolean,
    default: false
  },
  closable: {
    type: Boolean,
    default: false
  },
  disableTransitions: {
    type: Boolean,
    default: false
  },
  hit: {
    type: Boolean,
    default: false
  },
  color: {
    type: String,
    default: ''
  }
})

// 标签类型
const tagType = computed(() => {
  const statusConfig = props.statusMap[props.status]
  return statusConfig?.type || 'info'
})

// 标签文本
const label = computed(() => {
  const statusConfig = props.statusMap[props.status]
  return statusConfig?.label || props.status
})
</script>