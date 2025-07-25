<template>
  <div class="error-page">
    <div class="error-content">
      <div class="error-image">
        <img src="/images/404.svg" alt="404" />
      </div>
      <div class="error-info">
        <h1 class="error-code">404</h1>
        <h2 class="error-title">页面不存在</h2>
        <p class="error-description">
          抱歉，您访问的页面不存在或已被删除
        </p>
        <div class="error-actions">
          <el-button type="primary" @click="goHome">
            <el-icon><House /></el-icon>
            返回首页
          </el-button>
          <el-button @click="goBack">
            <el-icon><Back /></el-icon>
            返回上页
          </el-button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { House, Back } from '@element-plus/icons-vue'

const router = useRouter()

const goHome = () => {
  router.push('/dashboard')
}

const goBack = () => {
  router.go(-1)
}
</script>

<style lang="scss" scoped>
.error-page {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100vh;
  background: var(--el-bg-color-page);
  
  .error-content {
    display: flex;
    align-items: center;
    max-width: 800px;
    padding: 40px;
    
    .error-image {
      flex: 1;
      text-align: center;
      margin-right: 40px;
      
      img {
        max-width: 300px;
        width: 100%;
        height: auto;
      }
    }
    
    .error-info {
      flex: 1;
      
      .error-code {
        font-size: 72px;
        font-weight: bold;
        color: var(--el-color-primary);
        margin: 0 0 20px;
        line-height: 1;
      }
      
      .error-title {
        font-size: 32px;
        color: var(--el-text-color-primary);
        margin: 0 0 16px;
      }
      
      .error-description {
        font-size: 16px;
        color: var(--el-text-color-regular);
        margin: 0 0 32px;
        line-height: 1.6;
      }
      
      .error-actions {
        display: flex;
        gap: 16px;
      }
    }
  }
}

@media (max-width: 768px) {
  .error-page {
    .error-content {
      flex-direction: column;
      text-align: center;
      padding: 20px;
      
      .error-image {
        margin-right: 0;
        margin-bottom: 40px;
      }
      
      .error-info {
        .error-actions {
          justify-content: center;
        }
      }
    }
  }
}
</style>