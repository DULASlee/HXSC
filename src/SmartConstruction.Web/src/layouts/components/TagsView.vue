<template>
  <div class="tags-view-container">
    <el-scrollbar class="tags-view-wrapper" ref="scrollbarRef">
      <div class="tags-view-content" ref="contentRef">
        <router-link
          v-for="tag in visitedViews"
          :key="tag.path"
          :to="{ path: tag.path, query: tag.query }"
          :class="['tags-view-item', { active: isActive(tag) }]"
          @click.middle="closeSelectedTag(tag)"
          @contextmenu.prevent="openMenu(tag, $event)"
        >
          <span class="tag-title">{{ tag.title }}</span>
          <el-icon
            v-if="!isAffix(tag)"
            class="tag-close"
            @click.prevent.stop="closeSelectedTag(tag)"
          >
            <Close />
          </el-icon>
        </router-link>
      </div>
    </el-scrollbar>

    <!-- 右键菜单 -->
    <ul
      v-show="contextMenu.visible"
      :style="{ left: contextMenu.left + 'px', top: contextMenu.top + 'px' }"
      class="contextmenu"
    >
      <li @click="refreshSelectedTag(contextMenu.selectedTag)">
        <el-icon><Refresh /></el-icon>
        刷新
      </li>
      <li
        v-if="!isAffix(contextMenu.selectedTag)"
        @click="closeSelectedTag(contextMenu.selectedTag)"
      >
        <el-icon><Close /></el-icon>
        关闭
      </li>
      <li @click="closeOthersTags">
        <el-icon><CircleClose /></el-icon>
        关闭其他
      </li>
      <li @click="closeAllTags(contextMenu.selectedTag)">
        <el-icon><FolderDelete /></el-icon>
        关闭所有
      </li>
    </ul>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, computed, watch, nextTick, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import type { RouteLocationNormalized } from 'vue-router'
import { Close, Refresh, CircleClose, FolderDelete } from '@element-plus/icons-vue'
import { useAppStore } from '@/stores/app'

interface TagView {
  path: string
  name?: string | symbol
  title: string
  query?: Record<string, any>
  params?: Record<string, any>
  affix?: boolean
  meta?: Record<string, any>
}

const route = useRoute()
const router = useRouter()
const appStore = useAppStore()

const scrollbarRef = ref()
const contentRef = ref()

// 访问过的视图
const visitedViews = ref<TagView[]>([])

// 右键菜单
const contextMenu = reactive({
  visible: false,
  top: 0,
  left: 0,
  selectedTag: {} as TagView
})

// 固定标签（不可关闭）
const affixTags = ref<TagView[]>([])

// 计算属性
const isActive = computed(() => (tag: TagView) => {
  return tag.path === route.path
})

const isAffix = computed(() => (tag: TagView) => {
  return tag.affix
})

// 添加标签
function addTag(tag?: RouteLocationNormalized) {
  const targetRoute = tag || route
  
  if (targetRoute.name && !targetRoute.meta?.noTagsView) {
    const newTag: TagView = {
      path: targetRoute.path,
      name: targetRoute.name,
      title: (targetRoute.meta?.title as string) || 'Unknown',
      query: targetRoute.query,
      params: targetRoute.params,
      affix: targetRoute.meta?.affix as boolean,
      meta: targetRoute.meta
    }

    // 检查是否已存在
    const existingIndex = visitedViews.value.findIndex(v => v.path === newTag.path)
    if (existingIndex > -1) {
      // 更新现有标签
      visitedViews.value[existingIndex] = newTag
    } else {
      // 添加新标签
      visitedViews.value.push(newTag)
    }

    // 添加到缓存
    if (newTag.name && newTag.meta?.keepAlive) {
      appStore.addCachedView(newTag.name as string)
    }
  }
}

// 关闭选中的标签
function closeSelectedTag(view: TagView) {
  const index = visitedViews.value.findIndex(v => v.path === view.path)
  if (index > -1) {
    visitedViews.value.splice(index, 1)
    
    // 从缓存中移除
    if (view.name) {
      appStore.removeCachedView(view.name as string)
    }

    // 如果关闭的是当前标签，跳转到最后一个标签
    if (isActive.value(view)) {
      toLastView()
    }
  }
}

// 关闭其他标签
function closeOthersTags() {
  const currentTag = visitedViews.value.find(v => v.path === route.path)
  visitedViews.value = visitedViews.value.filter(v => v.affix || v.path === route.path)
  
  // 清理缓存
  appStore.clearCachedViews()
  visitedViews.value.forEach(v => {
    if (v.name && v.meta?.keepAlive) {
      appStore.addCachedView(v.name as string)
    }
  })
}

// 关闭所有标签
function closeAllTags(view: TagView) {
  visitedViews.value = visitedViews.value.filter(v => v.affix)
  
  // 清理缓存
  appStore.clearCachedViews()
  visitedViews.value.forEach(v => {
    if (v.name && v.meta?.keepAlive) {
      appStore.addCachedView(v.name as string)
    }
  })

  if (affixTags.value.some(tag => tag.path === view.path)) {
    return
  }
  toLastView()
}

// 刷新选中的标签
function refreshSelectedTag(view: TagView) {
  if (view.name) {
    appStore.removeCachedView(view.name as string)
    nextTick(() => {
      if (view.name) {
        appStore.addCachedView(view.name as string)
      }
    })
  }

  if (view.path === route.path) {
    // 刷新当前页面
    router.replace({
      path: '/redirect' + view.path,
      query: view.query
    })
  }
}

// 跳转到最后一个视图
function toLastView() {
  const latestView = visitedViews.value[visitedViews.value.length - 1]
  if (latestView && latestView.path !== route.path) {
    router.push(latestView.path)
  } else {
    // 如果没有其他标签，跳转到首页
    if (latestView?.name === 'Dashboard') {
      router.replace({ path: '/redirect' + latestView.path })
    } else {
      router.push('/')
    }
  }
}

// 打开右键菜单
function openMenu(tag: TagView, e: MouseEvent) {
  const menuMinWidth = 105
  const offsetLeft = contentRef.value.getBoundingClientRect().left
  const offsetWidth = contentRef.value.offsetWidth
  const maxLeft = offsetWidth - menuMinWidth

  const left = e.clientX - offsetLeft + 15
  if (left > maxLeft) {
    contextMenu.left = maxLeft
  } else {
    contextMenu.left = left
  }

  contextMenu.top = e.clientY
  contextMenu.visible = true
  contextMenu.selectedTag = tag
}

// 关闭右键菜单
function closeMenu() {
  contextMenu.visible = false
}

// 初始化固定标签
function initAffixTags() {
  const routes = router.getRoutes()
  
  routes.forEach(route => {
    if (route.meta?.affix) {
      const tag: TagView = {
        path: route.path,
        name: route.name,
        title: (route.meta.title as string) || 'Unknown',
        affix: true,
        meta: route.meta
      }
      affixTags.value.push(tag)
      visitedViews.value.push(tag)
    }
  })
}

// 移动到当前标签
function moveToCurrentTag() {
  nextTick(() => {
    const tags = contentRef.value.querySelectorAll('.tags-view-item')
    const currentTag = tags[visitedViews.value.findIndex(v => v.path === route.path)]
    
    if (currentTag) {
      scrollbarRef.value?.setScrollLeft(currentTag.offsetLeft - 100)
    }
  })
}

// 监听路由变化
watch(route, () => {
  addTag()
  moveToCurrentTag()
})

// 监听点击事件，关闭右键菜单
watch(() => contextMenu.visible, (visible) => {
  if (visible) {
    document.body.addEventListener('click', closeMenu)
  } else {
    document.body.removeEventListener('click', closeMenu)
  }
})

onMounted(() => {
  initAffixTags()
  addTag()
})
</script>

<style lang="scss" scoped>
.tags-view-container {
  height: 34px;
  width: 100%;
  background: #fff;
  border-bottom: 1px solid #d8dce5;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.12), 0 0 3px 0 rgba(0, 0, 0, 0.04);

  .tags-view-wrapper {
    .tags-view-content {
      display: flex;
      align-items: center;
      height: 34px;
      padding: 0 15px;
      white-space: nowrap;
    }
  }

  .tags-view-item {
    display: inline-flex;
    align-items: center;
    position: relative;
    cursor: pointer;
    height: 26px;
    line-height: 26px;
    border: 1px solid #d8dce5;
    color: #495057;
    background: #fff;
    padding: 0 8px;
    font-size: 12px;
    margin-left: 5px;
    margin-top: 4px;
    text-decoration: none;
    border-radius: 3px;
    transition: all 0.3s;

    &:first-of-type {
      margin-left: 15px;
    }

    &:last-of-type {
      margin-right: 15px;
    }

    &.active {
      background-color: var(--el-color-primary);
      color: #fff;
      border-color: var(--el-color-primary);

      &::before {
        content: '';
        background: #fff;
        display: inline-block;
        width: 8px;
        height: 8px;
        border-radius: 50%;
        position: relative;
        margin-right: 4px;
      }
    }

    &:hover {
      color: var(--el-color-primary);
    }

    .tag-title {
      max-width: 120px;
      overflow: hidden;
      white-space: nowrap;
      text-overflow: ellipsis;
    }

    .tag-close {
      width: 16px;
      height: 16px;
      margin-left: 4px;
      border-radius: 50%;
      text-align: center;
      transition: all 0.3s;
      transform-origin: 100% 50%;

      &:hover {
        background-color: #b4bccc;
        color: #fff;
      }
    }
  }
}

.contextmenu {
  margin: 0;
  background: #fff;
  z-index: 3000;
  position: absolute;
  list-style-type: none;
  padding: 5px 0;
  border-radius: 4px;
  font-size: 12px;
  font-weight: 400;
  color: #333;
  box-shadow: 2px 2px 3px 0 rgba(0, 0, 0, 0.3);

  li {
    margin: 0;
    padding: 7px 16px;
    cursor: pointer;
    display: flex;
    align-items: center;

    &:hover {
      background: #eee;
    }

    .el-icon {
      margin-right: 8px;
      font-size: 14px;
    }
  }
}

// 暗色主题
html.dark {
  .tags-view-container {
    background: #141414;
    border-bottom-color: #303030;

    .tags-view-item {
      background: #262626;
      border-color: #303030;
      color: rgba(255, 255, 255, 0.85);

      &:hover {
        color: var(--el-color-primary);
      }

      &.active {
        background-color: var(--el-color-primary);
        color: #fff;
        border-color: var(--el-color-primary);
      }
    }
  }

  .contextmenu {
    background: #262626;
    color: rgba(255, 255, 255, 0.85);
    box-shadow: 2px 2px 3px 0 rgba(0, 0, 0, 0.5);

    li:hover {
      background: #303030;
    }
  }
}
</style>