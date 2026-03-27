<template>
  <div>
    <a-card title="个人信息">
      <a-descriptions :column="1" bordered>
        <a-descriptions-item label="用户名">{{ userStore.userName }}</a-descriptions-item>
        <a-descriptions-item label="昵称">{{ userStore.nickName }}</a-descriptions-item>
        <a-descriptions-item label="角色">{{ userStore.roles.join(', ') }}</a-descriptions-item>
      </a-descriptions>
    </a-card>
  </div>
</template>

<script setup lang="ts">
import { useUserStore } from '@/store'
const userStore = useUserStore()
</script>
