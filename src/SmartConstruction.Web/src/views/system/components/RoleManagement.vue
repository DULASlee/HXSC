<template>
  <div class="role-management">
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加角色
      </el-button>
      <el-button type="success" @click="handleExport">
        <el-icon><Download /></el-icon>
        导出数据
      </el-button>
    </div>

    <!-- 搜索区域 -->
    <div class="search-container">
      <el-form :model="searchForm" inline>
        <el-form-item label="角色名称">
          <el-input
            v-model="searchForm.roleName"
            placeholder="请输入角色名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="角色编码">
          <el-input
            v-model="searchForm.roleCode"
            placeholder="请输入角色编码"
            clearable
          />
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
      <el-table-column prop="roleCode" label="角色编码" width="120" />
      <el-table-column prop="roleName" label="角色名称" />
      <el-table-column prop="roleType" label="角色类型">
        <template #default="{ row }">
          <el-tag :type="getRoleTypeColor(row.roleType)">
            {{ getRoleTypeName(row.roleType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="dataScope" label="数据权限">
        <template #default="{ row }">
          <el-tag :type="getDataScopeColor(row.dataScope)">
            {{ getDataScopeName(row.dataScope) }}
          </el-tag>
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
      <el-table-column label="操作" width="250" fixed="right">
        <template #default="{ row }">
          <el-button size="small" @click="handlePermission(row)">
            权限配置
          </el-button>
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
      width="600px"
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
            <el-form-item label="角色编码" prop="roleCode">
              <el-input
                v-model="formData.roleCode"
                placeholder="请输入角色编码"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="角色名称" prop="roleName">
              <el-input
                v-model="formData.roleName"
                placeholder="请输入角色名称"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="角色类型" prop="roleType">
              <el-select
                v-model="formData.roleType"
                placeholder="请选择角色类型"
                style="width: 100%"
              >
                <el-option label="系统角色" value="1" />
                <el-option label="业务角色" value="2" />
                <el-option label="自定义角色" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据权限" prop="dataScope">
              <el-select
                v-model="formData.dataScope"
                placeholder="请选择数据权限"
                style="width: 100%"
              >
                <el-option label="全部数据权限" value="1" />
                <el-option label="自定义数据权限" value="2" />
                <el-option label="本部门数据权限" value="3" />
                <el-option label="本部门及以下数据权限" value="4" />
                <el-option label="仅本人数据权限" value="5" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
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
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="formData.status">
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="角色描述">
          <el-input
            v-model="formData.description"
            type="textarea"
            :rows="3"
            placeholder="请输入角色描述"
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

    <!-- 权限配置对话框 -->
    <el-dialog v-model="permissionDialogVisible" title="权限配置" width="800px">
      <div class="permission-config">
        <div class="role-info">
          <el-descriptions :column="2" border>
            <el-descriptions-item label="角色名称">{{
              currentRole?.roleName
            }}</el-descriptions-item>
            <el-descriptions-item label="角色编码">{{
              currentRole?.roleCode
            }}</el-descriptions-item>
          </el-descriptions>
        </div>

        <div class="permission-tree">
          <h4>菜单权限</h4>
          <el-tree
            ref="menuTreeRef"
            :data="menuTreeData"
            :props="{ children: 'children', label: 'menuName' }"
            show-checkbox
            node-key="id"
            :default-checked-keys="checkedMenuKeys"
            :default-expand-all="true"
          />
        </div>
      </div>
      <template #footer>
        <el-button @click="permissionDialogVisible = false">取消</el-button>
        <el-button
          type="primary"
          @click="handleSavePermission"
          :loading="permissionLoading"
        >
          保存权限
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
  roleName: "",
  roleCode: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    roleCode: "ADMIN",
    roleName: "超级管理员",
    roleType: 1,
    dataScope: 1,
    sort: 1,
    status: 1,
    createTime: "2024-01-01 10:00:00",
    description: "系统超级管理员，拥有所有权限",
    remark: "系统内置角色，不可删除",
  },
  {
    id: 2,
    roleCode: "PROJECT_MANAGER",
    roleName: "项目经理",
    roleType: 2,
    dataScope: 3,
    sort: 2,
    status: 1,
    createTime: "2024-01-01 10:30:00",
    description: "项目管理人员，负责项目整体管理",
    remark: "业务角色",
  },
  {
    id: 3,
    roleCode: "ENGINEER",
    roleName: "工程师",
    roleType: 2,
    dataScope: 4,
    sort: 3,
    status: 1,
    createTime: "2024-01-01 11:00:00",
    description: "技术工程师，负责技术相关工作",
    remark: "技术角色",
  },
  {
    id: 4,
    roleCode: "WORKER",
    roleName: "普通工人",
    roleType: 2,
    dataScope: 5,
    sort: 4,
    status: 1,
    createTime: "2024-01-01 11:30:00",
    description: "现场工人，只能查看自己的相关信息",
    remark: "基础角色",
  },
  {
    id: 5,
    roleCode: "FINANCE",
    roleName: "财务人员",
    roleType: 2,
    dataScope: 3,
    sort: 5,
    status: 1,
    createTime: "2024-01-01 12:00:00",
    description: "财务管理人员，负责财务相关工作",
    remark: "财务角色",
  },
]);

// 菜单树数据
const menuTreeData = ref([
  {
    id: 1,
    menuName: "首页",
    children: [],
  },
  {
    id: 2,
    menuName: "项目管理",
    children: [
      { id: 21, menuName: "项目列表" },
      { id: 22, menuName: "项目新增" },
      { id: 23, menuName: "项目编辑" },
      { id: 24, menuName: "项目删除" },
    ],
  },
  {
    id: 3,
    menuName: "工人管理",
    children: [
      { id: 31, menuName: "工人列表" },
      { id: 32, menuName: "工人新增" },
      { id: 33, menuName: "工人编辑" },
      { id: 34, menuName: "工人删除" },
    ],
  },
  {
    id: 4,
    menuName: "考勤管理",
    children: [
      { id: 41, menuName: "考勤记录" },
      { id: 42, menuName: "考勤统计" },
      { id: 43, menuName: "考勤导出" },
    ],
  },
  {
    id: 5,
    menuName: "监控管理",
    children: [
      { id: 51, menuName: "设备监控" },
      { id: 52, menuName: "设备管理" },
      { id: 53, menuName: "监控配置" },
    ],
  },
  {
    id: 6,
    menuName: "系统管理",
    children: [
      { id: 61, menuName: "用户管理" },
      { id: 62, menuName: "角色管理" },
      { id: 63, menuName: "菜单管理" },
      { id: 64, menuName: "权限配置" },
    ],
  },
]);

const loading = ref(false);
const dialogVisible = ref(false);
const permissionDialogVisible = ref(false);
const submitLoading = ref(false);
const permissionLoading = ref(false);
const formRef = ref<FormInstance>();
const menuTreeRef = ref();
const currentRole = ref<any>(null);
const checkedMenuKeys = ref<number[]>([]);

// 分页
const pagination = reactive({
  currentPage: 1,
  pageSize: 10,
  total: 5,
});

// 表单数据
const formData = reactive({
  id: null,
  roleCode: "",
  roleName: "",
  roleType: "",
  dataScope: "",
  sort: 0,
  status: 1,
  description: "",
  remark: "",
});

// 表单验证规则
const formRules = {
  roleCode: [{ required: true, message: "请输入角色编码", trigger: "blur" }],
  roleName: [{ required: true, message: "请输入角色名称", trigger: "blur" }],
  roleType: [{ required: true, message: "请选择角色类型", trigger: "change" }],
  dataScope: [{ required: true, message: "请选择数据权限", trigger: "change" }],
};

// 对话框标题
const dialogTitle = computed(() => {
  return formData.id ? "编辑角色" : "添加角色";
});

// 获取角色类型名称
const getRoleTypeName = (type: number) => {
  const typeMap: Record<number, string> = {
    1: "系统角色",
    2: "业务角色",
    3: "自定义角色",
  };
  return typeMap[type] || "未知";
};

// 获取角色类型颜色
const getRoleTypeColor = (type: number) => {
  const colorMap: Record<number, string> = {
    1: "danger",
    2: "primary",
    3: "success",
  };
  return colorMap[type] || "info";
};

// 获取数据权限名称
const getDataScopeName = (scope: number) => {
  const scopeMap: Record<number, string> = {
    1: "全部数据",
    2: "自定义数据",
    3: "本部门数据",
    4: "本部门及以下",
    5: "仅本人数据",
  };
  return scopeMap[scope] || "未知";
};

// 获取数据权限颜色
const getDataScopeColor = (scope: number) => {
  const colorMap: Record<number, string> = {
    1: "danger",
    2: "warning",
    3: "primary",
    4: "success",
    5: "info",
  };
  return colorMap[scope] || "info";
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
    roleName: "",
    roleCode: "",
    status: "",
  });
  handleSearch();
};

// 添加
const handleAdd = () => {
  Object.assign(formData, {
    id: null,
    roleCode: "",
    roleName: "",
    roleType: "",
    dataScope: "",
    sort: 0,
    status: 1,
    description: "",
    remark: "",
  });
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (row: any) => {
  Object.assign(formData, { ...row });
  dialogVisible.value = true;
};

// 权限配置
const handlePermission = (row: any) => {
  currentRole.value = row;
  // 模拟获取角色已有权限
  checkedMenuKeys.value = [1, 21, 22, 31, 32, 41, 51, 61];
  permissionDialogVisible.value = true;
};

// 保存权限
const handleSavePermission = () => {
  const checkedKeys = menuTreeRef.value.getCheckedKeys();
  const halfCheckedKeys = menuTreeRef.value.getHalfCheckedKeys();

  permissionLoading.value = true;
  setTimeout(() => {
    permissionLoading.value = false;
    permissionDialogVisible.value = false;
    ElMessage.success("权限配置保存成功");
    console.log("选中的权限:", [...checkedKeys, ...halfCheckedKeys]);
  }, 1000);
};

// 切换状态
const handleToggleStatus = (row: any) => {
  const action = row.status === 1 ? "禁用" : "启用";
  ElMessageBox.confirm(`确认${action}角色 "${row.roleName}" 吗？`, "提示", {
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
  if (row.roleCode === "ADMIN") {
    ElMessage.warning("系统内置角色不能删除");
    return;
  }

  ElMessageBox.confirm(`确认删除角色 "${row.roleName}" 吗？`, "警告", {
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
.role-management {
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

.permission-config {
  padding: 0;
}

.role-info {
  margin-bottom: 20px;
}

.permission-tree h4 {
  margin: 0 0 15px 0;
  color: var(--app-text-color);
  font-size: 16px;
  font-weight: 500;
}
</style>
