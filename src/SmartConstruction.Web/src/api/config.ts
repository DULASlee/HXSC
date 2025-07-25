// =============================================
// API配置管理
// =============================================

// API端点配置接口
export interface ApiEndpoint {
  name: string;
  path: string;
  method: 'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH';
  description?: string;
  params?: Record<string, any>;
  body?: Record<string, any>;
  response?: Record<string, any>;
}

// API模块配置接口
export interface ApiModule {
  name: string;
  basePath: string;
  endpoints: ApiEndpoint[];
}

// 完整的API配置
export interface ApiConfig {
  baseURL: string;
  timeout: number;
  modules: ApiModule[];
}

// 默认API配置
export const defaultApiConfig: ApiConfig = {
  baseURL: '',
  timeout: 30000,
  modules: [
    {
      name: 'auth',
      basePath: '/api/auth',
      endpoints: [
        {
          name: 'login',
          path: '/login',
          method: 'POST',
          description: '用户登录',
          body: {
            username: 'string',
            password: 'string',
            tenantCode: 'string',
            rememberMe: 'boolean?'
          },
          response: {
            success: 'boolean',
            data: {
              user: 'User',
              token: {
                accessToken: 'string',
                refreshToken: 'string',
                expiresIn: 'number'
              },
              permissions: 'string[]',
              menus: 'Menu[]'
            }
          }
        },
        {
          name: 'logout',
          path: '/logout',
          method: 'POST',
          description: '用户登出'
        },
        {
          name: 'getCurrentUser',
          path: '/user-info',
          method: 'GET',
          description: '获取当前用户信息'
        },
        {
          name: 'refreshToken',
          path: '/refresh-token',
          method: 'POST',
          description: '刷新令牌'
        },
        {
          name: 'changePassword',
          path: '/change-password',
          method: 'POST',
          description: '修改密码'
        },
        {
          name: 'validateToken',
          path: '/validate',
          method: 'GET',
          description: '验证令牌'
        }
      ]
    },
    {
      name: 'users',
      basePath: '/api/user',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取用户列表'
        },
        {
          name: 'getById',
          path: '/{id}',
          method: 'GET',
          description: '获取用户详情'
        },
        {
          name: 'create',
          path: '',
          method: 'POST',
          description: '创建用户'
        },
        {
          name: 'update',
          path: '/{id}',
          method: 'PUT',
          description: '更新用户'
        },
        {
          name: 'delete',
          path: '/{id}',
          method: 'DELETE',
          description: '删除用户'
        },
        {
          name: 'assignRoles',
          path: '/{id}/roles',
          method: 'POST',
          description: '分配角色'
        }
      ]
    },
    {
      name: 'roles',
      basePath: '/api/roles',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取角色列表'
        },
        {
          name: 'getById',
          path: '/{id}',
          method: 'GET',
          description: '获取角色详情'
        },
        {
          name: 'create',
          path: '',
          method: 'POST',
          description: '创建角色'
        },
        {
          name: 'update',
          path: '/{id}',
          method: 'PUT',
          description: '更新角色'
        },
        {
          name: 'delete',
          path: '/{id}',
          method: 'DELETE',
          description: '删除角色'
        },
        {
          name: 'assignPermissions',
          path: '/{id}/permissions',
          method: 'POST',
          description: '分配权限'
        }
      ]
    },
    {
      name: 'organizations',
      basePath: '/api/organizations',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取组织列表'
        },
        {
          name: 'getTree',
          path: '/tree',
          method: 'GET',
          description: '获取组织树'
        },
        {
          name: 'getById',
          path: '/{id}',
          method: 'GET',
          description: '获取组织详情'
        },
        {
          name: 'create',
          path: '',
          method: 'POST',
          description: '创建组织'
        },
        {
          name: 'update',
          path: '/{id}',
          method: 'PUT',
          description: '更新组织'
        },
        {
          name: 'delete',
          path: '/{id}',
          method: 'DELETE',
          description: '删除组织'
        }
      ]
    },
    {
      name: 'tenants',
      basePath: '/api/tenants',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取租户列表'
        },
        {
          name: 'getById',
          path: '/{id}',
          method: 'GET',
          description: '获取租户详情'
        },
        {
          name: 'create',
          path: '',
          method: 'POST',
          description: '创建租户'
        },
        {
          name: 'update',
          path: '/{id}',
          method: 'PUT',
          description: '更新租户'
        },
        {
          name: 'delete',
          path: '/{id}',
          method: 'DELETE',
          description: '删除租户'
        },
        {
          name: 'updateStatus',
          path: '/{id}/status',
          method: 'POST',
          description: '更新租户状态'
        }
      ]
    },
    {
      name: 'resources',
      basePath: '/api/resources',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取资源列表'
        },
        {
          name: 'getTree',
          path: '/tree',
          method: 'GET',
          description: '获取资源树'
        },
        {
          name: 'getById',
          path: '/{id}',
          method: 'GET',
          description: '获取资源详情'
        },
        {
          name: 'create',
          path: '',
          method: 'POST',
          description: '创建资源'
        },
        {
          name: 'update',
          path: '/{id}',
          method: 'PUT',
          description: '更新资源'
        },
        {
          name: 'delete',
          path: '/{id}',
          method: 'DELETE',
          description: '删除资源'
        }
      ]
    },
    {
      name: 'metadata',
      basePath: '/api/metadata',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取元数据列表'
        },
        {
          name: 'getById',
          path: '/{id}',
          method: 'GET',
          description: '获取元数据详情'
        },
        {
          name: 'create',
          path: '',
          method: 'POST',
          description: '创建元数据'
        },
        {
          name: 'update',
          path: '/{id}',
          method: 'PUT',
          description: '更新元数据'
        },
        {
          name: 'delete',
          path: '/{id}',
          method: 'DELETE',
          description: '删除元数据'
        }
      ]
    },
    {
      name: 'audit',
      basePath: '/api/audit',
      endpoints: [
        {
          name: 'getList',
          path: '',
          method: 'GET',
          description: '获取审计日志'
        },
        {
          name: 'export',
          path: '/export',
          method: 'GET',
          description: '导出审计日志'
        }
      ]
    },
    {
      name: 'system',
      basePath: '/api/system',
      endpoints: [
        {
          name: 'getInfo',
          path: '/info',
          method: 'GET',
          description: '获取系统信息'
        },
        {
          name: 'seedData',
          path: '/seed-data',
          method: 'POST',
          description: '初始化数据'
        }
      ]
    },
    {
      name: 'cache',
      basePath: '/api/cache',
      endpoints: [
        {
          name: 'getStatus',
          path: '/status',
          method: 'GET',
          description: '获取缓存状态'
        },
        {
          name: 'clear',
          path: '/clear',
          method: 'POST',
          description: '清除缓存'
        }
      ]
    }
  ]
};

// API配置管理器
export class ApiConfigManager {
  private config: ApiConfig;

  constructor(config: ApiConfig = defaultApiConfig) {
    this.config = config;
  }

  // 获取完整配置
  getConfig(): ApiConfig {
    return this.config;
  }

  // 获取模块配置
  getModule(name: string): ApiModule | undefined {
    return this.config.modules.find(module => module.name === name);
  }

  // 获取端点配置
  getEndpoint(moduleName: string, endpointName: string): ApiEndpoint | undefined {
    const module = this.getModule(moduleName);
    return module?.endpoints.find(endpoint => endpoint.name === endpointName);
  }

  // 构建完整的URL
  buildUrl(moduleName: string, endpointName: string, params?: Record<string, any>): string {
    const module = this.getModule(moduleName);
    const endpoint = this.getEndpoint(moduleName, endpointName);
    
    if (!module || !endpoint) {
      throw new Error(`API endpoint not found: ${moduleName}.${endpointName}`);
    }

    let path = module.basePath + endpoint.path;
    
    // 替换路径参数
    if (params) {
      Object.keys(params).forEach(key => {
        path = path.replace(`{${key}}`, params[key]);
      });
    }

    return this.config.baseURL + path;
  }

  // 更新配置
  updateConfig(newConfig: Partial<ApiConfig>) {
    this.config = { ...this.config, ...newConfig };
  }

  // 添加模块
  addModule(module: ApiModule) {
    const existingIndex = this.config.modules.findIndex(m => m.name === module.name);
    if (existingIndex >= 0) {
      this.config.modules[existingIndex] = module;
    } else {
      this.config.modules.push(module);
    }
  }

  // 添加端点
  addEndpoint(moduleName: string, endpoint: ApiEndpoint) {
    const module = this.getModule(moduleName);
    if (module) {
      const existingIndex = module.endpoints.findIndex(e => e.name === endpoint.name);
      if (existingIndex >= 0) {
        module.endpoints[existingIndex] = endpoint;
      } else {
        module.endpoints.push(endpoint);
      }
    }
  }
}

// 导出默认配置管理器实例
export const apiConfigManager = new ApiConfigManager();