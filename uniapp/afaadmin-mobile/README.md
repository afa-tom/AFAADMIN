# AFAADMIN M6 — UniApp 移动端

基于 UniApp (Vue3 + TypeScript) 的跨平台移动端管理 App。

## 技术栈

- **框架**: UniApp + Vue3 + TypeScript + Vite
- **状态管理**: Pinia
- **UI**: 自定义组件（无第三方 UI 库依赖）
- **多端支持**: App (Android/iOS) + H5 + 微信小程序

## 项目结构

```
afaadmin-mobile/
├── src/
│   ├── api/              # API 请求层
│   │   ├── types.ts      # 类型定义（与 Web 端共享 DTO）
│   │   ├── request.ts    # uni.request 统一封装 + Token 自动刷新
│   │   ├── auth.ts       # 认证接口
│   │   ├── user.ts       # 用户管理
│   │   ├── role.ts       # 角色管理
│   │   ├── dept.ts       # 部门管理
│   │   └── dict.ts       # 字典管理
│   ├── store/            # Pinia 状态管理
│   │   └── modules/
│   │       ├── user.ts   # 用户状态 + 权限
│   │       └── app.ts    # 应用状态 + 字典缓存
│   ├── utils/            # 工具函数
│   │   ├── auth.ts       # Token 持久化 (uni.setStorageSync)
│   │   ├── index.ts      # 通用工具
│   │   └── platform.ts   # 平台判断 + 状态栏高度
│   ├── pages/            # 页面
│   │   ├── login/        # 登录
│   │   ├── workbench/    # 工作台首页
│   │   ├── user/         # 用户管理（列表 + 详情）
│   │   ├── role/         # 角色管理
│   │   ├── dept/         # 部门管理（树形折叠）
│   │   ├── dict/         # 字典管理（类型 + 数据）
│   │   └── profile/      # 个人中心 + 修改密码
│   ├── static/           # 静态资源
│   ├── App.vue           # 应用入口
│   ├── main.ts           # 主入口
│   ├── manifest.json     # UniApp 配置
│   ├── pages.json        # 页面路由 + TabBar
│   └── uni.scss          # 全局样式变量
├── package.json
├── vite.config.ts
└── tsconfig.json
```

## 核心特性

### 1. 统一请求封装
- 基于 `uni.request` 封装，自动携带 Token
- Token 过期自动刷新（RefreshToken 机制）
- 失败重试 + 请求队列管理
- 多环境 BaseURL 切换（dev / staging / prod）
- H5 走 Vite Proxy，App 走直连

### 2. 权限控制
- Pinia 存储用户权限列表
- `userStore.hasPermission('sys:user:add')` 按钮级权限判断
- 超级管理员 `*:*:*` 通配

### 3. Token 持久化
- `uni.setStorageSync` 跨端存储
- App 启动自动恢复登录状态
- Token 过期自动跳转登录页

### 4. 移动端适配
- 自定义导航栏适配状态栏高度
- 下拉刷新 + 上拉加载
- 树形数据折叠展开交互

### 5. 与 Web 端复用
- API 类型定义 (`types.ts`) 与 Web 端 DTO 保持一致
- 工具函数（日期格式化、防抖等）可抽取为共享包
- 统一 API 错误码

## 快速开始

### 环境要求
- Node.js >= 18
- HBuilderX（可选，用于 App 打包）

### 开发调试

```bash
# 安装依赖
npm install

# H5 开发模式（推荐前后端联调）
npm run dev:h5
# 访问 http://localhost:5173

# App 开发模式
npm run dev:app

# 微信小程序
npm run dev:mp-weixin
```

### 构建

```bash
# H5
npm run build:h5

# App (需要 HBuilderX 云打包)
npm run build:app

# 微信小程序
npm run build:mp-weixin
```

### 后端配置

确保后端 API 运行在 `http://localhost:5000`，H5 开发模式通过 Vite Proxy 自动转发 `/api` 请求。

App 端需修改 `src/api/request.ts` 中的 `ENV_CONFIG.development` 为实际后端地址。

## 默认账号

- 用户名: `admin`
- 密码: `admin123`

## 待完善（后续迭代）

- [ ] 扫码功能（资产盘点场景）
- [ ] 消息推送集成（UniPush 2.0）
- [ ] 语音输入（对接 AI Copilot）
- [ ] 离线缓存策略
- [ ] 生物识别登录
- [ ] 微信小程序 openid 绑定
- [ ] Tab 图标替换为正式 SVG 图标
