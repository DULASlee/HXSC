<template>
  <div class="page-generation">
    <el-row :gutter="20">
      <!-- 左侧模块选择 -->
      <el-col :span="8">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>选择模块</span>
              <el-button
                type="primary"
                size="small"
                @click="handleRefreshModules"
              >
                <el-icon><Refresh /></el-icon>
                刷新
              </el-button>
            </div>
          </template>

          <div class="module-search">
            <el-input
              v-model="moduleSearchText"
              placeholder="搜索模块"
              clearable
            >
              <template #prefix>
                <el-icon><Search /></el-icon>
              </template>
            </el-input>
          </div>

          <div class="module-list">
            <div
              v-for="module in filteredModules"
              :key="module.id"
              :class="[
                'module-item',
                { active: selectedModule?.id === module.id },
              ]"
              @click="handleSelectModule(module)"
            >
              <div class="module-info">
                <div class="module-name">{{ module.moduleName }}</div>
                <div class="module-code">{{ module.moduleCode }}</div>
              </div>
              <el-tag
                :type="getModuleTypeColor(module.moduleType)"
                size="small"
              >
                {{ getModuleTypeName(module.moduleType) }}
              </el-tag>
            </div>
          </div>
        </el-card>
      </el-col>

      <!-- 右侧生成配置 -->
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>页面生成配置</span>
              <div v-if="selectedModule">
                <el-button
                  type="success"
                  @click="handleGenerateAll"
                  :loading="generateLoading"
                >
                  <el-icon><Magic /></el-icon>
                  一键生成
                </el-button>
              </div>
            </div>
          </template>

          <div v-if="!selectedModule" class="no-selection">
            <el-empty description="请选择一个模块进行页面生成配置" />
          </div>

          <div v-else class="generation-config">
            <!-- 模块信息 -->
            <div class="module-info-panel">
              <el-descriptions :column="2" border>
                <el-descriptions-item label="模块名称">{{
                  selectedModule.moduleName
                }}</el-descriptions-item>
                <el-descriptions-item label="模块编码">{{
                  selectedModule.moduleCode
                }}</el-descriptions-item>
                <el-descriptions-item label="数据表名">{{
                  selectedModule.tableName
                }}</el-descriptions-item>
                <el-descriptions-item label="实体类名">{{
                  selectedModule.entityName
                }}</el-descriptions-item>
              </el-descriptions>
            </div>

            <!-- 生成配置 -->
            <el-tabs v-model="activeTab" type="border-card">
              <!-- 页面生成配置 -->
              <el-tab-pane label="页面生成配置" name="page">
                <div class="config-section">
                  <h4>生成选项</h4>
                  <el-checkbox-group v-model="generateConfig.options">
                    <el-row :gutter="20">
                      <el-col :span="8">
                        <el-checkbox label="entity">实体类</el-checkbox>
                      </el-col>
                      <el-col :span="8">
                        <el-checkbox label="mapper">Mapper接口</el-checkbox>
                      </el-col>
                      <el-col :span="8">
                        <el-checkbox label="service">Service层</el-checkbox>
                      </el-col>
                    </el-row>
                    <el-row :gutter="20">
                      <el-col :span="8">
                        <el-checkbox label="controller"
                          >Controller层</el-checkbox
                        >
                      </el-col>
                      <el-col :span="8">
                        <el-checkbox label="vue">Vue页面</el-checkbox>
                      </el-col>
                      <el-col :span="8">
                        <el-checkbox label="sql">SQL脚本</el-checkbox>
                      </el-col>
                    </el-row>
                  </el-checkbox-group>
                </div>

                <div class="config-section">
                  <h4>前端框架</h4>
                  <el-radio-group v-model="generateConfig.frontendType">
                    <el-radio label="vue3">Vue3 + Element Plus</el-radio>
                    <el-radio label="react">React + Ant Design</el-radio>
                    <el-radio label="angular">Angular + NG-ZORRO</el-radio>
                  </el-radio-group>
                </div>

                <div class="config-section">
                  <h4>后端框架</h4>
                  <el-radio-group v-model="generateConfig.backendType">
                    <el-radio label="springboot">Spring Boot</el-radio>
                    <el-radio label="springcloud">Spring Cloud</el-radio>
                    <el-radio label="nodejs">Node.js</el-radio>
                  </el-radio-group>
                </div>

                <div class="config-section">
                  <h4>数据库类型</h4>
                  <el-radio-group v-model="generateConfig.databaseType">
                    <el-radio label="mysql">MySQL</el-radio>
                    <el-radio label="postgresql">PostgreSQL</el-radio>
                    <el-radio label="oracle">Oracle</el-radio>
                    <el-radio label="sqlserver">SQL Server</el-radio>
                  </el-radio-group>
                </div>
              </el-tab-pane>

              <!-- 数据库迁移 -->
              <el-tab-pane label="数据库迁移" name="migration">
                <div class="config-section">
                  <h4>数据库连接配置</h4>
                  <el-form :model="dbConfig" label-width="120px">
                    <el-row :gutter="20">
                      <el-col :span="12">
                        <el-form-item label="数据库类型">
                          <el-select
                            v-model="dbConfig.type"
                            style="width: 100%"
                          >
                            <el-option label="MySQL" value="mysql" />
                            <el-option label="PostgreSQL" value="postgresql" />
                            <el-option label="Oracle" value="oracle" />
                          </el-select>
                        </el-form-item>
                      </el-col>
                      <el-col :span="12">
                        <el-form-item label="主机地址">
                          <el-input
                            v-model="dbConfig.host"
                            placeholder="localhost"
                          />
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row :gutter="20">
                      <el-col :span="12">
                        <el-form-item label="端口">
                          <el-input
                            v-model="dbConfig.port"
                            placeholder="3306"
                          />
                        </el-form-item>
                      </el-col>
                      <el-col :span="12">
                        <el-form-item label="数据库名">
                          <el-input
                            v-model="dbConfig.database"
                            placeholder="database_name"
                          />
                        </el-form-item>
                      </el-col>
                    </el-row>
                    <el-row :gutter="20">
                      <el-col :span="12">
                        <el-form-item label="用户名">
                          <el-input
                            v-model="dbConfig.username"
                            placeholder="username"
                          />
                        </el-form-item>
                      </el-col>
                      <el-col :span="12">
                        <el-form-item label="密码">
                          <el-input
                            v-model="dbConfig.password"
                            type="password"
                            placeholder="password"
                          />
                        </el-form-item>
                      </el-col>
                    </el-row>
                  </el-form>

                  <div class="migration-actions">
                    <el-button
                      type="primary"
                      @click="handleTestConnection"
                      :loading="testLoading"
                    >
                      <el-icon><Link /></el-icon>
                      测试连接
                    </el-button>
                    <el-button
                      type="success"
                      @click="handleExecuteMigration"
                      :loading="migrationLoading"
                    >
                      <el-icon><Upload /></el-icon>
                      执行迁移
                    </el-button>
                  </div>
                </div>
              </el-tab-pane>

              <!-- 生成历史 -->
              <el-tab-pane label="生成历史" name="history">
                <div class="config-section">
                  <h4>生成记录</h4>
                  <el-table :data="generationHistory" style="width: 100%">
                    <el-table-column
                      prop="generateTime"
                      label="生成时间"
                      width="180"
                    />
                    <el-table-column prop="generateType" label="生成类型">
                      <template #default="{ row }">
                        <el-tag>{{ row.generateType }}</el-tag>
                      </template>
                    </el-table-column>
                    <el-table-column prop="status" label="状态">
                      <template #default="{ row }">
                        <el-tag
                          :type="
                            row.status === 'success' ? 'success' : 'danger'
                          "
                        >
                          {{ row.status === "success" ? "成功" : "失败" }}
                        </el-tag>
                      </template>
                    </el-table-column>
                    <el-table-column
                      prop="description"
                      label="描述"
                      show-overflow-tooltip
                    />
                    <el-table-column label="操作" width="120">
                      <template #default="{ row }">
                        <el-button size="small" @click="handleViewLog(row)">
                          查看日志
                        </el-button>
                      </template>
                    </el-table-column>
                  </el-table>
                </div>
              </el-tab-pane>
            </el-tabs>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 生成进度对话框 -->
    <el-dialog
      v-model="progressVisible"
      title="代码生成进度"
      width="600px"
      :close-on-click-modal="false"
      :close-on-press-escape="false"
    >
      <div class="progress-container">
        <el-steps :active="currentStep" direction="vertical">
          <el-step title="生成实体类" :status="getStepStatus(0)" />
          <el-step title="生成Mapper接口" :status="getStepStatus(1)" />
          <el-step title="生成Service层" :status="getStepStatus(2)" />
          <el-step title="生成Controller层" :status="getStepStatus(3)" />
          <el-step title="生成Vue页面" :status="getStepStatus(4)" />
          <el-step title="执行数据库迁移" :status="getStepStatus(5)" />
        </el-steps>

        <div class="progress-info">
          <el-progress
            :percentage="progressPercentage"
            :status="progressStatus"
          />
          <p class="progress-text">{{ progressText }}</p>
        </div>
      </div>
      <template #footer>
        <el-button @click="progressVisible = false" :disabled="generateLoading">
          {{ generateLoading ? "生成中..." : "关闭" }}
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { ElMessage } from "element-plus";

// 模块数据
const modules = ref([
  {
    id: 1,
    moduleCode: "USER_MGMT",
    moduleName: "用户管理",
    moduleType: "business",
    tableName: "sys_user",
    entityName: "SysUser",
  },
  {
    id: 2,
    moduleCode: "PROJECT_MGMT",
    moduleName: "项目管理",
    moduleType: "business",
    tableName: "biz_project",
    entityName: "BizProject",
  },
  {
    id: 3,
    moduleCode: "WORKER_MGMT",
    moduleName: "工人管理",
    moduleType: "business",
    tableName: "biz_worker",
    entityName: "BizWorker",
  },
]);

// 生成历史
const generationHistory = ref([
  {
    generateTime: "2024-01-16 10:30:00",
    generateType: "完整生成",
    status: "success",
    description: "生成用户管理模块的所有代码文件",
  },
  {
    generateTime: "2024-01-15 14:20:00",
    generateType: "仅前端",
    status: "success",
    description: "生成项目管理模块的Vue页面",
  },
  {
    generateTime: "2024-01-14 09:15:00",
    generateType: "数据库迁移",
    status: "failed",
    description: "执行工人管理模块的数据库迁移失败",
  },
]);

const moduleSearchText = ref("");
const selectedModule = ref<any>(null);
const activeTab = ref("page");
const generateLoading = ref(false);
const testLoading = ref(false);
const migrationLoading = ref(false);
const progressVisible = ref(false);
const currentStep = ref(0);
const progressPercentage = ref(0);
const progressStatus = ref<"success" | "exception" | undefined>();
const progressText = ref("");

// 生成配置
const generateConfig = reactive({
  options: ["entity", "mapper", "service", "controller", "vue", "sql"],
  frontendType: "vue3",
  backendType: "springboot",
  databaseType: "mysql",
});

// 数据库配置
const dbConfig = reactive({
  type: "mysql",
  host: "localhost",
  port: "3306",
  database: "",
  username: "",
  password: "",
});

// 过滤后的模块列表
const filteredModules = computed(() => {
  if (!moduleSearchText.value) {
    return modules.value;
  }
  return modules.value.filter(
    (module) =>
      module.moduleName.includes(moduleSearchText.value) ||
      module.moduleCode.includes(moduleSearchText.value),
  );
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

// 获取步骤状态
const getStepStatus = (step: number) => {
  if (step < currentStep.value) {
    return "finish";
  } else if (step === currentStep.value) {
    return "process";
  } else {
    return "wait";
  }
};

// 刷新模块列表
const handleRefreshModules = () => {
  ElMessage.success("模块列表已刷新");
};

// 选择模块
const handleSelectModule = (module: any) => {
  selectedModule.value = module;
  dbConfig.database = `db_${module.moduleCode.toLowerCase()}`;
};

// 测试数据库连接
const handleTestConnection = () => {
  testLoading.value = true;
  setTimeout(() => {
    testLoading.value = false;
    ElMessage.success("数据库连接测试成功");
  }, 2000);
};

// 执行数据库迁移
const handleExecuteMigration = () => {
  migrationLoading.value = true;
  setTimeout(() => {
    migrationLoading.value = false;
    ElMessage.success("数据库迁移执行成功");

    // 添加生成记录
    generationHistory.value.unshift({
      generateTime: new Date().toLocaleString(),
      generateType: "数据库迁移",
      status: "success",
      description: `执行${selectedModule.value.moduleName}模块的数据库迁移`,
    });
  }, 3000);
};

// 一键生成
const handleGenerateAll = () => {
  if (!selectedModule.value) return;

  generateLoading.value = true;
  progressVisible.value = true;
  currentStep.value = 0;
  progressPercentage.value = 0;
  progressStatus.value = undefined;

  const steps = [
    "正在生成实体类...",
    "正在生成Mapper接口...",
    "正在生成Service层...",
    "正在生成Controller层...",
    "正在生成Vue页面...",
    "正在执行数据库迁移...",
  ];

  const executeStep = (step: number) => {
    if (step >= steps.length) {
      // 生成完成
      generateLoading.value = false;
      progressPercentage.value = 100;
      progressStatus.value = "success";
      progressText.value = "代码生成完成！";

      ElMessage.success("代码生成成功");

      // 添加生成记录
      generationHistory.value.unshift({
        generateTime: new Date().toLocaleString(),
        generateType: "完整生成",
        status: "success",
        description: `生成${selectedModule.value.moduleName}模块的所有代码文件`,
      });

      return;
    }

    currentStep.value = step;
    progressText.value = steps[step];
    progressPercentage.value = Math.round((step / steps.length) * 100);

    setTimeout(() => {
      executeStep(step + 1);
    }, 1500);
  };

  executeStep(0);
};

// 查看生成日志
const handleViewLog = (row: any) => {
  ElMessage.info(`查看生成日志: ${row.description}`);
};

onMounted(() => {
  // 默认选择第一个模块
  if (modules.value.length > 0) {
    handleSelectModule(modules.value[0]);
  }
});
</script>

<style scoped>
.page-generation {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.module-search {
  margin-bottom: 15px;
}

.module-list {
  max-height: 600px;
  overflow-y: auto;
}

.module-item {
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

.module-item:hover {
  background-color: var(--el-color-primary-light-9);
  border-color: var(--el-color-primary);
}

.module-item.active {
  background-color: var(--el-color-primary-light-8);
  border-color: var(--el-color-primary);
}

.module-info {
  flex: 1;
}

.module-name {
  font-weight: 500;
  color: var(--app-text-color);
  margin-bottom: 4px;
}

.module-code {
  font-size: 12px;
  color: var(--app-text-color-regular);
}

.no-selection {
  padding: 60px 0;
  text-align: center;
}

.generation-config {
  padding: 0;
}

.module-info-panel {
  margin-bottom: 20px;
}

.config-section {
  margin-bottom: 30px;
}

.config-section h4 {
  margin: 0 0 15px 0;
  color: var(--app-text-color);
  font-size: 16px;
  font-weight: 500;
}

.migration-actions {
  margin-top: 20px;
  display: flex;
  gap: 12px;
}

.progress-container {
  display: flex;
  gap: 30px;
}

.progress-info {
  flex: 1;
  padding-left: 20px;
}

.progress-text {
  margin-top: 10px;
  color: var(--app-text-color-regular);
  font-size: 14px;
}

:deep(.el-checkbox-group) {
  .el-checkbox {
    margin-bottom: 10px;
  }
}

:deep(.el-radio-group) {
  .el-radio {
    margin-bottom: 10px;
    margin-right: 20px;
  }
}
</style>
