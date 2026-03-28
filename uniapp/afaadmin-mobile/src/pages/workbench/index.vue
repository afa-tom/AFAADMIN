<template>
  <view class="workbench">
    <!-- 顶部问候 -->
    <view class="header">
      <view class="header-bg" />
      <view class="header-content" :style="{ paddingTop: statusBarHeight + 'px' }">
        <view class="greeting">
          <text class="greeting-text">{{ greetingText }}</text>
          <text class="user-name">{{ userStore.nickName || userStore.userName }}</text>
        </view>
        <view class="avatar" @tap="navigateTo('/pages/profile/index')">
          <text class="avatar-text">{{ (userStore.nickName || 'U').charAt(0) }}</text>
        </view>
      </view>
    </view>

    <!-- AI 助手入口 -->
    <view class="section">
      <view class="ai-entry card" @tap="navigateTo('/pages/ai/index')">
        <view class="ai-icon">🤖</view>
        <view class="ai-info">
          <text class="ai-title">AI 助手</text>
          <text class="ai-desc text-secondary text-sm">问我任何关于系统的问题</text>
        </view>
        <text class="arrow">›</text>
      </view>
    </view>

    <!-- 快捷操作 -->
    <view class="section">
      <view class="section-title">快捷操作</view>
      <view class="grid-menu">
        <view
          v-for="item in menuItems"
          :key="item.path"
          class="grid-item"
          @tap="navigateTo(item.path)"
        >
          <view class="grid-icon" :style="{ background: item.bgColor }">
            <text class="icon-text">{{ item.icon }}</text>
          </view>
          <text class="grid-label">{{ item.label }}</text>
        </view>
      </view>
    </view>

    <!-- 系统概览 -->
    <view class="section">
      <view class="section-title">系统概览</view>
      <view class="stat-row">
        <view v-for="stat in statList" :key="stat.label" class="stat-card">
          <text class="stat-value" :style="{ color: stat.color }">{{ stat.value }}</text>
          <text class="stat-label">{{ stat.label }}</text>
        </view>
      </view>
    </view>

    <!-- 公告提示 -->
    <view class="section">
      <view class="section-title">系统公告</view>
      <view class="card">
        <view class="notice-item">
          <text class="notice-tag">通知</text>
          <text class="notice-text">AFAADMIN v1.0.0 AI 助手已上线</text>
        </view>
        <view class="notice-item">
          <text class="notice-tag warn">提醒</text>
          <text class="notice-text">请定期修改密码以确保账户安全</text>
        </view>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useUserStore } from '@/store/modules/user'
import { getStatusBarHeight } from '@/utils/platform'

const userStore = useUserStore()
const statusBarHeight = ref(getStatusBarHeight())

const greetingText = computed(() => {
  const h = new Date().getHours()
  if (h < 6) return '凌晨好'
  if (h < 12) return '上午好'
  if (h < 14) return '中午好'
  if (h < 18) return '下午好'
  return '晚上好'
})

const menuItems = [
  { label: '用户管理', icon: '👤', path: '/pages/user/index', bgColor: '#E8F3FF' },
  { label: '角色管理', icon: '🛡️', path: '/pages/role/index', bgColor: '#E8FFFB' },
  { label: '部门管理', icon: '🏢', path: '/pages/dept/index', bgColor: '#FFF7E8' },
  { label: '字典管理', icon: '📖', path: '/pages/dict/index', bgColor: '#FFE8E8' },
]

const statList = ref([
  { label: '用户数', value: '--', color: '#165DFF' },
  { label: '角色数', value: '--', color: '#0FC6C2' },
  { label: '部门数', value: '--', color: '#FF7D00' },
])

function navigateTo(path: string) {
  if (path === '/pages/profile/index') {
    uni.switchTab({ url: path })
  } else {
    uni.navigateTo({ url: path })
  }
}
</script>

<style scoped>
.workbench { padding-bottom: 120rpx; }

.header { position: relative; overflow: hidden; }
.header-bg {
  position: absolute; top: 0; left: 0; right: 0; height: 400rpx;
  background: linear-gradient(135deg, #165DFF 0%, #4080FF 100%);
  border-radius: 0 0 40rpx 40rpx;
}
.header-content {
  position: relative; padding: 24rpx 32rpx 40rpx;
  display: flex; justify-content: space-between; align-items: center;
}
.greeting-text { color: rgba(255,255,255,0.8); font-size: 28rpx; }
.user-name { display: block; color: #fff; font-size: 40rpx; font-weight: 700; margin-top: 8rpx; }
.avatar {
  width: 80rpx; height: 80rpx; border-radius: 50%;
  background: rgba(255,255,255,0.25);
  display: flex; align-items: center; justify-content: center;
}
.avatar-text { color: #fff; font-size: 36rpx; font-weight: 600; }

.section { padding: 0 24rpx; margin-top: 32rpx; }
.section-title { font-size: 32rpx; font-weight: 600; color: #1D2129; margin-bottom: 20rpx; }

/* AI 入口 */
.ai-entry {
  display: flex; align-items: center; gap: 20rpx; padding: 28rpx 24rpx;
  background: linear-gradient(135deg, #F0E8FF 0%, #E8F3FF 100%);
  border: none;
}
.ai-icon { font-size: 48rpx; }
.ai-info { flex: 1; }
.ai-title { font-size: 32rpx; font-weight: 600; color: #1D2129; display: block; }
.ai-desc { display: block; margin-top: 4rpx; }
.arrow { font-size: 36rpx; color: #C9CDD4; }

.grid-menu {
  display: flex; flex-wrap: wrap; background: #fff;
  border-radius: 16rpx; padding: 24rpx 0;
}
.grid-item { width: 25%; display: flex; flex-direction: column; align-items: center; padding: 16rpx 0; }
.grid-icon {
  width: 88rpx; height: 88rpx; border-radius: 20rpx;
  display: flex; align-items: center; justify-content: center; margin-bottom: 12rpx;
}
.icon-text { font-size: 36rpx; }
.grid-label { font-size: 24rpx; color: #4E5969; }

.stat-row { display: flex; gap: 16rpx; }
.stat-card { flex: 1; background: #fff; border-radius: 16rpx; padding: 24rpx; text-align: center; }
.stat-value { font-size: 44rpx; font-weight: 700; }
.stat-label { display: block; font-size: 24rpx; color: #86909C; margin-top: 8rpx; }

.notice-item {
  display: flex; align-items: center; padding: 16rpx 0;
  border-bottom: 1rpx solid #F2F3F5;
}
.notice-item:last-child { border-bottom: none; }
.notice-tag {
  background: #E8F3FF; color: #165DFF; font-size: 22rpx;
  padding: 4rpx 12rpx; border-radius: 6rpx; margin-right: 16rpx; flex-shrink: 0;
}
.notice-tag.warn { background: #FFF7E8; color: #FF7D00; }
.notice-text { font-size: 26rpx; color: #4E5969; }
</style>
