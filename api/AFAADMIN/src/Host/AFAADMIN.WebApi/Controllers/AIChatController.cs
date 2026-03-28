using System.Text.Json;
using AFAADMIN.AI.Config;
using AFAADMIN.AI.Models;
using AFAADMIN.AI.Services;
using AFAADMIN.Common.Models;
using AFAADMIN.Web.Core.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AFAADMIN.WebApi.Controllers;

/// <summary>
/// AI Copilot 聊天接口
/// </summary>
[Route("api/ai")]
[Authorize]
public class AIChatController : ApiControllerBase
{
    private readonly IAIChatService _chatService;
    private readonly IChatSessionService _sessionService;
    private readonly AIConfig _config;

    public AIChatController(IAIChatService chatService,
        IChatSessionService sessionService, AIConfig config)
    {
        _chatService = chatService;
        _sessionService = sessionService;
        _config = config;
    }

    /// <summary>
    /// 发送消息（非流式）
    /// </summary>
    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request)
    {
        if (!_config.Enabled)
            return Ok(ApiResult<string>.Fail("AI 助手未启用", 400));

        if (string.IsNullOrWhiteSpace(request.Message))
            return Ok(ApiResult.Fail("消息不能为空"));

        var userId = HttpContext.GetUserId();
        var result = await _chatService.ChatAsync(userId, request);
        return Ok(ApiResult<ChatResponse>.Success(result));
    }

    /// <summary>
    /// 流式对话（SSE）
    /// </summary>
    [HttpPost("chat/stream")]
    public async Task ChatStream([FromBody] ChatRequest request)
    {
        Response.ContentType = "text/event-stream";
        Response.Headers.CacheControl = "no-cache";
        Response.Headers.Connection = "keep-alive";

        if (!_config.Enabled)
        {
            await WriteSSEAsync(new ChatStreamEvent
            {
                Event = "error",
                Content = "AI 助手未启用"
            });
            return;
        }

        var userId = HttpContext.GetUserId();
        var cancellationToken = HttpContext.RequestAborted;

        try
        {
            await foreach (var evt in _chatService.ChatStreamAsync(
                userId, request, cancellationToken))
            {
                await WriteSSEAsync(evt);
            }
        }
        catch (OperationCanceledException)
        {
            // 客户端断开
        }
        catch (Exception ex)
        {
            await WriteSSEAsync(new ChatStreamEvent
            {
                Event = "error",
                Content = "服务异常，请稍后再试"
            });
        }
    }

    /// <summary>
    /// 获取会话列表
    /// </summary>
    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessions()
    {
        var userId = HttpContext.GetUserId();
        var sessions = await _sessionService.GetSessionListAsync(userId);
        return Ok(ApiResult<List<ChatSessionInfo>>.Success(sessions));
    }

    /// <summary>
    /// 获取会话历史
    /// </summary>
    [HttpGet("session/{sessionId}/history")]
    public async Task<IActionResult> GetHistory(string sessionId)
    {
        var history = await _sessionService.GetHistoryAsync(sessionId);
        return Ok(ApiResult<List<ChatHistoryItem>>.Success(history));
    }

    /// <summary>
    /// 删除会话
    /// </summary>
    [HttpDelete("session/{sessionId}")]
    public async Task<IActionResult> DeleteSession(string sessionId)
    {
        await _sessionService.DeleteSessionAsync(sessionId);
        return Ok(ApiResult.Success("会话已删除"));
    }

    /// <summary>
    /// AI 服务状态
    /// </summary>
    [HttpGet("status")]
    [AllowAnonymous]
    public IActionResult GetStatus()
    {
        return Ok(ApiResult<object>.Success(new
        {
            Enabled = _config.Enabled,
            Provider = _config.Enabled ? _config.Provider : "N/A",
            Model = _config.Enabled ? _config.ModelId : "N/A"
        }));
    }

    private async Task WriteSSEAsync(ChatStreamEvent evt)
    {
        var json = JsonSerializer.Serialize(evt, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await Response.WriteAsync($"data: {json}\n\n");
        await Response.Body.FlushAsync();
    }
}
