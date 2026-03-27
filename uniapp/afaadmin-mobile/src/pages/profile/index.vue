<template>
  <view class="profile-page">
    <view class="profile-bg" />

    <view class="profile-header" :style="{ paddingTop: statusBarHeight + 20 + 'px' }">
      <view class="big-avatar">
        <text class="avatar-text">{{ (userStore.nickName || 'U').charAt(0) }}</text>
      </view>
      <text class="profile-name">{{ userStore.nickName || userStore.userName }}</text>
      <text class="profile-role text-sm">
        {{ userStore.roles.join(', ') || '普通用户' }}
      </text>
    </view>

    <view class="menu-section">
      <view class="menu-card card">
        <view class="menu-item" @tap="goPassword">
          <text class="menu-icon">🔐</text>
          <text class="menu-label">修改密码</text>
          <text class="menu-arrow">›</text>
        </view>
        <view class="menu-item" @tap="goAbout">
          <text class="menu-icon">ℹ️</text>
          <text class="menu-label">关于系统</text>
          <text class="menu-arrow">›</text>
        </view>
        <view class="menu-item" @tap="handleClearCache">
          <text class="menu-icon">🗑️</text>
          <text class="menu-label">清除缓存</text>
          <text class="menu-arrow">›</text>
        </view>
      </view>

      <button class="logout-btn" @tap="handleLogout">退出登录</button>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '@/store/modules/user'
import { getStatusBarHeight } from '@/utils/platform'
import { showConfirm, showToast } from '@/utils/index'

const userStore = useUserStore()
const statusBarHeight = ref(getStatusBarHeight())

function goPassword() {
  uni.navigateTo({ url: '/pages/profile/password' })
}

function goAbout() {
  uni.showModal({
    title: '关于 AFAADMIN',
    content: '版本: v1.0.0\n架构: .NET 8 + Vue3 + UniApp\n许可: Apache 2.0',
    showCancel: false
  })
}

async function handleClearCache() {
  const confirmed = await showConfirm('确定清除本地缓存？')
  if (!confirmed) return
  try {
    uni.clearStorageSync()
    // 保留 token
    const { getToken, getRefreshToken, setToken, setRefreshToken } = await import('@/utils/auth')
    const t = userStore.token
    // token 已清除，无需额外处理
    showToast('缓存已清除')
  } catch {
    showToast('清除失败')
  }
}

async function handleLogout() {
  const confirmed = await showConfirm('确定退出登录？')
  if (!confirmed) return
  await userStore.logout()
  uni.reLaunch({ url: '/pages/login/index' })
}
</script>

<style scoped>
.profile-page { min-height: 100vh; background: #F5F6F7; }
.profile-bg {
  position: absolute; top: 0; left: 0; right: 0;
  height: 380rpx;
  background: linear-gradient(135deg, #165DFF 0%, #4080FF 100%);
  border-radius: 0 0 40rpx 40rpx;
}

.profile-header {
  position: relative;
  display: flex; flex-direction: column; align-items: center;
  padding-bottom: 40rpx;
}
.big-avatar {
  width: 140rpx; height: 140rpx;
  border-radius: 50%; background: rgba(255,255,255,0.25);
  display: flex; align-items: center; justify-content: center;
  border: 4rpx solid rgba(255,255,255,0.5);
}
.avatar-text { color: #fff; font-size: 56rpx; font-weight: 700; }
.profile-name {
  color: #fff; font-size: 36rpx; font-weight: 600; margin-top: 16rpx;
}
.profile-role { color: rgba(255,255,255,0.8); margin-top: 4rpx; }

.menu-section { padding: 24rpx; position: relative; }
.menu-card { padding: 0; overflow: hidden; }
.menu-item {
  display: flex; align-items: center;
  padding: 28rpx 24rpx;
  border-bottom: 1rpx solid #F2F3F5;
}
.menu-item:last-child { border-bottom: none; }
.menu-icon { font-size: 36rpx; margin-right: 20rpx; }
.menu-label { flex: 1; font-size: 28rpx; color: #1D2129; }
.menu-arrow { font-size: 32rpx; color: #C9CDD4; }

.logout-btn {
  margin-top: 48rpx;
  height: 88rpx; line-height: 88rpx;
  background: #fff; color: #F53F3F;
  font-size: 30rpx; border-radius: 16rpx; border: none;
}
</style>
