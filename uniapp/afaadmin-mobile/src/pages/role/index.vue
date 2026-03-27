<template>
  <view class="container">
    <scroll-view scroll-y class="list-wrap" refresher-enabled
      :refresher-triggered="refreshing" @refresherrefresh="onRefresh">
      <view v-for="item in roleList" :key="item.id" class="card role-card">
        <view class="flex-between">
          <view>
            <text class="role-name">{{ item.roleName }}</text>
            <text class="role-code text-secondary text-sm">{{ item.roleCode }}</text>
          </view>
          <view class="status-tag" :class="item.status === 1 ? 'active' : 'inactive'">
            {{ item.status === 1 ? '正常' : '停用' }}
          </view>
        </view>
        <view class="role-meta mt-16 text-secondary text-sm">
          <text>排序: {{ item.sort }}</text>
          <text style="margin-left: 32rpx;">菜单数: {{ item.menuIds?.length || 0 }}</text>
        </view>
        <view v-if="item.remark" class="role-remark mt-16 text-secondary text-sm">
          {{ item.remark }}
        </view>
      </view>

      <view v-if="roleList.length === 0 && !loading" class="empty-tip">暂无数据</view>
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getRoleList } from '@/api/role'
import type { RoleItem } from '@/api/types'

const roleList = ref<RoleItem[]>([])
const loading = ref(false)
const refreshing = ref(false)

onMounted(() => loadData())

async function loadData() {
  loading.value = true
  try {
    const result = await getRoleList()
    roleList.value = result.data
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function onRefresh() { refreshing.value = true; loadData() }
</script>

<style scoped>
.list-wrap { height: calc(100vh - 100rpx); }
.role-card { padding: 28rpx; }
.role-name { font-size: 30rpx; font-weight: 600; color: #1D2129; }
.role-code { display: block; margin-top: 4rpx; }
.status-tag { font-size: 22rpx; padding: 4rpx 16rpx; border-radius: 8rpx; }
.status-tag.active { background: #E8FFEA; color: #00B42A; }
.status-tag.inactive { background: #FFECE8; color: #F53F3F; }
.empty-tip { text-align: center; padding: 48rpx 0; color: #86909C; }
</style>
