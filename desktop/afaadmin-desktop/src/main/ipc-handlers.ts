import { ipcMain, dialog, BrowserWindow, app, clipboard, Notification } from 'electron'
import { writeFileSync, readFileSync, existsSync, mkdirSync, readdirSync, statSync } from 'fs'
import { join, basename } from 'path'
import { setAutoStart } from './autostart'

export function setupIpcHandlers(mainWindow: BrowserWindow): void {

  // ===== 窗口控制 =====
  ipcMain.on('window:minimize', () => mainWindow.minimize())
  ipcMain.on('window:maximize', () => {
    mainWindow.isMaximized() ? mainWindow.unmaximize() : mainWindow.maximize()
  })
  ipcMain.on('window:close', () => mainWindow.close())
  ipcMain.handle('window:isMaximized', () => mainWindow.isMaximized())

  mainWindow.on('maximize', () => {
    mainWindow.webContents.send('window:maximized-changed', true)
  })
  mainWindow.on('unmaximize', () => {
    mainWindow.webContents.send('window:maximized-changed', false)
  })

  // ===== 文件操作 =====

  // 选择文件
  ipcMain.handle('dialog:openFile', async (_event, options) => {
    const result = await dialog.showOpenDialog(mainWindow, {
      properties: ['openFile', 'multiSelections'],
      filters: options?.filters || [{ name: '所有文件', extensions: ['*'] }],
      ...options
    })
    return result
  })

  // 选择文件夹
  ipcMain.handle('dialog:openDirectory', async () => {
    const result = await dialog.showOpenDialog(mainWindow, {
      properties: ['openDirectory']
    })
    return result
  })

  // 保存文件对话框
  ipcMain.handle('dialog:saveFile', async (_event, options) => {
    const result = await dialog.showSaveDialog(mainWindow, {
      filters: options?.filters || [
        { name: 'Excel', extensions: ['xlsx'] },
        { name: 'PDF', extensions: ['pdf'] },
        { name: '所有文件', extensions: ['*'] }
      ],
      defaultPath: options?.defaultPath,
      ...options
    })
    return result
  })

  // 写入文件到本地磁盘
  ipcMain.handle('fs:writeFile', async (_event, filePath: string, data: string, encoding?: string) => {
    try {
      const dir = join(filePath, '..')
      if (!existsSync(dir)) mkdirSync(dir, { recursive: true })
      writeFileSync(filePath, Buffer.from(data, (encoding as BufferEncoding) || 'base64'))
      return { success: true }
    } catch (err: any) {
      return { success: false, error: err.message }
    }
  })

  // 读取本地文件
  ipcMain.handle('fs:readFile', async (_event, filePath: string) => {
    try {
      if (!existsSync(filePath)) return { success: false, error: '文件不存在' }
      const data = readFileSync(filePath)
      return { success: true, data: data.toString('base64'), name: basename(filePath) }
    } catch (err: any) {
      return { success: false, error: err.message }
    }
  })

  // 列出目录文件
  ipcMain.handle('fs:listDir', async (_event, dirPath: string) => {
    try {
      if (!existsSync(dirPath)) return { success: false, error: '目录不存在' }
      const items = readdirSync(dirPath).map(name => {
        const fullPath = join(dirPath, name)
        const stat = statSync(fullPath)
        return { name, isDirectory: stat.isDirectory(), size: stat.size }
      })
      return { success: true, items }
    } catch (err: any) {
      return { success: false, error: err.message }
    }
  })

  // ===== 剪贴板 =====
  ipcMain.handle('clipboard:readText', () => clipboard.readText())
  ipcMain.handle('clipboard:writeText', (_event, text: string) => {
    clipboard.writeText(text)
    return true
  })
  ipcMain.handle('clipboard:readImage', () => {
    const image = clipboard.readImage()
    if (image.isEmpty()) return null
    return image.toDataURL()
  })

  // ===== 系统通知 =====
  ipcMain.on('notification:show', (_event, options: { title: string; body: string }) => {
    new Notification({
      title: options.title,
      body: options.body,
      icon: join(__dirname, '../../resources/icon.png')
    }).show()
  })

  // ===== 自启动 =====
  ipcMain.handle('app:setAutoStart', (_event, enable: boolean) => {
    setAutoStart(enable)
    return true
  })
  ipcMain.handle('app:getAutoStart', () => {
    return app.getLoginItemSettings().openAtLogin
  })

  // ===== 应用信息 =====
  ipcMain.handle('app:getInfo', () => ({
    version: app.getVersion(),
    name: app.getName(),
    platform: process.platform,
    arch: process.arch,
    electronVersion: process.versions.electron,
    nodeVersion: process.versions.node,
    chromeVersion: process.versions.chrome,
    userDataPath: app.getPath('userData'),
    appPath: app.getAppPath()
  }))

  // ===== 用 Shell 打开外部链接 =====
  ipcMain.on('shell:openExternal', (_event, url: string) => {
    const { shell } = require('electron')
    shell.openExternal(url)
  })
}
