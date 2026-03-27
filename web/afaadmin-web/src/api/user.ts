import service, { type ApiResult, type PageResult } from './request'

export interface UserItem {
  id: number
  deptId: number | null
  userName: string
  nickName: string
  phone: string
  email: string
  avatar: string
  status: number
  remark: string
  createTime: string
  deptName: string
  roleIds: number[]
}

export interface UserQuery {
  userName?: string
  phone?: string
  status?: number
  deptId?: number
  pageIndex: number
  pageSize: number
}

export interface CreateUserParams {
  deptId?: number
  userName: string
  nickName?: string
  password: string
  phone?: string
  email?: string
  status: number
  remark?: string
  roleIds: number[]
}

export interface UpdateUserParams {
  id: number
  deptId?: number
  nickName?: string
  phone?: string
  email?: string
  status: number
  remark?: string
  roleIds: number[]
}

export function getUserPage(params: UserQuery) {
  return service.get<ApiResult<PageResult<UserItem>>>('/system/user/page', { params })
}

export function getUserById(id: number) {
  return service.get<ApiResult<UserItem>>(`/system/user/${id}`)
}

export function createUser(data: CreateUserParams) {
  return service.post<ApiResult<number>>('/system/user', data)
}

export function updateUser(data: UpdateUserParams) {
  return service.put<ApiResult>('/system/user', data)
}

export function deleteUser(id: number) {
  return service.delete<ApiResult>(`/system/user/${id}`)
}

export function resetPassword(data: { userId: number; newPassword: string }) {
  return service.put<ApiResult>('/system/user/reset-password', data)
}

export function setUserRoles(userId: number, roleIds: number[]) {
  return service.put<ApiResult>(`/system/user/${userId}/roles`, roleIds)
}
