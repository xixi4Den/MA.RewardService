using MA.RewardService.IntegrationTests.Framework;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MA.RewardService.IntegrationTests.Extensions;

public static class IntegrationTestingWebAppFactoryExtensions
{
    internal static async Task RunServiceBusConsumer<TConsumer, TEvent>(this IntegrationTestingWebAppFactory that, TEvent @event)
        where TConsumer : IConsumer<TEvent>
        where TEvent : class
    {
        var childScope = that.Services.CreateScope();
        var eventConsumer = childScope.ServiceProvider.GetRequiredService<TConsumer>();
        var context = CreateContext(@event);
        
        await eventConsumer.Consume(context);
    }
    
    private static ConsumeContext<T> CreateContext<T>(T message) where T : class
    {
        var contextMock = new Mock<ConsumeContext<T>>();
        contextMock.SetupGet(x => x.Message)
            .Returns(message);
        
        return contextMock.Object;
    }
}