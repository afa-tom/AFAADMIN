namespace AFAADMIN.AI.Models;

/// <summary>
/// 聊天请求
/// </summary>
public class ChatRequest
{
    /// <summary>
    /// 会话 ID（为空则创建新会话）
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// 用户消息
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 聊天响应
/// </summary>
public class ChatResponse
{
    public string SessionId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 结构化数据（表格/图表/操作指令）
    /// </summary>
    public object? StructuredData { get; set; }

    /// <summary>
    /// 数据类型: text / table / chart / action
    /// </summary>
    public string DataType { get; set; } = "text";
}

/// <summary>
/// SSE 流式事件
/// </summary>
public class ChatStreamEvent
{
    /// <summary>
    /// 事件类型: start / content / function_call / end / error
    /// </summary>
    public string Event { get; set; } = "content";

    /// <summary>
    /// 内容片段
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 会话 ID
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    /// 函数调用信息
    /// </summary>
    public string? FunctionName { get; set; }

    /// <summary>
    /// 结构化数据
    /// </summary>
    public object? Data { get; set; }
}

/// <summary>
/// 会话历史记录
/// </summary>
public class ChatHistoryItem
{
    public string Role { get; set; } = string.Empty;  // user / assistant
    public string Content { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.Now;
}

/// <summary>
/// 会话信息
/// </summary>
public class ChatSessionInfo
{
    public string SessionId { get; set; } = string.Empty;
    public string Title { get; set; } = "新对话";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime LastActiveAt { get; set; } = DateTime.Now;
    public int MessageCount { get; set; }
}
