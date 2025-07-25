// 中文错误消息
export default {
  // 错误类型
  errorType: {
    NETWORK: '网络错误',
    AUTH: '认证错误',
    PERMISSION: '权限错误',
    BUSINESS: '业务错误',
    SERVER: '服务器错误',
    CLIENT: '客户端错误',
    UNKNOWN: '未知错误'
  },
  
  // 通用错误消息
  common: {
    unknownError: '发生未知错误',
    networkError: '网络连接失败，请检查您的网络连接',
    timeoutError: '请求超时，请稍后重试',
    serverError: '服务器内部错误',
    dataError: '数据格式错误',
    validationError: '数据验证失败',
    notFound: '请求的资源不存在',
    operationFailed: '操作失败',
    systemError: '系统错误，请联系管理员'
  },
  
  // 认证相关错误
  auth: {
    invalidToken: '无效的访问令牌',
    expiredToken: '访问令牌已过期',
    invalidCredentials: '用户名或密码错误',
    accountDisabled: '账号已被禁用',
    accountLocked: '账号已被锁定',
    accountExpired: '账号已过期',
    passwordExpired: '密码已过期，请修改密码',
    invalidCaptcha: '验证码错误',
    invalidTenant: '无效的租户',
    tenantDisabled: '租户已被禁用',
    tenantExpired: '租户已过期',
    loginRequired: '请先登录',
    sessionExpired: '会话已过期，请重新登录',
    tooManyAttempts: '登录尝试次数过多，请稍后再试'
  },
  
  // 权限相关错误
  permission: {
    denied: '权限不足，无法执行此操作',
    accessDenied: '拒绝访问',
    noPermission: '您没有权限访问该资源',
    forbidden: '禁止访问',
    unauthorized: '未授权的操作'
  },
  
  // 表单验证错误
  validation: {
    required: '{field}不能为空',
    minLength: '{field}长度不能小于{min}个字符',
    maxLength: '{field}长度不能超过{max}个字符',
    pattern: '{field}格式不正确',
    email: '请输入有效的邮箱地址',
    url: '请输入有效的URL',
    number: '请输入有效的数字',
    integer: '请输入整数',
    min: '{field}不能小于{min}',
    max: '{field}不能大于{max}',
    confirmPassword: '两次输入的密码不一致'
  },
  
  // 业务错误
  business: {
    resourceNotFound: '请求的资源不存在',
    resourceAlreadyExists: '资源已存在',
    resourceConflict: '资源冲突',
    invalidOperation: '无效的操作',
    operationNotAllowed: '不允许的操作',
    dataIntegrityViolation: '数据完整性冲突',
    dependencyExists: '存在依赖关系，无法执行操作',
    statusNotAllowed: '当前状态不允许此操作',
    quotaExceeded: '已超出配额限制'
  },
  
  // 网络错误
  network: {
    connectionFailed: '网络连接失败',
    requestTimeout: '请求超时',
    serverUnreachable: '无法连接到服务器',
    networkUnavailable: '网络不可用',
    connectionReset: '连接被重置',
    connectionClosed: '连接已关闭'
  },
  
  // 服务器错误
  server: {
    internalError: '服务器内部错误',
    serviceUnavailable: '服务不可用',
    gatewayError: '网关错误',
    databaseError: '数据库操作失败',
    externalServiceError: '外部服务调用失败',
    configurationError: '服务器配置错误',
    resourceExhausted: '服务器资源耗尽'
  },
  
  // 客户端错误
  client: {
    badRequest: '请求参数错误',
    invalidArgument: '无效的参数',
    unsupportedOperation: '不支持的操作',
    tooManyRequests: '请求过于频繁，请稍后再试',
    clientTimeout: '客户端超时',
    aborted: '操作已取消',
    browserNotSupported: '浏览器不支持此功能'
  }
}