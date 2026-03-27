<template>
  <div class="tabs-view">
    <a-tabs
      :active-key="route.path"
      type="card-gutter"
      size="small"
      editable
      hide-content
      @change="onTabChange"
      @delete="onTabClose"
    >
      <a-tab-pane
        v-for="tag in appStore.tagList"
        :key="tag.path"
        :title="tag.title"
        :closable="!tag.affix"
      />
    </a-tabs>
  </div>
</template>

<script setup lang="ts">
import { watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAppStore } from '@/store'

const route = useRoute()
const router = useRouter()
const appStore = useAppStore()

watch(
  () => route.path,
  (path) => {
    if (route.meta?.title) {
      appStore.addTag({
        name: route.name as string,
        path,
        title: route.meta.title as string,
        affix: route.meta.affix as boolean
      })
    }
  },
  { immediate: true }
)

function onTabChange(key: string | number) {
  router.push(key as string)
}

function onTabClose(key: string | number) {
  appStore.removeTag(key as string)
  if (route.path === key) {
    const last = appStore.tagList[appStore.tagList.length - 1]
    router.push(last?.path || '/dashboard')
  }
}
</script>

<style scoped lang="less">
.tabs-view {
  padding: 4px 16px 0;
  background: #fff;
  border-bottom: 1px solid #e5e6eb;
}
</style>
