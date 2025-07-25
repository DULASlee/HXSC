<template>
  <div class="online-users">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>在线用户监控</span>
          <el-button type="primary" @click="refreshData">刷新</el-button>
        </div>
      </template>

      <!-- 统计信息 -->
      <el-row :gutter="20" class="stats-row">
        <el-col :span="6">
          <el-statistic title="在线用户总数" :value="stats.totalOnline" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="今日登录" :value="stats.todayLogin" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="活跃用户" :value="stats.activeUsers" />
        </el-col>
        <el-col :span="6">
          <el-statistic title="平均在线时长" :value="stats.avgOnlineTime" suffix="分钟" />
        </el-col>
      </el-row>

      <!-- 在线用户列表 -->
      <el-table :data="onlineUsers" style="width: 100%" v-loading="loading">
        <el-table-column prop="username" label="用户名" />
        <el-table-column prop="realName" label="真实姓名" />
        <el-table-column prop="loginTime" label="登录时间" />
        <el-table-column prop="lastActivity" label="最后活动" />
        <el-table-column prop="ipAddress" label="IP地址" />
        <el-table-column prop="browser" label="浏览器" />
        <el-table-column label="状态">
          <template #default="{ row }">
            <el-tag :type="row.status === 'active' ? 'success' : 'warning'">
              {{ row.status === 'active' ? '活跃' : '空闲' }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作">
          <template #default="{ row }">
            <el-button type="danger" size="small" @click="forceLogout(row)">
              强制下线
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'

// 响应式数据
const loading = ref(false)
const stats = ref({
  totalOnline: 156,
  todayLogin: 342,
  activeUsers: 128,
  avgOnlineTime: 45
})

const onlineUsers = ref([
  {
    id: 1,
    username: 'admin',
    realName: '系统管理员',
    loginTime: '2024-01-15 09:30:00',
    lastActivity: '2024-01-15 14:25:00',
    ipAddress: '192.168.1.100',
    browser: 'Chrome 120.0',
    status: 'active'
  },
  {
    id: 2,
    username: 'user001',
    realName: '张三',
    loginTime: '2024-01-15 08:45:00',
    lastActivity: '2024-01-15 14:20:00',
    ipAddress: '192.168.1.101',
    browser: 'Firefox 121.0',
    status: 'idle'
  },
  {
    id: 3,
    username: 'user002',
    realName: '李四',
    loginTime: '2024-01-15 10:15:00',
    lastActivity: '2024-01-15 14:24:00',
    ipAddress: '192.168.1.102',
    browser: 'Edge 120.0',
    status: 'active'
  }
])

// 刷新数据
const refreshData = () => {
  loading.value = true
  // 模拟API调用
  setTimeout(() => {
    stats.value.totalOnline = Math.floor(Math.random() * 200) + 100
    stats.value.activeUsers = Math.floor(Math.random() * 150) + 80
    loading.value = false
    ElMessage.success('数据已刷新')
  }, 1000)
}

// 强制下线
const forceLogout = async (user: any) => {
  try {
    await ElMessageBox.confirm(
      `确定要强制用户 ${user.realName} 下线吗？`,
      '确认操作',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }
    )
    
    // 模拟API调用
    ElMessage.success(`用户 ${user.realName} 已被强制下线`)
    
    // 从列表中移除
    const index = onlineUsers.value.findIndex(u => u.id === user.id)
    if (index > -1) {
      onlineUsers.value.splice(index, 1)
      stats.value.totalOnline--
    }
  } catch {
    // 用户取消操作
  }
}

onMounted(() => {
  refreshData()
})
</script>

<style lang="scss" scoped>
.online-users {
  padding: var(--spacing-lg);
  background-color: var(--bg-body);
  min-height: calc(100vh - 50px);
  
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .stats-row {
    margin-bottom: var(--spacing-lg);
    
    .el-statistic {
      text-align: center;
      padding: var(--spacing-md);
      background: var(--bg-elevated);
      border-radius: var(--radius-md);
      transition: all 0.3s ease;
      
      &:hover {
        box-shadow: var(--shadow-sm);
      }
    }
  }
  
  // 响应式适配
  @media (max-width: 768px) {
    padding: var(--spacing-md);
    
    .stats-row {
      .el-col {
        margin-bottom: var(--spacing-sm);
      }
    }
  }
}
</style>