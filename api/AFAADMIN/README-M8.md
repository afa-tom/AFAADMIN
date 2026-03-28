# AFAADMIN M8 交付说明

## AI Copilot 后端

### 1. Semantic Kernel 集成
- `AFAADMIN.AI` 模块集成 Microsoft Semantic Kernel
- 支持 OpenAI / Azure OpenAI / 兼容 API 提供商
- 配置文件: `configs/ai.json`

### 2. Native Functions（Function Calling）
- `get_user_count` — 查询用户总数
- `get_role_list` — 获取角色列表
- `get_dept_tree` — 获取部门结构
- `search_users` — 搜索用户
- `get_dict_data` — 查询字典数据
- `get_system_overview` — 系统总览统计
- `get_recent_users` — 最近创建的用户

### 3. 对话式 API
- `POST /api/ai/chat` — 非流式聊天
- `POST /api/ai/chat/stream` — SSE 流式输出
- `GET /api/ai/sessions` — 会话列表
- `GET /api/ai/session/{id}/history` — 会话历史
- `DELETE /api/ai/session/{id}` — 删除会话
- `GET /api/ai/status` — AI 服务状态

### 4. 会话管理
- 基于 Redis 的会话历史存储
- 支持多轮上下文连续对话
- 自动生成会话标题
- 会话过期自动清理

## AI 助手 — Web 端
- 右下角悬浮按钮呼出 AI 对话面板
- SSE 流式逐字输出
- 预设快捷问题
- 多轮对话支持

## AI 助手 — 移动端 App
- 工作台首页 AI 快捷入口
- 独立 AI 对话页面
- 非流式请求（兼容小程序）
- 适配移动端交互

## AI 助手 — 桌面端
- `Ctrl+Shift+Space` 全局快捷键唤出（M7 已预留）
- 后续扩展: 剪贴板智能分析、本地文件拖拽上下文

## DevOps
- `.gitlab-ci.yml` 覆盖全部构建产物
  - `build-api` — .NET 后端 + Docker 镜像
  - `build-web` — Vue3 前端 dist
  - `build-app-h5` — UniApp H5 版本
  - `build-desktop` — Electron 安装包
  - `test` — 单元测试
  - `deploy` — 手动部署

## 配置说明

### configs/ai.json
```json
{
  "AI": {
    "Enabled": true,
    "Provider": "OpenAI",
    "ModelId": "gpt-4o-mini",
    "ApiKey": "sk-your-api-key",
    "Endpoint": "",
    "SystemPrompt": "...",
    "MaxHistoryMessages": 20,
    "SessionExpireMinutes": 60
  }
}
```

Provider 支持:
- `OpenAI` — 标准 OpenAI API
- `AzureOpenAI` — Azure 部署
- 自定义 Endpoint 兼容第三方 API（如 DeepSeek、通义千问等）

## 三端 AI 能力差异

| 能力 | Web | App | 桌面 |
|------|-----|-----|------|
| 对话面板 | 右下角悬浮 | 独立页面 | 全局悬浮窗(预留) |
| 输入方式 | 键盘 | 键盘 | 键盘+文件拖拽(预留) |
| 流式输出 | SSE ✅ | 非流式 | SSE ✅ |
| 预设问题 | ✅ | ✅ | ✅ |
