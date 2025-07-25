import { http } from "@/utils/http";
import type { Menu } from "@/types/models";

/**
 * 获取当前登录用户的菜单
 */
export function getUserMenus() {
  return http.get<Menu[]>("/api/menu/user-menus");
}

/**
 * 获取菜单树
 */
export function getMenuTree() {
  return http.get<Menu[]>("/api/menu/tree");
}
