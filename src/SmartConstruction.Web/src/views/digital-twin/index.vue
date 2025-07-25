<template>
  <div class="digital-twin-page">
    <el-card class="page-card">
      <template #header>
        <div class="card-header">
          <h2>数字孪生智慧工地系统</h2>
          <p class="page-description">基于Three.js的3D可视化工地管理平台</p>
        </div>
      </template>
      
      <div class="content-grid">
        <!-- 核心3D可视化模块 -->
        <div class="module-section">
          <h3 class="section-title">🌟 核心3D可视化模块</h3>
          <el-row :gutter="20">
            <el-col :span="6">
              <el-card class="feature-card primary" @click="handleNavigation('/digital-twin/command-center')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><Monitor /></el-icon>
                  <h3>指挥中心</h3>
                  <p>工地全局三维地图<br/>实时数据叠加预警</p>
                  <div class="feature-badge">3D BIM</div>
                </div>
              </el-card>
            </el-col>
            
            <el-col :span="6">
              <el-card class="feature-card success" @click="handleNavigation('/digital-twin/attendance')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><User /></el-icon>
                  <h3>实名制考勤</h3>
                  <p>人员定位轨迹回放<br/>异常考勤3D预警</p>
                  <div class="feature-badge">轨迹</div>
                </div>
              </el-card>
            </el-col>
            
            <el-col :span="6">
              <el-card class="feature-card warning" @click="handleNavigation('/digital-twin/crane-elevator')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><Setting /></el-icon>
                  <h3>塔吊升降机</h3>
                  <p>设备姿态实时同步<br/>防碰撞边界预警</p>
                  <div class="feature-badge">MQTT</div>
                </div>
              </el-card>
            </el-col>
            
            <el-col :span="6">
              <el-card class="feature-card info" @click="handleNavigation('/digital-twin/environment')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><DataAnalysis /></el-icon>
                  <h3>扬尘噪音</h3>
                  <p>环境热力图显示<br/>超标区域自动治理</p>
                  <div class="feature-badge">热力图</div>
                </div>
              </el-card>
            </el-col>
          </el-row>
        </div>
        
        <!-- 管理功能模块 -->
        <div class="module-section">
          <h3 class="section-title">🔧 管理功能模块</h3>
          <el-row :gutter="20">
            <el-col :span="6">
              <el-card class="feature-card" @click="handleNavigation('/digital-twin/dashboard')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><PieChart /></el-icon>
                  <h3>数据大屏</h3>
                  <p>综合数据可视化展示</p>
                </div>
              </el-card>
            </el-col>
            
            <el-col :span="6">
              <el-card class="feature-card" @click="handleNavigation('/digital-twin/device')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><Monitor /></el-icon>
                  <h3>设备管理</h3>
                  <p>物联网设备监控管理</p>
                </div>
              </el-card>
            </el-col>
            
            <el-col :span="6">
              <el-card class="feature-card" @click="handleNavigation('/digital-twin/model')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><Box /></el-icon>
                  <h3>模型管理</h3>
                  <p>3D模型资源管理</p>
                </div>
              </el-card>
            </el-col>
            
            <el-col :span="6">
              <el-card class="feature-card" @click="handleNavigation('/digital-twin/analysis')">
                <div class="feature-content">
                  <el-icon class="feature-icon"><TrendCharts /></el-icon>
                  <h3>数据分析</h3>
                  <p>孪生数据统计分析</p>
                </div>
              </el-card>
            </el-col>
          </el-row>
        </div>
        
        <!-- 系统状态 -->
        <div class="status-section">
          <el-row :gutter="20">
            <el-col :span="8">
              <div class="status-card">
                <div class="status-item">
                  <span class="status-label">在线设备</span>
                  <span class="status-value">{{ systemStatus.onlineDevices }}</span>
                </div>
                <div class="status-item">
                  <span class="status-label">在场人员</span>
                  <span class="status-value">{{ systemStatus.onlinePersonnel }}</span>
                </div>
              </div>
            </el-col>
            <el-col :span="8">
              <div class="status-card">
                <div class="status-item">
                  <span class="status-label">预警数量</span>
                  <span class="status-value warning">{{ systemStatus.alerts }}</span>
                </div>
                <div class="status-item">
                  <span class="status-label">系统状态</span>
                  <span class="status-value success">正常运行</span>
                </div>
              </div>
            </el-col>
            <el-col :span="8">
              <div class="status-card">
                <div class="status-item">
                  <span class="status-label">今日考勤</span>
                  <span class="status-value">{{ systemStatus.todayAttendance }}%</span>
                </div>
                <div class="status-item">
                  <span class="status-label">环境指数</span>
                  <span class="status-value">{{ systemStatus.environmentIndex }}</span>
                </div>
              </div>
            </el-col>
          </el-row>
        </div>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { Monitor, Setting, User, DataAnalysis, PieChart, Box, TrendCharts } from '@element-plus/icons-vue'

const router = useRouter()

// 系统状态数据
const systemStatus = ref({
  onlineDevices: 0,
  onlinePersonnel: 0,
  alerts: 0,
  todayAttendance: 0,
  environmentIndex: 0
})

/// <summary>
/// 导航到指定页面
/// </summary>
const handleNavigation = (path: string) => {
  router.push(path)
}

/// <summary>
/// 加载系统状态数据
/// </summary>
const loadSystemStatus = async () => {
  // 模拟数据，后续对接真实API
  systemStatus.value = {
    onlineDevices: 48,
    onlinePersonnel: 156,
    alerts: 3,
    todayAttendance: 92,
    environmentIndex: 87
  }
}

onMounted(() => {
  loadSystemStatus()
})
</script>

<style lang="scss" scoped>
.digital-twin-page {
  padding: 20px;
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  
  .page-card {
    border-radius: 16px;
    border: none;
    box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
    backdrop-filter: blur(10px);
    
    .card-header {
      text-align: center;
      padding: 20px 0;
      
      h2 {
        margin: 0 0 10px 0;
        color: var(--el-text-color-primary);
        font-size: 28px;
        font-weight: 600;
      }
      
      .page-description {
        margin: 0;
        color: var(--el-text-color-secondary);
        font-size: 16px;
      }
    }
  }
  
  .content-grid {
    .module-section {
      margin-bottom: 40px;
      
      .section-title {
        margin: 0 0 20px 0;
        color: var(--el-text-color-primary);
        font-size: 20px;
        font-weight: 600;
        display: flex;
        align-items: center;
        
        &::after {
          content: '';
          flex: 1;
          height: 2px;
          background: linear-gradient(90deg, var(--el-color-primary) 0%, transparent 100%);
          margin-left: 16px;
        }
      }
    }
    
    .feature-card {
      cursor: pointer;
      transition: all 0.3s ease;
      border-radius: 12px;
      border: 2px solid transparent;
      overflow: hidden;
      position: relative;
      
      &:hover {
        transform: translateY(-8px);
        box-shadow: 0 12px 40px rgba(0, 0, 0, 0.15);
      }
      
      &.primary {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        
        .feature-content .feature-icon {
          color: white;
        }
      }
      
      &.success {
        background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
        color: white;
        
        .feature-content .feature-icon {
          color: white;
        }
      }
      
      &.warning {
        background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
        color: white;
        
        .feature-content .feature-icon {
          color: white;
        }
      }
      
      &.info {
        background: linear-gradient(135deg, #4facfe 0%, #00f2fe 100%);
        color: white;
        
        .feature-content .feature-icon {
          color: white;
        }
      }
      
      .feature-content {
        text-align: center;
        padding: 30px 20px;
        position: relative;
        
        .feature-icon {
          font-size: 48px;
          margin-bottom: 16px;
          color: var(--el-color-primary);
          display: block;
        }
        
        h3 {
          margin: 0 0 12px 0;
          font-size: 18px;
          font-weight: 600;
        }
        
        p {
          margin: 0;
          font-size: 14px;
          opacity: 0.9;
          line-height: 1.5;
        }
        
        .feature-badge {
          position: absolute;
          top: 10px;
          right: 10px;
          background: rgba(255, 255, 255, 0.2);
          color: white;
          padding: 4px 8px;
          border-radius: 12px;
          font-size: 12px;
          font-weight: 500;
          backdrop-filter: blur(10px);
        }
      }
    }
    
    .status-section {
      margin-top: 30px;
      
      .status-card {
        background: white;
        border-radius: 12px;
        padding: 20px;
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
        border: 1px solid var(--el-border-color-lighter);
        
        .status-item {
          display: flex;
          justify-content: space-between;
          align-items: center;
          margin-bottom: 12px;
          
          &:last-child {
            margin-bottom: 0;
          }
          
          .status-label {
            color: var(--el-text-color-secondary);
            font-size: 14px;
          }
          
          .status-value {
            font-weight: 600;
            font-size: 16px;
            color: var(--el-text-color-primary);
            
            &.warning {
              color: var(--el-color-warning);
            }
            
            &.success {
              color: var(--el-color-success);
            }
          }
        }
      }
    }
  }
}

// 深色主题适配
[data-theme="dark"] {
  .digital-twin-page {
    background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
    
    .page-card {
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.1);
    }
    
    .status-card {
      background: rgba(255, 255, 255, 0.05) !important;
      border: 1px solid rgba(255, 255, 255, 0.1) !important;
    }
  }
}
</style>