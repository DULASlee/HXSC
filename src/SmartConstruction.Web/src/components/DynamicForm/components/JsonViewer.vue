<template>
  <div class="json-viewer">
    <div v-if="isObject(value)" class="json-object">
      <div
        v-for="(val, key) in value"
        :key="key"
        class="json-item"
        :class="{ 'json-item--expandable': isExpandable(val) }"
      >
        <div class="json-key-line" @click="toggleExpand(key)">
          <el-icon
            v-if="isExpandable(val)"
            class="expand-icon"
            :class="{ 'expanded': expandedKeys.has(key) }"
          >
            <CaretRight />
          </el-icon>
          <span class="json-key">{{ key }}:</span>
          <span v-if="!isExpandable(val) || !expandedKeys.has(key)" class="json-value">
            {{ formatValue(val) }}
          </span>
        </div>
        <div
          v-if="isExpandable(val) && expandedKeys.has(key)"
          class="json-nested"
        >
          <json-viewer
            :value="val"
            :expand-depth="expandDepth - 1"
            :level="level + 1"
          />
        </div>
      </div>
    </div>
    
    <div v-else-if="isArray(value)" class="json-array">
      <div
        v-for="(item, index) in value"
        :key="index"
        class="json-item"
        :class="{ 'json-item--expandable': isExpandable(item) }"
      >
        <div class="json-key-line" @click="toggleExpand(index)">
          <el-icon
            v-if="isExpandable(item)"
            class="expand-icon"
            :class="{ 'expanded': expandedKeys.has(index) }"
          >
            <CaretRight />
          </el-icon>
          <span class="json-key">[{{ index }}]:</span>
          <span v-if="!isExpandable(item) || !expandedKeys.has(index)" class="json-value">
            {{ formatValue(item) }}
          </span>
        </div>
        <div
          v-if="isExpandable(item) && expandedKeys.has(index)"
          class="json-nested"
        >
          <json-viewer
            :value="item"
            :expand-depth="expandDepth - 1"
            :level="level + 1"
          />
        </div>
      </div>
    </div>
    
    <div v-else class="json-primitive">
      <span class="json-value">{{ formatValue(value) }}</span>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { CaretRight } from '@element-plus/icons-vue'

interface Props {
  value: any
  expandDepth?: number
  level?: number
}

const props = withDefaults(defineProps<Props>(), {
  expandDepth: 2,
  level: 0
})

const expandedKeys = ref(new Set<string | number>())

// 判断是否为对象
function isObject(val: any): boolean {
  return val !== null && typeof val === 'object' && !Array.isArray(val)
}

// 判断是否为数组
function isArray(val: any): boolean {
  return Array.isArray(val)
}

// 判断是否可展开
function isExpandable(val: any): boolean {
  return isObject(val) || isArray(val)
}

// 格式化值
function formatValue(val: any): string {
  if (val === null) return 'null'
  if (val === undefined) return 'undefined'
  if (typeof val === 'string') return `"${val}"`
  if (typeof val === 'boolean') return val.toString()
  if (typeof val === 'number') return val.toString()
  if (isArray(val)) return `Array(${val.length})`
  if (isObject(val)) return `Object(${Object.keys(val).length})`
  return String(val)
}

// 切换展开状态
function toggleExpand(key: string | number) {
  if (expandedKeys.value.has(key)) {
    expandedKeys.value.delete(key)
  } else {
    expandedKeys.value.add(key)
  }
}

// 获取值类型对应的CSS类
function getValueTypeClass(val: any): string {
  if (val === null) return 'json-null'
  if (val === undefined) return 'json-undefined'
  if (typeof val === 'string') return 'json-string'
  if (typeof val === 'boolean') return 'json-boolean'
  if (typeof val === 'number') return 'json-number'
  return 'json-default'
}

// 初始化展开状态
onMounted(() => {
  if (props.expandDepth > 0) {
    if (isObject(props.value)) {
      Object.keys(props.value).forEach(key => {
        if (isExpandable(props.value[key])) {
          expandedKeys.value.add(key)
        }
      })
    } else if (isArray(props.value)) {
      props.value.forEach((item: any, index: number) => {
        if (isExpandable(item)) {
          expandedKeys.value.add(index)
        }
      })
    }
  }
})
</script>

<style lang="scss" scoped>
.json-viewer {
  font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
  font-size: 13px;
  line-height: 1.4;

  .json-item {
    margin: 2px 0;

    &.json-item--expandable {
      .json-key-line {
        cursor: pointer;
        
        &:hover {
          background-color: var(--el-fill-color-light);
        }
      }
    }
  }

  .json-key-line {
    display: flex;
    align-items: center;
    padding: 2px 4px;
    border-radius: 2px;

    .expand-icon {
      margin-right: 4px;
      font-size: 12px;
      transition: transform 0.2s;
      color: var(--el-text-color-secondary);

      &.expanded {
        transform: rotate(90deg);
      }
    }
  }

  .json-key {
    color: var(--el-color-primary);
    font-weight: 500;
    margin-right: 8px;
  }

  .json-value {
    color: var(--el-text-color-regular);

    &.json-string {
      color: var(--el-color-success);
    }

    &.json-number {
      color: var(--el-color-warning);
    }

    &.json-boolean {
      color: var(--el-color-info);
    }

    &.json-null,
    &.json-undefined {
      color: var(--el-text-color-secondary);
      font-style: italic;
    }
  }

  .json-nested {
    margin-left: 20px;
    border-left: 1px solid var(--el-border-color-lighter);
    padding-left: 8px;
  }

  .json-primitive {
    .json-value {
      font-weight: 500;
    }
  }
}
</style>