<template>
  <div class="monitoring-container">
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

    <!-- 监控设备管理 -->
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ i18nStore.t("menu.monitoring") }}</span>
          <div class="header-actions">
            <el-button type="warning" @click="handleRefreshAll">
              <el-icon><Refresh /></el-icon>
              刷新所有设备
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
          <el-form-item label="设备名称">
            <el-input
              v-model="searchForm.deviceName"
              placeholder="请输入设备名称"
              clearable
            />
          </el-form-item>
          <el-form-item label="设备类型">
            <el-select
              v-model="searchForm.deviceType"
              placeholder="请选择设备类型"
              clearable
            >
              <el-option label="摄像头" value="1" />
              <el-option label="传感器" value="2" />
              <el-option label="门禁" value="3" />
              <el-option label="环境监测" value="4" />
            </el-select>
          </el-form-item>
          <el-form-item label="设备状态">
            <el-select
              v-model="searchForm.status"
              placeholder="请选择状态"
              clearable
            >
              <el-option label="在线" value="1" />
              <el-option label="离线" value="2" />
              <el-option label="故障" value="3" />
              <el-option label="维护中" value="4" />
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
        <el-table-column prop="deviceId" label="设备编号" width="120" />
        <el-table-column prop="deviceName" label="设备名称" />
        <el-table-column prop="deviceType" label="设备类型">
          <template #default="{ row }">
            <el-tag :type="getDeviceTypeColor(row.deviceType)">
              {{ getDeviceTypeName(row.deviceType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="location" label="安装位置" />
        <el-table-column prop="project" label="所属项目" />
        <el-table-column prop="status" label="设备状态">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              <el-icon class="status-icon">
                <component :is="getStatusIcon(row.status)" />
              </el-icon>
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="lastHeartbeat" label="最后心跳" />
        <el-table-column prop="installDate" label="安装日期" />
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button
              size="small"
              type="success"
              @click="handleView(row)"
              v-if="row.status === 1"
            >
              查看监控
            </el-button>
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

    <!-- 监控预览对话框 -->
    <el-dialog v-model="previewVisible" title="监控预览" width="800px">
      <div class="monitor-preview">
        <div class="video-container">
          <div class="video-placeholder">
            <el-icon :size="60"><VideoCamera /></el-icon>
            <p>{{ currentDevice?.deviceName }} - 实时监控</p>
            <p class="video-tip">监控画面预览（模拟）</p>
          </div>
        </div>
        <div class="monitor-controls">
          <el-button-group>
            <el-button
              ><el-icon><ZoomIn /></el-icon>放大</el-button
            >
            <el-button
              ><el-icon><ZoomOut /></el-icon>缩小</el-button
            >
            <el-button
              ><el-icon><Camera /></el-icon>截图</el-button
            >
            <el-button
              ><el-icon><VideoPlay /></el-icon>录制</el-button
            >
          </el-button-group>
        </div>
      </div>
    </el-dialog>
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
    title: "设备总数",
    value: "24台",
    icon: "Monitor",
    color: "#409EFF",
  },
  {
    title: "在线设备",
    value: "20台",
    icon: "CircleCheck",
    color: "#67C23A",
  },
  {
    title: "离线设备",
    value: "3台",
    icon: "CircleClose",
    color: "#F56C6C",
  },
  {
    title: "故障设备",
    value: "1台",
    icon: "Warning",
    color: "#E6A23C",
  },
]);

// 搜索表单
const searchForm = reactive({
  deviceName: "",
  deviceType: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    deviceId: "CAM001",
    deviceName: "大门入口摄像头",
    deviceType: 1,
    location: "项目大门",
    project: "智慧工地A项目",
    status: 1,
    lastHeartbeat: "2024-01-16 14:30:25",
    installDate: "2024-01-01",
  },
  {
    id: 2,
    deviceId: "CAM002",
    deviceName: "施工区域摄像头",
    deviceType: 1,
    location: "主施工区",
    project: "智慧工地A项目",
    status: 1,
    lastHeartbeat: "2024-01-16 14:30:20",
    installDate: "2024-01-02",
  },
  {
    id: 3,
    deviceId: "SEN001",
    deviceName: "扬尘监测传感器",
    deviceType: 2,
    location: "施工现场中央",
    project: "智慧工地A项目",
    status: 1,
    lastHeartbeat: "2024-01-16 14:29:45",
    installDate: "2024-01-03",
  },
  {
    id: 4,
    deviceId: "ACC001",
    deviceName: "工人通道门禁",
    deviceType: 3,
    location: "工人入口",
    project: "智慧工地B项目",
    status: 2,
    lastHeartbeat: "2024-01-16 13:45:12",
    installDate: "2024-01-05",
  },
  {
    id: 5,
    deviceId: "ENV001",
    deviceName: "环境监测站",
    deviceType: 4,
    location: "办公区域",
    project: "智慧工地A项目",
    status: 3,
    lastHeartbeat: "2024-01-16 12:20:30",
    installDate: "2024-01-10",
  },
]);

const loading = ref(false);
const previewVisible = ref(false);
const currentDevice = ref<any>(null);

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 5,
});

// 获取设备类型名称
const getDeviceTypeName = (type: number) => {
  const typeMap: Record<number, string> = {
    1: "摄像头",
    2: "传感器",
    3: "门禁",
    4: "环境监测",
  };
  return typeMap[type] || "未知";
};

// 获取设备类型颜色
const getDeviceTypeColor = (type: number) => {
  const colorMap: Record<number, string> = {
    1: "primary",
    2: "success",
    3: "warning",
    4: "info",
  };
  return colorMap[type] || "info";
};

// 获取状态类型
const getStatusType = (status: number) => {
  const typeMap: Record<number, string> = {
    1: "success",
    2: "info",
    3: "danger",
    4: "warning",
  };
  return typeMap[status] || "info";
};

// 获取状态文本
const getStatusText = (status: number) => {
  const textMap: Record<number, string> = {
    1: "在线",
    2: "离线",
    3: "故障",
    4: "维护中",
  };
  return textMap[status] || "未知";
};

// 获取状态图标
const getStatusIcon = (status: number) => {
  const iconMap: Record<number, string> = {
    1: "CircleCheck",
    2: "CircleClose",
    3: "Warning",
    4: "Tools",
  };
  return iconMap[status] || "QuestionFilled";
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
  searchForm.deviceName = "";
  searchForm.deviceType = "";
  searchForm.status = "";
  handleSearch();
};

// 添加
const handleAdd = () => {
  ElMessage.info("添加监控设备功能开发中...");
};

// 刷新所有设备
const handleRefreshAll = () => {
  loading.value = true;
  setTimeout(() => {
    loading.value = false;
    ElMessage.success("设备状态刷新完成");
  }, 2000);
};

// 查看监控
const handleView = (row: any) => {
  currentDevice.value = row;
  previewVisible.value = true;
};

// 编辑
const handleEdit = (row: any) => {
  ElMessage.info(`编辑设备: ${row.deviceName}`);
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确认删除设备 "${row.deviceName}" 吗？`, "警告", {
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
.monitoring-container {
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

.status-icon {
  margin-right: 4px;
}

.monitor-preview {
  text-align: center;
}

.video-container {
  margin-bottom: 20px;
}

.video-placeholder {
  height: 400px;
  background-color: #f5f7f9;
  border: 2px dashed #dcdfe6;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: var(--app-text-color-regular);
}

.video-placeholder p {
  margin: 10px 0;
  font-size: 16px;
}

.video-tip {
  font-size: 14px !important;
  color: var(--app-text-color-placeholder) !important;
}

.monitor-controls {
  display: flex;
  justify-content: center;
}
</style>
