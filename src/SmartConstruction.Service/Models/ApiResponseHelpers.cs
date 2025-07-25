namespace SmartConstruction.Service.Models
{
    internal static class ApiResponseHelpers<T>
    {

        /// <summary>
        /// 创建成功响应
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="message">消息</param>
        /// <returns>成功响应</returns>
        public static ApiResponse<T> Success(T? data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// 创建错误响应
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="data">数据</param>
        /// <returns>错误响应</returns>
        public static ApiResponse<T> Error(string message, T? data = default)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Data = data
            };
        }
    }
}