// =============================================
// 智能化API客户端
// =============================================
import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import { ElMessage } from 'element-plus';
import { apiConfigManager, type ApiConfig, type ApiModule, type ApiEndpoint } from './config';
import { getToken, removeToken } from '@/utils/auth';
import { useUserStore } from '@/stores/user';
import type { ApiResponse } from '@/types/global';

// 请求选项接口
export interface RequestOptions extends AxiosRequestConfig {
  showError?: boolean;
  showLoading?: boolean;
  timeout?: number;
}

// API客户端类
export class ApiClient {
  private instance: AxiosInstance;
  private config: ApiConfig;
  private isRefreshing = false;
  private failedQueue: Array<{
    resolve: (value: any) => void;
    reject: (reason: any) => void;
  }> = [];

  constructor(config?: Partial<ApiConfig>) {
    this.config = config ? { ...apiConfigManager.getConfig(), ...config } : apiConfigManager.getConfig();
    
    // 创建axios实例
    this.instance = axios.create({
      baseURL: this.config.baseURL,
      timeout: this.config.timeout,
      withCredentials: true
    });

    this.setupInterceptors();
  }

  // 设置拦截器
  private setupInterceptors() {
    // 请求拦截器
    this.instance.interceptors.request.use(
      (config) => {
        // 添加认证token
        const token = getToken();
        if (token) {
          config.headers['Authorization'] = `Bearer ${token}`;
        }

        // 添加租户信息
        const userStore = useUserStore();
        if (userStore.currentTenant?.id) {
          config.headers['X-Tenant-Id'] = userStore.currentTenant.id;
        }

        // 添加语言信息
        config.headers['Accept-Language'] = 'zh-CN';

        console.log(`🚀 API Request: ${config.method?.toUpperCase()} ${config.url}`, {
          headers: config.headers,
          data: config.data,
          params: config.params
        });

        return config;
      },
      (error) => {
        console.error('❌ Request Error:', error);
        return Promise.reject(error);
      }
    );

    // 响应拦截器
    this.instance.interceptors.response.use(
      (response: AxiosResponse<ApiResponse>) => {
        const res = response.data;

        console.log(`✅ API Response: ${response.config.method?.toUpperCase()} ${response.config.url}`, {
          status: response.status,
          data: res
        });

        // 检查业务状态码
        if (res.code !== 200) {
          ElMessage.error(res.message || '请求失败');

          // 特殊错误码处理
          if (res.code === 401) {
            this.handleUnauthorized();
          } else if (res.code === 403) {
            ElMessage.error('没有权限访问该资源');
          }

          return Promise.reject(new Error(res.message || '请求失败'));
        }

        return res;
      },
      async (error) => {
        console.error('❌ Response Error:', error);

        const originalRequest = error.config;

        // 处理401错误（token过期）
        if (error.response?.status === 401 && !originalRequest._retry) {
          if (this.isRefreshing) {
            return new Promise((resolve, reject) => {
              this.failedQueue.push({ resolve, reject });
            })
              .then((token) => {
                originalRequest.headers['Authorization'] = `Bearer ${token}`;
                return this.instance(originalRequest);
              })
              .catch((err) => {
                return Promise.reject(err);
              });
          }

          originalRequest._retry = true;
          this.isRefreshing = true;

          try {
            // 尝试刷新token
            const userStore = useUserStore();
            const newToken = await userStore.refreshTokenAction();
            
            originalRequest.headers['Authorization'] = `Bearer ${newToken}`;
            this.processQueue(null, newToken);
            
            return this.instance(originalRequest);
          } catch (refreshError) {
            this.processQueue(refreshError, null);
            this.handleUnauthorized();
            return Promise.reject(refreshError);
          } finally {
            this.isRefreshing = false;
          }
        }

        // 处理其他HTTP错误
        this.handleHttpError(error);
        return Promise.reject(error);
      }
    );
  }

  // 处理队列
  private processQueue(error: any, token: string | null = null) {
    this.failedQueue.forEach((prom) => {
      if (error) {
        prom.reject(error);
      } else {
        prom.resolve(token);
      }
    });
    this.failedQueue = [];
  }

  // 处理未授权错误
  private handleUnauthorized() {
    const userStore = useUserStore();
    userStore.resetUser();
    removeToken();
    window.location.href = '/login';
  }

  // 处理HTTP错误
  private handleHttpError(error: any) {
    let message = '网络错误，请稍后重试';
    
    if (error.response) {
      switch (error.response.status) {
        case 400:
          message = '请求参数错误';
          break;
        case 401:
          message = '未授权，请重新登录';
          break;
        case 403:
          message = '拒绝访问';
          break;
        case 404:
          message = '请求地址不存在';
          break;
        case 500:
          message = '服务器内部错误';
          break;
        default:
          message = `请求失败: ${error.response.status}`;
      }
    } else if (error.message.includes('timeout')) {
      message = '请求超时，请重试';
    }

    ElMessage.error(message);
  }

  // 通用请求方法
  async request<T = any>(
    moduleName: string,
    endpointName: string,
    data?: any,
    options: RequestOptions = {}
  ): Promise<ApiResponse<T>> {
    const endpoint = apiConfigManager.getEndpoint(moduleName, endpointName);
    if (!endpoint) {
      throw new Error(`API endpoint not found: ${moduleName}.${endpointName}`);
    }

    const url = apiConfigManager.buildUrl(moduleName, endpointName, data?.pathParams);
    
    const config: AxiosRequestConfig = {
      method: endpoint.method,
      url,
      ...options
    };

    // 根据请求方法设置数据
    if (['POST', 'PUT', 'PATCH'].includes(endpoint.method)) {
      config.data = data?.body || data;
    } else {
      config.params = data?.query || data;
    }

    return this.instance.request<ApiResponse<T>>(config);
  }

  // GET请求
  async get<T = any>(
    moduleName: string,
    endpointName: string,
    params?: any,
    options?: RequestOptions
  ): Promise<ApiResponse<T>> {
    return this.request<T>(moduleName, endpointName, { query: params }, { ...options, method: 'GET' });
  }

  // POST请求
  async post<T = any>(
    moduleName: string,
    endpointName: string,
    data?: any,
    options?: RequestOptions
  ): Promise<ApiResponse<T>> {
    return this.request<T>(moduleName, endpointName, { body: data }, { ...options, method: 'POST' });
  }

  // PUT请求
  async put<T = any>(
    moduleName: string,
    endpointName: string,
    data?: any,
    options?: RequestOptions
  ): Promise<ApiResponse<T>> {
    return this.request<T>(moduleName, endpointName, { body: data }, { ...options, method: 'PUT' });
  }

  // DELETE请求
  async delete<T = any>(
    moduleName: string,
    endpointName: string,
    params?: any,
    options?: RequestOptions
  ): Promise<ApiResponse<T>> {
    return this.request<T>(moduleName, endpointName, { pathParams: params }, { ...options, method: 'DELETE' });
  }

  // 批量请求
  async batch<T = any>(requests: Array<{
    moduleName: string;
    endpointName: string;
    data?: any;
    options?: RequestOptions;
  }>): Promise<ApiResponse<T>[]> {
    const promises = requests.map(req => 
      this.request<T>(req.moduleName, req.endpointName, req.data, req.options)
    );
    
    return Promise.all(promises);
  }

  // 上传文件
  async upload<T = any>(
    moduleName: string,
    endpointName: string,
    file: File,
    data?: any,
    onProgress?: (progress: number) => void
  ): Promise<ApiResponse<T>> {
    const formData = new FormData();
    formData.append('file', file);
    
    if (data) {
      Object.keys(data).forEach(key => {
        formData.append(key, data[key]);
      });
    }

    return this.request<T>(moduleName, endpointName, { body: formData }, {
      headers: {
        'Content-Type': 'multipart/form-data'
      },
      onUploadProgress: (progressEvent) => {
        if (onProgress && progressEvent.total) {
          const progress = Math.round((progressEvent.loaded * 100) / progressEvent.total);
          onProgress(progress);
        }
      }
    });
  }

  // 下载文件
  async download(
    moduleName: string,
    endpointName: string,
    params?: any,
    filename?: string
  ): Promise<void> {
    const response = await this.request(moduleName, endpointName, params, {
      responseType: 'blob'
    });

    // 创建下载链接
    const blob = new Blob([response.data]);
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename || 'download';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  // 取消请求
  createCancelToken() {
    return axios.CancelToken.source();
  }

  // 更新配置
  updateConfig(config: Partial<ApiConfig>) {
    this.config = { ...this.config, ...config };
    this.instance.defaults.baseURL = this.config.baseURL;
    this.instance.defaults.timeout = this.config.timeout;
  }
}

// 创建默认客户端实例
export const apiClient = new ApiClient();

// 导出便捷方法
export const api = {
  // 认证相关
  auth: {
    login: (data: any) => apiClient.post('auth', 'login', data),
    logout: () => apiClient.post('auth', 'logout'),
    getCurrentUser: () => apiClient.get('auth', 'getCurrentUser'),
    refreshToken: (data: any) => apiClient.post('auth', 'refreshToken', data),
    changePassword: (data: any) => apiClient.post('auth', 'changePassword', data),
    validateToken: () => apiClient.get('auth', 'validateToken')
  },

  // 用户管理
  users: {
    getList: (params?: any) => apiClient.get('users', 'getList', params),
    getById: (id: string) => apiClient.get('users', 'getById', { id }),
    create: (data: any) => apiClient.post('users', 'create', data),
    update: (id: string, data: any) => apiClient.put('users', 'update', data),
    delete: (id: string) => apiClient.delete('users', 'delete', { id }),
    assignRoles: (id: string, data: any) => apiClient.post('users', 'assignRoles', data)
  },

  // 角色管理
  roles: {
    getList: (params?: any) => apiClient.get('roles', 'getList', params),
    getById: (id: string) => apiClient.get('roles', 'getById', { id }),
    create: (data: any) => apiClient.post('roles', 'create', data),
    update: (id: string, data: any) => apiClient.put('roles', 'update', data),
    delete: (id: string) => apiClient.delete('roles', 'delete', { id }),
    assignPermissions: (id: string, data: any) => apiClient.post('roles', 'assignPermissions', data)
  },

  // 组织管理
  organizations: {
    getList: (params?: any) => apiClient.get('organizations', 'getList', params),
    getTree: () => apiClient.get('organizations', 'getTree'),
    getById: (id: string) => apiClient.get('organizations', 'getById', { id }),
    create: (data: any) => apiClient.post('organizations', 'create', data),
    update: (id: string, data: any) => apiClient.put('organizations', 'update', data),
    delete: (id: string) => apiClient.delete('organizations', 'delete', { id })
  },

  // 租户管理
  tenants: {
    getList: (params?: any) => apiClient.get('tenants', 'getList', params),
    getById: (id: string) => apiClient.get('tenants', 'getById', { id }),
    create: (data: any) => apiClient.post('tenants', 'create', data),
    update: (id: string, data: any) => apiClient.put('tenants', 'update', data),
    delete: (id: string) => apiClient.delete('tenants', 'delete', { id }),
    updateStatus: (id: string, data: any) => apiClient.post('tenants', 'updateStatus', data)
  },

  // 资源管理
  resources: {
    getList: (params?: any) => apiClient.get('resources', 'getList', params),
    getTree: () => apiClient.get('resources', 'getTree'),
    getById: (id: string) => apiClient.get('resources', 'getById', { id }),
    create: (data: any) => apiClient.post('resources', 'create', data),
    update: (id: string, data: any) => apiClient.put('resources', 'update', data),
    delete: (id: string) => apiClient.delete('resources', 'delete', { id })
  },

  // 元数据管理
  metadata: {
    getList: (params?: any) => apiClient.get('metadata', 'getList', params),
    getById: (id: string) => apiClient.get('metadata', 'getById', { id }),
    create: (data: any) => apiClient.post('metadata', 'create', data),
    update: (id: string, data: any) => apiClient.put('metadata', 'update', data),
    delete: (id: string) => apiClient.delete('metadata', 'delete', { id })
  },

  // 审计日志
  audit: {
    getList: (params?: any) => apiClient.get('audit', 'getList', params),
    export: (params?: any) => apiClient.download('audit', 'export', params, 'audit-logs.xlsx')
  },

  // 系统管理
  system: {
    getInfo: () => apiClient.get('system', 'getInfo'),
    seedData: () => apiClient.post('system', 'seedData')
  },

  // 缓存管理
  cache: {
    getStatus: () => apiClient.get('cache', 'getStatus'),
    clear: () => apiClient.post('cache', 'clear')
  }
};

export default apiClient;