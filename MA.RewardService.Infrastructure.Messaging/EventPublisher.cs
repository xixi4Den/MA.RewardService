using MA.RewardService.Application.Abstractions;
using MassTransit;

namespace MA.RewardService.Infrastructure.Messaging;

public class EventPublisher: IEventPublisher
{
    private readonly IBus _bus;

    public EventPublisher(IBus bus)
    {
        _bus = bus;
    }
    
    public async Task PublishAsync<TEvent>(TEvent e, CancellationToken ct)
    {
        await _bus.Publish(e, ct);
    }
}