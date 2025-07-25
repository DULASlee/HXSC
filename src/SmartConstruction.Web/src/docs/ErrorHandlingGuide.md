# 错误处理系统使用指南

本文档提供了关于如何在项目中使用错误处理系统的指导。

## 1. 错误处理系统概述

我们的错误处理系统包含以下主要组件：

- **ErrorHandler**: 错误解析和处理工具类
- **errorService**: 统一的错误处理服务
- **ErrorMessage**: 友好的错误展示组件
- **GlobalErrorHandler**: 全局错误捕获组件
- **logService**: 错误日志收集和上报服务
- **errorI18nService**: 错误国际化服务
- **formValidation**: 表单验证错误处理工具

## 2. 如何处理错误

### 2.1 使用全局错误处理函数

最简单的方式是使用全局错误处理函数 `window.$handleError`：

```typescript
try {
  // 可能会抛出错误的代码
} catch (error) {
  window.$handleError(error)
}
```

或者使用依赖注入的方式：

```typescript
import { inject } from 'vue'

const handleGlobalError = inject('handleGlobalError')

try {
  // 可能会抛出错误的代码
} catch (error) {
  if (handleGlobalError) {
    handleGlobalError(error)
  }
}
```

### 2.2 使用错误处理服务

如果需要更多控制，可以直接使用错误处理服务：

```typescript
import { errorService } from '@/services/errorService'

try {
  // 可能会抛出错误的代码
} catch (error) {
  errorService.handleError(error, {
    notification: 'dialog',  // 'message', 'notification', 'dialog', 'none'
    redirect: '/error',      // 可选的重定向路径
    showDetails: true        // 是否显示详细信息
  })
}
```

### 2.3 使用错误消息组件

在组件中显示错误消息：

```vue
<template>
  <error-message
    type="error"
    title="操作失败"
    message="无法保存数据"
    :show-details="true"
    details="详细错误信息..."
    :show-actions="true"
    :show-retry="true"
    :show-close="true"
    @retry="handleRetry"
    @close="handleClose"
  />
</template>

<script setup>
import ErrorMessage from '@/components/ErrorMessage.vue'

const handleRetry = () => {
  // 重试逻辑
}

const handleClose = () => {
  // 关闭逻辑
}
</script>
```

## 3. 处理表单验证错误

使用表单验证工具处理表单验证错误：

```typescript
import { validateForm, createFormRules } from '@/utils/formValidation'
import type { FormInstance } from 'element-plus'

const formRef = ref<FormInstance>()
const form = reactive({
  username: '',
  email: ''
})

// 创建国际化的表单验证规则
const rules = createFormRules({
  username: [
    { required: true, trigger: 'blur' },
    { min: 3, max: 20, trigger: 'blur' }
  ],
  email: [
    { required: true, trigger: 'blur' },
    { type: 'email', trigger: 'blur' }
  ]
})

// 提交表单
const submitForm = async () => {
  if (!formRef.value) return
  
  const result = await validateForm(formRef.value)
  if (result.valid) {
    // 表单验证通过，执行提交逻辑
  } else {
    // 表单验证失败，错误已由validateForm内部处理
    console.log('验证错误:', result.errors)
    console.log('第一个错误:', result.firstError)
    console.log('错误消息:', result.errorMessage)
  }
}
```

## 4. 错误日志收集和上报

使用日志服务记录错误：

```typescript
import { logService, LogLevel } from '@/services/logService'

// 记录不同级别的日志
logService.debug('调试信息', '详细信息', { data: 'value' })
logService.info('信息消息', '详细信息', { data: 'value' })
logService.warn('警告消息', '详细信息', { data: 'value' })
logService.error('错误消息', '详细信息', { data: 'value' })
logService.fatal('致命错误', '详细信息', { data: 'value' })

// 记录错误对象
try {
  // 可能会抛出错误的代码
} catch (error) {
  logService.logError(error)
}

// 手动上报日志
logService.reportLogsNow()
```

## 5. 错误国际化

使用错误国际化服务获取国际化的错误消息：

```typescript
import { errorI18nService } from '@/services/errorI18nService'

// 获取错误类型的国际化文本
const errorTypeText = errorI18nService.getErrorTypeText('NETWORK')

// 获取错误消息的国际化文本
const errorMessage = errorI18nService.getErrorMessage('NETWORK_ERROR', '默认消息')

// 获取验证错误消息
const validationMessage = errorI18nService.getValidationMessage('required', '用户名', { min: 3 })
```

## 6. 错误监控

访问错误监控页面查看和管理系统中的错误：

- 路径: `/monitor/errors`
- 功能:
  - 查看错误列表
  - 查看日志记录
  - 查看错误统计和趋势
  - 配置错误处理设置

## 7. 最佳实践

1. **统一处理API错误**：在API请求层统一处理错误，避免在每个组件中重复处理。

2. **提供友好的错误消息**：错误消息应该清晰、具体，告诉用户发生了什么以及如何解决。

3. **区分用户错误和系统错误**：用户错误（如表单验证错误）应该友好提示；系统错误应该记录详细信息并提供简洁的用户提示。

4. **提供重试机制**：对于网络错误等临时性错误，提供重试机制。

5. **记录详细的错误信息**：记录足够的上下文信息，以便于调试和分析。

6. **国际化错误消息**：根据用户的语言偏好显示错误消息。

7. **优雅降级**：当发生错误时，尽量保持应用的其他部分正常工作。

## 8. 错误类型参考

| 错误类型 | 描述 | 处理方式 |
|---------|------|---------|
| NETWORK | 网络连接错误 | 提示用户检查网络连接，提供重试选项 |
| AUTH | 认证相关错误 | 重定向到登录页面，提示用户重新登录 |
| PERMISSION | 权限相关错误 | 提示用户没有权限，可能重定向到403页面 |
| BUSINESS | 业务逻辑错误 | 显示具体的业务错误消息 |
| SERVER | 服务器错误 | 提示用户服务器错误，记录详细日志 |
| CLIENT | 客户端错误 | 提示用户客户端错误，可能需要刷新页面 |
| UNKNOWN | 未知错误 | 提示用户发生未知错误，记录详细日志 |

## 9. 错误码参考

| 错误码 | 描述 | 处理方式 |
|-------|------|---------|
| NETWORK_ERROR | 网络连接失败 | 提示用户检查网络连接 |
| TIMEOUT_ERROR | 请求超时 | 提示用户请求超时，可以重试 |
| AUTH_EXPIRED_TOKEN | 访问令牌已过期 | 重定向到登录页面 |
| AUTH_INVALID_CREDENTIALS | 用户名或密码错误 | 提示用户重新输入凭据 |
| PERMISSION_DENIED | 权限不足 | 提示用户没有权限执行此操作 |
| RESOURCE_NOT_FOUND | 资源不存在 | 提示用户请求的资源不存在 |
| VALIDATION_ERROR | 数据验证失败 | 显示具体的验证错误消息 |
| BUSINESS_ERROR | 业务处理失败 | 显示具体的业务错误消息 |
| SYSTEM_ERROR | 系统内部错误 | 提示用户系统错误，联系管理员 |

## 10. 扩展和定制

错误处理系统是可扩展的，你可以根据需要添加更多的错误类型和处理方式：

1. 在 `utils/error.ts` 中添加新的错误类型
2. 在 `services/errorService.ts` 中添加新的错误处理逻辑
3. 在 `locales/zh-CN/error.ts` 和 `locales/en/error.ts` 中添加新的错误消息
4. 在 `services/errorI18nService.ts` 中添加新的错误码映射

## 11. 示例

查看 `src/components/ErrorHandlingExample.vue` 和 `src/views/examples/ErrorHandlingDemo.vue` 获取更多使用示例。