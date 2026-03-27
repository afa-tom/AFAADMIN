<script setup lang="ts">
import { onLaunch } from '@dcloudio/uni-app'
import { useUserStore } from '@/store/modules/user'

onLaunch(async () => {
  console.log('App Launch')
  const userStore = useUserStore()
  // 如果有 token，尝试获取用户信息
  if (userStore.token) {
    try {
      await userStore.fetchUserInfo()
    } catch {
      userStore.resetState()
      uni.reLaunch({ url: '/pages/login/index' })
    }
  }
})
</script>

<style>
page {
  background-color: #F5F6F7;
  font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
  font-size: 28rpx;
  color: #1D2129;
}

/* 全局通用类 */
.container { padding: 24rpx; }
.card {
  background: #fff;
  border-radius: 16rpx;
  padding: 24rpx;
  margin-bottom: 24rpx;
}
.flex-row { display: flex; align-items: center; }
.flex-between { display: flex; align-items: center; justify-content: space-between; }
.flex-1 { flex: 1; }
.text-primary { color: #165DFF; }
.text-success { color: #00B42A; }
.text-danger { color: #F53F3F; }
.text-secondary { color: #86909C; }
.text-sm { font-size: 24rpx; }
.text-lg { font-size: 32rpx; }
.text-xl { font-size: 36rpx; }
.text-bold { font-weight: 600; }
.mt-16 { margin-top: 16rpx; }
.mt-24 { margin-top: 24rpx; }
.mb-16 { margin-bottom: 16rpx; }
.mb-24 { margin-bottom: 24rpx; }
.px-24 { padding-left: 24rpx; padding-right: 24rpx; }
</style>
