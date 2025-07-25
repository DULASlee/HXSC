<template>
  <div v-if="!item.meta || !item.meta.hidden">
    <template v-if="hasOneShowingChild(item.children, item) && (!onlyOneChild.children || onlyOneChild.noShowingChildren) && !item.meta?.alwaysShow">
      <app-link v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path)">
        <el-menu-item :index="resolvePath(onlyOneChild.path)" :class="{'submenu-title-no-dropdown':!isNest}">
          <el-icon v-if="onlyOneChild.meta.icon||item.meta.icon"><component :is="onlyOneChild.meta.icon||item.meta.icon" /></el-icon>
          <template #title>{{ i18nStore.t('menu.' + onlyOneChild.meta.title) }}</template>
        </el-menu-item>
      </app-link>
    </template>

    <el-sub-menu v-else ref="subMenu" :index="resolvePath(item.path)" popper-append-to-body>
      <template #title>
        <el-icon v-if="item.meta && item.meta.icon"><component :is="item.meta.icon" /></el-icon>
        <span>{{ i18nStore.t('menu.' + item.meta.title) }}</span>
      </template>
      <sidebar-item
        v-for="child in item.children"
        :key="child.path"
        :is-nest="true"
        :item="child"
        :base-path="resolvePath(item.path)"
        class="nest-menu"
      />
    </el-sub-menu>
  </div>
</template>

<script lang="ts">
export default {
  name: 'SidebarItem'
}
</script>

<script setup lang="ts">
import { ref } from 'vue';
import path from 'path-browserify';
import { useI18nStore } from '../../stores/i18n';
import AppLink from './Link.vue';

defineOptions({
  name: 'SidebarItem'
})

const props = defineProps({
  item: {
    type: Object,
    required: true
  },
  isNest: {
    type: Boolean,
    default: false
  },
  basePath: {
    type: String,
    default: ''
  }
});

const i18nStore = useI18nStore();
const onlyOneChild = ref(null);

function hasOneShowingChild(children = [], parent) {
  const showingChildren = children.filter(item => {
    if (item.meta?.hidden) {
      return false;
    } else {
      onlyOneChild.value = item;
      return true;
    }
  });

  if (showingChildren.length === 1) {
    return true;
  }

  if (showingChildren.length === 0) {
    onlyOneChild.value = { ...parent, path: '', noShowingChildren: true };
    return true;
  }

  return false;
}

function resolvePath(routePath) {
  if (/^(https?:|mailto:|tel:)/.test(routePath)) {
    return routePath;
  }
  return path.resolve(props.basePath, routePath);
}
</script> 