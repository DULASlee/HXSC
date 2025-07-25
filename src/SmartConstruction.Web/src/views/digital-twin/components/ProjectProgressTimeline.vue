<template>
  <div class="project-progress-timeline">
    <div class="timeline-header">
      <h3>{{ title }}</h3>
      <div class="header-actions">
        <el-select v-model="selectedProject" placeholder="选择项目" size="small" @change="handleProjectChange">
          <el-option
            v-for="project in projects"
            :key="project.id"
            :label="project.name"
            :value="project.id"
          />
        </el-select>
        <el-button type="primary" size="small" @click="handleViewDetail">查看详情</el-button>
      </div>
    </div>
    
    <div class="timeline-content">
      <div class="progress-overview">
        <div class="progress-info">
          <div class="info-item">
            <span class="label">项目状态:</span>
            <span class="value">
              <el-tag :type="getStatusType(projectData.status)">{{ getStatusText(projectData.status) }}</el-tag>
            </span>
          </div>
          <div class="info-item">
            <span class="label">开始日期:</span>
            <span class="value">{{ projectData.startDate || '未设置' }}</span>
          </div>
          <div class="info-item">
            <span class="label">计划完成:</span>
            <span class="value">{{ projectData.plannedEndDate || '未设置' }}</span>
          </div>
          <div class="info-item">
            <span class="label">实际进度:</span>
            <span class="value">{{ projectData.progress || 0 }}%</span>
          </div>
        </div>
        
        <div class="progress-bar">
          <el-progress 
            :percentage="projectData.progress || 0" 
            :color="getProgressColor(projectData.progress || 0)"
            :stroke-width="12"
            :format="(val) => val + '%'"
          />
        </div>
        
        <div class="time-remaining">
          <div class="remaining-days" :class="getRemainingDaysClass(projectData.remainingDays || 0)">
            <span class="value">{{ projectData.remainingDays || 0 }}</span>
            <span class="label">剩余天数</span>
          </div>
          <div class="completion-status">
            <div class="status-item">
              <div class="status-icon" :class="getCompletionStatusClass(projectData.timeStatus || '')">
                <el-icon><Timer /></el-icon>
              </div>
              <div class="status-text">{{ getTimeStatusText(projectData.timeStatus || '') }}</div>
            </div>
            <div class="status-item">
              <div class="status-icon" :class="getCompletionStatusClass(projectData.budgetStatus || '')">
                <el-icon><Money /></el-icon>
              </div>
              <div class="status-text">{{ getBudgetStatusText(projectData.budgetStatus || '') }}</div>
            </div>
          </div>
        </div>
      </div>
      
      <div class="timeline-container">
        <h4>项目里程碑</h4>
        <el-timeline>
          <el-timeline-item
            v-for="milestone in milestones"
            :key="milestone.id"
            :timestamp="milestone.date"
            :type="getMilestoneType(milestone)"
            :color="getMilestoneColor(milestone)"
            :hollow="milestone.status === 'pending'"
            :size="milestone.important ? 'large' : 'normal'"
          >
            <div class="milestone-content">
              <div class="milestone-header">
                <h5 class="milestone-title">{{ milestone.title }}</h5>
                <el-tag size="small" :type="getMilestoneStatusType(milestone.status)">
                  {{ getMilestoneStatusText(milestone.status) }}
                </el-tag>
              </div>
              <div class="milestone-description">{{ milestone.description }}</div>
              <div class="milestone-meta">
                <div class="meta-item" v-if="milestone.owner">
                  <el-icon><User /></el-icon>
                  <span>{{ milestone.owner }}</span>
                </div>
                <div class="meta-item" v-if="milestone.completionDate && milestone.status === 'completed'">
                  <el-icon><Calendar /></el-icon>
                  <span>完成于: {{ milestone.completionDate }}</span>
                </div>
              </div>
            </div>
          </el-timeline-item>
        </el-timeline>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { ElMessage } from 'element-plus'
import { Timer, Money, User, Calendar } from '@element-plus/icons-vue'

const props = defineProps({
  title: {
    type: String,
    default: '项目进度'
  },
  projectId: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['project-change', 'view-detail'])

// 状态
const projects = ref<any[]>([])
const selectedProject = ref('')
const projectData = ref<any>({})
const milestones = ref<any[]>([])

// 加载项目列表
const loadProjects = async () => {
  try {
    // 模拟从API获取项目列表
    // 实际项目中应该调用真实的API
    const mockProjects = [
      {
        id: 'proj001',
        name: '智慧工地示范项目A区'
      },
      {
        id: 'proj002',
        name: '智慧工地示范项目B区'
      },
      {
        id: 'proj003',
        name: '智慧工地示范项目C区'
      }
    ]
    
    projects.value = mockProjects
    
    // 如果有传入的项目ID，则选中该项目
    if (props.projectId && projects.value.some(p => p.id === props.projectId)) {
      selectedProject.value = props.projectId
    } else if (projects.value.length > 0) {
      selectedProject.value = projects.value[0].id
    }
    
    // 加载项目数据
    await loadProjectData()
  } catch (error) {
    console.error('Failed to load projects:', error)
    ElMessage.error('加载项目列表失败')
  }
}

// 加载项目数据
const loadProjectData = async () => {
  if (!selectedProject.value) return
  
  try {
    // 模拟从API获取项目数据
    // 实际项目中应该调用真实的API
    const mockProjectData = {
      id: selectedProject.value,
      name: projects.value.find(p => p.id === selectedProject.value)?.name || '',
      status: selectedProject.value === 'proj003' ? 'completed' : 'active',
      startDate: '2024-01-15',
      plannedEndDate: '2024-12-31',
      actualEndDate: selectedProject.value === 'proj003' ? '2024-05-30' : null,
      progress: selectedProject.value === 'proj001' ? 65 : (selectedProject.value === 'proj002' ? 78 : 100),
      remainingDays: selectedProject.value === 'proj001' ? 160 : (selectedProject.value === 'proj002' ? 120 : 0),
      timeStatus: selectedProject.value === 'proj001' ? 'behind' : (selectedProject.value === 'proj002' ? 'ontrack' : 'completed'),
      budgetStatus: selectedProject.value === 'proj001' ? 'overbudget' : (selectedProject.value === 'proj002' ? 'ontrack' : 'underbudget')
    }
    
    projectData.value = mockProjectData
    
    // 加载里程碑
    await loadMilestones()
  } catch (error) {
    console.error('Failed to load project data:', error)
    ElMessage.error('加载项目数据失败')
  }
}

// 加载里程碑
const loadMilestones = async () => {
  if (!selectedProject.value) return
  
  try {
    // 模拟从API获取里程碑数据
    // 实际项目中应该调用真实的API
    const mockMilestones = [
      {
        id: 'ms001',
        title: '项目启动',
        description: '项目正式启动，完成团队组建和初始规划',
        date: '2024-01-15',
        status: 'completed',
        completionDate: '2024-01-15',
        owner: '张项目',
        important: true
      },
      {
        id: 'ms002',
        title: '基础设施建设',
        description: '完成基础设施建设，包括临时道路、水电接入等',
        date: '2024-02-28',
        status: 'completed',
        completionDate: '2024-03-05',
        owner: '李工程',
        important: false
      },
      {
        id: 'ms003',
        title: '主体结构施工',
        description: '完成主体结构施工，包括地基、框架等',
        date: '2024-06-30',
        status: selectedProject.value === 'proj001' ? 'inprogress' : (selectedProject.value === 'proj002' ? 'completed' : 'completed'),
        completionDate: selectedProject.value === 'proj002' ? '2024-06-25' : (selectedProject.value === 'proj003' ? '2024-04-15' : null),
        owner: '王结构',
        important: true
      },
      {
        id: 'ms004',
        title: '内部装修',
        description: '完成内部装修，包括墙面、地面、天花板等',
        date: '2024-09-30',
        status: selectedProject.value === 'proj001' ? 'pending' : (selectedProject.value === 'proj002' ? 'inprogress' : 'completed'),
        completionDate: selectedProject.value === 'proj003' ? '2024-05-10' : null,
        owner: '赵装修',
        important: false
      },
      {
        id: 'ms005',
        title: '设备安装',
        description: '完成设备安装，包括电梯、空调、消防等',
        date: '2024-11-15',
        status: selectedProject.value === 'proj003' ? 'completed' : 'pending',
        completionDate: selectedProject.value === 'proj003' ? '2024-05-20' : null,
        owner: '钱设备',
        important: false
      },
      {
        id: 'ms006',
        title: '项目验收',
        description: '完成项目验收，包括质量验收、安全验收等',
        date: '2024-12-15',
        status: selectedProject.value === 'proj003' ? 'completed' : 'pending',
        completionDate: selectedProject.value === 'proj003' ? '2024-05-30' : null,
        owner: '孙验收',
        important: true
      }
    ]
    
    milestones.value = mockMilestones
  } catch (error) {
    console.error('Failed to load milestones:', error)
    ElMessage.error('加载里程碑数据失败')
  }
}

// 处理项目变更
const handleProjectChange = () => {
  loadProjectData()
  emit('project-change', selectedProject.value)
}

// 处理查看详情
const handleViewDetail = () => {
  emit('view-detail', {
    projectId: selectedProject.value,
    projectName: projectData.value.name
  })
}

// 工具函数
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'active': '进行中',
    'completed': '已完成',
    'pending': '待开始',
    'suspended': '已暂停'
  }
  return statusMap[status] || status
}

const getStatusType = (status: string) => {
  const typeMap: Record<string, string> = {
    'active': 'success',
    'completed': 'info',
    'pending': 'warning',
    'suspended': 'danger'
  }
  return typeMap[status] || 'info'
}

const getProgressColor = (progress: number) => {
  if (progress >= 80) return '#2ecc71'
  if (progress >= 60) return '#3498db'
  if (progress >= 40) return '#f39c12'
  return '#e74c3c'
}

const getRemainingDaysClass = (days: number) => {
  if (days <= 0) return 'completed'
  if (days <= 30) return 'critical'
  if (days <= 60) return 'warning'
  return 'normal'
}

const getCompletionStatusClass = (status: string) => {
  if (status === 'completed') return 'completed'
  if (status === 'ontrack') return 'ontrack'
  if (status === 'behind' || status === 'overbudget') return 'behind'
  return 'normal'
}

const getTimeStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'ahead': '提前',
    'ontrack': '按计划',
    'behind': '延期',
    'completed': '已完成'
  }
  return statusMap[status] || status
}

const getBudgetStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'underbudget': '节约预算',
    'ontrack': '预算平衡',
    'overbudget': '超出预算'
  }
  return statusMap[status] || status
}

const getMilestoneType = (milestone: any) => {
  if (milestone.status === 'completed') return 'success'
  if (milestone.status === 'inprogress') return 'primary'
  if (milestone.status === 'pending') return 'info'
  return 'info'
}

const getMilestoneColor = (milestone: any) => {
  if (milestone.status === 'completed') return '#2ecc71'
  if (milestone.status === 'inprogress') return '#3498db'
  if (milestone.status === 'pending') return '#7f8c8d'
  return '#7f8c8d'
}

const getMilestoneStatusType = (status: string) => {
  const typeMap: Record<string, string> = {
    'completed': 'success',
    'inprogress': 'primary',
    'pending': 'info',
    'delayed': 'danger'
  }
  return typeMap[status] || 'info'
}

const getMilestoneStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'completed': '已完成',
    'inprogress': '进行中',
    'pending': '待开始',
    'delayed': '已延期'
  }
  return statusMap[status] || status
}

// 监听项目ID变化
watch(() => props.projectId, (newVal) => {
  if (newVal && newVal !== selectedProject.value) {
    selectedProject.value = newVal
    loadProjectData()
  }
})

// 生命周期钩子
onMounted(() => {
  loadProjects()
})
</script>

<style lang="scss" scoped>
.project-progress-timeline {
  width: 100%;
  height: 100%;
  
  .timeline-header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 16px;
    
    h3 {
      margin: 0;
      font-size: 18px;
      font-weight: 600;
      color: #ffffff;
    }
    
    .header-actions {
      display: flex;
      gap: 12px;
    }
  }
  
  .timeline-content {
    display: flex;
    flex-direction: column;
    gap: 20px;
    
    .progress-overview {
      display: grid;
      grid-template-columns: 1fr 2fr 1fr;
      gap: 20px;
      background: rgba(255, 255, 255, 0.05);
      border-radius: 8px;
      padding: 16px;
      
      .progress-info {
        .info-item {
          display: flex;
          margin-bottom: 8px;
          
          &:last-child {
            margin-bottom: 0;
          }
          
          .label {
            width: 80px;
            color: #7f8c8d;
          }
          
          .value {
            flex: 1;
            color: #ffffff;
          }
        }
      }
      
      .progress-bar {
        display: flex;
        align-items: center;
      }
      
      .time-remaining {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        gap: 16px;
        
        .remaining-days {
          display: flex;
          flex-direction: column;
          align-items: center;
          
          .value {
            font-size: 32px;
            font-weight: 700;
          }
          
          .label {
            font-size: 14px;
            color: #7f8c8d;
          }
          
          &.normal {
            .value {
              color: #3498db;
            }
          }
          
          &.warning {
            .value {
              color: #f39c12;
            }
          }
          
          &.critical {
            .value {
              color: #e74c3c;
            }
          }
          
          &.completed {
            .value {
              color: #2ecc71;
            }
          }
        }
        
        .completion-status {
          display: flex;
          gap: 16px;
          
          .status-item {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 4px;
            
            .status-icon {
              width: 32px;
              height: 32px;
              border-radius: 50%;
              display: flex;
              align-items: center;
              justify-content: center;
              
              &.ontrack {
                background: rgba(46, 204, 113, 0.2);
                color: #2ecc71;
              }
              
              &.behind {
                background: rgba(231, 76, 60, 0.2);
                color: #e74c3c;
              }
              
              &.completed {
                background: rgba(52, 152, 219, 0.2);
                color: #3498db;
              }
            }
            
            .status-text {
              font-size: 12px;
              color: #7f8c8d;
            }
          }
        }
      }
    }
    
    .timeline-container {
      background: rgba(255, 255, 255, 0.05);
      border-radius: 8px;
      padding: 16px;
      
      h4 {
        margin: 0 0 16px;
        font-size: 16px;
        font-weight: 600;
        color: #ffffff;
      }
      
      .milestone-content {
        .milestone-header {
          display: flex;
          align-items: center;
          justify-content: space-between;
          margin-bottom: 8px;
          
          .milestone-title {
            margin: 0;
            font-size: 16px;
            font-weight: 600;
            color: #ffffff;
          }
        }
        
        .milestone-description {
          font-size: 14px;
          color: #bdc3c7;
          margin-bottom: 8px;
        }
        
        .milestone-meta {
          display: flex;
          gap: 16px;
          
          .meta-item {
            display: flex;
            align-items: center;
            gap: 4px;
            font-size: 12px;
            color: #7f8c8d;
          }
        }
      }
    }
  }
}

// 响应式设计
@media (max-width: 1200px) {
  .project-progress-timeline {
    .timeline-content {
      .progress-overview {
        grid-template-columns: 1fr 1fr;
        
        .time-remaining {
          grid-column: 1 / -1;
          flex-direction: row;
          justify-content: space-around;
        }
      }
    }
  }
}

@media (max-width: 768px) {
  .project-progress-timeline {
    .timeline-header {
      flex-direction: column;
      align-items: flex-start;
      gap: 12px;
      
      .header-actions {
        width: 100%;
      }
    }
    
    .timeline-content {
      .progress-overview {
        grid-template-columns: 1fr;
        
        .progress-info, .progress-bar, .time-remaining {
          grid-column: 1;
        }
      }
    }
  }
}
</style>