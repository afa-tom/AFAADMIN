using AFAADMIN.Common.Cache;
using AFAADMIN.System.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.System.Application.EventHandlers;

/// <summary>
/// 缓存失效处理器 — 监听业务事件，自动清除相关缓存
/// </summary>
public class CacheInvalidationHandler :
    INotificationHandler<UserRoleChangedEvent>,
    INotificationHandler<RolePermissionChangedEvent>,
    INotificationHandler<DictDataChangedEvent>
{
    private readonly ICacheService _cache;
    private readonly ILogger<CacheInvalidationHandler> _logger;

    public CacheInvalidationHandler(ICacheService cache,
        ILogger<CacheInvalidationHandler> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// 用户角色变更 → 清除该用户权限缓存
    /// </summary>
    public async Task Handle(UserRoleChangedEvent notification,
        CancellationToken cancellationToken)
    {
        await _cache.RemoveAsync(CacheKeys.UserPermissions(notification.UserId));
        await _cache.RemoveAsync(CacheKeys.UserRoles(notification.UserId));
        _logger.LogDebug("已清除用户 {UserId} 的权限缓存", notification.UserId);
    }

    /// <summary>
    /// 角色权限变更 → 清除所有用户权限缓存（简单策略，后续可优化）
    /// </summary>
    public async Task Handle(RolePermissionChangedEvent notification,
        CancellationToken cancellationToken)
    {
        await _cache.RemoveByPrefixAsync(CacheKeys.PermPrefix);
        await _cache.RemoveByPrefixAsync(CacheKeys.RolePrefix);
        _logger.LogDebug("角色 {RoleId} 权限变更，已清除所有权限缓存", notification.RoleId);
    }

    /// <summary>
    /// 字典数据变更 → 清除字典缓存
    /// </summary>
    public async Task Handle(DictDataChangedEvent notification,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(notification.DictCode))
        {
            await _cache.RemoveAsync(CacheKeys.DictData(notification.DictCode));
        }
        else
        {
            await _cache.RemoveByPrefixAsync(CacheKeys.DictPrefix);
        }
        _logger.LogDebug("字典缓存已清除");
    }
}

/// <summary>
/// 用户创建事件处理器（示例：可扩展发送通知、写日志等）
/// </summary>
public class UserCreatedHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedHandler> _logger;

    public UserCreatedHandler(ILogger<UserCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("新用户创建: {UserName} (ID: {UserId})",
            notification.UserName, notification.UserId);
        // 后续可扩展：发邮件、写操作日志等
        return Task.CompletedTask;
    }
}
