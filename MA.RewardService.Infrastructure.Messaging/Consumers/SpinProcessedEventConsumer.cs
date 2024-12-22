using System.Text.Json;
using MA.RewardService.Application.Feature.HandleMissionProgress;
using MA.SlotService.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MA.RewardService.Infrastructure.Messaging.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
public class SpinProcessedEventConsumer(
    ILogger<SpinProcessedEventConsumer> logger,
    IMediator mediator)
    : IConsumer<SpinProcessedEvent>
{
    public async Task Consume(ConsumeContext<SpinProcessedEvent> context)
    {
        var message = context.Message;
        logger.LogDebug("Received message {MessageId} of type {EventType}: {Message}", context.MessageId, nameof(SpinProcessedEvent), JsonSerializer.Serialize(message));
        
        var command = new HandleMissionProgressCommand(message.UserId, message.SpinId, message.Result);
        await mediator.Send(command);
        
        logger.LogDebug("Message {MessageId} has been processed", context.MessageId);
    }
}