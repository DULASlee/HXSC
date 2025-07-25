using Serilog;
using Serilog.Context;
using System.Diagnostics;
using System.Text.Json;

namespace SmartConstruction.Service.Infrastructure.Logging
{
    /// <summary>
    /// 日志中间件 - 重构版本
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Microsoft.Extensions.Logging.ILogger<LoggingMiddleware> _logger;
        private readonly Serilog.ILogger _auditLogger;

        public LoggingMiddleware(RequestDelegate next, Microsoft.Extensions.Logging.ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _auditLogger = LoggingConfiguration.CreateAuditLogger();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var traceId = LoggingConfiguration.GenerateTraceId();
            
            // 设置分布式追踪上下文
            using (LogContext.PushProperty("TraceId", traceId))
            using (LogContext.PushProperty("RequestPath", context.Request.Path))
            using (LogContext.PushProperty("RequestMethod", context.Request.Method))
            using (LogContext.PushProperty("UserAgent", context.Request.Headers.UserAgent.ToString()))
            using (LogContext.PushProperty("ClientIP", GetClientIP(context)))
            {
                try
                {
                    // 记录请求开始
                    _logger.LogInformation("API请求开始 | TraceId: {TraceId} | Path: {RequestPath} | Method: {RequestMethod}", 
                        traceId, context.Request.Path, context.Request.Method);

                    // 记录请求参数（脱敏处理）
                    if (context.Request.ContentLength > 0 && context.Request.ContentType?.Contains("application/json") == true)
                    {
                        var requestBody = await GetRequestBodyAsync(context);
                        var maskedBody = LoggingConfiguration.MaskSensitiveData(requestBody);
                        _logger.LogDebug("请求参数 | TraceId: {TraceId} | Body: {@RequestBody}", traceId, maskedBody);
                    }

                    // 执行下一个中间件
                    await _next(context);

                    // 记录响应
                    stopwatch.Stop();
                    _logger.LogInformation("API请求完成 | TraceId: {TraceId} | StatusCode: {StatusCode} | Duration: {Duration}ms", 
                        traceId, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);

                    // 审计日志
                    await LogAuditAsync(context, traceId, stopwatch.ElapsedMilliseconds, null);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    
                    // 记录异常详情
                    _logger.LogError(ex, "API请求异常 | TraceId: {TraceId} | ErrorCode: {ErrorCode} | Message: {Message} | StackTrace: {StackTrace}", 
                        traceId, 
                        GetErrorCode(ex), 
                        ex.Message, 
                        ex.StackTrace);

                    // 审计异常
                    await LogAuditAsync(context, traceId, stopwatch.ElapsedMilliseconds, ex);

                    // 重新抛出异常
                    throw;
                }
            }
        }

        /// <summary>
        /// 获取请求体
        /// </summary>
        private async Task<object> GetRequestBodyAsync(HttpContext context)
        {
            try
            {
                context.Request.EnableBuffering();
                var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;
                
                if (!string.IsNullOrEmpty(body))
                {
                    return JsonSerializer.Deserialize<object>(body) ?? body;
                }
                return string.Empty;
            }
            catch
            {
                return "无法读取请求体";
            }
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        private string GetClientIP(HttpContext context)
        {
            return context.Request.Headers["X-Forwarded-For"].FirstOrDefault() 
                ?? context.Request.Headers["X-Real-IP"].FirstOrDefault() 
                ?? context.Connection.RemoteIpAddress?.ToString() 
                ?? "Unknown";
        }

        /// <summary>
        /// 获取错误码
        /// </summary>
        private string GetErrorCode(Exception ex)
        {
            return ex switch
            {
                ArgumentException => "INVALID_ARGUMENT",
                UnauthorizedAccessException => "UNAUTHORIZED",
                InvalidOperationException => "INVALID_OPERATION",
                _ => "UNKNOWN_ERROR"
            };
        }

        /// <summary>
        /// 记录审计日志
        /// </summary>
        private async Task LogAuditAsync(HttpContext context, string traceId, long duration, Exception? exception)
        {
            try
            {
                var auditData = new
                {
                    TraceId = traceId,
                    Timestamp = DateTime.UtcNow,
                    RequestPath = context.Request.Path.ToString(),
                    RequestMethod = context.Request.Method,
                    StatusCode = context.Response.StatusCode,
                    Duration = duration,
                    UserId = context.User?.Identity?.Name ?? "Anonymous",
                    ClientIP = GetClientIP(context),
                    UserAgent = context.Request.Headers.UserAgent.ToString(),
                    Exception = exception != null ? new
                    {
                        Type = exception.GetType().Name,
                        Message = exception.Message,
                        StackTrace = exception.StackTrace
                    } : null
                };

                _auditLogger.Information("审计日志 | {@AuditData}", auditData);
            }
            catch (Exception auditEx)
            {
                _logger.LogWarning(auditEx, "审计日志记录失败 | TraceId: {TraceId}", traceId);
            }
        }
    }
} 