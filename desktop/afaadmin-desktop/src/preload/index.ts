import { contextBridge, ipcRenderer } from 'electron'

/**
 * 安全地暴露 API 给渲染进程
 * 遵循 contextIsolation + nodeIntegration:false 安全规范
 */
const api = {
  // ===== 窗口控制 =====
  window: {
    minimize: () => ipcRenderer.send('window:minimize'),
    maximize: () => ipcRenderer.send('window:maximize'),
    close: () => ipcRenderer.send('window:close'),
    isMaximized: () => ipcRenderer.invoke('window:isMaximized'),
    onMaximizedChanged: (callback: (isMaximized: boolean) => void) => {
      ipcRenderer.on('window:maximized-changed', (_event, value) => callback(value))
    }
  },

  // ===== 文件操作 =====
  dialog: {
    openFile: (options?: any) => ipcRenderer.invoke('dialog:openFile', options),
    openDirectory: () => ipcRenderer.invoke('dialog:openDirectory'),
    saveFile: (options?: any) => ipcRenderer.invoke('dialog:saveFile', options)
  },

  fs: {
    writeFile: (path: string, data: string, encoding?: string) =>
      ipcRenderer.invoke('fs:writeFile', path, data, encoding),
    readFile: (path: string) => ipcRenderer.invoke('fs:readFile', path),
    listDir: (path: string) => ipcRenderer.invoke('fs:listDir', path)
  },

  // ===== 剪贴板 =====
  clipboard: {
    readText: () => ipcRenderer.invoke('clipboard:readText'),
    writeText: (text: string) => ipcRenderer.invoke('clipboard:writeText', text),
    readImage: () => ipcRenderer.invoke('clipboard:readImage')
  },

  // ===== 系统通知 =====
  notification: {
    show: (options: { title: string; body: string }) =>
      ipcRenderer.send('notification:show', options)
  },

  // ===== 应用控制 =====
  app: {
    getInfo: () => ipcRenderer.invoke('app:getInfo'),
    setAutoStart: (enable: boolean) => ipcRenderer.invoke('app:setAutoStart', enable),
    getAutoStart: () => ipcRenderer.invoke('app:getAutoStart'),
    openExternal: (url: string) => ipcRenderer.send('shell:openExternal', url)
  },

  // ===== 更新事件 =====
  updater: {
    onDownloading: (callback: () => void) => {
      ipcRenderer.on('update:downloading', () => callback())
    },
    onProgress: (callback: (percent: number) => void) => {
      ipcRenderer.on('update:progress', (_event, percent) => callback(percent))
    }
  },

  // ===== AI 助手（M8 预留）=====
  ai: {
    onToggle: (callback: () => void) => {
      ipcRenderer.on('toggle-ai-assistant', () => callback())
    }
  },

  // ===== 平台信息 =====
  platform: process.platform
}

contextBridge.exposeInMainWorld('electronAPI', api)
