using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IPointsCalculator, PointsCalculator>();
        services.AddSingleton<IMissionProgressProcessor, MissionProgressProcessor>();
        
        return services;
    }
}