// ... existing code ...
      // [统一口径] 确保抛出的是我们自己创建的、信息完整的Error对象
      return Promise.reject(businessError)
    } else {
      return res
    }
  },
  (error) => {
    // [最终改造] 统一处理网络错误和HTTP错误
// ... existing code ...
