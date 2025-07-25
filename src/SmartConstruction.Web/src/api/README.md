# API 服务使用指南

本文档提供了如何使用统一API服务层的示例和最佳实践。

## 基本用法

### 导入API服务

```typescript
// 方式1：导入特定服务
import { authService, userService } from '@/api';

// 方式2：导入所有服务
import apiServices from '@/api';
const { auth, user } = apiServices;
```

### 认证相关

```typescript
// 用户登录
const login = async () => {
  try {
    const response = await authService.login({
      username: 'admin',
      password: '123456',
      tenantCode: 'system'
    });
    
    if (response.success) {
      // 登录成功，处理返回数据
      const { accessToken, user } = response.data;
      // 存储token和用户信息
    }
  } catch (error) {
    console.error('登录失败:', error);
  }
};

// 获取当前用户信息
const getCurrentUser = async () => {
  try {
    const response = await authService.getCurrentUser();
    if (response.success) {
      // 处理用户信息
      const user = response.data;
    }
  } catch (error) {
    console.error('获取用户信息失败:', error);
  }
};

// 退出登录
const logout = async () => {
  try {
    await authService.logout();
    // 清除本地存储的用户信息和token
  } catch (error) {
    console.error('退出登录失败:', error);
  }
};
```

### 用户管理

```typescript
// 获取用户列表
const getUserList = async () => {
  try {
    const response = await userService.getList({
      pageNumber: 1,
      pageSize: 10,
      keyword: '搜索关键词'
    });
    
    if (response.success) {
      const { items, totalCount } = response.data;
      // 处理用户列表数据
    }
  } catch (error) {
    console.error('获取用户列表失败:', error);
  }
};

// 创建用户
const createUser = async () => {
  try {
    const response = await userService.create({
      username: 'newuser',
      password: 'password123',
      displayName: '新用户',
      email: 'newuser@example.com',
      organizationId: 'org-001',
      roleIds: ['role-001', 'role-002']
    });
    
    if (response.success) {
      // 用户创建成功
    }
  } catch (error) {
    console.error('创建用户失败:', error);
  }
};

// 更新用户
const updateUser = async (userId: string) => {
  try {
    const response = await userService.update(userId, {
      displayName: '更新的用户名',
      email: 'updated@example.com'
    });
    
    if (response.success) {
      // 用户更新成功
    }
  } catch (error) {
    console.error('更新用户失败:', error);
  }
};

// 删除用户
const deleteUser = async (userId: string) => {
  try {
    const response = await userService.delete(userId);
    if (response.success) {
      // 用户删除成功
    }
  } catch (error) {
    console.error('删除用户失败:', error);
  }
};
```

### 角色管理

```typescript
// 获取角色列表
const getRoleList = async () => {
  try {
    const response = await roleService.getList();
    if (response.success) {
      // 处理角色列表
    }
  } catch (error) {
    console.error('获取角色列表失败:', error);
  }
};

// 为角色分配权限
const assignPermissions = async (roleId: string) => {
  try {
    const response = await roleService.assignPermissions(roleId, {
      permissionIds: ['perm-001', 'perm-002', 'perm-003']
    });
    
    if (response.success) {
      // 权限分配成功
    }
  } catch (error) {
    console.error('分配权限失败:', error);
  }
};
```

### 组织管理

```typescript
// 获取组织树
const getOrganizationTree = async () => {
  try {
    const response = await organizationService.getTree();
    if (response.success) {
      // 处理组织树数据
      const orgTree = response.data;
    }
  } catch (error) {
    console.error('获取组织树失败:', error);
  }
};
```

### 租户管理

```typescript
// 获取租户列表
const getTenantList = async () => {
  try {
    const response = await tenantService.getList({
      pageNumber: 1,
      pageSize: 20
    });
    
    if (response.success) {
      // 处理租户列表
    }
  } catch (error) {
    console.error('获取租户列表失败:', error);
  }
};

// 激活租户
const activateTenant = async (tenantId: string) => {
  try {
    const response = await tenantService.activate(tenantId);
    if (response.success) {
      // 租户激活成功
    }
  } catch (error) {
    console.error('激活租户失败:', error);
  }
};
```

### 系统管理

```typescript
// 获取系统信息
const getSystemInfo = async () => {
  try {
    const response = await systemService.getInfo();
    if (response.success) {
      // 处理系统信息
      const systemInfo = response.data;
    }
  } catch (error) {
    console.error('获取系统信息失败:', error);
  }
};

// 清除缓存
const clearCache = async () => {
  try {
    const response = await systemService.clearCache();
    if (response.success) {
      // 缓存清除成功
    }
  } catch (error) {
    console.error('清除缓存失败:', error);
  }
};
```

## 扩展API服务

如果需要添加新的API服务，可以按照以下步骤进行：

1. 在 `src/types/api.ts` 中定义相关的类型
2. 创建新的服务类文件 `src/api/services/your-service.service.ts`
3. 在 `src/api/services/index.ts` 中导出新服务
4. 使用新服务

### 示例：添加新的文件服务

```typescript
// 1. 在 src/types/api.ts 中添加类型
export interface FileUploadResult {
  url: string;
  filename: string;
  size: number;
  mimeType: string;
}

// 2. 创建服务类文件 src/api/services/file.service.ts
import { request } from '../request';
import type { ApiResponse } from '@/types/global';
import type { FileUploadResult } from '@/types/api';

export class FileService {
  private baseUrl = '/api/files';

  async upload(file: File): Promise<ApiResponse<FileUploadResult>> {
    const formData = new FormData();
    formData.append('file', file);
    
    return request.post<FileUploadResult>(`${this.baseUrl}/upload`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
  }

  async download(fileId: string): Promise<Blob> {
    return request.get(`${this.baseUrl}/download/${fileId}`, {}, {
      responseType: 'blob'
    });
  }
}

// 3. 在 src/api/services/index.ts 中导出
export const fileService = new FileService();
export type { FileService };
apiServices.file = fileService;

// 4. 使用新服务
import { fileService } from '@/api';

const uploadFile = async (file: File) => {
  try {
    const response = await fileService.upload(file);
    if (response.success) {
      const { url } = response.data;
      // 处理上传结果
    }
  } catch (error) {
    console.error('文件上传失败:', error);
  }
};
```