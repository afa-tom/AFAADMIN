/**
 * 平台判断工具
 */

export function isApp(): boolean {
  // #ifdef APP-PLUS
  return true
  // #endif
  return false
}

export function isH5(): boolean {
  // #ifdef H5
  return true
  // #endif
  return false
}

export function isWeixin(): boolean {
  // #ifdef MP-WEIXIN
  return true
  // #endif
  return false
}

/**
 * 获取状态栏高度
 */
export function getStatusBarHeight(): number {
  const sysInfo = uni.getSystemInfoSync()
  return sysInfo.statusBarHeight || 0
}

/**
 * 获取导航栏总高度（状态栏 + 导航栏）
 */
export function getNavBarHeight(): number {
  const statusBar = getStatusBarHeight()
  // #ifdef MP-WEIXIN
  const menuButton = uni.getMenuButtonBoundingClientRect()
  return menuButton.bottom + menuButton.top - statusBar * 2 + statusBar
  // #endif
  return statusBar + 44 // 默认导航栏 44px
}
