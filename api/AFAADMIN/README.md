# AFAADMIN — 模块化单体管理后台

基于 **.NET 8 + Vue3 + UniApp + Electron** 的模块化单体（Modular Monolith）全端管理后台系统，采用 DDD 分层架构，支持 Web、移动端、桌面端三端协同。

## 技术栈

| 层面     | 技术                                                         |
| -------- | ------------------------------------------------------------ |
| 后端     | .NET 8 WebAPI · SqlSugar ORM · DDD 分层 · MediatR 事件总线   |
| 安全     | 国密 SM3/SM4 · JWT 认证 · RBAC 权限 · API 报文加解密 · 限流防抖 |
| 缓存     | Redis（Token 黑名单 / 权限缓存 / 字典缓存 / AI 会话）        |
| 存储     | 本地文件 / MinIO 对象存储（可切换）                          |
| AI       | Semantic Kernel · Function Calling · SSE 流式输出            |
| Web 前端 | Vue3 + Vite + TypeScript + Arco Design Vue                   |
| 移动端   | UniApp (Vue3) · 多端支持 (App / H5 / 微信小程序)             |
| 桌面端   | Electron + electron-vite · 自定义标题栏 / 系统托盘 / 自动更新 |
| 部署     | Docker + Nginx + GitLab CI/CD                                |

## 工程结构

```
AFAADMIN/
├── api/AFAADMIN/                      # 后端 .NET 8
│   ├── src/
│   │   ├── Host/AFAADMIN.WebApi/      # 启动入口 (Program.cs、Controllers、configs/)
│   │   ├── Framework/                 # 底层基座
│   │   │   ├── AFAADMIN.Common        # 通用模型、异常、DI 标记、国密工具、雪花 ID
│   │   │   ├── AFAADMIN.Database      # SqlSugar 封装、泛型仓储、敏感字段加解密
│   │   │   ├── AFAADMIN.Web.Core      # 全局过滤器、JWT、限流、Redis、Swagger
│   │   │   ├── AFAADMIN.Storage       # 文件存储 (Local / MinIO)
│   │   │   ├── AFAADMIN.EventBus      # MediatR 事件总线
│   │   │   ├── AFAADMIN.AI            # Semantic Kernel AI Copilot
│   │   │   └── AFAADMIN.Tools         # 图片处理、二维码、验证码
│   │   └── Modules/
│   │       └── System/                # RBAC 系统模块 (DDD 三层)
│   │           ├── Domain             # 实体、领域事件
│   │           ├── Application        # DTO、服务、校验、事件处理
│   │           └── Infrastructure     # 种子数据初始化
│   ├── deploy/nginx/                  # Nginx 反向代理配置
│   ├── Dockerfile
│   ├── docker-compose.yml
│   └── .gitlab-ci.yml
├── web/afaadmin-web/                  # Vue3 Web 前端
├── uniapp/afaadmin-mobile/            # UniApp 移动端
└── desktop/afaadmin-desktop/          # Electron 桌面端
```

## 快速开始

### 环境要求

- .NET 8 SDK
- Node.js >= 18
- MySQL 8.0+ (或 PostgreSQL 14+ / SQL Server)
- Redis 7+
- Docker & Docker Compose (部署用)

### 后端运行

```bash
cd api/AFAADMIN

# 还原依赖
dotnet restore

# 配置数据库连接
# 编辑 src/Host/AFAADMIN.WebApi/configs/database.json

# 配置安全密钥
# 复制 configs/security.json.example → configs/security.json 并修改

# 运行
cd src/Host/AFAADMIN.WebApi
dotnet run

# Swagger: http://localhost:5000/swagger
```

### Web 前端运行

```bash
cd web/afaadmin-web
npm install
npm run dev
# 访问 http://localhost:3000
```

### 移动端运行

```bash
cd uniapp/afaadmin-mobile
npm install
npm run dev:h5    # H5 模式
# 访问 http://localhost:5173
```

### 桌面端运行

```bash
cd desktop/afaadmin-desktop
npm install
npm run dev
```

### Docker 部署

```bash
cd api/AFAADMIN
docker-compose up -d --build
# API: http://localhost:5000
# Nginx: http://localhost:80
```

### 默认账号

- 用户名: `admin`
- 密码: `admin123`

## 配置说明

配置文件分离存放于 `src/Host/AFAADMIN.WebApi/configs/` 目录：

| 文件            | 用途                                                   |
| --------------- | ------------------------------------------------------ |
| `database.json` | 数据库连接（DbType、ConnectionString）                 |
| `redis.json`    | Redis 缓存连接                                         |
| `security.json` | JWT 密钥 / SM4 密钥 / 限流规则 (**已加入 .gitignore**) |
| `storage.json`  | 文件存储引擎（Local / MinIO）                          |
| `ai.json`       | AI Copilot 配置（Provider / ApiKey / ModelId）         |

> 首次使用请复制 `security.json.example` → `security.json` 并修改密钥。

## API 接口一览

### 认证

| 方法 | 路径                 | 说明                     |
| ---- | -------------------- | ------------------------ |
| POST | `/api/auth/login`    | 登录                     |
| POST | `/api/auth/refresh`  | 刷新 Token               |
| GET  | `/api/auth/userinfo` | 当前用户信息             |
| POST | `/api/auth/logout`   | 登出（Token 加入黑名单） |

### 用户管理

| 方法   | 路径                              | 权限                |
| ------ | --------------------------------- | ------------------- |
| GET    | `/api/system/user/page`           | `sys:user:list`     |
| GET    | `/api/system/user/{id}`           | `sys:user:list`     |
| POST   | `/api/system/user`                | `sys:user:add`      |
| PUT    | `/api/system/user`                | `sys:user:edit`     |
| DELETE | `/api/system/user/{id}`           | `sys:user:delete`   |
| PUT    | `/api/system/user/reset-password` | `sys:user:resetpwd` |
| PUT    | `/api/system/user/{id}/roles`     | `sys:user:edit`     |

### 角色 / 菜单 / 部门 / 字典

遵循相同 RESTful 风格，权限标识遵循 `sys:resource:action` 规范。

### 文件管理

| 方法   | 路径                                  | 说明       |
| ------ | ------------------------------------- | ---------- |
| POST   | `/api/system/file/upload`             | 单文件上传 |
| POST   | `/api/system/file/upload/batch`       | 批量上传   |
| GET    | `/api/system/file/download?filePath=` | 文件下载   |
| DELETE | `/api/system/file?filePath=`          | 删除文件   |

### AI Copilot

| 方法   | 路径                           | 说明         |
| ------ | ------------------------------ | ------------ |
| POST   | `/api/ai/chat`                 | 非流式聊天   |
| POST   | `/api/ai/chat/stream`          | SSE 流式输出 |
| GET    | `/api/ai/sessions`             | 会话列表     |
| GET    | `/api/ai/session/{id}/history` | 会话历史     |
| DELETE | `/api/ai/session/{id}`         | 删除会话     |
| GET    | `/api/ai/status`               | AI 服务状态  |

### 验证码 / 工具

| 方法 | 路径                           | 说明           |
| ---- | ------------------------------ | -------------- |
| GET  | `/api/captcha`                 | 获取图形验证码 |
| POST | `/api/captcha/verify`          | 校验验证码     |
| GET  | `/api/captcha/qrcode?content=` | 生成二维码     |

## 核心架构特性

- **自动 DI 注入**：实现 `IScopedDependency` / `ITransientDependency` / `ISingletonDependency` 标记接口的类自动注册
- **统一响应**：所有接口返回 `ApiResult<T>`（Code / Message / Data / Timestamp）
- **全局异常处理**：`GlobalExceptionFilter` 捕获 `BusinessException` 和未知异常
- **敏感字段加密**：`[SensitiveField]` 属性标记的字段自动 SM4-ECB 加解密
- **权限控制**：`[Permission("sys:user:add")]` 按钮级权限，超级管理员（`admin` 角色）自动放行
- **缓存策略**：权限缓存 30 分钟、字典缓存 2 小时，业务变更事件自动清除
- **事件驱动**：MediatR 进程内事件总线，用户/角色/字典变更自动触发缓存失效

## 三端 AI 助手能力

| 能力     | Web            | App      | 桌面                   |
| -------- | -------------- | -------- | ---------------------- |
| 对话入口 | 右下角悬浮按钮 | 独立页面 | 全局悬浮窗 (预留)      |
| 输入方式 | 键盘           | 键盘     | 键盘 + 文件拖拽 (预留) |
| 流式输出 | SSE            | 非流式   | SSE                    |
| 预设问题 | ✅              | ✅        | ✅                      |

## 开发里程碑

| 里程碑         | 状态 | 核心产出                                                     |
| -------------- | ---- | ------------------------------------------------------------ |
| M1 工程骨架    | ✅    | 项目结构、统一响应、异常处理、Serilog、Swagger、Docker       |
| M2 数据+安全   | ✅    | SqlSugar 多库、SM3/SM4 国密、API 加解密中间件、限流          |
| M3 RBAC 权限   | ✅    | 用户/角色/菜单/部门/字典 CRUD、JWT 认证、权限过滤器、种子数据 |
| M4 基础设施    | ✅    | Redis 缓存、文件存储、MediatR 事件总线、图片/二维码/验证码、雪花 ID |
| M5 Web 前端    | ✅    | Vue3 + Arco Design 管理后台、动态路由、按钮权限指令          |
| M6 移动端 App  | ✅    | UniApp 跨平台 App (App / H5 / 小程序)                        |
| M7 桌面端      | ✅    | Electron 桌面应用、系统托盘、全局快捷键、自动更新            |
| M8 AI + DevOps | ✅    | Semantic Kernel AI Copilot、三端对话、GitLab CI 全端构建     |

## CI/CD

GitLab CI 覆盖全部构建产物：

| Stage           | 产物                                |
| --------------- | ----------------------------------- |
| `build-api`     | .NET 后端编译 + Docker 镜像         |
| `build-web`     | Vue3 前端 dist 静态资源             |
| `build-app-h5`  | UniApp H5 版本                      |
| `build-desktop` | Electron 安装包 (Win / Mac / Linux) |
| `test`          | 后端单元测试                        |
| `deploy`        | 手动部署到 ACK                      |

## 许可证

[Apache License 2.0](https://claude.ai/chat/LICENSE)
