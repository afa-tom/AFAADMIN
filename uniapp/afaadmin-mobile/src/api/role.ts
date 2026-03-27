import { get, post, put, del } from './request'
import type { RoleItem } from './types'

export function getRoleList() {
  return get<RoleItem[]>('/system/role/list')
}

export function getRoleById(id: number) {
  return get<RoleItem>(`/system/role/${id}`)
}

export function createRole(data: any) {
  return post<number>('/system/role', data)
}

export function updateRole(data: any) {
  return put('/system/role', data)
}

export function deleteRole(id: number) {
  return del(`/system/role/${id}`)
}
