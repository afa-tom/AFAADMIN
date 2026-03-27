<template>
  <a-breadcrumb>
    <a-breadcrumb-item v-for="item in breadcrumbs" :key="item.path">
      <router-link v-if="item.path" :to="item.path">{{ item.title }}</router-link>
      <span v-else>{{ item.title }}</span>
    </a-breadcrumb-item>
  </a-breadcrumb>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const breadcrumbs = computed(() => {
  const items: { title: string; path?: string }[] = []
  for (const matched of route.matched) {
    if (matched.meta?.title) {
      items.push({
        title: matched.meta.title as string,
        path: matched.path !== route.path ? matched.path : undefined
      })
    }
  }
  return items
})
</script>
