import request from "../utils/request";
import type { Organization } from "../types/global";
import type { PaginatedResponse } from "../types/global";

export function getOrganizationTree() {
  return request<Organization[]>({
    url: "/api/organization/tree",
    method: "get",
  });
}

export function getOrganizations(params?: any) {
  return request<PaginatedResponse<Organization>>({
    url: "/api/organization",
    method: "get",
    params,
  });
}

export function getOrganization(id: string) {
  return request<Organization>({
    url: `/api/organization/${id}`,
    method: "get",
  });
}

export function createOrganization(data: Partial<Organization>) {
  return request<Organization>({
    url: "/api/organization",
    method: "post",
    data,
  });
}

export function updateOrganization(id: string, data: Partial<Organization>) {
  return request<Organization>({
    url: `/api/organization/${id}`,
    method: "put",
    data,
  });
}

export function deleteOrganization(id: string) {
  return request({
    url: `/api/organization/${id}`,
    method: "delete",
  });
}
