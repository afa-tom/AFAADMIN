# AFAADMIN - 模块化单体管理后台

基于 .NET 8 + Vue3 的模块化单体（Modular Monolith）管理后台系统。

## 技术栈

- **后端**: .NET 8 WebAPI + SqlSugar + DDD 分层
- **前端**: Vue3 + Vite + Arco Design Vue (M5 阶段)
- **部署**: Docker + Nginx + 阿里云 ACK
- **安全**: 国密 SM3/SM4 + JWT + 限流防抖

## 快速开始

### 1. 环境要求

- .NET 8 SDK
- Docker & Docker Compose (部署用)
- MySQL 8.0+ / PostgreSQL 14+ (M2 阶段)

### 2. 本地运行

```bash
# 还原依赖
dotnet restore

# 运行（开发模式）
cd src/Host/AFAADMIN.WebApi
dotnet run

# 访问 Swagger: http://localhost:5000/swagger
```

### 3. Docker 部署

```bash
docker-compose up -d --build
# API: http://localhost:5000
# Nginx: http://localhost:80
```

### 4. 验证接口

```bash
# 心跳检测
curl http://localhost:5000/api/system/ping

# 测试统一响应
curl http://localhost:5000/api/system/test/success

# 测试业务异常
curl http://localhost:5000/api/system/test/biz-error

# 测试配置加载
curl http://localhost:5000/api/system/test/config
```

## 工程结构

```
AFAADMIN/
├── src/
│   ├── Host/AFAADMIN.WebApi          # 启动入口
│   ├── Framework/                     # 底层基座
│   │   ├── AFAADMIN.Common           # 通用模型、异常、DI标记
│   │   ├── AFAADMIN.Web.Core         # 过滤器、Serilog、Swagger
│   │   ├── AFAADMIN.Database         # SqlSugar 封装 (M2)
│   │   ├── AFAADMIN.Storage          # 文件存储 (M4)
│   │   ├── AFAADMIN.EventBus         # 事件总线 (M4)
│   │   ├── AFAADMIN.AI               # AI Copilot (M6)
│   │   └── AFAADMIN.Tools            # 工具链 (M4)
│   ├── Modules/
│   │   └── System/                   # RBAC 权限模块 (M3)
│   │       ├── Domain                # 领域层
│   │       ├── Application           # 应用层
│   │       └── Infrastructure        # 基础设施层
│   └── Frontend/                     # 前端工程 (M5)
├── deploy/nginx/                     # Nginx 配置
├── Dockerfile
├── docker-compose.yml
└── .gitlab-ci.yml
```

## 配置说明

配置文件分离存放于 `src/Host/AFAADMIN.WebApi/configs/` 目录:

| 文件 | 用途 |
|------|------|
| `database.json` | 数据库连接 |
| `redis.json` | Redis 缓存 |
| `security.json` | JWT / 加密 / 限流 (已加入 .gitignore) |

> **注意**: `security.json` 包含密钥，已在 `.gitignore` 中排除。首次使用请复制 `security.json.example` 并修改。

## 开发里程碑

- [x] M1: 工程骨架搭建
- [ ] M2: 数据层 + 安全基建
- [ ] M3: RBAC 权限模块
- [ ] M4: 基础设施补全
- [ ] M5: 前端工程
- [ ] M6: AI Copilot + DevOps
