import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router'
import NProgress from 'nprogress'
import 'nprogress/nprogress.css'
import { isLoggedIn, getToken } from '@/utils/auth'
import { useUserStore } from '@/store/modules/user'
import { usePermissionStore } from '@/store/modules/permission'

NProgress.configure({ showSpinner: false })

const staticRoutes: RouteRecordRaw[] = [
  {
    path: '/login',
    name: 'Login',
    component: () => import('@/views/login/index.vue'),
    meta: { title: '登录' }
  },
  {
    path: '/:pathMatch(.*)*',
    name: 'NotFound',
    component: () => import('@/views/error/404.vue'),
    meta: { title: '404' }
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes: staticRoutes,
  scrollBehavior: () => ({ top: 0 })
})

const whiteList = ['/login']

router.beforeEach(async (to, _from, next) => {
  NProgress.start()
  document.title = `${to.meta.title || ''} - AFAADMIN`

  if (whiteList.includes(to.path)) {
    next()
    return
  }

  if (!getToken()) {
    next(`/login?redirect=${to.path}`)
    return
  }

  const userStore = useUserStore()
  const permStore = usePermissionStore()

  if (!userStore.isLoggedIn) {
    try {
      await userStore.fetchUserInfo()
      // 动态路由已添加，需要重新导航
      next({ ...to, replace: true })
    } catch {
      userStore.resetState()
      next('/login')
    }
    return
  }

  next()
})

router.afterEach(() => {
  NProgress.done()
})

export default router
