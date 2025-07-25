namespace SmartConstruction.Service.Models
{
    /// <summary>
    /// 非泛型 API 响应包装类
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// 操作是否成功（不再使用静态属性）
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// 创建成功响应（兼容现有调用方式）
        /// </summary>
        public static ApiResponse CreateSuccess(object? data = null, string message = "操作成功")
        {
            return new ApiResponse
            {
                Success = true, // 使用实例属性而不是静态属性
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 创建失败响应（兼容现有调用方式）
        /// </summary>
        public static ApiResponse CreateError(string message)
        {
            return new ApiResponse
            {
                Success = false, // 使用实例属性而不是静态属性
                Message = message
            };
        }

        /// <summary>
        /// 创建失败响应（兼容现有调用方式）
        /// </summary>
        public static ApiResponse Failure(string message)
        {
            return CreateError(message);
        }
    }

    /// <summary>
    /// 泛型 API 响应包装类
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess { get; set; }  // 重命名为 IsSuccess 避免方法名冲突

        /// <summary>
        /// 消息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 创建失败响应（兼容现有调用方式）
        /// </summary>
        public static ApiResponse<T> Error(string message)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,  // 使用重命名的属性
                Message = message
            };
        }

        /// <summary>
        /// 创建失败响应（兼容现有调用方式）
        /// </summary>
        public static ApiResponse<T> Failure(string message)
        {
            return Error(message);
        }

        /// <summary>
        /// 创建成功响应（兼容现有调用方式）
        /// </summary>
        public static ApiResponse<T> Success(T data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,  // 使用重命名的属性
                Message = message,
                Data = data
            };
        }
    }
}
