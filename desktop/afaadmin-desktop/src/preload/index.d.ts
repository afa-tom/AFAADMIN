export interface ElectronAPI {
  window: {
    minimize: () => void
    maximize: () => void
    close: () => void
    isMaximized: () => Promise<boolean>
    onMaximizedChanged: (callback: (isMaximized: boolean) => void) => void
  }
  dialog: {
    openFile: (options?: any) => Promise<{ canceled: boolean; filePaths: string[] }>
    openDirectory: () => Promise<{ canceled: boolean; filePaths: string[] }>
    saveFile: (options?: any) => Promise<{ canceled: boolean; filePath: string | undefined }>
  }
  fs: {
    writeFile: (path: string, data: string, encoding?: string) => Promise<{ success: boolean; error?: string }>
    readFile: (path: string) => Promise<{ success: boolean; data?: string; name?: string; error?: string }>
    listDir: (path: string) => Promise<{ success: boolean; items?: any[]; error?: string }>
  }
  clipboard: {
    readText: () => Promise<string>
    writeText: (text: string) => Promise<boolean>
    readImage: () => Promise<string | null>
  }
  notification: {
    show: (options: { title: string; body: string }) => void
  }
  app: {
    getInfo: () => Promise<Record<string, string>>
    setAutoStart: (enable: boolean) => Promise<boolean>
    getAutoStart: () => Promise<boolean>
    openExternal: (url: string) => void
  }
  updater: {
    onDownloading: (callback: () => void) => void
    onProgress: (callback: (percent: number) => void) => void
  }
  ai: {
    onToggle: (callback: () => void) => void
  }
  platform: string
}

declare global {
  interface Window {
    electronAPI: ElectronAPI
  }
}
