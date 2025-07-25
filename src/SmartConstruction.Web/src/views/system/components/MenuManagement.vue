<template>
  <div class="menu-management">
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>
        添加菜单
      </el-button>
      <el-button type="success" @click="handleExpandAll">
        <el-icon><Sort /></el-icon>
        {{ isExpandAll ? "收起全部" : "展开全部" }}
      </el-button>
    </div>

    <!-- 搜索区域 -->
    <div class="search-container">
      <el-form :model="searchForm" inline>
        <el-form-item label="菜单名称">
          <el-input
            v-model="searchForm.menuName"
            placeholder="请输入菜单名称"
            clearable
          />
        </el-form-item>
        <el-form-item label="菜单类型">
          <el-select
            v-model="searchForm.menuType"
            placeholder="请选择类型"
            clearable
          >
            <el-option label="目录" value="M" />
            <el-option label="菜单" value="C" />
            <el-option label="按钮" value="F" />
          </el-select>
        </el-form-item>
        <el-form-item label="状态">
          <el-select
            v-model="searchForm.status"
            placeholder="请选择状态"
            clearable
          >
            <el-option label="显示" value="0" />
            <el-option label="隐藏" value="1" />
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
      row-key="menuId"
      :default-expand-all="isExpandAll"
      :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
    >
      <el-table-column prop="menuName" label="菜单名称" width="200" />
      <el-table-column prop="icon" label="图标" width="80">
        <template #default="{ row }">
          <el-icon v-if="row.icon" :size="16">
            <component :is="row.icon" />
          </el-icon>
        </template>
      </el-table-column>
      <el-table-column prop="orderNum" label="排序" width="80" />
      <el-table-column prop="perms" label="权限标识" show-overflow-tooltip />
      <el-table-column
        prop="component"
        label="组件路径"
        show-overflow-tooltip
      />
      <el-table-column prop="menuType" label="类型" width="80">
        <template #default="{ row }">
          <el-tag :type="getMenuTypeColor(row.menuType)">
            {{ getMenuTypeName(row.menuType) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="visible" label="状态" width="80">
        <template #default="{ row }">
          <el-tag :type="row.visible === '0' ? 'success' : 'info'">
            {{ row.visible === "0" ? "显示" : "隐藏" }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="180" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button size="small" @click="handleAdd(row)"> 新增 </el-button>
          <el-button size="small" @click="handleEdit(row)"> 编辑 </el-button>
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
          <el-col :span="24">
            <el-form-item label="上级菜单" prop="parentId">
              <el-tree-select
                v-model="formData.parentId"
                :data="menuTreeData"
                :props="{
                  value: 'menuId',
                  label: 'menuName',
                  children: 'children',
                }"
                placeholder="选择上级菜单"
                style="width: 100%"
                check-strictly
                :render-after-expand="false"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="菜单类型" prop="menuType">
              <el-radio-group v-model="formData.menuType">
                <el-radio label="M">目录</el-radio>
                <el-radio label="C">菜单</el-radio>
                <el-radio label="F">按钮</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="菜单图标" v-if="formData.menuType !== 'F'">
              <el-select
                v-model="formData.icon"
                placeholder="选择图标"
                style="width: 100%"
              >
                <el-option label="首页" value="HomeFilled">
                  <el-icon><HomeFilled /></el-icon> HomeFilled
                </el-option>
                <el-option label="项目" value="Briefcase">
                  <el-icon><Briefcase /></el-icon> Briefcase
                </el-option>
                <el-option label="用户" value="User">
                  <el-icon><User /></el-icon> User
                </el-option>
                <el-option label="日历" value="Calendar">
                  <el-icon><Calendar /></el-icon> Calendar
                </el-option>
                <el-option label="监控" value="VideoCamera">
                  <el-icon><VideoCamera /></el-icon> VideoCamera
                </el-option>
                <el-option label="设置" value="Setting">
                  <el-icon><Setting /></el-icon> Setting
                </el-option>
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="菜单名称" prop="menuName">
              <el-input
                v-model="formData.menuName"
                placeholder="请输入菜单名称"
              />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="显示排序" prop="orderNum">
              <el-input-number
                v-model="formData.orderNum"
                :min="0"
                :max="999"
                style="width: 100%"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20" v-if="formData.menuType !== 'F'">
          <el-col :span="12">
            <el-form-item label="路由地址" prop="path">
              <el-input v-model="formData.path" placeholder="请输入路由地址" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="formData.menuType === 'C'">
            <el-form-item label="组件路径" prop="component">
              <el-input
                v-model="formData.component"
                placeholder="请输入组件路径"
              />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="权限标识" prop="perms">
              <el-input v-model="formData.perms" placeholder="请输入权限标识" />
            </el-form-item>
          </el-col>
          <el-col :span="12" v-if="formData.menuType !== 'F'">
            <el-form-item label="路由参数">
              <el-input v-model="formData.query" placeholder="请输入路由参数" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20" v-if="formData.menuType !== 'F'">
          <el-col :span="12">
            <el-form-item label="是否外链">
              <el-radio-group v-model="formData.isFrame">
                <el-radio label="0">是</el-radio>
                <el-radio label="1">否</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="是否缓存">
              <el-radio-group v-model="formData.isCache">
                <el-radio label="0">缓存</el-radio>
                <el-radio label="1">不缓存</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="菜单状态">
              <el-radio-group v-model="formData.visible">
                <el-radio label="0">显示</el-radio>
                <el-radio label="1">隐藏</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="菜单状态">
              <el-radio-group v-model="formData.status">
                <el-radio label="0">正常</el-radio>
                <el-radio label="1">停用</el-radio>
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
  menuName: "",
  menuType: "",
  status: "",
});

// 表格数据
const tableData = ref([
  {
    menuId: 1,
    menuName: "系统管理",
    parentId: 0,
    orderNum: 1,
    path: "/system",
    component: "",
    query: "",
    isFrame: "1",
    isCache: "0",
    menuType: "M",
    visible: "0",
    status: "0",
    perms: "",
    icon: "Setting",
    createTime: "2024-01-01 10:00:00",
    remark: "系统管理目录",
    children: [
      {
        menuId: 100,
        menuName: "用户管理",
        parentId: 1,
        orderNum: 1,
        path: "user",
        component: "system/user/index",
        query: "",
        isFrame: "1",
        isCache: "0",
        menuType: "C",
        visible: "0",
        status: "0",
        perms: "system:user:list",
        icon: "User",
        createTime: "2024-01-01 10:00:00",
        remark: "用户管理菜单",
        children: [
          {
            menuId: 1001,
            menuName: "用户查询",
            parentId: 100,
            orderNum: 1,
            path: "",
            component: "",
            query: "",
            isFrame: "1",
            isCache: "0",
            menuType: "F",
            visible: "0",
            status: "0",
            perms: "system:user:query",
            icon: "",
            createTime: "2024-01-01 10:00:00",
            remark: "",
          },
          {
            menuId: 1002,
            menuName: "用户新增",
            parentId: 100,
            orderNum: 2,
            path: "",
            component: "",
            query: "",
            isFrame: "1",
            isCache: "0",
            menuType: "F",
            visible: "0",
            status: "0",
            perms: "system:user:add",
            icon: "",
            createTime: "2024-01-01 10:00:00",
            remark: "",
          },
          {
            menuId: 1003,
            menuName: "用户修改",
            parentId: 100,
            orderNum: 3,
            path: "",
            component: "",
            query: "",
            isFrame: "1",
            isCache: "0",
            menuType: "F",
            visible: "0",
            status: "0",
            perms: "system:user:edit",
            icon: "",
            createTime: "2024-01-01 10:00:00",
            remark: "",
          },
          {
            menuId: 1004,
            menuName: "用户删除",
            parentId: 100,
            orderNum: 4,
            path: "",
            component: "",
            query: "",
            isFrame: "1",
            isCache: "0",
            menuType: "F",
            visible: "0",
            status: "0",
            perms: "system:user:remove",
            icon: "",
            createTime: "2024-01-01 10:00:00",
            remark: "",
          },
        ],
      },
      {
        menuId: 101,
        menuName: "角色管理",
        parentId: 1,
        orderNum: 2,
        path: "role",
        component: "system/role/index",
        query: "",
        isFrame: "1",
        isCache: "0",
        menuType: "C",
        visible: "0",
        status: "0",
        perms: "system:role:list",
        icon: "UserFilled",
        createTime: "2024-01-01 10:00:00",
        remark: "角色管理菜单",
      },
      {
        menuId: 102,
        menuName: "菜单管理",
        parentId: 1,
        orderNum: 3,
        path: "menu",
        component: "system/menu/index",
        query: "",
        isFrame: "1",
        isCache: "0",
        menuType: "C",
        visible: "0",
        status: "0",
        perms: "system:menu:list",
        icon: "Menu",
        createTime: "2024-01-01 10:00:00",
        remark: "菜单管理菜单",
      },
    ],
  },
  {
    menuId: 2,
    menuName: "项目管理",
    parentId: 0,
    orderNum: 2,
    path: "/project",
    component: "",
    query: "",
    isFrame: "1",
    isCache: "0",
    menuType: "M",
    visible: "0",
    status: "0",
    perms: "",
    icon: "Briefcase",
    createTime: "2024-01-01 10:00:00",
    remark: "项目管理目录",
    children: [
      {
        menuId: 200,
        menuName: "项目列表",
        parentId: 2,
        orderNum: 1,
        path: "list",
        component: "project/list/index",
        query: "",
        isFrame: "1",
        isCache: "0",
        menuType: "C",
        visible: "0",
        status: "0",
        perms: "project:list:view",
        icon: "List",
        createTime: "2024-01-01 10:00:00",
        remark: "项目列表菜单",
      },
    ],
  },
]);

// 菜单树数据（用于选择上级菜单）
const menuTreeData = ref([
  {
    menuId: 0,
    menuName: "主类目",
    children: tableData.value,
  },
]);

const loading = ref(false);
const dialogVisible = ref(false);
const submitLoading = ref(false);
const formRef = ref<FormInstance>();
const isExpandAll = ref(true);

// 表单数据
const formData = reactive({
  menuId: null,
  parentId: 0,
  menuName: "",
  orderNum: 0,
  path: "",
  component: "",
  query: "",
  isFrame: "1",
  isCache: "0",
  menuType: "M",
  visible: "0",
  status: "0",
  perms: "",
  icon: "",
  remark: "",
});

// 表单验证规则
const formRules = {
  menuName: [{ required: true, message: "请输入菜单名称", trigger: "blur" }],
  orderNum: [{ required: true, message: "请输入显示排序", trigger: "blur" }],
  path: [{ required: true, message: "请输入路由地址", trigger: "blur" }],
};

// 对话框标题
const dialogTitle = computed(() => {
  return formData.menuId ? "编辑菜单" : "添加菜单";
});

// 获取菜单类型名称
const getMenuTypeName = (type: string) => {
  const typeMap: Record<string, string> = {
    M: "目录",
    C: "菜单",
    F: "按钮",
  };
  return typeMap[type] || "未知";
};

// 获取菜单类型颜色
const getMenuTypeColor = (type: string) => {
  const colorMap: Record<string, string> = {
    M: "warning",
    C: "primary",
    F: "success",
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
    menuName: "",
    menuType: "",
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
    menuId: null,
    parentId: row?.menuId || 0,
    menuName: "",
    orderNum: 0,
    path: "",
    component: "",
    query: "",
    isFrame: "1",
    isCache: "0",
    menuType: "M",
    visible: "0",
    status: "0",
    perms: "",
    icon: "",
    remark: "",
  });
  dialogVisible.value = true;
};

// 编辑
const handleEdit = (row: any) => {
  Object.assign(formData, { ...row });
  dialogVisible.value = true;
};

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确认删除菜单 "${row.menuName}" 吗？`, "警告", {
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
        ElMessage.success(formData.menuId ? "编辑成功" : "添加成功");
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
.menu-management {
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
