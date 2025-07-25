<template>
  <el-card class="stat-card" :body-style="{ padding: '0' }" :shadow="shadow">
    <div class="stat-content" :class="{ 'is-loading': loading }">
      <div class="stat-icon" :style="{ backgroundColor: iconBgColor }">
        <el-icon :size="iconSize" :color="iconColor">
          <component :is="icon" />
        </el-icon>
      </div>
      
      <div class="stat-info">
        <div class="stat-title">{{ title }}</div>
        <div class="stat-value">{{ value }}</div>
        <div class="stat-footer" v-if="footer">{{ footer }}</div>
      </div>
    </div>
  </el-card>
</template>

<script setup lang="ts">
defineProps({
  title: {
    type: String,
    required: true
  },
  value: {
    type: [String, Number],
    required: true
  },
  footer: {
    type: String,
    default: ''
  },
  icon: {
    type: String,
    required: true
  },
  iconColor: {
    type: String,
    default: '#ffffff'
  },
  iconBgColor: {
    type: String,
    default: '#409EFF'
  },
  iconSize: {
    type: Number,
    default: 24
  },
  loading: {
    type: Boolean,
    default: false
  },
  shadow: {
    type: String,
    default: 'hover'
  }
})
</script>

<style lang="scss" scoped>
.stat-card {
  .stat-content {
    display: flex;
    align-items: center;
    padding: 20px;
    
    &.is-loading {
      opacity: 0.6;
    }
    
    .stat-icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 60px;
      height: 60px;
      border-radius: 10px;
      margin-right: 15px;
    }
    
    .stat-info {
      flex: 1;
      
      .stat-title {
        font-size: 14px;
        color: var(--el-text-color-secondary);
        margin-bottom: 5px;
      }
      
      .stat-value {
        font-size: 24px;
        font-weight: bold;
        color: var(--el-text-color-primary);
      }
      
      .stat-footer {
        font-size: 12px;
        color: var(--el-text-color-secondary);
        margin-top: 5px;
      }
    }
  }
}
</style>