import service, { type ApiResult } from './request'

export interface DictTypeItem {
  id: number
  dictName: string
  dictCode: string
  status: number
  remark: string
  createTime: string
}

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

// 字典类型
export function getDictTypeList() {
  return service.get<ApiResult<DictTypeItem[]>>('/system/dict/type/list')
}

export function createDictType(data: Partial<DictTypeItem>) {
  return service.post<ApiResult<number>>('/system/dict/type', data)
}

export function updateDictType(data: Partial<DictTypeItem>) {
  return service.put<ApiResult>('/system/dict/type', data)
}

export function deleteDictType(id: number) {
  return service.delete<ApiResult>(`/system/dict/type/${id}`)
}

// 字典数据
export function getDictDataByTypeId(typeId: number) {
  return service.get<ApiResult<DictDataItem[]>>(`/system/dict/data/list/${typeId}`)
}

export function getDictDataByCode(code: string) {
  return service.get<ApiResult<DictDataItem[]>>(`/system/dict/data/code/${code}`)
}

export function createDictData(data: Partial<DictDataItem>) {
  return service.post<ApiResult<number>>('/system/dict/data', data)
}

export function updateDictData(data: Partial<DictDataItem>) {
  return service.put<ApiResult>('/system/dict/data', data)
}

export function deleteDictData(id: number) {
  return service.delete<ApiResult>(`/system/dict/data/${id}`)
}
