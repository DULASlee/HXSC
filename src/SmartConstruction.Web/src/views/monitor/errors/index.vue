<template>
  <div class="error-monitor-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>错误监控面板</span>
          <div class="header-actions">
            <el-button type="primary" size="small" @click="refreshData">
              <el-icon><Refresh /></el-icon> 刷新
            </el-button>
            <el-button type="danger" size="small" @click="clearErrors">
              <el-icon><Delete /></el-icon> 清空
            </el-button>
          </div>
        </div>
      </template>
      
      <el-tabs v-model="activeTab">
        <el-tab-pane label="错误列表" name="errors">
          <el-form :inline="true" class="filter-form">
            <el-form-item label="错误类型">
              <el-select v-model="filters.type" placeholder="全部" clearable>
                <el-option v-for="type in errorTypes" :key="type.value" :label="type.label" :value="type.value" />
              </el-select>
            </el-form-item>
            <el-form-item label="时间范围">
              <el-date-picker
                v-model="filters.timeRange"
                type="datetimerange"
                range-separator="至"
                start-placeholder="开始时间"
                end-placeholder="结束时间"
                format="YYYY-MM-DD HH:mm:ss"
                value-format="YYYY-MM-DD HH:mm:ss"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="applyFilters">筛选</el-button>
              <el-button @click="resetFilters">重置</el-button>
            </el-form-item>
          </el-form>
          
          <el-table
            v-loading="loading"
            :data="filteredErrors"
            style="width: 100%"
            border
            stripe
            :default-sort="{ prop: 'timestamp', order: 'descending' }"
          >
            <el-table-column type="expand">
              <template #default="props">
                <div class="error-details">
                  <pre>{{ formatErrorDetails(props.row) }}</pre>
                </div>
              </template>
            </el-table-column>
            <el-table-column prop="type" label="类型" width="120">
              <template #default="scope">
                <el-tag :type="getErrorTagType(scope.row.type)">{{ getErrorTypeLabel(scope.row.type) }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="code" label="错误码" width="150" />
            <el-table-column prop="message" label="错误信息" min-width="250" show-overflow-tooltip />
            <el-table-column prop="timestamp" label="时间" width="180" sortable>
              <template #default="scope">
                {{ formatTime(scope.row.timestamp) }}
              </template>
            </el-table-column>
            <el-table-column prop="url" label="URL" min-width="200" show-overflow-tooltip />
            <el-table-column label="操作" width="120" fixed="right">
              <template #default="scope">
                <el-button type="primary" link @click="showErrorDetail(scope.row)">
                  详情
                </el-button>
                <el-button type="danger" link @click="removeError(scope.row)">
                  删除
                </el-button>
              </template>
            </el-table-column>
          </el-table>
          
          <div class="pagination-container">
            <el-pagination
              v-model:current-page="currentPage"
              v-model:page-size="pageSize"
              :page-sizes="[10, 20, 50, 100]"
              layout="total, sizes, prev, pager, next, jumper"
              :total="filteredErrors.length"
              @size-change="handleSizeChange"
              @current-change="handleCurrentChange"
            />
          </div>
        </el-tab-pane>
        
        <el-tab-pane label="日志记录" name="logs">
          <el-form :inline="true" class="filter-form">
            <el-form-item label="日志级别">
              <el-select v-model="logFilters.level" placeholder="全部" clearable>
                <el-option v-for="level in logLevels" :key="level.value" :label="level.label" :value="level.value" />
              </el-select>
            </el-form-item>
            <el-form-item label="时间范围">
              <el-date-picker
                v-model="logFilters.timeRange"
                type="datetimerange"
                range-separator="至"
                start-placeholder="开始时间"
                end-placeholder="结束时间"
                format="YYYY-MM-DD HH:mm:ss"
                value-format="YYYY-MM-DD HH:mm:ss"
              />
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="applyLogFilters">筛选</el-button>
              <el-button @click="resetLogFilters">重置</el-button>
            </el-form-item>
          </el-form>
          
          <el-table
            v-loading="logLoading"
            :data="filteredLogs"
            style="width: 100%"
            border
            stripe
            :default-sort="{ prop: 'timestamp', order: 'descending' }"
          >
            <el-table-column type="expand">
              <template #default="props">
                <div class="log-details">
                  <pre>{{ formatLogDetails(props.row) }}</pre>
                </div>
              </template>
            </el-table-column>
            <el-table-column prop="level" label="级别" width="100">
              <template #default="scope">
                <el-tag :type="getLogLevelTagType(scope.row.level)">{{ getLogLevelLabel(scope.row.level) }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="message" label="消息" min-width="250" show-overflow-tooltip />
            <el-table-column prop="timestamp" label="时间" width="180" sortable>
              <template #default="scope">
                {{ formatTime(scope.row.timestamp) }}
              </template>
            </el-table-column>
            <el-table-column prop="source" label="来源" width="120" />
            <el-table-column prop="user.username" label="用户" width="120" />
            <el-table-column label="操作" width="120" fixed="right">
              <template #default="scope">
                <el-button type="primary" link @click="showLogDetail(scope.row)">
                  详情
                </el-button>
              </template>
            </el-table-column>
          </el-table>
          
          <div class="pagination-container">
            <el-pagination
              v-model:current-page="logCurrentPage"
              v-model:page-size="logPageSize"
              :page-sizes="[10, 20, 50, 100]"
              layout="total, sizes, prev, pager, next, jumper"
              :total="filteredLogs.length"
              @size-change="handleLogSizeChange"
              @current-change="handleLogCurrentChange"
            />
          </div>
        </el-tab-pane>
        
        <el-tab-pane label="统计分析" name="stats">
          <el-row :gutter="20">
            <el-col :span="12">
              <div class="chart-container">
                <h3>错误类型分布</h3>
                <div id="errorTypeChart" style="width: 100%; height: 300px;"></div>
              </div>
            </el-col>
            <el-col :span="12">
              <div class="chart-container">
                <h3>错误趋势</h3>
                <div id="errorTrendChart" style="width: 100%; height: 300px;"></div>
              </div>
            </el-col>
          </el-row>
          
          <el-row :gutter="20" class="mt-20">
            <el-col :span="24">
              <div class="chart-container">
                <h3>常见错误TOP 10</h3>
                <el-table
                  :data="topErrors"
                  style="width: 100%"
                  border
                  stripe
                >
                  <el-table-column prop="code" label="错误码" width="150" />
                  <el-table-column prop="message" label="错误信息" min-width="250" show-overflow-tooltip />
                  <el-table-column prop="count" label="出现次数" width="120" />
                  <el-table-column prop="percentage" label="占比" width="120">
                    <template #default="scope">
                      {{ scope.row.percentage }}%
                    </template>
                  </el-table-column>
                  <el-table-column prop="lastOccurred" label="最近出现" width="180">
                    <template #default="scope">
                      {{ formatTime(scope.row.lastOccurred) }}
                    </template>
                  </el-table-column>
                </el-table>
              </div>
            </el-col>
          </el-row>
        </el-tab-pane>
        
        <el-tab-pane label="设置" name="settings">
          <el-form label-width="120px" class="settings-form">
            <el-form-item label="错误历史记录">
              <el-input-number v-model="settings.maxErrorHistory" :min="10" :max="1000" />
              <span class="form-help">保留的最大错误历史记录数量</span>
            </el-form-item>
            
            <el-form-item label="日志自动上报">
              <el-switch v-model="settings.autoReport" />
              <span class="form-help">是否自动上报错误日志到服务器</span>
            </el-form-item>
            
            <el-form-item label="上报间隔">
              <el-input-number v-model="settings.reportInterval" :min="10000" :max="300000" :step="10000" />
              <span class="form-help">自动上报的时间间隔（毫秒）</span>
            </el-form-item>
            
            <el-form-item label="上报URL">
              <el-input v-model="settings.reportUrl" placeholder="日志上报的服务器地址" />
            </el-form-item>
            
            <el-form-item>
              <el-button type="primary" @click="saveSettings">保存设置</el-button>
              <el-button @click="resetSettings">重置</el-button>
            </el-form-item>
          </el-form>
        </el-tab-pane>
      </el-tabs>
    </el-card>
    
    <!-- 错误详情对话框 -->
    <el-dialog
      v-model="errorDetailVisible"
      title="错误详情"
      width="60%"
      destroy-on-close
    >
      <div v-if="selectedError" class="error-detail-content">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="错误类型">
            <el-tag :type="getErrorTagType(selectedError.type)">{{ getErrorTypeLabel(selectedError.type) }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="错误码">{{ selectedError.code }}</el-descriptions-item>
          <el-descriptions-item label="错误消息">{{ selectedError.message }}</el-descriptions-item>
          <el-descriptions-item label="发生时间">{{ formatTime(selectedError.timestamp) }}</el-descriptions-item>
          <el-descriptions-item label="URL" v-if="selectedError.url">{{ selectedError.url }}</el-descriptions-item>
          <el-descriptions-item label="详细信息" v-if="selectedError.details">
            <pre>{{ selectedError.details }}</pre>
          </el-descriptions-item>
          <el-descriptions-item label="堆栈信息" v-if="selectedError.stack">
            <pre>{{ selectedError.stack }}</pre>
          </el-descriptions-item>
        </el-descriptions>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="errorDetailVisible = false">关闭</el-button>
        </span>
      </template>
    </el-dialog>
    
    <!-- 日志详情对话框 -->
    <el-dialog
      v-model="logDetailVisible"
      title="日志详情"
      width="60%"
      destroy-on-close
    >
      <div v-if="selectedLog" class="log-detail-content">
        <el-descriptions :column="1" border>
          <el-descriptions-item label="日志级别">
            <el-tag :type="getLogLevelTagType(selectedLog.level)">{{ getLogLevelLabel(selectedLog.level) }}</el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="消息">{{ selectedLog.message }}</el-descriptions-item>
          <el-descriptions-item label="时间">{{ formatTime(selectedLog.timestamp) }}</el-descriptions-item>
          <el-descriptions-item label="来源">{{ selectedLog.source }}</el-descriptions-item>
          <el-descriptions-item label="用户" v-if="selectedLog.user">
            {{ selectedLog.user.username || selectedLog.user.id || '未知用户' }}
          </el-descriptions-item>
          <el-descriptions-item label="URL" v-if="selectedLog.url">{{ selectedLog.url }}</el-descriptions-item>
          <el-descriptions-item label="详细信息" v-if="selectedLog.details">
            <pre>{{ selectedLog.details }}</pre>
          </el-descriptions-item>
          <el-descriptions-item label="数据" v-if="selectedLog.data">
            <pre>{{ JSON.stringify(selectedLog.data, null, 2) }}</pre>
          </el-descriptions-item>
        </el-descriptions>
      </div>
      <template #footer>
        <span class="dialog-footer">
          <el-button @click="logDetailVisible = false">关闭</el-button>
        </span>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, onMounted, onUnmounted, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Refresh, Delete } from '@element-plus/icons-vue'
import { errorService } from '@/services/errorService'
import { ErrorType } from '@/utils/error'
import { logService, LogLevel, type LogEntry } from '@/services/logService'
import type { ErrorInfo } from '@/utils/error'
import * as echarts from 'echarts/core'
import { PieChart, LineChart, BarChart } from 'echarts/charts'
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent
} from 'echarts/components'
import { CanvasRenderer } from 'echarts/renderers'

// 注册 ECharts 组件
echarts.use([
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  GridComponent,
  PieChart,
  LineChart,
  BarChart,
  CanvasRenderer
])

// 活动标签页
const activeTab = ref('errors')

// 错误列表相关
const loading = ref(false)
const errors = ref<ErrorInfo[]>([])
const currentPage = ref(1)
const pageSize = ref(10)
const filters = reactive({
  type: '',
  timeRange: [] as string[]
})

// 日志列表相关
const logLoading = ref(false)
const logs = ref<LogEntry[]>([])
const logCurrentPage = ref(1)
const logPageSize = ref(10)
const logFilters = reactive({
  level: '',
  timeRange: [] as string[]
})

// 错误详情对话框
const errorDetailVisible = ref(false)
const selectedError = ref<ErrorInfo | null>(null)

// 日志详情对话框
const logDetailVisible = ref(false)
const selectedLog = ref<LogEntry | null>(null)

// 设置
const settings = reactive({
  maxErrorHistory: 50,
  autoReport: true,
  reportInterval: 60000,
  reportUrl: '/api/logs'
})

// 错误类型选项
const errorTypes = [
  { value: ErrorType.NETWORK, label: '网络错误' },
  { value: ErrorType.AUTH, label: '认证错误' },
  { value: ErrorType.PERMISSION, label: '权限错误' },
  { value: ErrorType.BUSINESS, label: '业务错误' },
  { value: ErrorType.SERVER, label: '服务器错误' },
  { value: ErrorType.CLIENT, label: '客户端错误' },
  { value: ErrorType.UNKNOWN, label: '未知错误' }
]

// 日志级别选项
const logLevels = [
  { value: LogLevel.DEBUG, label: '调试' },
  { value: LogLevel.INFO, label: '信息' },
  { value: LogLevel.WARN, label: '警告' },
  { value: LogLevel.ERROR, label: '错误' },
  { value: LogLevel.FATAL, label: '致命' }
]

// 过滤后的错误列表
const filteredErrors = computed(() => {
  let result = [...errors.value]
  
  // 按类型过滤
  if (filters.type) {
    result = result.filter(error => error.type === filters.type)
  }
  
  // 按时间范围过滤
  if (filters.timeRange && filters.timeRange.length === 2) {
    const startTime = new Date(filters.timeRange[0]).getTime()
    const endTime = new Date(filters.timeRange[1]).getTime()
    result = result.filter(error => error.timestamp >= startTime && error.timestamp <= endTime)
  }
  
  return result
})

// 分页后的错误列表
const paginatedErrors = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value
  const end = start + pageSize.value
  return filteredErrors.value.slice(start, end)
})

// 过滤后的日志列表
const filteredLogs = computed(() => {
  let result = [...logs.value]
  
  // 按级别过滤
  if (logFilters.level) {
    result = result.filter(log => log.level === logFilters.level)
  }
  
  // 按时间范围过滤
  if (logFilters.timeRange && logFilters.timeRange.length === 2) {
    const startTime = new Date(logFilters.timeRange[0]).getTime()
    const endTime = new Date(logFilters.timeRange[1]).getTime()
    result = result.filter(log => log.timestamp >= startTime && log.timestamp <= endTime)
  }
  
  return result
})

// 分页后的日志列表
const paginatedLogs = computed(() => {
  const start = (logCurrentPage.value - 1) * logPageSize.value
  const end = start + logPageSize.value
  return filteredLogs.value.slice(start, end)
})

// 错误统计 - TOP 10
const topErrors = computed(() => {
  const errorMap = new Map<string, { code: string; message: string; count: number; lastOccurred: number }>()
  
  errors.value.forEach(error => {
    const key = `${error.code}:${error.message}`
    if (errorMap.has(key)) {
      const item = errorMap.get(key)!
      item.count++
      item.lastOccurred = Math.max(item.lastOccurred, error.timestamp)
    } else {
      errorMap.set(key, {
        code: error.code,
        message: error.message,
        count: 1,
        lastOccurred: error.timestamp
      })
    }
  })
  
  const totalErrors = errors.value.length
  
  return Array.from(errorMap.values())
    .sort((a, b) => b.count - a.count)
    .slice(0, 10)
    .map(item => ({
      ...item,
      percentage: totalErrors > 0 ? ((item.count / totalErrors) * 100).toFixed(2) : '0.00'
    }))
})

// 获取错误类型标签类型
const getErrorTagType = (type: string) => {
  switch (type) {
    case ErrorType.NETWORK:
      return 'warning'
    case ErrorType.AUTH:
      return 'danger'
    case ErrorType.PERMISSION:
      return 'danger'
    case ErrorType.BUSINESS:
      return 'warning'
    case ErrorType.SERVER:
      return 'danger'
    case ErrorType.CLIENT:
      return 'warning'
    default:
      return 'info'
  }
}

// 获取错误类型标签文本
const getErrorTypeLabel = (type: string) => {
  const errorType = errorTypes.find(t => t.value === type)
  return errorType ? errorType.label : '未知类型'
}

// 获取日志级别标签类型
const getLogLevelTagType = (level: string) => {
  switch (level) {
    case LogLevel.DEBUG:
      return 'info'
    case LogLevel.INFO:
      return 'success'
    case LogLevel.WARN:
      return 'warning'
    case LogLevel.ERROR:
      return 'danger'
    case LogLevel.FATAL:
      return 'danger'
    default:
      return 'info'
  }
}

// 获取日志级别标签文本
const getLogLevelLabel = (level: string) => {
  const logLevel = logLevels.find(l => l.value === level)
  return logLevel ? logLevel.label : '未知级别'
}

// 格式化时间
const formatTime = (timestamp: number) => {
  return new Date(timestamp).toLocaleString()
}

// 格式化错误详情
const formatErrorDetails = (error: ErrorInfo) => {
  return `错误类型: ${error.type}
错误码: ${error.code}
错误消息: ${error.message}
${error.details ? `详情: ${error.details}` : ''}
${error.url ? `URL: ${error.url}` : ''}
${error.stack ? `堆栈: ${error.stack}` : ''}`
}

// 格式化日志详情
const formatLogDetails = (log: LogEntry) => {
  return `级别: ${log.level}
消息: ${log.message}
时间: ${formatTime(log.timestamp)}
来源: ${log.source}
${log.user ? `用户: ${log.user.username || log.user.id || '未知用户'}` : ''}
${log.url ? `URL: ${log.url}` : ''}
${log.details ? `详情: ${log.details}` : ''}
${log.data ? `数据: ${JSON.stringify(log.data, null, 2)}` : ''}`
}

// 刷新数据
const refreshData = () => {
  fetchErrors()
  fetchLogs()
}

// 获取错误列表
const fetchErrors = () => {
  loading.value = true
  try {
    errors.value = errorService.getErrorHistory()
  } catch (error) {
    ElMessage.error('获取错误列表失败')
    console.error('Failed to fetch errors:', error)
  } finally {
    loading.value = false
  }
}

// 获取日志列表
const fetchLogs = () => {
  logLoading.value = true
  try {
    logs.value = logService.getLogs()
  } catch (error) {
    ElMessage.error('获取日志列表失败')
    console.error('Failed to fetch logs:', error)
  } finally {
    logLoading.value = false
  }
}

// 清空错误
const clearErrors = () => {
  ElMessageBox.confirm('确定要清空所有错误记录吗？', '警告', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(() => {
    errorService.clearErrorHistory()
    errors.value = []
    ElMessage.success('错误记录已清空')
  })
}

// 删除单个错误
const removeError = (error: ErrorInfo) => {
  const index = errors.value.findIndex(e => e === error)
  if (index !== -1) {
    errors.value.splice(index, 1)
    ElMessage.success('错误记录已删除')
  }
}

// 显示错误详情
const showErrorDetail = (error: ErrorInfo) => {
  selectedError.value = error
  errorDetailVisible.value = true
}

// 显示日志详情
const showLogDetail = (log: LogEntry) => {
  selectedLog.value = log
  logDetailVisible.value = true
}

// 应用错误筛选
const applyFilters = () => {
  currentPage.value = 1
}

// 重置错误筛选
const resetFilters = () => {
  filters.type = ''
  filters.timeRange = []
  currentPage.value = 1
}

// 应用日志筛选
const applyLogFilters = () => {
  logCurrentPage.value = 1
}

// 重置日志筛选
const resetLogFilters = () => {
  logFilters.level = ''
  logFilters.timeRange = []
  logCurrentPage.value = 1
}

// 处理错误分页大小变化
const handleSizeChange = (size: number) => {
  pageSize.value = size
  currentPage.value = 1
}

// 处理错误当前页变化
const handleCurrentChange = (page: number) => {
  currentPage.value = page
}

// 处理日志分页大小变化
const handleLogSizeChange = (size: number) => {
  logPageSize.value = size
  logCurrentPage.value = 1
}

// 处理日志当前页变化
const handleLogCurrentChange = (page: number) => {
  logCurrentPage.value = page
}

// 保存设置
const saveSettings = () => {
  try {
    // 更新错误服务设置
    // errorService.setMaxHistorySize(settings.maxErrorHistory)
    
    // 更新日志服务设置
    logService.setAutoReport(settings.autoReport)
    logService.setReportUrl(settings.reportUrl)
    logService.setReportInterval(settings.reportInterval)
    
    ElMessage.success('设置已保存')
  } catch (error) {
    ElMessage.error('保存设置失败')
    console.error('Failed to save settings:', error)
  }
}

// 重置设置
const resetSettings = () => {
  settings.maxErrorHistory = 50
  settings.autoReport = true
  settings.reportInterval = 60000
  settings.reportUrl = '/api/logs'
}

// 初始化图表
let errorTypeChart: echarts.ECharts | null = null
let errorTrendChart: echarts.ECharts | null = null

// 初始化错误类型分布图表
const initErrorTypeChart = () => {
  const chartDom = document.getElementById('errorTypeChart')
  if (!chartDom) return
  
  errorTypeChart = echarts.init(chartDom)
  
  const typeCount = new Map<string, number>()
  errors.value.forEach(error => {
    const count = typeCount.get(error.type) || 0
    typeCount.set(error.type, count + 1)
  })
  
  const data = Array.from(typeCount.entries()).map(([type, count]) => ({
    name: getErrorTypeLabel(type),
    value: count
  }))
  
  const option = {
    title: {
      text: '错误类型分布',
      left: 'center'
    },
    tooltip: {
      trigger: 'item',
      formatter: '{a} <br/>{b}: {c} ({d}%)'
    },
    legend: {
      orient: 'vertical',
      left: 'left',
      data: data.map(item => item.name)
    },
    series: [
      {
        name: '错误类型',
        type: 'pie',
        radius: '50%',
        data,
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowOffsetX: 0,
            shadowColor: 'rgba(0, 0, 0, 0.5)'
          }
        }
      }
    ]
  }
  
  errorTypeChart.setOption(option)
}

// 初始化错误趋势图表
const initErrorTrendChart = () => {
  const chartDom = document.getElementById('errorTrendChart')
  if (!chartDom) return
  
  errorTrendChart = echarts.init(chartDom)
  
  // 按天统计错误数量
  const today = new Date()
  today.setHours(0, 0, 0, 0)
  
  const days = 7 // 显示最近7天的数据
  const dateMap = new Map<string, number>()
  
  // 初始化日期
  for (let i = days - 1; i >= 0; i--) {
    const date = new Date(today)
    date.setDate(date.getDate() - i)
    const dateStr = date.toISOString().split('T')[0]
    dateMap.set(dateStr, 0)
  }
  
  // 统计错误数量
  errors.value.forEach(error => {
    const date = new Date(error.timestamp)
    const dateStr = date.toISOString().split('T')[0]
    
    if (dateMap.has(dateStr)) {
      const count = dateMap.get(dateStr) || 0
      dateMap.set(dateStr, count + 1)
    }
  })
  
  const xAxisData = Array.from(dateMap.keys())
  const seriesData = Array.from(dateMap.values())
  
  const option = {
    title: {
      text: '错误趋势',
      left: 'center'
    },
    tooltip: {
      trigger: 'axis',
      axisPointer: {
        type: 'shadow'
      }
    },
    xAxis: {
      type: 'category',
      data: xAxisData
    },
    yAxis: {
      type: 'value'
    },
    series: [
      {
        name: '错误数量',
        type: 'line',
        data: seriesData,
        markPoint: {
          data: [
            { type: 'max', name: '最大值' },
            { type: 'min', name: '最小值' }
          ]
        }
      }
    ]
  }
  
  errorTrendChart.setOption(option)
}

// 更新图表
const updateCharts = () => {
  if (activeTab.value === 'stats') {
    initErrorTypeChart()
    initErrorTrendChart()
  }
}

// 监听标签页变化
watch(activeTab, (newValue) => {
  if (newValue === 'stats') {
    // 延迟执行，确保DOM已经渲染
    setTimeout(() => {
      initErrorTypeChart()
      initErrorTrendChart()
    }, 100)
  }
})

// 监听错误列表变化
watch(errors, () => {
  if (activeTab.value === 'stats') {
    updateCharts()
  }
})

// 监听窗口大小变化
const handleResize = () => {
  if (errorTypeChart) {
    errorTypeChart.resize()
  }
  if (errorTrendChart) {
    errorTrendChart.resize()
  }
}

// 组件挂载
onMounted(() => {
  fetchErrors()
  fetchLogs()
  
  // 监听窗口大小变化
  window.addEventListener('resize', handleResize)
})

// 组件卸载
onUnmounted(() => {
  // 移除窗口大小变化监听
  window.removeEventListener('resize', handleResize)
  
  // 销毁图表实例
  if (errorTypeChart) {
    errorTypeChart.dispose()
    errorTypeChart = null
  }
  if (errorTrendChart) {
    errorTrendChart.dispose()
    errorTrendChart = null
  }
})
</script>

<style lang="scss" scoped>
.error-monitor-container {
  padding: 20px;
  
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    .header-actions {
      display: flex;
      gap: 10px;
    }
  }
  
  .filter-form {
    margin-bottom: 20px;
  }
  
  .error-details,
  .log-details {
    padding: 10px;
    background-color: var(--el-fill-color-light);
    border-radius: 4px;
    
    pre {
      white-space: pre-wrap;
      word-break: break-word;
      font-family: monospace;
      font-size: 12px;
      margin: 0;
    }
  }
  
  .pagination-container {
    margin-top: 20px;
    display: flex;
    justify-content: flex-end;
  }
  
  .chart-container {
    background-color: var(--el-bg-color);
    padding: 20px;
    border-radius: 4px;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    margin-bottom: 20px;
    
    h3 {
      margin-top: 0;
      margin-bottom: 20px;
      text-align: center;
    }
  }
  
  .mt-20 {
    margin-top: 20px;
  }
  
  .settings-form {
    max-width: 600px;
    margin: 0 auto;
    
    .form-help {
      margin-left: 10px;
      color: var(--el-text-color-secondary);
      font-size: 12px;
    }
  }
}

.error-detail-content,
.log-detail-content {
  max-height: 60vh;
  overflow-y: auto;
  
  pre {
    white-space: pre-wrap;
    word-break: break-word;
    font-family: monospace;
    font-size: 12px;
    background-color: var(--el-fill-color-light);
    padding: 10px;
    border-radius: 4px;
    margin: 0;
  }
}
</style>