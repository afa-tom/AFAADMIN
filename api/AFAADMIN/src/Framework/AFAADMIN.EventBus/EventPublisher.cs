using AFAADMIN.Common.DependencyInjection;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AFAADMIN.EventBus;

/// <summary>
/// 基于 MediatR 的事件发布器
/// </summary>
public class EventPublisher : IEventPublisher, IScopedDependency
{
    private readonly IMediator _mediator;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IMediator mediator, ILogger<EventPublisher> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : INotification
    {
        var eventName = typeof(TEvent).Name;
        _logger.LogDebug("发布事件: {EventName}", eventName);

        try
        {
            await _mediator.Publish(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "事件处理异常: {EventName}", eventName);
            throw;
        }
    }
}
