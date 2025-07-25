<template>
  <div class="attendance-container">
    <!-- 统计卡片 -->
    <el-row :gutter="20" class="stats-row">
      <el-col :span="6" v-for="item in statsData" :key="item.title">
        <el-card class="stats-card">
          <div class="stats-content">
            <div class="stats-icon" :style="{ backgroundColor: item.color }">
              <el-icon :size="20"><component :is="item.icon" /></el-icon>
            </div>
            <div class="stats-info">
              <div class="stats-value">{{ item.value }}</div>
              <div class="stats-title">{{ item.title }}</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ i18nStore.t("menu.attendance") }}</span>
          <div class="header-actions">
            <el-button type="success" @click="handleExport">
              <el-icon><Download /></el-icon>
              导出考勤
            </el-button>
            <el-button type="primary" @click="handleAdd">
              <el-icon><Plus /></el-icon>
              {{ i18nStore.t("common.add") }}
            </el-button>
          </div>
        </div>
      </template>

      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="工人姓名">
            <el-input
              v-model="searchForm.workerName"
              placeholder="请输入工人姓名"
              clearable
            />
          </el-form-item>
          <el-form-item label="考勤日期">
            <el-date-picker
              v-model="searchForm.dateRange"
              type="daterange"
              range-separator="至"
              start-placeholder="开始日期"
              end-placeholder="结束日期"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
            />
          </el-form-item>
          <el-form-item label="考勤状态">
            <el-select
              v-model="searchForm.status"
              placeholder="请选择状态"
              clearable
            >
              <el-option label="正常" value="1" />
              <el-option label="迟到" value="2" />
              <el-option label="早退" value="3" />
              <el-option label="缺勤" value="4" />
              <el-option label="请假" value="5" />
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
        <el-table-column prop="workerId" label="工号" width="80" />
        <el-table-column prop="workerName" label="工人姓名" />
        <el-table-column prop="project" label="所属项目" />
        <el-table-column prop="date" label="考勤日期" />
        <el-table-column prop="checkInTime" label="签到时间" />
        <el-table-column prop="checkOutTime" label="签退时间" />
        <el-table-column prop="workHours" label="工作时长">
          <template #default="{ row }"> {{ row.workHours }}小时 </template>
        </el-table-column>
        <el-table-column prop="status" label="考勤状态">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="remark" label="备注" />
        <el-table-column label="操作" width="150">
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
import { ref, reactive, onMounted, computed } from "vue";
import { useI18nStore } from "@/stores/i18n";
import { ElMessage, ElMessageBox } from "element-plus";

const i18nStore = useI18nStore();

// 统计数据
const statsData = computed(() => [
  {
    title: "今日出勤",
    value: "156人",
    icon: "UserFilled",
    color: "#67C23A",
  },
  {
    title: "迟到人数",
    value: "8人",
    icon: "Clock",
    color: "#E6A23C",
  },
  {
    title: "请假人数",
    value: "12人",
    icon: "DocumentRemove",
    color: "#409EFF",
  },
  {
    title: "出勤率",
    value: "92.5%",
    icon: "TrendCharts",
    color: "#F56C6C",
  },
]);

// 搜索表单
const searchForm = reactive({
  workerName: "",
  dateRange: [],
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    workerId: "W001",
    workerName: "张三",
    project: "智慧工地A项目",
    date: "2024-01-16",
    checkInTime: "08:00:00",
    checkOutTime: "18:00:00",
    workHours: 9,
    status: 1,
    remark: "正常出勤",
  },
  {
    id: 2,
    workerId: "W002",
    workerName: "王五",
    project: "智慧工地B项目",
    date: "2024-01-16",
    checkInTime: "08:15:00",
    checkOutTime: "18:00:00",
    workHours: 8.75,
    status: 2,
    remark: "迟到15分钟",
  },
  {
    id: 3,
    workerId: "W003",
    workerName: "刘七",
    project: "智慧工地A项目",
    date: "2024-01-16",
    checkInTime: "08:00:00",
    checkOutTime: "17:30:00",
    workHours: 8.5,
    status: 3,
    remark: "早退30分钟",
  },
  {
    id: 4,
    workerId: "W004",
    workerName: "李四",
    project: "智慧工地C项目",
    date: "2024-01-16",
    checkInTime: "-",
    checkOutTime: "-",
    workHours: 0,
    status: 5,
    remark: "病假",
  },
  {
    id: 5,
    workerId: "W005",
    workerName: "赵六",
    project: "智慧工地A项目",
    date: "2024-01-16",
    checkInTime: "-",
    checkOutTime: "-",
    workHours: 0,
    status: 4,
    remark: "无故缺勤",
  },
]);

const loading = ref(false);

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 5,
});

// 获取状态类型
const getStatusType = (status: number) => {
  const typeMap: Record<number, string> = {
    1: "success",
    2: "warning",
    3: "warning",
    4: "danger",
    5: "info",
  };
  return typeMap[status] || "info";
};

// 获取状态文本
const getStatusText = (status: number) => {
  const textMap: Record<number, string> = {
    1: "正常",
    2: "迟到",
    3: "早退",
    4: "缺勤",
    5: "请假",
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
  searchForm.workerName = "";
  searchForm.dateRange = [];
  searchForm.status = "";
  handleSearch();
};

// 添加
const handleAdd = () => {
  ElMessage.info("添加考勤记录功能开发中...");
};

// 导出
const handleExport = () => {
  ElMessage.info("导出考勤数据功能开发中...");
};

// 编辑
const handleEdit = (row: any) => {
  ElMessage.info(`编辑考勤记录: ${row.workerName} - ${row.date}`);
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(
    `确认删除 "${row.workerName}" 在 ${row.date} 的考勤记录吗？`,
    "警告",
    {
      confirmButtonText: i18nStore.t("common.confirm"),
      cancelButtonText: i18nStore.t("common.cancel"),
      type: "warning",
    },
  )
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
.attendance-container {
  padding: 0;
}

.stats-row {
  margin-bottom: 20px;
}

.stats-card {
  height: 100px;
}

.stats-content {
  display: flex;
  align-items: center;
  height: 100%;
}

.stats-icon {
  width: 50px;
  height: 50px;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  margin-right: 16px;
}

.stats-info {
  flex: 1;
}

.stats-value {
  font-size: 20px;
  font-weight: bold;
  color: var(--app-text-color);
  margin-bottom: 4px;
}

.stats-title {
  font-size: 14px;
  color: var(--app-text-color-regular);
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.header-actions {
  display: flex;
  gap: 12px;
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
