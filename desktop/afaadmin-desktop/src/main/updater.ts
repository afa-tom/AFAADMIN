import { autoUpdater } from 'electron-updater'
import { BrowserWindow, dialog } from 'electron'

export function checkForUpdates(mainWindow: BrowserWindow): void {
  autoUpdater.autoDownload = false
  autoUpdater.autoInstallOnAppQuit = true

  autoUpdater.on('update-available', (info) => {
    dialog.showMessageBox(mainWindow, {
      type: 'info',
      title: '发现新版本',
      message: `新版本 v${info.version} 可用，是否立即下载？`,
      buttons: ['立即下载', '稍后提醒'],
      defaultId: 0
    }).then(({ response }) => {
      if (response === 0) {
        autoUpdater.downloadUpdate()
        mainWindow.webContents.send('update:downloading')
      }
    })
  })

  autoUpdater.on('update-downloaded', () => {
    dialog.showMessageBox(mainWindow, {
      type: 'info',
      title: '更新就绪',
      message: '新版本已下载完成，重启应用以安装更新。',
      buttons: ['立即重启', '稍后'],
      defaultId: 0
    }).then(({ response }) => {
      if (response === 0) {
        autoUpdater.quitAndInstall(false, true)
      }
    })
  })

  autoUpdater.on('download-progress', (progress) => {
    mainWindow.webContents.send('update:progress', progress.percent)
  })

  autoUpdater.on('error', (err) => {
    console.error('[AutoUpdater] 更新检查失败:', err.message)
  })

  // 延迟 5 秒后检查更新
  setTimeout(() => {
    autoUpdater.checkForUpdates().catch((err) => {
      console.error('[AutoUpdater] 检查更新异常:', err.message)
    })
  }, 5000)
}
