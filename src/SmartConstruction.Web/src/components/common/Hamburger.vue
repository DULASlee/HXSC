<template>
  <div style="padding: 0 15px;" @click="toggleClick">
    <el-icon :size="20">
      <component :is="isActive ? Fold : Expand" />
    </el-icon>
  </div>
</template>

<script lang="ts">
export default {
  name: 'Hamburger'
}
</script>

<script setup lang="ts">
import { Fold, Expand } from '@element-plus/icons-vue'

defineProps({
  isActive: {
    type: Boolean,
    default: false
  }
})

const emit = defineEmits(['toggleClick'])

const toggleClick = () => {
  emit('toggleClick')
}
</script>

<style scoped>
.hamburger {
  display: inline-block;
  vertical-align: middle;
  width: 20px;
  height: 20px;
}
</style> 