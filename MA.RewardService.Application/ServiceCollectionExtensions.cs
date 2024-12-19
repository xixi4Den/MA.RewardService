using System.Reflection;
using MA.RewardService.Application.Configuration;
using MA.RewardService.Application.Services;
using MA.RewardService.Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
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