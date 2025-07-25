# 智能建造管理系统 - 后端API对接文档

## 📋 目录
- [基础信息](#基础信息)
- [认证与授权](#认证与授权)
- [用户管理](#用户管理)
- [角色权限管理](#角色权限管理)
- [项目管理](#项目管理)
- [设备管理](#设备管理)
- [工人管理](#工人管理)
- [考勤管理](#考勤管理)
- [安全管理](#安全管理)
- [数字孪生大屏](#数字孪生大屏)
- [错误码说明](#错误码说明)

---

## 🔧 基础信息

### 接口基础信息
- **基础URL**: `http://localhost:5000/api`
- **协议**: HTTP/HTTPS
- **数据格式**: JSON
- **字符编码**: UTF-8
- **认证方式**: Bearer Token (JWT)

### 通用响应格式
```json
{
  "success": true,
  "message": "操作成功",
  "data": {
    // 具体数据
  }
}
```

### 请求头
```http
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant-Id: {tenantId}
X-Device-Id: {deviceId}
```

---

## 🔐 认证与授权

### 1. 用户登录
**接口地址**: `POST /api/auth/login`

**请求参数**:
```json
{
  "username": "admin",
  "password": "123456",
  "tenantCode": "default",
  "deviceId": "device_001",
  "deviceType": "web"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "登录成功",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "refresh_token_here",
    "expiresIn": 3600,
    "tokenType": "Bearer",
    "user": {
      "id": "user_id",
      "username": "admin",
      "displayName": "系统管理员",
      "email": "admin@example.com",
      "roles": ["admin"],
      "permissions": ["user:read", "user:write"]
    }
  }
}
```

### 2. 刷新令牌
**接口地址**: `POST /api/auth/refresh-token`

**请求参数**:
```json
{
  "refreshToken": "refresh_token_here",
  "deviceId": "device_001"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "令牌刷新成功",
  "data": {
    "accessToken": "new_access_token",
    "refreshToken": "new_refresh_token",
    "expiresIn": 3600,
    "tokenType": "Bearer"
  }
}
```

### 3. 用户登出
**接口地址**: `POST /api/auth/logout`

**请求头**:
```http
Authorization: Bearer {token}
X-Device-Id: {deviceId}
```

**响应示例**:
```json
{
  "success": true,
  "message": "登出成功",
  "data": null
}
```

### 4. 获取当前用户信息
**接口地址**: `GET /api/auth/user-info`

**请求头**:
```http
Authorization: Bearer {token}
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "user_id",
    "username": "admin",
    "displayName": "系统管理员",
    "email": "admin@example.com",
    "mobile": "13800138000",
    "avatar": "avatar_url",
    "roles": [
      {
        "id": "role_id",
        "name": "管理员",
        "code": "admin"
      }
    ],
    "permissions": ["user:read", "user:write", "role:read"],
    "tenant": {
      "id": "tenant_id",
      "name": "默认租户",
      "code": "default"
    }
  }
}
```

### 5. 获取用户菜单
**接口地址**: `GET /api/auth/user-menus`

**请求头**:
```http
Authorization: Bearer {token}
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "menus": [
      {
        "id": "menu_id",
        "name": "系统管理",
        "path": "/system",
        "icon": "setting",
        "sort": 1,
        "children": [
          {
            "id": "submenu_id",
            "name": "用户管理",
            "path": "/system/user",
            "icon": "user",
            "sort": 1,
            "permission": "user:read"
          }
        ]
      }
    ]
  }
}
```

### 6. 验证令牌
**接口地址**: `POST /api/auth/validate-token`

**请求头**:
```http
Authorization: Bearer {token}
```

**响应示例**:
```json
{
  "success": true,
  "message": "令牌有效",
  "data": null
}
```

### 7. 获取服务器时间
**接口地址**: `GET /api/auth/server-time`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "serverTime": "2024-01-15T10:30:00",
    "utcTime": "2024-01-15T02:30:00Z",
    "timeZone": "中国标准时间",
    "timestamp": 1705312200
  }
}
```

---

## 👥 用户管理

### 1. 获取用户列表
**接口地址**: `GET /api/user`

**请求参数**:
```http
?page=1&size=10&keyword=admin&roleId=role_id&status=active
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "user_id",
        "username": "admin",
        "displayName": "系统管理员",
        "email": "admin@example.com",
        "mobile": "13800138000",
        "status": "active",
        "lastLoginTime": "2024-01-15T10:30:00",
        "createdAt": "2024-01-01T00:00:00"
      }
    ],
    "total": 100,
    "page": 1,
    "size": 10
  }
}
```

### 2. 获取用户详情
**接口地址**: `GET /api/user/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "user_id",
    "username": "admin",
    "displayName": "系统管理员",
    "email": "admin@example.com",
    "mobile": "13800138000",
    "status": "active",
    "roles": [
      {
        "id": "role_id",
        "name": "管理员",
        "code": "admin"
      }
    ],
    "permissions": ["user:read", "user:write"],
    "createdAt": "2024-01-01T00:00:00",
    "updatedAt": "2024-01-15T10:30:00"
  }
}
```

### 3. 创建用户
**接口地址**: `POST /api/user`

**请求参数**:
```json
{
  "username": "newuser",
  "password": "123456",
  "displayName": "新用户",
  "email": "newuser@example.com",
  "mobile": "13800138001",
  "roleIds": ["role_id_1", "role_id_2"],
  "status": "active"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": {
    "id": "new_user_id",
    "username": "newuser",
    "displayName": "新用户"
  }
}
```

### 4. 更新用户
**接口地址**: `PUT /api/user/{id}`

**请求参数**:
```json
{
  "displayName": "更新后的用户名",
  "email": "updated@example.com",
  "mobile": "13800138002",
  "roleIds": ["role_id_1"],
  "status": "active"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": {
    "id": "user_id",
    "displayName": "更新后的用户名"
  }
}
```

### 5. 删除用户
**接口地址**: `DELETE /api/user/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

---

## 🛡️ 角色权限管理

### 1. 获取权限树
**接口地址**: `GET /api/permission/tree`

**请求参数**:
```http
?tenantId=tenant_id
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": [
    {
      "id": "permission_id",
      "name": "用户管理",
      "code": "user",
      "type": "menu",
      "children": [
        {
          "id": "sub_permission_id",
          "name": "查看用户",
          "code": "user:read",
          "type": "permission"
        },
        {
          "id": "sub_permission_id_2",
          "name": "创建用户",
          "code": "user:create",
          "type": "permission"
        }
      ]
    }
  ]
}
```

### 2. 检查权限
**接口地址**: `POST /api/permission/check`

**请求参数**:
```json
{
  "permissionCode": "user:read",
  "context": {
    "resourceId": "user_id",
    "tenantId": "tenant_id"
  }
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "检查完成",
  "data": true
}
```

### 3. 检查任意权限
**接口地址**: `POST /api/permission/check-any`

**请求参数**:
```json
{
  "permissionCodes": ["user:read", "user:write", "role:read"]
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "检查完成",
  "data": true
}
```

### 4. 检查所有权限
**接口地址**: `POST /api/permission/check-all`

**请求参数**:
```json
{
  "permissionCodes": ["user:read", "user:write"]
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "检查完成",
  "data": true
}
```

### 5. 获取用户权限
**接口地址**: `GET /api/permission/user-permissions`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": [
    "user:read",
    "user:write",
    "role:read",
    "role:write"
  ]
}
```

### 6. 为角色分配权限
**接口地址**: `POST /api/permission/assign-to-role/{roleId}`

**请求参数**:
```json
{
  "permissionIds": ["permission_id_1", "permission_id_2"]
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "分配成功",
  "data": null
}
```

### 7. 为用户分配权限
**接口地址**: `POST /api/permission/assign-to-user/{userId}`

**请求参数**:
```json
{
  "permissionIds": ["permission_id_1", "permission_id_2"]
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "分配成功",
  "data": null
}
```

### 8. 创建权限
**接口地址**: `POST /api/permission`

**请求参数**:
```json
{
  "name": "新权限",
  "code": "new:permission",
  "type": "permission",
  "parentId": "parent_permission_id",
  "description": "权限描述"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": "new_permission_id"
}
```

### 9. 更新权限
**接口地址**: `PUT /api/permission/{id}`

**请求参数**:
```json
{
  "name": "更新后的权限名",
  "code": "updated:permission",
  "description": "更新后的描述"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": null
}
```

### 10. 删除权限
**接口地址**: `DELETE /api/permission/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

### 11. 获取角色列表
**接口地址**: `GET /api/role`

**请求参数**:
```http
?page=1&size=10&keyword=admin&status=active
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "role_id",
        "name": "管理员",
        "code": "admin",
        "description": "系统管理员角色",
        "status": "active",
        "userCount": 5,
        "createdAt": "2024-01-01T00:00:00"
      }
    ],
    "total": 10,
    "page": 1,
    "size": 10
  }
}
```

### 12. 获取角色详情
**接口地址**: `GET /api/role/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "role_id",
    "name": "管理员",
    "code": "admin",
    "description": "系统管理员角色",
    "status": "active",
    "permissions": [
      {
        "id": "permission_id",
        "name": "用户管理",
        "code": "user:read"
      }
    ],
    "users": [
      {
        "id": "user_id",
        "username": "admin",
        "displayName": "系统管理员"
      }
    ],
    "createdAt": "2024-01-01T00:00:00",
    "updatedAt": "2024-01-15T10:30:00"
  }
}
```

### 13. 创建角色
**接口地址**: `POST /api/role`

**请求参数**:
```json
{
  "name": "新角色",
  "code": "new_role",
  "description": "新角色描述",
  "permissionIds": ["permission_id_1", "permission_id_2"],
  "status": "active"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": {
    "id": "new_role_id",
    "name": "新角色"
  }
}
```

### 14. 更新角色
**接口地址**: `PUT /api/role/{id}`

**请求参数**:
```json
{
  "name": "更新后的角色名",
  "description": "更新后的描述",
  "permissionIds": ["permission_id_1"],
  "status": "active"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": {
    "id": "role_id",
    "name": "更新后的角色名"
  }
}
```

### 15. 删除角色
**接口地址**: `DELETE /api/role/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

---

## 🏗️ 项目管理

### 1. 获取项目列表
**接口地址**: `GET /api/project`

**请求参数**:
```http
?page=1&size=10&keyword=项目名&status=active&companyId=company_id
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "project_id",
        "projectCode": "PRJ001",
        "projectName": "智慧工地示范项目",
        "projectAddress": "北京市朝阳区",
        "projectManager": "张三",
        "startDate": "2024-01-01",
        "endDate": "2024-12-31",
        "status": "active",
        "progress": 65.5,
        "company": {
          "id": "company_id",
          "companyName": "建设公司"
        }
      }
    ],
    "total": 50,
    "page": 1,
    "size": 10
  }
}
```

### 2. 获取项目详情
**接口地址**: `GET /api/project/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "project_id",
    "projectCode": "PRJ001",
    "projectName": "智慧工地示范项目",
    "projectAddress": "北京市朝阳区",
    "projectManager": "张三",
    "startDate": "2024-01-01",
    "endDate": "2024-12-31",
    "status": "active",
    "progress": 65.5,
    "investment": 100000000,
    "description": "项目描述",
    "company": {
      "id": "company_id",
      "companyName": "建设公司"
    },
    "teams": [
      {
        "id": "team_id",
        "name": "施工队A",
        "specialty": "土建"
      }
    ],
    "workers": [
      {
        "id": "worker_id",
        "fullName": "李四",
        "specialty": "木工"
      }
    ],
    "devices": [
      {
        "id": "device_id",
        "deviceName": "监控摄像头",
        "deviceType": "camera"
      }
    ]
  }
}
```

### 3. 创建项目
**接口地址**: `POST /api/project`

**请求参数**:
```json
{
  "projectCode": "PRJ002",
  "projectName": "新项目",
  "projectAddress": "项目地址",
  "projectManager": "项目经理",
  "startDate": "2024-02-01",
  "endDate": "2024-12-31",
  "investment": 50000000,
  "description": "项目描述",
  "companyId": "company_id"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": {
    "id": "new_project_id",
    "projectName": "新项目"
  }
}
```

### 4. 更新项目
**接口地址**: `PUT /api/project/{id}`

**请求参数**:
```json
{
  "projectName": "更新后的项目名",
  "projectAddress": "更新后的地址",
  "projectManager": "新的项目经理",
  "status": "active",
  "progress": 70.0
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": {
    "id": "project_id",
    "projectName": "更新后的项目名"
  }
}
```

### 5. 删除项目
**接口地址**: `DELETE /api/project/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

---

## 📱 设备管理

### 1. 获取设备列表
**接口地址**: `GET /api/device`

**请求参数**:
```http
?page=1&size=10&keyword=设备名&deviceType=camera&projectId=project_id&status=online
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "device_id",
        "deviceCode": "DEV001",
        "deviceName": "监控摄像头",
        "deviceType": "camera",
        "model": "HD-1080P",
        "manufacturer": "海康威视",
        "status": "online",
        "location": "工地大门",
        "ipAddress": "192.168.1.100",
        "lastHeartbeat": "2024-01-15T10:30:00",
        "project": {
          "id": "project_id",
          "projectName": "智慧工地项目"
        }
      }
    ],
    "total": 100,
    "page": 1,
    "size": 10
  }
}
```

### 2. 获取设备详情
**接口地址**: `GET /api/device/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "device_id",
    "deviceCode": "DEV001",
    "deviceName": "监控摄像头",
    "deviceType": "camera",
    "model": "HD-1080P",
    "manufacturer": "海康威视",
    "status": "online",
    "location": "工地大门",
    "ipAddress": "192.168.1.100",
    "macAddress": "00:11:22:33:44:55",
    "lastHeartbeat": "2024-01-15T10:30:00",
    "maintenanceRecords": [
      {
        "id": "record_id",
        "maintenanceType": "定期保养",
        "maintenanceDate": "2024-01-10",
        "description": "清洁镜头"
      }
    ],
    "project": {
      "id": "project_id",
      "projectName": "智慧工地项目"
    }
  }
}
```

### 3. 创建设备
**接口地址**: `POST /api/device`

**请求参数**:
```json
{
  "deviceCode": "DEV002",
  "deviceName": "新设备",
  "deviceType": "sensor",
  "model": "TEMP-001",
  "manufacturer": "传感器厂商",
  "location": "工地现场",
  "ipAddress": "192.168.1.101",
  "projectId": "project_id"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": {
    "id": "new_device_id",
    "deviceName": "新设备"
  }
}
```

### 4. 更新设备
**接口地址**: `PUT /api/device/{id}`

**请求参数**:
```json
{
  "deviceName": "更新后的设备名",
  "location": "新的位置",
  "status": "online"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": {
    "id": "device_id",
    "deviceName": "更新后的设备名"
  }
}
```

### 5. 删除设备
**接口地址**: `DELETE /api/device/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

---

## 👷 工人管理

### 1. 获取工人列表
**接口地址**: `GET /api/worker`

**请求参数**:
```http
?page=1&size=10&keyword=工人名&specialty=木工&projectId=project_id&teamId=team_id
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "worker_id",
        "workerCode": "WRK001",
        "fullName": "张三",
        "idCardNumber": "110101199001011234",
        "gender": "male",
        "birthDate": "1990-01-01",
        "phoneNumber": "13800138000",
        "specialty": "木工",
        "skillLevel": "高级",
        "status": "active",
        "team": {
          "id": "team_id",
          "name": "木工队"
        },
        "project": {
          "id": "project_id",
          "projectName": "智慧工地项目"
        }
      }
    ],
    "total": 200,
    "page": 1,
    "size": 10
  }
}
```

### 2. 获取工人详情
**接口地址**: `GET /api/worker/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "worker_id",
    "workerCode": "WRK001",
    "fullName": "张三",
    "idCardNumber": "110101199001011234",
    "gender": "male",
    "birthDate": "1990-01-01",
    "phoneNumber": "13800138000",
    "specialty": "木工",
    "skillLevel": "高级",
    "status": "active",
    "address": "北京市朝阳区",
    "emergencyContact": "李四",
    "emergencyPhone": "13900139000",
    "attendanceProfile": {
      "faceImage": "face_image_url",
      "idCardFrontImg": "id_front_url",
      "idCardBackImg": "id_back_url",
      "contractImg": "contract_url",
      "trainingCertImg": "training_cert_url",
      "healthCertImg": "health_cert_url"
    },
    "team": {
      "id": "team_id",
      "name": "木工队"
    },
    "project": {
      "id": "project_id",
      "projectName": "智慧工地项目"
    }
  }
}
```

### 3. 创建工人
**接口地址**: `POST /api/worker`

**请求参数**:
```json
{
  "workerCode": "WRK002",
  "fullName": "新工人",
  "idCardNumber": "110101199001011235",
  "gender": "male",
  "birthDate": "1990-01-02",
  "phoneNumber": "13800138001",
  "specialty": "电工",
  "skillLevel": "中级",
  "address": "北京市朝阳区",
  "emergencyContact": "紧急联系人",
  "emergencyPhone": "13900139001",
  "teamId": "team_id",
  "projectId": "project_id"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": {
    "id": "new_worker_id",
    "fullName": "新工人"
  }
}
```

### 4. 更新工人
**接口地址**: `PUT /api/worker/{id}`

**请求参数**:
```json
{
  "fullName": "更新后的工人名",
  "phoneNumber": "13800138002",
  "specialty": "更新后的工种",
  "skillLevel": "高级",
  "status": "active"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": {
    "id": "worker_id",
    "fullName": "更新后的工人名"
  }
}
```

### 5. 删除工人
**接口地址**: `DELETE /api/worker/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

---

## 📊 考勤管理

### 1. 获取考勤记录
**接口地址**: `GET /api/attendance`

**请求参数**:
```http
?page=1&size=10&projectId=project_id&workerId=worker_id&teamId=team_id&startDate=2024-01-01&endDate=2024-01-15&attendanceType=checkin
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "attendance_id",
        "worker": {
          "id": "worker_id",
          "fullName": "张三",
          "workerCode": "WRK001"
        },
        "project": {
          "id": "project_id",
          "projectName": "智慧工地项目"
        },
        "team": {
          "id": "team_id",
          "name": "木工队"
        },
        "attendanceType": "checkin",
        "attendanceTime": "2024-01-15T08:00:00",
        "deviceId": "device_id",
        "location": "工地大门",
        "source": "face_recognition",
        "status": "normal"
      }
    ],
    "total": 1000,
    "page": 1,
    "size": 10
  }
}
```

### 2. 获取考勤统计
**接口地址**: `GET /api/attendance/statistics`

**请求参数**:
```http
?projectId=project_id&teamId=team_id&date=2024-01-15
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "totalWorkers": 100,
    "presentWorkers": 95,
    "absentWorkers": 3,
    "lateWorkers": 2,
    "attendanceRate": 95.0,
    "onTimeRate": 93.0,
    "statistics": {
      "checkin": {
        "total": 95,
        "onTime": 93,
        "late": 2
      },
      "checkout": {
        "total": 95,
        "onTime": 90,
        "early": 5
      }
    }
  }
}
```

### 3. 获取考勤趋势
**接口地址**: `GET /api/attendance/trends`

**请求参数**:
```http
?projectId=project_id&startDate=2024-01-01&endDate=2024-01-15&type=daily
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "trends": [
      {
        "date": "2024-01-01",
        "totalWorkers": 100,
        "presentWorkers": 95,
        "attendanceRate": 95.0
      },
      {
        "date": "2024-01-02",
        "totalWorkers": 100,
        "presentWorkers": 98,
        "attendanceRate": 98.0
      }
    ]
  }
}
```

---

## 🚨 安全管理

### 1. 获取安全事故列表
**接口地址**: `GET /api/safety-incident`

**请求参数**:
```http
?page=1&size=10&projectId=project_id&severity=high&status=open&startDate=2024-01-01&endDate=2024-01-15
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "items": [
      {
        "id": "incident_id",
        "incidentCode": "SAF001",
        "title": "高空作业未系安全带",
        "description": "工人在3楼作业时未系安全带",
        "severity": "high",
        "status": "open",
        "incidentDate": "2024-01-15T10:30:00",
        "location": "3楼施工现场",
        "reporter": "安全员",
        "project": {
          "id": "project_id",
          "projectName": "智慧工地项目"
        }
      }
    ],
    "total": 50,
    "page": 1,
    "size": 10
  }
}
```

### 2. 获取安全事故详情
**接口地址**: `GET /api/safety-incident/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "id": "incident_id",
    "incidentCode": "SAF001",
    "title": "高空作业未系安全带",
    "description": "工人在3楼作业时未系安全带",
    "severity": "high",
    "status": "open",
    "incidentDate": "2024-01-15T10:30:00",
    "location": "3楼施工现场",
    "reporter": "安全员",
    "assignedTo": "安全主管",
    "resolution": "已要求工人系好安全带",
    "resolutionDate": "2024-01-15T11:00:00",
    "images": ["image1_url", "image2_url"],
    "project": {
      "id": "project_id",
      "projectName": "智慧工地项目"
    }
  }
}
```

### 3. 创建安全事故
**接口地址**: `POST /api/safety-incident`

**请求参数**:
```json
{
  "title": "新安全事故",
  "description": "事故描述",
  "severity": "medium",
  "incidentDate": "2024-01-15T10:30:00",
  "location": "事故地点",
  "reporter": "报告人",
  "projectId": "project_id"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "创建成功",
  "data": {
    "id": "new_incident_id",
    "title": "新安全事故"
  }
}
```

### 4. 更新安全事故
**接口地址**: `PUT /api/safety-incident/{id}`

**请求参数**:
```json
{
  "title": "更新后的事故标题",
  "status": "resolved",
  "assignedTo": "新的负责人",
  "resolution": "解决方案",
  "resolutionDate": "2024-01-15T12:00:00"
}
```

**响应示例**:
```json
{
  "success": true,
  "message": "更新成功",
  "data": {
    "id": "incident_id",
    "title": "更新后的事故标题"
  }
}
```

### 5. 删除安全事故
**接口地址**: `DELETE /api/safety-incident/{id}`

**响应示例**:
```json
{
  "success": true,
  "message": "删除成功",
  "data": null
}
```

---

## 🖥️ 数字孪生大屏

### 1. 获取指挥中心总览
**接口地址**: `GET /api/digital-twin/command-center/overview`

**请求参数**:
```http
?projectId=project_id
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "projectSummary": {
      "totalProjects": 15,
      "activeProjects": 12,
      "completedProjects": 3,
      "totalInvestment": 2800000000
    },
    "personnelSummary": {
      "totalPersonnel": 1850,
      "onSitePersonnel": 1245,
      "offSitePersonnel": 605,
      "attendanceRate": 92.5
    },
    "equipmentSummary": {
      "totalEquipment": 267,
      "onlineEquipment": 234,
      "offlineEquipment": 21,
      "faultEquipment": 12,
      "onlineRate": 87.6
    },
    "safetySummary": {
      "totalIncidents": 23,
      "pendingIncidents": 5,
      "resolvedIncidents": 18,
      "safetyScore": 89.2
    }
  }
}
```

### 2. 获取项目列表及状态
**接口地址**: `GET /api/digital-twin/command-center/projects`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": [
    {
      "id": "project_id",
      "name": "智慧工地示范项目A区",
      "status": "active",
      "progress": 65.8,
      "location": {
        "lng": 116.397459,
        "lat": 39.909042,
        "address": "北京市朝阳区建国路88号"
      },
      "personnel": {
        "total": 245,
        "onSite": 189
      },
      "equipment": {
        "total": 32,
        "online": 28
      },
      "startDate": "2024-01-15",
      "endDate": "2024-12-31",
      "investment": 150000000
    }
  ]
}
```

### 3. 获取实时数据统计
**接口地址**: `GET /api/digital-twin/command-center/realtime-stats`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "currentTime": "2024-01-15T10:30:00",
    "onlineUsers": 156,
    "activeDevices": 234,
    "todayAttendance": 1245,
    "todayIncidents": 2,
    "systemStatus": "normal",
    "alerts": [
      {
        "id": "alert_id",
        "type": "safety",
        "level": "warning",
        "message": "检测到未戴安全帽人员",
        "time": "2024-01-15T10:25:00"
      }
    ]
  }
}
```

### 4. 获取考勤总览统计
**接口地址**: `GET /api/digital-twin/attendance/overview`

**请求参数**:
```http
?projectId=project_id&date=2024-01-15
```

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "date": "2024-01-15",
    "totalWorkers": 245,
    "presentWorkers": 235,
    "absentWorkers": 8,
    "lateWorkers": 2,
    "attendanceRate": 95.9,
    "onTimeRate": 95.1,
    "checkinStats": {
      "total": 235,
      "onTime": 223,
      "late": 12
    },
    "checkoutStats": {
      "total": 235,
      "onTime": 228,
      "early": 7
    }
  }
}
```

### 5. 获取实时考勤动态
**接口地址**: `GET /api/digital-twin/attendance/realtime`

**响应示例**:
```json
{
  "success": true,
  "message": "获取成功",
  "data": {
    "recentRecords": [
      {
        "id": "record_id",
        "workerName": "张三",
        "workerCode": "WRK001",
        "teamName": "木工队",
        "attendanceType": "checkin",
        "attendanceTime": "2024-01-15T08:05:00",
        "status": "late",
        "location": "工地大门"
      }
    ],
    "statistics": {
      "lastHour": {
        "checkin": 45,
        "checkout": 12
      },
      "today": {
        "checkin": 235,
        "checkout": 0
      }
    }
  }
}
```

---

## ❌ 错误码说明

### HTTP状态码
- `200` - 请求成功
- `201` - 创建成功
- `400` - 请求参数错误
- `401` - 未授权（需要登录）
- `403` - 禁止访问（权限不足）
- `404` - 资源不存在
- `409` - 资源冲突
- `422` - 请求格式正确但语义错误
- `500` - 服务器内部错误

### 业务错误码
```json
{
  "success": false,
  "message": "错误描述",
  "errorCode": "ERROR_CODE",
  "data": null
}
```

### 常见错误码
- `AUTH_INVALID_CREDENTIALS` - 用户名或密码错误
- `AUTH_TOKEN_EXPIRED` - 令牌已过期
- `AUTH_TOKEN_INVALID` - 令牌无效
- `AUTH_PERMISSION_DENIED` - 权限不足
- `USER_NOT_FOUND` - 用户不存在
- `USER_ALREADY_EXISTS` - 用户已存在
- `ROLE_NOT_FOUND` - 角色不存在
- `PERMISSION_NOT_FOUND` - 权限不存在
- `PROJECT_NOT_FOUND` - 项目不存在
- `DEVICE_NOT_FOUND` - 设备不存在
- `WORKER_NOT_FOUND` - 工人不存在
- `VALIDATION_ERROR` - 参数验证失败
- `INTERNAL_ERROR` - 服务器内部错误

### 错误响应示例
```json
{
  "success": false,
  "message": "用户名或密码错误",
  "errorCode": "AUTH_INVALID_CREDENTIALS",
  "data": null
}
```

---

## 📝 使用说明

### 1. 认证流程
1. 调用登录接口获取 `accessToken` 和 `refreshToken`
2. 在后续请求中使用 `Authorization: Bearer {accessToken}` 头
3. 当 `accessToken` 过期时，使用 `refreshToken` 刷新
4. 登出时调用登出接口清除令牌

### 2. 权限控制
1. 系统基于RBAC（基于角色的访问控制）模型
2. 用户通过角色获得权限
3. 可以单独为用户分配权限
4. 权限检查支持上下文参数

### 3. 分页查询
- 使用 `page` 和 `size` 参数进行分页
- 响应中包含 `total`、`page`、`size` 信息
- 默认 `page=1`，`size=10`

### 4. 数据过滤
- 使用查询参数进行数据过滤
- 支持多条件组合查询
- 日期范围使用 `startDate` 和 `endDate` 参数

### 5. 文件上传
- 支持图片、文档等文件上传
- 使用 `multipart/form-data` 格式
- 文件大小限制为 10MB

---

## 🔄 更新日志

### v1.0.0 (2024-01-15)
- 初始版本发布
- 支持用户认证和权限管理
- 支持项目管理、设备管理、工人管理
- 支持考勤管理和安全管理
- 支持数字孪生大屏数据接口

---

## 📞 技术支持

如有问题，请联系技术支持团队：
- 邮箱：support@smartconstruction.com
- 电话：400-123-4567
- 工作时间：周一至周五 9:00-18:00 