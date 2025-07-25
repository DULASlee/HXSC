<template>
  <el-dropdown trigger="click" @command="handleSetLanguage">
    <el-button 
      :icon="Switch" 
      circle 
      :title="$t('system.language')"
    />
    <template #dropdown>
      <el-dropdown-menu class="language-dropdown">
        <div class="language-header">
          <el-icon><Switch /></el-icon>
          <span>{{ $t('system.language') }}</span>
        </div>
        <el-dropdown-item 
          v-for="lang in supportedLanguages"
          :key="lang.code"
          :command="lang.code" 
          :disabled="currentLanguage === lang.code"
        >
          <span class="language-item">
            <span class="language-icon">{{ lang.icon }}</span>
            <span class="language-name">{{ lang.name }}</span>
            <el-icon v-if="currentLanguage === lang.code" class="language-check"><Check /></el-icon>
          </span>
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { ElMessage } from 'element-plus'
import { Switch, Check } from '@element-plus/icons-vue'
import { useI18nStore } from '@/stores/i18n'
import { useAppStore } from '@/stores/app'

const i18nStore = useI18nStore()
const appStore = useAppStore()

// 当前语言
const currentLanguage = computed(() => i18nStore.locale)

// 支持的语言列表
const supportedLanguages = ref([
  { code: 'zh-CN', name: '简体中文', icon: '🇨🇳' },
  { code: 'en-US', name: 'English', icon: '🇺🇸' },
  { code: 'ja-JP', name: '日本語', icon: '🇯🇵' },
])

// 切换语言
const handleSetLanguage = (langCode: string | number | boolean) => {
  try {
    // 切换语言
    i18nStore.setLocale(langCode as any)
    appStore.setLanguage(langCode as any)
    
    // 刷新页面以应用主题
    // window.location.reload()

    ElMessage.success($t.value('system.langChangeSuccess'))
  } catch (error) {
    console.error('语言切换失败:', error)
    ElMessage.error($t.value('system.langChangeFailed'))
  }
}

// 获取翻译文本的辅助函数
const $t = computed(() => i18nStore.t)
</script>

<style lang="scss" scoped>
.language-dropdown {
  .language-header {
    display: flex;
    align-items: center;
    padding: 8px 12px;
    font-size: 14px;
    font-weight: 500;
    color: var(--el-text-color-primary);
    border-bottom: 1px solid var(--el-border-color-light);

    .el-icon {
      margin-right: 8px;
    }
  }

  .language-item {
    display: flex;
    align-items: center;
    width: 100%;

    .language-icon {
      margin-right: 8px;
      font-size: 16px;
    }

    .language-name {
      flex-grow: 1;
    }

    .language-check {
      color: var(--el-color-primary);
    }
  }
}
</style>