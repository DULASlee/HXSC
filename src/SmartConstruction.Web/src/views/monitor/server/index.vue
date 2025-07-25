<template>
  <div class="server-monitor">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>服务监控</span>
          <el-button type="primary" @click="refreshData">刷新</el-button>
        </div>
      </template>

      <el-row :gutter="20">
        <!-- 系统信息 -->
        <el-col :span="8">
          <el-card class="info-card">
            <template #header>
              <span>系统信息</span>
            </template>
            <div class="info-list">
              <div class="info-item">
                <span class="label">操作系统：</span>
                <span class="value">{{ serverInfo.os }}</span>
              </div>
              <div class="info-item">
                <span class="label">CPU使用率：</span>
                <span class="value">{{ serverInfo.cpuUsage }}%</span>
              </div>
              <div class="info-item">
                <span class="label">内存使用率：</span>
                <span class="value">{{ serverInfo.memoryUsage }}%</span>
              </div>
              <div class="info-item">
                <span class="label">磁盘使用率：</span>
                <span class="value">{{ serverInfo.diskUsage }}%</span>
              </div>
            </div>
          </el-card>
        </el-col>

        <!-- 应用信息 -->
        <el-col :span="8">
          <el-card class="info-card">
            <template #header>
              <span>应用信息</span>
            </template>
            <div class="info-list">
              <div class="info-item">
                <span class="label">运行时间：</span>
                <span class="value">{{ serverInfo.uptime }}</span>
              </div>
              <div class="info-item">
                <span class="label">版本：</span>
                <span class="value">{{ serverInfo.version }}</span>
              </div>
              <div class="info-item">
                <span class="label">端口：</span>
                <span class="value">{{ serverInfo.port }}</span>
              </div>
              <div class="info-item">
                <span class="label">状态：</span>
                <el-tag :type="serverInfo.status === 'running' ? 'success' : 'danger'">
                  {{ serverInfo.status === 'running' ? '运行中' : '已停止' }}
                </el-tag>
              </div>
            </div>
          </el-card>
        </el-col>

        <!-- 性能指标 -->
        <el-col :span="8">
          <el-card class="info-card">
            <template #header>
              <span>性能指标</span>
            </template>
            <div class="info-list">
              <div class="info-item">
                <span class="label">请求数/分钟：</span>
                <span class="value">{{ serverInfo.requestsPerMinute }}</span>
              </div>
              <div class="info-item">
                <span class="label">平均响应时间：</span>
                <span class="value">{{ serverInfo.avgResponseTime }}ms</span>
              </div>
              <div class="info-item">
                <span class="label">错误率：</span>
                <span class="value">{{ serverInfo.errorRate }}%</span>
              </div>
              <div class="info-item">
                <span class="label">活跃连接：</span>
                <span class="value">{{ serverInfo.activeConnections }}</span>
              </div>
            </div>
          </el-card>
        </el-col>
      </el-row>

      <!-- 实时监控图表 -->
      <el-card style="margin-top: 20px;">
        <template #header>
          <span>实时监控</span>
        </template>
        <div class="chart-container">
          <div class="chart-placeholder">
            <el-empty description="图表功能开发中" />
          </div>
        </div>
      </el-card>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'

// 响应式数据
const serverInfo = ref({
  os: 'Windows Server 2019',
  cpuUsage: 45.2,
  memoryUsage: 68.5,
  diskUsage: 72.1,
  uptime: '15天 8小时 32分钟',
  version: 'v1.0.0',
  port: 5000,
  status: 'running',
  requestsPerMinute: 1250,
  avgResponseTime: 45,
  errorRate: 0.2,
  activeConnections: 156
})

// 刷新数据
const refreshData = () => {
  // 模拟数据更新
  serverInfo.value.cpuUsage = Math.random() * 100
  serverInfo.value.memoryUsage = Math.random() * 100
  serverInfo.value.requestsPerMinute = Math.floor(Math.random() * 2000) + 500
  serverInfo.value.avgResponseTime = Math.floor(Math.random() * 100) + 20
}
</script>

<style lang="scss" scoped>
.server-monitor {
  padding: var(--spacing-lg);
  background-color: var(--bg-body);
  min-height: calc(100vh - 50px);
  
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .info-card {
    height: 100%;
    background-color: var(--bg-container);
    transition: all 0.3s ease;
    
    &:hover {
      box-shadow: var(--shadow-md);
    }
    
    .info-list {
      .info-item {
        display: flex;
        justify-content: space-between;
        margin-bottom: var(--spacing-sm);
        padding: var(--spacing-xs) 0;
        border-bottom: 1px solid var(--border-color-light);
        
        &:last-child {
          border-bottom: none;
        }
        
        .label {
          color: var(--text-secondary);
          font-weight: 500;
        }
        
        .value {
          color: var(--text-primary);
          font-weight: 600;
        }
      }
    }
  }
  
  .chart-container {
    .chart-placeholder {
      height: 300px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }
  
  // 响应式适配
  @media (max-width: 768px) {
    padding: var(--spacing-md);
    
    .el-row {
      .el-col {
        margin-bottom: var(--spacing-md);
      }
    }
  }
}
</style>