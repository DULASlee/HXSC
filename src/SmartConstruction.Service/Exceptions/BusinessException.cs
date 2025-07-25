using System;

namespace SmartConstruction.Service.Exceptions
{
    /// <summary>
    /// 代表一个业务逻辑相关的异常。
    /// 用于在应用程序中清晰地分离业务规则错误和系统级错误。
    /// </summary>
    public class BusinessException : Exception
    {
        /// <summary>
        /// 错误代码，可用于前端国际化或特定逻辑处理。
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// 初始化一个新的 BusinessException 实例。
        /// </summary>
        public BusinessException()
        {
        }

        /// <summary>
        /// 使用指定的错误消息初始化一个新的 BusinessException 实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        public BusinessException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// 使用指定的错误消息和错误代码初始化一个新的 BusinessException 实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        /// <param name="errorCode">业务错误代码。</param>
        public BusinessException(string message, string errorCode)
            : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 使用指定的错误消息和对导致此异常的内部异常的引用来初始化一个新的 BusinessException 实例。
        /// </summary>
        /// <param name="message">描述错误的消息。</param>
        /// <param name="innerException">导致当前异常的异常。</param>
        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
} 