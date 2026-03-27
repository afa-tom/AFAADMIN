import { defineStore } from 'pinia'
import type { RouteRecordRaw } from 'vue-router'
import { getMenuTree, type MenuItem } from '@/api/menu'
import router from '@/router'
import Layout from '@/layout/index.vue'

// 动态导入页面组件
const modules = import.meta.glob('@/views/**/*.vue')

function resolveComponent(component: string) {
  const path = `/src/views/${component}.vue`
  return modules[path] || modules[`/src/views/${component}/index.vue`]
}

function menusToRoutes(menus: MenuItem[]): RouteRecordRaw[] {
  const routes: RouteRecordRaw[] = []
  for (const menu of menus) {
    if (menu.menuType === 3) continue // 按钮不生成路由
    if (!menu.path) continue

    const route: RouteRecordRaw = {
      path: menu.path,
      name: `menu_${menu.id}`,
      meta: {
        title: menu.menuName,
        icon: menu.icon,
        permission: menu.permission,
        visible: menu.visible,
        sort: menu.sort
      },
      component: menu.component ? resolveComponent(menu.component) : undefined,
      children: []
    }

    if (menu.children?.length) {
      route.children = menusToRoutes(menu.children)
    }

    routes.push(route)
  }
  return routes
}

export const usePermissionStore = defineStore('permission', {
  state: () => ({
    menuList: [] as MenuItem[],
    addedRoutes: false
  }),

  actions: {
    async generateRoutes() {
      try {
        const { data } = await getMenuTree()
        this.menuList = data.data || []

        const dynamicRoutes = menusToRoutes(this.menuList)
        const layoutRoute: RouteRecordRaw = {
          path: '/',
          component: Layout,
          redirect: '/dashboard',
          children: [
            {
              path: 'dashboard',
              name: 'Dashboard',
              component: () => import('@/views/dashboard/index.vue'),
              meta: { title: '首页', icon: 'icon-home', affix: true }
            },
            {
              path: 'profile',
              name: 'Profile',
              component: () => import('@/views/profile/index.vue'),
              meta: { title: '个人中心', visible: false }
            },
            ...this.flattenRoutes(dynamicRoutes)
          ]
        }

        router.addRoute(layoutRoute)
        this.addedRoutes = true
      } catch (e) {
        console.error('生成路由失败', e)
      }
    },

    /** 将树形路由扁平化（因为所有页面共用 Layout） */
    flattenRoutes(routes: RouteRecordRaw[], parentPath = ''): RouteRecordRaw[] {
      const result: RouteRecordRaw[] = []
      for (const route of routes) {
        const fullPath = parentPath ? `${parentPath}/${route.path}` : route.path as string
        if (route.component) {
          result.push({ ...route, path: fullPath, children: [] })
        }
        if (route.children?.length) {
          result.push(...this.flattenRoutes(route.children, fullPath))
        }
      }
      return result
    }
  }
})
