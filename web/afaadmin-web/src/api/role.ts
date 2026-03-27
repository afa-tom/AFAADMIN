import service, { type ApiResult } from './request'

export interface RoleItem {
  id: number
  roleName: string
  roleCode: string
  sort: number
  status: number
  remark: string
  createTime: string
  menuIds: number[]
}

export function getRoleList() {
  return service.get<ApiResult<RoleItem[]>>('/system/role/list')
}

export function getRoleById(id: number) {
  return service.get<ApiResult<RoleItem>>(`/system/role/${id}`)
}

export function createRole(data: Partial<RoleItem>) {
  return service.post<ApiResult<number>>('/system/role', data)
}

export function updateRole(data: Partial<RoleItem>) {
  return service.put<ApiResult>('/system/role', data)
}

export function deleteRole(id: number) {
  return service.delete<ApiResult>(`/system/role/${id}`)
}

export function setRoleMenus(roleId: number, menuIds: number[]) {
  return service.put<ApiResult>(`/system/role/${roleId}/menus`, menuIds)
}
