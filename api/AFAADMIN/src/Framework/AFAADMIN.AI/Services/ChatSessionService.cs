using System.Text.Json;
using AFAADMIN.AI.Config;
using AFAADMIN.AI.Models;
using AFAADMIN.Common.Cache;
using AFAADMIN.Common.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.AI.Services;

/// <summary>
/// 基于 Redis 的会话管理实现
/// </summary>
public class ChatSessionService : IChatSessionService, IScopedDependency
{
    private readonly ICacheService _cache;
    private readonly AIConfig _config;
    private readonly ILogger<ChatSessionService> _logger;

    private const string SessionPrefix = "afa:ai:session:";
    private const string UserSessionsPrefix = "afa:ai:user_sessions:";

    public ChatSessionService(ICacheService cache, AIConfig config,
        ILogger<ChatSessionService> logger)
    {
        _cache = cache;
        _config = config;
        _logger = logger;
    }

    public async Task<string> CreateSessionAsync(long userId)
    {
        var sessionId = $"s_{userId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
        var info = new ChatSessionInfo
        {
            SessionId = sessionId,
            Title = "新对话",
            CreatedAt = DateTime.Now,
            LastActiveAt = DateTime.Now,
            MessageCount = 0
        };

        var expiry = TimeSpan.FromMinutes(_config.SessionExpireMinutes);
        await _cache.SetAsync($"{SessionPrefix}{sessionId}:info", info, expiry);
        await _cache.SetAsync($"{SessionPrefix}{sessionId}:history",
            new List<ChatHistoryItem>(), expiry);

        // 将 sessionId 加入用户的会话列表
        var userSessions = await _cache.GetAsync<List<string>>($"{UserSessionsPrefix}{userId}")
            ?? new List<string>();
        userSessions.Insert(0, sessionId);
        if (userSessions.Count > 50) userSessions = userSessions.Take(50).ToList();
        await _cache.SetAsync($"{UserSessionsPrefix}{userId}", userSessions,
            TimeSpan.FromDays(7));

        _logger.LogDebug("创建 AI 会话: {SessionId} for user {UserId}", sessionId, userId);
        return sessionId;
    }

    public async Task<List<ChatHistoryItem>> GetHistoryAsync(string sessionId)
    {
        return await _cache.GetAsync<List<ChatHistoryItem>>(
            $"{SessionPrefix}{sessionId}:history") ?? new List<ChatHistoryItem>();
    }

    public async Task AppendMessageAsync(string sessionId, string role, string content)
    {
        var history = await GetHistoryAsync(sessionId);
        history.Add(new ChatHistoryItem { Role = role, Content = content, Time = DateTime.Now });

        // 限制历史消息数量
        if (history.Count > _config.MaxHistoryMessages)
            history = history.Skip(history.Count - _config.MaxHistoryMessages).ToList();

        var expiry = TimeSpan.FromMinutes(_config.SessionExpireMinutes);
        await _cache.SetAsync($"{SessionPrefix}{sessionId}:history", history, expiry);

        // 更新会话信息
        var info = await _cache.GetAsync<ChatSessionInfo>($"{SessionPrefix}{sessionId}:info");
        if (info != null)
        {
            info.LastActiveAt = DateTime.Now;
            info.MessageCount = history.Count;
            // 用第一条用户消息作为标题
            if (info.Title == "新对话" && role == "user")
                info.Title = content.Length > 20 ? content[..20] + "..." : content;
            await _cache.SetAsync($"{SessionPrefix}{sessionId}:info", info, expiry);
        }
    }

    public async Task<List<ChatSessionInfo>> GetSessionListAsync(long userId)
    {
        var sessionIds = await _cache.GetAsync<List<string>>($"{UserSessionsPrefix}{userId}")
            ?? new List<string>();

        var result = new List<ChatSessionInfo>();
        foreach (var sid in sessionIds)
        {
            var info = await _cache.GetAsync<ChatSessionInfo>($"{SessionPrefix}{sid}:info");
            if (info != null) result.Add(info);
        }
        return result;
    }

    public async Task DeleteSessionAsync(string sessionId)
    {
        await _cache.RemoveAsync($"{SessionPrefix}{sessionId}:info");
        await _cache.RemoveAsync($"{SessionPrefix}{sessionId}:history");
    }

    public async Task UpdateTitleAsync(string sessionId, string title)
    {
        var info = await _cache.GetAsync<ChatSessionInfo>($"{SessionPrefix}{sessionId}:info");
        if (info != null)
        {
            info.Title = title;
            var expiry = TimeSpan.FromMinutes(_config.SessionExpireMinutes);
            await _cache.SetAsync($"{SessionPrefix}{sessionId}:info", info, expiry);
        }
    }
}
