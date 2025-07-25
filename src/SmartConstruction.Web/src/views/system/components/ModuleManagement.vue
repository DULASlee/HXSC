<template>
  <div class="module-management">
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加模块
      </el-button>
      <el-button type="success" @click="handleImport">
        <el-icon><Upload /></el-icon>
        导入模块
      </el-button>
    </div>

    <!-- 搜索区域 -->
    <div class="search-container">
      <el-form :model="searchForm" inline>
        <el-form-item label="模块名称">
          <el-input
            v-model="searchForm.moduleName"
            placeholder="请输入模块名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="模块类型">
          <el-select
            v-model="searchForm.moduleType"
            placeholder="请选择类型"
            clearable
          >
            <el-option label="业务模块" value="business" />
            <el-option label="基础模块" value="base" />
            <el-option label="工具模块" value="tool" />
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
      <el-table-column prop="moduleCode" label="模块编码" width="120" />
      <el-table-column prop="moduleName" label="模块名称" />
      <el-table-column prop="moduleType" label="模块类型">
        <template #default="{ row }">
          <el-tag :type="getModuleTypeColor(row.moduleType)">
            {{ getModuleTypeName(row.moduleType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="tableName" label="数据表名" />
      <el-table-column prop="entityName" label="实体类名" />
      <el-table-column prop="author" label="作者" />
      <el-table-column prop="version" label="版本" width="80" />
      <el-table-column prop="createTime" label="创建时间" width="180" />
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button size="small" @click="handlePreview(row)"> 预览 </el-button>
          <el-button size="small" type="success" @click="handleGenerate(row)">
            生成代码
          </el-button>
          <el-button size="small" @click="handleEdit(row)"> 编辑 </el-button>
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
            <el-form-item label="模块编码" prop="moduleCode">
              <el-input
                v-model="formData.moduleCode"
                placeholder="请输入模块编码"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="模块名称" prop="moduleName">
              <el-input
                v-model="formData.moduleName"
                placeholder="请输入模块名称"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="模块类型" prop="moduleType">
              <el-select
                v-model="formData.moduleType"
                placeholder="请选择类型"
                style="width: 100%"
              >
                <el-option label="业务模块" value="business" />
                <el-option label="基础模块" value="base" />
                <el-option label="工具模块" value="tool" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据表名" prop="tableName">
              <el-input
                v-model="formData.tableName"
                placeholder="请输入数据表名"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="实体类名" prop="entityName">
              <el-input
                v-model="formData.entityName"
                placeholder="请输入实体类名"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="作者" prop="author">
              <el-input v-model="formData.author" placeholder="请输入作者" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="包路径" prop="packageName">
          <el-input v-model="formData.packageName" placeholder="请输入包路径" />
        </el-form-item>
        <el-form-item label="模块描述">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="3"
            placeholder="请输入模块描述"
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
  moduleName: "",
  moduleType: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    moduleCode: "USER_MGMT",
    moduleName: "用户管理",
    moduleType: "business",
    tableName: "sys_user",
    entityName: "SysUser",
    packageName: "com.example.system.user",
    author: "系统管理员",
    version: "1.0.0",
    createTime: "2024-01-01 10:00:00",
    description: "系统用户管理模块",
  },
  {
    id: 2,
    moduleCode: "PROJECT_MGMT",
    moduleName: "项目管理",
    moduleType: "business",
    tableName: "biz_project",
    entityName: "BizProject",
    packageName: "com.example.business.project",
    author: "开发人员",
    version: "1.0.0",
    createTime: "2024-01-02 10:00:00",
    description: "项目信息管理模块",
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
  total: 2,
});

// 表单数据
const formData = reactive({
  id: null,
  moduleCode: "",
  moduleName: "",
  moduleType: "business",
  tableName: "",
  entityName: "",
  packageName: "",
  author: "",
  version: "1.0.0",
  description: "",
});

// 表单验证规则
const formRules = {
  moduleCode: [{ required: true, message: "请输入模块编码", trigger: "blur" }],
  moduleName: [{ required: true, message: "请输入模块名称", trigger: "blur" }],
  moduleType: [
    { required: true, message: "请选择模块类型", trigger: "change" },
  ],
  tableName: [{ required: true, message: "请输入数据表名", trigger: "blur" }],
  entityName: [{ required: true, message: "请输入实体类名", trigger: "blur" }],
  author: [{ required: true, message: "请输入作者", trigger: "blur" }],
};

// 对话框标题
const dialogTitle = computed(() => {
  return formData.id ? "编辑模块" : "添加模块";
});

// 获取模块类型名称
const getModuleTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    business: "业务模块",
    base: "基础模块",
    tool: "工具模块",
  };
  return typeMap[type] || "未知";
};

// 获取模块类型颜色
const getModuleTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    business: "primary",
    base: "success",
    tool: "warning",
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
    moduleName: "",
    moduleType: "",
  });
  handleSearch();
};

// 添加
const handleAdd = () => {
  Object.assign(formData, {
    id: null,
    moduleCode: "",
    moduleName: "",
    moduleType: "business",
    tableName: "",
    entityName: "",
    packageName: "",
    author: "",
    version: "1.0.0",
    description: "",
  });
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (row: any) => {
  Object.assign(formData, { ...row });
  dialogVisible.value = true;
};

// 预览
const handlePreview = (row: any) => {
  ElMessage.info(`预览模块: ${row.moduleName}`);
};

// 生成代码
const handleGenerate = (row: any) => {
  ElMessageBox.confirm(`确认生成模块 "${row.moduleName}" 的代码吗？`, "提示", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      ElMessage.success("代码生成成功");
    })
    .catch(() => {});
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确认删除模块 "${row.moduleName}" 吗？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      ElMessage.success("删除成功");
    })
    .catch(() => {});
};

// 导入模块
const handleImport = () => {
  ElMessage.info("导入模块功能开发中...");
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
.module-management {
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
