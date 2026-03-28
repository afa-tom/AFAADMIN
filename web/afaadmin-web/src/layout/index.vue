<template>
  <a-layout class="main-layout">
    <a-layout-sider
      :width="220"
      :collapsed-width="48"
      :collapsed="appStore.collapsed"
      collapsible
      breakpoint="lg"
      @collapse="appStore.toggleCollapse"
    >
      <div class="logo" @click="$router.push('/dashboard')">
        <img src="/vite.svg" alt="logo" />
        <span v-if="!appStore.collapsed" class="logo-text">AFAADMIN</span>
      </div>
      <Sidebar />
    </a-layout-sider>

    <a-layout>
      <a-layout-header class="header">
        <Topbar />
      </a-layout-header>

      <TabsView />

      <a-layout-content class="content">
        <router-view v-slot="{ Component }">
          <transition name="fade" mode="out-in">
            <keep-alive>
              <component :is="Component" />
            </keep-alive>
          </transition>
        </router-view>
      </a-layout-content>
    </a-layout>

    <!-- AI 助手（M8） -->
    <AIAssistant />
  </a-layout>
</template>

<script setup lang="ts">
import { useAppStore } from '@/store'
import Sidebar from './components/Sidebar.vue'
import Topbar from './components/Topbar.vue'
import TabsView from './components/TabsView.vue'
import AIAssistant from './components/AIAssistant.vue'

const appStore = useAppStore()
</script>

<style scoped lang="less">
.main-layout {
  height: 100vh;
}

.logo {
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  cursor: pointer;
  background: rgba(255, 255, 255, 0.08);

  img { width: 28px; height: 28px; }
  .logo-text {
    color: #fff;
    font-size: 16px;
    font-weight: 600;
    white-space: nowrap;
  }
}

.header {
  height: 48px;
  padding: 0 16px;
  display: flex;
  align-items: center;
  background: #fff;
  border-bottom: 1px solid #e5e6eb;
}

.content {
  margin: 16px;
  padding: 16px;
  background: #fff;
  border-radius: 4px;
  overflow: auto;
}
</style>
