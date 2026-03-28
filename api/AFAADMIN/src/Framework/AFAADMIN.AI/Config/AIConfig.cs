namespace AFAADMIN.AI.Config;

/// <summary>
/// AI Copilot 配置（映射 configs/ai.json）
/// </summary>
public class AIConfig
{
    /// <summary>
    /// 是否启用 AI 助手
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// 大模型提供商: OpenAI / AzureOpenAI / Custom
    /// </summary>
    public string Provider { get; set; } = "OpenAI";

    /// <summary>
    /// 模型名称
    /// </summary>
    public string ModelId { get; set; } = "gpt-4o-mini";

    /// <summary>
    /// API Key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// API Endpoint（自定义提供商或 Azure 时使用）
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// 系统提示词
    /// </summary>
    public string SystemPrompt { get; set; } = "你是 AFAADMIN 系统的 AI 助手。你可以帮助用户查询系统信息、管理用户、角色、部门等。请用中文回答。";

    /// <summary>
    /// 单次会话最大历史消息数
    /// </summary>
    public int MaxHistoryMessages { get; set; } = 20;

    /// <summary>
    /// 会话过期时间（分钟）
    /// </summary>
    public int SessionExpireMinutes { get; set; } = 60;

    /// <summary>
    /// 最大 Token 数
    /// </summary>
    public int MaxTokens { get; set; } = 2048;

    /// <summary>
    /// Temperature (0-2)
    /// </summary>
    public double Temperature { get; set; } = 0.7;
}
