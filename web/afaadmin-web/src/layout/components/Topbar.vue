<template>
  <div class="topbar">
    <div class="topbar-left">
      <a-button type="text" @click="appStore.toggleCollapse">
        <template #icon>
          <icon-menu-fold v-if="!appStore.collapsed" />
          <icon-menu-unfold v-else />
        </template>
      </a-button>
      <Breadcrumb />
    </div>

    <div class="topbar-right">
      <a-dropdown>
        <div class="user-info">
          <a-avatar :size="28" :style="{ backgroundColor: '#165DFF' }">
            {{ userStore.nickName?.charAt(0) || 'U' }}
          </a-avatar>
          <span class="user-name">{{ userStore.nickName || userStore.userName }}</span>
          <icon-down />
        </div>
        <template #content>
          <a-doption @click="$router.push('/profile')">
            <template #icon><icon-user /></template>
            个人中心
          </a-doption>
          <a-doption @click="handleLogout">
            <template #icon><icon-export /></template>
            退出登录
          </a-doption>
        </template>
      </a-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'
import { Modal } from '@arco-design/web-vue'
import { useUserStore, useAppStore } from '@/store'
import { IconMenuFold, IconMenuUnfold, IconDown, IconUser, IconExport } from '@arco-design/web-vue/es/icon'
import Breadcrumb from './Breadcrumb.vue'

const router = useRouter()
const userStore = useUserStore()
const appStore = useAppStore()

function handleLogout() {
  Modal.confirm({
    title: '提示',
    content: '确定要退出登录吗？',
    onOk: async () => {
      await userStore.logout()
      router.push('/login')
    }
  })
}
</script>

<style scoped lang="less">
.topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
}
.topbar-left {
  display: flex;
  align-items: center;
  gap: 4px;
}
.user-info {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  padding: 4px 8px;
  border-radius: 4px;
  &:hover { background: #f2f3f5; }
}
.user-name { font-size: 14px; }
</style>
