import { get, post, put, del } from './request'
import type { DictTypeItem, DictDataItem } from './types'

export function getDictTypeList() {
  return get<DictTypeItem[]>('/system/dict/type/list')
}

export function createDictType(data: any) {
  return post<number>('/system/dict/type', data)
}

export function updateDictType(data: any) {
  return put('/system/dict/type', data)
}

export function deleteDictType(id: number) {
  return del(`/system/dict/type/${id}`)
}

export function getDictDataByTypeId(typeId: number) {
  return get<DictDataItem[]>(`/system/dict/data/list/${typeId}`)
}

export function getDictDataByCode(code: string) {
  return get<DictDataItem[]>(`/system/dict/data/code/${code}`)
}

export function createDictData(data: any) {
  return post<number>('/system/dict/data', data)
}

export function updateDictData(data: any) {
  return put('/system/dict/data', data)
}

export function deleteDictData(id: number) {
  return del(`/system/dict/data/${id}`)
}
