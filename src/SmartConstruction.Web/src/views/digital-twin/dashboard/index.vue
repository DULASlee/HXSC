<template>
  <div class="digital-twin-dashboard">
    <div class="dashboard-header">
      <div class="header-left">
        <h1>数字孪生数据大屏</h1>
        <p>智慧工地三维可视化监控中心</p>
      </div>
      <div class="header-right">
        <el-button @click="toggleFullscreen" type="primary" :icon="FullScreen">
          {{ isFullscreen ? '退出全屏' : '全屏显示' }}
        </el-button>
        <el-button @click="refreshData" :icon="Refresh">刷新数据</el-button>
      </div>
    </div>

    <div class="dashboard-content">
      <!-- 顶部统计卡片 -->
    </div>
  </div>
</template>