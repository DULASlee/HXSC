<template>
  <div class="tabs-container">
    <el-tabs
      v-model="appStore.activeTab"
      type="card"
      closable
      @tab-click="handleTabClick"
      @tab-remove="handleTabRemove"
    >
      <el-tab-pane
        v-for="tab in appStore.tabs"
        :key="tab.path"
        :name="tab.path"
        :closable="tab.closable !== false"
      >
        <template #label>
          <span
            @contextmenu.prevent="handleContextMenu($event, tab)"
            class="tab-label"
          >
            {{ i18nStore.t(`menu.${getMenuKey(tab.name)}`) || tab.title }}
          </span>
        </template>
      </el-tab-pane>
    </el-tabs>

    <!-- 右键菜单 -->
    <el-dropdown
      ref="contextMenuRef"
      trigger="contextmenu"
      :virtual-ref="contextMenuTarget"
      virtual-triggering
      @command="handleContextMenuCommand"
    >
      <template #dropdown>
        <el-dropdown-menu>
          <el-dropdown-item command="refresh">
            <el-icon><Refresh /></el-icon>
            {{ i18nStore.t("tabs.refresh") }}
          </el-dropdown-item>
          <el-dropdown-item command="closeOther">
            <el-icon><Close /></el-icon>
            {{ i18nStore.t("tabs.closeOther") }}
          </el-dropdown-item>
          <el-dropdown-item command="closeAll">
            <el-icon><CircleClose /></el-icon>
            {{ i18nStore.t("tabs.closeAll") }}
          </el-dropdown-item>
        </el-dropdown-menu>
      </template>
    </el-dropdown>
  </div>
</template>

<script setup lang="ts">
import { ref, nextTick } from "vue";
import { useRouter } from "vue-router";
import { useAppStore } from "@/stores/app";
import { useI18nStore } from "@/stores/i18n";
import type { TabsPaneContext } from "element-plus";

const router = useRouter();
const appStore = useAppStore();
const i18nStore = useI18nStore();

const contextMenuRef = ref();
const contextMenuTarget = ref();
const currentContextTab = ref("");

// 获取菜单键名
const getMenuKey = (name: string) => {
  const keyMap: Record<string, string> = {
    Dashboard: "dashboard",
    Project: "project",
    Worker: "worker",
    Attendance: "attendance",
    Monitoring: "monitoring",
    System: "system",
  };
  return keyMap[name] || name.toLowerCase();
};

// 获取标签页标题
const getTabTitle = (name: string) => {
  const tab = appStore.tabs.find((t) => t.name === name);
  return tab?.title || name;
};

// 标签页点击事件
const handleTabClick = (tab: TabsPaneContext) => {
  const tabPath = tab.props.name as string;
  if (tabPath !== router.currentRoute.value.path) {
    appStore.setActiveTab(tabPath);
    router.push(tabPath);
  }
};

// 标签页移除事件
const handleTabRemove = (tabPath: string) => {
  appStore.removeTab(tabPath);

  // 如果移除的是当前激活的标签页，跳转到新的激活标签页
  if (appStore.activeTab && appStore.tabs.length > 0) {
    router.push(appStore.activeTab);
  }
};

// 右键菜单事件
const handleContextMenu = (event: MouseEvent, tab: any) => {
  event.preventDefault();
  currentContextTab.value = tab.path;
  contextMenuTarget.value = event.target;

  nextTick(() => {
    contextMenuRef.value?.handleOpen();
  });
};

// 右键菜单命令处理
const handleContextMenuCommand = (command: string) => {
  switch (command) {
    case "refresh":
      // 刷新当前页面
      router.go(0);
      break;
    case "closeOther":
      appStore.closeOtherTabs(currentContextTab.value);
      if (appStore.activeTab !== currentContextTab.value) {
        router.push(currentContextTab.value);
      }
      break;
    case "closeAll":
      appStore.closeAllTabs();
      if (appStore.activeTab) {
        router.push(appStore.activeTab);
      }
      break;
  }
};
</script>

<style scoped>
.tabs-container {
  background-color: var(--app-content-bg);
  border-bottom: 1px solid var(--app-border-color);

  :deep(.el-tabs__header) {
    margin: 0;

    .el-tabs__nav-wrap {
      padding: 0 20px;
    }

    .el-tabs__nav {
      border: none;
    }

    .el-tabs__item {
      border: 1px solid var(--app-border-color);
      border-bottom: none;
      margin-right: 4px;
      background-color: var(--app-content-bg);
      color: var(--app-text-color-regular);

      &.is-active {
        background-color: var(--el-color-primary);
        color: #ffffff;
        border-color: var(--el-color-primary);
      }

      &:hover:not(.is-active) {
        background-color: var(--el-color-primary-light-9);
        color: var(--el-color-primary);
      }
    }
  }
}

.tab-label {
  display: inline-block;
  width: 100%;
  height: 100%;
}
</style>
