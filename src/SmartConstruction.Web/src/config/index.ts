// 项目配置文件

// 接口基础路径
export const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL || "http://localhost:5000";

// WebSocket 地址
export const WS_URL = import.meta.env.VITE_WS_URL || "ws://localhost:5000/hubs";

// 项目标题
export const APP_TITLE = import.meta.env.VITE_APP_TITLE || "智慧工地管理系统";

// 本地存储前缀
export const STORAGE_PREFIX = "smart_construction_";

// Token 存储键名
export const TOKEN_KEY = `${STORAGE_PREFIX}token`;

// 用户信息存储键名
export const USER_INFO_KEY = `${STORAGE_PREFIX}user_info`;

// 请求超时时间 (毫秒)
export const REQUEST_TIMEOUT = 15000;

// 默认分页大小
export const DEFAULT_PAGE_SIZE = 10;

// 文件上传配置
export const UPLOAD_CONFIG = {
  // 文件大小限制 (MB)
  maxSize: 10,
  // 允许的文件类型
  allowTypes: [
    ".jpg",
    ".jpeg",
    ".png",
    ".gif",
    ".pdf",
    ".doc",
    ".docx",
    ".xls",
    ".xlsx",
  ],
};

// 日期格式化配置
export const DATE_FORMAT = {
  date: "YYYY-MM-DD",
  datetime: "YYYY-MM-DD HH:mm:ss",
  time: "HH:mm:ss",
};
