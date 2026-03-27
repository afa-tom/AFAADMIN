# AFAADMIN M4 交付说明

## 新增功能

### 1. Redis 缓存集成
- `ICacheService` 统一接口 + `RedisCacheService` 实现
- `CacheKeys` 集中管理缓存 Key
- Token 黑名单中间件（登出后 Token 立即失效）
- RefreshToken 从内存迁移至 Redis
- 权限缓存（30分钟过期，角色变更自动失效）
- 字典缓存（2小时过期，数据变更自动清除）

### 2. 文件存储引擎
- `IStorageProvider` 统一接口
- `LocalStorageProvider` 本地存储
- `MinioStorageProvider` 对象存储
- 文件上传/下载/删除 API
- 通过配置一键切换存储后端

### 3. 工具链
- ImageSharp 图片压缩/缩放/缩略图
- QRCoder 二维码生成
- Lazy.Captcha 图形验证码

### 4. MediatR 事件总线
- `IEventPublisher` 发布/订阅
- 领域事件: UserCreated, UserRoleChanged, RolePermissionChanged, DictDataChanged
- 缓存失效处理器自动清除关联缓存

### 5. 雪花算法 ID 生成器
- `SnowflakeIdGenerator` 分布式唯一 ID
- `IdHelper` 全局静态入口

## 新增接口
- `POST /api/captcha` — 获取图形验证码
- `POST /api/captcha/verify` — 校验验证码
- `GET /api/captcha/qrcode?content=xxx` — 生成二维码
- `POST /api/system/file/upload` — 文件上传
- `POST /api/system/file/upload/batch` — 批量上传
- `GET /api/system/file/download?filePath=xxx` — 文件下载
- `DELETE /api/system/file?filePath=xxx` — 删除文件
- `GET /api/system/m4/redis` — Redis 连接测试
- `GET /api/system/m4/snowflake` — 雪花 ID 测试
- `GET /api/system/m4/storage` — 存储引擎测试

## Docker 部署
docker-compose 新增 Redis 容器，配置持久化。
