import { globalShortcut, BrowserWindow } from 'electron'

export function setupShortcuts(mainWindow: BrowserWindow): void {
  // Ctrl+Shift+A — 快速唤起窗口
  globalShortcut.register('CommandOrControl+Shift+A', () => {
    if (mainWindow.isVisible()) {
      mainWindow.focus()
    } else {
      mainWindow.show()
      mainWindow.focus()
    }
  })

  // Ctrl+Shift+Space — 唤出 AI 助手（M8 阶段接入，先预留）
  globalShortcut.register('CommandOrControl+Shift+Space', () => {
    mainWindow.show()
    mainWindow.focus()
    mainWindow.webContents.send('toggle-ai-assistant')
  })

  // F5 — 刷新页面（开发调试用）
  globalShortcut.register('F5', () => {
    mainWindow.webContents.reload()
  })
}
