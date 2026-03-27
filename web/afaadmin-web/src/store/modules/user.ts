import { defineStore } from 'pinia'
import { login as loginApi, getUserInfo, logout as logoutApi, type CurrentUser, type LoginParams } from '@/api/auth'
import { setToken, setRefreshToken, clearToken } from '@/utils/auth'
import { usePermissionStore } from './permission'

interface UserState {
  id: number
  userName: string
  nickName: string
  avatar: string
  roles: string[]
  permissions: string[]
}

export const useUserStore = defineStore('user', {
  state: (): UserState => ({
    id: 0,
    userName: '',
    nickName: '',
    avatar: '',
    roles: [],
    permissions: []
  }),

  getters: {
    isLoggedIn: (state) => state.id > 0,
    isSuperAdmin: (state) => state.roles.includes('admin')
  },

  actions: {
    async login(params: LoginParams) {
      const { data } = await loginApi(params)
      setToken(data.data.accessToken)
      setRefreshToken(data.data.refreshToken)
    },

    async fetchUserInfo() {
      const { data } = await getUserInfo()
      const info: CurrentUser = data.data
      this.id = info.id
      this.userName = info.userName
      this.nickName = info.nickName || info.userName
      this.avatar = info.avatar || ''
      this.roles = info.roles
      this.permissions = info.permissions

      // 生成动态路由
      const permStore = usePermissionStore()
      await permStore.generateRoutes()
    },

    async logout() {
      try {
        await logoutApi()
      } catch {}
      this.resetState()
      clearToken()
    },

    resetState() {
      this.id = 0
      this.userName = ''
      this.nickName = ''
      this.avatar = ''
      this.roles = []
      this.permissions = []
    },

    hasPermission(code: string): boolean {
      if (this.permissions.includes('*:*:*')) return true
      return this.permissions.includes(code)
    }
  }
})
