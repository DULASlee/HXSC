import request from "../utils/request";
import type { MetadataDefinition, Metadata } from "../types/global";

export function getMetadataDefinitions(params?: any) {
  return request<MetadataDefinition[]>({
    url: "/metadata/definitions",
    method: "get",
    params,
  });
}

export function createMetadataDefinition(data: MetadataDefinition) {
  return request<MetadataDefinition>({
    url: "/metadata/definitions",
    method: "post",
    data,
  });
}

export function updateMetadataDefinition(
  id: string,
  data: Partial<MetadataDefinition>,
) {
  return request<MetadataDefinition>({
    url: `/metadata/definitions/${id}`,
    method: "put",
    data,
  });
}

export function deleteMetadataDefinition(id: string) {
  return request({
    url: `/metadata/definitions/${id}`,
    method: "delete",
  });
}

export function getMetadata(
  entityType: string,
  entityId: string,
  metaKey?: string,
) {
  return request<Metadata | Metadata[]>({
    url: `/metadata/${entityType}/${entityId}${metaKey ? `/${metaKey}` : ""}`,
    method: "get",
  });
}

export function setMetadata(
  entityType: string,
  entityId: string,
  metaKey: string,
  value: any,
) {
  return request<Metadata>({
    url: `/metadata/${entityType}/${entityId}/${metaKey}`,
    method: "put",
    data: { value },
  });
}

export function deleteMetadata(
  entityType: string,
  entityId: string,
  metaKey: string,
) {
  return request({
    url: `/metadata/${entityType}/${entityId}/${metaKey}`,
    method: "delete",
  });
}
