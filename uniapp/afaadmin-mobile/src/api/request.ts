import { getToken, clearToken, getRefreshToken, setToken, setRefreshToken } from '@/utils/auth'
import type { ApiResult } from './types'

/** 环境配置 */
const ENV_CONFIG: Record<string, string> = {
  development: 'http://localhost:5000/api',
  // #ifdef H5
  // H5 走 vite proxy
  h5dev: '/api',
  // #endif
  staging: 'https://staging.your-domain.com/api',
  production: 'https://api.your-domain.com/api'
}

function getBaseUrl(): string {
  // #ifdef H5
  return '/api'
  // #endif
  // #ifndef H5
  return ENV_CONFIG.development
  // #endif
}

const BASE_URL = getBaseUrl()

let isRefreshing = false
let refreshQueue: Array<() => void> = []

/**
 * 统一请求封装
 */
export function request<T = any>(options: {
  url: string
  method?: 'GET' | 'POST' | 'PUT' | 'DELETE'
  data?: any
  header?: Record<string, string>
  showError?: boolean
}): Promise<ApiResult<T>> {
  const { url, method = 'GET', data, header = {}, showError = true } = options

  const token = getToken()
  if (token) {
    header['Authorization'] = `Bearer ${token}`
  }
  header['Content-Type'] = header['Content-Type'] || 'application/json'

  return new Promise((resolve, reject) => {
    uni.request({
      url: `${BASE_URL}${url}`,
      method,
      data,
      header,
      success: async (res) => {
        const result = res.data as ApiResult<T>

        if (result.code === 200) {
          resolve(result)
          return
        }

        // Token 过期 → 尝试刷新
        if (result.code === 401) {
          const refreshed = await tryRefreshToken()
          if (refreshed) {
            // 重新发起请求
            try {
              const retryResult = await request<T>(options)
              resolve(retryResult)
            } catch (e) {
              reject(e)
            }
            return
          }
          clearToken()
          uni.reLaunch({ url: '/pages/login/index' })
          reject(new Error('登录已过期'))
          return
        }

        if (showError) {
          uni.showToast({ title: result.message || '请求失败', icon: 'none' })
        }
        reject(new Error(result.message))
      },
      fail: (err) => {
        if (showError) {
          uni.showToast({ title: '网络异常', icon: 'none' })
        }
        reject(err)
      }
    })
  })
}

/**
 * 尝试刷新 Token
 */
async function tryRefreshToken(): Promise<boolean> {
  const refreshTokenStr = getRefreshToken()
  if (!refreshTokenStr) return false

  if (isRefreshing) {
    return new Promise((resolve) => {
      refreshQueue.push(() => resolve(true))
    })
  }

  isRefreshing = true
  try {
    const res = await new Promise<UniApp.RequestSuccessCallbackResult>((resolve, reject) => {
      uni.request({
        url: `${BASE_URL}/auth/refresh`,
        method: 'POST',
        data: { refreshToken: refreshTokenStr },
        header: { 'Content-Type': 'application/json' },
        success: resolve,
        fail: reject
      })
    })

    const result = res.data as ApiResult<{ accessToken: string; refreshToken: string }>
    if (result.code === 200) {
      setToken(result.data.accessToken)
      setRefreshToken(result.data.refreshToken)
      refreshQueue.forEach(cb => cb())
      refreshQueue = []
      return true
    }
  } catch {}
  finally {
    isRefreshing = false
  }
  return false
}

/** GET 请求 */
export function get<T = any>(url: string, data?: any) {
  return request<T>({ url, method: 'GET', data })
}

/** POST 请求 */
export function post<T = any>(url: string, data?: any) {
  return request<T>({ url, method: 'POST', data })
}

/** PUT 请求 */
export function put<T = any>(url: string, data?: any) {
  return request<T>({ url, method: 'PUT', data })
}

/** DELETE 请求 */
export function del<T = any>(url: string, data?: any) {
  return request<T>({ url, method: 'DELETE', data })
}

export default { request, get, post, put, del }
