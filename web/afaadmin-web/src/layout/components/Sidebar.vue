<template>
  <a-menu
    :selected-keys="selectedKeys"
    :default-open-keys="openKeys"
    auto-open-selected
    @menu-item-click="onMenuClick"
  >
    <template v-for="menu in visibleMenus" :key="menu.id">
      <template v-if="menu.children?.length">
        <a-sub-menu :key="`sub_${menu.id}`">
          <template #icon><icon-apps /></template>
          <template #title>{{ menu.menuName }}</template>
          <a-menu-item v-for="child in getVisibleChildren(menu)" :key="child.path || child.id">
            {{ child.menuName }}
          </a-menu-item>
        </a-sub-menu>
      </template>
      <template v-else>
        <a-menu-item :key="menu.path || menu.id">
          <template #icon><icon-apps /></template>
          {{ menu.menuName }}
        </a-menu-item>
      </template>
    </template>
  </a-menu>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { usePermissionStore } from '@/store'
import { IconApps } from '@arco-design/web-vue/es/icon'
import type { MenuItem } from '@/api/menu'

const route = useRoute()
const router = useRouter()
const permStore = usePermissionStore()

const visibleMenus = computed(() =>
  permStore.menuList.filter(m => m.menuType !== 3 && m.visible !== false)
)

function getVisibleChildren(menu: MenuItem) {
  return (menu.children || []).filter(m => m.menuType !== 3 && m.visible !== false)
}

const selectedKeys = computed(() => [route.path.replace(/^\//, '')])
const openKeys = computed(() => {
  const parts = route.path.split('/').filter(Boolean)
  return parts.length > 1 ? [`sub_${findParentId(parts[0])}`] : []
})

function findParentId(path: string) {
  const menu = permStore.menuList.find(m => m.path === '/' + path || m.path === path)
  return menu?.id || ''
}

function onMenuClick(key: string) {
  const path = key.startsWith('/') ? key : `/${key}`
  router.push(path)
}
</script>
