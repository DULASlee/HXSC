import request from "../utils/request";
import type { LoginParams, TokenInfo, User } from "../types/global";

export function login(data: LoginParams) {
  return request<{
    token: TokenInfo;
    user: User;
  }>({
    url: "/api/auth/login",
    method: "post",
    data,
  });
}

export function getInfo() {
  return request<{
    user: User;
    roles: any[];
    permissions: string[];
  }>({
    url: "/api/auth/user-info",
    method: "get",
  });
}

export function logout() {
  return request({
    url: "/api/auth/logout",
    method: "post",
  });
}

export function refreshToken(data: { refreshToken: string }) {
  return request<TokenInfo>({
    url: "/api/auth/refresh-token",
    method: "post",
    data,
  });
}
// 获取用户菜单
export function getUserMenus() {
  return request({
    url: "/api/menu/user-menus",
    method: "get",
  });
}

export function validateToken() {
  return request({
    url: "/api/auth/validate-token",
    method: "post",
  });
}

export function getServerTime() {
  return request<{
    serverTime: Date;
    utcTime: Date;
    timeZone: string;
    timestamp: number;
  }>({
    url: "/api/auth/server-time",
    method: "get",
  });
}

export function healthCheck() {
  return request<{
    status: string;
    timestamp: Date;
    version: string;
    environment: string;
    database: string;
  }>({
    url: "/api/auth/health",
    method: "get",
  });
}