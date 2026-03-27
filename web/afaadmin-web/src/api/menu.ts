import service, { type ApiResult } from './request'

export interface MenuItem {
  id: number
  parentId: number
  menuName: string
  menuType: number
  permission: string
  path: string
  component: string
  icon: string
  sort: number
  visible: boolean
  status: number
  createTime: string
  children: MenuItem[]
}

export function getMenuTree() {
  return service.get<ApiResult<MenuItem[]>>('/system/menu/tree')
}

export function getMenuById(id: number) {
  return service.get<ApiResult<MenuItem>>(`/system/menu/${id}`)
}

export function createMenu(data: Partial<MenuItem>) {
  return service.post<ApiResult<number>>('/system/menu', data)
}

export function updateMenu(data: Partial<MenuItem>) {
  return service.put<ApiResult>('/system/menu', data)
}

export function deleteMenu(id: number) {
  return service.delete<ApiResult>(`/system/menu/${id}`)
}

export function getMenuIdsByRole(roleId: number) {
  return service.get<ApiResult<number[]>>(`/system/menu/role/${roleId}`)
}
