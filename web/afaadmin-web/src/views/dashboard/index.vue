<template>
  <div class="dashboard">
    <a-row :gutter="16">
      <a-col :span="6" v-for="card in statCards" :key="card.title">
        <a-card hoverable>
          <a-statistic :title="card.title" :value="card.value">
            <template #prefix>
              <component :is="card.icon" :style="{ color: card.color }" />
            </template>
          </a-statistic>
        </a-card>
      </a-col>
    </a-row>

    <a-card title="系统信息" style="margin-top: 16px">
      <a-descriptions :column="2" bordered>
        <a-descriptions-item label="系统名称">AFAADMIN 管理后台</a-descriptions-item>
        <a-descriptions-item label="技术栈">.NET 8 + Vue3</a-descriptions-item>
        <a-descriptions-item label="当前用户">{{ userStore.nickName }}</a-descriptions-item>
        <a-descriptions-item label="角色">{{ userStore.roles.join(', ') }}</a-descriptions-item>
      </a-descriptions>
    </a-card>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/store'
import { IconUser, IconSafe, IconMenu, IconStorage } from '@arco-design/web-vue/es/icon'

const userStore = useUserStore()

const statCards = [
  { title: '用户总数', value: '--', icon: IconUser, color: '#165DFF' },
  { title: '角色总数', value: '--', icon: IconSafe, color: '#0FC6C2' },
  { title: '菜单总数', value: '--', icon: IconMenu, color: '#FF7D00' },
  { title: '存储用量', value: '--', icon: IconStorage, color: '#F53F3F' }
]
</script>

<style scoped lang="less">
.dashboard {
  :deep(.arco-card) { border-radius: 4px; }
}
</style>
