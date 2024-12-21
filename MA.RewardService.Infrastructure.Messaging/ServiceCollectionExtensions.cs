using MA.RewardService.Application.Abstractions;
using MA.RewardService.Infrastructure.Messaging.Configuration;
using MA.RewardService.Infrastructure.Messaging.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Infrastructure.Messaging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessagingServices(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = services.AddRabbitConfiguration(configuration);
        
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.AddConsumer<SpinProcessedEventConsumer>();
            
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitConfig.Host, "/", h =>
                {
                    h.Username(rabbitConfig.Username);
                    h.Password(rabbitConfig.Password);
                });
                
                cfg.ConfigureEndpoints(context);
            });
        });
        
        services.AddScoped<IEventPublisher, EventPublisher>();
        
        return services;
    }
    
    private static RabbitConfiguration AddRabbitConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var configurationSection = configuration.GetRequiredSection(RabbitConfiguration.Key);
        services.AddOptions<RabbitConfiguration>().Bind(configurationSection);
        var result = configurationSection.Get<RabbitConfiguration>();
        
        return result!;
    }
}