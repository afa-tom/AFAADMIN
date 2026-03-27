const TOKEN_KEY = 'afa_access_token'
const REFRESH_KEY = 'afa_refresh_token'

export function getToken(): string {
  return uni.getStorageSync(TOKEN_KEY) || ''
}

export function setToken(token: string) {
  uni.setStorageSync(TOKEN_KEY, token)
}

export function getRefreshToken(): string {
  return uni.getStorageSync(REFRESH_KEY) || ''
}

export function setRefreshToken(token: string) {
  uni.setStorageSync(REFRESH_KEY, token)
}

export function clearToken() {
  uni.removeStorageSync(TOKEN_KEY)
  uni.removeStorageSync(REFRESH_KEY)
}

export function isLoggedIn(): boolean {
  return !!getToken()
}
