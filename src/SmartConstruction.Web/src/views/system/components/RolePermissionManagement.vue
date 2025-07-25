<template>
  <div class="role-permission-management">
    <el-row :gutter="20">
      <!-- 左侧角色列表 -->
      <el-col :span="8">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>角色列表</span>
              <el-button
                type="primary"
                size="small"
                @click="handleRefreshRoles"
              >
                <el-icon><Refresh /></el-icon>
                刷新
              </el-button>
            </div>
          </template>

          <div class="role-search">
            <el-input
              v-model="roleSearchText"
              placeholder="搜索角色"
              clearable
              @input="handleRoleSearch"
            >
              <template #prefix>
                <el-icon><Search /></el-icon>
              </template>
            </el-input>
          </div>

          <div class="role-list">
            <div
              v-for="role in filteredRoles"
              :key="role.id"
              :class="['role-item', { active: selectedRole?.id === role.id }]"
              @click="handleSelectRole(role)"
            >
              <div class="role-info">
                <div class="role-name">{{ role.roleName }}</div>
                <div class="role-code">{{ role.roleCode }}</div>
              </div>
              <el-tag :type="getRoleTypeColor(role.roleType)" size="small">
                {{ getRoleTypeName(role.roleType) }}
              </el-tag>
            </div>
          </div>
        </el-card>
      </el-col>

      <!-- 右侧权限配置 -->
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>权限配置</span>
              <div v-if="selectedRole">
                <el-button
                  type="success"
                  @click="handleSavePermission"
                  :loading="saveLoading"
                >
                  <el-icon><Check /></el-icon>
                  保存权限
                </el-button>
                <el-button @click="handleResetPermission">
                  <el-icon><RefreshLeft /></el-icon>
                  重置
                </el-button>
              </div>
            </div>
          </template>

          <div v-if="!selectedRole" class="no-selection">
            <el-empty description="请选择一个角色进行权限配置" />
          </div>

          <div v-else class="permission-config">
            <!-- 角色信息 -->
            <div class="role-info-panel">
              <el-descriptions :column="2" border>
                <el-descriptions-item label="角色名称">{{
                  selectedRole.roleName
                }}</el-descriptions-item>
                <el-descriptions-item label="角色编码">{{
                  selectedRole.roleCode
                }}</el-descriptions-item>
                <el-descriptions-item label="角色类型">
                  <el-tag :type="getRoleTypeColor(selectedRole.roleType)">
                    {{ getRoleTypeName(selectedRole.roleType) }}
                  </el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="数据权限">
                  <el-tag :type="getDataScopeColor(selectedRole.dataScope)">
                    {{ getDataScopeName(selectedRole.dataScope) }}
                  </el-tag>
                </el-descriptions-item>
              </el-descriptions>
            </div>

            <!-- 权限配置标签页 -->
            <el-tabs v-model="activeTab" type="border-card">
              <!-- 菜单权限 -->
              <el-tab-pane label="菜单权限" name="menu">
                <div class="permission-section">
                  <div class="section-header">
                    <h4>菜单权限配置</h4>
                    <div class="tree-actions">
                      <el-button size="small" @click="handleExpandAll">
                        {{ isMenuExpanded ? "收起全部" : "展开全部" }}
                      </el-button>
                      <el-button size="small" @click="handleCheckAll">
                        {{ isAllChecked ? "取消全选" : "全选" }}
                      </el-button>
                    </div>
                  </div>
                  <el-tree
                    ref="menuTreeRef"
                    :data="menuTreeData"
                    :props="{ children: 'children', label: 'menuName' }"
                    show-checkbox
                    node-key="menuId"
                    :default-checked-keys="checkedMenuKeys"
                    :default-expand-all="isMenuExpanded"
                    @check="handleMenuCheck"
                  >
                    <template #default="{ node, data }">
                      <span class="tree-node">
                        <el-icon v-if="data.icon" class="node-icon">
                          <component :is="data.icon" />
                        </el-icon>
                        <span class="node-label">{{ data.menuName }}</span>
                        <el-tag
                          :type="getMenuTypeColor(data.menuType)"
                          size="small"
                          class="node-tag"
                        >
                          {{ getMenuTypeName(data.menuType) }}
                        </el-tag>
                      </span>
                    </template>
                  </el-tree>
                </div>
              </el-tab-pane>

              <!-- 数据权限 -->
              <el-tab-pane label="数据权限" name="data">
                <div class="permission-section">
                  <div class="section-header">
                    <h4>数据权限配置</h4>
                  </div>

                  <el-form :model="dataPermissionForm" label-width="120px">
                    <el-form-item label="数据范围">
                      <el-radio-group v-model="dataPermissionForm.dataScope">
                        <el-radio :label="1">全部数据权限</el-radio>
                        <el-radio :label="2">自定义数据权限</el-radio>
                        <el-radio :label="3">本部门数据权限</el-radio>
                        <el-radio :label="4">本部门及以下数据权限</el-radio>
                        <el-radio :label="5">仅本人数据权限</el-radio>
                      </el-radio-group>
                    </el-form-item>

                    <el-form-item
                      label="部门权限"
                      v-if="dataPermissionForm.dataScope === 2"
                    >
                      <el-tree
                        ref="deptTreeRef"
                        :data="deptTreeData"
                        :props="{ children: 'children', label: 'deptName' }"
                        show-checkbox
                        node-key="id"
                        :default-checked-keys="checkedDeptKeys"
                        :default-expand-all="true"
                      />
                    </el-form-item>
                  </el-form>
                </div>
              </el-tab-pane>

              <!-- 操作日志 -->
              <el-tab-pane label="操作日志" name="log">
                <div class="permission-section">
                  <div class="section-header">
                    <h4>权限变更日志</h4>
                  </div>

                  <el-table :data="permissionLogs" style="width: 100%">
                    <el-table-column
                      prop="operateTime"
                      label="操作时间"
                      width="180"
                    />
                    <el-table-column prop="operator" label="操作人" />
                    <el-table-column prop="operateType" label="操作类型">
                      <template #default="{ row }">
                        <el-tag
                          :type="
                            row.operateType === '授权' ? 'success' : 'warning'
                          "
                        >
                          {{ row.operateType }}
                        </el-tag>
                      </template>
                    </el-table-column>
                    <el-table-column
                      prop="description"
                      label="操作描述"
                      show-overflow-tooltip
                    />
                  </el-table>
                </div>
              </el-tab-pane>
            </el-tabs>
          </div>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { ElMessage } from "element-plus";

// 角色数据
const roles = ref([
  {
    id: 1,
    roleCode: "ADMIN",
    roleName: "超级管理员",
    roleType: 1,
    dataScope: 1,
  },
  {
    id: 2,
    roleCode: "PROJECT_MANAGER",
    roleName: "项目经理",
    roleType: 2,
    dataScope: 3,
  },
  {
    id: 3,
    roleCode: "ENGINEER",
    roleName: "工程师",
    roleType: 2,
    dataScope: 4,
  },
  {
    id: 4,
    roleCode: "WORKER",
    roleName: "普通工人",
    roleType: 2,
    dataScope: 5,
  },
  {
    id: 5,
    roleCode: "FINANCE",
    roleName: "财务人员",
    roleType: 2,
    dataScope: 3,
  },
]);

// 菜单树数据
const menuTreeData = ref([
  {
    menuId: 1,
    menuName: "首页",
    menuType: "C",
    icon: "HomeFilled",
    children: [],
  },
  {
    menuId: 2,
    menuName: "项目管理",
    menuType: "M",
    icon: "Briefcase",
    children: [
      { menuId: 21, menuName: "项目列表", menuType: "C", icon: "List" },
      { menuId: 22, menuName: "项目新增", menuType: "F" },
      { menuId: 23, menuName: "项目编辑", menuType: "F" },
      { menuId: 24, menuName: "项目删除", menuType: "F" },
    ],
  },
  {
    menuId: 3,
    menuName: "工人管理",
    menuType: "M",
    icon: "User",
    children: [
      { menuId: 31, menuName: "工人列表", menuType: "C", icon: "List" },
      { menuId: 32, menuName: "工人新增", menuType: "F" },
      { menuId: 33, menuName: "工人编辑", menuType: "F" },
      { menuId: 34, menuName: "工人删除", menuType: "F" },
    ],
  },
  {
    menuId: 4,
    menuName: "考勤管理",
    menuType: "M",
    icon: "Calendar",
    children: [
      { menuId: 41, menuName: "考勤记录", menuType: "C", icon: "List" },
      { menuId: 42, menuName: "考勤统计", menuType: "C", icon: "DataAnalysis" },
      { menuId: 43, menuName: "考勤导出", menuType: "F" },
    ],
  },
  {
    menuId: 5,
    menuName: "监控管理",
    menuType: "M",
    icon: "VideoCamera",
    children: [
      { menuId: 51, menuName: "设备监控", menuType: "C", icon: "Monitor" },
      { menuId: 52, menuName: "设备管理", menuType: "C", icon: "Setting" },
      { menuId: 53, menuName: "监控配置", menuType: "F" },
    ],
  },
  {
    menuId: 6,
    menuName: "系统管理",
    menuType: "M",
    icon: "Setting",
    children: [
      { menuId: 61, menuName: "用户管理", menuType: "C", icon: "User" },
      { menuId: 62, menuName: "角色管理", menuType: "C", icon: "UserFilled" },
      { menuId: 63, menuName: "菜单管理", menuType: "C", icon: "Menu" },
      { menuId: 64, menuName: "权限配置", menuType: "C", icon: "Key" },
    ],
  },
]);

// 部门树数据
const deptTreeData = ref([
  {
    id: 1,
    deptName: "总经理办公室",
    children: [{ id: 11, deptName: "秘书处" }],
  },
  {
    id: 2,
    deptName: "工程部",
    children: [
      { id: 21, deptName: "设计科" },
      { id: 22, deptName: "施工科" },
    ],
  },
  {
    id: 3,
    deptName: "财务部",
    children: [],
  },
]);

// 权限变更日志
const permissionLogs = ref([
  {
    operateTime: "2024-01-16 10:30:00",
    operator: "系统管理员",
    operateType: "授权",
    description: "为项目经理角色添加了项目管理模块的所有权限",
  },
  {
    operateTime: "2024-01-15 14:20:00",
    operator: "系统管理员",
    operateType: "撤权",
    description: "撤销了工程师角色的用户管理权限",
  },
  {
    operateTime: "2024-01-14 09:15:00",
    operator: "系统管理员",
    operateType: "授权",
    description: "为财务人员角色配置了本部门数据权限",
  },
]);

const roleSearchText = ref("");
const selectedRole = ref<any>(null);
const activeTab = ref("menu");
const saveLoading = ref(false);
const isMenuExpanded = ref(true);
const isAllChecked = ref(false);

const menuTreeRef = ref();
const deptTreeRef = ref();

const checkedMenuKeys = ref<number[]>([]);
const checkedDeptKeys = ref<number[]>([]);

// 数据权限表单
const dataPermissionForm = reactive({
  dataScope: 1,
});

// 过滤后的角色列表
const filteredRoles = computed(() => {
  if (!roleSearchText.value) {
    return roles.value;
  }
  return roles.value.filter(
    (role) =>
      role.roleName.includes(roleSearchText.value) ||
      role.roleCode.includes(roleSearchText.value),
  );
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

// 角色搜索
const handleRoleSearch = () => {
  // 搜索逻辑已在computed中处理
};

// 刷新角色列表
const handleRefreshRoles = () => {
  ElMessage.success("角色列表已刷新");
};

// 选择角色
const handleSelectRole = (role: any) => {
  selectedRole.value = role;

  // 模拟加载角色权限
  if (role.roleCode === "ADMIN") {
    checkedMenuKeys.value = [
      1, 2, 21, 22, 23, 24, 3, 31, 32, 33, 34, 4, 41, 42, 43, 5, 51, 52, 53, 6,
      61, 62, 63, 64,
    ];
    checkedDeptKeys.value = [1, 11, 2, 21, 22, 3];
    dataPermissionForm.dataScope = 1;
  } else if (role.roleCode === "PROJECT_MANAGER") {
    checkedMenuKeys.value = [1, 2, 21, 22, 23, 3, 31, 32, 4, 41, 42, 5, 51, 52];
    checkedDeptKeys.value = [2, 21, 22];
    dataPermissionForm.dataScope = 3;
  } else {
    checkedMenuKeys.value = [1, 2, 21, 3, 31];
    checkedDeptKeys.value = [];
    dataPermissionForm.dataScope = 5;
  }
};

// 展开/收起全部菜单
const handleExpandAll = () => {
  isMenuExpanded.value = !isMenuExpanded.value;
};

// 全选/取消全选
const handleCheckAll = () => {
  if (isAllChecked.value) {
    menuTreeRef.value.setCheckedKeys([]);
  } else {
    const allKeys = getAllMenuKeys(menuTreeData.value);
    menuTreeRef.value.setCheckedKeys(allKeys);
  }
  isAllChecked.value = !isAllChecked.value;
};

// 获取所有菜单键
const getAllMenuKeys = (menus: any[]): number[] => {
  let keys: number[] = [];
  menus.forEach((menu) => {
    keys.push(menu.menuId);
    if (menu.children && menu.children.length > 0) {
      keys = keys.concat(getAllMenuKeys(menu.children));
    }
  });
  return keys;
};

// 菜单选择变化
const handleMenuCheck = () => {
  const checkedKeys = menuTreeRef.value.getCheckedKeys();
  isAllChecked.value =
    checkedKeys.length === getAllMenuKeys(menuTreeData.value).length;
};

// 保存权限
const handleSavePermission = () => {
  if (!selectedRole.value) return;

  saveLoading.value = true;

  const checkedMenuKeys = menuTreeRef.value.getCheckedKeys();
  const halfCheckedMenuKeys = menuTreeRef.value.getHalfCheckedKeys();
  const checkedDeptKeys = deptTreeRef.value?.getCheckedKeys() || [];

  setTimeout(() => {
    saveLoading.value = false;
    ElMessage.success("权限配置保存成功");

    // 添加操作日志
    permissionLogs.value.unshift({
      operateTime: new Date().toLocaleString(),
      operator: "当前用户",
      operateType: "授权",
      description: `为${selectedRole.value.roleName}角色配置了权限`,
    });

    console.log("保存的权限配置:", {
      roleId: selectedRole.value.id,
      menuPermissions: [...checkedMenuKeys, ...halfCheckedMenuKeys],
      dataScope: dataPermissionForm.dataScope,
      deptPermissions: checkedDeptKeys,
    });
  }, 1000);
};

// 重置权限
const handleResetPermission = () => {
  if (selectedRole.value) {
    handleSelectRole(selectedRole.value);
    ElMessage.info("权限配置已重置");
  }
};

onMounted(() => {
  // 默认选择第一个角色
  if (roles.value.length > 0) {
    handleSelectRole(roles.value[0]);
  }
});
</script>

<style scoped>
.role-permission-management {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.role-search {
  margin-bottom: 15px;
}

.role-list {
  max-height: 600px;
  overflow-y: auto;
}

.role-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  margin-bottom: 8px;
  border: 1px solid var(--app-border-color);
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.3s;
}

.role-item:hover {
  background-color: var(--el-color-primary-light-9);
  border-color: var(--el-color-primary);
}

.role-item.active {
  background-color: var(--el-color-primary-light-8);
  border-color: var(--el-color-primary);
}

.role-info {
  flex: 1;
}

.role-name {
  font-weight: 500;
  color: var(--app-text-color);
  margin-bottom: 4px;
}

.role-code {
  font-size: 12px;
  color: var(--app-text-color-regular);
}

.no-selection {
  padding: 60px 0;
  text-align: center;
}

.permission-config {
  padding: 0;
}

.role-info-panel {
  margin-bottom: 20px;
}

.permission-section {
  padding: 20px 0;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.section-header h4 {
  margin: 0;
  color: var(--app-text-color);
  font-size: 16px;
  font-weight: 500;
}

.tree-actions {
  display: flex;
  gap: 8px;
}

.tree-node {
  display: flex;
  align-items: center;
  gap: 8px;
  flex: 1;
}

.node-icon {
  font-size: 14px;
}

.node-label {
  flex: 1;
}

.node-tag {
  margin-left: auto;
}

:deep(.el-tree-node__content) {
  height: 32px;
}
</style>
