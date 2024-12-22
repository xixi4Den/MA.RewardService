using System.Reflection;
using MA.RewardService.Application.Abstractions;
using MA.RewardService.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddScoped<IRewardsGrantor, RewardsGrantor>();
        
        return services;
    }

    
}