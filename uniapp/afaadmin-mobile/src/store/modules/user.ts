import { defineStore } from 'pinia'
import { login as loginApi, getUserInfo, logout as logoutApi } from '@/api/auth'
import { setToken, setRefreshToken, clearToken, getToken } from '@/utils/auth'
import type { LoginParams, CurrentUser } from '@/api/types'

export const useUserStore = defineStore('user', {
  state: () => ({
    token: getToken(),
    id: 0,
    userName: '',
    nickName: '',
    avatar: '',
    roles: [] as string[],
    permissions: [] as string[]
  }),

  getters: {
    isLoggedIn: (state) => state.id > 0,
    isSuperAdmin: (state) => state.roles.includes('admin')
  },

  actions: {
    async login(params: LoginParams) {
      const result = await loginApi(params)
      this.token = result.data.accessToken
      setToken(result.data.accessToken)
      setRefreshToken(result.data.refreshToken)
    },

    async fetchUserInfo() {
      const result = await getUserInfo()
      const info: CurrentUser = result.data
      this.id = info.id
      this.userName = info.userName
      this.nickName = info.nickName || info.userName
      this.avatar = info.avatar || ''
      this.roles = info.roles
      this.permissions = info.permissions
    },

    async logout() {
      try { await logoutApi() } catch {}
      this.resetState()
      clearToken()
    },

    resetState() {
      this.token = ''
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
