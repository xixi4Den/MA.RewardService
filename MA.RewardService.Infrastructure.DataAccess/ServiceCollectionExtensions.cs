using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Infrastructure.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MA.RewardService.Infrastructure.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisDataAccessServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect("localhost"));

        services.AddScoped<IMissionProgressRepository, MissionProgressRepository>();
        
        return services;
    }
}