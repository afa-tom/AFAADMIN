import { app } from 'electron'

export function setupAutoStart(): void {
  // 读取当前自启动状态（可由用户通过托盘菜单切换）
  const settings = app.getLoginItemSettings()
  console.log('[AutoStart] 当前状态:', settings.openAtLogin ? '已开启' : '未开启')
}

export function setAutoStart(enable: boolean): void {
  app.setLoginItemSettings({
    openAtLogin: enable,
    // macOS 隐藏 Dock 图标启动
    openAsHidden: true
  })
}
