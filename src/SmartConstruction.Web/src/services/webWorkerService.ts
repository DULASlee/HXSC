// Web Worker服务
interface WorkerTask {
  id: string
  type: string
  data: any
  resolve: (result: any) => void
  reject: (error: any) => void
  timeout?: number
}

interface WorkerConfig {
  maxWorkers: number
  taskTimeout: number
  workerScript: string
}

class WebWorkerService {
  private workers: Worker[] = []
  private availableWorkers: Worker[] = []
  private busyWorkers: Set<Worker> = new Set()
  private taskQueue: WorkerTask[] = []
  private activeTasks: Map<string, WorkerTask> = new Map()
  private config: WorkerConfig

  constructor(config: Partial<WorkerConfig> = {}) {
    this.config = {
      maxWorkers: navigator.hardwareConcurrency || 4,
      taskTimeout: 30000, // 30秒
      workerScript: '/workers/main.js',
      ...config
    }

    this.initWorkers()
  }

  // 初始化Worker池
  private initWorkers() {
    for (let i = 0; i < this.config.maxWorkers; i++) {
      this.createWorker()
    }
  }

  // 创建Worker
  private createWorker(): Worker {
    try {
      const worker = new Worker(this.config.workerScript)
      
      worker.onmessage = (event) => {
        this.handleWorkerMessage(worker, event)
      }

      worker.onerror = (error) => {
        this.handleWorkerError(worker, error)
      }

      this.workers.push(worker)
      this.availableWorkers.push(worker)
      
      return worker
    } catch (error) {
      console.error('Failed to create worker:', error)
      throw error
    }
  }

  // 处理Worker消息
  private handleWorkerMessage(worker: Worker, event: MessageEvent) {
    const { taskId, result, error } = event.data

    const task = this.activeTasks.get(taskId)
    if (!task) {
      console.warn('Received message for unknown task:', taskId)
      return
    }

    // 清理任务
    this.activeTasks.delete(taskId)
    this.releaseWorker(worker)

    // 处理结果
    if (error) {
      task.reject(new Error(error))
    } else {
      task.resolve(result)
    }

    // 处理队列中的下一个任务
    this.processQueue()
  }

  // 处理Worker错误
  private handleWorkerError(worker: Worker, error: ErrorEvent) {
    console.error('Worker error:', error)

    // 找到使用此Worker的任务
    for (const [taskId, task] of this.activeTasks.entries()) {
      // 简单的错误处理，实际项目中可能需要更复杂的逻辑
      task.reject(new Error(`Worker error: ${error.message}`))
      this.activeTasks.delete(taskId)
    }

    // 重新创建Worker
    this.replaceWorker(worker)
  }

  // 替换损坏的Worker
  private replaceWorker(brokenWorker: Worker) {
    // 从数组中移除损坏的Worker
    const workerIndex = this.workers.indexOf(brokenWorker)
    if (workerIndex !== -1) {
      this.workers.splice(workerIndex, 1)
    }

    const availableIndex = this.availableWorkers.indexOf(brokenWorker)
    if (availableIndex !== -1) {
      this.availableWorkers.splice(availableIndex, 1)
    }

    this.busyWorkers.delete(brokenWorker)

    // 终止损坏的Worker
    brokenWorker.terminate()

    // 创建新的Worker
    this.createWorker()
  }

  // 获取可用的Worker
  private getAvailableWorker(): Worker | null {
    return this.availableWorkers.pop() || null
  }

  // 释放Worker
  private releaseWorker(worker: Worker) {
    this.busyWorkers.delete(worker)
    this.availableWorkers.push(worker)
  }

  // 处理任务队列
  private processQueue() {
    while (this.taskQueue.length > 0 && this.availableWorkers.length > 0) {
      const task = this.taskQueue.shift()!
      const worker = this.getAvailableWorker()!
      
      this.executeTask(worker, task)
    }
  }

  // 执行任务
  private executeTaskInternal(worker: Worker, task: WorkerTask) {
    this.busyWorkers.add(worker)
    this.activeTasks.set(task.id, task)

    // 设置超时
    if (task.timeout) {
      setTimeout(() => {
        if (this.activeTasks.has(task.id)) {
          this.activeTasks.delete(task.id)
          this.releaseWorker(worker)
          task.reject(new Error('Task timeout'))
        }
      }, task.timeout)
    }

    // 发送任务到Worker
    worker.postMessage({
      taskId: task.id,
      type: task.type,
      data: task.data
    })
  }

  // 执行任务
  async executeTask<T = any>(
    type: string, 
    data: any, 
    timeout?: number
  ): Promise<T> {
    return new Promise((resolve, reject) => {
      const task: WorkerTask = {
        id: `task_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`,
        type,
        data,
        resolve,
        reject,
        timeout: timeout || this.config.taskTimeout
      }

      const availableWorker = this.getAvailableWorker()
      
      if (availableWorker) {
        // 立即执行任务
        this.executeTaskInternal(availableWorker, task)
      } else {
        // 添加到队列
        this.taskQueue.push(task)
      }
    })
  }

  // 批量执行任务
  async executeBatch<T = any>(
    tasks: Array<{ type: string; data: any }>,
    timeout?: number
  ): Promise<T[]> {
    const promises = tasks.map(task => 
      this.executeTask<T>(task.type, task.data, timeout)
    )
    
    return Promise.all(promises)
  }

  // 数据处理任务
  async processData<T = any>(data: any, processorType: string = 'default'): Promise<T> {
    return this.executeTask('processData', { data, processorType })
  }

  // 图像处理任务
  async processImage(
    imageData: ImageData | ArrayBuffer, 
    options: any = {}
  ): Promise<ImageData> {
    return this.executeTask('processImage', { imageData, options })
  }

  // 复杂计算任务
  async performCalculation<T = any>(
    calculation: string, 
    params: any = {}
  ): Promise<T> {
    return this.executeTask('calculate', { calculation, params })
  }

  // 数据排序任务
  async sortData<T = any>(
    data: T[], 
    sortConfig: { key?: string; order?: 'asc' | 'desc'; custom?: string }
  ): Promise<T[]> {
    return this.executeTask('sortData', { data, sortConfig })
  }

  // 数据过滤任务
  async filterData<T = any>(
    data: T[], 
    filterConfig: { conditions: any[]; logic?: 'and' | 'or' }
  ): Promise<T[]> {
    return this.executeTask('filterData', { data, filterConfig })
  }

  // 数据聚合任务
  async aggregateData<T = any>(
    data: T[], 
    aggregateConfig: { groupBy?: string; operations: any[] }
  ): Promise<any> {
    return this.executeTask('aggregateData', { data, aggregateConfig })
  }

  // 文本处理任务
  async processText(
    text: string, 
    operations: string[]
  ): Promise<string> {
    return this.executeTask('processText', { text, operations })
  }

  // 加密/解密任务
  async cryptoOperation(
    data: string, 
    operation: 'encrypt' | 'decrypt', 
    key: string
  ): Promise<string> {
    return this.executeTask('crypto', { data, operation, key })
  }

  // 获取Worker状态
  getWorkerStatus() {
    return {
      totalWorkers: this.workers.length,
      availableWorkers: this.availableWorkers.length,
      busyWorkers: this.busyWorkers.size,
      queuedTasks: this.taskQueue.length,
      activeTasks: this.activeTasks.size
    }
  }

  // 清空任务队列
  clearQueue() {
    this.taskQueue.forEach(task => {
      task.reject(new Error('Task cancelled'))
    })
    this.taskQueue.length = 0
  }

  // 取消特定任务
  cancelTask(taskId: string): boolean {
    // 从队列中移除
    const queueIndex = this.taskQueue.findIndex(task => task.id === taskId)
    if (queueIndex !== -1) {
      const task = this.taskQueue.splice(queueIndex, 1)[0]
      task.reject(new Error('Task cancelled'))
      return true
    }

    // 取消活动任务（这里只能标记，实际取消需要Worker支持）
    const activeTask = this.activeTasks.get(taskId)
    if (activeTask) {
      this.activeTasks.delete(taskId)
      activeTask.reject(new Error('Task cancelled'))
      return true
    }

    return false
  }

  // 销毁所有Worker
  destroy() {
    // 清空队列
    this.clearQueue()

    // 取消所有活动任务
    this.activeTasks.forEach(task => {
      task.reject(new Error('Service destroyed'))
    })
    this.activeTasks.clear()

    // 终止所有Worker
    this.workers.forEach(worker => {
      worker.terminate()
    })

    // 清空数组
    this.workers.length = 0
    this.availableWorkers.length = 0
    this.busyWorkers.clear()
  }

  // 动态调整Worker数量
  adjustWorkerCount(newCount: number) {
    const currentCount = this.workers.length
    
    if (newCount > currentCount) {
      // 增加Worker
      for (let i = currentCount; i < newCount; i++) {
        this.createWorker()
      }
    } else if (newCount < currentCount) {
      // 减少Worker
      const workersToRemove = currentCount - newCount
      for (let i = 0; i < workersToRemove; i++) {
        const worker = this.availableWorkers.pop()
        if (worker) {
          const index = this.workers.indexOf(worker)
          if (index !== -1) {
            this.workers.splice(index, 1)
          }
          worker.terminate()
        }
      }
    }

    this.config.maxWorkers = newCount
  }
}

// 创建默认实例
export const webWorkerService = new WebWorkerService()

// 导出类以便创建自定义实例
export { WebWorkerService }

// 便捷函数
export const useWebWorker = () => {
  return {
    processData: webWorkerService.processData.bind(webWorkerService),
    processImage: webWorkerService.processImage.bind(webWorkerService),
    performCalculation: webWorkerService.performCalculation.bind(webWorkerService),
    sortData: webWorkerService.sortData.bind(webWorkerService),
    filterData: webWorkerService.filterData.bind(webWorkerService),
    aggregateData: webWorkerService.aggregateData.bind(webWorkerService),
    processText: webWorkerService.processText.bind(webWorkerService),
    cryptoOperation: webWorkerService.cryptoOperation.bind(webWorkerService),
    getStatus: webWorkerService.getWorkerStatus.bind(webWorkerService),
    executeBatch: webWorkerService.executeBatch.bind(webWorkerService)
  }
}

export default webWorkerService