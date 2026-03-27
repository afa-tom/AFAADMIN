<template>
  <view class="container" v-if="user">
    <view class="card profile-header">
      <view class="big-avatar">
        <text class="big-avatar-text">{{ (user.nickName || user.userName).charAt(0) }}</text>
      </view>
      <text class="detail-name">{{ user.nickName || user.userName }}</text>
      <view class="status-tag" :class="user.status === 1 ? 'active' : 'inactive'">
        {{ user.status === 1 ? '正常' : '停用' }}
      </view>
    </view>

    <view class="card">
      <view class="info-row">
        <text class="info-label">用户名</text>
        <text class="info-value">{{ user.userName }}</text>
      </view>
      <view class="info-row">
        <text class="info-label">昵称</text>
        <text class="info-value">{{ user.nickName || '-' }}</text>
      </view>
      <view class="info-row">
        <text class="info-label">部门</text>
        <text class="info-value">{{ user.deptName || '-' }}</text>
      </view>
      <view class="info-row">
        <text class="info-label">手机号</text>
        <text class="info-value">{{ user.phone || '-' }}</text>
      </view>
      <view class="info-row">
        <text class="info-label">邮箱</text>
        <text class="info-value">{{ user.email || '-' }}</text>
      </view>
      <view class="info-row">
        <text class="info-label">创建时间</text>
        <text class="info-value">{{ user.createTime }}</text>
      </view>
      <view class="info-row" v-if="user.remark">
        <text class="info-label">备注</text>
        <text class="info-value">{{ user.remark }}</text>
      </view>
    </view>

    <!-- 操作按钮 -->
    <view class="action-row" v-if="userStore.hasPermission('sys:user:edit')">
      <button class="action-btn" @tap="handleToggleStatus">
        {{ user.status === 1 ? '停用账号' : '启用账号' }}
      </button>
      <button class="action-btn danger" @tap="handleResetPwd" v-if="userStore.hasPermission('sys:user:resetpwd')">
        重置密码
      </button>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { onLoad } from '@dcloudio/uni-app'
import { getUserById, updateUser, resetPassword } from '@/api/user'
import { useUserStore } from '@/store/modules/user'
import { showConfirm, showToast } from '@/utils/index'
import type { UserItem } from '@/api/types'

const userStore = useUserStore()
const user = ref<UserItem | null>(null)
let userId = 0

onLoad((query) => {
  userId = Number(query?.id || 0)
  loadDetail()
})

async function loadDetail() {
  if (!userId) return
  const result = await getUserById(userId)
  user.value = result.data
}

async function handleToggleStatus() {
  if (!user.value) return
  const newStatus = user.value.status === 1 ? 0 : 1
  const confirmText = newStatus === 0 ? '确定停用该账号？' : '确定启用该账号？'
  const confirmed = await showConfirm(confirmText)
  if (!confirmed) return

  await updateUser({
    id: user.value.id,
    status: newStatus,
    roleIds: user.value.roleIds
  })
  showToast('操作成功')
  loadDetail()
}

async function handleResetPwd() {
  const confirmed = await showConfirm('确定将密码重置为 123456 ？')
  if (!confirmed) return

  await resetPassword({ userId, newPassword: '123456' })
  showToast('密码已重置为 123456')
}
</script>

<style scoped>
.profile-header {
  display: flex; flex-direction: column; align-items: center;
  padding: 48rpx 24rpx;
}
.big-avatar {
  width: 128rpx; height: 128rpx;
  border-radius: 50%; background: #E8F3FF;
  display: flex; align-items: center; justify-content: center;
  margin-bottom: 16rpx;
}
.big-avatar-text { color: #165DFF; font-size: 52rpx; font-weight: 700; }
.detail-name { font-size: 36rpx; font-weight: 600; margin-bottom: 12rpx; }
.status-tag {
  font-size: 24rpx; padding: 4rpx 20rpx; border-radius: 8rpx;
}
.status-tag.active { background: #E8FFEA; color: #00B42A; }
.status-tag.inactive { background: #FFECE8; color: #F53F3F; }

.info-row {
  display: flex; justify-content: space-between;
  padding: 24rpx 0; border-bottom: 1rpx solid #F2F3F5;
}
.info-row:last-child { border-bottom: none; }
.info-label { color: #86909C; font-size: 28rpx; }
.info-value { color: #1D2129; font-size: 28rpx; }

.action-row {
  display: flex; gap: 24rpx; padding: 0 24rpx; margin-top: 32rpx;
}
.action-btn {
  flex: 1; height: 80rpx; line-height: 80rpx;
  background: #F2F3F5; color: #1D2129;
  font-size: 28rpx; border-radius: 12rpx; border: none;
}
.action-btn.danger { background: #FFECE8; color: #F53F3F; }
</style>
