<template>
  <view class="container">
    <view class="card">
      <view class="form-item">
        <text class="label">原密码</text>
        <input v-model="form.oldPassword" class="input" password placeholder="请输入原密码" />
      </view>
      <view class="form-item">
        <text class="label">新密码</text>
        <input v-model="form.newPassword" class="input" password placeholder="请输入新密码（至少6位）" />
      </view>
      <view class="form-item">
        <text class="label">确认密码</text>
        <input v-model="form.confirmPassword" class="input" password placeholder="请再次输入新密码" />
      </view>
    </view>

    <button class="submit-btn" :loading="loading" @tap="handleSubmit">确认修改</button>
  </view>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useUserStore } from '@/store/modules/user'
import { resetPassword } from '@/api/user'
import { showToast } from '@/utils/index'

const userStore = useUserStore()
const loading = ref(false)

const form = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

async function handleSubmit() {
  if (!form.oldPassword) return showToast('请输入原密码')
  if (!form.newPassword) return showToast('请输入新密码')
  if (form.newPassword.length < 6) return showToast('新密码至少6位')
  if (form.newPassword !== form.confirmPassword) return showToast('两次密码不一致')

  loading.value = true
  try {
    await resetPassword({ userId: userStore.id, newPassword: form.newPassword })
    showToast('密码修改成功，请重新登录')
    setTimeout(async () => {
      await userStore.logout()
      uni.reLaunch({ url: '/pages/login/index' })
    }, 1500)
  } catch (e: any) {
    showToast(e.message || '修改失败')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.form-item { margin-bottom: 32rpx; }
.label {
  display: block; font-size: 28rpx; color: #1D2129;
  font-weight: 500; margin-bottom: 12rpx;
}
.input {
  width: 100%; height: 88rpx;
  border: 2rpx solid #E5E6EB; border-radius: 12rpx;
  padding: 0 24rpx; font-size: 28rpx; box-sizing: border-box;
}
.submit-btn {
  margin-top: 48rpx;
  height: 88rpx; line-height: 88rpx;
  background: #165DFF; color: #fff;
  font-size: 30rpx; border-radius: 12rpx; border: none;
}
</style>
