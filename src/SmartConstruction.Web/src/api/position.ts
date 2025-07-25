import request from "../utils/request";
import type { Position } from "../types/global";
import type { PaginatedResponse } from "../types/global";

export function getPositions(params?: any) {
  return request<PaginatedResponse<Position>>({
    url: "/positions",
    method: "get",
    params,
  });
}

export function getPosition(id: string) {
  return request<Position>({
    url: `/positions/${id}`,
    method: "get",
  });
}

export function createPosition(data: Partial<Position>) {
  return request<Position>({
    url: "/positions",
    method: "post",
    data,
  });
}

export function updatePosition(id: string, data: Partial<Position>) {
  return request<Position>({
    url: `/positions/${id}`,
    method: "put",
    data,
  });
}

export function deletePosition(id: string) {
  return request({
    url: `/positions/${id}`,
    method: "delete",
  });
}
