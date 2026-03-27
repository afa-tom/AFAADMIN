using MediatR;

namespace AFAADMIN.EventBus;

/// <summary>
/// 事件发布器接口
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// 发布事件（进程内，所有订阅者依次处理）
    /// </summary>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : INotification;
}
