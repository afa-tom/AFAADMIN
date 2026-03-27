import axios, { type AxiosResponse, type InternalAxiosRequestConfig } from 'axios'
import { Message } from '@arco-design/web-vue'
import { getToken, clearToken } from '@/utils/auth'
import router from '@/router'

export interface ApiResult<T = any> {
  code: number
  message: string
  data: T
  timestamp: number
}

export interface PageResult<T = any> {
  pageIndex: number
  pageSize: number
  totalCount: number
  totalPages: number
  items: T[]
}

const service = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 30000,
  headers: { 'Content-Type': 'application/json' }
})

// 请求拦截
service.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = getToken()
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  error => Promise.reject(error)
)

// 响应拦截
service.interceptors.response.use(
  (response: AxiosResponse<ApiResult>) => {
    const res = response.data
    if (res.code === 200) {
      return response
    }
    if (res.code === 401) {
      clearToken()
      router.push('/login')
      Message.error('登录已过期，请重新登录')
      return Promise.reject(new Error(res.message))
    }
    Message.error(res.message || '请求失败')
    return Promise.reject(new Error(res.message))
  },
  error => {
    if (error.response?.status === 429) {
      Message.warning('请求过于频繁，请稍后再试')
    } else {
      Message.error(error.message || '网络异常')
    }
    return Promise.reject(error)
  }
)

export default service
