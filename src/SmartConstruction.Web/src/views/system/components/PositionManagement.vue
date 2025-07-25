<template>
  <div class="position-management">
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加岗位
      </el-button>
      <el-button type="success" @click="handleExport">
        <el-icon><Download /></el-icon>
        导出数据
      </el-button>
    </div>

    <!-- 搜索区域 -->
    <div class="search-container">
      <el-form :model="searchForm" inline>
        <el-form-item label="岗位名称">
          <el-input
            v-model="searchForm.positionName"
            placeholder="请输入岗位名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="所属部门">
          <el-select
            v-model="searchForm.deptId"
            placeholder="请选择部门"
            clearable
          >
            <el-option label="总经理办公室" value="1" />
            <el-option label="工程部" value="2" />
            <el-option label="财务部" value="3" />
            <el-option label="设计科" value="21" />
            <el-option label="施工科" value="22" />
          </el-select>
        </el-form-item>
        <el-form-item label="岗位类型">
          <el-select
            v-model="searchForm.positionType"
            placeholder="请选择类型"
            clearable
          >
            <el-option label="管理岗" value="1" />
            <el-option label="技术岗" value="2" />
            <el-option label="操作岗" value="3" />
            <el-option label="服务岗" value="4" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select
            v-model="searchForm.status"
            placeholder="请选择状态"
            clearable
          >
            <el-option label="启用" value="1" />
            <el-option label="禁用" value="0" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="handleSearch">
            <el-icon><Search /></el-icon>
            搜索
          </el-button>
          <el-button @click="handleReset">
            <el-icon><Refresh /></el-icon>
            重置
          </el-button>
        </el-form-item>
      </el-form>
    </div>

    <!-- 表格区域 -->
    <el-table :data="tableData" style="width: 100%" v-loading="loading">
      <el-table-column prop="positionCode" label="岗位编码" width="120" />
      <el-table-column prop="positionName" label="岗位名称" />
      <el-table-column prop="deptName" label="所属部门" />
      <el-table-column prop="positionType" label="岗位类型">
        <template #default="{ row }">
          <el-tag :type="getPositionTypeColor(row.positionType)">
            {{ getPositionTypeName(row.positionType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="level" label="岗位级别" />
      <el-table-column prop="minSalary" label="最低薪资">
        <template #default="{ row }">
          {{ row.minSalary ? `¥${row.minSalary}` : "-" }}
        </template>
      </el-table-column>
      <el-table-column prop="maxSalary" label="最高薪资">
        <template #default="{ row }">
          {{ row.maxSalary ? `¥${row.maxSalary}` : "-" }}
        </template>
      </el-table-column>
      <el-table-column prop="sort" label="排序" width="80" />
      <el-table-column prop="status" label="状态" width="80">
        <template #default="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? "启用" : "禁用" }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="180" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button size="small" @click="handleEdit(row)"> 编辑 </el-button>
          <el-button
            size="small"
            :type="row.status === 1 ? 'warning' : 'success'"
            @click="handleToggleStatus(row)"
          >
            {{ row.status === 1 ? "禁用" : "启用" }}
          </el-button>
          <el-button size="small" type="danger" @click="handleDelete(row)">
            删除
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

    <!-- 添加/编辑对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="700px"
      @close="handleDialogClose"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="岗位编码" prop="positionCode">
              <el-input
                v-model="formData.positionCode"
                placeholder="请输入岗位编码"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="岗位名称" prop="positionName">
              <el-input
                v-model="formData.positionName"
                placeholder="请输入岗位名称"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="所属部门" prop="deptId">
              <el-select
                v-model="formData.deptId"
                placeholder="请选择部门"
                style="width: 100%"
              >
                <el-option label="总经理办公室" value="1" />
                <el-option label="工程部" value="2" />
                <el-option label="财务部" value="3" />
                <el-option label="设计科" value="21" />
                <el-option label="施工科" value="22" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="岗位类型" prop="positionType">
              <el-select
                v-model="formData.positionType"
                placeholder="请选择类型"
                style="width: 100%"
              >
                <el-option label="管理岗" value="1" />
                <el-option label="技术岗" value="2" />
                <el-option label="操作岗" value="3" />
                <el-option label="服务岗" value="4" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="岗位级别" prop="level">
              <el-select
                v-model="formData.level"
                placeholder="请选择级别"
                style="width: 100%"
              >
                <el-option label="初级" value="1" />
                <el-option label="中级" value="2" />
                <el-option label="高级" value="3" />
                <el-option label="专家级" value="4" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="排序" prop="sort">
              <el-input-number
                v-model="formData.sort"
                :min="0"
                :max="999"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="最低薪资">
              <el-input-number
                v-model="formData.minSalary"
                :min="0"
                :precision="2"
                placeholder="请输入最低薪资"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="最高薪资">
              <el-input-number
                v-model="formData.maxSalary"
                :min="0"
                :precision="2"
                placeholder="请输入最高薪资"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="formData.status">
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="岗位职责">
          <el-input
            v-model="formData.responsibilities"
            type="textarea"
            :rows="3"
            placeholder="请输入岗位职责"
          />
        </el-form-item>
        <el-form-item label="任职要求">
          <el-input
            v-model="formData.requirements"
            type="textarea"
            :rows="3"
            placeholder="请输入任职要求"
          />
        </el-form-item>
        <el-form-item label="备注">
          <el-input
            v-model="formData.remark"
            type="textarea"
            :rows="2"
            placeholder="请输入备注"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button
          type="primary"
          @click="handleSubmit"
          :loading="submitLoading"
        >
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { ElMessage, ElMessageBox, type FormInstance } from "element-plus";

// 搜索表单
const searchForm = reactive({
  positionName: "",
  deptId: "",
  positionType: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    positionCode: "P001",
    positionName: "总经理",
    deptName: "总经理办公室",
    deptId: "1",
    positionType: 1,
    level: "4",
    minSalary: 50000,
    maxSalary: 100000,
    sort: 1,
    status: 1,
    createTime: "2024-01-01 10:00:00",
    responsibilities: "负责公司整体运营管理，制定公司发展战略",
    requirements: "本科以上学历，10年以上管理经验",
    remark: "公司最高管理职位",
  },
  {
    id: 2,
    positionCode: "P002",
    positionName: "工程总监",
    deptName: "工程部",
    deptId: "2",
    positionType: 1,
    level: "3",
    minSalary: 30000,
    maxSalary: 50000,
    sort: 2,
    status: 1,
    createTime: "2024-01-01 10:30:00",
    responsibilities: "负责工程项目的整体管理和技术指导",
    requirements: "工程类专业本科以上，8年以上工程管理经验",
    remark: "工程部门负责人",
  },
  {
    id: 3,
    positionCode: "P003",
    positionName: "高级工程师",
    deptName: "设计科",
    deptId: "21",
    positionType: 2,
    level: "3",
    minSalary: 15000,
    maxSalary: 25000,
    sort: 3,
    status: 1,
    createTime: "2024-01-01 11:00:00",
    responsibilities: "负责工程设计、技术方案制定",
    requirements: "工程类专业本科以上，5年以上设计经验",
    remark: "技术骨干岗位",
  },
  {
    id: 4,
    positionCode: "P004",
    positionName: "施工员",
    deptName: "施工科",
    deptId: "22",
    positionType: 3,
    level: "2",
    minSalary: 8000,
    maxSalary: 12000,
    sort: 4,
    status: 1,
    createTime: "2024-01-01 11:30:00",
    responsibilities: "负责现场施工管理，确保施工质量和进度",
    requirements: "建筑类专业大专以上，3年以上施工经验",
    remark: "现场管理岗位",
  },
  {
    id: 5,
    positionCode: "P005",
    positionName: "财务经理",
    deptName: "财务部",
    deptId: "3",
    positionType: 1,
    level: "3",
    minSalary: 20000,
    maxSalary: 30000,
    sort: 5,
    status: 1,
    createTime: "2024-01-01 12:00:00",
    responsibilities: "负责公司财务管理，财务报表编制",
    requirements: "财务类专业本科以上，中级会计师以上",
    remark: "财务部门负责人",
  },
]);

const loading = ref(false);
const dialogVisible = ref(false);
const submitLoading = ref(false);
const formRef = ref<FormInstance>();

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 5,
});

// 表单数据
const formData = reactive({
  id: null,
  positionCode: "",
  positionName: "",
  deptId: "",
  positionType: "",
  level: "",
  minSalary: null,
  maxSalary: null,
  sort: 0,
  status: 1,
  responsibilities: "",
  requirements: "",
  remark: "",
});

// 表单验证规则
const formRules = {
  positionCode: [
    { required: true, message: "请输入岗位编码", trigger: "blur" },
  ],
  positionName: [
    { required: true, message: "请输入岗位名称", trigger: "blur" },
  ],
  deptId: [{ required: true, message: "请选择所属部门", trigger: "change" }],
  positionType: [
    { required: true, message: "请选择岗位类型", trigger: "change" },
  ],
  level: [{ required: true, message: "请选择岗位级别", trigger: "change" }],
};

// 对话框标题
const dialogTitle = computed(() => {
  return formData.id ? "编辑岗位" : "添加岗位";
});

// 获取岗位类型名称
const getPositionTypeName = (type: number) => {
  const typeMap: Record<number, string> = {
    1: "管理岗",
    2: "技术岗",
    3: "操作岗",
    4: "服务岗",
  };
  return typeMap[type] || "未知";
};

// 获取岗位类型颜色
const getPositionTypeColor = (type: number) => {
  const colorMap: Record<number, string> = {
    1: "danger",
    2: "primary",
    3: "success",
    4: "warning",
  };
  return colorMap[type] || "info";
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
  Object.assign(searchForm, {
    positionName: "",
    deptId: "",
    positionType: "",
    status: "",
  });
  handleSearch();
};

// 添加
const handleAdd = () => {
  Object.assign(formData, {
    id: null,
    positionCode: "",
    positionName: "",
    deptId: "",
    positionType: "",
    level: "",
    minSalary: null,
    maxSalary: null,
    sort: 0,
    status: 1,
    responsibilities: "",
    requirements: "",
    remark: "",
  });
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (row: any) => {
  Object.assign(formData, { ...row });
  dialogVisible.value = true;
};

// 切换状态
const handleToggleStatus = (row: any) => {
  const action = row.status === 1 ? "禁用" : "启用";
  ElMessageBox.confirm(`确认${action}岗位 "${row.positionName}" 吗？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      row.status = row.status === 1 ? 0 : 1;
      ElMessage.success(`${action}成功`);
    })
    .catch(() => {});
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确认删除岗位 "${row.positionName}" 吗？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      ElMessage.success("删除成功");
    })
    .catch(() => {});
};

// 导出
const handleExport = () => {
  ElMessage.info("导出功能开发中...");
};

// 提交表单
const handleSubmit = async () => {
  if (!formRef.value) return;

  await formRef.value.validate((valid) => {
    if (valid) {
      submitLoading.value = true;
      setTimeout(() => {
        submitLoading.value = false;
        dialogVisible.value = false;
        ElMessage.success(formData.id ? "编辑成功" : "添加成功");
        handleSearch();
      }, 1000);
    }
  });
};

// 关闭对话框
const handleDialogClose = () => {
  formRef.value?.resetFields();
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
.position-management {
  padding: 0;
}

.toolbar {
  margin-bottom: 20px;
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
