<template>
  <div class="system-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ i18nStore.t("menu.system") }}</span>
        </div>
      </template>

      <el-tabs v-model="activeTab" type="border-card">
        <!-- 基础设置 -->
        <el-tab-pane label="基础设置" name="basic">
          <div class="settings-section">
            <h3>{{ i18nStore.t("system.theme") }}</h3>
            <el-radio-group v-model="currentTheme" @change="handleThemeChange">
              <el-radio label="light">{{
                i18nStore.t("system.lightTheme")
              }}</el-radio>
              <el-radio label="dark">{{
                i18nStore.t("system.darkTheme")
              }}</el-radio>
              <el-radio label="blue">{{
                i18nStore.t("system.blueTheme")
              }}</el-radio>
            </el-radio-group>
          </div>

          <div class="settings-section">
            <h3>{{ i18nStore.t("system.language") }}</h3>
            <el-radio-group
              v-model="currentLanguage"
              @change="handleLanguageChange"
            >
              <el-radio label="zh-CN">🇨🇳 简体中文</el-radio>
              <el-radio label="en-US">🇺🇸 English</el-radio>
              <el-radio label="ja-JP">🇯🇵 日本語</el-radio>
            </el-radio-group>
          </div>
        </el-tab-pane>

        <!-- 字典管理 -->
        <el-tab-pane
          :label="i18nStore.t('system.dictionaryManagement')"
          name="dictionary"
        >
          <DictionaryManagement />
        </el-tab-pane>

        <!-- 用户管理 -->
        <el-tab-pane :label="i18nStore.t('system.userManagement')" name="users">
          <el-tabs v-model="userActiveTab" type="card">
            <el-tab-pane
              :label="i18nStore.t('system.tenantManagement')"
              name="tenant"
            >
              <TenantManagement />
            </el-tab-pane>
            <el-tab-pane
              :label="i18nStore.t('system.companyManagement')"
              name="company"
            >
              <CompanyManagement />
            </el-tab-pane>
            <el-tab-pane
              :label="i18nStore.t('system.departmentManagement')"
              name="department"
            >
              <DepartmentManagement />
            </el-tab-pane>
            <el-tab-pane
              :label="i18nStore.t('system.positionManagement')"
              name="position"
            >
              <PositionManagement />
            </el-tab-pane>
          </el-tabs>
        </el-tab-pane>

        <!-- 权限管理 -->
        <el-tab-pane
          :label="i18nStore.t('system.permissionManagement')"
          name="permissions"
        >
          <el-tabs v-model="permissionActiveTab" type="card">
            <el-tab-pane
              :label="i18nStore.t('system.roleManagement')"
              name="role"
            >
              <RoleManagement />
            </el-tab-pane>
            <el-tab-pane
              :label="i18nStore.t('system.menuManagement')"
              name="menu"
            >
              <MenuManagement />
            </el-tab-pane>
            <el-tab-pane
              :label="i18nStore.t('system.rolePermission')"
              name="rolePermission"
            >
              <RolePermissionManagement />
            </el-tab-pane>
          </el-tabs>
        </el-tab-pane>

        <!-- 代码生成 -->
        <el-tab-pane
          :label="i18nStore.t('system.codeGeneration')"
          name="codeGen"
        >
          <el-tabs v-model="codeGenActiveTab" type="card">
            <el-tab-pane
              :label="i18nStore.t('system.moduleManagement')"
              name="module"
            >
              <ModuleManagement />
            </el-tab-pane>
            <el-tab-pane
              :label="i18nStore.t('system.pageGeneration')"
              name="pageGen"
            >
              <PageGeneration />
            </el-tab-pane>
          </el-tabs>
        </el-tab-pane>
      </el-tabs>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from "vue";
import { useAppStore } from "@/stores/app";
import { useI18nStore } from "@/stores/i18n";
import type { ThemeType, LanguageType } from "@/stores/app";

// 导入子组件
import DictionaryManagement from "./components/DictionaryManagement.vue";
import TenantManagement from "./components/TenantManagement.vue";
import CompanyManagement from "./components/CompanyManagement.vue";
import DepartmentManagement from "./components/DepartmentManagement.vue";
import PositionManagement from "./components/PositionManagement.vue";
import RoleManagement from "./components/RoleManagement.vue";
import MenuManagement from "./components/MenuManagement.vue";
import RolePermissionManagement from "./components/RolePermissionManagement.vue";
import ModuleManagement from "./components/ModuleManagement.vue";
import PageGeneration from "./components/PageGeneration.vue";

const appStore = useAppStore();
const i18nStore = useI18nStore();

// 主标签页状态
const activeTab = ref("basic");

// 子标签页状态
const userActiveTab = ref("tenant");
const permissionActiveTab = ref("role");
const codeGenActiveTab = ref("module");

const currentTheme = computed({
  get: () => appStore.theme,
  set: (value: ThemeType) => appStore.setTheme(value),
});

const currentLanguage = computed({
  get: () => appStore.language,
  set: (value: LanguageType) => {
    appStore.setLanguage(value);
    i18nStore.setLocale(value);
  },
});

const handleThemeChange = (theme: ThemeType) => {
  appStore.setTheme(theme);
};

const handleLanguageChange = (language: LanguageType) => {
  appStore.setLanguage(language);
  i18nStore.setLocale(language);
};
</script>

<style scoped>
.system-container {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.settings-section {
  margin-bottom: 30px;
  padding: 20px;
  background-color: var(--app-border-color-extra-light);
  border-radius: 8px;
}

.settings-section h3 {
  margin: 0 0 15px 0;
  color: var(--app-text-color);
  font-size: 16px;
  font-weight: 500;
}

.content-placeholder {
  padding: 60px 0;
}

:deep(.el-radio-group) {
  .el-radio {
    margin-right: 20px;
    margin-bottom: 10px;
  }
}
</style>
