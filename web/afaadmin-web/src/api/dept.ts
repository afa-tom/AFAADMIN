import service, { type ApiResult } from './request'

export interface DeptItem {
  id: number
  parentId: number
  deptName: string
  sort: number
  leader: string
  phone: string
  status: number
  createTime: string
  children: DeptItem[]
}

export function getDeptTree() {
  return service.get<ApiResult<DeptItem[]>>('/system/dept/tree')
}

export function getDeptById(id: number) {
  return service.get<ApiResult<DeptItem>>(`/system/dept/${id}`)
}

export function createDept(data: Partial<DeptItem>) {
  return service.post<ApiResult<number>>('/system/dept', data)
}

export function updateDept(data: Partial<DeptItem>) {
  return service.put<ApiResult>('/system/dept', data)
}

export function deleteDept(id: number) {
  return service.delete<ApiResult>(`/system/dept/${id}`)
}
