# AFAADMIN M7 — Electron 桌面端

基于 Electron + Vue3 的跨平台桌面管理应用，封装 Web 管理后台并增加桌面专属增强能力。

## 技术栈

- **框架**: Electron 33 + Vue3 + TypeScript
- **构建**: electron-vite + electron-builder
- **安全**: contextIsolation + preload 暴露安全 API
- **更新**: electron-updater 自动更新

## 项目结构

```
afaadmin-desktop/
├── src/
│   ├── main/                 # 主进程
│   │   ├── index.ts          # 应用入口、窗口创建
│   │   ├── tray.ts           # 系统托盘
│   │   ├── shortcuts.ts      # 全局快捷键
│   │   ├── autostart.ts      # 开机自启动
│   │   ├── ipc-handlers.ts   # IPC 通信处理（文件/剪贴板/通知等）
│   │   └── updater.ts        # 自动更新
│   ├── preload/              # 预加载脚本
│   │   ├── index.ts          # contextBridge 暴露安全 API
│   │   └── index.d.ts        # 类型定义
│   └── renderer/             # 渲染进程（Vue3）
│       ├── index.html
│       └── src/
│           ├── main.ts
│           └── App.vue       # 桌面端 Shell（含自定义标题栏）
├── resources/                # 应用图标
├── electron-builder.yml      # 打包配置
├── electron.vite.config.ts   # electron-vite 配置
└── package.json
```

## 核心特性

### 1. 自定义标题栏
- 无边框窗口 + 自绘最小化/最大化/关闭按钮
- macOS 适配隐藏标题栏保留交通灯
- 窗口拖拽区域正确设置

### 2. 系统托盘
- 关闭窗口最小化到托盘（不退出应用）
- 托盘右键菜单（显示窗口、开机自启、退出）
- 点击托盘图标恢复窗口

### 3. 全局快捷键
- `Ctrl+Shift+A` — 快速唤起窗口
- `Ctrl+Shift+Space` — AI 助手（M8 预留）
- `F5` — 刷新页面

### 4. 本地文件操作
- 原生文件/文件夹选择对话框
- 直接写入本地磁盘（突破浏览器限制）
- 拖拽上传文件夹
- 导出报表到用户指定目录

### 5. 剪贴板集成
- 读取/写入系统剪贴板文本
- 读取剪贴板图片（截图粘贴）

### 6. 系统通知
- 原生系统通知（Notification API）

### 7. 自动更新
- electron-updater 检测新版本
- 后台下载 + 提示重启安装
- 支持 generic / GitHub Releases 更新源

### 8. 安全加固
- `contextIsolation: true` + `nodeIntegration: false`
- 所有 Node.js 操作通过 preload + IPC 通信
- CSP 内容安全策略配置
- 外部链接用系统浏览器打开

## 快速开始

### 环境要求
- Node.js >= 18
- 对应平台的构建工具链

### 开发调试

```bash
# 安装依赖
npm install

# 开发模式（自动启动 Electron + Vite HMR）
npm run dev
```

### 构建安装包

```bash
# 当前平台
npm run dist

# 指定平台
npm run dist:win     # Windows (NSIS + Portable)
npm run dist:mac     # macOS (.dmg)
npm run dist:linux   # Linux (.AppImage + .deb)
```

### 与 Web 端集成

正式集成时，将 `afaadmin-web` 构建产物加载到渲染进程：

1. 方式一：将 `afaadmin-web` 作为 git submodule 引入
2. 方式二：CI 中先构建 web 端，将 `dist/` 复制到 Electron 的 `src/renderer/`
3. 开发时可通过修改 `webUrl` 指向 `http://localhost:3000`（Web 端 dev server）

## 与 Web 端的复用策略

| 层面 | 复用方式 |
|------|----------|
| API 类型定义 | 共享 `api/types.ts` |
| 工具函数 | 抽取到 `shared/utils/` |
| 业务页面 | 渲染进程加载 Web 端构建产物 |
| 桌面增强 | 通过 `window.electronAPI` 调用原生能力 |

## 待完善（后续迭代）

- [ ] SQLite 离线缓存（断网可查看历史数据）
- [ ] AI 助手悬浮窗（M8 接入）
- [ ] 剪贴板智能分析
- [ ] 本地文件拖拽到 AI 对话框
- [ ] 代码签名（Windows / macOS）
- [ ] 增量更新（asar 差量）
