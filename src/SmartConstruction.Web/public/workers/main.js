// Web Worker主文件
// 任务处理器映射
const taskProcessors = {
  processData: processData,
  processImage: processImage,
  calculate: performCalculation,
  sortData: sortData,
  filterData: filterData,
  aggregateData: aggregateData,
  processText: processText,
  crypto: cryptoOperation
}

// 监听主线程消息
self.onmessage = function(event) {
  const { taskId, type, data } = event.data
  
  try {
    const processor = taskProcessors[type]
    if (!processor) {
      throw new Error(`Unknown task type: ${type}`)
    }

    // 执行任务
    const result = processor(data)
    
    // 如果结果是Promise，等待完成
    if (result instanceof Promise) {
      result
        .then(res => {
          self.postMessage({ taskId, result: res })
        })
        .catch(error => {
          self.postMessage({ taskId, error: error.message })
        })
    } else {
      // 同步结果
      self.postMessage({ taskId, result })
    }
  } catch (error) {
    self.postMessage({ taskId, error: error.message })
  }
}

// 数据处理
function processData({ data, processorType }) {
  switch (processorType) {
    case 'normalize':
      return normalizeData(data)
    case 'transform':
      return transformData(data)
    case 'validate':
      return validateData(data)
    default:
      return data
  }
}

// 数据标准化
function normalizeData(data) {
  if (Array.isArray(data)) {
    return data.map(item => {
      if (typeof item === 'object' && item !== null) {
        const normalized = {}
        for (const [key, value] of Object.entries(item)) {
          // 标准化键名（转为驼峰命名）
          const normalizedKey = key.replace(/_([a-z])/g, (_, letter) => letter.toUpperCase())
          normalized[normalizedKey] = value
        }
        return normalized
      }
      return item
    })
  }
  return data
}

// 数据转换
function transformData(data) {
  if (Array.isArray(data)) {
    return data.map(item => {
      if (typeof item === 'object' && item !== null) {
        // 转换日期字符串为Date对象
        const transformed = { ...item }
        for (const [key, value] of Object.entries(transformed)) {
          if (typeof value === 'string' && /^\d{4}-\d{2}-\d{2}/.test(value)) {
            transformed[key] = new Date(value)
          }
        }
        return transformed
      }
      return item
    })
  }
  return data
}

// 数据验证
function validateData(data) {
  const errors = []
  
  if (Array.isArray(data)) {
    data.forEach((item, index) => {
      if (typeof item === 'object' && item !== null) {
        // 检查必填字段
        if (!item.id) {
          errors.push(`Item at index ${index} is missing required field: id`)
        }
        // 检查数据类型
        if (item.age && typeof item.age !== 'number') {
          errors.push(`Item at index ${index} has invalid age type`)
        }
      }
    })
  }
  
  return {
    isValid: errors.length === 0,
    errors,
    data
  }
}

// 图像处理
function processImage({ imageData, options }) {
  const { width, height, data } = imageData
  const processedData = new Uint8ClampedArray(data)
  
  switch (options.type) {
    case 'grayscale':
      return applyGrayscale(processedData, width, height)
    case 'blur':
      return applyBlur(processedData, width, height, options.radius || 1)
    case 'brightness':
      return adjustBrightness(processedData, options.value || 0)
    case 'contrast':
      return adjustContrast(processedData, options.value || 1)
    default:
      return { data: processedData, width, height }
  }
}

// 灰度处理
function applyGrayscale(data, width, height) {
  for (let i = 0; i < data.length; i += 4) {
    const gray = data[i] * 0.299 + data[i + 1] * 0.587 + data[i + 2] * 0.114
    data[i] = gray     // R
    data[i + 1] = gray // G
    data[i + 2] = gray // B
    // Alpha通道保持不变
  }
  return { data, width, height }
}

// 模糊处理
function applyBlur(data, width, height, radius) {
  // 简单的盒式模糊
  const output = new Uint8ClampedArray(data)
  const size = radius * 2 + 1
  
  for (let y = 0; y < height; y++) {
    for (let x = 0; x < width; x++) {
      let r = 0, g = 0, b = 0, a = 0, count = 0
      
      for (let dy = -radius; dy <= radius; dy++) {
        for (let dx = -radius; dx <= radius; dx++) {
          const ny = y + dy
          const nx = x + dx
          
          if (ny >= 0 && ny < height && nx >= 0 && nx < width) {
            const idx = (ny * width + nx) * 4
            r += data[idx]
            g += data[idx + 1]
            b += data[idx + 2]
            a += data[idx + 3]
            count++
          }
        }
      }
      
      const idx = (y * width + x) * 4
      output[idx] = r / count
      output[idx + 1] = g / count
      output[idx + 2] = b / count
      output[idx + 3] = a / count
    }
  }
  
  return { data: output, width, height }
}

// 亮度调整
function adjustBrightness(data, value) {
  for (let i = 0; i < data.length; i += 4) {
    data[i] = Math.max(0, Math.min(255, data[i] + value))     // R
    data[i + 1] = Math.max(0, Math.min(255, data[i + 1] + value)) // G
    data[i + 2] = Math.max(0, Math.min(255, data[i + 2] + value)) // B
  }
  return data
}

// 对比度调整
function adjustContrast(data, value) {
  const factor = (259 * (value + 255)) / (255 * (259 - value))
  
  for (let i = 0; i < data.length; i += 4) {
    data[i] = Math.max(0, Math.min(255, factor * (data[i] - 128) + 128))     // R
    data[i + 1] = Math.max(0, Math.min(255, factor * (data[i + 1] - 128) + 128)) // G
    data[i + 2] = Math.max(0, Math.min(255, factor * (data[i + 2] - 128) + 128)) // B
  }
  return data
}

// 复杂计算
function performCalculation({ calculation, params }) {
  switch (calculation) {
    case 'fibonacci':
      return fibonacci(params.n || 10)
    case 'prime':
      return isPrime(params.n || 2)
    case 'factorial':
      return factorial(params.n || 1)
    case 'matrix':
      return matrixMultiply(params.a, params.b)
    case 'statistics':
      return calculateStatistics(params.data || [])
    default:
      throw new Error(`Unknown calculation: ${calculation}`)
  }
}

// 斐波那契数列
function fibonacci(n) {
  if (n <= 1) return n
  let a = 0, b = 1
  for (let i = 2; i <= n; i++) {
    [a, b] = [b, a + b]
  }
  return b
}

// 质数检测
function isPrime(n) {
  if (n < 2) return false
  if (n === 2) return true
  if (n % 2 === 0) return false
  
  for (let i = 3; i * i <= n; i += 2) {
    if (n % i === 0) return false
  }
  return true
}

// 阶乘
function factorial(n) {
  if (n <= 1) return 1
  let result = 1
  for (let i = 2; i <= n; i++) {
    result *= i
  }
  return result
}

// 矩阵乘法
function matrixMultiply(a, b) {
  const rows = a.length
  const cols = b[0].length
  const common = b.length
  const result = []
  
  for (let i = 0; i < rows; i++) {
    result[i] = []
    for (let j = 0; j < cols; j++) {
      let sum = 0
      for (let k = 0; k < common; k++) {
        sum += a[i][k] * b[k][j]
      }
      result[i][j] = sum
    }
  }
  
  return result
}

// 统计计算
function calculateStatistics(data) {
  if (!Array.isArray(data) || data.length === 0) {
    return { mean: 0, median: 0, mode: 0, std: 0 }
  }
  
  const sorted = [...data].sort((a, b) => a - b)
  const n = data.length
  
  // 平均值
  const mean = data.reduce((sum, val) => sum + val, 0) / n
  
  // 中位数
  const median = n % 2 === 0 
    ? (sorted[n / 2 - 1] + sorted[n / 2]) / 2
    : sorted[Math.floor(n / 2)]
  
  // 众数
  const frequency = {}
  data.forEach(val => {
    frequency[val] = (frequency[val] || 0) + 1
  })
  const mode = Object.keys(frequency).reduce((a, b) => 
    frequency[a] > frequency[b] ? a : b
  )
  
  // 标准差
  const variance = data.reduce((sum, val) => sum + Math.pow(val - mean, 2), 0) / n
  const std = Math.sqrt(variance)
  
  return { mean, median, mode: Number(mode), std }
}

// 数据排序
function sortData({ data, sortConfig }) {
  const { key, order = 'asc', custom } = sortConfig
  
  if (custom) {
    // 自定义排序函数
    return data.sort(new Function('a', 'b', custom))
  }
  
  return data.sort((a, b) => {
    let aVal = key ? a[key] : a
    let bVal = key ? b[key] : b
    
    // 处理不同数据类型
    if (typeof aVal === 'string') aVal = aVal.toLowerCase()
    if (typeof bVal === 'string') bVal = bVal.toLowerCase()
    
    if (aVal < bVal) return order === 'asc' ? -1 : 1
    if (aVal > bVal) return order === 'asc' ? 1 : -1
    return 0
  })
}

// 数据过滤
function filterData({ data, filterConfig }) {
  const { conditions, logic = 'and' } = filterConfig
  
  return data.filter(item => {
    const results = conditions.map(condition => {
      const { field, operator, value } = condition
      const itemValue = item[field]
      
      switch (operator) {
        case 'eq': return itemValue === value
        case 'ne': return itemValue !== value
        case 'gt': return itemValue > value
        case 'gte': return itemValue >= value
        case 'lt': return itemValue < value
        case 'lte': return itemValue <= value
        case 'contains': return String(itemValue).includes(value)
        case 'startsWith': return String(itemValue).startsWith(value)
        case 'endsWith': return String(itemValue).endsWith(value)
        default: return false
      }
    })
    
    return logic === 'and' 
      ? results.every(Boolean)
      : results.some(Boolean)
  })
}

// 数据聚合
function aggregateData({ data, aggregateConfig }) {
  const { groupBy, operations } = aggregateConfig
  
  if (!groupBy) {
    // 不分组，直接聚合
    return operations.reduce((result, op) => {
      result[op.name] = performAggregation(data, op)
      return result
    }, {})
  }
  
  // 分组聚合
  const groups = data.reduce((acc, item) => {
    const key = item[groupBy]
    if (!acc[key]) acc[key] = []
    acc[key].push(item)
    return acc
  }, {})
  
  const result = {}
  for (const [key, group] of Object.entries(groups)) {
    result[key] = operations.reduce((groupResult, op) => {
      groupResult[op.name] = performAggregation(group, op)
      return groupResult
    }, {})
  }
  
  return result
}

// 执行聚合操作
function performAggregation(data, operation) {
  const { type, field } = operation
  const values = field ? data.map(item => item[field]) : data
  
  switch (type) {
    case 'count':
      return data.length
    case 'sum':
      return values.reduce((sum, val) => sum + (Number(val) || 0), 0)
    case 'avg':
      const sum = values.reduce((sum, val) => sum + (Number(val) || 0), 0)
      return sum / values.length
    case 'min':
      return Math.min(...values.map(Number))
    case 'max':
      return Math.max(...values.map(Number))
    case 'first':
      return values[0]
    case 'last':
      return values[values.length - 1]
    default:
      return null
  }
}

// 文本处理
function processText({ text, operations }) {
  let result = text
  
  operations.forEach(operation => {
    switch (operation) {
      case 'uppercase':
        result = result.toUpperCase()
        break
      case 'lowercase':
        result = result.toLowerCase()
        break
      case 'trim':
        result = result.trim()
        break
      case 'removeSpaces':
        result = result.replace(/\s+/g, '')
        break
      case 'normalizeSpaces':
        result = result.replace(/\s+/g, ' ').trim()
        break
      case 'removeNumbers':
        result = result.replace(/\d/g, '')
        break
      case 'removeSpecialChars':
        result = result.replace(/[^a-zA-Z0-9\s]/g, '')
        break
      case 'wordCount':
        result = result.split(/\s+/).filter(word => word.length > 0).length
        break
    }
  })
  
  return result
}

// 加密/解密操作
function cryptoOperation({ data, operation, key }) {
  // 简单的Caesar密码实现
  const shift = key.length % 26
  
  if (operation === 'encrypt') {
    return data.replace(/[a-zA-Z]/g, char => {
      const start = char <= 'Z' ? 65 : 97
      return String.fromCharCode(((char.charCodeAt(0) - start + shift) % 26) + start)
    })
  } else if (operation === 'decrypt') {
    return data.replace(/[a-zA-Z]/g, char => {
      const start = char <= 'Z' ? 65 : 97
      return String.fromCharCode(((char.charCodeAt(0) - start - shift + 26) % 26) + start)
    })
  }
  
  return data
}