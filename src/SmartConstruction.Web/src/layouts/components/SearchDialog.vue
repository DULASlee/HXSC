<template>
  <el-dialog
    v-model="dialogVisible"
    title="全局搜索"
    width="600px"
    :show-close="false"
    :close-on-click-modal="true"
    class="search-dialog"
  >
    <div class="search-container">
      <el-input
        ref="searchInputRef"
        v-model="searchKeyword"
        placeholder="搜索菜单、页面、功能..."
        size="large"
        clearable
        @input="handleSearch"
        @keydown.enter="handleEnter"
        @keydown.up="handleUp"
        @keydown.down="handleDown"
      >
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>

      <div v-if="searchResults.length > 0" class="search-results">
        <div class="result-header">
          <span>搜索结果 ({{ searchResults.length }})</span>
          <span class="result-tip">使用 ↑↓ 键导航，Enter 键选择</span>
        </div>
        
        <el-scrollbar max-height="400px">
          <div
            v-for="(item, index) in searchResults"
            :key="item.path"
            :class="['result-item', { active: activeIndex === index }]"
            @click="handleSelect(item)"
            @mouseenter="activeIndex = index"
          >
            <div class="item-icon">
              <el-icon v-if="item.icon">
                <component :is="item.icon" />
              </el-icon>
              <el-icon v-else>
                <Document />
              </el-icon>
            </div>
            
            <div class="item-content">
              <div class="item-title" v-html="highlightKeyword(item.title)"></div>
              <div class="item-path">{{ item.breadcrumb }}</div>
            </div>
            
            <div class="item-type">
              <el-tag :type="getTypeTagType(item.type)" size="small">
                {{ getTypeLabel(item.type) }}
              </el-tag>
            </div>
          </div>
        </el-scrollbar>
      </div>

      <div v-else-if="searchKeyword && !loading" class="no-results">
        <el-empty description="未找到相关结果" :image-size="80" />
      </div>

      <div v-if="!searchKeyword" class="search-tips">
        <div class="tip-section">
          <h4>搜索提示</h4>
          <ul>
            <li>输入菜单名称快速导航</li>
            <li>支持拼音首字母搜索</li>
            <li>使用空格分隔多个关键词</li>
          </ul>
        </div>
        
        <div class="tip-section">
          <h4>快捷键</h4>
          <ul>
            <li><kbd>Ctrl</kbd> + <kbd>K</kbd> 打开搜索</li>
            <li><kbd>↑</kbd> <kbd>↓</kbd> 选择结果</li>
            <li><kbd>Enter</kbd> 确认选择</li>
            <li><kbd>Esc</kbd> 关闭搜索</li>
          </ul>
        </div>
      </div>
    </div>
  </el-dialog>
</template>

<script setup lang="ts">
import { ref, computed, watch, nextTick, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { Search, Document } from '@element-plus/icons-vue'
import { useMenuStore } from '@/stores/menu'
import type { Menu } from '@/types/global'

interface SearchResult {
  path: string
  title: string
  icon?: string
  type: 'menu' | 'page' | 'function'
  breadcrumb: string
  keywords: string[]
}

interface Props {
  modelValue: boolean
}

const props = defineProps<Props>()
const emit = defineEmits(['update:modelValue'])

const router = useRouter()
const menuStore = useMenuStore()

const searchInputRef = ref()
const searchKeyword = ref('')
const searchResults = ref<SearchResult[]>([])
const activeIndex = ref(0)
const loading = ref(false)

// 对话框显示状态
const dialogVisible = computed({
  get: () => props.modelValue,
  set: (value) => emit('update:modelValue', value)
})

// 搜索数据源
const searchData = ref<SearchResult[]>([])

// 初始化搜索数据
function initSearchData() {
  const results: SearchResult[] = []
  
  // 从菜单生成搜索数据
  function processMenus(menus: Menu[], breadcrumbs: string[] = []) {
    menus.forEach(menu => {
      if (menu.isEnabled && menu.isVisible) {
        const currentBreadcrumbs = [...breadcrumbs, menu.name]
        
        results.push({
          path: menu.path || `/${menu.code}`,
          title: menu.name,
          icon: menu.icon,
          type: menu.type === 'Menu' ? 'menu' : 'function',
          breadcrumb: currentBreadcrumbs.join(' / '),
          keywords: [
            menu.name,
            menu.code,
            ...getPinyin(menu.name), // 拼音支持
            ...currentBreadcrumbs
          ]
        })
        
        if (menu.children && menu.children.length > 0) {
          processMenus(menu.children, currentBreadcrumbs)
        }
      }
    })
  }
  
  processMenus(menuStore.menuTree)
  searchData.value = results
}

// 获取拼音首字母（简化实现）
function getPinyin(text: string): string[] {
  // 这里可以集成拼音库，简化实现
  const pinyinMap: Record<string, string> = {
    '用户': 'yh',
    '管理': 'gl',
    '系统': 'xt',
    '设置': 'sz',
    '权限': 'qx',
    '角色': 'js',
    '组织': 'zz',
    '菜单': 'cd',
    '资源': 'zy',
    '租户': 'zh',
    '元数据': 'ysj',
    '配置': 'pz',
    '监控': 'jk',
    '日志': 'rz',
    '统计': 'tj',
    '报表': 'bb'
  }
  
  return Object.keys(pinyinMap)
    .filter(key => text.includes(key))
    .map(key => pinyinMap[key])
}

// 搜索处理
function handleSearch() {
  if (!searchKeyword.value.trim()) {
    searchResults.value = []
    return
  }
  
  loading.value = true
  
  // 模拟异步搜索
  setTimeout(() => {
    const keyword = searchKeyword.value.toLowerCase().trim()
    const keywords = keyword.split(' ').filter(k => k)
    
    const results = searchData.value.filter(item => {
      return keywords.every(kw => 
        item.keywords.some(itemKeyword => 
          itemKeyword.toLowerCase().includes(kw)
        )
      )
    })
    
    // 按匹配度排序
    searchResults.value = results.sort((a, b) => {
      const aScore = getMatchScore(a, keyword)
      const bScore = getMatchScore(b, keyword)
      return bScore - aScore
    }).slice(0, 20) // 限制结果数量
    
    activeIndex.value = 0
    loading.value = false
  }, 200)
}

// 计算匹配分数
function getMatchScore(item: SearchResult, keyword: string): number {
  let score = 0
  const lowerTitle = item.title.toLowerCase()
  const lowerKeyword = keyword.toLowerCase()
  
  // 完全匹配
  if (lowerTitle === lowerKeyword) score += 100
  // 开头匹配
  else if (lowerTitle.startsWith(lowerKeyword)) score += 50
  // 包含匹配
  else if (lowerTitle.includes(lowerKeyword)) score += 20
  
  // 拼音匹配
  if (item.keywords.some(k => k.includes(lowerKeyword))) score += 10
  
  return score
}

// 高亮关键词
function highlightKeyword(text: string): string {
  if (!searchKeyword.value) return text
  
  const keyword = searchKeyword.value.trim()
  const regex = new RegExp(`(${keyword})`, 'gi')
  return text.replace(regex, '<mark>$1</mark>')
}

// 获取类型标签样式
function getTypeTagType(type: string) {
  const typeMap: Record<string, any> = {
    'menu': 'primary',
    'page': 'success',
    'function': 'warning'
  }
  return typeMap[type] || 'default'
}

// 获取类型标签文本
function getTypeLabel(type: string): string {
  const labelMap: Record<string, string> = {
    'menu': '菜单',
    'page': '页面',
    'function': '功能'
  }
  return labelMap[type] || type
}

// 键盘事件处理
function handleEnter() {
  if (searchResults.value.length > 0 && activeIndex.value >= 0) {
    handleSelect(searchResults.value[activeIndex.value])
  }
}

function handleUp() {
  if (activeIndex.value > 0) {
    activeIndex.value--
  } else {
    activeIndex.value = searchResults.value.length - 1
  }
}

function handleDown() {
  if (activeIndex.value < searchResults.value.length - 1) {
    activeIndex.value++
  } else {
    activeIndex.value = 0
  }
}

// 选择结果
function handleSelect(item: SearchResult) {
  router.push(item.path)
  dialogVisible.value = false
  searchKeyword.value = ''
  searchResults.value = []
}

// 全局快捷键
function handleGlobalKeydown(e: KeyboardEvent) {
  // Ctrl+K 打开搜索
  if (e.ctrlKey && e.key === 'k') {
    e.preventDefault()
    dialogVisible.value = true
  }
  
  // Esc 关闭搜索
  if (e.key === 'Escape' && dialogVisible.value) {
    dialogVisible.value = false
  }
}

// 监听对话框显示状态
watch(dialogVisible, (visible) => {
  if (visible) {
    nextTick(() => {
      searchInputRef.value?.focus()
    })
  } else {
    searchKeyword.value = ''
    searchResults.value = []
    activeIndex.value = 0
  }
})

// 监听菜单数据变化
watch(() => menuStore.menuTree, () => {
  initSearchData()
}, { deep: true, immediate: true })

onMounted(() => {
  document.addEventListener('keydown', handleGlobalKeydown)
  initSearchData()
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleGlobalKeydown)
})
</script>

<style lang="scss" scoped>
.search-dialog {
  :deep(.el-dialog__body) {
    padding: 20px;
  }
}

.search-container {
  .search-results {
    margin-top: 16px;
    
    .result-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 8px 12px;
      background-color: var(--el-bg-color-page);
      border-radius: 4px;
      margin-bottom: 8px;
      font-size: 12px;
      color: var(--el-text-color-secondary);
      
      .result-tip {
        font-size: 11px;
      }
    }
    
    .result-item {
      display: flex;
      align-items: center;
      padding: 12px;
      border-radius: 6px;
      cursor: pointer;
      transition: background-color 0.2s;
      
      &:hover,
      &.active {
        background-color: var(--el-bg-color-page);
      }
      
      .item-icon {
        width: 32px;
        height: 32px;
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: var(--el-color-primary-light-9);
        border-radius: 6px;
        margin-right: 12px;
        
        .el-icon {
          font-size: 16px;
          color: var(--el-color-primary);
        }
      }
      
      .item-content {
        flex: 1;
        min-width: 0;
        
        .item-title {
          font-size: 14px;
          font-weight: 500;
          color: var(--el-text-color-primary);
          margin-bottom: 4px;
          
          :deep(mark) {
            background-color: var(--el-color-warning-light-7);
            color: var(--el-color-warning-dark-2);
            padding: 0 2px;
            border-radius: 2px;
          }
        }
        
        .item-path {
          font-size: 12px;
          color: var(--el-text-color-secondary);
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
        }
      }
      
      .item-type {
        margin-left: 12px;
      }
    }
  }
  
  .no-results {
    margin-top: 40px;
    text-align: center;
  }
  
  .search-tips {
    margin-top: 24px;
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 24px;
    
    .tip-section {
      h4 {
        margin: 0 0 12px 0;
        font-size: 14px;
        font-weight: 500;
        color: var(--el-text-color-primary);
      }
      
      ul {
        margin: 0;
        padding: 0;
        list-style: none;
        
        li {
          padding: 4px 0;
          font-size: 13px;
          color: var(--el-text-color-secondary);
          
          kbd {
            display: inline-block;
            padding: 2px 6px;
            font-size: 11px;
            line-height: 1;
            color: var(--el-text-color-primary);
            background-color: var(--el-bg-color-page);
            border: 1px solid var(--el-border-color);
            border-radius: 3px;
            margin: 0 2px;
          }
        }
      }
    }
  }
}
</style>