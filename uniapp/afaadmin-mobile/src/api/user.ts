import { get, post, put, del } from './request'
import type { UserItem, UserQuery, PageResult } from './types'

export function getUserPage(params: UserQuery) {
  return get<PageResult<UserItem>>('/system/user/page', params)
}

export function getUserById(id: number) {
  return get<UserItem>(`/system/user/${id}`)
}

export function createUser(data: any) {
  return post<number>('/system/user', data)
}

export function updateUser(data: any) {
  return put('/system/user', data)
}

export function deleteUser(id: number) {
  return del(`/system/user/${id}`)
}

export function resetPassword(data: { userId: number; newPassword: string }) {
  return put('/system/user/reset-password', data)
}

export function setUserRoles(userId: number, roleIds: number[]) {
  return put(`/system/user/${userId}/roles`, roleIds)
}
