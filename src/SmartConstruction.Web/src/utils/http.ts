import axios from 'axios'
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from 'axios'
// 彻底移除对 ElMessage 的直接依赖，改由 errorService 统一处理
// import { ElMessage } from 'element-plus'
// [最终清理] 彻底移除旧的、独立的 auth utils
// import { getToken, setToken, getRefreshToken, setRefreshToken } from '@/utils/auth'
// router 的依赖保留，用于在极端情况下跳转
import router from '@/router'

interface CustomAxiosRequestConfig extends InternalAxiosRequestConfig {
  _isRefreshToken?: boolean;
}

// [最终清理] 移除旧的全局状态变量
// let isRefreshing = false;
// let requests: Array<(token: string) => void> = [];

const service: AxiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '',
  timeout: 15000
});

service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    // [终极军改] 直接从 localStorage 读取状态，避免初始化时序问题
    try {
      const userStateString = localStorage.getItem('user')
      if (userStateString) {
        const userState = JSON.parse(userStateString)
        const token = userState.token
        const tenantId = userState.currentTenant?.id

        if (token && !(config as any)._isRefreshToken) {
          config.headers.Authorization = `Bearer ${token}`
        }

        if (tenantId) {
          config.headers['X-Tenant-Id'] = tenantId
        }
      }
    } catch (e) {
      console.error('[请求拦截器] 从localStorage解析状态失败:', e)
    }

    return config
  },
  (error) => {
    console.error('请求配置错误:', error)
    return Promise.reject(error)
  },
)

service.interceptors.response.use(
  (response: AxiosResponse) => {
    const config = response.config as CustomAxiosRequestConfig
    const url = config.url || ''

    // 对登录和刷新token的响应，直接返回最原始的 data 部分
    if (config._isRefreshToken || url.includes('/api/auth/login')) {
      return response.data
    }

    // 直接返回blob响应
    if (response.config.responseType === 'blob') {
      return response
    }

    // 统一处理业务响应
    const res = response.data
    const isSuccess = res.success ?? res.isSuccess ?? res.IsSuccess

    if (isSuccess) {
      // 成功时，返回核心的 data 字段
      return res.data
    }

    // 业务失败处理
    const errorMessage = res.message || '未知错误'
    const businessError = new Error(`业务错误: ${errorMessage}`)
    Object.assign(businessError, { code: res.code, responseData: res })

    import('@/services/errorService').then(({ errorService }) => {
      errorService.handleError(businessError)
    })

    return Promise.reject(businessError)
  },
  async (error) => {
    const originalRequest = error.config as CustomAxiosRequestConfig

    if (error.response?.status === 401 && !originalRequest._isRetry) {
      originalRequest._isRetry = true
      try {
        // [终极军改] 在需要时动态导入 store
        const { useUserStore } = await import('@/stores/user')
        const userStore = useUserStore()
        const newAccessToken = await userStore.refreshToken()
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`
        return service(originalRequest)
      }
      catch (refreshError) {
        // refreshToken 内部已经处理了重置状态和跳转到登录页的逻辑
        // 这里不再需要调用 logout()，否则会引起死循环
        console.error('刷新令牌失败，store已处理重置，此处不再重复操作:', refreshError)
        // 直接将错误抛出，让最外层的调用者知道请求失败了
        return Promise.reject(refreshError)
      }
    }

    // 其他错误则直接抛出
    const errorMessage = `网络错误: ${error.message}`
    const networkError = new Error(errorMessage)
    Object.assign(networkError, { originalError: error })

    import('@/services/errorService').then(({ errorService }) => {
      errorService.handleError(networkError)
    })

    return Promise.reject(networkError)
  },
)

// [最终清理] 彻底删除整个旧的、未使用的 handleUnauthorized 函数
/*
function handleUnauthorized(config: CustomAxiosRequestConfig): Promise<any> {
  ...
}
*/

// [最终清理] 彻底删除整个旧的、独立的 refreshToken 函数
/*
async function refreshToken(): Promise<string> {
  ...
}
*/

// 封装HTTP方法
export const http = {
  get: <T>(url: string, params?: any, config?: AxiosRequestConfig) => 
    service.get<T, T>(url, { params, ...config }),

  post: <T>(url: string, data?: any, config?: AxiosRequestConfig) => 
    service.post<T, T>(url, data, config),

  put: <T>(url: string, data?: any, config?: AxiosRequestConfig) => 
    service.put<T, T>(url, data, config),

  delete: <T>(url: string, params?: any, config?: AxiosRequestConfig) => 
    service.delete<T, T>(url, { params, ...config })
};

export default service;