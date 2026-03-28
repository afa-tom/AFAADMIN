using AFAADMIN.AI.Config;
using AFAADMIN.AI.Models;
using AFAADMIN.Common.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Runtime.CompilerServices;

namespace AFAADMIN.AI.Services;

/// <summary>
/// 基于 Semantic Kernel 的 AI 聊天服务
/// </summary>
public class AIChatService : IAIChatService, IScopedDependency
{
    private readonly Kernel _kernel;
    private readonly AIConfig _config;
    private readonly IChatSessionService _sessionService;
    private readonly ILogger<AIChatService> _logger;

    public AIChatService(Kernel kernel, AIConfig config,
        IChatSessionService sessionService, ILogger<AIChatService> logger)
    {
        _kernel = kernel;
        _config = config;
        _sessionService = sessionService;
        _logger = logger;
    }

    public async Task<ChatResponse> ChatAsync(long userId, ChatRequest request)
    {
        var sessionId = request.SessionId;
        if (string.IsNullOrEmpty(sessionId))
            sessionId = await _sessionService.CreateSessionAsync(userId);

        // 构建聊天历史
        var chatHistory = await BuildChatHistoryAsync(sessionId);
        chatHistory.AddUserMessage(request.Message);

        // 保存用户消息
        await _sessionService.AppendMessageAsync(sessionId, "user", request.Message);

        try
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var settings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = _config.MaxTokens,
                Temperature = _config.Temperature,
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
            };

            var result = await chatService.GetChatMessageContentAsync(
                chatHistory, settings, _kernel);

            var responseText = result.Content ?? "";
            await _sessionService.AppendMessageAsync(sessionId, "assistant", responseText);

            return new ChatResponse
            {
                SessionId = sessionId,
                Message = responseText,
                DataType = "text"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI 聊天异常");
            return new ChatResponse
            {
                SessionId = sessionId,
                Message = "抱歉，AI 助手暂时无法响应，请稍后再试。",
                DataType = "text"
            };
        }
    }

    public async IAsyncEnumerable<ChatStreamEvent> ChatStreamAsync(
        long userId, ChatRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var sessionId = request.SessionId;
        if (string.IsNullOrEmpty(sessionId))
            sessionId = await _sessionService.CreateSessionAsync(userId);

        // 发送开始事件
        yield return new ChatStreamEvent
        {
            Event = "start",
            SessionId = sessionId
        };

        var chatHistory = await BuildChatHistoryAsync(sessionId);
        chatHistory.AddUserMessage(request.Message);
        await _sessionService.AppendMessageAsync(sessionId, "user", request.Message);

        var fullResponse = new System.Text.StringBuilder();

        ChatStreamEvent? errorEvent = null;
        bool hasError = false;

        var chatService = _kernel.GetRequiredService<IChatCompletionService>();
        var settings = new OpenAIPromptExecutionSettings
        {
            MaxTokens = _config.MaxTokens,
            Temperature = _config.Temperature,
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };

        IAsyncEnumerable<StreamingChatMessageContent>? stream = null;
        try
        {
            stream = chatService.GetStreamingChatMessageContentsAsync(
                chatHistory, settings, _kernel, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AI 流式聊天初始化异常");
            hasError = true;
            errorEvent = new ChatStreamEvent
            {
                Event = "error",
                Content = "AI 助手暂时无法响应，请稍后再试。"
            };
        }

        if (hasError)
        {
            yield return errorEvent!;
            yield break;
        }

        await foreach (var chunk in stream!.WithCancellation(cancellationToken))
        {
            if (cancellationToken.IsCancellationRequested) break;

            var content = chunk.Content;
            if (!string.IsNullOrEmpty(content))
            {
                fullResponse.Append(content);
                yield return new ChatStreamEvent
                {
                    Event = "content",
                    Content = content
                };
            }
        }

        // 保存完整响应
        var responseText = fullResponse.ToString();
        if (!string.IsNullOrEmpty(responseText))
            await _sessionService.AppendMessageAsync(sessionId, "assistant", responseText);

        yield return new ChatStreamEvent
        {
            Event = "end",
            SessionId = sessionId
        };
    }

    /// <summary>
    /// 从 Redis 加载历史并构建 SK ChatHistory
    /// </summary>
    private async Task<ChatHistory> BuildChatHistoryAsync(string sessionId)
    {
        var chatHistory = new ChatHistory(_config.SystemPrompt);
        var history = await _sessionService.GetHistoryAsync(sessionId);

        foreach (var item in history)
        {
            if (item.Role == "user")
                chatHistory.AddUserMessage(item.Content);
            else if (item.Role == "assistant")
                chatHistory.AddAssistantMessage(item.Content);
        }

        return chatHistory;
    }
}
