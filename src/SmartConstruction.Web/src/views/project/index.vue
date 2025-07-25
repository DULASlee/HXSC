<template>
  <div class="project-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ i18nStore.t("menu.project") }}</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            {{ i18nStore.t("common.add") }}
          </el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="项目名称">
            <el-input
              v-model="searchForm.name"
              :placeholder="'请输入项目名称'"
              clearable
            />
          </el-form-item>
          <el-form-item label="项目状态">
            <el-select
              v-model="searchForm.status"
              placeholder="请选择状态"
              clearable
            >
              <el-option label="进行中" value="1" />
              <el-option label="已完成" value="2" />
              <el-option label="暂停" value="3" />
              <el-option label="计划中" value="4" />
            </el-select>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" @click="handleSearch">
              <el-icon><Search /></el-icon>
              {{ i18nStore.t("common.search") }}
            </el-button>
            <el-button @click="handleReset">
              <el-icon><Refresh /></el-icon>
              {{ i18nStore.t("common.reset") }}
            </el-button>
          </el-form-item>
        </el-form>
      </div>

      <!-- 表格区域 -->
      <el-table :data="tableData" style="width: 100%" v-loading="loading">
        <el-table-column prop="name" label="项目名称" />
        <el-table-column prop="location" label="项目地址" />
        <el-table-column prop="manager" label="项目经理" />
        <el-table-column prop="status" label="项目状态">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="startDate" label="开始时间" />
        <el-table-column prop="endDate" label="结束时间" />
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="handleEdit(row)">
              {{ i18nStore.t("common.edit") }}
            </el-button>
            <el-button size="small" type="danger" @click="handleDelete(row)">
              {{ i18nStore.t("common.delete") }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>

      <!-- 分页 -->
      <div class="pagination-container">
        <el-pagination
          v-model:current-page="pagination.currentPage"
          v-model:page-size="pagination.pageSize"
          :page-sizes="[10, 20, 50, 100]"
          :total="pagination.total"
          layout="total, sizes, prev, pager, next, jumper"
          @size-change="handleSizeChange"
          @current-change="handleCurrentChange"
        />
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from "vue";
import { useI18nStore } from "@/stores/i18n";
import { ElMessage, ElMessageBox } from "element-plus";

const i18nStore = useI18nStore();

// 搜索表单
const searchForm = reactive({
  name: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    name: "智慧工地A项目",
    location: "北京市朝阳区",
    manager: "张三",
    status: 1,
    startDate: "2024-01-01",
    endDate: "2024-06-30",
  },
  {
    id: 2,
    name: "智慧工地B项目",
    location: "上海市浦东新区",
    manager: "李四",
    status: 2,
    startDate: "2023-08-01",
    endDate: "2023-12-31",
  },
  {
    id: 3,
    name: "智慧工地C项目",
    location: "广州市天河区",
    manager: "王五",
    status: 3,
    startDate: "2024-02-01",
    endDate: "2024-08-31",
  },
]);

const loading = ref(false);

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 3,
});

// 获取状态类型
const getStatusType = (status: number) => {
  const typeMap: Record<number, string> = {
    1: "success",
    2: "info",
    3: "warning",
    4: "danger",
  };
  return typeMap[status] || "info";
};

// 获取状态文本
const getStatusText = (status: number) => {
  const textMap: Record<number, string> = {
    1: "进行中",
    2: "已完成",
    3: "暂停",
    4: "计划中",
  };
  return textMap[status] || "未知";
};

// 搜索
const handleSearch = () => {
  loading.value = true;
  setTimeout(() => {
    loading.value = false;
    ElMessage.success("搜索完成");
  }, 1000);
};

// 重置
const handleReset = () => {
  searchForm.name = "";
  searchForm.status = "";
  handleSearch();
};

// 添加
const handleAdd = () => {
  ElMessage.info("添加项目功能开发中...");
};

// 编辑
const handleEdit = (row: any) => {
  ElMessage.info(`编辑项目: ${row.name}`);
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确认删除项目 "${row.name}" 吗？`, "警告", {
    confirmButtonText: i18nStore.t("common.confirm"),
    cancelButtonText: i18nStore.t("common.cancel"),
    type: "warning",
  })
    .then(() => {
      ElMessage.success("删除成功");
    })
    .catch(() => {
      ElMessage.info("已取消删除");
    });
};

// 分页大小改变
const handleSizeChange = (val: number) => {
  pagination.pageSize = val;
  handleSearch();
};

// 当前页改变
const handleCurrentChange = (val: number) => {
  pagination.currentPage = val;
  handleSearch();
};

onMounted(() => {
  handleSearch();
});
</script>

<style scoped>
.project-container {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.search-container {
  margin-bottom: 20px;
  padding: 20px;
  background-color: var(--app-border-color-extra-light);
  border-radius: 8px;
}

.pagination-container {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
}
</style>
