using AFAADMIN.AI.Models;

namespace AFAADMIN.AI.Services;

/// <summary>
/// AI 聊天服务接口
/// </summary>
public interface IAIChatService
{
    /// <summary>
    /// 发送消息（非流式）
    /// </summary>
    Task<ChatResponse> ChatAsync(long userId, ChatRequest request);

    /// <summary>
    /// 流式发送消息（SSE）
    /// </summary>
    IAsyncEnumerable<ChatStreamEvent> ChatStreamAsync(long userId, ChatRequest request,
        CancellationToken cancellationToken = default);
}
