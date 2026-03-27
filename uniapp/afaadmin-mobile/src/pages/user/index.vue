<template>
  <view class="container">
    <!-- 搜索栏 -->
    <view class="search-bar card">
      <input
        v-model="searchKey"
        class="search-input"
        placeholder="搜索用户名"
        confirm-type="search"
        @confirm="handleSearch"
      />
      <text class="search-btn" @tap="handleSearch">搜索</text>
    </view>

    <!-- 列表 -->
    <scroll-view
      scroll-y
      class="list-wrap"
      refresher-enabled
      :refresher-triggered="refreshing"
      @refresherrefresh="onRefresh"
      @scrolltolower="onLoadMore"
    >
      <view v-for="item in userList" :key="item.id" class="card user-card" @tap="goDetail(item.id)">
        <view class="flex-between">
          <view class="flex-row">
            <view class="user-avatar">
              <text class="avatar-char">{{ (item.nickName || item.userName).charAt(0) }}</text>
            </view>
            <view>
              <text class="user-name">{{ item.nickName || item.userName }}</text>
              <text class="user-account text-secondary text-sm">{{ item.userName }}</text>
            </view>
          </view>
          <view class="status-tag" :class="item.status === 1 ? 'active' : 'inactive'">
            {{ item.status === 1 ? '正常' : '停用' }}
          </view>
        </view>
        <view class="user-info mt-16">
          <text class="info-item" v-if="item.deptName">🏢 {{ item.deptName }}</text>
          <text class="info-item" v-if="item.phone">📱 {{ item.phone }}</text>
        </view>
      </view>

      <view v-if="userList.length === 0 && !loading" class="empty-tip">
        <text>暂无数据</text>
      </view>

      <view v-if="noMore && userList.length > 0" class="load-tip text-secondary text-sm">
        已加载全部
      </view>

      <view v-if="loading" class="load-tip text-secondary text-sm">
        加载中...
      </view>
    </scroll-view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getUserPage } from '@/api/user'
import type { UserItem } from '@/api/types'

const userList = ref<UserItem[]>([])
const loading = ref(false)
const refreshing = ref(false)
const noMore = ref(false)
const searchKey = ref('')
const pageIndex = ref(1)
const pageSize = 15

onMounted(() => loadData())

async function loadData(append = false) {
  if (loading.value) return
  loading.value = true
  try {
    const result = await getUserPage({
      userName: searchKey.value || undefined,
      pageIndex: pageIndex.value,
      pageSize
    })
    const items = result.data.items
    if (append) {
      userList.value.push(...items)
    } else {
      userList.value = items
    }
    noMore.value = userList.value.length >= result.data.totalCount
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

function handleSearch() {
  pageIndex.value = 1
  noMore.value = false
  loadData()
}

function onRefresh() {
  refreshing.value = true
  pageIndex.value = 1
  noMore.value = false
  loadData()
}

function onLoadMore() {
  if (noMore.value || loading.value) return
  pageIndex.value++
  loadData(true)
}

function goDetail(id: number) {
  uni.navigateTo({ url: `/pages/user/detail?id=${id}` })
}
</script>

<style scoped>
.list-wrap { height: calc(100vh - 200rpx); }

.search-bar {
  display: flex; align-items: center; gap: 16rpx;
}
.search-input {
  flex: 1; height: 72rpx;
  background: #F5F6F7; border-radius: 12rpx;
  padding: 0 24rpx; font-size: 28rpx;
}
.search-btn {
  color: #165DFF; font-size: 28rpx; font-weight: 500;
  flex-shrink: 0;
}

.user-card { padding: 28rpx; }
.user-avatar {
  width: 76rpx; height: 76rpx;
  border-radius: 50%; background: #E8F3FF;
  display: flex; align-items: center; justify-content: center;
  margin-right: 20rpx;
}
.avatar-char { color: #165DFF; font-size: 32rpx; font-weight: 600; }
.user-name { font-size: 30rpx; font-weight: 600; color: #1D2129; }
.user-account { display: block; margin-top: 4rpx; }

.status-tag {
  font-size: 22rpx; padding: 4rpx 16rpx;
  border-radius: 8rpx;
}
.status-tag.active { background: #E8FFEA; color: #00B42A; }
.status-tag.inactive { background: #FFECE8; color: #F53F3F; }

.user-info { display: flex; flex-wrap: wrap; gap: 24rpx; }
.info-item { font-size: 24rpx; color: #86909C; }

.empty-tip, .load-tip {
  text-align: center; padding: 48rpx 0;
}
</style>
