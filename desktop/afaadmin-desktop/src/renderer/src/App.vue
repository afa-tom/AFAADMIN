<template>
  <div class="desktop-app">
    <!-- 自定义标题栏 -->
    <div class="titlebar" :class="{ 'is-mac': isMac }">
      <div class="titlebar-drag">
        <span class="titlebar-logo">▣</span>
        <span class="titlebar-text">AFAADMIN 桌面端</span>
      </div>
      <div v-if="!isMac" class="titlebar-actions">
        <button class="tb-btn" @click="handleMinimize" title="最小化">─</button>
        <button class="tb-btn" @click="handleMaximize" title="最大化">
          {{ isMaximized ? '❐' : '□' }}
        </button>
        <button class="tb-btn close" @click="handleClose" title="关闭">✕</button>
      </div>
    </div>

    <!-- 主内容区 -->
    <div class="main-content">
      <div class="sidebar">
        <div class="nav-item" :class="{ active: activeTab === 'home' }" @click="activeTab = 'home'">🏠 首页</div>
        <div class="nav-item" :class="{ active: activeTab === 'web' }" @click="activeTab = 'web'">🌐 管理后台</div>
        <div class="nav-item" :class="{ active: activeTab === 'files' }" @click="activeTab = 'files'">📁 文件管理</div>
        <div class="nav-item" :class="{ active: activeTab === 'tools' }" @click="activeTab = 'tools'">🔧 桌面工具</div>
        <div class="nav-item" :class="{ active: activeTab === 'settings' }" @click="activeTab = 'settings'">⚙️ 设置</div>
      </div>

      <div class="content">
        <!-- 首页 -->
        <div v-if="activeTab === 'home'" class="page">
          <h2>欢迎使用 AFAADMIN 桌面端</h2>
          <div class="info-cards">
            <div class="info-card">
              <div class="card-label">应用版本</div>
              <div class="card-value">{{ appInfo.version || '1.0.0' }}</div>
            </div>
            <div class="info-card">
              <div class="card-label">运行平台</div>
              <div class="card-value">{{ platformLabel }}</div>
            </div>
            <div class="info-card">
              <div class="card-label">Electron</div>
              <div class="card-value">{{ appInfo.electronVersion || '-' }}</div>
            </div>
            <div class="info-card">
              <div class="card-label">Chrome</div>
              <div class="card-value">{{ appInfo.chromeVersion || '-' }}</div>
            </div>
          </div>
          <p class="hint">
            提示: 使用 <kbd>Ctrl+Shift+A</kbd> 全局唤起窗口，关闭窗口将最小化到系统托盘。
          </p>
        </div>

        <!-- 管理后台（嵌入 Web 端） -->
        <div v-if="activeTab === 'web'" class="page web-page">
          <div class="web-toolbar">
            <input v-model="webUrl" class="url-input" @keyup.enter="loadWeb" />
            <button class="action-btn" @click="loadWeb">加载</button>
          </div>
          <webview
            ref="webviewRef"
            :src="webUrl"
            class="webview"
            allowpopups
          />
        </div>

        <!-- 文件管理 -->
        <div v-if="activeTab === 'files'" class="page">
          <h3>本地文件操作</h3>
          <div class="action-row">
            <button class="action-btn" @click="handleOpenFile">📂 打开文件</button>
            <button class="action-btn" @click="handleOpenDir">📁 打开文件夹</button>
            <button class="action-btn" @click="handleExport">💾 导出示例</button>
          </div>
          <div v-if="selectedFiles.length" class="file-list">
            <div class="file-item" v-for="f in selectedFiles" :key="f">{{ f }}</div>
          </div>

          <h3 class="mt-24">剪贴板工具</h3>
          <div class="action-row">
            <button class="action-btn" @click="readClipboard">📋 读取剪贴板</button>
            <button class="action-btn" @click="copyToClipboard">📝 复制测试文本</button>
          </div>
          <div v-if="clipboardContent" class="clipboard-preview">
            <pre>{{ clipboardContent }}</pre>
          </div>
        </div>

        <!-- 桌面工具 -->
        <div v-if="activeTab === 'tools'" class="page">
          <h3>系统通知</h3>
          <div class="action-row">
            <button class="action-btn" @click="sendNotification">🔔 发送测试通知</button>
          </div>

          <h3 class="mt-24">拖拽上传（演示）</h3>
          <div
            class="drop-zone"
            :class="{ dragover: isDragOver }"
            @dragenter.prevent="isDragOver = true"
            @dragleave.prevent="isDragOver = false"
            @dragover.prevent
            @drop.prevent="handleDrop"
          >
            <p>将文件拖拽到此处</p>
            <div v-if="droppedFiles.length" class="dropped-list">
              <div v-for="f in droppedFiles" :key="f.name" class="file-item">
                {{ f.name }} ({{ (f.size / 1024).toFixed(1) }} KB)
              </div>
            </div>
          </div>
        </div>

        <!-- 设置 -->
        <div v-if="activeTab === 'settings'" class="page">
          <h3>应用设置</h3>
          <div class="setting-row">
            <span>开机自启动</span>
            <label class="toggle">
              <input type="checkbox" v-model="autoStart" @change="toggleAutoStart" />
              <span class="slider"></span>
            </label>
          </div>
          <div class="setting-row">
            <span>后端 API 地址</span>
            <input v-model="webUrl" class="setting-input" />
          </div>

          <h3 class="mt-24">关于</h3>
          <div class="about-info">
            <p>AFAADMIN 桌面端 v{{ appInfo.version || '1.0.0' }}</p>
            <p>基于 Electron + Vue3 构建</p>
            <p>© 2024 AFAADMIN. Apache 2.0 License.</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

const activeTab = ref('home')
const isMaximized = ref(false)
const appInfo = ref<Record<string, string>>({})
const webUrl = ref('http://localhost:3000')
const selectedFiles = ref<string[]>([])
const clipboardContent = ref('')
const autoStart = ref(false)
const isDragOver = ref(false)
const droppedFiles = ref<{ name: string; size: number }[]>([])
const webviewRef = ref<any>(null)

const api = window.electronAPI
const isMac = api?.platform === 'darwin'

const platformLabel = computed(() => {
  const p = api?.platform || 'unknown'
  const labels: Record<string, string> = { win32: 'Windows', darwin: 'macOS', linux: 'Linux' }
  return labels[p] || p
})

onMounted(async () => {
  if (!api) return

  const info = await api.app.getInfo()
  appInfo.value = info

  autoStart.value = await api.app.getAutoStart()

  api.window.onMaximizedChanged((val) => {
    isMaximized.value = val
  })
  isMaximized.value = await api.window.isMaximized()
})

// 窗口控制
function handleMinimize() { api?.window.minimize() }
function handleMaximize() { api?.window.maximize() }
function handleClose() { api?.window.close() }

// 文件操作
async function handleOpenFile() {
  if (!api) return
  const result = await api.dialog.openFile({
    filters: [
      { name: '文档', extensions: ['xlsx', 'csv', 'pdf', 'docx', 'txt'] },
      { name: '所有文件', extensions: ['*'] }
    ]
  })
  if (!result.canceled) selectedFiles.value = result.filePaths
}

async function handleOpenDir() {
  if (!api) return
  const result = await api.dialog.openDirectory()
  if (!result.canceled) {
    const dir = result.filePaths[0]
    const list = await api.fs.listDir(dir)
    if (list.success) {
      selectedFiles.value = list.items!.map((i: any) =>
        `${i.isDirectory ? '📁' : '📄'} ${i.name} ${i.isDirectory ? '' : `(${(i.size / 1024).toFixed(1)}KB)`}`
      )
    }
  }
}

async function handleExport() {
  if (!api) return
  const result = await api.dialog.saveFile({
    defaultPath: 'afaadmin-export.txt',
    filters: [{ name: '文本文件', extensions: ['txt'] }]
  })
  if (!result.canceled && result.filePath) {
    const content = btoa('AFAADMIN 桌面端导出测试数据\n时间: ' + new Date().toISOString())
    const writeResult = await api.fs.writeFile(result.filePath, content)
    if (writeResult.success) {
      api.notification.show({ title: '导出成功', body: `文件已保存到 ${result.filePath}` })
    }
  }
}

// 剪贴板
async function readClipboard() {
  if (!api) return
  clipboardContent.value = await api.clipboard.readText()
}
async function copyToClipboard() {
  if (!api) return
  await api.clipboard.writeText('AFAADMIN 桌面端剪贴板测试 - ' + new Date().toLocaleString())
  clipboardContent.value = '已复制到剪贴板'
}

// 通知
function sendNotification() {
  api?.notification.show({
    title: 'AFAADMIN 桌面端',
    body: '这是一条系统通知测试消息 🎉'
  })
}

// 拖拽
function handleDrop(e: DragEvent) {
  isDragOver.value = false
  const files = e.dataTransfer?.files
  if (files) {
    droppedFiles.value = Array.from(files).map(f => ({ name: f.name, size: f.size }))
  }
}

// Web
function loadWeb() {
  if (webviewRef.value) {
    webviewRef.value.loadURL(webUrl.value)
  }
}

// 自启动
async function toggleAutoStart() {
  await api?.app.setAutoStart(autoStart.value)
}
</script>

<style>
* { margin: 0; padding: 0; box-sizing: border-box; }
body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif; }

.desktop-app { height: 100vh; display: flex; flex-direction: column; background: #f5f6f7; }

/* 标题栏 */
.titlebar {
  height: 36px; background: #1e1e2e; color: #cdd6f4;
  display: flex; align-items: center; justify-content: space-between;
  -webkit-app-region: drag; user-select: none; flex-shrink: 0;
}
.titlebar.is-mac { padding-left: 80px; }
.titlebar-drag { display: flex; align-items: center; gap: 8px; padding-left: 12px; }
.titlebar-logo { font-size: 16px; color: #89b4fa; }
.titlebar-text { font-size: 13px; font-weight: 500; }
.titlebar-actions { display: flex; -webkit-app-region: no-drag; }
.tb-btn {
  width: 46px; height: 36px; border: none; background: transparent;
  color: #cdd6f4; font-size: 12px; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
}
.tb-btn:hover { background: rgba(255,255,255,0.1); }
.tb-btn.close:hover { background: #e53935; color: #fff; }

/* 布局 */
.main-content { flex: 1; display: flex; overflow: hidden; }
.sidebar {
  width: 200px; background: #313244; color: #cdd6f4;
  padding: 12px 0; flex-shrink: 0; overflow-y: auto;
}
.nav-item {
  padding: 10px 20px; cursor: pointer; font-size: 14px;
  border-left: 3px solid transparent; transition: all 0.15s;
}
.nav-item:hover { background: rgba(255,255,255,0.06); }
.nav-item.active { background: rgba(137,180,250,0.12); border-left-color: #89b4fa; color: #89b4fa; }

.content { flex: 1; overflow-y: auto; padding: 24px; }
.page h2 { color: #1d2129; margin-bottom: 20px; }
.page h3 { color: #1d2129; margin-bottom: 12px; font-size: 16px; }

/* 卡片 */
.info-cards { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; margin-bottom: 24px; }
.info-card {
  background: #fff; border-radius: 8px; padding: 20px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.08);
}
.card-label { font-size: 13px; color: #86909c; margin-bottom: 8px; }
.card-value { font-size: 20px; font-weight: 600; color: #165dff; }

.hint { color: #86909c; font-size: 14px; margin-top: 16px; }
kbd {
  background: #e5e6eb; padding: 2px 6px; border-radius: 4px;
  font-family: monospace; font-size: 12px;
}

/* 操作 */
.action-row { display: flex; gap: 12px; flex-wrap: wrap; margin-bottom: 16px; }
.action-btn {
  padding: 8px 16px; border-radius: 6px; border: 1px solid #e5e6eb;
  background: #fff; cursor: pointer; font-size: 14px; color: #1d2129;
  transition: all 0.15s;
}
.action-btn:hover { border-color: #165dff; color: #165dff; }

.file-list, .dropped-list { margin-top: 8px; }
.file-item {
  padding: 6px 12px; background: #fff; border-radius: 4px;
  margin-bottom: 4px; font-size: 13px; color: #4e5969;
}

.clipboard-preview {
  background: #fff; border-radius: 6px; padding: 12px; margin-top: 8px;
}
.clipboard-preview pre { white-space: pre-wrap; font-size: 13px; color: #1d2129; }

/* 拖拽区 */
.drop-zone {
  border: 2px dashed #c9cdd4; border-radius: 8px; padding: 40px;
  text-align: center; color: #86909c; transition: all 0.2s;
}
.drop-zone.dragover { border-color: #165dff; background: #e8f3ff; }

/* 设置 */
.setting-row {
  display: flex; align-items: center; justify-content: space-between;
  padding: 12px 0; border-bottom: 1px solid #f2f3f5; font-size: 14px;
}
.setting-input {
  width: 300px; padding: 6px 12px; border: 1px solid #e5e6eb;
  border-radius: 4px; font-size: 14px;
}
.toggle { position: relative; width: 44px; height: 22px; }
.toggle input { opacity: 0; width: 0; height: 0; }
.slider {
  position: absolute; inset: 0; background: #c9cdd4;
  border-radius: 22px; cursor: pointer; transition: 0.3s;
}
.slider:before {
  content: ''; position: absolute; width: 18px; height: 18px;
  left: 2px; bottom: 2px; background: #fff; border-radius: 50%;
  transition: 0.3s;
}
.toggle input:checked + .slider { background: #165dff; }
.toggle input:checked + .slider:before { transform: translateX(22px); }

.about-info { background: #fff; border-radius: 8px; padding: 16px; }
.about-info p { color: #4e5969; font-size: 14px; line-height: 1.8; }

.mt-24 { margin-top: 24px; }

/* Web 嵌入 */
.web-page { display: flex; flex-direction: column; height: 100%; }
.web-toolbar { display: flex; gap: 8px; margin-bottom: 8px; }
.url-input {
  flex: 1; padding: 6px 12px; border: 1px solid #e5e6eb;
  border-radius: 4px; font-size: 14px;
}
.webview { flex: 1; border: 1px solid #e5e6eb; border-radius: 4px; }
</style>
