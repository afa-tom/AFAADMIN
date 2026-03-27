/** 统一响应格式 */
export interface ApiResult<T = any> {
  code: number
  message: string
  data: T
  timestamp: number
}

/** 分页结果 */
export interface PageResult<T = any> {
  pageIndex: number
  pageSize: number
  totalCount: number
  totalPages: number
  items: T[]
}

/** 登录请求 */
export interface LoginParams {
  userName: string
  password: string
}

/** 登录响应 */
export interface LoginResult {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

/** 当前用户 */
export interface CurrentUser {
  id: number
  userName: string
  nickName: string
  avatar: string
  roles: string[]
  permissions: string[]
}

/** 用户 */
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

/** 角色 */
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

/** 部门 */
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

/** 字典类型 */
export interface DictTypeItem {
  id: number
  dictName: string
  dictCode: string
  status: number
  remark: string
  createTime: string
}

/** 字典数据 */
export interface DictDataItem {
  id: number
  dictTypeId: number
  dictLabel: string
  dictValue: string
  sort: number
  cssClass: string
  status: number
  remark: string
}
