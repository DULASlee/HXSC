<template>
  <div class="placeholder-view">
    <el-empty description="组件加载失败">
      <el-button type="primary" @click="$router.go(-1)">返回上一页</el-button>
    </el-empty>
  </div>
</template>

<script setup lang="ts">
// 占位符组件，用于组件加载失败时显示
</script>

<style lang="scss" scoped>
.placeholder-view {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 400px;
  padding: 20px;
}
</style>