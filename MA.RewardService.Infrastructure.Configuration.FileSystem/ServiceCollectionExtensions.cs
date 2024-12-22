using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Infrastructure.Configuration.FileSystem.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Infrastructure.Configuration.FileSystem;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFileSystemConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMissionsConfiguration(configuration);

        services.AddSingleton<IMissionsConfigurationProvider, MissionsConfigurationFileSystemProvider>();
        
        return services;
    }
    
    private static IServiceCollection AddMissionsConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var configurationSection = configuration.GetRequiredSection(MissionsConfig.Key);
        services.AddOptions<MissionsConfig>().Bind(configurationSection);

        return services;
    }
}