import { get, post } from './request'
import type { LoginParams, LoginResult, CurrentUser } from './types'

export function login(data: LoginParams) {
  return post<LoginResult>('/auth/login', data)
}

export function refreshToken(token: string) {
  return post<LoginResult>('/auth/refresh', { refreshToken: token })
}

export function getUserInfo() {
  return get<CurrentUser>('/auth/userinfo')
}

export function logout() {
  return post('/auth/logout')
}
