import service, { type ApiResult } from './request'

export interface LoginParams {
  userName: string
  password: string
}

export interface LoginResult {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

export interface CurrentUser {
  id: number
  userName: string
  nickName: string
  avatar: string
  roles: string[]
  permissions: string[]
}

export function login(data: LoginParams) {
  return service.post<ApiResult<LoginResult>>('/auth/login', data)
}

export function refreshToken(refreshToken: string) {
  return service.post<ApiResult<LoginResult>>('/auth/refresh', { refreshToken })
}

export function getUserInfo() {
  return service.get<ApiResult<CurrentUser>>('/auth/userinfo')
}

export function logout() {
  return service.post<ApiResult>('/auth/logout')
}
