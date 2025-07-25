import { http } from '@/utils/http'
import type { ApiResponse, LoginRequest, LoginResponse, TokenInfo, Menu } from '@/types/global'

// 刷新令牌请求参数
export interface RefreshTokenRequest {
  refreshToken: string
}

// 用户菜单响应
export interface UserMenusResponse {
  menus: Menu[]
  permissions: string[]
}

// 用户信息响应
export interface UserInfoResponse {
  user: {
    id: string
    username: string
    displayName: string
    email?: string
    mobile?: string
    avatar?: string
    status: number
    tenantId: string
    organizationId?: string
    organizationPath?: string
    roles: Array<{
      id: string
      code: string
      name: string
      dataScope: string
      status: number
      isSystem: boolean
      createdAt: string
    }>
    createdAt: string
  }
  permissions: string[]
}

/**
 * 用户登录
 */
export function login(data: LoginRequest) {
  // API路径使用小写
  return http.post<ApiResponse<LoginResponse>>('/api/auth/login', data)
}

/**
 * 用户登出
 */
export function logout() {
  return http.post<ApiResponse<void>>('/api/auth/logout')
}

/**
 * @description 刷新令牌
 * @param {string} refreshToken - 旧的刷新令牌
 */
export function refreshTokenApi(refreshToken: string) {
  // 注意：此请求专门用于刷新令牌，需要一个特殊的axios实例或配置
  // 以避免循环触发401错误。在 http.ts 中已通过 _isRefreshToken 标志处理。
  return http.post<any>(
    '/api/auth/refresh-token',
    { refreshToken },
    { _isRefreshToken: true } as any,
  )
}

/**
 * 验证令牌有效性
 */
export function validateToken() {
  return http.get<ApiResponse<{ valid: boolean }>>('/api/auth/validate')
}

/**
 * 获取验证码
 */
export function getCaptcha() {
  return http.get<ApiResponse<{
    key: string
    image: string
  }>>('/api/auth/captcha')
}

/**
 * 发送短信验证码
 */
export function sendSmsCode(mobile: string) {
  return http.post<ApiResponse<void>>('/api/auth/sms-code', { mobile })
}

/**
 * 发送邮箱验证码
 */
export function sendEmailCode(email: string) {
  return http.post<ApiResponse<void>>('/api/auth/email-code', { email })
}

/**
 * 手机号登录
 */
export function loginByMobile(data: {
  tenantCode: string
  mobile: string
  code: string
  deviceId?: string
  deviceType?: string
}) {
  return http.post<ApiResponse<LoginResponse>>('/api/auth/login-mobile', data)
}

/**
 * 邮箱登录
 */
export function loginByEmail(data: {
  tenantCode: string
  email: string
  code: string
  deviceId?: string
  deviceType?: string
}) {
  return http.post<ApiResponse<LoginResponse>>('/api/auth/login-email', data)
}

/**
 * 第三方登录
 */
export function loginByThirdParty(data: {
  provider: 'wechat' | 'dingtalk' | 'feishu'
  code: string
  state?: string
}) {
  return http.post<ApiResponse<LoginResponse>>('/api/auth/login-third-party', data)
}

/**
 * 忘记密码 - 发送重置邮件
 */
export function forgotPassword(data: {
  tenantCode: string
  email: string
}) {
  return http.post<ApiResponse<void>>('/api/auth/forgot-password', data)
}

/**
 * 绑定手机号
 */
export function bindMobile(data: {
  mobile: string
  code: string
}) {
  return http.post<ApiResponse<void>>('/api/auth/bind-mobile', data)
}

/**
 * 绑定邮箱
 */
export function bindEmail(data: {
  email: string
  code: string
}) {
  return http.post<ApiResponse<void>>('/api/auth/bind-email', data)
}

/**
 * 解绑手机号
 */
export function unbindMobile(data: {
  code: string
}) {
  return http.post<ApiResponse<void>>('/api/auth/unbind-mobile', data)
}

/**
 * 解绑邮箱
 */
export function unbindEmail(data: {
  code: string
}) {
  return http.post<ApiResponse<void>>('/api/auth/unbind-email', data)
}