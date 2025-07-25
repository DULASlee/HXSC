<template>
  <el-icon :size="size" :color="color">
    <component :is="iconComponent" />
  </el-icon>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import * as ElementPlusIconsVue from '@element-plus/icons-vue';

interface Props {
  name: string;
  size?: number | string;
  color?: string;
}

const props = defineProps<Props>();

const iconComponent = computed(() => {
  // 将串式命名转换为帕斯卡命名 (e.g., 'office-building' -> 'OfficeBuilding')
  const iconName = props.name
    .split('-')
    .map(part => part.charAt(0).toUpperCase() + part.slice(1))
    .join('');
    
  return (ElementPlusIconsVue as any)[iconName] || (ElementPlusIconsVue as any)['QuestionFilled'];
});
</script> 