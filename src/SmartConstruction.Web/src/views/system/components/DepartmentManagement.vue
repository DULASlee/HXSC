<template>
  <div class="department-management">
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加部门
      </el-button>
      <el-button type="success" @click="handleExpandAll">
        <el-icon><Sort /></el-icon>
        {{ isExpandAll ? "收起全部" : "展开全部" }}
      </el-button>
    </div>

    <!-- 搜索区域 -->
    <div class="search-container">
      <el-form :model="searchForm" inline>
        <el-form-item label="部门名称">
          <el-input
            v-model="searchForm.deptName"
            placeholder="请输入部门名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="所属公司">
          <el-select
            v-model="searchForm.companyId"
            placeholder="请选择公司"
            clearable
          >
            <el-option label="华建集团总部" value="1" />
            <el-option label="华建北京分公司" value="11" />
            <el-option label="中建科技总部" value="2" />
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
    <el-table
      :data="tableData"
      style="width: 100%"
      v-loading="loading"
      row-key="id"
      :default-expand-all="isExpandAll"
      :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
    >
      <el-table-column prop="deptName" label="部门名称" width="200" />
      <el-table-column prop="companyName" label="所属公司" />
      <el-table-column prop="deptCode" label="部门编码" />
      <el-table-column prop="leader" label="部门负责人" />
      <el-table-column prop="phone" label="联系电话" />
      <el-table-column prop="email" label="邮箱" />
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
          <el-button size="small" @click="handleAdd(row)"> 新增 </el-button>
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
            <el-form-item label="部门名称" prop="deptName">
              <el-input
                v-model="formData.deptName"
                placeholder="请输入部门名称"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="部门编码" prop="deptCode">
              <el-input
                v-model="formData.deptCode"
                placeholder="请输入部门编码"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="所属公司" prop="companyId">
              <el-select
                v-model="formData.companyId"
                placeholder="请选择公司"
                style="width: 100%"
              >
                <el-option label="华建集团总部" value="1" />
                <el-option label="华建北京分公司" value="11" />
                <el-option label="中建科技总部" value="2" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="上级部门" prop="parentId">
              <el-tree-select
                v-model="formData.parentId"
                :data="deptTreeData"
                :props="{
                  value: 'id',
                  label: 'deptName',
                  children: 'children',
                }"
                placeholder="请选择上级部门"
                style="width: 100%"
                check-strictly
                :render-after-expand="false"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="部门负责人" prop="leader">
              <el-input
                v-model="formData.leader"
                placeholder="请输入部门负责人"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="phone">
              <el-input v-model="formData.phone" placeholder="请输入联系电话" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="邮箱" prop="email">
              <el-input v-model="formData.email" placeholder="请输入邮箱" />
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
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="formData.status">
                <el-radio :label="1">启用</el-radio>
                <el-radio :label="0">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="备注">
          <el-input
            v-model="formData.remark"
            type="textarea"
            :rows="3"
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
  deptName: "",
  companyId: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    deptName: "总经理办公室",
    companyName: "华建集团总部",
    companyId: "1",
    deptCode: "GM001",
    leader: "张总",
    phone: "010-88888888",
    email: "gm@huajian.com",
    sort: 1,
    status: 1,
    createTime: "2024-01-01 10:00:00",
    remark: "总经理办公室",
    children: [
      {
        id: 11,
        deptName: "秘书处",
        companyName: "华建集团总部",
        companyId: "1",
        deptCode: "SEC001",
        leader: "李秘书",
        phone: "010-88888801",
        email: "sec@huajian.com",
        sort: 1,
        status: 1,
        createTime: "2024-01-01 11:00:00",
        remark: "秘书处",
      },
    ],
  },
  {
    id: 2,
    deptName: "工程部",
    companyName: "华建集团总部",
    companyId: "1",
    deptCode: "ENG001",
    leader: "王工",
    phone: "010-88888889",
    email: "eng@huajian.com",
    sort: 2,
    status: 1,
    createTime: "2024-01-01 10:30:00",
    remark: "工程技术部门",
    children: [
      {
        id: 21,
        deptName: "设计科",
        companyName: "华建集团总部",
        companyId: "1",
        deptCode: "DES001",
        leader: "刘设计师",
        phone: "010-88888821",
        email: "design@huajian.com",
        sort: 1,
        status: 1,
        createTime: "2024-01-01 11:30:00",
        remark: "设计科",
      },
      {
        id: 22,
        deptName: "施工科",
        companyName: "华建集团总部",
        companyId: "1",
        deptCode: "CON001",
        leader: "赵工程师",
        phone: "010-88888822",
        email: "construction@huajian.com",
        sort: 2,
        status: 1,
        createTime: "2024-01-01 11:45:00",
        remark: "施工科",
      },
    ],
  },
  {
    id: 3,
    deptName: "财务部",
    companyName: "华建集团总部",
    companyId: "1",
    deptCode: "FIN001",
    leader: "钱会计",
    phone: "010-88888890",
    email: "finance@huajian.com",
    sort: 3,
    status: 1,
    createTime: "2024-01-01 10:45:00",
    remark: "财务管理部门",
  },
]);

// 部门树形数据（用于选择上级部门）
const deptTreeData = ref([
  {
    id: "",
    deptName: "无上级部门",
    children: [],
  },
  ...tableData.value,
]);

const loading = ref(false);
const dialogVisible = ref(false);
const submitLoading = ref(false);
const formRef = ref<FormInstance>();
const isExpandAll = ref(true);

// 表单数据
const formData = reactive({
  id: null,
  deptName: "",
  deptCode: "",
  companyId: "",
  parentId: "",
  leader: "",
  phone: "",
  email: "",
  sort: 0,
  status: 1,
  remark: "",
});

// 表单验证规则
const formRules = {
  deptName: [{ required: true, message: "请输入部门名称", trigger: "blur" }],
  deptCode: [{ required: true, message: "请输入部门编码", trigger: "blur" }],
  companyId: [{ required: true, message: "请选择所属公司", trigger: "change" }],
  leader: [{ required: true, message: "请输入部门负责人", trigger: "blur" }],
  phone: [
    {
      pattern: /^1[3-9]\d{9}$/,
      message: "请输入正确的手机号码",
      trigger: "blur",
    },
  ],
  email: [{ type: "email", message: "请输入正确的邮箱格式", trigger: "blur" }],
};

// 对话框标题
const dialogTitle = computed(() => {
  return formData.id ? "编辑部门" : "添加部门";
});

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
    deptName: "",
    companyId: "",
    status: "",
  });
  handleSearch();
};

// 展开/收起全部
const handleExpandAll = () => {
  isExpandAll.value = !isExpandAll.value;
};

// 添加
const handleAdd = (row?: any) => {
  Object.assign(formData, {
    id: null,
    deptName: "",
    deptCode: "",
    companyId: row?.companyId || "",
    parentId: row?.id || "",
    leader: "",
    phone: "",
    email: "",
    sort: 0,
    status: 1,
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
  ElMessageBox.confirm(`确认${action}部门 "${row.deptName}" 吗？`, "提示", {
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
  ElMessageBox.confirm(`确认删除部门 "${row.deptName}" 吗？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      ElMessage.success("删除成功");
    })
    .catch(() => {});
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

onMounted(() => {
  handleSearch();
});
</script>

<style scoped>
.department-management {
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
</style>
