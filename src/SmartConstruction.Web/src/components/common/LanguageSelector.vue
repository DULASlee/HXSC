<template>
  <div class="language-selector">
    <el-icon><Switch /></el-icon>
    <el-select
      v-model="currentLanguage"
      @change="handleLanguageChange"
      class="language-select"
    >
      <el-option
        v-for="item in languageOptions"
        :key="item.value"
        :label="item.label"
        :value="item.value"
      ></el-option>
    </el-select>
  </div>
</template>

<script setup lang="ts">
import { useAppStore } from "@/stores/app";
import { useI18nStore } from "@/stores/i18n";
import type { LanguageType } from "@/stores/app";

const appStore = useAppStore();
const i18nStore = useI18nStore();

const languageNames = {
  "zh-CN": "简体中文",
  "en-US": "English",
  "ja-JP": "日本語",
};

const languageOptions = [
  { value: "zh-CN", label: "🇨🇳 简体中文" },
  { value: "en-US", label: "🇺🇸 English" },
  { value: "ja-JP", label: "🇯🇵 日本語" },
];

const currentLanguage = computed(() => {
  return languageOptions.find(item => item.value === appStore.language)?.value;
});

const getCurrentLanguageName = () => {
  return languageNames[appStore.language] || languageNames["zh-CN"];
};

const handleLanguageChange = (language: LanguageType) => {
  appStore.setLanguage(language);
  i18nStore.setLocale(language);

  // 可以在这里添加Element Plus的语言切换
  // 需要动态导入Element Plus的语言包
};
</script>

<style scoped>
.language-selector {
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
