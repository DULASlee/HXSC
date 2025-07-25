import { defineStore } from "pinia";
import { ref, computed } from "vue";

// 多语言配置
const messages = {
  "zh-CN": {
    // 通用
    common: {
      confirm: "确认",
      cancel: "取消",
      save: "保存",
      delete: "删除",
      edit: "编辑",
      add: "添加",
      search: "搜索",
      reset: "重置",
      submit: "提交",
      back: "返回",
      close: "关闭",
      refresh: "刷新",
      loading: "加载中...",
      noData: "暂无数据",
      operation: "操作",
      status: "状态",
      createTime: "创建时间",
      updateTime: "更新时间",
      remark: "备注",
    },
    // 菜单
    menu: {
      dashboard: "首页",
      project: "项目管理",
      worker: "工人管理",
      attendance: "考勤管理",
      monitoring: "监控管理",
      system: "系统设置",
    },
    // 系统
    system: {
      title: "智慧工地管理系统",
      welcome: "欢迎使用",
      logout: "退出登录",
      logoutConfirm: "确认退出系统吗?",
      profile: "个人信息",
      changePassword: "修改密码",
      settings: "系统设置",
      theme: "主题设置",
      language: "语言设置",
      lightTheme: "浅色主题",
      darkTheme: "深色主题",
      blueTheme: "蓝色主题",
      userManagement: "用户管理",
      permissionManagement: "权限管理",
      codeGeneration: "代码生成",
      tenantManagement: "租户管理",
      companyManagement: "公司管理",
      departmentManagement: "部门管理",
      positionManagement: "岗位管理",
      roleManagement: "角色管理",
      menuManagement: "菜单管理",
      rolePermission: "角色权限",
      moduleManagement: "模块管理",
      pageGeneration: "页面生成",
      dictionaryManagement: "字典管理",
    },
    // 标签页
    tabs: {
      closeOther: "关闭其他",
      closeAll: "关闭所有",
      refresh: "刷新页面",
    },
  },
  "en-US": {
    common: {
      confirm: "Confirm",
      cancel: "Cancel",
      save: "Save",
      delete: "Delete",
      edit: "Edit",
      add: "Add",
      search: "Search",
      reset: "Reset",
      submit: "Submit",
      back: "Back",
      close: "Close",
      refresh: "Refresh",
      loading: "Loading...",
      noData: "No Data",
      operation: "Operation",
      status: "Status",
      createTime: "Create Time",
      updateTime: "Update Time",
      remark: "Remark",
    },
    menu: {
      dashboard: "Dashboard",
      project: "Project Management",
      worker: "Worker Management",
      attendance: "Attendance Management",
      monitoring: "Monitoring Management",
      system: "System Settings",
    },
    system: {
      title: "Smart Construction Management System",
      welcome: "Welcome",
      logout: "Logout",
      logoutConfirm: "Are you sure to logout?",
      profile: "Profile",
      changePassword: "Change Password",
      settings: "Settings",
      theme: "Theme",
      language: "Language",
      lightTheme: "Light Theme",
      darkTheme: "Dark Theme",
      blueTheme: "Blue Theme",
      userManagement: "User Management",
      permissionManagement: "Permission Management",
      codeGeneration: "Code Generation",
      tenantManagement: "Tenant Management",
      companyManagement: "Company Management",
      departmentManagement: "Department Management",
      positionManagement: "Position Management",
      roleManagement: "Role Management",
      menuManagement: "Menu Management",
      rolePermission: "Role Permission",
      moduleManagement: "Module Management",
      pageGeneration: "Page Generation",
      dictionaryManagement: "Dictionary Management",
    },
    tabs: {
      closeOther: "Close Others",
      closeAll: "Close All",
      refresh: "Refresh",
    },
  },
  "ja-JP": {
    common: {
      confirm: "確認",
      cancel: "キャンセル",
      save: "保存",
      delete: "削除",
      edit: "編集",
      add: "追加",
      search: "検索",
      reset: "リセット",
      submit: "送信",
      back: "戻る",
      close: "閉じる",
      refresh: "更新",
      loading: "読み込み中...",
      noData: "データなし",
      operation: "操作",
      status: "ステータス",
      createTime: "作成時間",
      updateTime: "更新時間",
      remark: "備考",
    },
    menu: {
      dashboard: "ダッシュボード",
      project: "プロジェクト管理",
      worker: "作業員管理",
      attendance: "出勤管理",
      monitoring: "監視管理",
      system: "システム設定",
    },
    system: {
      title: "スマート建設管理システム",
      welcome: "ようこそ",
      logout: "ログアウト",
      logoutConfirm: "ログアウトしますか？",
      profile: "プロフィール",
      changePassword: "パスワード変更",
      settings: "設定",
      theme: "テーマ",
      language: "言語",
      lightTheme: "ライトテーマ",
      darkTheme: "ダークテーマ",
      blueTheme: "ブルーテーマ",
      userManagement: "ユーザー管理",
      permissionManagement: "権限管理",
      codeGeneration: "コード生成",
      tenantManagement: "テナント管理",
      companyManagement: "会社管理",
      departmentManagement: "部門管理",
      positionManagement: "職位管理",
      roleManagement: "ロール管理",
      menuManagement: "メニュー管理",
      rolePermission: "ロール権限",
      moduleManagement: "モジュール管理",
      pageGeneration: "ページ生成",
      dictionaryManagement: "辞書管理",
    },
    tabs: {
      closeOther: "他を閉じる",
      closeAll: "すべて閉じる",
      refresh: "ページ更新",
    },
  },
};

export const useI18nStore = defineStore("i18n", () => {
  const locale = ref<keyof typeof messages>("zh-CN");

  // 获取当前语言的翻译文本
  const t = computed(() => {
    return (key: string) => {
      const keys = key.split(".");
      let result: any = messages[locale.value];

      for (const k of keys) {
        result = result?.[k];
      }

      return result || key;
    };
  });

  // 设置语言
  const setLocale = (newLocale: keyof typeof messages) => {
    locale.value = newLocale;
  };

  return {
    locale,
    t,
    setLocale,
  };
});
