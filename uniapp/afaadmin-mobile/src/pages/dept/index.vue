<template>
  <view class="container">
    <scroll-view scroll-y class="list-wrap" refresher-enabled
      :refresher-triggered="refreshing" @refresherrefresh="onRefresh">
      <view v-for="item in flatList" :key="item.id" class="card dept-card"
        :style="{ marginLeft: item.level * 32 + 'rpx' }">
        <view class="flex-between">
          <view class="flex-row">
            <text v-if="item.hasChildren" class="expand-icon" @tap="toggleExpand(item.id)">
              {{ expandedIds.has(item.id) ? '▼' : '▶' }}
            </text>
            <text v-else class="expand-icon placeholder">·</text>
            <text class="dept-name">{{ item.deptName }}</text>
          </view>
          <view class="status-tag" :class="item.status === 1 ? 'active' : 'inactive'">
            {{ item.status === 1 ? '正常' : '停用' }}
          </view>
        </view>
        <view class="dept-meta mt-16 text-secondary text-sm" v-if="item.leader || item.phone">
          <text v-if="item.leader">负责人: {{ item.leader }}</text>
          <text v-if="item.phone" style="margin-left: 24rpx;">{{ item.phone }}</text>
        </view>
      </view>

      <view v-if="flatList.length === 0 && !loading" class="empty-tip">暂无数据</view>
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { getDeptTree } from '@/api/dept'
import type { DeptItem } from '@/api/types'

interface FlatDept extends DeptItem {
  level: number
  hasChildren: boolean
}

const deptTree = ref<DeptItem[]>([])
const loading = ref(false)
const refreshing = ref(false)
const expandedIds = ref(new Set<number>())

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const result = await getDeptTree()
    deptTree.value = result.data
    // 默认展开第一级
    deptTree.value.forEach(d => expandedIds.value.add(d.id))
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function onRefresh() { refreshing.value = true; loadData() }

function toggleExpand(id: number) {
  if (expandedIds.value.has(id)) expandedIds.value.delete(id)
  else expandedIds.value.add(id)
}

const flatList = computed(() => {
  const result: FlatDept[] = []
  function walk(items: DeptItem[], level: number) {
    for (const item of items) {
      const hasChildren = !!(item.children && item.children.length > 0)
      result.push({ ...item, level, hasChildren })
      if (hasChildren && expandedIds.value.has(item.id)) {
        walk(item.children!, level + 1)
      }
    }
  }
  walk(deptTree.value, 0)
  return result
})
</script>

<style scoped>
.list-wrap { height: calc(100vh - 100rpx); }
.dept-card { padding: 24rpx; }
.expand-icon {
  font-size: 20rpx; color: #86909C; margin-right: 12rpx; width: 28rpx; text-align: center;
}
.expand-icon.placeholder { color: #C9CDD4; }
.dept-name { font-size: 30rpx; font-weight: 500; color: #1D2129; }
.status-tag { font-size: 22rpx; padding: 4rpx 16rpx; border-radius: 8rpx; }
.status-tag.active { background: #E8FFEA; color: #00B42A; }
.status-tag.inactive { background: #FFECE8; color: #F53F3F; }
.empty-tip { text-align: center; padding: 48rpx 0; color: #86909C; }
</style>
