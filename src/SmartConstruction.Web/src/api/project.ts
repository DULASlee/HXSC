import request from "../utils/request";
import type { Project } from "../types/global";
import type { PaginatedResponse } from "../types/global";

export function getProjects(params?: any) {
  return request<PaginatedResponse<Project>>({
    url: "/api/projects",
    method: "get",
    params,
  });
}

export function getProject(id: string) {
  return request<Project>({
    url: `/api/projects/${id}`,
    method: "get",
  });
}

export function createProject(data: Partial<Project>) {
  return request<Project>({
    url: "/api/projects",
    method: "post",
    data,
  });
}

export function updateProject(id: string, data: Partial<Project>) {
  return request<Project>({
    url: `/api/projects/${id}`,
    method: "put",
    data,
  });
}

export function deleteProject(id: string) {
  return request({
    url: `/api/projects/${id}`,
    method: "delete",
  });
}

export function checkProjectCode(projectCode: string, excludeId?: string) {
  return request<{ exists: boolean }>({
    url: "/api/projects/check-code",
    method: "get",
    params: { projectCode, excludeId },
  });
}

export function getProjectsByCompany(companyId: string, params?: any) {
  return request<PaginatedResponse<Project>>({
    url: `/api/projects/by-company/${companyId}`,
    method: "get",
    params,
  });
}