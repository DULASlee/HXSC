<template>
  <div class="dictionary-management">
    <el-row :gutter="20">
      <!-- 左侧字典类型 -->
      <el-col :span="8">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>字典类型</span>
              <el-button type="primary" size="small" @click="handleAddType">
                <el-icon><Plus /></el-icon>
                添加类型
              </el-button>
            </div>
          </template>

          <div class="type-search">
            <el-input
              v-model="typeSearchText"
              placeholder="搜索字典类型"
              clearable
            >
              <template #prefix>
                <el-icon><Search /></el-icon>
              </template>
            </el-input>
          </div>

          <div class="type-list">
            <div
              v-for="type in filteredTypes"
              :key="type.id"
              :class="['type-item', { active: selectedType?.id === type.id }]"
              @click="handleSelectType(type)"
            >
              <div class="type-info">
                <div class="type-name">{{ type.dictName }}</div>
                <div class="type-code">{{ type.dictType }}</div>
              </div>
              <el-tag
                :type="type.status === '0' ? 'success' : 'danger'"
                size="small"
              >
                {{ type.status === "0" ? "启用" : "禁用" }}
              </el-tag>
            </div>
          </div>
        </el-card>
      </el-col>

      <!-- 右侧字典数据 -->
      <el-col :span="16">
        <el-card>
          <template #header>
            <div class="card-header">
              <span>字典数据</span>
              <div v-if="selectedType">
                <el-button type="primary" @click="handleAddData">
                  <el-icon><Plus /></el-icon>
                  添加数据
                </el-button>
              </div>
            </div>
          </template>

          <div v-if="!selectedType" class="no-selection">
            <el-empty description="请选择一个字典类型查看数据" />
          </div>

          <div v-else class="data-content">
            <!-- 字典类型信息 -->
            <div class="type-info-panel">
              <el-descriptions :column="2" border>
                <el-descriptions-item label="字典名称">{{
                  selectedType.dictName
                }}</el-descriptions-item>
                <el-descriptions-item label="字典类型">{{
                  selectedType.dictType
                }}</el-descriptions-item>
                <el-descriptions-item label="状态">
                  <el-tag
                    :type="selectedType.status === '0' ? 'success' : 'danger'"
                  >
                    {{ selectedType.status === "0" ? "启用" : "禁用" }}
                  </el-tag>
                </el-descriptions-item>
                <el-descriptions-item label="备注">{{
                  selectedType.remark || "-"
                }}</el-descriptions-item>
              </el-descriptions>
            </div>

            <!-- 表格区域 -->
            <el-table
              :data="dictDataList"
              style="width: 100%"
              v-loading="dataLoading"
            >
              <el-table-column prop="dictSort" label="排序" width="80" />
              <el-table-column prop="dictLabel" label="数据标签" />
              <el-table-column prop="dictValue" label="数据键值" />
              <el-table-column prop="listClass" label="样式属性" width="120">
                <template #default="{ row }">
                  <el-tag v-if="row.listClass" :type="row.listClass">
                    {{ row.listClass }}
                  </el-tag>
                  <span v-else>-</span>
                </template>
              </el-table-column>
              <el-table-column prop="status" label="状态" width="80">
                <template #default="{ row }">
                  <el-tag :type="row.status === '0' ? 'success' : 'danger'">
                    {{ row.status === "0" ? "启用" : "禁用" }}
                  </el-tag>
                </template>
              </el-table-column>
              <el-table-column
                prop="remark"
                label="备注"
                show-overflow-tooltip
              />
              <el-table-column label="操作" width="150" fixed="right">
                <template #default="{ row }">
                  <el-button size="small" @click="handleEditData(row)">
                    编辑
                  </el-button>
                  <el-button
                    size="small"
                    type="danger"
                    @click="handleDeleteData(row)"
                  >
                    删除
                  </el-button>
                </template>
              </el-table-column>
            </el-table>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <!-- 字典类型对话框 -->
    <el-dialog
      v-model="typeDialogVisible"
      :title="typeDialogTitle"
      width="600px"
      @close="handleTypeDialogClose"
    >
      <el-form
        ref="typeFormRef"
        :model="typeFormData"
        :rules="typeFormRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="字典名称" prop="dictName">
              <el-input v-model="typeFormData.dictName" placeholder="请输入字典名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="字典类型" prop="dictType">
              <el-input v-model="typeFormData.dictType" placeholder="请输入字典类型" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="typeFormData.status">
                <el-radio label="0">启用</el-radio>
                <el-radio label="1">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="备注">
          <el-input
            v-model="typeFormData.remark"
            type="textarea"
            :rows="3"
            placeholder="请输入备注"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="typeDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleTypeSubmit" :loading="typeSubmitLoading">
          确定
        </el-button>
      </template>
    </el-dialog>

    <!-- 字典数据对话框 -->
    <el-dialog
      v-model="dataDialogVisible"
      :title="dataDialogTitle"
      width="600px"
      @close="handleDataDialogClose"
    >
      <el-form
        ref="dataFormRef"
        :model="dataFormData"
        :rules="dataFormRules"
        label-width="100px"
      >
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="数据标签" prop="dictLabel">
              <el-input v-model="dataFormData.dictLabel" placeholder="请输入数据标签" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="数据键值" prop="dictValue">
              <el-input v-model="dataFormData.dictValue" placeholder="请输入数据键值" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="显示排序" prop="dictSort">
              <el-input-number v-model="dataFormData.dictSort" :min="0" :max="999" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="状态" prop="status">
              <el-radio-group v-model="dataFormData.status">
                <el-radio label="0">启用</el-radio>
                <el-radio label="1">禁用</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="样式属性">
          <el-select v-model="dataFormData.listClass" placeholder="请选择样式" clearable style="width: 100%">
            <el-option label="默认" value="default" />
            <el-option label="主要" value="primary" />
            <el-option label="成功" value="success" />
            <el-option label="信息" value="info" />
            <el-option label="警告" value="warning" />
            <el-option label="危险" value="danger" />
          </el-select>
        </el-form-item>
        <el-form-item label="备注">
          <el-input
            v-model="dataFormData.remark"
            type="textarea"
            :rows="3"
            placeholder="请输入备注"
          />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dataDialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleDataSubmit" :loading="dataSubmitLoading">
          确定
        </el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, computed } from "vue";
import { ElMessage, ElMessageBox } from "element-plus";

// 字典类型数据 - 智慧工地和智慧建造相关的默认字典
const dictTypes = ref([
  {
    id: 1,
    dictName: "项目状态",
    dictType: "project_status",
    status: "0",
    remark: "项目进展状态分类",
  },
  {
    id: 2,
    dictName: "工人类型",
    dictType: "worker_type",
    status: "0",
    remark: "工人工种分类",
  },
  {
    id: 3,
    dictName: "考勤状态",
    dictType: "attendance_status",
    status: "0",
    remark: "考勤记录状态",
  },
  {
    id: 4,
    dictName: "设备类型",
    dictType: "device_type",
    status: "0",
    remark: "监控设备类型分类",
  },
  {
    id: 5,
    dictName: "安全等级",
    dictType: "safety_level",
    status: "0",
    remark: "安全风险等级分类",
  },
  {
    id: 6,
    dictName: "材料类型",
    dictType: "material_type",
    status: "0",
    remark: "建筑材料分类",
  },
  {
    id: 7,
    dictName: "质量等级",
    dictType: "quality_level",
    status: "0",
    remark: "工程质量等级",
  },
  {
    id: 8,
    dictName: "施工阶段",
    dictType: "construction_phase",
    status: "0",
    remark: "施工进度阶段",
  },
]);

// 字典数据映射
const dictDataMap: Record<string, any[]> = {
  project_status: [
    {
      dictCode: 1,
      dictSort: 1,
      dictLabel: "规划中",
      dictValue: "0",
      listClass: "info",
      status: "0",
      remark: "项目处于规划阶段",
    },
    {
      dictCode: 2,
      dictSort: 2,
      dictLabel: "进行中",
      dictValue: "1",
      listClass: "primary",
      status: "0",
      remark: "项目正在施工",
    },
    {
      dictCode: 3,
      dictSort: 3,
      dictLabel: "已完成",
      dictValue: "2",
      listClass: "success",
      status: "0",
      remark: "项目已完工",
    },
    {
      dictCode: 4,
      dictSort: 4,
      dictLabel: "已暂停",
      dictValue: "3",
      listClass: "warning",
      status: "0",
      remark: "项目暂时停工",
    },
    {
      dictCode: 5,
      dictSort: 5,
      dictLabel: "已取消",
      dictValue: "4",
      listClass: "danger",
      status: "0",
      remark: "项目已取消",
    },
  ],
  worker_type: [
    {
      dictCode: 6,
      dictSort: 1,
      dictLabel: "钢筋工",
      dictValue: "1",
      listClass: "primary",
      status: "0",
      remark: "钢筋绑扎作业",
    },
    {
      dictCode: 7,
      dictSort: 2,
      dictLabel: "木工",
      dictValue: "2",
      listClass: "success",
      status: "0",
      remark: "模板制作安装",
    },
    {
      dictCode: 8,
      dictSort: 3,
      dictLabel: "混凝土工",
      dictValue: "3",
      listClass: "info",
      status: "0",
      remark: "混凝土浇筑",
    },
    {
      dictCode: 9,
      dictSort: 4,
      dictLabel: "电工",
      dictValue: "4",
      listClass: "warning",
      status: "0",
      remark: "电气安装作业",
    },
    {
      dictCode: 10,
      dictSort: 5,
      dictLabel: "焊工",
      dictValue: "5",
      listClass: "danger",
      status: "0",
      remark: "金属焊接作业",
    },
    {
      dictCode: 11,
      dictSort: 6,
      dictLabel: "架子工",
      dictValue: "6",
      listClass: "primary",
      status: "0",
      remark: "脚手架搭设",
    },
    {
      dictCode: 12,
      dictSort: 7,
      dictLabel: "瓦工",
      dictValue: "7",
      listClass: "success",
      status: "0",
      remark: "砌筑抹灰作业",
    },
    {
      dictCode: 13,
      dictSort: 8,
      dictLabel: "管理人员",
      dictValue: "8",
      listClass: "info",
      status: "0",
      remark: "现场管理人员",
    },
  ],
  attendance_status: [
    {
      dictCode: 14,
      dictSort: 1,
      dictLabel: "正常",
      dictValue: "1",
      listClass: "success",
      status: "0",
      remark: "正常出勤",
    },
    {
      dictCode: 15,
      dictSort: 2,
      dictLabel: "迟到",
      dictValue: "2",
      listClass: "warning",
      status: "0",
      remark: "迟到",
    },
    {
      dictCode: 16,
      dictSort: 3,
      dictLabel: "早退",
      dictValue: "3",
      listClass: "warning",
      status: "0",
      remark: "早退",
    },
    {
      dictCode: 17,
      dictSort: 4,
      dictLabel: "缺勤",
      dictValue: "4",
      listClass: "danger",
      status: "0",
      remark: "无故缺勤",
    },
    {
      dictCode: 18,
      dictSort: 5,
      dictLabel: "请假",
      dictValue: "5",
      listClass: "info",
      status: "0",
      remark: "请假",
    },
  ],
  device_type: [
    {
      dictCode: 19,
      dictSort: 1,
      dictLabel: "摄像头",
      dictValue: "1",
      listClass: "primary",
      status: "0",
      remark: "视频监控设备",
    },
    {
      dictCode: 20,
      dictSort: 2,
      dictLabel: "传感器",
      dictValue: "2",
      listClass: "success",
      status: "0",
      remark: "环境监测传感器",
    },
    {
      dictCode: 21,
      dictSort: 3,
      dictLabel: "门禁系统",
      dictValue: "3",
      listClass: "info",
      status: "0",
      remark: "出入口控制",
    },
    {
      dictCode: 22,
      dictSort: 4,
      dictLabel: "扬尘监测",
      dictValue: "4",
      listClass: "warning",
      status: "0",
      remark: "扬尘浓度监测",
    },
    {
      dictCode: 23,
      dictSort: 5,
      dictLabel: "噪音监测",
      dictValue: "5",
      listClass: "danger",
      status: "0",
      remark: "噪音分贝监测",
    },
  ],
  safety_level: [
    {
      dictCode: 24,
      dictSort: 1,
      dictLabel: "低风险",
      dictValue: "1",
      listClass: "success",
      status: "0",
      remark: "安全风险较低",
    },
    {
      dictCode: 25,
      dictSort: 2,
      dictLabel: "中风险",
      dictValue: "2",
      listClass: "warning",
      status: "0",
      remark: "安全风险中等",
    },
    {
      dictCode: 26,
      dictSort: 3,
      dictLabel: "高风险",
      dictValue: "3",
      listClass: "danger",
      status: "0",
      remark: "安全风险较高",
    },
    {
      dictCode: 27,
      dictSort: 4,
      dictLabel: "极高风险",
      dictValue: "4",
      listClass: "danger",
      status: "0",
      remark: "安全风险极高",
    },
  ],
  material_type: [
    {
      dictCode: 28,
      dictSort: 1,
      dictLabel: "钢材",
      dictValue: "1",
      listClass: "primary",
      status: "0",
      remark: "钢筋、钢板等",
    },
    {
      dictCode: 29,
      dictSort: 2,
      dictLabel: "水泥",
      dictValue: "2",
      listClass: "info",
      status: "0",
      remark: "各种水泥制品",
    },
    {
      dictCode: 30,
      dictSort: 3,
      dictLabel: "砂石",
      dictValue: "3",
      listClass: "warning",
      status: "0",
      remark: "砂子、石子等",
    },
    {
      dictCode: 31,
      dictSort: 4,
      dictLabel: "木材",
      dictValue: "4",
      listClass: "success",
      status: "0",
      remark: "模板、木方等",
    },
    {
      dictCode: 32,
      dictSort: 5,
      dictLabel: "砖块",
      dictValue: "5",
      listClass: "danger",
      status: "0",
      remark: "红砖、空心砖等",
    },
  ],
  quality_level: [
    {
      dictCode: 33,
      dictSort: 1,
      dictLabel: "优良",
      dictValue: "1",
      listClass: "success",
      status: "0",
      remark: "质量优良",
    },
    {
      dictCode: 34,
      dictSort: 2,
      dictLabel: "合格",
      dictValue: "2",
      listClass: "primary",
      status: "0",
      remark: "质量合格",
    },
    {
      dictCode: 35,
      dictSort: 3,
      dictLabel: "不合格",
      dictValue: "3",
      listClass: "danger",
      status: "0",
      remark: "质量不合格",
    },
  ],
  construction_phase: [
    {
      dictCode: 36,
      dictSort: 1,
      dictLabel: "基础施工",
      dictValue: "1",
      listClass: "info",
      status: "0",
      remark: "地基基础阶段",
    },
    {
      dictCode: 37,
      dictSort: 2,
      dictLabel: "主体施工",
      dictValue: "2",
      listClass: "primary",
      status: "0",
      remark: "主体结构阶段",
    },
    {
      dictCode: 38,
      dictSort: 3,
      dictLabel: "装饰装修",
      dictValue: "3",
      listClass: "success",
      status: "0",
      remark: "装饰装修阶段",
    },
    {
      dictCode: 39,
      dictSort: 4,
      dictLabel: "设备安装",
      dictValue: "4",
      listClass: "warning",
      status: "0",
      remark: "机电设备安装",
    },
    {
      dictCode: 40,
      dictSort: 5,
      dictLabel: "竣工验收",
      dictValue: "5",
      listClass: "success",
      status: "0",
      remark: "竣工验收阶段",
    },
  ],
};

const typeSearchText = ref("");
const selectedType = ref<any>(null);
const dictDataList = ref<any[]>([]);
const dataLoading = ref(false);

// 对话框状态
const typeDialogVisible = ref(false);
const dataDialogVisible = ref(false);
const typeSubmitLoading = ref(false);
const dataSubmitLoading = ref(false);

// 表单引用
const typeFormRef = ref();
const dataFormRef = ref();

// 字典类型表单
const typeFormData = reactive({
  dictId: null,
  dictName: "",
  dictType: "",
  status: "0",
  remark: "",
});

// 字典数据表单
const dataFormData = reactive({
  dictCode: null,
  dictSort: 0,
  dictLabel: "",
  dictValue: "",
  dictType: "",
  listClass: "",
  status: "0",
  remark: "",
});

// 表单验证规则
const typeFormRules = {
  dictName: [{ required: true, message: "请输入字典名称", trigger: "blur" }],
  dictType: [{ required: true, message: "请输入字典类型", trigger: "blur" }],
};

const dataFormRules = {
  dictLabel: [{ required: true, message: "请输入数据标签", trigger: "blur" }],
  dictValue: [{ required: true, message: "请输入数据键值", trigger: "blur" }],
  dictSort: [{ required: true, message: "请输入显示排序", trigger: "blur" }],
};

// 对话框标题
const typeDialogTitle = computed(() => {
  return typeFormData.dictId ? "编辑字典类型" : "添加字典类型";
});

const dataDialogTitle = computed(() => {
  return dataFormData.dictCode ? "编辑字典数据" : "添加字典数据";
});

// 过滤后的字典类型
const filteredTypes = computed(() => {
  if (!typeSearchText.value) {
    return dictTypes.value;
  }
  return dictTypes.value.filter(
    (type) =>
      type.dictName.includes(typeSearchText.value) ||
      type.dictType.includes(typeSearchText.value),
  );
});

// 选择字典类型
const handleSelectType = (type: any) => {
  selectedType.value = type;
  loadDictData(type.dictType);
};

// 加载字典数据
const loadDictData = (dictType: string) => {
  dataLoading.value = true;
  setTimeout(() => {
    const data = dictDataMap[dictType] || [];
    dictDataList.value = data;
    dataLoading.value = false;
  }, 500);
};

// 字典类型操作
const handleAddType = () => {
  Object.assign(typeFormData, {
    dictId: null,
    dictName: "",
    dictType: "",
    status: "0",
    remark: "",
  });
  typeDialogVisible.value = true;
};

const handleTypeSubmit = async () => {
  if (!typeFormRef.value) return;

  await typeFormRef.value.validate((valid: boolean) => {
    if (valid) {
      typeSubmitLoading.value = true;
      setTimeout(() => {
        typeSubmitLoading.value = false;
        typeDialogVisible.value = false;
        ElMessage.success(typeFormData.dictId ? "编辑成功" : "添加成功");
      }, 1000);
    }
  });
};

const handleTypeDialogClose = () => {
  typeFormRef.value?.resetFields();
};

// 字典数据操作
const handleAddData = () => {
  if (!selectedType.value) return;

  Object.assign(dataFormData, {
    dictCode: null,
    dictSort: 0,
    dictLabel: "",
    dictValue: "",
    dictType: selectedType.value.dictType,
    listClass: "",
    status: "0",
    remark: "",
  });
  dataDialogVisible.value = true;
};

const handleEditData = (row: any) => {
  Object.assign(dataFormData, { ...row });
  dataDialogVisible.value = true;
};

const handleDataSubmit = async () => {
  if (!dataFormRef.value) return;

  await dataFormRef.value.validate((valid: boolean) => {
    if (valid) {
      dataSubmitLoading.value = true;
      setTimeout(() => {
        dataSubmitLoading.value = false;
        dataDialogVisible.value = false;
        ElMessage.success(dataFormData.dictCode ? "编辑成功" : "添加成功");
        loadDictData(selectedType.value.dictType);
      }, 1000);
    }
  });
};

const handleDataDialogClose = () => {
  dataFormRef.value?.resetFields();
};

const handleDeleteData = (row: any) => {
  ElMessageBox.confirm(`确认删除字典数据 "${row.dictLabel}" 吗？`, "警告", {
    confirmButtonText: "确定",
    cancelButtonText: "取消",
    type: "warning",
  })
    .then(() => {
      ElMessage.success("删除成功");
    })
    .catch(() => {});
};

onMounted(() => {
  // 默认选择第一个字典类型
  if (dictTypes.value.length > 0) {
    handleSelectType(dictTypes.value[0]);
  }
});
</script>

<style scoped>
.dictionary-management {
  padding: 0;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.type-search {
  margin-bottom: 15px;
}

.type-list {
  max-height: 600px;
  overflow-y: auto;
}

.type-item {
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

.type-item:hover {
  background-color: var(--el-color-primary-light-9);
  border-color: var(--el-color-primary);
}

.type-item.active {
  background-color: var(--el-color-primary-light-8);
  border-color: var(--el-color-primary);
}

.type-info {
  flex: 1;
}

.type-name {
  font-weight: 500;
  color: var(--app-text-color);
  margin-bottom: 4px;
}

.type-code {
  font-size: 12px;
  color: var(--app-text-color-regular);
}

.no-selection {
  padding: 60px 0;
  text-align: center;
}

.data-content {
  padding: 0;
}

.type-info-panel {
  margin-bottom: 20px;
}
</style>
