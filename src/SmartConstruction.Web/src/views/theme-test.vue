<template>
  <div class="theme-test-container">
    <div class="test-header">
      <h1>🎨 主题换肤系统测试</h1>
      <p>测试5种主题风格的切换效果</p>
    </div>
    
    <div class="theme-showcase">
      <el-row :gutter="20">
        <el-col :xs="24" :sm="12" :md="8" :lg="6" v-for="theme in availableThemes" :key="theme.name">
          <el-card 
            class="theme-card"
            :class="{ 'active-theme': currentTheme === theme.name }"
            @click="setTheme(theme.name as any)"
          >
            <div class="theme-preview" :style="{ background: theme.preview }">
              <div class="theme-icon">{{ theme.icon }}</div>
            </div>
            <div class="theme-info">
              <h3>{{ theme.label }}</h3>
              <p>{{ theme.description }}</p>
              <el-tag v-if="currentTheme === theme.name" type="primary" size="small">
                当前主题
              </el-tag>
            </div>
          </el-card>
        </el-col>
      </el-row>
    </div>
    
    <div class="test-components">
      <h2>组件样式测试</h2>
      
      <el-row :gutter="20">
        <el-col :span="12">
          <el-card title="按钮测试">
            <div class="button-group">
              <el-button type="primary">主要按钮</el-button>
              <el-button type="success">成功按钮</el-button>
              <el-button type="warning">警告按钮</el-button>
              <el-button type="danger">危险按钮</el-button>
            </div>
          </el-card>
        </el-col>
        
        <el-col :span="12">
          <el-card title="表单测试">
            <el-form>
              <el-form-item label="输入框">
                <el-input placeholder="请输入内容" />
              </el-form-item>
              <el-form-item label="选择器">
                <el-select placeholder="请选择">
                  <el-option label="选项1" value="1" />
                  <el-option label="选项2" value="2" />
                </el-select>
              </el-form-item>
            </el-form>
          </el-card>
        </el-col>
      </el-row>
      
      <el-row :gutter="20" style="margin-top: 20px;">
        <el-col :span="24">
          <el-card title="表格测试">
            <el-table :data="tableData" style="width: 100%">
              <el-table-column prop="name" label="姓名" />
              <el-table-column prop="age" label="年龄" />
              <el-table-column prop="address" label="地址" />
            </el-table>
          </el-card>
        </el-col>
      </el-row>
    </div>
    
    <div class="theme-actions">
      <el-button @click="nextTheme" type="primary" size="large">
        切换到下一个主题
      </el-button>
      <el-button @click="resetTheme" size="large">
        重置为默认主题
      </el-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useTheme } from '@/composables/useTheme'
import { ElMessage } from 'element-plus'

// 使用主题组合式函数
const { 
  currentTheme, 
  availableThemes, 
  setTheme, 
  nextTheme, 
  resetTheme 
} = useTheme()

// 测试数据
const tableData = ref([
  { name: '张三', age: 25, address: '北京市朝阳区' },
  { name: '李四', age: 30, address: '上海市浦东新区' },
  { name: '王五', age: 28, address: '广州市天河区' }
])

// 主题切换处理
const handleThemeChange = async (themeName: string) => {
  try {
    await setTheme(themeName as any)
    ElMessage.success(`已切换到${themeName}主题`)
  } catch (error) {
    ElMessage.error('主题切换失败')
  }
}
</script>

<style lang="scss" scoped>
.theme-test-container {
  padding: var(--spacing-lg);
  max-width: 1200px;
  margin: 0 auto;
  
  .test-header {
    text-align: center;
    margin-bottom: var(--spacing-xl);
    
    h1 {
      color: var(--primary-color);
      margin-bottom: var(--spacing-sm);
    }
    
    p {
      color: var(--text-secondary);
      font-size: 16px;
    }
  }
  
  .theme-showcase {
    margin-bottom: var(--spacing-xl);
    
    .theme-card {
      cursor: pointer;
      transition: all 0.3s ease;
      border: 2px solid transparent;
      
      &:hover {
        transform: translateY(-4px);
        box-shadow: var(--shadow-lg);
      }
      
      &.active-theme {
        border-color: var(--primary-color);
        box-shadow: var(--shadow-md);
      }
      
      .theme-preview {
        height: 80px;
        border-radius: var(--radius-md);
        display: flex;
        align-items: center;
        justify-content: center;
        margin-bottom: var(--spacing-md);
        
        .theme-icon {
          font-size: 32px;
        }
      }
      
      .theme-info {
        text-align: center;
        
        h3 {
          margin: 0 0 var(--spacing-xs) 0;
          color: var(--text-primary);
        }
        
        p {
          margin: 0 0 var(--spacing-sm) 0;
          color: var(--text-secondary);
          font-size: 12px;
          line-height: 1.4;
        }
      }
    }
  }
  
  .test-components {
    margin-bottom: var(--spacing-xl);
    
    h2 {
      color: var(--text-primary);
      margin-bottom: var(--spacing-lg);
    }
    
    .button-group {
      display: flex;
      gap: var(--spacing-sm);
      flex-wrap: wrap;
    }
  }
  
  .theme-actions {
    text-align: center;
    padding: var(--spacing-lg);
    background: var(--bg-container);
    border-radius: var(--radius-lg);
    
    .el-button {
      margin: 0 var(--spacing-sm);
    }
  }
}

// 响应式适配
@media (max-width: 768px) {
  .theme-test-container {
    padding: var(--spacing-md);
    
    .theme-showcase {
      .theme-card {
        margin-bottom: var(--spacing-md);
      }
    }
    
    .theme-actions {
      .el-button {
        display: block;
        width: 100%;
        margin: var(--spacing-xs) 0;
      }
    }
  }
}
</style>