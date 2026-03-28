using AFAADMIN.AI.Models;

namespace AFAADMIN.AI.Services;

/// <summary>
/// 聊天会话管理接口
/// </summary>
public interface IChatSessionService
{
    /// <summary>
    /// 创建新会话
    /// </summary>
    Task<string> CreateSessionAsync(long userId);

    /// <summary>
    /// 获取会话历史
    /// </summary>
    Task<List<ChatHistoryItem>> GetHistoryAsync(string sessionId);

    /// <summary>
    /// 追加消息到会话
    /// </summary>
    Task AppendMessageAsync(string sessionId, string role, string content);

    /// <summary>
    /// 获取用户的会话列表
    /// </summary>
    Task<List<ChatSessionInfo>> GetSessionListAsync(long userId);

    /// <summary>
    /// 删除会话
    /// </summary>
    Task DeleteSessionAsync(string sessionId);

    /// <summary>
    /// 更新会话标题
    /// </summary>
    Task UpdateTitleAsync(string sessionId, string title);
}
