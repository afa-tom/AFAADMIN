import { app, BrowserWindow, shell, globalShortcut } from 'electron'
import { join } from 'path'
import { electronApp, optimizer, is } from '@electron-toolkit/utils'
import { setupTray } from './tray'
import { setupShortcuts } from './shortcuts'
import { setupAutoStart } from './autostart'
import { setupIpcHandlers } from './ipc-handlers'
import { checkForUpdates } from './updater'

let mainWindow: BrowserWindow | null = null

function createWindow(): void {
  mainWindow = new BrowserWindow({
    width: 1280,
    height: 800,
    minWidth: 960,
    minHeight: 640,
    show: false,
    frame: false,            // 无边框窗口（自定义标题栏）
    titleBarStyle: 'hidden', // macOS 隐藏标题栏但保留交通灯
    trafficLightPosition: { x: 16, y: 16 },
    backgroundColor: '#f5f6f7',
    icon: join(__dirname, '../../resources/icon.png'),
    webPreferences: {
      preload: join(__dirname, '../preload/index.js'),
      sandbox: false,
      contextIsolation: true,
      nodeIntegration: false,
      webSecurity: true
    }
  })

  mainWindow.on('ready-to-show', () => {
    mainWindow?.show()
  })

  // 外部链接用系统浏览器打开
  mainWindow.webContents.setWindowOpenHandler(({ url }) => {
    if (url.startsWith('https:') || url.startsWith('http:')) {
      shell.openExternal(url)
    }
    return { action: 'deny' }
  })

  // 开发模式加载本地 Vite 服务或 Web 端 dev server
  if (is.dev && process.env['ELECTRON_RENDERER_URL']) {
    mainWindow.loadURL(process.env['ELECTRON_RENDERER_URL'])
  } else {
    mainWindow.loadFile(join(__dirname, '../renderer/index.html'))
  }

  // 关闭窗口时最小化到托盘
  mainWindow.on('close', (e) => {
    if (!app.isQuitting) {
      e.preventDefault()
      mainWindow?.hide()
    }
  })
}

app.whenReady().then(() => {
  // 设置 appUserModelId (Windows)
  electronApp.setAppUserModelId('com.afaadmin.desktop')

  // 开发工具优化
  app.on('browser-window-created', (_, window) => {
    optimizer.watchWindowShortcuts(window)
  })

  createWindow()

  // 初始化各模块
  setupTray(mainWindow!)
  setupShortcuts(mainWindow!)
  setupAutoStart()
  setupIpcHandlers(mainWindow!)

  // 检查更新（生产环境）
  if (!is.dev) {
    checkForUpdates(mainWindow!)
  }

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) {
      createWindow()
    } else {
      mainWindow?.show()
    }
  })
})

app.on('before-quit', () => {
  ;(app as any).isQuitting = true
})

app.on('window-all-closed', () => {
  globalShortcut.unregisterAll()
  if (process.platform !== 'darwin') {
    app.quit()
  }
})
