<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item v-for="(item, index) in levelList" :key="item.path">
        <span v-if="item.redirect === 'noRedirect' || index == levelList.length - 1" class="no-redirect">{{ i18nStore.t('menu.' + item.meta.title) }}</span>
        <a v-else @click.prevent="handleLink(item)">{{ i18nStore.t('menu.' + item.meta.title) }}</a>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script lang="ts">
export default {
  name: 'Breadcrumb'
}
</script>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import type { RouteLocationMatched } from 'vue-router';
import { useI18nStore } from '../../stores/i18n';

const route = useRoute();
const router = useRouter();
const i18nStore = useI18nStore();
const levelList = ref<RouteLocationMatched[]>([]);

const getBreadcrumb = () => {
  let matched = route.matched.filter(item => item.meta && item.meta.title);
  const first = matched[0];

  if (!isDashboard(first)) {
    matched = [{ path: '/dashboard', meta: { title: 'dashboard' }} as any].concat(matched);
  }

  levelList.value = matched.filter(item => item.meta && item.meta.title && item.meta.breadcrumb !== false);
}

const isDashboard = (route?: RouteLocationMatched) => {
  const name = route && route.name;
  if (!name) {
    return false;
  }
  return name.toString().trim().toLocaleLowerCase() === 'Dashboard'.toLocaleLowerCase();
}

const handleLink = (item: RouteLocationMatched) => {
  const { redirect, path } = item;
  if (redirect) {
    router.push(redirect as string);
    return;
  }
  router.push(path);
}

watch(() => route.path, () => {
  getBreadcrumb();
}, { immediate: true });

getBreadcrumb();
</script>

<style lang="scss" scoped>
.app-breadcrumb.el-breadcrumb {
  display: inline-block;
  font-size: 14px;
  line-height: 50px;
  margin-left: 8px;

  .no-redirect {
    color: #97a8be;
    cursor: text;
  }
}
</style> 