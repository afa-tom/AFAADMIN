import { get, post, put, del } from './request'
import type { DeptItem } from './types'

export function getDeptTree() {
  return get<DeptItem[]>('/system/dept/tree')
}

export function getDeptById(id: number) {
  return get<DeptItem>(`/system/dept/${id}`)
}

export function createDept(data: any) {
  return post<number>('/system/dept', data)
}

export function updateDept(data: any) {
  return put('/system/dept', data)
}

export function deleteDept(id: number) {
  return del(`/system/dept/${id}`)
}
