<template>
  <el-card class="detail-card" :body-style="{ padding: '0' }">
    <template #header>
      <div class="card-header">
        <span>{{ title }}</span>
        <div class="header-actions">
          <slot name="header-actions"></slot>
        </div>
      </div>
    </template>
    
    <div class="card-content" :class="{ 'is-loading': loading }">
      <el-skeleton :loading="loading" animated :rows="6" v-if="loading">
        <template #template>
          <div style="padding: 20px">
            <el-skeleton-item variant="text" style="width: 30%" />
            <div style="display: flex; align-items: center; justify-content: space-between; margin-top: 20px">
              <el-skeleton-item variant="text" style="margin-right: 16px; width: 40%" />
              <el-skeleton-item variant="text" style="width: 30%" />
            </div>
            <el-skeleton-item variant="text" style="margin-top: 20px; width: 50%" />
            <el-skeleton-item variant="text" style="margin-top: 20px; width: 60%" />
            <el-skeleton-item variant="text" style="margin-top: 20px; width: 45%" />
            <el-skeleton-item variant="text" style="margin-top: 20px; width: 55%" />
          </div>
        </template>
      </el-skeleton>
      
      <div v-else>
        <slot></slot>
      </div>
    </div>
    
    <div class="card-footer" v-if="$slots.footer">
      <slot name="footer"></slot>
    </div>
  </el-card>
</template>

<script setup lang="ts">
defineProps({
  title: {
    type: String,
    required: true
  },
  loading: {
    type: Boolean,
    default: false
  }
})
</script>

<style lang="scss" scoped>
.detail-card {
  margin-bottom: 20px;
  
  .card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    .header-actions {
      display: flex;
      gap: 10px;
    }
  }
  
  .card-content {
    padding: 20px;
    
    &.is-loading {
      min-height: 200px;
    }
  }
  
  .card-footer {
    padding: 15px 20px;
    border-top: 1px solid var(--el-border-color-lighter);
    display: flex;
    justify-content: flex-end;
    gap: 10px;
  }
}
</style>