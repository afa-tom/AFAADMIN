<template>
  <div class="login-page">
    <div class="login-card">
      <div class="login-header">
        <img src="/vite.svg" alt="logo" class="login-logo" />
        <h2>AFAADMIN 管理后台</h2>
      </div>

      <a-form :model="form" layout="vertical" @submit="handleLogin">
        <a-form-item field="userName" label="用户名" :rules="[{ required: true, message: '请输入用户名' }]">
          <a-input v-model="form.userName" placeholder="请输入用户名" allow-clear>
            <template #prefix><icon-user /></template>
          </a-input>
        </a-form-item>

        <a-form-item field="password" label="密码" :rules="[{ required: true, message: '请输入密码' }]">
          <a-input-password v-model="form.password" placeholder="请输入密码" allow-clear>
            <template #prefix><icon-lock /></template>
          </a-input-password>
        </a-form-item>

        <a-button type="primary" html-type="submit" long :loading="loading" style="margin-top: 8px">
          登 录
        </a-button>
      </a-form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { Message } from '@arco-design/web-vue'
import { useUserStore } from '@/store'
import { IconUser, IconLock } from '@arco-design/web-vue/es/icon'

const router = useRouter()
const route = useRoute()
const userStore = useUserStore()

const loading = ref(false)
const form = reactive({
  userName: 'admin',
  password: 'admin123'
})

async function handleLogin({ errors }: any) {
  if (errors) return
  loading.value = true
  try {
    await userStore.login(form)
    Message.success('登录成功')
    const redirect = (route.query.redirect as string) || '/dashboard'
    router.push(redirect)
  } catch (e: any) {
    Message.error(e.message || '登录失败')
  } finally {
    loading.value = false
  }
}
</script>

<style scoped lang="less">
.login-page {
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.login-card {
  width: 380px;
  padding: 40px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.12);
}

.login-header {
  text-align: center;
  margin-bottom: 32px;
  h2 { margin: 12px 0 0; color: #1d2129; font-size: 20px; }
}

.login-logo { width: 48px; height: 48px; }
</style>
