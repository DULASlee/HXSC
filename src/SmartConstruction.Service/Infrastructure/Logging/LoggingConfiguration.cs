using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Diagnostics;
using System.Text.Json;

namespace SmartConstruction.Service.Infrastructure.Logging
{
    /// <summary>
    /// 日志配置类 - 重构版本
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// 配置Serilog结构化日志 - 重构版本
        /// </summary>
        /// <param name="configuration">配置对象</param>
        /// <param name="environment">环境信息</param>
        /// <returns>配置好的Logger</returns>
        public static Serilog.ILogger ConfigureLogging(IConfiguration configuration, IWebHostEnvironment environment)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithProperty("Environment", environment.EnvironmentName)
                .Enrich.WithProperty("Service", "SmartConstruction.Service")
                .Enrich.WithProperty("Version", "1.0.0")
                .WriteTo.Console(
                    theme: AnsiConsoleTheme.Code,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/app-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}{NewLine}{Properties:j}{NewLine}")
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error || e.Level == LogEventLevel.Fatal)
                    .WriteTo.File(
                        path: "logs/errors-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 90,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}{NewLine}{Properties:j}{NewLine}"))
                .CreateLogger();

            return logger;
        }

        /// <summary>
        /// 创建性能日志记录器
        /// </summary>
        /// <returns>性能日志记录器</returns>
        public static Serilog.ILogger CreatePerformanceLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("LoggerType", "Performance")
                .WriteTo.File(
                    path: "logs/performance-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 7,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}{NewLine}{Properties:j}{NewLine}")
                .CreateLogger();
        }

        /// <summary>
        /// 创建审计日志记录器
        /// </summary>
        /// <returns>审计日志记录器</returns>
        public static Serilog.ILogger CreateAuditLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("LoggerType", "Audit")
                .WriteTo.File(
                    path: "logs/audit-.log",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{TraceId}] {Message:lj}{NewLine}{Exception}{NewLine}{Properties:j}{NewLine}")
                .CreateLogger();
        }

        /// <summary>
        /// 生成分布式追踪ID
        /// </summary>
        /// <returns>追踪ID</returns>
        public static string GenerateTraceId()
        {
            var now = DateTime.UtcNow;
            var guid = Guid.NewGuid().ToString("N")[..8];
            return $"req-{now:yyyyMMdd}-{guid}";
        }

        /// <summary>
        /// 安全脱敏处理
        /// </summary>
        /// <param name="data">原始数据</param>
        /// <returns>脱敏后的数据</returns>
        public static object MaskSensitiveData(object data)
        {
            if (data == null) return null;

            try
            {
                var json = JsonSerializer.Serialize(data);
                var sensitiveFields = new[] { "password", "token", "secret", "key", "api_key", "authorization" };
                
                foreach (var field in sensitiveFields)
                {
                    json = System.Text.RegularExpressions.Regex.Replace(
                        json, 
                        $"\"{field}\"\\s*:\\s*\"[^\"]*\"", 
                        $"\"{field}\":\"***\"",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                }

                return JsonSerializer.Deserialize<object>(json) ?? data;
            }
            catch
            {
                return data;
            }
        }
    }
} 