<template>
  <view class="login-page">
    <view class="login-header">
      <image class="logo" src="/static/logo.png" mode="aspectFit" />
      <text class="title">AFAADMIN</text>
      <text class="subtitle">移动端管理系统</text>
    </view>

    <view class="login-form">
      <view class="form-item">
        <text class="label">用户名</text>
        <input
          v-model="form.userName"
          class="input"
          placeholder="请输入用户名"
          :maxlength="64"
        />
      </view>

      <view class="form-item">
        <text class="label">密码</text>
        <input
          v-model="form.password"
          class="input"
          placeholder="请输入密码"
          :password="!showPwd"
          :maxlength="32"
        />
        <text class="toggle-pwd" @tap="showPwd = !showPwd">
          {{ showPwd ? '隐藏' : '显示' }}
        </text>
      </view>

      <button class="login-btn" :loading="loading" @tap="handleLogin">
        登 录
      </button>
    </view>

    <view class="login-footer">
      <text class="text-secondary text-sm">v1.0.0 · Powered by AFAADMIN</text>
    </view>
  </view>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useUserStore } from '@/store/modules/user'

const userStore = useUserStore()
const loading = ref(false)
const showPwd = ref(false)

const form = reactive({
  userName: 'admin',
  password: 'admin123'
})

async function handleLogin() {
  if (!form.userName.trim()) {
    return uni.showToast({ title: '请输入用户名', icon: 'none' })
  }
  if (!form.password.trim()) {
    return uni.showToast({ title: '请输入密码', icon: 'none' })
  }

  loading.value = true
  try {
    await userStore.login(form)
    await userStore.fetchUserInfo()
    uni.showToast({ title: '登录成功', icon: 'success' })
    setTimeout(() => {
      uni.switchTab({ url: '/pages/workbench/index' })
    }, 500)
  } catch (e: any) {
    uni.showToast({ title: e.message || '登录失败', icon: 'none' })
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  padding: 0 60rpx;
  background: linear-gradient(180deg, #E8F3FF 0%, #F5F6F7 40%);
}

.login-header {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding-top: 180rpx;
  margin-bottom: 80rpx;
}

.logo {
  width: 120rpx;
  height: 120rpx;
  margin-bottom: 20rpx;
}

.title {
  font-size: 48rpx;
  font-weight: 700;
  color: #1D2129;
  letter-spacing: 4rpx;
}

.subtitle {
  font-size: 26rpx;
  color: #86909C;
  margin-top: 8rpx;
}

.login-form {
  background: #fff;
  border-radius: 24rpx;
  padding: 48rpx 36rpx;
  box-shadow: 0 8rpx 32rpx rgba(0, 0, 0, 0.06);
}

.form-item {
  margin-bottom: 36rpx;
  position: relative;
}

.label {
  display: block;
  font-size: 28rpx;
  color: #1D2129;
  font-weight: 500;
  margin-bottom: 12rpx;
}

.input {
  width: 100%;
  height: 88rpx;
  border: 2rpx solid #E5E6EB;
  border-radius: 12rpx;
  padding: 0 24rpx;
  font-size: 28rpx;
  box-sizing: border-box;
}

.input:focus {
  border-color: #165DFF;
}

.toggle-pwd {
  position: absolute;
  right: 24rpx;
  bottom: 24rpx;
  font-size: 24rpx;
  color: #165DFF;
}

.login-btn {
  width: 100%;
  height: 88rpx;
  line-height: 88rpx;
  background: #165DFF;
  color: #fff;
  font-size: 32rpx;
  font-weight: 600;
  border-radius: 12rpx;
  border: none;
  margin-top: 16rpx;
}

.login-btn[loading] {
  opacity: 0.7;
}

.login-footer {
  flex: 1;
  display: flex;
  align-items: flex-end;
  justify-content: center;
  padding-bottom: 60rpx;
}
</style>
