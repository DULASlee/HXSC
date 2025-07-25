<template>
  <div class="menu-management">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>菜单管理</span>
          <el-button type="primary" @click="handleAdd">新增菜单</el-button>
        </div>
      </template>
      
      <el-table :data="menuList" style="width: 100%">
        <el-table-column prop="name" label="菜单名称" />
        <el-table-column prop="path" label="路径" />
        <el-table-column prop="icon" label="图标" />
        <el-table-column prop="sort" label="排序" width="80" />
        <el-table-column label="操作" width="200">
          <template #default="scope">
            <el-button size="small" @click="handleEdit(scope.row)">编辑</el-button>
            <el-button size="small" type="danger" @click="handleDelete(scope.row)">删除</el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage } from 'element-plus'

interface MenuItem {
  id: number
  name: string
  path: string
  icon: string
  sort: number
}

const menuList = ref<MenuItem[]>([])

const handleAdd = () => {
  ElMessage.info('新增菜单功能待实现')
}

const handleEdit = (row: MenuItem) => {
  ElMessage.info(`编辑菜单: ${row.name}`)
}

const handleDelete = (row: MenuItem) => {
  ElMessage.info(`删除菜单: ${row.name}`)
}

onMounted(() => {
  // 模拟数据
  menuList.value = [
    { id: 1, name: '仪表板', path: '/dashboard', icon: 'el-icon-data-board', sort: 1 },
    { id: 2, name: '用户管理', path: '/user', icon: 'el-icon-user', sort: 2 },
    { id: 3, name: '设备管理', path: '/device', icon: 'el-icon-cpu', sort: 3 }
  ]
})
</script>

<style scoped>
.menu-management {
  padding: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>