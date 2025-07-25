using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace SmartConstruction.Service.Infrastructure.Logging
{
    /// <summary>
    /// 增强日志服务 - 重构版本
    /// </summary>
    public class LoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly Serilog.ILogger _auditLogger;
        private readonly Serilog.ILogger _performanceLogger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
            _auditLogger = LoggingConfiguration.CreateAuditLogger();
            _performanceLogger = LoggingConfiguration.CreatePerformanceLogger();
        }

        /// <summary>
        /// 记录信息日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="parameters">参数</param>
        public void LogInformation(string message, params object[] parameters)
        {
            _logger.LogInformation(message, parameters);
        }

        /// <summary>
        /// 记录警告日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="parameters">参数</param>
        public void LogWarning(string message, params object[] parameters)
        {
            _logger.LogWarning(message, parameters);
        }

        /// <summary>
        /// 记录错误日志（带异常）
        /// </summary>
        /// <param name="exception">异常</param>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">消息</param>
        /// <param name="parameters">参数</param>
        public void LogError(Exception exception, string errorCode, string message, params object[] parameters)
        {
            var traceId = LoggingConfiguration.GenerateTraceId();
            var maskedParams = LoggingConfiguration.MaskSensitiveData(parameters);

            _logger.LogError(exception, 
                "错误日志 | TraceId: {TraceId} | ErrorCode: {ErrorCode} | Message: {Message} | Parameters: {@Parameters} | StackTrace: {StackTrace}", 
                traceId, errorCode, message, maskedParams, exception.StackTrace);
        }

        /// <summary>
        /// 记录错误日志（无异常）
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="message">消息</param>
        /// <param name="parameters">参数</param>
        public void LogError(string errorCode, string message, params object[] parameters)
        {
            var traceId = LoggingConfiguration.GenerateTraceId();
            var maskedParams = LoggingConfiguration.MaskSensitiveData(parameters);

            _logger.LogError("错误日志 | TraceId: {TraceId} | ErrorCode: {ErrorCode} | Message: {Message} | Parameters: {@Parameters}", 
                traceId, errorCode, message, maskedParams);
        }

        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="parameters">参数</param>
        public void LogDebug(string message, params object[] parameters)
        {
            var maskedParams = LoggingConfiguration.MaskSensitiveData(parameters);
            _logger.LogDebug(message, maskedParams);
        }

        /// <summary>
        /// 记录性能日志
        /// </summary>
        /// <param name="operation">操作名称</param>
        /// <param name="duration">耗时（毫秒）</param>
        /// <param name="parameters">参数</param>
        public void LogPerformance(string operation, long duration, params object[] parameters)
        {
            var traceId = LoggingConfiguration.GenerateTraceId();
            var maskedParams = LoggingConfiguration.MaskSensitiveData(parameters);

            _performanceLogger.Information(
                "性能日志 | TraceId: {TraceId} | Operation: {Operation} | Duration: {Duration}ms | Parameters: {@Parameters}", 
                traceId, operation, duration, maskedParams);
        }

        /// <summary>
        /// 记录审计日志
        /// </summary>
        /// <param name="action">操作</param>
        /// <param name="userId">用户ID</param>
        /// <param name="resource">资源</param>
        /// <param name="parameters">参数</param>
        public void LogAudit(string action, string userId, string resource, params object[] parameters)
        {
            var traceId = LoggingConfiguration.GenerateTraceId();
            var maskedParams = LoggingConfiguration.MaskSensitiveData(parameters);

            _auditLogger.Information(
                "审计日志 | TraceId: {TraceId} | Action: {Action} | UserId: {UserId} | Resource: {Resource} | Parameters: {@Parameters} | Timestamp: {Timestamp}", 
                traceId, action, userId, resource, maskedParams, DateTime.UtcNow);
        }

        /// <summary>
        /// 记录安全日志
        /// </summary>
        /// <param name="securityEvent">安全事件</param>
        /// <param name="userId">用户ID</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="details">详细信息</param>
        public void LogSecurity(string securityEvent, string userId, string ipAddress, object details)
        {
            var traceId = LoggingConfiguration.GenerateTraceId();
            var maskedDetails = LoggingConfiguration.MaskSensitiveData(details);

            _auditLogger.Warning(
                "安全日志 | TraceId: {TraceId} | SecurityEvent: {SecurityEvent} | UserId: {UserId} | IPAddress: {IPAddress} | Details: {@Details} | Timestamp: {Timestamp}", 
                traceId, securityEvent, userId, ipAddress, maskedDetails, DateTime.UtcNow);
        }

        /// <summary>
        /// 记录业务异常
        /// </summary>
        /// <param name="businessException">业务异常</param>
        /// <param name="operation">操作</param>
        /// <param name="parameters">参数</param>
        public void LogBusinessException(Exception businessException, string operation, params object[] parameters)
        {
            var traceId = LoggingConfiguration.GenerateTraceId();
            var maskedParams = LoggingConfiguration.MaskSensitiveData(parameters);

            _logger.LogError(businessException,
                "业务异常 | TraceId: {TraceId} | Operation: {Operation} | ErrorCode: {ErrorCode} | Message: {Message} | Parameters: {@Parameters} | StackTrace: {StackTrace}", 
                traceId, operation, GetBusinessErrorCode(businessException), businessException.Message, maskedParams, businessException.StackTrace);
        }

        /// <summary>
        /// 获取业务错误码
        /// </summary>
        private string GetBusinessErrorCode(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => "NULL_ARGUMENT",
                ArgumentException => "INVALID_ARGUMENT",
                InvalidOperationException => "INVALID_OPERATION",
                UnauthorizedAccessException => "UNAUTHORIZED",
                NotSupportedException => "NOT_SUPPORTED",
                TimeoutException => "TIMEOUT",
                _ => "BUSINESS_ERROR"
            };
        }

        /// <summary>
        /// 记录方法执行时间
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="action">执行操作</param>
        public async Task LogMethodExecutionAsync(string methodName, Func<Task> action)
        {
            var stopwatch = Stopwatch.StartNew();
            var traceId = LoggingConfiguration.GenerateTraceId();

            try
            {
                _logger.LogDebug("方法开始执行 | TraceId: {TraceId} | Method: {MethodName}", traceId, methodName);
                await action();
                stopwatch.Stop();
                
                _logger.LogDebug("方法执行完成 | TraceId: {TraceId} | Method: {MethodName} | Duration: {Duration}ms", 
                    traceId, methodName, stopwatch.ElapsedMilliseconds);
                
                LogPerformance(methodName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError(ex, "METHOD_EXECUTION_ERROR", $"方法执行异常: {methodName}", new { MethodName = methodName, Duration = stopwatch.ElapsedMilliseconds });
                throw;
            }
        }

        /// <summary>
        /// 记录方法执行时间（带返回值）
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="methodName">方法名</param>
        /// <param name="action">执行操作</param>
        /// <returns>执行结果</returns>
        public async Task<T> LogMethodExecutionAsync<T>(string methodName, Func<Task<T>> action)
        {
            var stopwatch = Stopwatch.StartNew();
            var traceId = LoggingConfiguration.GenerateTraceId();

            try
            {
                _logger.LogDebug("方法开始执行 | TraceId: {TraceId} | Method: {MethodName}", traceId, methodName);
                var result = await action();
                stopwatch.Stop();
                
                _logger.LogDebug("方法执行完成 | TraceId: {TraceId} | Method: {MethodName} | Duration: {Duration}ms", 
                    traceId, methodName, stopwatch.ElapsedMilliseconds);
                
                LogPerformance(methodName, stopwatch.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogError(ex, "METHOD_EXECUTION_ERROR", $"方法执行异常: {methodName}", new { MethodName = methodName, Duration = stopwatch.ElapsedMilliseconds });
                throw;
            }
        }
    }
} 