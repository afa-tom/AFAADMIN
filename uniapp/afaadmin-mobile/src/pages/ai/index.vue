<template>
  <view class="ai-page">
    <!-- 自定义导航栏 -->
    <view class="nav-bar" :style="{ paddingTop: statusBarHeight + 'px' }">
      <view class="nav-content">
        <text class="nav-back" @tap="goBack">‹</text>
        <text class="nav-title">AI 助手</text>
        <text class="nav-action" @tap="newSession">新对话</text>
      </view>
    </view>

    <!-- 消息列表 -->
    <scroll-view
      scroll-y
      class="msg-list"
      :scroll-top="scrollTop"
      :style="{ paddingTop: navHeight + 'px' }"
    >
      <view v-if="messages.length === 0" class="welcome">
        <text class="welcome-title">你好！我是 AI 助手 🤖</text>
        <text class="welcome-sub">试试这些问题：</text>
        <view class="suggestions">
          <text class="sug-item" @tap="sendQuick('系统有多少用户？')">系统有多少用户？</text>
          <text class="sug-item" @tap="sendQuick('查看角色列表')">查看角色列表</text>
          <text class="sug-item" @tap="sendQuick('系统总览')">系统总览</text>
        </view>
      </view>

      <view v-for="(msg, i) in messages" :key="i"
        class="msg-row" :class="msg.role">
        <view class="msg-avatar">
          <text>{{ msg.role === 'user' ? '我' : 'AI' }}</text>
        </view>
        <view class="msg-bubble">
          <text class="msg-text">{{ msg.content }}</text>
        </view>
      </view>

      <view v-if="loading" class="msg-row assistant">
        <view class="msg-avatar"><text>AI</text></view>
        <view class="msg-bubble">
          <text class="msg-text typing">思考中...</text>
        </view>
      </view>

      <view style="height: 20rpx;" />
    </scroll-view>

    <!-- 输入框 -->
    <view class="input-bar">
      <input
        v-model="inputText"
        class="chat-input"
        placeholder="输入消息..."
        confirm-type="send"
        :disabled="loading"
        @confirm="sendMessage"
      />
      <view class="send-btn" :class="{ disabled: !inputText.trim() || loading }"
        @tap="sendMessage">
        <text>发送</text>
      </view>
    </view>
  </view>
</template>

<script setup lang="ts">
import { ref, nextTick } from 'vue'
import { getStatusBarHeight } from '@/utils/platform'
import { chat } from '@/api/ai'

interface Message {
  role: 'user' | 'assistant'
  content: string
}

const statusBarHeight = ref(getStatusBarHeight())
const navHeight = ref(statusBarHeight.value + 44)
const inputText = ref('')
const messages = ref<Message[]>([])
const loading = ref(false)
const sessionId = ref<string | undefined>()
const scrollTop = ref(0)

function goBack() {
  uni.navigateBack()
}

function newSession() {
  messages.value = []
  sessionId.value = undefined
}

function sendQuick(text: string) {
  inputText.value = text
  sendMessage()
}

function scrollToBottom() {
  nextTick(() => {
    scrollTop.value = scrollTop.value === 99999 ? 99998 : 99999
  })
}

async function sendMessage() {
  const text = inputText.value.trim()
  if (!text || loading.value) return

  messages.value.push({ role: 'user', content: text })
  inputText.value = ''
  loading.value = true
  scrollToBottom()

  try {
    const result = await chat({ sessionId: sessionId.value, message: text })
    sessionId.value = result.data.sessionId
    messages.value.push({ role: 'assistant', content: result.data.message })
  } catch (e: any) {
    messages.value.push({ role: 'assistant', content: e.message || '请求失败' })
  } finally {
    loading.value = false
    scrollToBottom()
  }
}
</script>

<style scoped>
.ai-page {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #F5F6F7;
}

.nav-bar {
  position: fixed; top: 0; left: 0; right: 0; z-index: 10;
  background: linear-gradient(135deg, #165DFF, #4080FF);
}
.nav-content {
  height: 88rpx;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 24rpx;
}
.nav-back { color: #fff; font-size: 44rpx; padding: 0 16rpx; }
.nav-title { color: #fff; font-size: 34rpx; font-weight: 600; }
.nav-action { color: rgba(255,255,255,0.8); font-size: 28rpx; }

.msg-list { flex: 1; padding: 24rpx; padding-bottom: 140rpx; }

.welcome {
  text-align: center; padding: 60rpx 0;
}
.welcome-title { font-size: 36rpx; font-weight: 600; color: #1D2129; display: block; }
.welcome-sub { font-size: 26rpx; color: #86909C; margin-top: 12rpx; display: block; }
.suggestions { display: flex; flex-wrap: wrap; justify-content: center; gap: 16rpx; margin-top: 32rpx; }
.sug-item {
  padding: 12rpx 24rpx; background: #E8F3FF; border-radius: 32rpx;
  font-size: 26rpx; color: #165DFF;
}

.msg-row { display: flex; gap: 16rpx; margin-bottom: 24rpx; }
.msg-row.user { flex-direction: row-reverse; }

.msg-avatar {
  width: 64rpx; height: 64rpx; border-radius: 50%;
  background: #E8F3FF; display: flex; align-items: center; justify-content: center;
  flex-shrink: 0;
}
.msg-avatar text { font-size: 24rpx; color: #165DFF; font-weight: 600; }
.msg-row.user .msg-avatar { background: #165DFF; }
.msg-row.user .msg-avatar text { color: #fff; }

.msg-bubble {
  max-width: 540rpx; padding: 20rpx 24rpx;
  border-radius: 4rpx 24rpx 24rpx 24rpx;
  background: #fff;
}
.msg-row.user .msg-bubble {
  background: #165DFF;
  border-radius: 24rpx 4rpx 24rpx 24rpx;
}
.msg-text { font-size: 28rpx; line-height: 1.6; color: #1D2129; }
.msg-row.user .msg-text { color: #fff; }
.typing { color: #86909C; }

.input-bar {
  position: fixed; bottom: 0; left: 0; right: 0;
  display: flex; align-items: center; gap: 16rpx;
  padding: 16rpx 24rpx;
  padding-bottom: calc(16rpx + env(safe-area-inset-bottom));
  background: #fff;
  border-top: 1rpx solid #E5E6EB;
}
.chat-input {
  flex: 1; height: 72rpx; background: #F5F6F7;
  border-radius: 36rpx; padding: 0 28rpx; font-size: 28rpx;
}
.send-btn {
  width: 120rpx; height: 72rpx; border-radius: 36rpx;
  background: #165DFF; display: flex; align-items: center; justify-content: center;
}
.send-btn text { color: #fff; font-size: 28rpx; font-weight: 500; }
.send-btn.disabled { opacity: 0.5; }
</style>
