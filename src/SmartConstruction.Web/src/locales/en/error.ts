// English error messages
export default {
  // Error types
  errorType: {
    NETWORK: 'Network Error',
    AUTH: 'Authentication Error',
    PERMISSION: 'Permission Error',
    BUSINESS: 'Business Error',
    SERVER: 'Server Error',
    CLIENT: 'Client Error',
    UNKNOWN: 'Unknown Error'
  },
  
  // Common error messages
  common: {
    unknownError: 'An unknown error occurred',
    networkError: 'Network connection failed, please check your network',
    timeoutError: 'Request timed out, please try again later',
    serverError: 'Internal server error',
    dataError: 'Data format error',
    validationError: 'Data validation failed',
    notFound: 'The requested resource does not exist',
    operationFailed: 'Operation failed',
    systemError: 'System error, please contact administrator'
  },
  
  // Authentication related errors
  auth: {
    invalidToken: 'Invalid access token',
    expiredToken: 'Access token has expired',
    invalidCredentials: 'Incorrect username or password',
    accountDisabled: 'Account has been disabled',
    accountLocked: 'Account has been locked',
    accountExpired: 'Account has expired',
    passwordExpired: 'Password has expired, please change your password',
    invalidCaptcha: 'Invalid verification code',
    invalidTenant: 'Invalid tenant',
    tenantDisabled: 'Tenant has been disabled',
    tenantExpired: 'Tenant has expired',
    loginRequired: 'Please login first',
    sessionExpired: 'Session has expired, please login again',
    tooManyAttempts: 'Too many login attempts, please try again later'
  },
  
  // Permission related errors
  permission: {
    denied: 'Insufficient permissions to perform this operation',
    accessDenied: 'Access denied',
    noPermission: 'You do not have permission to access this resource',
    forbidden: 'Forbidden access',
    unauthorized: 'Unauthorized operation'
  },
  
  // Form validation errors
  validation: {
    required: '{field} cannot be empty',
    minLength: '{field} must be at least {min} characters',
    maxLength: '{field} cannot exceed {max} characters',
    pattern: '{field} format is incorrect',
    email: 'Please enter a valid email address',
    url: 'Please enter a valid URL',
    number: 'Please enter a valid number',
    integer: 'Please enter an integer',
    min: '{field} cannot be less than {min}',
    max: '{field} cannot be greater than {max}',
    confirmPassword: 'The two passwords do not match'
  },
  
  // Business errors
  business: {
    resourceNotFound: 'The requested resource does not exist',
    resourceAlreadyExists: 'Resource already exists',
    resourceConflict: 'Resource conflict',
    invalidOperation: 'Invalid operation',
    operationNotAllowed: 'Operation not allowed',
    dataIntegrityViolation: 'Data integrity violation',
    dependencyExists: 'Dependencies exist, cannot perform operation',
    statusNotAllowed: 'Current status does not allow this operation',
    quotaExceeded: 'Quota limit exceeded'
  },
  
  // Network errors
  network: {
    connectionFailed: 'Network connection failed',
    requestTimeout: 'Request timed out',
    serverUnreachable: 'Cannot connect to server',
    networkUnavailable: 'Network unavailable',
    connectionReset: 'Connection reset',
    connectionClosed: 'Connection closed'
  },
  
  // Server errors
  server: {
    internalError: 'Internal server error',
    serviceUnavailable: 'Service unavailable',
    gatewayError: 'Gateway error',
    databaseError: 'Database operation failed',
    externalServiceError: 'External service call failed',
    configurationError: 'Server configuration error',
    resourceExhausted: 'Server resources exhausted'
  },
  
  // Client errors
  client: {
    badRequest: 'Bad request parameters',
    invalidArgument: 'Invalid argument',
    unsupportedOperation: 'Unsupported operation',
    tooManyRequests: 'Too many requests, please try again later',
    clientTimeout: 'Client timeout',
    aborted: 'Operation aborted',
    browserNotSupported: 'Browser does not support this feature'
  }
}