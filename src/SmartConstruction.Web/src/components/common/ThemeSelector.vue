<template>
  <el-dropdown @command="handleThemeChange">
    <span class="theme-selector">
      <el-icon><Sunny /></el-icon>
      {{ i18nStore.t("system.theme") }}
      <el-icon class="el-icon--right"><ArrowDown /></el-icon>
    </span>
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item
          command="light"
          :class="{ 'is-active': appStore.theme === 'light' }"
        >
          <el-icon><Sunny /></el-icon>
          {{ i18nStore.t("system.lightTheme") }}
        </el-dropdown-item>
        <el-dropdown-item
          command="dark"
          :class="{ 'is-active': appStore.theme === 'dark' }"
        >
          <el-icon><Moon /></el-icon>
          {{ i18nStore.t("system.darkTheme") }}
        </el-dropdown-item>
        <el-dropdown-item
          command="blue"
          :class="{ 'is-active': appStore.theme === 'blue' }"
        >
          <el-icon><Star /></el-icon>
          {{ i18nStore.t("system.blueTheme") }}
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
import { useAppStore } from "@/stores/app";
import { useI18nStore } from "@/stores/i18n";
import type { ThemeType } from "@/stores/app";

const appStore = useAppStore();
const i18nStore = useI18nStore();

const handleThemeChange = (theme: ThemeType) => {
  appStore.setTheme(theme);
};
</script>

<style scoped>
.theme-selector {
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 0 12px;
  color: var(--app-text-color-regular);

  &:hover {
    color: var(--el-color-primary);
  }
}

:deep(.el-dropdown-menu__item) {
  &.is-active {
    color: var(--el-color-primary);
    background-color: var(--el-color-primary-light-9);
  }
}
</style>
