<template>
  <!-- 悬浮按钮 -->
  <div class="ai-fab" @click="visible = !visible" :class="{ active: visible }">
    <icon-robot :size="24" />
  </div>

  <!-- 聊天面板 -->
  <transition name="slide-up">
    <div v-if="visible" class="ai-panel">
      <div class="ai-header">
        <div class="ai-title">
          <icon-robot :size="18" />
          <span>AI 助手</span>
        </div>
        <div class="ai-actions">
          <a-tooltip content="新对话">
            <icon-plus :size="16" style="cursor:pointer" @click="newSession" />
          </a-tooltip>
          <icon-close :size="16" style="cursor:pointer;margin-left:12px" @click="visible = false" />
        </div>
      </div>

      <div class="ai-messages" ref="messagesRef">
        <div v-if="messages.length === 0" class="ai-welcome">
          <p>你好！我是 AFAADMIN AI 助手 🤖</p>
          <p class="ai-welcome-sub">你可以问我关于系统的问题，例如：</p>
          <div class="ai-suggestions">
            <span class="suggestion" @click="sendQuick('系统有多少用户？')">系统有多少用户？</span>
            <span class="suggestion" @click="sendQuick('查看角色列表')">查看角色列表</span>
            <span class="suggestion" @click="sendQuick('显示部门结构')">显示部门结构</span>
            <span class="suggestion" @click="sendQuick('系统总览')">系统总览</span>
          </div>
        </div>

        <div
          v-for="(msg, i) in messages"
          :key="i"
          class="ai-msg"
          :class="msg.role"
        >
          <div class="msg-avatar">
            {{ msg.role === 'user' ? '我' : 'AI' }}
          </div>
          <div class="msg-content">
            <pre class="msg-text">{{ msg.content }}</pre>
          </div>
        </div>

        <div v-if="streaming" class="ai-msg assistant">
          <div class="msg-avatar">AI</div>
          <div class="msg-content">
            <pre class="msg-text">{{ streamContent }}<span class="cursor">▌</span></pre>
          </div>
        </div>
      </div>

      <div class="ai-input">
        <a-input
          v-model="inputText"
          placeholder="输入消息..."
          :disabled="streaming"
          allow-clear
          @keyup.enter="sendMessage"
        >
          <template #suffix>
            <icon-send
              :size="18"
              :style="{ cursor: streaming ? 'not-allowed' : 'pointer', color: inputText ? '#165DFF' : '#c9cdd4' }"
              @click="sendMessage"
            />
          </template>
        </a-input>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { ref, nextTick, watch } from 'vue'
import { IconRobot, IconPlus, IconClose, IconSend } from '@arco-design/web-vue/es/icon'
import { chatStream, type ChatStreamEvent } from '@/api/ai'

interface Message {
  role: 'user' | 'assistant'
  content: string
}

const visible = ref(false)
const inputText = ref('')
const messages = ref<Message[]>([])
const streaming = ref(false)
const streamContent = ref('')
const sessionId = ref<string | undefined>()
const messagesRef = ref<HTMLElement>()
let abortController: AbortController | null = null

function scrollToBottom() {
  nextTick(() => {
    if (messagesRef.value) {
      messagesRef.value.scrollTop = messagesRef.value.scrollHeight
    }
  })
}

watch(streamContent, scrollToBottom)

function sendQuick(text: string) {
  inputText.value = text
  sendMessage()
}

function newSession() {
  messages.value = []
  sessionId.value = undefined
  streamContent.value = ''
  streaming.value = false
  if (abortController) {
    abortController.abort()
    abortController = null
  }
}

function sendMessage() {
  const text = inputText.value.trim()
  if (!text || streaming.value) return

  messages.value.push({ role: 'user', content: text })
  inputText.value = ''
  streaming.value = true
  streamContent.value = ''
  scrollToBottom()

  abortController = chatStream(
    { sessionId: sessionId.value, message: text },
    (event: ChatStreamEvent) => {
      switch (event.event) {
        case 'start':
          sessionId.value = event.sessionId
          break
        case 'content':
          streamContent.value += event.content || ''
          break
        case 'end':
          messages.value.push({ role: 'assistant', content: streamContent.value })
          streamContent.value = ''
          streaming.value = false
          break
        case 'error':
          messages.value.push({ role: 'assistant', content: event.content || '出错了' })
          streaming.value = false
          break
      }
    },
    () => {
      if (streaming.value && streamContent.value) {
        messages.value.push({ role: 'assistant', content: streamContent.value })
        streamContent.value = ''
      }
      streaming.value = false
    },
    (err) => {
      messages.value.push({ role: 'assistant', content: `请求失败: ${err}` })
      streaming.value = false
    }
  )
}
</script>

<style scoped lang="less">
.ai-fab {
  position: fixed;
  right: 24px;
  bottom: 24px;
  width: 52px;
  height: 52px;
  border-radius: 50%;
  background: #165DFF;
  color: #fff;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  box-shadow: 0 4px 16px rgba(22, 93, 255, 0.4);
  z-index: 999;
  transition: all 0.3s;
  &:hover { transform: scale(1.08); }
  &.active { background: #4080FF; }
}

.ai-panel {
  position: fixed;
  right: 24px;
  bottom: 88px;
  width: 400px;
  height: 560px;
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.15);
  z-index: 998;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.ai-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 14px 16px;
  background: linear-gradient(135deg, #165DFF, #4080FF);
  color: #fff;
}
.ai-title {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 600;
  font-size: 15px;
}
.ai-actions { display: flex; align-items: center; }

.ai-messages {
  flex: 1;
  overflow-y: auto;
  padding: 16px;
}

.ai-welcome {
  text-align: center;
  padding: 24px 0;
  color: #4e5969;
  p { margin: 4px 0; }
  .ai-welcome-sub { font-size: 13px; color: #86909c; margin-top: 8px; }
}

.ai-suggestions {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  justify-content: center;
  margin-top: 16px;
}
.suggestion {
  padding: 6px 12px;
  background: #f2f3f5;
  border-radius: 16px;
  font-size: 13px;
  color: #165DFF;
  cursor: pointer;
  transition: background 0.2s;
  &:hover { background: #e8f3ff; }
}

.ai-msg {
  display: flex;
  gap: 10px;
  margin-bottom: 16px;
  &.user { flex-direction: row-reverse; }
  &.user .msg-content {
    background: #165DFF;
    color: #fff;
    border-radius: 12px 2px 12px 12px;
  }
  &.assistant .msg-content {
    background: #f2f3f5;
    color: #1d2129;
    border-radius: 2px 12px 12px 12px;
  }
}

.msg-avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: #e8f3ff;
  color: #165DFF;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 600;
  flex-shrink: 0;
}
.ai-msg.user .msg-avatar { background: #165DFF; color: #fff; }

.msg-content {
  max-width: 280px;
  padding: 10px 14px;
}
.msg-text {
  margin: 0;
  font-family: inherit;
  font-size: 14px;
  line-height: 1.6;
  white-space: pre-wrap;
  word-break: break-word;
}

.cursor {
  animation: blink 1s infinite;
}
@keyframes blink {
  0%, 50% { opacity: 1; }
  51%, 100% { opacity: 0; }
}

.ai-input {
  padding: 12px 16px;
  border-top: 1px solid #e5e6eb;
}

.slide-up-enter-active,
.slide-up-leave-active {
  transition: all 0.3s ease;
}
.slide-up-enter-from,
.slide-up-leave-to {
  opacity: 0;
  transform: translateY(20px);
}
</style>
