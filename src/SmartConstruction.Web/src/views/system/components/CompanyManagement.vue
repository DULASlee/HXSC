<template>
  <div class="company-management">
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加公司
      </el-button>
      <el-button type="success" @click="handleExport">
        <el-icon><Download /></el-icon>
        导出数据
      </el-button>
    </div>

    <!-- 搜索区域 -->
    <div class="search-container">
      <el-form :model="searchForm" inline>
        <el-form-item label="公司名称">
          <el-input
            v-model="searchForm.companyName"
            placeholder="请输入公司名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="所属租户">
          <el-select
            v-model="searchForm.tenantId"
            placeholder="请选择租户"
            clearable
          >
            <el-option label="华建集团" value="1" />
            <el-option label="中建科技" value="2" />
            <el-option label="绿城建设" value="3" />
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
      default-expand-all
    >
      <el-table-column prop="companyName" label="公司名称" />
      <el-table-column prop="tenantName" label="所属租户" />
      <el-table-column prop="companyCode" label="公司编码" />
      <el-table-column prop="legalPerson" label="法人代表" />
      <el-table-column prop="contactPhone" label="联系电话" />
      <el-table-column prop="businessLicense" label="营业执照号" />
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
      width="800px"
      @close="handleDialogClose"
    >
      <el-form
        ref="formRef"
        :model="formData"
        :rules="formRules"
        label-width="120px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="公司名称" prop="companyName">
              <el-input
                v-model="formData.companyName"
                placeholder="请输入公司名称"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="公司编码" prop="companyCode">
              <el-input
                v-model="formData.companyCode"
                placeholder="请输入公司编码"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="所属租户" prop="tenantId">
              <el-select
                v-model="formData.tenantId"
                placeholder="请选择租户"
                style="width: 100%"
              >
                <el-option label="华建集团" value="1" />
                <el-option label="中建科技" value="2" />
                <el-option label="绿城建设" value="3" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="上级公司" prop="parentId">
              <el-select
                v-model="formData.parentId"
                placeholder="请选择上级公司"
                style="width: 100%"
              >
                <el-option label="无" value="" />
                <el-option label="华建集团总部" value="1" />
                <el-option label="中建科技总部" value="2" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="法人代表" prop="legalPerson">
              <el-input
                v-model="formData.legalPerson"
                placeholder="请输入法人代表"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="联系电话" prop="contactPhone">
              <el-input
                v-model="formData.contactPhone"
                placeholder="请输入联系电话"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="营业执照号" prop="businessLicense">
              <el-input
                v-model="formData.businessLicense"
                placeholder="请输入营业执照号"
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
        <el-form-item label="注册地址" prop="address">
          <el-input
            v-model="formData.address"
            type="textarea"
            :rows="2"
            placeholder="请输入注册地址"
          />
        </el-form-item>
        <el-form-item label="经营范围">
          <el-input
            v-model="formData.businessScope"
            type="textarea"
            :rows="3"
            placeholder="请输入经营范围"
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
  companyName: "",
  tenantId: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    id: 1,
    companyName: "华建集团总部",
    tenantName: "华建集团",
    tenantId: "1",
    companyCode: "HJ001",
    legalPerson: "张建华",
    contactPhone: "010-88888888",
    businessLicense: "91110000123456789X",
    address: "北京市朝阳区建国路88号",
    businessScope: "建筑工程施工、设计、监理",
    status: 1,
    createTime: "2024-01-01 10:00:00",
    remark: "集团总部",
    children: [
      {
        id: 11,
        companyName: "华建北京分公司",
        tenantName: "华建集团",
        tenantId: "1",
        companyCode: "HJ011",
        legalPerson: "李明",
        contactPhone: "010-66666666",
        businessLicense: "91110000123456780X",
        address: "北京市海淀区中关村大街100号",
        businessScope: "建筑工程施工",
        status: 1,
        createTime: "2024-01-02 10:00:00",
        remark: "北京分公司",
      },
    ],
  },
  {
    id: 2,
    companyName: "中建科技总部",
    tenantName: "中建科技",
    tenantId: "2",
    companyCode: "ZJ001",
    legalPerson: "王科技",
    contactPhone: "021-88888888",
    businessLicense: "91310000123456789X",
    address: "上海市浦东新区世纪大道100号",
    businessScope: "智慧建造技术开发、咨询服务",
    status: 1,
    createTime: "2024-01-02 14:30:00",
    remark: "科技总部",
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
  companyName: "",
  companyCode: "",
  tenantId: "",
  parentId: "",
  legalPerson: "",
  contactPhone: "",
  businessLicense: "",
  address: "",
  businessScope: "",
  status: 1,
  remark: "",
});

// 表单验证规则
const formRules = {
  companyName: [{ required: true, message: "请输入公司名称", trigger: "blur" }],
  companyCode: [{ required: true, message: "请输入公司编码", trigger: "blur" }],
  tenantId: [{ required: true, message: "请选择所属租户", trigger: "change" }],
  legalPerson: [{ required: true, message: "请输入法人代表", trigger: "blur" }],
  contactPhone: [
    { required: true, message: "请输入联系电话", trigger: "blur" },
  ],
  businessLicense: [
    { required: true, message: "请输入营业执照号", trigger: "blur" },
  ],
};

// 对话框标题
const dialogTitle = computed(() => {
  return formData.id ? "编辑公司" : "添加公司";
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
    companyName: "",
    tenantId: "",
    status: "",
  });
  handleSearch();
};

// 添加
const handleAdd = () => {
  Object.assign(formData, {
    id: null,
    companyName: "",
    companyCode: "",
    tenantId: "",
    parentId: "",
    legalPerson: "",
    contactPhone: "",
    businessLicense: "",
    address: "",
    businessScope: "",
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
  ElMessageBox.confirm(`确认${action}公司 "${row.companyName}" 吗？`, "提示", {
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
  ElMessageBox.confirm(`确认删除公司 "${row.companyName}" 吗？`, "警告", {
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
.company-management {
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
