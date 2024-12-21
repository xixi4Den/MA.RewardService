namespace MA.RewardService.Application.Abstractions;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent e, CancellationToken ct);
}