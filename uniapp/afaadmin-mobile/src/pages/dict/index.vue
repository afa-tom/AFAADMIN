<template>
  <view class="container">
    <!-- 字典类型列表 -->
    <view v-if="!selectedType" class="section-label mb-16">字典类型</view>
    <view v-if="!selectedType">
      <scroll-view scroll-y class="list-wrap" refresher-enabled
        :refresher-triggered="refreshing" @refresherrefresh="onRefresh">
        <view v-for="item in typeList" :key="item.id" class="card type-card" @tap="selectType(item)">
          <view class="flex-between">
            <view>
              <text class="type-name">{{ item.dictName }}</text>
              <text class="type-code text-secondary text-sm">{{ item.dictCode }}</text>
            </view>
            <text class="arrow">›</text>
          </view>
        </view>
        <view v-if="typeList.length === 0 && !loading" class="empty-tip">暂无数据</view>
      </scroll-view>
    </view>

    <!-- 字典数据列表 -->
    <view v-else>
      <view class="back-bar card" @tap="selectedType = null">
        <text class="text-primary">‹ 返回</text>
        <text class="type-title">{{ selectedType.dictName }}</text>
      </view>
      <scroll-view scroll-y class="list-wrap-data">
        <view v-for="item in dataList" :key="item.id" class="card data-card">
          <view class="flex-between">
            <view>
              <text class="data-label">{{ item.dictLabel }}</text>
              <text class="data-value text-secondary text-sm">值: {{ item.dictValue }}</text>
            </view>
            <view class="status-tag" :class="item.status === 1 ? 'active' : 'inactive'">
              {{ item.status === 1 ? '正常' : '停用' }}
            </view>
          </view>
        </view>
        <view v-if="dataList.length === 0 && !dataLoading" class="empty-tip">暂无数据</view>
      </scroll-view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { getDictTypeList, getDictDataByTypeId } from '@/api/dict'
import type { DictTypeItem, DictDataItem } from '@/api/types'

const typeList = ref<DictTypeItem[]>([])
const dataList = ref<DictDataItem[]>([])
const selectedType = ref<DictTypeItem | null>(null)
const loading = ref(false)
const dataLoading = ref(false)
const refreshing = ref(false)

onMounted(() => loadTypes())

async function loadTypes() {
  loading.value = true
  try {
    const result = await getDictTypeList()
    typeList.value = result.data
  } finally {
    loading.value = false
    refreshing.value = false
  }
}

async function selectType(item: DictTypeItem) {
  selectedType.value = item
  dataLoading.value = true
  try {
    const result = await getDictDataByTypeId(item.id)
    dataList.value = result.data
  } finally { dataLoading.value = false }
}

function onRefresh() { refreshing.value = true; loadTypes() }
</script>

<style scoped>
.section-label { font-size: 28rpx; color: #86909C; }
.list-wrap { height: calc(100vh - 200rpx); }
.list-wrap-data { height: calc(100vh - 260rpx); }
.type-card { padding: 28rpx; }
.type-name { font-size: 30rpx; font-weight: 500; color: #1D2129; }
.type-code { display: block; margin-top: 4rpx; }
.arrow { font-size: 36rpx; color: #C9CDD4; }

.back-bar {
  display: flex; align-items: center; gap: 16rpx;
}
.type-title { font-size: 30rpx; font-weight: 600; }

.data-card { padding: 24rpx; }
.data-label { font-size: 28rpx; font-weight: 500; color: #1D2129; }
.data-value { display: block; margin-top: 4rpx; }
.status-tag { font-size: 22rpx; padding: 4rpx 16rpx; border-radius: 8rpx; }
.status-tag.active { background: #E8FFEA; color: #00B42A; }
.status-tag.inactive { background: #FFECE8; color: #F53F3F; }
.empty-tip { text-align: center; padding: 48rpx 0; color: #86909C; }
</style>
