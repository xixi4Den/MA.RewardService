using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Infrastructure.DataAccess.Configuration;
using MA.RewardService.Infrastructure.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MA.RewardService.Infrastructure.DataAccess;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedisDataAccessServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedis(configuration);

        services.AddScoped<IMissionProgressRepository, MissionProgressRepository>();
        services.AddScoped<IGrantedRewardRepository, GrantedRewardRepository>();
        services.AddScoped<ISpinsLogRepository, SpinsLogRepository>();
        
        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConfiguration = AddRedisConfiguration(services, configuration);
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(redisConfiguration.ConnectionString));

        return services;
    }

    private static RedisConfiguration AddRedisConfiguration(IServiceCollection services, IConfiguration configuration)
    {
        var configurationSection = configuration.GetRequiredSection(RedisConfiguration.Key);
        services.AddOptions<RedisConfiguration>().Bind(configurationSection);
        var result = configurationSection.Get<RedisConfiguration>();
        
        return result!;
    }
}