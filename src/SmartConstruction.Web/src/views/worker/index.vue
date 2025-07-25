<template>
  <div class="worker-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ i18nStore.t("menu.worker") }}</span>
          <el-button type="primary" @click="handleAdd">
            <el-icon><Plus /></el-icon>
            {{ i18nStore.t("common.add") }}
          </el-button>
        </div>
      </template>

      <!-- 搜索区域 -->
      <div class="search-container">
        <el-form :model="searchForm" inline>
          <el-form-item label="工人姓名">
            <el-input
              v-model="searchForm.name"
              placeholder="请输入工人姓名"
              clearable
            />
          </el-form-item>
          <el-form-item label="工种">
            <el-select
              v-model="searchForm.jobType"
              placeholder="请选择工种"
              clearable
            >
              <el-option label="钢筋工" value="1" />
              <el-option label="木工" value="2" />
              <el-option label="混凝土工" value="3" />
              <el-option label="电工" value="4" />
              <el-option label="焊工" value="5" />
            </el-select>
          </el-form-item>
          <el-form-item label="状态">
            <el-select
              v-model="searchForm.status"
              placeholder="请选择状态"
              clearable
            >
              <el-option label="在职" value="1" />
              <el-option label="离职" value="2" />
              <el-option label="请假" value="3" />
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
        <el-table-column prop="id" label="工号" width="80" />
        <el-table-column prop="name" label="姓名" />
        <el-table-column prop="phone" label="联系电话" />
        <el-table-column prop="jobType" label="工种">
          <template #default="{ row }">
            <el-tag :type="getJobTypeColor(row.jobType)">
              {{ getJobTypeName(row.jobType) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column prop="project" label="所属项目" />
        <el-table-column prop="entryDate" label="入职时间" />
        <el-table-column prop="status" label="状态">
          <template #default="{ row }">
            <el-tag :type="getStatusType(row.status)">
              {{ getStatusText(row.status) }}
            </el-tag>
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template #default="{ row }">
            <el-button size="small" @click="handleView(row)"> 查看 </el-button>
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

    <!-- 工人详情对话框 -->
    <el-dialog v-model="dialogVisible" title="工人详情" width="600px">
      <div v-if="currentWorker" class="worker-detail">
        <el-descriptions :column="2" border>
          <el-descriptions-item label="工号">{{
            currentWorker.id
          }}</el-descriptions-item>
          <el-descriptions-item label="姓名">{{
            currentWorker.name
          }}</el-descriptions-item>
          <el-descriptions-item label="联系电话">{{
            currentWorker.phone
          }}</el-descriptions-item>
          <el-descriptions-item label="身份证号">{{
            currentWorker.idCard
          }}</el-descriptions-item>
          <el-descriptions-item label="工种">{{
            getJobTypeName(currentWorker.jobType)
          }}</el-descriptions-item>
          <el-descriptions-item label="所属项目">{{
            currentWorker.project
          }}</el-descriptions-item>
          <el-descriptions-item label="入职时间">{{
            currentWorker.entryDate
          }}</el-descriptions-item>
          <el-descriptions-item label="状态">
            <el-tag :type="getStatusType(currentWorker.status)">
              {{ getStatusText(currentWorker.status) }}
            </el-tag>
          </el-descriptions-item>
          <el-descriptions-item label="紧急联系人">{{
            currentWorker.emergencyContact
          }}</el-descriptions-item>
          <el-descriptions-item label="紧急联系电话">{{
            currentWorker.emergencyPhone
          }}</el-descriptions-item>
          <el-descriptions-item label="家庭住址" :span="2">{{
            currentWorker.address
          }}</el-descriptions-item>
        </el-descriptions>
      </div>
    </el-dialog>
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
  jobType: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: "W001",
    name: "张三",
    phone: "13800138001",
    idCard: "110101199001011234",
    jobType: 1,
    project: "智慧工地A项目",
    entryDate: "2024-01-15",
    status: 1,
    emergencyContact: "李四",
    emergencyPhone: "13800138002",
    address: "北京市朝阳区某某街道123号",
  },
  {
    id: "W002",
    name: "王五",
    phone: "13800138003",
    idCard: "110101199002021234",
    jobType: 2,
    project: "智慧工地B项目",
    entryDate: "2024-01-20",
    status: 1,
    emergencyContact: "赵六",
    emergencyPhone: "13800138004",
    address: "北京市海淀区某某路456号",
  },
  {
    id: "W003",
    name: "刘七",
    phone: "13800138005",
    idCard: "110101199003031234",
    jobType: 3,
    project: "智慧工地A项目",
    entryDate: "2023-12-01",
    status: 3,
    emergencyContact: "孙八",
    emergencyPhone: "13800138006",
    address: "北京市西城区某某胡同789号",
  },
]);

const loading = ref(false);
const dialogVisible = ref(false);
const currentWorker = ref<any>(null);

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 3,
});

// 获取工种名称
const getJobTypeName = (type: number) => {
  const typeMap: Record<number, string> = {
    1: "钢筋工",
    2: "木工",
    3: "混凝土工",
    4: "电工",
    5: "焊工",
  };
  return typeMap[type] || "未知";
};

// 获取工种颜色
const getJobTypeColor = (type: number) => {
  const colorMap: Record<number, string> = {
    1: "success",
    2: "warning",
    3: "info",
    4: "danger",
    5: "primary",
  };
  return colorMap[type] || "info";
};

// 获取状态类型
const getStatusType = (status: number) => {
  const typeMap: Record<number, string> = {
    1: "success",
    2: "info",
    3: "warning",
  };
  return typeMap[status] || "info";
};

// 获取状态文本
const getStatusText = (status: number) => {
  const textMap: Record<number, string> = {
    1: "在职",
    2: "离职",
    3: "请假",
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
  searchForm.jobType = "";
  searchForm.status = "";
  handleSearch();
};

// 添加
const handleAdd = () => {
  ElMessage.info("添加工人功能开发中...");
};

// 查看详情
const handleView = (row: any) => {
  currentWorker.value = row;
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (row: any) => {
  ElMessage.info(`编辑工人: ${row.name}`);
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确认删除工人 "${row.name}" 吗？`, "警告", {
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
.worker-container {
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

.worker-detail {
  padding: 20px 0;
}
</style>
